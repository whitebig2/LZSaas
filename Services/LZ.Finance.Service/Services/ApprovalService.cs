using System.Security.Cryptography;
using System.Text;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Finance.Service.Interface;
using LZ.Model;
using LZ.Model.Dtos.Reimbusement;
using LZ.Model.Enums;
using LZ.Model.Enums.Reimbusement;
using SqlSugar;
using SqlSugar.IOC;

namespace LZ.Finance.Service.Services;

public sealed class ApprovalService : ServicesBase<FIN_Reimb_ApprovalProcess>, IApprovalService, IScopedDependency
{
    private readonly ICurrentUser _user;
    private readonly IHttpService _httpService;

    /// <summary>接入当前用户身份和既有审批人查询依赖。</summary>
    /// <param name="user">当前登录用户。</param>
    /// <param name="httpService">既有审批人查询使用的 HTTP 服务。</param>
    public ApprovalService(ICurrentUser user, IHttpService httpService)
    {
        _user = user;
        _httpService = httpService;
    }

    private int CompanyId
    {
        get
        {
            if (!_user.IsAuthenticated || _user.CompanyID <= 0 || _user.UserID <= 0 ||
                _user.PlatformType is not (PlatformType.MiniAdmin or PlatformType.XFZMini))
                throw new UnauthorizedAccessException("当前用户不能访问报销审批功能");
            return _user.CompanyID;
        }
    }

    private ISqlSugarClient Main => DbScoped.SugarScope.GetConnection("Main");

    /// <summary>将小程序或分后台身份映射为审批账号及其部门，用于审批数据查询。</summary>
    /// <param name="companyId">当前企业 ID。</param>
    /// <returns>企业审批账号和可选部门。</returns>
    private async Task<(SYS_CompanyUser User, Live_Partner? Department)> GetViewerAsync(int companyId)
    {
        SYS_CompanyUser? actor;
        if (_user.PlatformType == PlatformType.XFZMini)
        {
            // 小程序令牌 ID 属于 Live_User，审批节点 ID 属于 SYS_CompanyUser。
            var miniUser = await Main.Queryable<Live_User>()
                .FirstAsync(x => x.UserID == _user.UserID && x.CompanyID == companyId && x.Enabled);
            if (miniUser is null || string.IsNullOrWhiteSpace(miniUser.Phone))
                throw new UnauthorizedAccessException("当前小程序用户没有有效的审批身份");
            var personnel = await Main.Queryable<Live_Personnel>()
                .Where(x => x.CompanyID == companyId && x.Phone == miniUser.Phone && x.Enabled)
                .ToListAsync();
            if (personnel.Count != 1 || personnel[0].PartnerGID is not Guid partnerGuid)
                throw new UnauthorizedAccessException("当前小程序用户没有唯一的审批人员关系");
            var matches = await Main.Queryable<SYS_CompanyUser>()
                .Where(x => x.CompanyID == companyId && x.Enabled &&
                            x.PersonnelID == personnel[0].UserID && x.PartnerGID == partnerGuid)
                .ToListAsync();
            if (matches.Count != 1 || matches[0].PersonnelGID != personnel[0].UserGID)
                throw new UnauthorizedAccessException("当前小程序用户没有有效的审批账号映射");
            actor = matches[0];
        }
        else
        {
            actor = await Main.Queryable<SYS_CompanyUser>()
                .FirstAsync(x => x.UserID == _user.UserID && x.CompanyID == companyId && x.Enabled);
        }
        if (actor is null)
            throw new UnauthorizedAccessException("当前企业用户不存在或已停用");
        var departmentGuid = _user.PlatformType == PlatformType.XFZMini ? actor.PartnerGID : actor.DeptGID;
        var department = departmentGuid is Guid deptGuid
            ? await Main.Queryable<Live_Partner>()
                .FirstAsync(x => x.PartnerGID == deptGuid && x.CompanyID == companyId)
            : null;
        return (actor, department);
    }

    /// <summary>取得必须具有有效部门的审批操作人身份。</summary>
    /// <param name="companyId">当前企业 ID。</param>
    /// <returns>审批账号和必需的部门。</returns>
    private async Task<(SYS_CompanyUser User, Live_Partner Department)> GetActorAsync(int companyId)
    {
        var (user, department) = await GetViewerAsync(companyId);
        if (department is null)
            throw new UnauthorizedAccessException("当前用户没有有效的审批部门");
        return (user, department);
    }

