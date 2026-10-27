namespace LZ.Model.BaiWangModel
{
    /// <summary>
    /// 创建开票申请请求
    /// </summary>
    public class BaiWangAddPreInvoiceRequest
    {
        /// <summary>
        /// 销方税号
        /// </summary>
        public string taxNo { get; set; } = string.Empty;

        /// <summary>
        /// 开票流水号（唯一）
        /// </summary>
        public string serialNo { get; set; } = string.Empty;

        /// <summary>
        /// 业务订单号
        /// </summary>
        public string orderNo { get; set; } = string.Empty;

        /// <summary>
        /// 发票类型代码，全电：01专票 02普票
        /// </summary>
        public string invoiceTypeCode { get; set; } = string.Empty;

        /// <summary>
        /// 开票类型：0正数发票 1负数发票
        /// </summary>
        public string invoiceType { get; set; } = "0";

        /// <summary>
        /// 含税标志：0不含税 1含税
        /// </summary>
        public string priceTaxMark { get; set; } = "1";

        /// <summary>
        /// 购方名称
        /// </summary>
        public string buyerName { get; set; } = string.Empty;

        /// <summary>
        /// 购方税号
        /// </summary>
        public string? buyerTaxNo { get; set; }

        /// <summary>
        /// 购方地址电话
        /// </summary>
        public string? buyerAddressPhone { get; set; }

        /// <summary>
        /// 购方银行账号
        /// </summary>
        public string? buyerBankAccount { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string? remarks { get; set; }

        /// <summary>
        /// 开票明细
        /// </summary>
        public List<BaiWangInvoiceDetail> invoiceDetailList { get; set; } = new();

        public List<BaiWangBalanceInfoList> balanceInfoList { get; set; } = new();
    }

    /// <summary>
    /// 开票明细
    /// </summary>
    public class BaiWangInvoiceDetail
    {
        /// <summary>
        /// 商品名称
        /// </summary>
        public string goodsName { get; set; } = string.Empty;

        /// <summary>
        /// 税收分类编码
        /// </summary>
        public string goodsCode { get; set; } = string.Empty;

        /// <summary>
        /// 数量
        /// </summary>
        public string? goodsQuantity { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public string? goodsPrice { get; set; }

        /// <summary>
        /// 税率，如 0.06
        /// </summary>
        public string? goodsTaxRate { get; set; }

        /// <summary>
        /// 含税金额
        /// </summary>
        public string? goodsTotalPriceTax { get; set; }


        /// <summary>
        /// 不含税金额
        /// </summary>
        public string? goodsTotalPrice { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string? goodsUnit { get; set; }

        /// <summary>
        /// 规格型号
        /// </summary>
        public string? goodsSpecification { get; set; }

        /// <summary>
        /// 明细行号
        /// </summary>
        public string? goodsLineNo { get; set; }

        /// <summary>
        /// 税额
        /// </summary>
        public string? goodsTotalTax { get; set; }
    }

        /// <summary>
    /// 差额凭证信息(差额征税标签为02时必填)
        /// </summary>
    public class BaiWangBalanceInfoList
    {
        /// <summary>
        /// 凭证类型10-专用发票、11-普通发票 必填
        /// </summary>
        public string balanceType { get; set; } = string.Empty;

        /// <summary>
        /// 凭证合计金额(精确到小数点后2位) 必填
        /// </summary>
        public string balanceTotalAmount { get; set; } = string.Empty;

        /// <summary>
        /// 本次扣除金额(精确到小数点后2位) 必填
        /// </summary>
        public string? balanceDeductAmount { get; set; } = string.Empty;


    }

    /// <summary>
    /// 查询开票申请请求
    /// </summary>
    public class BaiWangQueryPreInvoiceRequest
    {
        /// <summary>
        /// 销方税号
        /// </summary>
        public string taxNo { get; set; } = string.Empty;

        /// <summary>
        /// 开票流水号
        /// </summary>
        public string? serialNo { get; set; }

        /// <summary>
        /// 业务订单号
        /// </summary>
        public string? orderNo { get; set; }
    }

    /// <summary>
    /// 查询发票请求
    /// </summary>
    public class BaiWangQueryInvoiceRequest
    {
        /// <summary>
        /// 销方税号
        /// </summary>
        public string taxNo { get; set; } = string.Empty;

        /// <summary>
        /// 开票流水号
        /// </summary>
        public string? serialNo { get; set; }

        /// <summary>
        /// 业务订单号
        /// </summary>
        public string? orderNo { get; set; }

        /// <summary>
        /// 发票代码
        /// </summary>
        public string? invoiceCode { get; set; }

        /// <summary>
        /// 发票号码
        /// </summary>
        public string? invoiceNo { get; set; }
    }

    /// <summary>
    /// 开具发票请求
    /// </summary>
    public class BaiWangIssueInvoiceRequest
    {
        /// <summary>
        /// 销方税号
        /// </summary>
        public string taxNo { get; set; } = string.Empty;

        /// <summary>
        /// 开票流水号（唯一）
        /// </summary>
        public string? serialNo { get; set; }

        /// <summary>
        /// 业务订单号
        /// </summary>
        public string? orderNo { get; set; }

        /// <summary>
        /// 订单日期(yyyy-MMdd HH:mm:ss)
        /// </summary>
        public string orderDateTime { get; set; } = string.Empty;

        /// <summary>
        /// 开票终端/数电账号
        /// </summary>
        public string invoiceTerminalCode { get; set; } = string.Empty;

        /// <summary>
        /// 发票类型代码，全电：01专票 02普票 026电子发票(普通发票) 028电子发票(增值税专用发票)
        /// </summary>
        public string invoiceTypeCode { get; set; } = string.Empty;

        /// <summary>
        /// 开票类型：0正数发票 1负数发票
        /// </summary>
        public string invoiceType { get; set; } = "0";

        /// <summary>
        /// 含税标志：0不含税 1含税
        /// </summary>
        public string priceTaxMark { get; set; } = "1";

        /// <summary>
        /// 购方名称
        /// </summary>
        public string buyerName { get; set; } = string.Empty;

        /// <summary>
        /// 购方税号
        /// </summary>
        public string? buyerTaxNo { get; set; }

        /// <summary>
        /// 购方地址
        /// </summary>
        public string? buyerAddress { get; set; }

        /// <summary>
        /// 购方电话
        /// </summary>
        public string? buyerTelephone { get; set; }

        /// <summary>
        /// 购方地址电话（部分接口使用合并字段）
        /// </summary>
        public string? buyerAddressPhone { get; set; }

        /// <summary>
        /// 购方开户行
        /// </summary>
        public string? buyerBankName { get; set; }

        /// <summary>
        /// 购方账号
        /// </summary>
        public string? buyerBankNumber { get; set; }

        /// <summary>
        /// 购方银行账号（部分接口使用合并字段）
        /// </summary>
        public string? buyerBankAccount { get; set; }

        /// <summary>
        /// 销方地址
        /// </summary>
        public string? sellerAddress { get; set; }

        /// <summary>
        /// 销方电话
        /// </summary>
        public string? sellerPhone { get; set; }

        /// <summary>
        /// 销方开户行
        /// </summary>
        public string? sellerBankName { get; set; }

        /// <summary>
        /// 销方账号
        /// </summary>
        public string? sellerBankNumber { get; set; }

        /// <summary>
        /// 开票人
        /// </summary>
        public string drawer { get; set; } = string.Empty;

        /// <summary>
        /// 复核人
        /// </summary>
        public string? checker { get; set; }

        /// <summary>
        /// 收款人
        /// </summary>
        public string? payee { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string? remarks { get; set; }

        /// <summary>
        /// 交付邮箱
        /// </summary>
        public string? pushEmail { get; set; }

        /// <summary>
        /// 交付手机
        /// </summary>
        public string? pushPhone { get; set; }

        /// <summary>
        /// 回调地址
        /// </summary>
        public string? callBackUrl { get; set; }

        /// <summary>
        /// 差额征税标签(数电差额票时必填):01-全额开票,02-差额开票
        /// </summary>
        public string? taxationLabel { get; set; }

        /// <summary>
        /// 开票明细
        /// </summary>
        public List<BaiWangInvoiceDetail> invoiceDetailList { get; set; } = new();

        public List<BaiWangBalanceInfoList> balanceInfoList { get; set; } = new();
    }

    /// <summary>
    /// 发票冲红请求
    /// </summary>
    public class BaiWangFastRedRequest
    {
        /// <summary>
        /// 销方税号 必填
        /// </summary>
        public string taxNo { get; set; } = string.Empty;

        /// <summary>
        /// 业务订单号 必填
        /// </summary>
        public string orderNo { get; set; } = string.Empty;

        /// <summary>
        /// 开票终端/数电账号  
        /// </summary>
        public string? invoiceTerminalCode { get; set; }

        /// <summary>
        /// 原蓝票请求流水号(申请流水号/开票单号，任选一项必填)
        /// </summary>
        public string? originalSerialNo { get; set; }

        /// <summary>
        /// 原蓝票开票单号(申请流水号/开票单号，任选一项必填)
        /// </summary>
        public string? originalOrderNo { get; set; }

        /// <summary>
        /// 原蓝票数电号码(数电票时必填)
        /// </summary>
        public string? originalDigitInvoiceNo { get; set; }

        /// <summary>
        /// 交付邮箱
        /// </summary>
        public string? pushEmail { get; set; }

        /// <summary>
        /// 交付手机
        /// </summary>
        public string? pushPhone { get; set; }

        /// <summary>
        /// 回调地址
        /// </summary>
        public string? callBackUrl { get; set; }
    }
    /// <summary>
    /// 发票冲红红票确认单请求
    /// </summary>
    public class BaiWangRedConfirmRequest
    {
        /// <summary>
        /// 销方税号 必填
        /// </summary>
        public string taxNo { get; set; } = string.Empty;

        /// <summary>
        /// 操作类型:1-从局端下载红字确认单,2-库里查询红字确认单
        /// </summary>
        public string operatorType { get; set; } = string.Empty;

        /// <summary>
        /// 操作方身份:0-销方,1-购方,2-二手车拍卖单位,3-二手车市场
        /// </summary>
        public string? buySelSelector { get; set; }

        /// <summary>
        /// 红字确认单流水号
        /// </summary>
        public string? redConfirmSerialNo { get; set; }

        /// <summary>
        /// 原蓝票数电号码(仅查询操作时有效)
        /// </summary>
        public string? originalDigitInvoiceNo { get; set; }

        /// <summary>
        /// 回调地址
        /// </summary>
        public string? callBackUrl { get; set; }
    }

}
