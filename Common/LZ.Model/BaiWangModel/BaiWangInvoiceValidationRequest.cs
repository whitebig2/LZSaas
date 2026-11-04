using System.ComponentModel.DataAnnotations;

namespace LZ.Model.BaiWangModel
{
    /// <summary>
    /// 百望云发票真伪查验请求。公共参数（包括 token）由 BaiWangService 填充。
    /// </summary>
    public class BaiWangInvoiceValidationRequest : IValidatableObject
    {
        /// <summary>购方税号，最长 20 字节。</summary>
        [Required]
        [StringLength(20)]
        public string taxNo { get; set; } = string.Empty;

        /// <summary>发票代码，10 位或 12 位；数电票可不传。</summary>
        public string? invoiceCode { get; set; }

        /// <summary>发票号码，最长 20 字节。</summary>
        [Required]
        [StringLength(20)]
        public string invoiceNumber { get; set; } = string.Empty;

        /// <summary>开票日期，格式 yyyy-MM-dd。</summary>
        [Required]
        public string billingDate { get; set; } = string.Empty;

        /// <summary>校验码后 6 位；普通发票等票种必填，可传完整校验码。</summary>
        public string? checkCode_6 { get; set; }

        /// <summary>合计金额；不同票种应传价税合计、不含税金额或车价合计，详见百望云文档。</summary>
        public string? totalAmount { get; set; }

        /// <summary>发票类型代码；electricType 为 102 时必填。</summary>
        public string? invoiceType { get; set; }

        /// <summary>查验通道类型；为空走综服查验，102 走乐企查验。</summary>
        public string? electricType { get; set; }

        /// <summary>
        /// 校验文档明确规定的日期、票种及查验要素，避免无效请求消耗查验次数。
        /// </summary>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!DateTime.TryParseExact(billingDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out _))
            {
                yield return new ValidationResult("开票日期必须为 yyyy-MM-dd 格式。", new[] { nameof(billingDate) });
            }

            if (!string.IsNullOrEmpty(invoiceCode) && invoiceCode.Length != 10 && invoiceCode.Length != 12)
            {
                yield return new ValidationResult("发票代码必须为 10 位或 12 位。", new[] { nameof(invoiceCode) });
            }

            if (electricType == "102" && string.IsNullOrWhiteSpace(invoiceType))
            {
                yield return new ValidationResult("乐企查验必须提供发票类型。", new[] { nameof(invoiceType) });
            }

            if (invoiceType is "04" or "10" or "11" or "14" or "86")
            {
                if (string.IsNullOrWhiteSpace(checkCode_6) || checkCode_6.Length < 6 || checkCode_6.Length > 30)
                {
                    yield return new ValidationResult("该票种需要提供至少 6 位、最多 30 位的校验码。", new[] { nameof(checkCode_6) });
                }
            }
            else if (!string.IsNullOrWhiteSpace(invoiceType) && string.IsNullOrWhiteSpace(totalAmount))
            {
                yield return new ValidationResult("该票种需要提供合计金额。", new[] { nameof(totalAmount) });
            }
        }
    }
}
