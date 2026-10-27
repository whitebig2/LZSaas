using LZ.Model.Enums.Reimbusement;

namespace LZ.Model.Dtos.Reimbusement;

public sealed class ApprovalProcessSaveRequest
{
    public string ProcessName { get; set; } = "报销审批流程";
    public string? Description { get; set; }
    public List<ApprovalNodeSaveRequest> Nodes { get; set; } = new();
}

public sealed class ApprovalNodeSaveRequest
{
    public int NodeSeq { get; set; }
    public string NodeName { get; set; } = string.Empty;
    public Guid ApproverDeptId { get; set; }
    public int ApproverUserId { get; set; }
    public string? ApproverRoleName { get; set; }
    public bool AmountConditionEnabled { get; set; }
    public string AmountOperator { get; set; } = "GreaterThan";
    public decimal AmountThreshold { get; set; } = 5000m;
    public bool IsEnabled { get; set; } = true;
}

public sealed class ApprovalProcessDto
{
    public Guid ProcessGuid { get; set; }
    public string ProcessName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int VersionNo { get; set; }
    public DateTime? PublishedAt { get; set; }
    public List<ApprovalNodeDto> Nodes { get; set; } = new();
}

public sealed class ApprovalNodeDto
{
    public Guid NodeGuid { get; set; }
    public int NodeSeq { get; set; }
    public string NodeName { get; set; } = string.Empty;
    public Guid ApproverDeptId { get; set; }
    public string ApproverDeptName { get; set; } = string.Empty;
    public int ApproverUserId { get; set; }
    public string ApproverUserName { get; set; } = string.Empty;
    public string? ApproverRoleName { get; set; }
    public bool AmountConditionEnabled { get; set; }
    public string AmountOperator { get; set; } = "GreaterThan";
    public decimal AmountThreshold { get; set; }
    public bool IsEnabled { get; set; }
}

public sealed class ApprovalQuery
{
    public ReimbursementApplyStatus? ApplyStatus { get; set; }
    public InvoiceSupplementStatus? InvoiceSupplementStatus { get; set; }
    public string? ExpenseName { get; set; }
    public string? ApplyNo { get; set; }
    public DateTime? ApplyTimeStart { get; set; }
    public DateTime? ApplyTimeEnd { get; set; }
    public bool? PendingOnly { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public sealed class ApprovalPageDto<T>
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public long TotalCount { get; set; }
    public List<T> Items { get; set; } = new();
}

public sealed class ApprovalListItemDto
{
    public Guid ApplyGuid { get; set; }
    public string ApplyNo { get; set; } = string.Empty;
    public ReimbursementApplyType ApplyType { get; set; }
    public string ApplicantName { get; set; } = string.Empty;
    public string? ApplicantDeptName { get; set; }
    public string ExpenseName { get; set; } = string.Empty;
    public decimal ApplyAmount { get; set; }
    public ReimbursementApplyStatus ApplyStatus { get; set; }
    public InvoiceSupplementStatus InvoiceSupplementStatus { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public Guid? CurrentNodeGuid { get; set; }
    public string? CurrentNodeName { get; set; }
    public bool CanApprove { get; set; }
}

public sealed class ApprovalDetailDto
{
    public ApprovalListItemDto Application { get; set; } = new();
    public ReimbursementFlowType FlowType { get; set; }
    public string LegalEntityName { get; set; } = string.Empty;
    public string CostTypeName { get; set; } = string.Empty;
    public string PayeeName { get; set; } = string.Empty;
    public string PayeeBankName { get; set; } = string.Empty;
    public string PayeeBankCardNoMasked { get; set; } = string.Empty;
    public string ExpensePurpose { get; set; } = string.Empty;
    public string? Remark { get; set; }
    public decimal InvoiceTotalAmount { get; set; }
    public string? LastRejectReason { get; set; }
    public int? CurrentRoundNo { get; set; }
    public List<ApprovalInvoiceDto> Invoices { get; set; } = new();
    public List<ApprovalHistoryDto> History { get; set; } = new();
}

public sealed class ApprovalInvoiceDto
{
    public Guid InvoiceGuid { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public InvoiceUploadPurpose RelationPurpose { get; set; }
    public DateTime? RelatedAt { get; set; }
}

public sealed class ApprovalHistoryDto
{
    public int RoundNo { get; set; }
    public int NodeSeq { get; set; }
    public string NodeName { get; set; } = string.Empty;
    public string OperatorName { get; set; } = string.Empty;
    public ApprovalActionType ActionType { get; set; }
    public string? RejectReason { get; set; }
    public string? Comment { get; set; }
    public DateTime? OperatedAt { get; set; }
}

public class ApprovalDto 
{
    public int UserID { get; set; }
    public string UserName { get; set; }
}

public sealed class ApprovalApproveRequest
{
    public string? Comment { get; set; }
}

public sealed class ApprovalRejectRequest
{
    public string RejectReason { get; set; } = string.Empty;
}
