using LZ.Core;
using LZ.Core.Interface;
using LZ.Finance.Service.Interface;
using LZ.Model;
using LZ.Model.Dtos.Reimbursement;
using LZ.Model.Dtos.Reimbusement;
using LZ.Model.Enums;
using LZ.Model.Enums.Reimbusement;
using SqlSugar;
using SqlSugar.IOC;

namespace LZ.Finance.Service.Services;

public sealed class ReimbursementReadService : ServicesBase<FIN_Reimb_Apply>, IReimbursementReadService, IScopedDependency
{
    private readonly ICurrentUser _user;

    /// <summary>接入当前用户以限定本人和企业数据范围。</summary>
    /// <param name="user">当前登录用户。</param>
    public ReimbursementReadService(ICurrentUser user) => _user = user;
    private ISqlSugarClient Main => DbScoped.SugarScope.GetConnection("Main");

    private int CompanyId
    {
        get
        {
            if (!_user.IsAuthenticated || _user.CompanyID <= 0 || _user.UserID <= 0 ||
                _user.PlatformType != PlatformType.XFZMini)
                throw new UnauthorizedAccessException("仅当前企业的小程序用户可访问本人报销记录");
            return _user.CompanyID;
        }
    }

    /// <summary>分页查询当前小程序用户的报销和付款申请。</summary>
    /// <param name="query">状态、费用名称、时间、关键字和分页条件。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>本人申请分页及各单可用操作。</returns>
    public async Task<ApplicationPageDto<ApplicationListItemDto>> GetApplicationsAsync(
        ApplicationQuery query, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var companyId = CompanyId;
        if (query is null || query.PageIndex < 1 || query.PageSize is < 1 or > 100 ||
            query.ApplyTimeStart > query.ApplyTimeEnd)
            throw new ArgumentException("分页或申请时间范围无效");

        // 本人列表始终带租户和申请人条件，查询参数只能进一步缩小范围。
        var status = query.ApplyStatus.HasValue ? (byte)query.ApplyStatus.Value : (byte?)null;
        var supplementStatus = query.InvoiceSupplementStatus.HasValue
            ? (byte)query.InvoiceSupplementStatus.Value : (byte?)null;
        var keyword = query.Keyword?.Trim();
        var expenseName = query.ExpenseName?.Trim();
        var count = new RefAsync<int>();
        var applies = await Context.Queryable<FIN_Reimb_Apply>()
            .Where(x => x.CompanyId == companyId && x.ApplicantId == _user.UserID)
            .WhereIF(status.HasValue, x => x.ApplyStatus == status)
            .WhereIF(supplementStatus.HasValue, x => x.InvoiceSupplementStatus == supplementStatus)
            .WhereIF(!string.IsNullOrEmpty(expenseName), x => x.ExpenseName.Contains(expenseName!))
            .WhereIF(!string.IsNullOrEmpty(keyword), x => x.ApplyNo.Contains(keyword!) || x.ExpenseName.Contains(keyword!))
            .WhereIF(query.ApplyTimeStart.HasValue, x => x.SubmittedAt >= query.ApplyTimeStart)
            .WhereIF(query.ApplyTimeEnd.HasValue, x => x.SubmittedAt <= query.ApplyTimeEnd)
            .OrderBy(x => x.SubmittedAt, OrderByType.Desc)
            .OrderBy(x => x.Id, OrderByType.Desc)
            .ToPageListAsync(query.PageIndex, query.PageSize, count);
        return new ApplicationPageDto<ApplicationListItemDto>
        {
            PageIndex = query.PageIndex,
            PageSize = query.PageSize,
            TotalCount = count.Value,
            Items = applies.Select(MapListItem).ToList()
        };
    }