    /// <summary>查询当前企业生效的审批流程及按顺序排列的节点。</summary>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>生效流程；无配置时为 null。</returns>
    public async Task<ApprovalProcessDto?> GetActiveProcessAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var companyId = CompanyId;
        if (_user.PlatformType != PlatformType.MiniAdmin)
            throw new UnauthorizedAccessException("仅分后台用户可查看审核流程配置");
        var process = await Context.Queryable<FIN_Reimb_ApprovalProcess>()
            .Where(x => x.CompanyId == companyId && x.IsActive)
            .OrderByDescending(x => x.VersionNo).FirstAsync();
        if (process is null) return null;
        var nodes = await Context.Queryable<FIN_Reimb_ApprovalNode>()
            .Where(x => x.CompanyId == companyId && x.ProcessGuid == process.ProcessGuid)
            .OrderBy(x => x.NodeSeq).ToListAsync();
        return MapProcess(process, nodes);
    }

    /// <summary>校验、保存并发布新的企业审批流程版本。</summary>
    /// <param name="request">流程基本信息及动态审批节点。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>已发布的流程及节点。</returns>
    public async Task<ApprovalProcessDto> SaveAndPublishAsync(ApprovalProcessSaveRequest request, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var companyId = CompanyId;
        if (_user.PlatformType != PlatformType.MiniAdmin)
            throw new UnauthorizedAccessException("仅分后台用户可配置审核流程");
        if (request is null || string.IsNullOrWhiteSpace(request.ProcessName) || request.ProcessName.Trim().Length > 100 ||
            request.Description?.Length > 255 || request.Nodes is null || request.Nodes.Count == 0)
            throw new ArgumentException("流程名称及至少一个审批节点为必填项");
        var ordered = request.Nodes.OrderBy(x => x.NodeSeq).ToList();
        for (var index = 0; index < ordered.Count; index++)
        {
            var node = ordered[index];
            if (node.NodeSeq != index + 1 || string.IsNullOrWhiteSpace(node.NodeName) || node.NodeName.Trim().Length > 100 ||
                node.ApproverDeptId == Guid.Empty || node.ApproverUserId <= 0 || node.ApproverRoleName?.Length > 100 
                //|| node.AmountThreshold < 0 || decimal.Round(node.AmountThreshold, 2) != node.AmountThreshold 
                //|| node.AmountOperator is not ("GreaterThan" or "GreaterThanOrEqual")
                )
                throw new ArgumentException("审核节点顺序、审批人或金额条件无效");
        }
        //if (!ordered[0].IsEnabled || ordered[0].AmountConditionEnabled)
        //    throw new ArgumentException("首个审核节点必须启用且无金额条件");

        var deptIds = ordered.Select(x => x.ApproverDeptId).Distinct().ToList();
        var userIds = ordered.Select(x => x.ApproverUserId).Distinct().ToList();
        var departments = await Main.Queryable<Live_Partner>()
            .Where(x => x.CompanyID == companyId && deptIds.Contains(x.PartnerGID.Value)).ToListAsync();
        var users = await Main.Queryable<Live_Personnel>()
            .Where(x => x.CompanyID == companyId && x.Enabled && userIds.Contains(x.UserID)).ToListAsync();
        var deptById = departments.ToDictionary(x => x.PartnerGID!.Value);
        var userById = users.ToDictionary(x => x.UserID);
        foreach (var node in ordered)
        {
            if (!deptById.TryGetValue(node.ApproverDeptId, out var dept) ||
                !userById.TryGetValue(node.ApproverUserId, out var approver) || approver.PartnerGID != dept.PartnerGID)
                throw new ArgumentException($"第 {node.NodeSeq} 个节点的部门和审批人不匹配");
        }

        Context.Ado.BeginTran();
        try
        {
            // 发布操作锁住当前企业的版本范围，确保版本号递增且仅一个版本生效。
            await Context.Ado.SqlQueryAsync<Guid>(
                "SELECT ProcessGuid FROM FIN_Reimb_ApprovalProcess WITH (UPDLOCK, HOLDLOCK) WHERE CompanyId = @companyId",
                new SugarParameter("@companyId", companyId));
            var old = await Context.Queryable<FIN_Reimb_ApprovalProcess>()
                .Where(x => x.CompanyId == companyId).OrderByDescending(x => x.VersionNo).FirstAsync();
            var now = DateTime.Now;
            var process = new FIN_Reimb_ApprovalProcess
            {
                ProcessGuid = Guid.NewGuid(),
                CompanyId = companyId,
                ProcessName = request.ProcessName.Trim(),
                Description = request.Description?.Trim(),
                VersionNo = (old?.VersionNo ?? 0) + 1,
                IsActive = true,
                CreatedBy = _user.UserID,
                CreatedAt = now,
                PublishedAt = now
            };
            await Context.Updateable<FIN_Reimb_ApprovalProcess>()
                .SetColumns(x => x.IsActive == false).Where(x => x.CompanyId == companyId && x.IsActive)
                .ExecuteCommandAsync();
            
            await Context.Insertable(process).IgnoreColumns(x => new { x.Id, x.RowVersion }).ExecuteCommandAsync();
            var nodes = ordered.Select(x => new FIN_Reimb_ApprovalNode
            {
                NodeGuid = Guid.NewGuid(),
                CompanyId = companyId,
                ProcessGuid = process.ProcessGuid,
                NodeSeq = x.NodeSeq,
                NodeName = x.NodeName.Trim(),
                ApproverDeptId = x.ApproverDeptId,
                ApproverDeptName = deptById[x.ApproverDeptId].PartnerName,
                ApproverUserId = x.ApproverUserId,
                ApproverUserName = userById[x.ApproverUserId].FullName,
                ApproverRoleName = x.ApproverRoleName?.Trim(),
                AmountConditionEnabled = x.AmountConditionEnabled,
                AmountOperator = x.AmountOperator,
                AmountThreshold = x.AmountThreshold,
                IsEnabled = x.IsEnabled,
                CreatedAt = now
            }).ToList();
            await Context.Insertable(nodes).ExecuteCommandAsync();
            Context.Ado.CommitTran();
            return MapProcess(process, nodes);
        }
        catch
        {
            Context.Ado.RollbackTran();
            throw;
        }
    }

    /// <summary>按当前审批账号权限及筛选条件读取审批申请分页。</summary>
    /// <param name="query">待办状态、申请字段、日期和分页条件。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>审批申请分页。</returns>
    public async Task<ApprovalPageDto<ApprovalListItemDto>> GetPagedAsync(ApprovalQuery query, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var companyId = CompanyId;
        if (query is null || query.PageIndex < 1 || query.PageSize is < 1 or > 100 ||
            query.ApplyTimeStart > query.ApplyTimeEnd)
            throw new ArgumentException("分页或申请时间范围无效");
        var actor = await GetViewerAsync(companyId);
        List<Guid>? visibleApplyIds = null;
        if (_user.PlatformType == PlatformType.XFZMini)
        {
            var departmentGuid = actor.Department?.PartnerGID ?? Guid.Empty;
            var miniNodeIds = await Context.Queryable<FIN_Reimb_ApprovalNode>()
                .Where(x => x.CompanyId == companyId && x.ApproverUserId == actor.User.UserID &&
                            x.ApproverDeptId == departmentGuid && x.IsEnabled)
                .Select(x => x.NodeGuid).ToListAsync();
            var todoIds = miniNodeIds.Count == 0 ? new List<Guid>() :
                await Context.Queryable<FIN_Reimb_ApprovalInstance>()
                    .Where(x => x.CompanyId == companyId &&
                                x.InstanceStatus == (byte)ApprovalInstanceStatus.InProgress &&
                                miniNodeIds.Contains(x.CurrentNodeGuid.Value))
                    .Select(x => x.ApplyGuid).ToListAsync();
            var doneIds = await Context.Queryable<FIN_Reimb_ApprovalRecord>()
                .Where(x => x.CompanyId == companyId && x.OperatorId == actor.User.UserID)
                .Select(x => x.ApplyGuid).ToListAsync();
            visibleApplyIds = query.PendingOnly switch
            {
                true => todoIds,
                false => doneIds,
                null => todoIds.Concat(doneIds).Distinct().ToList()
            };
            if (visibleApplyIds.Count == 0)
                return new ApprovalPageDto<ApprovalListItemDto>
                {
                    PageIndex = query.PageIndex, PageSize = query.PageSize
                };
        }
        var status = query.ApplyStatus.HasValue ? (byte)query.ApplyStatus.Value : (byte?)null;
        var invoiceStatus = query.InvoiceSupplementStatus.HasValue ? (byte)query.InvoiceSupplementStatus.Value : (byte?)null;
        var listQuery = Context.Queryable<FIN_Reimb_Apply>()
            .Where(x => x.CompanyId == companyId)
            .WhereIF(visibleApplyIds is not null, x => visibleApplyIds!.Contains(x.ApplyGuid))
            .WhereIF(status.HasValue, x => x.ApplyStatus == status)
            .WhereIF(invoiceStatus.HasValue, x => x.InvoiceSupplementStatus == invoiceStatus)
            .WhereIF(!string.IsNullOrWhiteSpace(query.ExpenseName), x => x.ExpenseName.Contains(query.ExpenseName!))
            .WhereIF(!string.IsNullOrWhiteSpace(query.ApplyNo), x => x.ApplyNo.Contains(query.ApplyNo!))
            .WhereIF(query.ApplyTimeStart.HasValue, x => x.SubmittedAt >= query.ApplyTimeStart)
            .WhereIF(query.ApplyTimeEnd.HasValue, x => x.SubmittedAt <= query.ApplyTimeEnd)
            .WhereIF(query.PendingOnly == true, x => x.ApplyStatus == (byte)ReimbursementApplyStatus.PendingReview)
            .WhereIF(_user.PlatformType == PlatformType.MiniAdmin && query.PendingOnly == false,
                x => x.ApplyStatus != (byte)ReimbursementApplyStatus.PendingReview);
        var count = new RefAsync<int>();
        var applies = await listQuery.OrderBy(x => x.SubmittedAt, OrderByType.Desc)
            .OrderBy(x => x.Id, OrderByType.Desc)
            .ToPageListAsync(query.PageIndex, query.PageSize, count);
        var instanceIds = applies.Where(x => x.CurrentApprovalInstanceGuid.HasValue)
            .Select(x => x.CurrentApprovalInstanceGuid!.Value).Distinct().ToList();
        var instances = instanceIds.Count == 0 ? new List<FIN_Reimb_ApprovalInstance>() :
            await Context.Queryable<FIN_Reimb_ApprovalInstance>()
                .Where(x => x.CompanyId == companyId && instanceIds.Contains(x.InstanceGuid)).ToListAsync();
        var nodeIds = instances.Where(x => x.CurrentNodeGuid.HasValue)
            .Select(x => x.CurrentNodeGuid!.Value).Distinct().ToList();
        var nodes = nodeIds.Count == 0 ? new List<FIN_Reimb_ApprovalNode>() :
            await Context.Queryable<FIN_Reimb_ApprovalNode>()
                .Where(x => x.CompanyId == companyId && nodeIds.Contains(x.NodeGuid)).ToListAsync();
        var instanceById = instances.ToDictionary(x => x.InstanceGuid);
        var nodeById = nodes.ToDictionary(x => x.NodeGuid);
        return new ApprovalPageDto<ApprovalListItemDto>
        {
            PageIndex = query.PageIndex,
            PageSize = query.PageSize,
            TotalCount = count.Value,
            Items = applies.Select(x =>
            {
                instanceById.TryGetValue(x.CurrentApprovalInstanceGuid ?? Guid.Empty, out var instance);
                nodeById.TryGetValue(instance?.CurrentNodeGuid ?? Guid.Empty, out var node);
                return MapListItem(x, node, actor.User.UserID, actor.Department?.PartnerGID!.Value ?? Guid.Empty);
            }).ToList()
        };
    }

    /// <summary>查询申请表单、当前节点、审批记录及当前用户能否审批。</summary>
    /// <param name="applyGuid">申请 GUID。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>审批详情；不存在时为 null。</returns>
    public async Task<ApprovalDetailDto?> GetDetailAsync(Guid applyGuid, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var companyId = CompanyId;
        var apply = await Context.Queryable<FIN_Reimb_Apply>()
            .FirstAsync(x => x.CompanyId == companyId && x.ApplyGuid == applyGuid);
        if (apply is null) return null;
        var actor = await GetViewerAsync(companyId);
        var instance = apply.CurrentApprovalInstanceGuid is Guid currentGuid
            ? await Context.Queryable<FIN_Reimb_ApprovalInstance>()
                .FirstAsync(x => x.CompanyId == companyId && x.InstanceGuid == currentGuid && x.ApplyGuid == applyGuid)
            : await Context.Queryable<FIN_Reimb_ApprovalInstance>()
                .Where(x => x.CompanyId == companyId && x.ApplyGuid == applyGuid)
                .OrderByDescending(x => x.RoundNo).FirstAsync();
        var node = instance?.InstanceStatus == (byte)ApprovalInstanceStatus.InProgress &&
                   instance.CurrentNodeGuid is Guid nodeGuid
            ? await Context.Queryable<FIN_Reimb_ApprovalNode>()
                .FirstAsync(x => x.CompanyId == companyId && x.NodeGuid == nodeGuid && x.ProcessGuid == instance.ProcessGuid)
            : null;
        if (_user.PlatformType == PlatformType.XFZMini)
        {
            var isCurrent = node?.ApproverUserId == actor.User.UserID &&
                            node.ApproverDeptId == actor.Department?.PartnerGID;
            var wasOperator = await Context.Queryable<FIN_Reimb_ApprovalRecord>()
                .AnyAsync(x => x.CompanyId == companyId && x.ApplyGuid == applyGuid &&
                               x.OperatorId == actor.User.UserID);
            if (!isCurrent && !wasOperator)
                throw new UnauthorizedAccessException("当前用户无权查看此审批单");
        }
        var relations = await Context.Queryable<FIN_Reimb_ApplyInvoice>()
            .Where(x => x.CompanyId == companyId && x.ApplyGuid == applyGuid && x.IsActive).ToListAsync();
        var invoiceIds = relations.Select(x => x.InvoiceGuid).Distinct().ToList();
        var invoices = invoiceIds.Count == 0 ? new List<FIN_Reimb_InvoicePool>() :
            await Context.Queryable<FIN_Reimb_InvoicePool>()
                .Where(x => x.CompanyId == companyId && invoiceIds.Contains(x.InvoiceGuid)).ToListAsync();
        var invoiceById = invoices.ToDictionary(x => x.InvoiceGuid);
        var rounds = await Context.Queryable<FIN_Reimb_ApprovalInstance>()
            .Where(x => x.CompanyId == companyId && x.ApplyGuid == applyGuid).ToListAsync();
        var roundById = rounds.ToDictionary(x => x.InstanceGuid, x => x.RoundNo);
        var records = await Context.Queryable<FIN_Reimb_ApprovalRecord>()
            .Where(x => x.CompanyId == companyId && x.ApplyGuid == applyGuid)
            .OrderBy(x => x.OperatedAt).ToListAsync();
        return new ApprovalDetailDto
        {
            Application = MapListItem(apply, node, actor.User.UserID, actor.Department?.PartnerGID!.Value ?? Guid.Empty),
            FlowType = (ReimbursementFlowType)apply.FlowType,
            LegalEntityName = apply.LegalEntityName,
            CostTypeName = apply.CostTypeName,
            PayeeName = apply.PayeeName,
            PayeeBankName = apply.PayeeBankName,
            PayeeBankCardNoMasked = apply.PayeeBankCardNoMasked,
            ExpensePurpose = apply.ExpensePurpose,
            Remark = apply.Remark,
            InvoiceTotalAmount = apply.InvoiceTotalAmount,
            LastRejectReason = apply.LastRejectReason,
            CurrentRoundNo = instance?.RoundNo,
            Invoices = relations.Where(x => invoiceById.ContainsKey(x.InvoiceGuid))
                .Select(x => new ApprovalInvoiceDto
                {
                    InvoiceGuid = x.InvoiceGuid,
                    InvoiceNumber = invoiceById[x.InvoiceGuid].InvoiceNumber,
                    TotalAmount = invoiceById[x.InvoiceGuid].TotalAmount,
                    RelationPurpose = (InvoiceUploadPurpose)x.RelationPurpose,
                    RelatedAt = x.RelatedAt
                }).ToList(),
            History = records.Select(x => new ApprovalHistoryDto
            {
                RoundNo = roundById.GetValueOrDefault(x.InstanceGuid),
                NodeSeq = x.NodeSeq,
                NodeName = x.NodeName,
                OperatorName = x.OperatorName,
                ActionType = (ApprovalActionType)x.ActionType,
                RejectReason = x.RejectReason,
                Comment = x.Comment,
                OperatedAt = x.OperatedAt
            }).ToList()
        };
    }

    /// <summary>处理分后台审批通过，不要求客户端幂等键。</summary>
    /// <param name="applyGuid">申请 GUID。</param>
    /// <param name="request">审批意见。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>异步审批任务。</returns>
    public Task ApproveAsync(Guid applyGuid, ApprovalApproveRequest request, CancellationToken cancellationToken = default)
    {
        if (_user.PlatformType != PlatformType.MiniAdmin)
            throw new UnauthorizedAccessException("小程序审批请使用带幂等键的入口");
        if (request?.Comment?.Length > 1000) throw new ArgumentException("审批意见不能超过1000字");
        return DecideAsync(applyGuid, ApprovalActionType.Approve, request?.Comment?.Trim(), null,
            null, cancellationToken);
    }

    /// <summary>处理分后台审批驳回，不要求客户端幂等键。</summary>
    /// <param name="applyGuid">申请 GUID。</param>
    /// <param name="request">驳回原因。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>异步审批任务。</returns>
    public Task RejectAsync(Guid applyGuid, ApprovalRejectRequest request, CancellationToken cancellationToken = default)
    {
        if (_user.PlatformType != PlatformType.MiniAdmin)
            throw new UnauthorizedAccessException("小程序审批请使用带幂等键的入口");
        if (string.IsNullOrWhiteSpace(request?.RejectReason) || request.RejectReason.Trim().Length > 1000)
            throw new ArgumentException("驳回原因必填且不能超过1000字");
        return DecideAsync(applyGuid, ApprovalActionType.Reject, null, request.RejectReason.Trim(),
            null, cancellationToken);
    }

    /// <summary>按小程序幂等键通过当前审批节点。</summary>
    /// <param name="applyGuid">申请 GUID。</param>
    /// <param name="request">审批意见。</param>
    /// <param name="idempotencyKey">本次审批操作的幂等键。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>异步审批任务。</returns>
    public Task ApproveWithKeyAsync(Guid applyGuid, ApprovalApproveRequest request, string idempotencyKey,
        CancellationToken cancellationToken = default)
    {
        ValidateMiniDecisionKey(idempotencyKey);
        if (request?.Comment?.Length > 1000) throw new ArgumentException("审批意见不能超过1000字");
        return DecideAsync(applyGuid, ApprovalActionType.Approve, request?.Comment?.Trim(), null,
            idempotencyKey.Trim(), cancellationToken);
    }

    /// <summary>按小程序幂等键驳回当前审批节点。</summary>
    /// <param name="applyGuid">申请 GUID。</param>
    /// <param name="request">驳回原因。</param>
    /// <param name="idempotencyKey">本次审批操作的幂等键。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>异步审批任务。</returns>
    public Task RejectWithKeyAsync(Guid applyGuid, ApprovalRejectRequest request, string idempotencyKey,
        CancellationToken cancellationToken = default)
    {
        ValidateMiniDecisionKey(idempotencyKey);
        if (string.IsNullOrWhiteSpace(request?.RejectReason) || request.RejectReason.Trim().Length > 1000)
            throw new ArgumentException("驳回原因必填且不能超过1000字");
        return DecideAsync(applyGuid, ApprovalActionType.Reject, null, request.RejectReason.Trim(),
            idempotencyKey.Trim(), cancellationToken);
    }

    /// <summary>校验小程序审批幂等键及调用方平台。</summary>
    /// <param name="key">客户端提供的幂等键。</param>
    private void ValidateMiniDecisionKey(string key)
    {
        if (_user.PlatformType != PlatformType.XFZMini)
            throw new UnauthorizedAccessException("仅小程序审批人可使用此入口");
        if (string.IsNullOrWhiteSpace(key) || key.Trim().Length > 100)
            throw new ArgumentException("审批幂等键必填且不能超过100字");
    }

    /// <summary>审批共用状态机：核验节点权限，写审批记录并推进、完成或驳回申请。</summary>
    /// <param name="applyGuid">申请 GUID。</param>
    /// <param name="action">通过或驳回动作。</param>
    /// <param name="comment">通过时的审批意见。</param>
    /// <param name="rejectReason">驳回时的原因。</param>
    /// <param name="idempotencyKey">小程序幂等键；分后台入口传 null。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    private async Task DecideAsync(Guid applyGuid, ApprovalActionType action, string? comment, string? rejectReason,
        string? idempotencyKey, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var companyId = CompanyId;
        var actor = await GetActorAsync(companyId);
        // 同一审批人、申请和动作的同一幂等键映射为同一个审批记录 ID。
        var recordGuid = idempotencyKey is null ? Guid.NewGuid() : new Guid(
            SHA256.HashData(Encoding.UTF8.GetBytes(
                $"{companyId}:{actor.User.UserID}:{applyGuid}:{(byte)action}:{idempotencyKey}"))
                .AsSpan(0, 16));
        if (idempotencyKey is not null && await DecisionExistsAsync(recordGuid, actor.User.UserID, applyGuid, action))
            return;
        Context.Ado.BeginTran();
        try
        {
            var apply = await Context.Queryable<FIN_Reimb_Apply>()
                .FirstAsync(x => x.CompanyId == companyId && x.ApplyGuid == applyGuid);
            if (apply?.ApplyStatus != (byte)ReimbursementApplyStatus.PendingReview ||
                apply.CurrentApprovalInstanceGuid is not Guid instanceGuid)
                throw new InvalidOperationException("申请不在待审核状态");
            var instance = await Context.Queryable<FIN_Reimb_ApprovalInstance>()
                .FirstAsync(x => x.CompanyId == companyId && x.ApplyGuid == applyGuid && x.InstanceGuid == instanceGuid);
            if (instance?.InstanceStatus != (byte)ApprovalInstanceStatus.InProgress ||
                instance.CurrentNodeGuid is not Guid nodeGuid)
                throw new InvalidOperationException("当前审批实例已处理或不存在");
            var node = await Context.Queryable<FIN_Reimb_ApprovalNode>()
                .FirstAsync(x => x.CompanyId == companyId && x.ProcessGuid == instance.ProcessGuid && x.NodeGuid == nodeGuid);
            if (node is null || !node.IsEnabled || node.ApproverUserId != actor.User.UserID ||
                node.ApproverDeptId != actor.Department?.PartnerGID!.Value)
                throw new UnauthorizedAccessException("当前用户不是此节点绑定的审批人");

            var now = DateTime.Now;
            FIN_Reimb_ApprovalNode? next = null;
            if (action == ApprovalActionType.Approve)
            {
                var candidates = await Context.Queryable<FIN_Reimb_ApprovalNode>()
                    .Where(x => x.CompanyId == companyId && x.ProcessGuid == instance.ProcessGuid &&
                                x.NodeSeq > node.NodeSeq && x.IsEnabled)
                    .OrderBy(x => x.NodeSeq).ToListAsync();
                next = candidates.FirstOrDefault(x => !x.AmountConditionEnabled ||
                    (x.AmountOperator == "GreaterThan" && apply.ApplyAmount > x.AmountThreshold) ||
                    (x.AmountOperator == "GreaterThanOrEqual" && apply.ApplyAmount >= x.AmountThreshold));
            }
            // RowVersion prevents two requests from advancing the same node twice.
            var changed = await Context.Ado.ExecuteCommandAsync(
                "UPDATE FIN_Reimb_ApprovalInstance SET CurrentNodeGuid = @nextGuid, CurrentNodeSeq = @nextSeq, " +
                "InstanceStatus = @status, EndedAt = @endedAt WHERE CompanyId = @companyId AND ApplyGuid = @applyGuid " +
                "AND InstanceGuid = @instanceGuid AND InstanceStatus = @inProgress AND CurrentNodeGuid = @nodeGuid " +
                "AND RowVersion = @rowVersion",
                new SugarParameter("@nextGuid", next?.NodeGuid), new SugarParameter("@nextSeq", next?.NodeSeq),
                new SugarParameter("@status", (byte)(action == ApprovalActionType.Reject
                    ? ApprovalInstanceStatus.Returned : next is null ? ApprovalInstanceStatus.Approved : ApprovalInstanceStatus.InProgress)),
                new SugarParameter("@endedAt", action == ApprovalActionType.Reject || next is null ? now : (DateTime?)null),
                new SugarParameter("@companyId", companyId), new SugarParameter("@applyGuid", applyGuid),
                new SugarParameter("@instanceGuid", instanceGuid),
                new SugarParameter("@inProgress", (byte)ApprovalInstanceStatus.InProgress),
                new SugarParameter("@nodeGuid", nodeGuid), new SugarParameter("@rowVersion", instance.RowVersion));
            if (changed != 1) throw new InvalidOperationException("审批状态已变化，请刷新后重试");
            await Context.Insertable(new FIN_Reimb_ApprovalRecord
            {
                RecordGuid = recordGuid,
                CompanyId = companyId,
                InstanceGuid = instanceGuid,
                ApplyGuid = applyGuid,
                NodeGuid = nodeGuid,
                NodeSeq = node.NodeSeq,
                NodeName = node.NodeName,
                OperatorId = actor.User.UserID,
                OperatorName = actor.User.FullName,
                OperatorDeptName = actor.Department?.PartnerName ?? string.Empty,
                ActionType = (byte)action,
                RejectReason = rejectReason,
                Comment = comment,
                OperatedAt = now
            }).ExecuteCommandAsync();
            if (action == ApprovalActionType.Reject)
            {
                changed = await Context.Updateable<FIN_Reimb_Apply>()
                    .SetColumns(x => new FIN_Reimb_Apply
                    {
                        ApplyStatus = (byte)ReimbursementApplyStatus.ReviewReturned,
                        CurrentApprovalInstanceGuid = null,
                        LastRejectReason = rejectReason,
                        LastRejectTime = now
                    })
                    .Where(x => x.CompanyId == companyId && x.ApplyGuid == applyGuid &&
                                x.ApplyStatus == (byte)ReimbursementApplyStatus.PendingReview &&
                                x.CurrentApprovalInstanceGuid == instanceGuid).ExecuteCommandAsync();
            }
            else if (next is null)
            {
                changed = await Context.Updateable<FIN_Reimb_Apply>()
                    .SetColumns(x => new FIN_Reimb_Apply
                    {
                        ApplyStatus = (byte)ReimbursementApplyStatus.PendingPayment,
                        CurrentApprovalInstanceGuid = null
                    })
                    .Where(x => x.CompanyId == companyId && x.ApplyGuid == applyGuid &&
                                x.ApplyStatus == (byte)ReimbursementApplyStatus.PendingReview &&
                                x.CurrentApprovalInstanceGuid == instanceGuid).ExecuteCommandAsync();
            }
            if ((action == ApprovalActionType.Reject || next is null) && changed != 1)
                throw new InvalidOperationException("申请状态已变化，请刷新后重试");
            Context.Ado.CommitTran();
        }
        catch
        {
            Context.Ado.RollbackTran();
            // 并发重试可能先命中行版本冲突，若记录已落库则视为同一次操作成功。
            if (idempotencyKey is not null &&
                await DecisionExistsAsync(recordGuid, actor.User.UserID, applyGuid, action))
                return;
            throw;
        }
    }

    /// <summary>判断相同审批人和幂等键生成的审批记录是否已落库。</summary>
    /// <param name="recordGuid">由幂等键派生的审批记录 GUID。</param>
    /// <param name="actorId">审批账号 ID。</param>
    /// <param name="applyGuid">申请 GUID。</param>
    /// <param name="action">审批动作。</param>
    /// <returns>记录是否存在。</returns>
    private Task<bool> DecisionExistsAsync(Guid recordGuid, int actorId, Guid applyGuid, ApprovalActionType action) =>
        Context.Queryable<FIN_Reimb_ApprovalRecord>()
            .AnyAsync(x => x.RecordGuid == recordGuid && x.CompanyId == _user.CompanyID &&
                           x.ApplyGuid == applyGuid && x.OperatorId == actorId && x.ActionType == (byte)action);

    /// <summary>在申请提交方的 Finance 事务内启动新一轮审批并绑定首节点。</summary>
    /// <param name="applyGuid">已写入或已退回的申请 GUID。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>新审批实例 GUID；外层调用方负责事务提交或回滚。</returns>
    public async Task<Guid> StartAsync(Guid applyGuid, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!_user.IsAuthenticated || _user.CompanyID <= 0 || _user.UserID <= 0 ||
            _user.PlatformType is not (PlatformType.MiniAdmin or PlatformType.XFZMini))
            throw new UnauthorizedAccessException("当前用户不能发起报销审批");
        var companyId = _user.CompanyID;
        var apply = await Context.Queryable<FIN_Reimb_Apply>()
            .FirstAsync(x => x.CompanyId == companyId && x.ApplyGuid == applyGuid);
        if (apply is null || apply.CurrentApprovalInstanceGuid.HasValue)
            throw new InvalidOperationException("申请不存在或已有进行中的审批实例");
        if (_user.PlatformType == PlatformType.XFZMini && apply.ApplicantId != _user.UserID)
            throw new UnauthorizedAccessException("只有原申请人可以重新发起审批");
        var process = await Context.Queryable<FIN_Reimb_ApprovalProcess>()
            .Where(x => x.CompanyId == companyId && x.IsActive).OrderByDescending(x => x.VersionNo).FirstAsync();
        if (process is null) throw new InvalidOperationException("当前企业未配置审核流程");
        var first = await Context.Queryable<FIN_Reimb_ApprovalNode>()
            .FirstAsync(x => x.CompanyId == companyId && x.ProcessGuid == process.ProcessGuid && x.NodeSeq == 1);
        if (first is null || !first.IsEnabled || first.AmountConditionEnabled)
            throw new InvalidOperationException("审核流程首节点配置无效");
        var old = await Context.Queryable<FIN_Reimb_ApprovalInstance>()
            .Where(x => x.CompanyId == companyId && x.ApplyGuid == applyGuid)
            .OrderByDescending(x => x.RoundNo).FirstAsync();
        var expectedStatus = old is null
            ? ReimbursementApplyStatus.PendingReview
            : ReimbursementApplyStatus.ReviewReturned;
        if (apply.ApplyStatus != (byte)expectedStatus ||
            old is not null && old.InstanceStatus != (byte)ApprovalInstanceStatus.Returned)
            throw new InvalidOperationException("申请状态不允许启动审批");
        var now = DateTime.Now;
        var instance = new FIN_Reimb_ApprovalInstance
        {
            InstanceGuid = Guid.NewGuid(),
            CompanyId = companyId,
            ApplyGuid = applyGuid,
            ApplicantId = apply.ApplicantId,
            ProcessGuid = process.ProcessGuid,
            ProcessVersionNo = process.VersionNo,
            RoundNo = (old?.RoundNo ?? 0) + 1,
            CurrentNodeGuid = first.NodeGuid,
            CurrentNodeSeq = first.NodeSeq,
            InstanceStatus = (byte)ApprovalInstanceStatus.InProgress,
            StartedAt = now
        };
        // 提交/重提服务负责外层事务，使申请、发票和审批实例一起提交或回滚。
        await Context.Insertable(instance).IgnoreColumns(x => new { x.Id, x.RowVersion }).ExecuteCommandAsync();
        var changed = await Context.Updateable<FIN_Reimb_Apply>()
            .SetColumns(x => new FIN_Reimb_Apply
            {
                ApplyStatus = (byte)ReimbursementApplyStatus.PendingReview,
                CurrentApprovalInstanceGuid = instance.InstanceGuid,
                LastRejectReason = null,
                LastRejectTime = null
            })
            .Where(x => x.CompanyId == companyId && x.ApplyGuid == applyGuid &&
                        x.ApplyStatus == (byte)expectedStatus &&
                        x.CurrentApprovalInstanceGuid == null).ExecuteCommandAsync();
        if (changed != 1) throw new InvalidOperationException("申请状态已变化，请刷新后重试");
        return instance.InstanceGuid;
    }

    /// <summary>将流程和节点实体转换为流程配置 DTO。</summary>
    /// <param name="process">流程实体。</param>
    /// <param name="nodes">按顺序读取的节点实体。</param>
    /// <returns>流程配置 DTO。</returns>
    private static ApprovalProcessDto MapProcess(FIN_Reimb_ApprovalProcess process, List<FIN_Reimb_ApprovalNode> nodes) => new()
    {
        ProcessGuid = process.ProcessGuid,
        ProcessName = process.ProcessName,
        Description = process.Description,
        VersionNo = process.VersionNo,
        PublishedAt = process.PublishedAt,
        Nodes = nodes.Select(x => new ApprovalNodeDto
        {
            NodeGuid = x.NodeGuid,
            NodeSeq = x.NodeSeq,
            NodeName = x.NodeName,
            ApproverDeptId = x.ApproverDeptId,
            ApproverDeptName = x.ApproverDeptName,
            ApproverUserId = x.ApproverUserId,
            ApproverUserName = x.ApproverUserName,
            ApproverRoleName = x.ApproverRoleName,
            AmountConditionEnabled = x.AmountConditionEnabled,
            AmountOperator = x.AmountOperator,
            AmountThreshold = x.AmountThreshold,
            IsEnabled = x.IsEnabled
        }).ToList()
    };

    /// <summary>将申请和当前节点映射为审批列表项。</summary>
    /// <param name="apply">申请实体。</param>
    /// <param name="node">当前审批节点；无节点时为 null。</param>
    /// <param name="userId">当前查看人的审批账号 ID。</param>
    /// <param name="deptId">当前查看人的审批部门 GUID。</param>
    /// <returns>包含可审批标识的列表项。</returns>
    private static ApprovalListItemDto MapListItem(FIN_Reimb_Apply apply, FIN_Reimb_ApprovalNode? node,
        int userId, Guid deptId) => new()
        {
            ApplyGuid = apply.ApplyGuid,
            ApplyNo = apply.ApplyNo,
            ApplyType = (ReimbursementApplyType)apply.ApplyType,
            ApplicantName = apply.ApplicantName,
            ApplicantDeptName = apply.ApplicantDeptName,
            ExpenseName = apply.ExpenseName,
            ApplyAmount = apply.ApplyAmount,
            ApplyStatus = (ReimbursementApplyStatus)apply.ApplyStatus,
            InvoiceSupplementStatus = (InvoiceSupplementStatus)apply.InvoiceSupplementStatus,
            SubmittedAt = apply.SubmittedAt,
            CurrentNodeGuid = node?.NodeGuid,
            CurrentNodeName = node?.NodeName,
            CanApprove = apply.ApplyStatus == (byte)ReimbursementApplyStatus.PendingReview &&
                     node?.ApproverUserId == userId && node.ApproverDeptId == deptId
        };

    /// <summary>查询指定部门或伙伴的既有审批人配置。</summary>
    /// <param name="partnerGid">部门或伙伴 GUID。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>审批人列表。</returns>
    public async Task<List<ApprovalDto>> GetApproversAsync(Guid partnerGid, CancellationToken cancellationToken = default)
    {
        var result  = await Main.Queryable<Live_Personnel>().Where(it=>it.PartnerGID == partnerGid).Select<ApprovalDto>().ToListAsync();
        return result;
    }
}
