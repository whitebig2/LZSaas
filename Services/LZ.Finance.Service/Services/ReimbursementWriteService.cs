using System.Security.Cryptography;
using System.Text;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Finance.Service.Interface;
using LZ.Model;
using LZ.Model.Dtos.Reimbursement;
using LZ.Model.Enums;
using LZ.Model.Enums.Reimbusement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SqlSugar;
using SqlSugar.IOC;

namespace LZ.Finance.Service.Services;

public sealed class ReimbursementWriteService : ServicesBase<FIN_Reimb_Apply>, IReimbursementWriteService, IScopedDependency
{
    private readonly ICurrentUser _user;
    private readonly IApprovalService _approval;
    private readonly IReimbursementReadService _read;
    private readonly IServiceProvider _services;
    private readonly ILogger<ReimbursementWriteService> _logger;

    /// <summary>接入当前用户、共用审批服务、申请查询和可选外部能力。</summary>
    /// <param name="user">当前登录用户。</param>
    /// <param name="approval">审批实例启动服务。</param>
    /// <param name="read">发票有效性校验服务。</param>
    /// <param name="services">预算、财务授权和银行卡加密扩展容器。</param>
    /// <param name="logger">预算补偿失败日志。</param>
    public ReimbursementWriteService(ICurrentUser user, IApprovalService approval,
        IReimbursementReadService read, IServiceProvider services,
        ILogger<ReimbursementWriteService> logger)
    {
        _user = user;
        _approval = approval;
        _read = read;
        _services = services;
        _logger = logger;
    }

    private int CompanyId
    {
        get
        {
            if (!_user.IsAuthenticated || _user.CompanyID <= 0 || _user.UserID <= 0 ||
                _user.PlatformType != PlatformType.XFZMini)
                throw new UnauthorizedAccessException("仅当前企业的小程序用户可提交报销申请");
            return _user.CompanyID;
        }
    }

    private ISqlSugarClient Main => DbScoped.SugarScope.GetConnection("Main");