    /// <summary>组合本人申请、发票、审批轨迹、补票异常和打款详情。</summary>
    /// <param name="applyGuid">申请 GUID。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>申请详情；不存在时为 null。</returns>
    public async Task<ApplicationDetailDto?> GetApplicationAsync(Guid applyGuid,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var companyId = CompanyId;
        var apply = await FindOwnedApplicationAsync(applyGuid, companyId);
        if (apply is null) return null;

        var detail = new ApplicationDetailDto
        {
            FlowType = (ReimbursementFlowType)apply.FlowType,
            LegalEntityId = apply.LegalEntityId,
            LegalEntityName = apply.LegalEntityName,
            CostTypeId = apply.CostTypeId,
            CostTypeName = apply.CostTypeName,
            PayeeName = apply.PayeeName,
            PayeeBankName = apply.PayeeBankName,
            PayeeBankCardNoMasked = apply.PayeeBankCardNoMasked,
            ExpensePurpose = apply.ExpensePurpose,
            Remark = apply.Remark,
            LastRejectReason = apply.LastRejectReason
        };
        CopyListFields(MapListItem(apply), detail);

        var relations = await Context.Queryable<FIN_Reimb_ApplyInvoice>()
            .Where(x => x.CompanyId == companyId && x.ApplyGuid == applyGuid && x.IsActive).ToListAsync();
        var invoiceGuids = relations.Select(x => x.InvoiceGuid).Distinct().ToList();
        var invoices = invoiceGuids.Count == 0 ? new List<FIN_Reimb_InvoicePool>() :
            await Context.Queryable<FIN_Reimb_InvoicePool>()
                .Where(x => x.CompanyId == companyId && invoiceGuids.Contains(x.InvoiceGuid)).ToListAsync();
        var sourceUploadGuids = invoices.Select(x => x.SourceUploadGuid).Distinct().ToList();
        // 再次申请沿用原发票池记录，原上传记录仍归属于首次申请。
        var uploads = sourceUploadGuids.Count == 0 ? new List<FIN_Reimb_InvoiceUpload>() :
            await Context.Queryable<FIN_Reimb_InvoiceUpload>()
                .Where(x => x.CompanyId == companyId && sourceUploadGuids.Contains(x.UploadGuid)).ToListAsync();
        var invoiceByGuid = invoices.ToDictionary(x => x.InvoiceGuid);
        var uploadByGuid = uploads.ToDictionary(x => x.UploadGuid);
        detail.Invoices = relations.Where(x => invoiceByGuid.ContainsKey(x.InvoiceGuid))
            .Select(x =>
            {
                var invoice = invoiceByGuid[x.InvoiceGuid];
                uploadByGuid.TryGetValue(invoice.SourceUploadGuid, out var upload);
                return new ApplicationInvoiceDto
                {
                    InvoiceGuid = invoice.InvoiceGuid,
                    InvoiceNumber = invoice.InvoiceNumber,
                    TotalAmount = invoice.TotalAmount,
                    RelationPurpose = x.RelationPurpose,
                    RelatedAt = x.RelatedAt,
                    FileUrl = upload?.FileUrl,
                    FileName = upload?.OriginalFileName
                };
            }).ToList();

        var instances = await Context.Queryable<FIN_Reimb_ApprovalInstance>()
            .Where(x => x.CompanyId == companyId && x.ApplyGuid == applyGuid).ToListAsync();
        var roundByGuid = instances.ToDictionary(x => x.InstanceGuid, x => x.RoundNo);
        var records = await Context.Queryable<FIN_Reimb_ApprovalRecord>()
            .Where(x => x.CompanyId == companyId && x.ApplyGuid == applyGuid)
            .OrderBy(x => x.OperatedAt).ToListAsync();
        detail.ApprovalHistory = records.Select(x => new ApprovalHistoryDto
        {
            RoundNo = roundByGuid.GetValueOrDefault(x.InstanceGuid),
            NodeSeq = x.NodeSeq,
            NodeName = x.NodeName,
            OperatorName = x.OperatorName,
            ActionType = (ApprovalActionType)x.ActionType,
            RejectReason = x.RejectReason,
            Comment = x.Comment,
            OperatedAt = x.OperatedAt
        }).ToList();

        var current = instances.FirstOrDefault(x => x.InstanceGuid == apply.CurrentApprovalInstanceGuid);
        if (current?.CurrentNodeGuid is Guid nodeGuid)
        {
            var node = await Context.Queryable<FIN_Reimb_ApprovalNode>()
                .FirstAsync(x => x.CompanyId == companyId && x.ProcessGuid == current.ProcessGuid && x.NodeGuid == nodeGuid);
            if (node is not null)
                detail.CurrentNode = new ApplicationNodeDto
                {
                    NodeName = node.NodeName,
                    ApproverDeptName = node.ApproverDeptName,
                    ApproverUserName = node.ApproverUserName
                };
        }

        var exception = await Context.Queryable<FIN_Reimb_PaymentException>()
            .FirstAsync(x => x.CompanyId == companyId && x.ApplyGuid == applyGuid);
        if (exception is not null)
            detail.PaymentException = new PaymentExceptionDto
            {
                RequiredInvoiceAmount = exception.RequiredInvoiceAmount,
                VerifiedInvoiceAmount = exception.VerifiedInvoiceAmount,
                ExceptionStatus = (PaymentExceptionStatus)exception.ExceptionStatus,
                SupplementCount = exception.SupplementCount,
                LastSupplementTime = exception.LastSupplementTime,
                ClosedAt = exception.ClosedAt
            };
        detail.PaymentOrder = await GetPaymentOrderCoreAsync(companyId, applyGuid);
        return detail;
    }

