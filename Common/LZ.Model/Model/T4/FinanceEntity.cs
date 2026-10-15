



 


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
	///  - 实体层
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
		
		public  string DetailIDS { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string DetailNames { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
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
		
		public  int? PartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PartnerName { get; set; }
		
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
		
		public  Guid? RecipientUnitID { get; set; }
		
		/// <summary>
		/// 是否含税
		/// </summary>		
		
		public  bool IsTaxPrice { get; set; }
		   
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



}


