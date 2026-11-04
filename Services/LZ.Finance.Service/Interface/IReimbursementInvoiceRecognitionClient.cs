using Microsoft.AspNetCore.Http;

namespace LZ.Finance.Service.Interface;

/// <summary>报销发票的图片识别与票面验真适配器。</summary>
public interface IReimbursementInvoiceRecognitionClient
{
    /// <summary>识别已上传的发票图片；PDF 会逐页转成图片并识别。</summary>
    /// <param name="file">客户端上传的原始发票文件。</param>
    /// <param name="extension">带点号的小写文件扩展名，例如 .pdf 或 .jpg。</param>
    /// <param name="fileUrl">原始文件上传到 OSS 后的地址；图片文件直接用此地址识别。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>发票号、购方税号、开票日期和 OCR 金额；开票日期为 yyyy-MM-dd。</returns>
    Task<ReimbursementRecognizedInvoice> RecognizeAsync(IFormFile file, string extension,
        string fileUrl, CancellationToken cancellationToken);

    /// <summary>将上传流程传入的发票要素送至百望云验真。</summary>
    /// <param name="invoice">OCR 识别出的验真要素；票面比对仍使用其中的购方税号，发票类型固定使用字符串 32。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>验真通过的发票号和价税合计金额；验真不通过时抛出异常。</returns>
    Task<ReimbursementVerifiedInvoice> VerifyAsync(ReimbursementRecognizedInvoice invoice,
        CancellationToken cancellationToken);
}

/// <summary>阿里云 OCR 识别结果；仅在报销验真过程中传递，不写入发票上传表。</summary>
/// <param name="InvoiceNumber">发票号码，作为验真与查重键。</param>
/// <param name="BuyerTaxNo">购方税号，作为百望云验真要素。</param>
/// <param name="BillingDate">开票日期，格式 yyyy-MM-dd。</param>
/// <param name="TotalAmount">OCR 识别的价税合计金额，用于与验真票面金额核对。</param>
public sealed record ReimbursementRecognizedInvoice(string InvoiceNumber, string BuyerTaxNo,
    string BillingDate, decimal TotalAmount);

/// <summary>百望云验真通过后的票面结果，供上传记录保存和后续金额合计使用。</summary>
/// <param name="InvoiceNumber">验真票面的发票号码。</param>
/// <param name="TotalAmount">验真票面的价税合计金额。</param>
public sealed record ReimbursementVerifiedInvoice(string InvoiceNumber, decimal TotalAmount);