    /// <summary>获取退回或已打款申请的再次申请预填表单。</summary>
    /// <param name="applyGuid">源申请 GUID。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>原申请及只读发票；不存在时为 null。</returns>
    public async Task<ResubmitPreloadDto?> GetResubmitPreloadAsync(Guid applyGuid,
        CancellationToken cancellationToken = default)
    {
        var application = await GetApplicationAsync(applyGuid, cancellationToken);
        if (application is null) return null;
        if (application.ApplyStatus is not (ReimbursementApplyStatus.ReviewReturned or
            ReimbursementApplyStatus.Paid))
            throw new InvalidOperationException("当前申请状态不允许再次申请");
        return new ResubmitPreloadDto { Application = application };
    }

    /// <summary>查询本人申请关联的打款单。</summary>
    /// <param name="applyGuid">申请 GUID。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>打款单；无记录时为 null。</returns>
    public async Task<PaymentOrderDto?> GetPaymentOrderAsync(Guid applyGuid,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var companyId = CompanyId;
        if (await FindOwnedApplicationAsync(applyGuid, companyId) is null) return null;
        return await GetPaymentOrderCoreAsync(companyId, applyGuid);
    }

    /// <summary>查询本人发票上传的最新 OCR、验真和重复校验结果。</summary>
    /// <param name="uploadGuid">上传记录 GUID。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>上传状态；不存在时为 null。</returns>
    public async Task<InvoiceUploadDto?> GetInvoiceUploadAsync(Guid uploadGuid,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var companyId = CompanyId;
        var upload = await Context.Queryable<FIN_Reimb_InvoiceUpload>()
            .FirstAsync(x => x.CompanyId == companyId && x.UploadUserId == _user.UserID && x.UploadGuid == uploadGuid);
        return upload is null ? null : MapUpload(upload);
    }

