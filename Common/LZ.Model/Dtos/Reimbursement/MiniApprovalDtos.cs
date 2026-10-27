namespace LZ.Model.Dtos.Reimbursement;

public sealed class MiniApprovalQuery
{
    // todo: 当前节点待办；done: 本人已处理；all: 两者合并。
    public string Status { get; set; } = "todo";
    public string? ExpenseName { get; set; }
    public string? ApplyNo { get; set; }
    public DateTime? ApplyTimeStart { get; set; }
    public DateTime? ApplyTimeEnd { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public sealed class MiniApproveRequest
{
    public string? Comment { get; set; }
    public string IdempotencyKey { get; set; } = string.Empty;
}

public sealed class MiniRejectRequest
{
    public string RejectReason { get; set; } = string.Empty;
    public string IdempotencyKey { get; set; } = string.Empty;
}
