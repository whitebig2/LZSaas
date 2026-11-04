using LZ.Model.Dtos.Reimbusement;
using LZ.Model.Enums.Reimbusement;

namespace LZ.Model.Dtos.Reimbursement;

public sealed class ApplicationQuery
{
    public ReimbursementApplyStatus? ApplyStatus { get; set; }
    public InvoiceSupplementStatus? InvoiceSupplementStatus { get; set; }
    public string? ExpenseName { get; set; }
    public string? Keyword { get; set; }
    public DateTime? ApplyTimeStart { get; set; }
    public DateTime? ApplyTimeEnd { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public sealed class ApplicationPageDto<T>
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public long TotalCount { get; set; }
    public List<T> Items { get; set; } = new();
}

public class ApplicationListItemDto
{
    public Guid ApplyGuid { get; set; }
    public string ApplyNo { get; set; } = string.Empty;
    public ReimbursementApplyType ApplyType { get; set; }
    public string ApplyTypeName { get; set; } = string.Empty;
    public string ApplicantName { get; set; } = string.Empty;
    public string ExpenseName { get; set; } = string.Empty;
    public decimal ApplyAmount { get; set; }
    public decimal InvoiceTotalAmount { get; set; }
    public ReimbursementApplyStatus ApplyStatus { get; set; }
    public string ApplyStatusName { get; set; } = string.Empty;
    public InvoiceSupplementStatus InvoiceSupplementStatus { get; set; }
    public string InvoiceSupplementStatusName { get; set; } = string.Empty;
    public DateTime? SubmittedAt { get; set; }
    public List<string> Actions { get; set; } = new();
}

public sealed class ApplicationDetailDto : ApplicationListItemDto
{
    public ReimbursementFlowType FlowType { get; set; }
    public Guid LegalEntityId { get; set; }
    public string LegalEntityName { get; set; } = string.Empty;
    public int CostTypeId { get; set; }
    public string CostTypeName { get; set; } = string.Empty;
    public string PayeeName { get; set; } = string.Empty;
    public string PayeeBankName { get; set; } = string.Empty;
    public string PayeeBankCardNoMasked { get; set; } = string.Empty;
    public string ExpensePurpose { get; set; } = string.Empty;
    public string? Remark { get; set; }
    public string? LastRejectReason { get; set; }
    public ApplicationNodeDto? CurrentNode { get; set; }
    public List<ApplicationInvoiceDto> Invoices { get; set; } = new();
    public List<ApprovalHistoryDto> ApprovalHistory { get; set; } = new();
    public PaymentExceptionDto? PaymentException { get; set; }
    public PaymentOrderDto? PaymentOrder { get; set; }
}

public sealed class ApplicationNodeDto
{
    public string NodeName { get; set; } = string.Empty;
    public string ApproverDeptName { get; set; } = string.Empty;
    public string ApproverUserName { get; set; } = string.Empty;
}

public sealed class ApplicationInvoiceDto
{
    public Guid InvoiceGuid { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public byte RelationPurpose { get; set; }
    public DateTime? RelatedAt { get; set; }
    public string? FileUrl { get; set; }
    public string? FileName { get; set; }
}

public sealed class PaymentExceptionDto
{
    public decimal RequiredInvoiceAmount { get; set; }
    public decimal VerifiedInvoiceAmount { get; set; }
    public PaymentExceptionStatus ExceptionStatus { get; set; }
    public int SupplementCount { get; set; }
    public DateTime? LastSupplementTime { get; set; }
    public DateTime? ClosedAt { get; set; }
}

public sealed class PaymentOrderDto
{
    public string PaymentNo { get; set; } = string.Empty;
    public decimal PayAmount { get; set; }
    public PaymentOrderStatus PayStatus { get; set; }
    public string? BankSerialNo { get; set; }
    public string? ReceiptUrl { get; set; }
    public DateTime? PaidAt { get; set; }
    public string? FailureReason { get; set; }
}

public sealed class ResubmitPreloadDto
{
    public ApplicationDetailDto Application { get; set; } = new();
    public bool AllowModifyInvoices => false;
}

/// <summary>单次发票上传的识别、验真和查重结果；IsEffective 为 true 时才可计入申请票额。</summary>
public sealed class InvoiceUploadDto
{
    /// <summary>本次上传记录标识，提交申请或补传时放入 UploadGuids。</summary>
    public Guid UploadGuid { get; set; }
    /// <summary>识别状态：0 待识别，1 成功，2 失败。</summary>
    public byte RecognitionStatus { get; set; }
    /// <summary>验真状态：0 待验真，1 成功，2 失败。</summary>
    public byte VerifyStatus { get; set; }
    /// <summary>查重状态：0 待查重，1 通过，2 重复。</summary>
    public byte DuplicateCheckStatus { get; set; }
    /// <summary>三项校验均通过且未被其他上传占用时为 true。</summary>
    public bool IsEffective { get; set; }
    /// <summary>识别出的发票号码；识别失败时为空。</summary>
    public string? InvoiceNumber { get; set; }
    /// <summary>验真票面的价税合计金额；验真未通过时为空，只有有效发票可计入合计。</summary>
    public decimal? TotalAmount { get; set; }
    /// <summary>失败时展示给上传人的提示；成功时为空。</summary>
    public string? FailureMessage { get; set; }
}

/// <summary>多张发票提交前预校验请求；仅计算金额，不绑定到申请。</summary>
public sealed class InvoiceValidationRequest
{
    /// <summary>本次拟使用的上传 GUID 列表，重复 GUID 属于无效请求。</summary>
    public List<Guid> UploadGuids { get; set; } = new();
    /// <summary>申请金额；有效发票金额合计须覆盖报销申请金额。</summary>
    public decimal ApplyAmount { get; set; }
    /// <summary>申请类型：报销或付款；付款申请允许票额不足后续补传。</summary>
    public ReimbursementApplyType ApplyType { get; set; }
    /// <summary>报销金额不足时用于选择审核版或预算版提示；可为空。</summary>
    public ReimbursementFlowType? FlowType { get; set; }
}

/// <summary>多张发票按号码去重后的预校验结果。</summary>
public sealed class InvoiceValidationDto
{
    /// <summary>去重后的有效票额是否达到申请金额；报销提交据此决定能否继续。</summary>
    public bool IsValid { get; set; }
    /// <summary>本次有效且号码不重复的发票价税合计金额。</summary>
    public decimal ValidInvoiceTotalAmount { get; set; }
    /// <summary>可计入金额并可用于本次申请绑定的上传 GUID。</summary>
    public List<Guid> ValidUploadGuids { get; set; } = new();
    /// <summary>识别、验真或查重未通过，以及在本次列表中重复的上传 GUID。</summary>
    public List<Guid> InvalidUploadGuids { get; set; } = new();
    /// <summary>报销发票金额不足时的提示；无提示时为空。</summary>
    public string? Message { get; set; }
}

public sealed class ApiResponse<T>
{
    public int Code { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public string TraceId { get; set; } = string.Empty;
}

/// <summary>报销申请、付款申请及再次提交共用的表单字段。</summary>
public class ApplicationFieldsRequest
{
    /// <summary>法人主体 ID；必填，须大于 0，对应法人主体选项。</summary>
    public Guid LegalEntityId { get; set; }
    /// <summary>法人主体名称快照；必填，最长 200 字符，随申请保存。</summary>
    public string LegalEntityName { get; set; } = string.Empty;
    /// <summary>成本类型 ID；必填，须大于 0，对应成本类型选项。</summary>
    public int CostTypeId { get; set; }
    /// <summary>成本类型名称快照；必填，最长 100 字符，随申请保存。</summary>
    public string CostTypeName { get; set; } = string.Empty;
    /// <summary>费用名称；必填，最长 200 字符。</summary>
    public string ExpenseName { get; set; } = string.Empty;
    /// <summary>收款人或收款单位名称；必填，最长 200 字符。</summary>
    public string PayeeName { get; set; } = string.Empty;
    /// <summary>收款账户的银行名称；必填，最长 200 字符。</summary>
    public string PayeeBankName { get; set; } = string.Empty;
    /// <summary>收款银行卡或银行账号原号；必填，最长 32 字符，服务端加密存储。</summary>
    public string PayeeBankCardNo { get; set; } = string.Empty;
    /// <summary>本次费用的用途说明；必填，最长 500 字符。</summary>
    public string ExpensePurpose { get; set; } = string.Empty;
    /// <summary>补充备注；可不传，最长 1000 字符。</summary>
    public string? Remark { get; set; }
    /// <summary>申请金额；必填，须大于 0，最多两位小数。报销申请须有不低于该金额的有效发票。</summary>
    public decimal ApplyAmount { get; set; }
    /// <summary>客户端生成的本次操作唯一键；必填，最长 100 字符。相同用户和操作重复提交同一键时返回同一申请。</summary>
    public string IdempotencyKey { get; set; } = string.Empty;
}

/// <summary>Submit 接口请求；包含公共表单字段、申请类型和首次上传的发票记录。</summary>
public sealed class ApplicationCreateRequest : ApplicationFieldsRequest
{
    /// <summary>申请类型：1 为报销申请，2 为付款申请；报销须有足额有效发票，付款允许后补发票。</summary>
    public ReimbursementApplyType ApplyType { get; set; }
    /// <summary>本次提交所选发票上传记录的 UploadGuid 列表，先通过 UploadInvoice 获取；报销申请须提供足额有效发票，付款申请可不传或传空列表，之后通过 Supplement 补票。</summary>
    public List<Guid> UploadGuids { get; set; } = new();
}

public sealed class ApplicationResubmitRequest : ApplicationFieldsRequest
{
    /// <summary>重提时新增的发票 GUID；可为空，原申请已关联的发票会继续计入金额。</summary>
    public List<Guid> UploadGuids { get; set; } = new();
}

public sealed class ApplicationCopyRequest : ApplicationFieldsRequest
{
    // 已打款申请可沿用原票新建；重提时金额保持原值，避免只读发票与金额失配。
    public Guid SourceApplyGuid { get; set; }
}

public sealed class ApplySubmitResultDto
{
    public Guid ApplyGuid { get; set; }
    public string ApplyNo { get; set; } = string.Empty;
    public ReimbursementApplyStatus ApplyStatus { get; set; }
    public ReimbursementFlowType FlowType { get; set; }
    public bool ExceptionCreated { get; set; }
}

public sealed class SupplementInvoiceRequest
{
    public List<Guid> UploadGuids { get; set; } = new();
    public string IdempotencyKey { get; set; } = string.Empty;
}

public sealed class CompletePaymentRequest
{
    public string IdempotencyKey { get; set; } = string.Empty;
    public string BankSerialNo { get; set; } = string.Empty;
    public string? ReceiptUrl { get; set; }
}

public sealed class FlowVersionDto
{
    public ReimbursementFlowType FlowType { get; set; }
    public string Source { get; set; } = string.Empty;
}

public sealed class DictItemDto
{
    public int Value { get; set; }
    public string Label { get; set; } = string.Empty;
}

/// <summary>UploadInvoice 的 multipart/form-data 请求参数。</summary>
public sealed class InvoiceUploadRequest
{
    /// <summary>补传时必填，指向待补票付款申请；首次上传时必须为空。</summary>
    public Guid? ApplyGuid { get; set; }
    /// <summary>上传用途：Initial 为首次申请上传，Supplement 为付款申请补传。</summary>
    public InvoiceUploadPurpose UploadPurpose { get; set; }
    /// <summary>发票原文件；支持 PDF、JPG、JPEG、PNG，每次上传一个文件。</summary>
    public Microsoft.AspNetCore.Http.IFormFile File { get; set; } = null!;
}
