



 


using System;
using SqlSugar;
using LZ.Model.Enums;
namespace LZ.Model
{	

	/// <summary>
	/// BankTypeEnumType - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Finance")]
	public partial class Czy_BankApplication
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string AppName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string AppID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AppSignType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AppPrivateKey { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AppCharset { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AppFormat { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AppPulicKey { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AppEncryptType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AppEncryptKey { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// BankTypeEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  BankTypeEnumType BankType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid CompanyGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CompanyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsEnable { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsDelete { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? DeleteTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? UpdateTime { get; set; }
		   
    }



	/// <summary>
	/// CzyPayTypeEnumType - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Finance")]
	public partial class Czy_PayOrder
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string BillsPayOrderNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RelBillsCode { get; set; }
		
		/// <summary>
		/// CzyPayTypeEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  CzyPayTypeEnumType PayType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? PaymentDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BusinessTypeCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string BusinessTypeName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string PayOrgCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string PayOrgName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string PayAccountNo { get; set; }
		
		/// <summary>
		/// CzyPayChannelEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  CzyPayChannelEnumType PayChannel { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal PayAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PayCurrencyType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PayCurrencyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool WhetherUrgent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string PayUse { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CreateByCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CreateByName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Purpose { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ReturnCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ReturnMsg { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ReturnFailReasons { get; set; }
		
		/// <summary>
		/// CzyPushStatusEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  CzyPushStatusEnumType PushStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? PushTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? TransStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PayStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PayStatusName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FailReasons { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? RelPaymentDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? RealPayAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? PayableAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? RefundAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? RefundDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? RealPayType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SerialNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PayFailReason { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TransInfoNoList { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Finance")]
	public partial class Czy_PayOrderDetail
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid PayOrderID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string BillsPayOrderNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal PrivateAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string ReceiveAccountNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string ReceiveAccountName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ReceiveAreaNameOfProvince { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ReceiveAreaNameOfCity { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ReceiveBranchCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ReceiveBranchName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ReceiveNature { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ReceiveCurrencyCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ReceiveCurrencyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OrderNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PayStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PayStatusName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FailReasons { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RelPaymentDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? PayAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? RefundAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RefundDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PayFailReason { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PayUse { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Purpose { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TransInfoNoList { get; set; }
		   
    }



	/// <summary>
	/// 发票是全额01还是差额02 - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Finance")]
	public partial class FIN_InvoiceConfig
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CompanyGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? RecipientUnitID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RecipientName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? IssuerUnitID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IssuerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Title { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Type { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TypeName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsEnabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PartnerID { get; set; }
		   
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PartnerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? PartnerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? DetailID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string DetailName { get; set; }
		
		/// <summary>
		/// 发票是全额01还是差额02
		/// </summary>		
		
		public  string TaxationLabel { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TaxationLabelName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? GoodsTaxRate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Phone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Email { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Content { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Finance")]
	public partial class FIN_InvoiceCustomer
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CompanyGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string LegalPerson { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string LegalPersonCardID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Phone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BankName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsEnabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Address { get; set; }
		   
    }



	/// <summary>
	/// 销方税号 - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Finance")]
	public partial class FIN_InvoiceInfo
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CompanyGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OrderNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? OrderDateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SerialNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? PartnerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EditTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AutoInvoiceMark { get; set; }
		
		/// <summary>
		/// 销方税号
		/// </summary>		
		
		public  string TaxNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SellerAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SellerPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SellerBankName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SellerBankNumber { get; set; }
		
		/// <summary>
		/// 单据类型:1-正数申请单,2-负数申请单
		/// </summary>		
		
		public  string OrderType { get; set; }
		
		/// <summary>
		/// 申请单号重复:0-报错(默认),1-覆盖
		/// </summary>		
		
		public  string OrderNoDuplicationProcessMethod { get; set; }
		
		/// <summary>
		/// 申请单号
		/// </summary>		
		
		public  string ApplyNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InvoiceTerminalCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InvoiceTypeCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InvoiceTypeCodeName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BuyerTaxNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BuyerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BuyerBankName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BuyerBankNumber { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BuyerAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BuyerTelephone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? TotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? GoodsID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string GoodsName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string GoodsCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? GoodsQuantity { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? GoodsPrice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? GoodsTaxRate { get; set; }
		
		/// <summary>
		/// 不含税金额(
		/// </summary>		
		
		public  decimal? GoodsTotalPrice { get; set; }
		
		/// <summary>
		/// 含税金额
		/// </summary>		
		
		public  decimal? GoodsTotalPriceTax { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? GoodsTotalTax { get; set; }
		
		/// <summary>
		/// 扣除额
		/// </summary>		
		
		public  decimal? DeductibleAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? InvoiceType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PriceTaxMark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Drawer { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PushEmail { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PushPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? IssuerUnitID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IssuerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? RecipientUnitID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RecipientName { get; set; }
		
		/// <summary>
		/// 是否含税
		/// </summary>		
		
		public  bool IsTaxPrice { get; set; }
		   
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TaxationLabel { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TaxationLabelName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Content { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RedOrderNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RedSerialNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RedConfirmSerialNo { get; set; }
		
		/// <summary>
		/// 数电发票号码
		/// </summary>		
		
		public  string DigitInvoiceNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RedDigitInvoiceNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Msg { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InvoiceBaseStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RedInvoiceBaseStatus { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Finance")]
	public partial class FIN_InvoiceOrderDetail
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CompanyGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string GoodsName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? GoodsTaxRate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string GoodsCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Type { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsEnabled { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Finance")]
	public partial class FIN_InvoicePartnerRelation
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? RecipientID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RecipientName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsEnabled { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Finance")]
	public partial class FIN_Reimb__BudgetApplyRecord
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CompanyGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Year { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ExpensesTitle { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CostType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ReceivePaymentCompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? PartnerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? UserGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SysDepartmentName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ApplyAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal AvailableAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? BudgetManageID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ReceivePaymentCompanyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PartnerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Description { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Bank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PayType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? PayerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Payer { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProcessStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Message { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? RollBackBudgetManageRecordID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? PayTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? RechargeLogID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SalaryGID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Finance")]
	public partial class FIN_Reimb__BudgetApplyRecordDetail
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? BudgetApplyRecordID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal Money { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? PartnerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PartnerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CompanyID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Finance")]
	public partial class FIN_Reimb__BudgetManage
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CompanyGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Year { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? PartnerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CostType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? GenerateType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ExpensesTitle { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal Percentage { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal LockAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UsedAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal AvailableAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal MonthTargetAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal HeadOfficeUsedAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool DirectlyPass { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Hidden { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool PublicUse { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Finance")]
	public partial class FIN_Reimb_Apply
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true)]
		public  Guid ApplyGuid { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsIdentity=true)]
		public  long Id { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int CompanyId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string ApplyNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  byte ApplyType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  byte FlowType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid ConfigGuid { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ApplicantId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string ApplicantName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? ApplicantDeptId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ApplicantDeptName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int LegalEntityId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string LegalEntityName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int CostTypeId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CostTypeName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string ExpenseName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string PayeeName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string PayeeBankName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  byte[] PayeeBankCardNoEncrypted { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string PayeeBankCardNoMasked { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string ExpensePurpose { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ApplyAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int InvoiceCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal InvoiceTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsBudgetPassed { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  byte ApplyStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  byte InvoiceSupplementStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CurrentApprovalInstanceGuid { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string LastRejectReason { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? LastRejectTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? SubmittedAt { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int CreatedBy { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreatedAt { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UpdatedBy { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? UpdatedAt { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  byte[] RowVersion { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Finance")]
	public partial class FIN_Reimb_ApplyInvoice
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true)]
		public  Guid RelGuid { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsIdentity=true)]
		public  long Id { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int CompanyId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid ApplyGuid { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid InvoiceGuid { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  byte RelationPurpose { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsActive { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UnlinkReason { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? RelatedAt { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Finance")]
	public partial class FIN_Reimb_ApprovalInstance
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true)]
		public  Guid InstanceGuid { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsIdentity=true)]
		public  long Id { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int CompanyId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid ApplyGuid { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ApplicantId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid ProcessGuid { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ProcessVersionNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int RoundNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CurrentNodeGuid { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CurrentNodeSeq { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  byte InstanceStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? StartedAt { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EndedAt { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  byte[] RowVersion { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Finance")]
	public partial class FIN_Reimb_ApprovalNode
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true)]
		public  Guid NodeGuid { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsIdentity=true)]
		public  long Id { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int CompanyId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid ProcessGuid { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int NodeSeq { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string NodeName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid ApproverDeptId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string ApproverDeptName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ApproverUserId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string ApproverUserName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ApproverRoleName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool AmountConditionEnabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string AmountOperator { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal AmountThreshold { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsEnabled { get; set; }
		   
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreatedAt { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Finance")]
	public partial class FIN_Reimb_ApprovalProcess
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true)]
		public  Guid ProcessGuid { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsIdentity=true)]
		public  long Id { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int CompanyId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string ProcessName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int VersionNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Description { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsActive { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int CreatedBy { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreatedAt { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? PublishedAt { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  byte[] RowVersion { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Finance")]
	public partial class FIN_Reimb_ApprovalRecord
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true)]
		public  Guid RecordGuid { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsIdentity=true)]
		public  long Id { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int CompanyId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid InstanceGuid { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid ApplyGuid { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid NodeGuid { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int NodeSeq { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string NodeName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int OperatorId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string OperatorName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OperatorDeptName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  byte ActionType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RejectReason { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Comment { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? OperatedAt { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Finance")]
	public partial class FIN_Reimb_BudgetConfig
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid GID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CompanyGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Title { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CostType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? GenerateType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal Percentage { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Enable { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? PartnerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool DirectlyPass { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool PublicUse { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsRelate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PartnerIDS { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PartnerNames { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Finance")]
	public partial class FIN_Reimb_BudgetReservation
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true)]
		public  Guid ReservationGuid { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsIdentity=true)]
		public  long Id { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int CompanyId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid ApplyGuid { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CostTypeName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ExternalBudgetId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string ExternalRequestNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  byte CheckResult { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  byte ReservationStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? AvailableBefore { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ReservedAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ResponseSnapshot { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreatedAt { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? UpdatedAt { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  byte[] RowVersion { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Finance")]
	public partial class FIN_Reimb_InvoicePool
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true)]
		public  Guid InvoiceGuid { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsIdentity=true)]
		public  long Id { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int CompanyId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string InvoiceNumber { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid SourceUploadGuid { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? VerifiedAt { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreatedAt { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Finance")]
	public partial class FIN_Reimb_InvoiceUpload
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true)]
		public  Guid UploadGuid { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsIdentity=true)]
		public  long Id { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int CompanyId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? ApplyGuid { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  byte UploadPurpose { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string FileUrl { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string FileType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OriginalFileName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FileMd5 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InvoiceNumber { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? TotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  byte RecognitionStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  byte VerifyStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  byte DuplicateCheckStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsEffective { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FailureCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FailureMessage { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ProviderRequestId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RawResultJson { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int UploadUserId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? UploadedAt { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CallbackAt { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  byte[] RowVersion { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Finance")]
	public partial class FIN_Reimb_PaymentException
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true)]
		public  Guid ExceptionGuid { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsIdentity=true)]
		public  long Id { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int CompanyId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid ApplyGuid { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RequiredInvoiceAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal VerifiedInvoiceAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  byte ExceptionStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int SupplementCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? LastSupplementTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreatedAt { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? LastEvaluatedAt { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? ClosedAt { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  byte[] RowVersion { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Finance")]
	public partial class FIN_Reimb_PaymentOrder
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true)]
		public  Guid OrderGuid { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsIdentity=true)]
		public  long Id { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int CompanyId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string PaymentNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid ApplyGuid { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal PayAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  byte PaymentType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BatchNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  byte PayStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string PayeeName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string PayeeBankName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string PayeeBankCardNoMasked { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PaidBy { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BankSerialNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ReceiptUrl { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string IdempotencyKey { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? PaidAt { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FailureReason { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreatedAt { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? UpdatedAt { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  byte[] RowVersion { get; set; }
		   
    }



}


