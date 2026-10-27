namespace LZ.Model.BaiWangModel
{
    /// <summary>
    /// 百望开放平台 OAuth Token 响应
    /// </summary>
    public class BaiWangTokenResponse
    {
        public string access_token { get; set; } = string.Empty;
        public string token_type { get; set; } = string.Empty;
        public string refresh_token { get; set; } = string.Empty;
        public int expires_in { get; set; }
        public string scope { get; set; } = string.Empty;
        public string error { get; set; } = string.Empty;
        public string error_description { get; set; } = string.Empty;
    }

    /// <summary>
    /// 百望开放平台通用响应
    /// </summary>
    public class BaiWangResponse<T>
    {
        public bool success { get; set; }
        public string method { get; set; } = string.Empty;
        public string requestId { get; set; } = string.Empty;
        public T? response { get; set; }
        public BaiWangErrorResponse? errorResponse { get; set; }
    }

    public class BaiWangErrorResponse
    {
        public string code { get; set; } = string.Empty;
        public string message { get; set; } = string.Empty;
        public string subCode { get; set; } = string.Empty;
        public string subMessage { get; set; } = string.Empty;
    }

    /// <summary>
    /// 创建开票申请响应
    /// </summary>
    public class BaiWangAddPreInvoiceResponse
    {
        public string serialNo { get; set; } = string.Empty;
        public string orderNo { get; set; } = string.Empty;
        public string invoiceNo { get; set; } = string.Empty;
        public string invoiceCode { get; set; } = string.Empty;
        public string status { get; set; } = string.Empty;
    }

    /// <summary>
    /// 查询开票申请响应
    /// </summary>
    public class BaiWangQueryPreInvoiceResponse
    {
        public string serialNo { get; set; } = string.Empty;
        public string orderNo { get; set; } = string.Empty;
        public string invoiceNo { get; set; } = string.Empty;
        public string invoiceCode { get; set; } = string.Empty;
        public string status { get; set; } = string.Empty;
        public string buyerName { get; set; } = string.Empty;
        public string buyerTaxNo { get; set; } = string.Empty;
        public string invoiceTotalPriceTax { get; set; } = string.Empty;
        public string invoiceDate { get; set; } = string.Empty;
        public string failReason { get; set; } = string.Empty;
    }

    /// <summary>
    /// 查询发票响应
    /// </summary>
    public class BaiWangQueryInvoiceResponse
    {
        public List<BaiWangQuery> list { get; set; } = new();
    }

    /// <summary>
    /// 查询发票响应
    /// </summary>
    public class BaiWangQuery
    {
        public string serialNo { get; set; } = string.Empty;
        public string orderNo { get; set; } = string.Empty;
        public string orderDateTime { get; set; } = string.Empty;
        public string invoiceTime { get; set; } = string.Empty;
        public string invoiceTerminalCode { get; set; } = string.Empty;
        public string taxNo { get; set; } = string.Empty;
        public string sellerName { get; set; } = string.Empty;
        public string sellerAddress { get; set; } = string.Empty;
        public string sellerPhone { get; set; } = string.Empty;
        public string sellerBankName { get; set; } = string.Empty;
        public string sellerBankNumber { get; set; } = string.Empty;
        public string buyerTaxNo { get; set; } = string.Empty;
        public string buyerName { get; set; } = string.Empty;
        public string buyerAddress { get; set; } = string.Empty;
        public string buyerTelephone { get; set; } = string.Empty;
        public string buyerBankName { get; set; } = string.Empty;
        public string buyerBankNumber { get; set; } = string.Empty;
        public string invoiceType { get; set; } = string.Empty;
        public string invoiceTypeCode { get; set; } = string.Empty;
        public string invoiceCode { get; set; } = string.Empty;
        public string invoiceNo { get; set; } = string.Empty;
        public string digitInvoiceNo { get; set; } = string.Empty;
        public string status { get; set; } = string.Empty;
        public string invoiceBaseStatus { get; set; } = string.Empty;
    }

    /// <summary>
    /// 开具发票响应
    /// </summary>
    public class BaiWangIssueInvoiceResponse
    {
        public string serialNo { get; set; } = string.Empty;
    }
    /// <summary>
    /// 开具发票响应
    /// </summary>
    public class BaiWangFastRedResponse
    {
        public string serialNo { get; set; } = string.Empty;
        public string orderNo { get; set; } = string.Empty;
        public string redInfoSerialNo { get; set; } = string.Empty;
        public string redConfirmSerialNo { get; set; } = string.Empty;
    }


    public class BaiWangRedConfirmResponse
    {
        public List<BaiWangRedConfirm> list { get; set; } = new();
    }
    public class BaiWangRedConfirm
    {
        public string redConfirmSerialNo { get; set; } = string.Empty;
        public string orderNo { get; set; } = string.Empty;
        public string redConfirmStatus { get; set; } = string.Empty;
        public string originalDigitInvoiceNo { get; set; } = string.Empty;
    }
}