    /// <summary>只读统计有效发票金额并检查是否覆盖申请金额。</summary>
    /// <param name="request">待校验上传 GUID、申请金额、申请类型和流程版本。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>有效金额、有效/无效上传 GUID 及报销不足额提示。</returns>
    public async Task<InvoiceValidationDto> ValidateInvoicesAsync(InvoiceValidationRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var companyId = CompanyId;
        if (request is null || request.ApplyAmount <= 0 || decimal.Round(request.ApplyAmount, 2) != request.ApplyAmount ||
            request.ApplyType is not (ReimbursementApplyType.Reimbursement or ReimbursementApplyType.Payment))
            throw new ArgumentException("发票校验参数无效");

        // 付款申请未选发票时按零票额计算；报销申请仍返回对应流程的不足额提示。
        var ids = request.UploadGuids ?? new List<Guid>();
        if (ids.Any(x => x == Guid.Empty) || ids.Count != ids.Distinct().Count())
            throw new ArgumentException("发票校验参数无效");
        var uploads = ids.Count == 0 ? new List<FIN_Reimb_InvoiceUpload>() :
            await Context.Queryable<FIN_Reimb_InvoiceUpload>()
                .Where(x => x.CompanyId == companyId && x.UploadUserId == _user.UserID && ids.Contains(x.UploadGuid))
                .ToListAsync();
        if (uploads.Count != ids.Count) throw new UnauthorizedAccessException("包含无权访问的发票上传记录");

        // 发票池记录说明该号码已经由另一张上传记录使用，不能重复计入金额。
        var numbers = uploads.Where(x => !string.IsNullOrWhiteSpace(x.InvoiceNumber))
            .Select(x => x.InvoiceNumber).Distinct().ToList();
        var pools = numbers.Count == 0 ? new List<FIN_Reimb_InvoicePool>() :
            await Context.Queryable<FIN_Reimb_InvoicePool>()
                .Where(x => x.CompanyId == companyId && numbers.Contains(x.InvoiceNumber)).ToListAsync();
        var occupied = pools.Where(x => uploads.All(u => u.UploadGuid != x.SourceUploadGuid))
            .Select(x => x.InvoiceNumber).ToHashSet();
        var valid = uploads.Where(x => x.RecognitionStatus == 1 && x.VerifyStatus == 1 &&
                                       x.DuplicateCheckStatus == 1 && x.IsEffective && x.TotalAmount > 0 &&
                                       !string.IsNullOrWhiteSpace(x.InvoiceNumber) && !occupied.Contains(x.InvoiceNumber))
            .GroupBy(x => x.InvoiceNumber).Select(x => x.First()).ToList();
        var validIds = valid.Select(x => x.UploadGuid).ToHashSet();
        var total = valid.Sum(x => x.TotalAmount ?? 0);
        var result = new InvoiceValidationDto
        {
            IsValid = valid.Count > 0 && total >= request.ApplyAmount,
            ValidInvoiceTotalAmount = total,
            ValidUploadGuids = valid.Select(x => x.UploadGuid).ToList(),
            InvalidUploadGuids = ids.Where(x => !validIds.Contains(x)).ToList()
        };
        if (!result.IsValid && request.ApplyType == ReimbursementApplyType.Reimbursement)
            result.Message = request.FlowType switch
            {
                ReimbursementFlowType.Audit => "发票金额不足，请点击付款申请，作废",
                ReimbursementFlowType.Budget => "发票金额不足，请点击付款申请",
                _ => null
            };
        return result;
    }

    /// <summary>按企业和当前申请人查找申请，隔离其他用户数据。</summary>
    /// <param name="applyGuid">申请 GUID。</param>
    /// <param name="companyId">当前企业 ID。</param>
    /// <returns>匹配的申请实体；无数据时为 null。</returns>
    private Task<FIN_Reimb_Apply> FindOwnedApplicationAsync(Guid applyGuid, int companyId) =>
        Context.Queryable<FIN_Reimb_Apply>()
            .FirstAsync(x => x.CompanyId == companyId && x.ApplicantId == _user.UserID && x.ApplyGuid == applyGuid);

    /// <summary>在已完成申请归属校验后读取打款单。</summary>
    /// <param name="companyId">当前企业 ID。</param>
    /// <param name="applyGuid">申请 GUID。</param>
    /// <returns>打款单；无记录时为 null。</returns>
    private async Task<PaymentOrderDto?> GetPaymentOrderCoreAsync(int companyId, Guid applyGuid)
    {
        var order = await Context.Queryable<FIN_Reimb_PaymentOrder>()
            .FirstAsync(x => x.CompanyId == companyId && x.ApplyGuid == applyGuid);
        return order is null ? null : new PaymentOrderDto
        {
            PaymentNo = order.PaymentNo,
            PayAmount = order.PayAmount,
            PayStatus = (PaymentOrderStatus)order.PayStatus,
            BankSerialNo = order.BankSerialNo,
            ReceiptUrl = order.ReceiptUrl,
            PaidAt = order.PaidAt,
            FailureReason = order.FailureReason
        };
    }