    /// <summary>根据当前申请人部门开关确定审核版或预算版。</summary>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>流程版本和判定来源。</returns>
    public async Task<FlowVersionDto> GetFlowVersionAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var companyId = CompanyId;
        var actor = await Main.Queryable<Live_Personnel>()
            .FirstAsync(x => x.CompanyID == companyId && x.UserID == _user.UserID && x.Enabled);
        if (actor is null) throw new UnauthorizedAccessException("当前企业用户不存在或已停用");
        if (actor.PartnerGID is not Guid deptGuid)
            throw new InvalidOperationException("当前用户未绑定部门，无法判定报销流程");
        var department = await Main.Queryable<Live_Partner>()
            .FirstAsync(x => x.CompanyID == companyId && x.PartnerGID == deptGuid);
        if (department is null) throw new InvalidOperationException("当前部门未配置报销流程");
        return new FlowVersionDto
        {
            FlowType = department.Enabled ? ReimbursementFlowType.Budget : ReimbursementFlowType.Audit,
            Source = "DepartmentBudgetSwitch"
        };
    }

    /// <summary>提交首次报销或付款申请。</summary>
    /// <param name="request">申请表单、类型、上传 GUID 和幂等键。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>申请标识、单号及当前状态。</returns>
    public async Task<ApplySubmitResultDto> SubmitAsync(ApplicationCreateRequest request,
        CancellationToken cancellationToken = default)
        => await SubmitCoreAsync(request, null, cancellationToken);

    /// <summary>统一处理首次提交与已打款复制新单；报销须有足额发票，付款可无票先提交并保留补票状态。</summary>
    /// <param name="request">新申请字段及幂等键。</param>
    /// <param name="copySource">已打款的源单；null 表示首次提交，付款申请可不附发票。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>新申请标识、单号和状态。</returns>
    private async Task<ApplySubmitResultDto> SubmitCoreAsync(ApplicationCreateRequest request,
        FIN_Reimb_Apply? copySource, CancellationToken cancellationToken)
    {
        var companyId = CompanyId;
        ValidateFields(request);
        if (request.ApplyType is not (ReimbursementApplyType.Reimbursement or ReimbursementApplyType.Payment))
            throw new ArgumentException("申请类型无效");
        var operation = copySource is null ? "submit" : $"copy:{copySource.ApplyGuid}";
        var applyGuid = IdempotentGuid(companyId, _user.UserID, operation, request.IdempotencyKey);
        var existing = await FindOwnedAsync(applyGuid, companyId);
        if (existing is not null) return MapResult(existing);

        var flow = await GetFlowVersionAsync(cancellationToken);
        var uploadGuids = request.UploadGuids ?? new List<Guid>();
        var uploads = copySource is null
            ? await LoadUploadsAsync(request.UploadGuids, companyId, null, 1)
            : new List<FIN_Reimb_InvoiceUpload>();
        var copiedRelations = copySource is null ? new List<FIN_Reimb_ApplyInvoice>() :
            await Context.Queryable<FIN_Reimb_ApplyInvoice>()
                .Where(x => x.CompanyId == companyId && x.ApplyGuid == copySource.ApplyGuid && x.IsActive)
                .ToListAsync();
        var copiedInvoiceIds = copiedRelations.Select(x => x.InvoiceGuid).Distinct().ToList();
        var copiedInvoices = copiedInvoiceIds.Count == 0 ? new List<FIN_Reimb_InvoicePool>() :
            await Context.Queryable<FIN_Reimb_InvoicePool>()
                .Where(x => x.CompanyId == companyId && copiedInvoiceIds.Contains(x.InvoiceGuid)).ToListAsync();
        if (copySource is not null && copiedInvoices.Count != copiedInvoiceIds.Count)
            throw new InvalidOperationException("原申请发票记录不完整");
        var copiedAmount = copiedInvoices.Sum(x => x.TotalAmount);
        InvoiceValidationDto valid;
        if (copySource is not null)
        {
            // 已打款再次申请复用原申请的发票快照，不触发 OCR 或重新验真。
            valid = new InvoiceValidationDto
            {
                IsValid = copiedInvoices.Count > 0 && copiedAmount >= request.ApplyAmount,
                ValidInvoiceTotalAmount = copiedAmount
            };
        }
        else if (request.ApplyType == ReimbursementApplyType.Payment && uploadGuids.Count == 0)
        {
            // 付款申请无票可先提交，零票额会生成待补票状态及异常记录。
            valid = new InvoiceValidationDto { IsValid = false, ValidInvoiceTotalAmount = 0 };
        }
        else
        {
            valid = await _read.ValidateInvoicesAsync(new InvoiceValidationRequest
            {
                UploadGuids = uploadGuids,
                ApplyAmount = request.ApplyAmount,
                ApplyType = request.ApplyType,
                FlowType = flow.FlowType
            }, cancellationToken);
        }
        // 报销申请无票或有效票额不足均在写入申请前阻断。
        if (request.ApplyType == ReimbursementApplyType.Reimbursement && !valid.IsValid)
            throw new ArgumentException(valid.Message ?? "发票金额不足，请点击付款申请");

        var now = DateTime.Now;
        var applyNo = (request.ApplyType == ReimbursementApplyType.Reimbursement ? "BX" : "FK") +
                      now.ToString("yyyyMMdd") + RandomNumberGenerator.GetInt32(0, 1000000).ToString("D6");
        // The bank account key is supplied by deployment, not stored in a source or configuration file.
        var encrypted = ProtectCard(request.PayeeBankCardNo);
        var actor = await Main.Queryable<Live_Personnel>()
            .FirstAsync(x => x.CompanyID == companyId && x.UserID == _user.UserID && x.Enabled);
        if (actor is null) throw new UnauthorizedAccessException("当前企业用户不存在或已停用");
        var dept = actor.DeptGID is Guid deptGuid
            ? await Main.Queryable<Live_Partner>().FirstAsync(x => x.CompanyID == companyId && x.PartnerGID == deptGuid)
            : null;

        BudgetDecision? budget = null;
        var budgetProvider = _services.GetService<IReimbursementBudgetProvider>();
        if (flow.FlowType == ReimbursementFlowType.Budget)
        {
            if (budgetProvider is null) throw new InvalidOperationException("预算校验暂不可用，请稍后重试");
            try
            {
                budget = await budgetProvider.CheckAndReserveAsync(companyId, _user.UserID, applyNo,
                    request.CostTypeId, request.ApplyAmount, cancellationToken);
                if (budget.WithinBudget && !budget.Reserved)
                    throw new InvalidOperationException("预算预留未完成");
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                throw new InvalidOperationException("预算校验暂不可用，请稍后重试", ex);
            }
        }

        var apply = new FIN_Reimb_Apply
        {
            ApplyGuid = applyGuid,
            CompanyId = companyId,
            ApplyNo = applyNo,
            ApplyType = (byte)request.ApplyType,
            FlowType = (byte)flow.FlowType,
            ConfigGuid = Guid.Empty,
            ApplicantId = _user.UserID,
            ApplicantName = actor.FullName,
            ApplicantDeptId = dept?.PartnerGID,
            ApplicantDeptName = dept?.PartnerName,
            LegalEntityId = request.LegalEntityId,
            LegalEntityName = request.LegalEntityName.Trim(),
            CostTypeId = request.CostTypeId,
            CostTypeName = request.CostTypeName.Trim(),
            ExpenseName = request.ExpenseName.Trim(),
            PayeeName = request.PayeeName.Trim(),
            PayeeBankName = request.PayeeBankName.Trim(),
            PayeeBankCardNoEncrypted = encrypted,
            PayeeBankCardNoMasked = MaskCard(request.PayeeBankCardNo),
            ExpensePurpose = request.ExpensePurpose.Trim(),
            Remark = request.Remark?.Trim(),
            ApplyAmount = request.ApplyAmount,
            InvoiceCount = copySource is null ? valid.ValidUploadGuids.Count : copiedInvoices.Count,
            InvoiceTotalAmount = valid.ValidInvoiceTotalAmount,
            IsBudgetPassed = budget?.WithinBudget == true,
            ApplyStatus = budget?.WithinBudget == true ? (byte)ReimbursementApplyStatus.PendingPayment :
                (byte)ReimbursementApplyStatus.PendingReview,
            InvoiceSupplementStatus = request.ApplyType == ReimbursementApplyType.Payment && !valid.IsValid
                ? (byte)InvoiceSupplementStatus.PendingInvoice : (byte)InvoiceSupplementStatus.NotApplicable,
            SubmittedAt = now,
            CreatedBy = _user.UserID,
            CreatedAt = now
        };
        var reservationMade = budget?.Reserved == true;
        Context.Ado.BeginTran();
        try
        {
            await Context.Insertable(apply).IgnoreColumns(x => new { x.Id, x.RowVersion }).ExecuteCommandAsync();
            if (copySource is null)
                await BindUploadsAsync(apply, uploads, valid.ValidUploadGuids, 1, now);
            else
                foreach (var relation in copiedRelations)
                    await Context.Insertable(new FIN_Reimb_ApplyInvoice
                    {
                        RelGuid = Guid.NewGuid(),
                        CompanyId = companyId,
                        ApplyGuid = applyGuid,
                        InvoiceGuid = relation.InvoiceGuid,
                        RelationPurpose = 1,
                        IsActive = true,
                        RelatedAt = now
                    }).IgnoreColumns(x => x.Id).ExecuteCommandAsync();
            if (apply.InvoiceSupplementStatus == (byte)InvoiceSupplementStatus.PendingInvoice)
                await Context.Insertable(new FIN_Reimb_PaymentException
                {
                    ExceptionGuid = Guid.NewGuid(),
                    CompanyId = companyId,
                    ApplyGuid = applyGuid,
                    RequiredInvoiceAmount = apply.ApplyAmount,
                    VerifiedInvoiceAmount = apply.InvoiceTotalAmount,
                    ExceptionStatus = (byte)PaymentExceptionStatus.PendingInvoice,
                    CreatedAt = now
                }).IgnoreColumns(x => new { x.Id, x.RowVersion }).ExecuteCommandAsync();
            if (budget is not null)
                await Context.Insertable(new FIN_Reimb_BudgetReservation
                {
                    ReservationGuid = Guid.NewGuid(),
                    CompanyId = companyId,
                    ApplyGuid = applyGuid,
                    CostTypeName = apply.CostTypeName,
                    ExternalBudgetId = budget.ExternalBudgetId,
                    ExternalRequestNo = applyNo,
                    CheckResult = budget.WithinBudget ? (byte)1 : (byte)2,
                    ReservationStatus = budget.Reserved ? (byte)1 : (byte)0,
                    AvailableBefore = budget.AvailableBefore,
                    ReservedAmount = budget.Reserved ? apply.ApplyAmount : 0,
                    ResponseSnapshot = budget.ResponseSnapshot,
                    CreatedAt = now
                }).IgnoreColumns(x => new { x.Id, x.RowVersion }).ExecuteCommandAsync();
            if (apply.ApplyStatus == (byte)ReimbursementApplyStatus.PendingReview)
                await _approval.StartAsync(applyGuid, cancellationToken);
            Context.Ado.CommitTran();
            return MapResult(apply);
        }
        catch
        {
            Context.Ado.RollbackTran();
            if (reservationMade && budgetProvider is not null)
                await ReleaseBudgetAsync(budgetProvider, applyNo);
            // 并发同键请求可能同时通过提交前检查；主键竞争失败后返回先提交的申请。
            var concurrent = await FindOwnedAsync(applyGuid, companyId);
            if (concurrent is not null) return MapResult(concurrent);
            throw;
        }
    }

    /// <summary>修改审核退回的原单，合并旧票与新票并重新执行预算/审批。</summary>
    /// <param name="applyGuid">原申请 GUID。</param>
    /// <param name="request">修改后的字段和可追加的首次上传发票 GUID。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>原申请最新状态。</returns>
    public async Task<ApplySubmitResultDto> ResubmitAsync(Guid applyGuid, ApplicationResubmitRequest request,
        CancellationToken cancellationToken = default)
    {
        var companyId = CompanyId;
        ValidateFields(request);
        var apply = await FindOwnedAsync(applyGuid, companyId);
        if (apply is null) throw new ArgumentException("申请不存在");
        if (apply.ApplyStatus == (byte)ReimbursementApplyStatus.PendingReview &&
            apply.CurrentApprovalInstanceGuid.HasValue)
            return MapResult(apply);
        if (apply.ApplyStatus != (byte)ReimbursementApplyStatus.ReviewReturned)
            throw new InvalidOperationException("仅审核退回的申请可以重提");
        var uploadGuids = request.UploadGuids ?? new List<Guid>();
        var newUploads = await LoadUploadsAsync(request.UploadGuids, companyId, null, 1);
        var oldRelations = await Context.Queryable<FIN_Reimb_ApplyInvoice>()
            .Where(x => x.CompanyId == companyId && x.ApplyGuid == applyGuid && x.IsActive).ToListAsync();
        var oldInvoiceIds = oldRelations.Select(x => x.InvoiceGuid).ToList();
        var oldInvoices = oldRelations.Count == 0 ? new List<FIN_Reimb_InvoicePool>() :
            await Context.Queryable<FIN_Reimb_InvoicePool>()
                .Where(x => x.CompanyId == companyId && oldInvoiceIds.Contains(x.InvoiceGuid))
                .ToListAsync();
        var validation = await _read.ValidateInvoicesAsync(new InvoiceValidationRequest
        {
            UploadGuids = uploadGuids,
            ApplyAmount = request.ApplyAmount,
            ApplyType = (ReimbursementApplyType)apply.ApplyType,
            FlowType = (ReimbursementFlowType)apply.FlowType
        }, cancellationToken);
        var total = oldInvoices.Sum(x => x.TotalAmount) + validation.ValidInvoiceTotalAmount;
        if (apply.ApplyType == 1 && total < request.ApplyAmount)
            throw new ArgumentException("发票金额不足，请点击付款申请，作废");
        var encrypted = ProtectCard(request.PayeeBankCardNo);
        var now = DateTime.Now;
        var remark = request.Remark?.Trim();
        var maskedCard = MaskCard(request.PayeeBankCardNo);
        var nextSupplementStatus = apply.ApplyType == 2 && total < request.ApplyAmount
            ? (byte)InvoiceSupplementStatus.PendingInvoice : (byte)InvoiceSupplementStatus.NotApplicable;
        var existingException = await Context.Queryable<FIN_Reimb_PaymentException>()
            .FirstAsync(x => x.CompanyId == companyId && x.ApplyGuid == applyGuid);
        BudgetDecision? budget = null;
        var budgetProvider = _services.GetService<IReimbursementBudgetProvider>();
        if (apply.FlowType == (byte)ReimbursementFlowType.Budget)
        {
            if (budgetProvider is null) throw new InvalidOperationException("预算校验暂不可用，请稍后重试");
            try
            {
                // 退回后金额或成本类型可能变化，重提前重新校验并预留预算。
                budget = await budgetProvider.CheckAndReserveAsync(companyId, _user.UserID,
                    apply.ApplyNo, request.CostTypeId, request.ApplyAmount, cancellationToken);
                if (budget.WithinBudget && !budget.Reserved)
                    throw new InvalidOperationException("预算预留未完成");
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                throw new InvalidOperationException("预算校验暂不可用，请稍后重试", ex);
            }
        }
        var budgetPassed = budget?.WithinBudget == true;
        var nextApplyStatus = budgetPassed
            ? (byte)ReimbursementApplyStatus.PendingPayment
            : (byte)ReimbursementApplyStatus.ReviewReturned;
        Context.Ado.BeginTran();
        try
        {
            var changed = await Context.Updateable<FIN_Reimb_Apply>()
                .SetColumns(x => new FIN_Reimb_Apply
                {
                    LegalEntityId = request.LegalEntityId,
                    LegalEntityName = request.LegalEntityName.Trim(),
                    CostTypeId = request.CostTypeId,
                    CostTypeName = request.CostTypeName.Trim(),
                    ExpenseName = request.ExpenseName.Trim(),
                    PayeeName = request.PayeeName.Trim(),
                    PayeeBankName = request.PayeeBankName.Trim(),
                    PayeeBankCardNoEncrypted = encrypted,
                    PayeeBankCardNoMasked = maskedCard,
                    ExpensePurpose = request.ExpensePurpose.Trim(),
                    Remark = remark,
                    ApplyAmount = request.ApplyAmount,
                    InvoiceTotalAmount = total,
                    InvoiceCount = oldInvoices.Count + validation.ValidUploadGuids.Count,
                    InvoiceSupplementStatus = nextSupplementStatus,
                    IsBudgetPassed = budgetPassed,
                    ApplyStatus = nextApplyStatus,
                    UpdatedBy = _user.UserID,
                    UpdatedAt = now
                })
                .Where(x => x.CompanyId == companyId && x.ApplicantId == _user.UserID &&
                            x.ApplyGuid == applyGuid && x.ApplyStatus == (byte)ReimbursementApplyStatus.ReviewReturned &&
                            x.RowVersion == apply.RowVersion).ExecuteCommandAsync();
            if (changed != 1) throw new InvalidOperationException("申请状态已变化，请刷新后重试");
            await BindUploadsAsync(apply, newUploads, validation.ValidUploadGuids, 1, now);
            if (apply.ApplyType == 2)
            {
                if (existingException is null && nextSupplementStatus == (byte)InvoiceSupplementStatus.PendingInvoice)
                    await Context.Insertable(new FIN_Reimb_PaymentException
                    {
                        ExceptionGuid = Guid.NewGuid(),
                        CompanyId = companyId,
                        ApplyGuid = applyGuid,
                        RequiredInvoiceAmount = request.ApplyAmount,
                        VerifiedInvoiceAmount = total,
                        ExceptionStatus = 1,
                        CreatedAt = now
                    }).IgnoreColumns(x => new { x.Id, x.RowVersion }).ExecuteCommandAsync();
                else if (existingException is not null)
                {
                    var exceptionStatus = nextSupplementStatus == (byte)InvoiceSupplementStatus.PendingInvoice ? (byte)1 : (byte)2;
                    var closedAt = exceptionStatus == 2 ? now : (DateTime?)null;
                    await Context.Updateable<FIN_Reimb_PaymentException>()
                        .SetColumns(x => new FIN_Reimb_PaymentException
                        {
                            RequiredInvoiceAmount = request.ApplyAmount,
                            VerifiedInvoiceAmount = total,
                            ExceptionStatus = exceptionStatus,
                            LastEvaluatedAt = now,
                            ClosedAt = closedAt
                        })
                        .Where(x => x.CompanyId == companyId && x.ApplyGuid == applyGuid)
                        .ExecuteCommandAsync();
                }
            }
            if (budget is not null)
                await Context.Insertable(new FIN_Reimb_BudgetReservation
                {
                    ReservationGuid = Guid.NewGuid(),
                    CompanyId = companyId,
                    ApplyGuid = applyGuid,
                    CostTypeName = request.CostTypeName.Trim(),
                    ExternalBudgetId = budget.ExternalBudgetId,
                    ExternalRequestNo = apply.ApplyNo,
                    CheckResult = budget.WithinBudget ? (byte)1 : (byte)2,
                    ReservationStatus = budget.Reserved ? (byte)1 : (byte)0,
                    AvailableBefore = budget.AvailableBefore,
                    ReservedAmount = budget.Reserved ? request.ApplyAmount : 0,
                    ResponseSnapshot = budget.ResponseSnapshot,
                    CreatedAt = now
                }).IgnoreColumns(x => new { x.Id, x.RowVersion }).ExecuteCommandAsync();
            if (budget?.WithinBudget != true)
                await _approval.StartAsync(applyGuid, cancellationToken);
            Context.Ado.CommitTran();
            apply.ApplyStatus = budget?.WithinBudget == true
                ? (byte)ReimbursementApplyStatus.PendingPayment
                : (byte)ReimbursementApplyStatus.PendingReview;
            apply.InvoiceSupplementStatus = nextSupplementStatus;
            return MapResult(apply);
        }
        catch
        {
            Context.Ado.RollbackTran();
            if (budget?.Reserved == true && budgetProvider is not null)
                await ReleaseBudgetAsync(budgetProvider, apply.ApplyNo);
            throw;
        }
    }

    /// <summary>再次申请：退回单重提原单，已打款单在金额不变时沿用旧票创建新单。</summary>
    /// <param name="request">源申请 GUID、新表单字段和幂等键。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>重提原单或复制新单的标识及状态。</returns>
    public async Task<ApplySubmitResultDto> CopyAsync(ApplicationCopyRequest request,
        CancellationToken cancellationToken = default)
    {
        var companyId = CompanyId;
        ValidateFields(request);
        var source = await FindOwnedAsync(request.SourceApplyGuid, companyId);
        if (source is null) throw new ArgumentException("原申请不存在");
        if (source.ApplyStatus == (byte)ReimbursementApplyStatus.ReviewReturned)
            return await ResubmitAsync(source.ApplyGuid, ToResubmit(request), cancellationToken);
        if (source.ApplyStatus != (byte)ReimbursementApplyStatus.Paid)
            throw new InvalidOperationException("当前申请状态不允许再次申请");
        if (source.ApplyAmount != request.ApplyAmount)
            throw new ArgumentException("沿用原发票时申请金额不得修改");
        // 用户确认已打款申请可以沿用原票新建；普通首次提交仍执行发票占用检查。
        return await SubmitCoreAsync(new ApplicationCreateRequest
        {
            ApplyType = (ReimbursementApplyType)source.ApplyType,
            LegalEntityId = request.LegalEntityId,
            LegalEntityName = request.LegalEntityName,
            CostTypeId = request.CostTypeId,
            CostTypeName = request.CostTypeName,
            ExpenseName = request.ExpenseName,
            PayeeName = request.PayeeName,
            PayeeBankName = request.PayeeBankName,
            PayeeBankCardNo = request.PayeeBankCardNo,
            ExpensePurpose = request.ExpensePurpose,
            Remark = request.Remark,
            ApplyAmount = request.ApplyAmount,
            IdempotencyKey = request.IdempotencyKey
        }, source, cancellationToken);
    }

    /// <summary>向待补票付款单关联已验真的补传发票并更新票额与异常状态。</summary>
    /// <param name="applyGuid">付款申请 GUID。</param>
    /// <param name="request">补传上传 GUID 和幂等键。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>累计有效票额和是否补齐。</returns>
    public async Task<InvoiceValidationDto> SupplementAsync(Guid applyGuid, SupplementInvoiceRequest request,
        CancellationToken cancellationToken = default)
    {
        var companyId = CompanyId;
        if (request is null || string.IsNullOrWhiteSpace(request.IdempotencyKey) || request.UploadGuids is null ||
            request.UploadGuids.Count == 0)
            throw new ArgumentException("补传发票及幂等键必填");
        var apply = await FindOwnedAsync(applyGuid, companyId);
        if (apply is null || apply.ApplyType != 2)
            throw new InvalidOperationException("申请不处于待补传发票状态");
        var uploads = await LoadUploadsAsync(request.UploadGuids, companyId, applyGuid, 2);
        var validation = await _read.ValidateInvoicesAsync(new InvoiceValidationRequest
        {
            UploadGuids = request.UploadGuids,
            ApplyAmount = apply.ApplyAmount,
            ApplyType = ReimbursementApplyType.Payment,
            FlowType = (ReimbursementFlowType)apply.FlowType
        }, cancellationToken);
        var existingRelations = await Context.Queryable<FIN_Reimb_ApplyInvoice>()
            .Where(x => x.CompanyId == companyId && x.ApplyGuid == applyGuid && x.IsActive).ToListAsync();
        var relatedInvoiceIds = existingRelations.Select(x => x.InvoiceGuid).ToList();
        var relatedUploads = relatedInvoiceIds.Count == 0 ? new List<FIN_Reimb_InvoicePool>() :
            await Context.Queryable<FIN_Reimb_InvoicePool>()
                .Where(x => x.CompanyId == companyId && relatedInvoiceIds.Contains(x.InvoiceGuid)).ToListAsync();
        var relatedUploadIds = relatedUploads.Select(x => x.SourceUploadGuid).ToHashSet();
        if (validation.ValidUploadGuids.Count > 0 &&
            validation.ValidUploadGuids.All(relatedUploadIds.Contains))
        {
            validation.ValidInvoiceTotalAmount = apply.InvoiceTotalAmount;
            validation.IsValid = apply.InvoiceTotalAmount >= apply.ApplyAmount;
            return validation;
        }
        if (apply.InvoiceSupplementStatus != 1)
            throw new InvalidOperationException("申请不处于待补传发票状态");
        if (validation.ValidUploadGuids.Any(relatedUploadIds.Contains))
            throw new InvalidOperationException("本次补传包含已关联发票");
        var exception = await Context.Queryable<FIN_Reimb_PaymentException>()
            .FirstAsync(x => x.CompanyId == companyId && x.ApplyGuid == applyGuid && x.ExceptionStatus == 1);
        if (exception is null) throw new InvalidOperationException("待补传异常记录不存在");
        var total = apply.InvoiceTotalAmount + validation.ValidInvoiceTotalAmount;
        var now = DateTime.Now;
        Context.Ado.BeginTran();
        try
        {
            await BindUploadsAsync(apply, uploads, validation.ValidUploadGuids, 2, now);
            var changed = await Context.Updateable<FIN_Reimb_Apply>()
                .SetColumns(x => new FIN_Reimb_Apply
                {
                    InvoiceTotalAmount = total,
                    InvoiceCount = apply.InvoiceCount + validation.ValidUploadGuids.Count,
                    InvoiceSupplementStatus = total >= apply.ApplyAmount ? (byte)2 : (byte)1,
                    UpdatedAt = now,
                    UpdatedBy = _user.UserID
                })
                .Where(x => x.CompanyId == companyId && x.ApplyGuid == applyGuid &&
                            x.InvoiceSupplementStatus == 1 && x.RowVersion == apply.RowVersion).ExecuteCommandAsync();
            if (changed != 1) throw new InvalidOperationException("申请状态已变化，请刷新后重试");
            changed = await Context.Updateable<FIN_Reimb_PaymentException>()
                .SetColumns(x => new FIN_Reimb_PaymentException
                {
                    VerifiedInvoiceAmount = total,
                    ExceptionStatus = total >= apply.ApplyAmount ? (byte)2 : (byte)1,
                    SupplementCount = exception.SupplementCount + 1,
                    LastSupplementTime = now,
                    LastEvaluatedAt = now,
                    ClosedAt = total >= apply.ApplyAmount ? now : null
                })
                .Where(x => x.CompanyId == companyId && x.ApplyGuid == applyGuid &&
                            x.ExceptionStatus == 1 && x.RowVersion == exception.RowVersion).ExecuteCommandAsync();
            if (changed != 1) throw new InvalidOperationException("异常状态已变化，请刷新后重试");
            Context.Ado.CommitTran();
            validation.ValidInvoiceTotalAmount = total;
            validation.IsValid = total >= apply.ApplyAmount;
            return validation;
        }
        catch { Context.Ado.RollbackTran(); throw; }
    }

    /// <summary>经财务权限校验后记录已完成打款，并将申请转为已打款。</summary>
    /// <param name="applyGuid">待打款申请 GUID。</param>
    /// <param name="request">银行流水号、回单地址和幂等键。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>确认生成的打款单。</returns>
    public async Task<PaymentOrderDto> CompletePaymentAsync(Guid applyGuid, CompletePaymentRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!_user.IsAuthenticated || _user.PlatformType != PlatformType.MiniAdmin ||
            _user.CompanyID <= 0 || _user.UserID <= 0)
            throw new UnauthorizedAccessException("仅分后台财务人员可确认打款");
        if (request is null || string.IsNullOrWhiteSpace(request.IdempotencyKey) ||
            string.IsNullOrWhiteSpace(request.BankSerialNo))
            throw new ArgumentException("幂等键和银行流水号必填");
        var companyId = _user.CompanyID;
        var authorizer = _services.GetService<IReimbursementPaymentAuthorizer>();
        if (authorizer is null || !await authorizer.CanCompleteAsync(companyId, _user.UserID, cancellationToken))
            throw new UnauthorizedAccessException("当前用户没有财务打款权限");
        var apply = await Context.Queryable<FIN_Reimb_Apply>()
            .FirstAsync(x => x.CompanyId == companyId && x.ApplyGuid == applyGuid);
        if (apply is null) throw new ArgumentException("申请不存在");
        var existing = await Context.Queryable<FIN_Reimb_PaymentOrder>()
            .FirstAsync(x => x.CompanyId == companyId && x.ApplyGuid == applyGuid);
        if (existing is not null)
        {
            if (existing.IdempotencyKey != request.IdempotencyKey || existing.PayStatus != 3)
                throw new InvalidOperationException("申请已有打款单，请核对打款结果");
            return MapPayment(existing);
        }
        if (apply.ApplyStatus != (byte)ReimbursementApplyStatus.PendingPayment)
            throw new InvalidOperationException("申请不处于待打款状态");
        var reservation = await Context.Queryable<FIN_Reimb_BudgetReservation>()
            .FirstAsync(x => x.CompanyId == companyId && x.ApplyGuid == applyGuid && x.ReservationStatus == 1);
        var budgetProvider = _services.GetService<IReimbursementBudgetProvider>();
        if (reservation is not null && budgetProvider is null)
            throw new InvalidOperationException("预算确认服务暂不可用");
        var now = DateTime.Now;
        var order = new FIN_Reimb_PaymentOrder
        {
            OrderGuid = Guid.NewGuid(),
            CompanyId = companyId,
            ApplyGuid = applyGuid,
            PaymentNo = "DK" + now.ToString("yyyyMMdd") + RandomNumberGenerator.GetInt32(0, 1000000).ToString("D6"),
            PayAmount = apply.ApplyAmount,
            PaymentType = 1,
            PayStatus = 3,
            PayeeName = apply.PayeeName,
            PayeeBankName = apply.PayeeBankName,
            PayeeBankCardNoMasked = apply.PayeeBankCardNoMasked,
            PaidBy = _user.UserID,
            BankSerialNo = request.BankSerialNo.Trim(),
            ReceiptUrl = request.ReceiptUrl?.Trim(),
            IdempotencyKey = request.IdempotencyKey.Trim(),
            PaidAt = now,
            CreatedAt = now
        };
        // Confirm external reservation before changing local state; provider calls must be idempotent.
        if (reservation is not null) await budgetProvider!.CommitAsync(reservation.ExternalRequestNo, cancellationToken);
        Context.Ado.BeginTran();
        try
        {
            await Context.Insertable(order).IgnoreColumns(x => new { x.Id, x.RowVersion }).ExecuteCommandAsync();
            var changed = await Context.Updateable<FIN_Reimb_Apply>()
                .SetColumns(x => x.ApplyStatus == (byte)ReimbursementApplyStatus.Paid)
                .Where(x => x.CompanyId == companyId && x.ApplyGuid == applyGuid &&
                            x.ApplyStatus == (byte)ReimbursementApplyStatus.PendingPayment &&
                            x.RowVersion == apply.RowVersion).ExecuteCommandAsync();
            if (changed != 1) throw new InvalidOperationException("申请状态已变化，请刷新后重试");
            if (reservation is not null)
                await Context.Updateable<FIN_Reimb_BudgetReservation>()
                    .SetColumns(x => x.ReservationStatus == 2)
                    .Where(x => x.CompanyId == companyId && x.ApplyGuid == applyGuid && x.ReservationStatus == 1)
                    .ExecuteCommandAsync();
            Context.Ado.CommitTran();
            return MapPayment(order);
        }
        catch { Context.Ado.RollbackTran(); throw; }
    }

    /// <summary>按企业及当前申请人查找原申请。</summary>
    /// <param name="applyGuid">申请 GUID。</param>
    /// <param name="companyId">当前企业 ID。</param>
    /// <returns>本人申请实体；无数据时为 null。</returns>
    private async Task<FIN_Reimb_Apply?> FindOwnedAsync(Guid applyGuid, int companyId) =>
        await Context.Queryable<FIN_Reimb_Apply>()
            .FirstAsync(x => x.CompanyId == companyId && x.ApplicantId == _user.UserID && x.ApplyGuid == applyGuid);

    /// <summary>核验已提供的上传记录；首次提交允许无票，补传必须提供发票。</summary>
    /// <param name="ids">上传 GUID；首次提交时 null 或空列表表示尚未上传发票。</param>
    /// <param name="companyId">当前企业 ID。</param>
    /// <param name="applyGuid">补传时的目标申请 GUID；首次上传为 null。</param>
    /// <param name="purpose">首次上传或补传用途值。</param>
    /// <returns>通过归属校验的上传记录；空列表表示本次未上传发票。</returns>
    private async Task<List<FIN_Reimb_InvoiceUpload>> LoadUploadsAsync(List<Guid>? ids, int companyId,
        Guid? applyGuid, byte purpose)
    {
        if (ids is null || ids.Count == 0)
        {
            if (purpose == 2) throw new ArgumentException("补传发票不能为空");
            return new List<FIN_Reimb_InvoiceUpload>();
        }
        if (ids.Any(x => x == Guid.Empty) || ids.Count != ids.Distinct().Count())
            throw new ArgumentException("发票上传记录无效");
        var uploads = await Context.Queryable<FIN_Reimb_InvoiceUpload>()
            .Where(x => x.CompanyId == companyId && x.UploadUserId == _user.UserID && ids.Contains(x.UploadGuid))
            .ToListAsync();
        if (uploads.Count != ids.Count || uploads.Any(x => x.UploadPurpose != purpose || x.ApplyGuid != applyGuid))
            throw new UnauthorizedAccessException("发票不属于当前用户或上传用途不匹配");
        return uploads;
    }

    /// <summary>在现有事务中锁定发票号，写入发票池和申请关联并回填上传记录。</summary>
    /// <param name="apply">目标申请。</param>
    /// <param name="uploads">本次上传的发票记录。</param>
    /// <param name="validIds">经识别、验真及查重后有效的上传 GUID。</param>
    /// <param name="purpose">首次关联或补传关联用途值。</param>
    /// <param name="now">本次操作时间。</param>
    private async Task BindUploadsAsync(FIN_Reimb_Apply apply, List<FIN_Reimb_InvoiceUpload> uploads,
        List<Guid> validIds, byte purpose, DateTime now)
    {
        var valid = validIds.ToHashSet();
        // 同租户同号码串行入池；第 4 章 DDL 未创建唯一索引，需要在 Finance 连接上加事务级锁。
        foreach (var upload in uploads.Where(x => valid.Contains(x.UploadGuid))
                     .OrderBy(x => x.InvoiceNumber, StringComparer.Ordinal))
        {
            var resource = "reimb-invoice:" + Convert.ToHexString(SHA256.HashData(
                Encoding.UTF8.GetBytes($"{apply.CompanyId}:{upload.InvoiceNumber}")));
            await Context.Ado.ExecuteCommandAsync(
                "DECLARE @result int; EXEC @result = sp_getapplock @Resource = @resource, " +
                "@LockMode = 'Exclusive', @LockOwner = 'Transaction'; " +
                "IF @result < 0 THROW 51000, 'Invoice lock failed', 1;",
                new SugarParameter("@resource", resource));
            var existing = await Context.Queryable<FIN_Reimb_InvoicePool>()
                .FirstAsync(x => x.CompanyId == apply.CompanyId && x.InvoiceNumber == upload.InvoiceNumber);
            if (existing is not null && existing.SourceUploadGuid != upload.UploadGuid)
                throw new InvalidOperationException("再次上传即可");
            var pool = existing ?? new FIN_Reimb_InvoicePool
            {
                InvoiceGuid = Guid.NewGuid(),
                CompanyId = apply.CompanyId,
                InvoiceNumber = upload.InvoiceNumber,
                TotalAmount = upload.TotalAmount!.Value,
                SourceUploadGuid = upload.UploadGuid,
                VerifiedAt = now,
                CreatedAt = now
            };
            if (existing is null) await Context.Insertable(pool).IgnoreColumns(x => x.Id).ExecuteCommandAsync();
            var relation = await Context.Queryable<FIN_Reimb_ApplyInvoice>()
                .FirstAsync(x => x.CompanyId == apply.CompanyId && x.InvoiceGuid == pool.InvoiceGuid && x.IsActive);
            if (relation is not null) throw new InvalidOperationException("发票已经关联申请，不能重复提交");
            await Context.Insertable(new FIN_Reimb_ApplyInvoice
            {
                RelGuid = Guid.NewGuid(),
                CompanyId = apply.CompanyId,
                ApplyGuid = apply.ApplyGuid,
                InvoiceGuid = pool.InvoiceGuid,
                RelationPurpose = purpose,
                IsActive = true,
                RelatedAt = now
            }).IgnoreColumns(x => x.Id).ExecuteCommandAsync();
            await Context.Updateable<FIN_Reimb_InvoiceUpload>()
                .SetColumns(x => x.ApplyGuid == apply.ApplyGuid)
                .Where(x => x.CompanyId == apply.CompanyId && x.UploadGuid == upload.UploadGuid &&
                            x.UploadUserId == _user.UserID).ExecuteCommandAsync();
        }
    }

    /// <summary>统一校验申请表单必填项、长度、金额精度和幂等键。</summary>
    /// <param name="request">待校验申请字段。</param>
    private static void ValidateFields(ApplicationFieldsRequest request)
    {
        if (request is null || request.LegalEntityId <= 0 || request.CostTypeId <= 0 ||
            string.IsNullOrWhiteSpace(request.LegalEntityName) || request.LegalEntityName.Length > 200 ||
            string.IsNullOrWhiteSpace(request.CostTypeName) || request.CostTypeName.Length > 100 ||
            string.IsNullOrWhiteSpace(request.ExpenseName) || request.ExpenseName.Length > 200 ||
            string.IsNullOrWhiteSpace(request.PayeeName) || request.PayeeName.Length > 200 ||
            string.IsNullOrWhiteSpace(request.PayeeBankName) || request.PayeeBankName.Length > 200 ||
            string.IsNullOrWhiteSpace(request.PayeeBankCardNo) || request.PayeeBankCardNo.Length > 32 ||
            string.IsNullOrWhiteSpace(request.ExpensePurpose) || request.ExpensePurpose.Length > 500 ||
            request.Remark?.Length > 1000 || request.ApplyAmount <= 0 ||
            decimal.Round(request.ApplyAmount, 2) != request.ApplyAmount ||
            string.IsNullOrWhiteSpace(request.IdempotencyKey) || request.IdempotencyKey.Length > 100)
            throw new ArgumentException("申请字段不完整或格式无效");
    }

    /// <summary>优先使用银行卡加密扩展，否则使用环境密钥加密。</summary>
    /// <param name="card">银行卡原号。</param>
    /// <returns>用于数据库密文列的字节。</returns>
    private byte[] ProtectCard(string card) =>
        _services.GetService<IReimbursementCardProtector>()?.Encrypt(card) ?? EncryptCard(card);

    /// <summary>使用环境变量中的 AES 密钥加密银行卡号。</summary>
    /// <param name="card">银行卡原号。</param>
    /// <returns>随机 nonce、认证 tag 与密文拼接后的字节。</returns>
    private static byte[] EncryptCard(string card)
    {
        var keyText = Environment.GetEnvironmentVariable("LZ_REIMBURSEMENT_CARD_KEY");
        if (string.IsNullOrWhiteSpace(keyText))
            throw new InvalidOperationException("银行卡加密密钥未配置");
        byte[] key;
        try { key = Convert.FromBase64String(keyText); }
        catch (FormatException) { throw new InvalidOperationException("银行卡加密密钥格式无效"); }
        if (key.Length != 32) throw new InvalidOperationException("银行卡加密密钥长度无效");
        var nonce = RandomNumberGenerator.GetBytes(12);
        var cipher = new byte[Encoding.UTF8.GetByteCount(card)];
        var tag = new byte[16];
        using var aes = new AesGcm(key, 16);
        aes.Encrypt(nonce, Encoding.UTF8.GetBytes(card), cipher, tag);
        return nonce.Concat(tag).Concat(cipher).ToArray();
    }

    /// <summary>将银行卡号脱敏用于详情及打款单展示。</summary>
    /// <param name="card">银行卡原号。</param>
    /// <returns>仅保留前四位和后四位的卡号。</returns>
    private static string MaskCard(string card) => card.Length <= 8
        ? new string('*', card.Length)
        : card[..4] + new string('*', card.Length - 8) + card[^4..];

    /// <summary>提交事务失败时补偿释放外部预算预留，并记录补偿失败。</summary>
    /// <param name="provider">预算扩展服务。</param>
    /// <param name="externalRequestNo">预留时的外部请求号。</param>
    private async Task ReleaseBudgetAsync(IReimbursementBudgetProvider provider, string externalRequestNo)
    {
        try { await provider.ReleaseAsync(externalRequestNo, CancellationToken.None); }
        catch (Exception ex)
        {
            // 保留原始数据库错误，并单独告警补偿失败，供运维按外部请求号核对。
            _logger.LogCritical(ex, "报销预算预留释放失败，外部请求号 {ExternalRequestNo}", externalRequestNo);
        }
    }

    /// <summary>由企业、用户、操作和幂等键生成稳定的申请 GUID。</summary>
    /// <param name="companyId">企业 ID。</param>
    /// <param name="userId">申请人 ID。</param>
    /// <param name="operation">提交或复制操作标识。</param>
    /// <param name="key">客户端幂等键。</param>
    /// <returns>稳定的申请 GUID。</returns>
    private static Guid IdempotentGuid(int companyId, int userId, string operation, string key)
    {
        if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("幂等键必填");
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes($"{companyId}:{userId}:{operation}:{key}"));
        return new Guid(bytes.AsSpan(0, 16));
    }

    /// <summary>提取提交或重提接口的申请结果。</summary>
    /// <param name="apply">申请实体。</param>
    /// <returns>申请 GUID、单号、状态及缺票标识。</returns>
    private static ApplySubmitResultDto MapResult(FIN_Reimb_Apply apply) => new()
    {
        ApplyGuid = apply.ApplyGuid,
        ApplyNo = apply.ApplyNo,
        ApplyStatus = (ReimbursementApplyStatus)apply.ApplyStatus,
        FlowType = (ReimbursementFlowType)apply.FlowType,
        ExceptionCreated = apply.InvoiceSupplementStatus == (byte)InvoiceSupplementStatus.PendingInvoice
    };

    /// <summary>提取可返回给调用方的打款单信息。</summary>
    /// <param name="order">打款单实体。</param>
    /// <returns>打款单 DTO。</returns>
    private static PaymentOrderDto MapPayment(FIN_Reimb_PaymentOrder order) => new()
    {
        PaymentNo = order.PaymentNo,
        PayAmount = order.PayAmount,
        PayStatus = (PaymentOrderStatus)order.PayStatus,
        BankSerialNo = order.BankSerialNo,
        ReceiptUrl = order.ReceiptUrl,
        PaidAt = order.PaidAt,
        FailureReason = order.FailureReason
    };

    /// <summary>将再次申请请求转换为审核退回原单的重提请求。</summary>
    /// <param name="source">含源单和新表单字段的再次申请请求。</param>
    /// <returns>原单重提请求。</returns>
    private static ApplicationResubmitRequest ToResubmit(ApplicationCopyRequest source) => new()
    {
        LegalEntityId = source.LegalEntityId,
        LegalEntityName = source.LegalEntityName,
        CostTypeId = source.CostTypeId,
        CostTypeName = source.CostTypeName,
        ExpenseName = source.ExpenseName,
        PayeeName = source.PayeeName,
        PayeeBankName = source.PayeeBankName,
        PayeeBankCardNo = source.PayeeBankCardNo,
        ExpensePurpose = source.ExpensePurpose,
        Remark = source.Remark,
        ApplyAmount = source.ApplyAmount,
        IdempotencyKey = source.IdempotencyKey
    };
}