    /// <summary>提取发票上传记录中的小程序可见状态。</summary>
    /// <param name="upload">发票上传实体。</param>
    /// <returns>上传状态 DTO。</returns>
    private static InvoiceUploadDto MapUpload(FIN_Reimb_InvoiceUpload upload) => new()
    {
        UploadGuid = upload.UploadGuid,
        RecognitionStatus = upload.RecognitionStatus,
        VerifyStatus = upload.VerifyStatus,
        DuplicateCheckStatus = upload.DuplicateCheckStatus,
        IsEffective = upload.IsEffective,
        InvoiceNumber = upload.InvoiceNumber,
        TotalAmount = upload.TotalAmount,
        FailureMessage = upload.FailureMessage
    };

    /// <summary>生成申请列表项及详情、补票、再次申请操作标识。</summary>
    /// <param name="apply">申请实体。</param>
    /// <returns>列表项 DTO。</returns>
    private static ApplicationListItemDto MapListItem(FIN_Reimb_Apply apply)
    {
        var item = new ApplicationListItemDto
        {
            ApplyGuid = apply.ApplyGuid,
            ApplyNo = apply.ApplyNo,
            ApplyType = (ReimbursementApplyType)apply.ApplyType,
            ApplyTypeName = apply.ApplyType == 1 ? "报销申请" : "付款申请",
            ApplicantName = apply.ApplicantName,
            ExpenseName = apply.ExpenseName,
            ApplyAmount = apply.ApplyAmount,
            InvoiceTotalAmount = apply.InvoiceTotalAmount,
            ApplyStatus = (ReimbursementApplyStatus)apply.ApplyStatus,
            ApplyStatusName = apply.ApplyStatus switch
            {
                10 => "待审核",
                20 => "审核退回",
                30 => "待打款",
                40 => "已打款",
                _ => string.Empty
            },
            InvoiceSupplementStatus = (InvoiceSupplementStatus)apply.InvoiceSupplementStatus,
            InvoiceSupplementStatusName = apply.InvoiceSupplementStatus switch
            {
                0 => "不适用",
                1 => "待补传发票",
                2 => "已关闭",
                _ => string.Empty
            },
            SubmittedAt = apply.SubmittedAt
        };
        item.Actions.Add("detail");
        if (apply.ApplyType == 2 && apply.InvoiceSupplementStatus == 1)
            item.Actions.Add("supplement");
        if (apply.ApplyStatus is (byte)ReimbursementApplyStatus.ReviewReturned or
            (byte)ReimbursementApplyStatus.Paid)
            item.Actions.Add("resubmit");
        return item;
    }

    /// <summary>将列表项共用字段复制到详情 DTO。</summary>
    /// <param name="source">已映射的列表项。</param>
    /// <param name="target">待填充的详情 DTO。</param>
    private static void CopyListFields(ApplicationListItemDto source, ApplicationListItemDto target)
    {
        target.ApplyGuid = source.ApplyGuid;
        target.ApplyNo = source.ApplyNo;
        target.ApplyType = source.ApplyType;
        target.ApplyTypeName = source.ApplyTypeName;
        target.ApplicantName = source.ApplicantName;
        target.ExpenseName = source.ExpenseName;
        target.ApplyAmount = source.ApplyAmount;
        target.InvoiceTotalAmount = source.InvoiceTotalAmount;
        target.ApplyStatus = source.ApplyStatus;
        target.ApplyStatusName = source.ApplyStatusName;
        target.InvoiceSupplementStatus = source.InvoiceSupplementStatus;
        target.InvoiceSupplementStatusName = source.InvoiceSupplementStatusName;
        target.SubmittedAt = source.SubmittedAt;
        target.Actions = source.Actions;
    }

    public async Task<Live_Partner?> GetCurrentPartnerAsync(Guid companyGId, int userId, CancellationToken cancellationToken = default)
    {
        return await Main.Queryable<Live_Personnel>().InnerJoin<Live_Partner>((personnel, partner) => personnel.CompanyGID == partner.CompanyGID && personnel.PartnerGID == partner.PartnerGID)
             .Where(personnel => personnel.CompanyGID == companyGId && personnel.UserID == userId)
             .Select<Live_Partner>()
             .FirstAsync(cancellationToken);
    }
}
