



 


using System;
using SqlSugar;
using LZ.Model.Enums;
namespace LZ.Model
{	

	/// <summary>
	/// 位置 - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_Advert
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CompanyGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CompanyID { get; set; }
		
		/// <summary>
		/// 位置
		/// </summary>		
		
		public  string Location { get; set; }
		
		/// <summary>
		/// 排序
		/// </summary>		
		
		public  int? Sort { get; set; }
		
		/// <summary>
		/// 显示方式
		/// </summary>		
		
		public  string ShowMethod { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Jsons { get; set; }
		
		/// <summary>
		/// 跳转的页面
		/// </summary>		
		
		public  string Page { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 是否显示按钮
		/// </summary>		
		
		public  bool IsButton { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Picture { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Enabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Action { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ActionParas { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_Advertisement
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
		
		public  string AdName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Location { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JumpType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JumpParas { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? FromGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Picture { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ShopID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? ShopGID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_Attribute
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int AttributeID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AttributeName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? TypeID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Type { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsImage { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProductID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Sort { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AttributeStrID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsDelete { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_AttributeValues
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ValueID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? AttributeID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Image { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ValueStrID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AttributeStrID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsDelete { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_Brand
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int BrandID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BrandName { get; set; }
		
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
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Picture { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsBrand { get; set; }
		   
    }



	/// <summary>
	/// 分类名称 - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_Categories
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int CateID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CompanyGID { get; set; }
		
		/// <summary>
		/// 分类名称
		/// </summary>		
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 父类ID
		/// </summary>		
		
		public  int? ParentID { get; set; }
		
		/// <summary>
		/// 分类类型
		/// </summary>		
		
		public  int? CateType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string MetaTitle { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string MetaKeywords { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string MetaDescription { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RewriteName { get; set; }
		
		/// <summary>
		/// 小图片
		/// </summary>		
		
		public  string SmallPicture { get; set; }
		
		/// <summary>
		/// 大图片
		/// </summary>		
		
		public  string BigPicture { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Prefix { get; set; }
		
		/// <summary>
		/// 是否有子类
		/// </summary>		
		
		public  bool HasChildren { get; set; }
		
		/// <summary>
		/// 分类说明
		/// </summary>		
		
		public  string Description { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 是否可用
		/// </summary>		
		
		public  bool Enabled { get; set; }
		
		/// <summary>
		/// 排序
		/// </summary>		
		
		public  int? Sort { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsVirtual { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_Channel
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ChannelID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ChannelName { get; set; }
		
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
		
		public  int? ShowType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PageID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		   
    }



	/// <summary>
	/// 评分 - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_Comment
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? UserGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProductID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SkuID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OrderID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 评分
		/// </summary>		
		
		public  int? Score { get; set; }
		
		/// <summary>
		/// 图片
		/// </summary>		
		
		public  string Pictures { get; set; }
		
		/// <summary>
		/// 评价
		/// </summary>		
		
		public  string Comment { get; set; }
		
		/// <summary>
		/// 点赞数
		/// </summary>		
		
		public  int? LikeCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SkuContent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FullName { get; set; }
		
		/// <summary>
		/// 是否匿名
		/// </summary>		
		
		public  bool IsAno { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserAvatar { get; set; }
		   
    }



	/// <summary>
	/// 优惠券类型 - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_Coupon
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int CouponID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CouponGID { get; set; }
		
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
		
		public  int? ShopID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? ShopGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CouponName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CouponPicture { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 优惠券类型
		/// </summary>		
		
		public  int? CouponType { get; set; }
		
		/// <summary>
		/// 抵扣金额
		/// </summary>		
		
		public  decimal CouponMoney { get; set; }
		
		/// <summary>
		/// 折扣
		/// </summary>		
		
		public  decimal? CouponDiscount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProductID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SkuID { get; set; }
		
		/// <summary>
		/// 兑换产品数量
		/// </summary>		
		
		public  int? ProductCount { get; set; }
		
		/// <summary>
		/// 状态
		/// </summary>		
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? StartTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EndTime { get; set; }
		
		/// <summary>
		/// 优惠券数量
		/// </summary>		
		
		public  int? CouponCount { get; set; }
		
		/// <summary>
		/// 用户获得优惠券的数量限制
		/// </summary>		
		
		public  int? UserLimitCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal LimitMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? LimitNumber { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UseCouponCount { get; set; }
		
		/// <summary>
		/// 使用限制 1为平台优惠券，2为店铺优惠券，3为商品优惠券
		/// </summary>		
		
		public  int? UseLimit { get; set; }
		
		/// <summary>
		/// group团购product商品
		/// </summary>		
		
		public  string CouponSetType { get; set; }
		
		/// <summary>
		/// 领取时间
		/// </summary>		
		
		public  DateTime? ReceiveStartTime { get; set; }
		
		/// <summary>
		/// 领取时间
		/// </summary>		
		
		public  DateTime? ReceiveEndTime { get; set; }
		
		/// <summary>
		/// 是否可叠加使用
		/// </summary>		
		
		public  bool IsStack { get; set; }
		
		/// <summary>
		/// 是否公开
		/// </summary>		
		
		public  bool IsOpen { get; set; }
		
		/// <summary>
		/// 优惠券使用类型，1为商品通用券2为指定商品券
		/// </summary>		
		
		public  int? CouponUseType { get; set; }
		   
    }



	/// <summary>
	/// 优惠券编号 - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_CouponItems
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 优惠券编号
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CouponNumber { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? GID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CouponID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CouponGID { get; set; }
		
		/// <summary>
		/// 兑换人
		/// </summary>		
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CouponName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 优惠券类型
		/// </summary>		
		
		public  int? CouponType { get; set; }
		
		/// <summary>
		/// 抵扣金额
		/// </summary>		
		
		public  decimal CouponMoney { get; set; }
		
		/// <summary>
		/// 折扣
		/// </summary>		
		
		public  decimal? CouponDiscount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProductID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SkuID { get; set; }
		
		/// <summary>
		/// 可兑换商品数量
		/// </summary>		
		
		public  int? ProductCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 绑定时间
		/// </summary>		
		
		public  DateTime? ExchangeTime { get; set; }
		
		/// <summary>
		/// 兑换人姓名
		/// </summary>		
		
		public  string FullName { get; set; }
		
		/// <summary>
		/// 核销人
		/// </summary>		
		
		public  int? WriteUserID { get; set; }
		
		/// <summary>
		/// 核销时间
		/// </summary>		
		
		public  DateTime? WriteTime { get; set; }
		
		/// <summary>
		/// 核销人姓名
		/// </summary>		
		
		public  string WriteFullName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OrderID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_CouponProduct
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CouponID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CouponGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProductID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SkuID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_Express
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ExpressName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ExpressType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ErpCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WeiXinCode { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_ExpressInfo
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ExpressNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string LogisticsStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SubLogisticsStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AreaCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AreaName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Desc { get; set; }
		   
    }



	/// <summary>
	/// 是否包邮 - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_FreightTemplate
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Name { get; set; }
		
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
		
		public  int? ShopID { get; set; }
		
		/// <summary>
		/// 是否包邮
		/// </summary>		
		
		public  bool IsFree { get; set; }
		
		/// <summary>
		/// 发货时间
		/// </summary>		
		
		public  int? SendHour { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Province { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string City { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string District { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Street { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProvinceID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CityID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? DistrictID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? StreetID { get; set; }
		
		/// <summary>
		/// 计价方式
		/// </summary>		
		
		public  int? PricingType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		   
    }



	/// <summary>
	/// 来源类型或消费类型 - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_Integral
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? FromUserID { get; set; }
		
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
		
		public  int? Integral { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 来源类型或消费类型
		/// </summary>		
		
		public  int? IntegralType { get; set; }
		
		/// <summary>
		/// 来源的id，岗位
		/// </summary>		
		
		public  int? FromID { get; set; }
		   
    }



	/// <summary>
	/// 来源类型或消费类型 - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_IntegralLog
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Integral { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OrderID { get; set; }
		
		/// <summary>
		/// 来源类型或消费类型
		/// </summary>		
		
		public  int? IntegralType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
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
		
		public  int? FromUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? FromJobID { get; set; }
		
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
		
		public  int? ToLZUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FullName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RefundID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PartnerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? NowIntegral { get; set; }
		   
    }



	/// <summary>
	/// 服务商ID - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_Order
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
		public  string OrderID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? OrderGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CompanyGID { get; set; }
		
		/// <summary>
		/// 服务商ID
		/// </summary>		
		
		public  int? PartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? PartnerGID { get; set; }
		
		/// <summary>
		/// 订单备注
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 订单状态
		/// </summary>		
		
		public  int? OrderStatus { get; set; }
		
		/// <summary>
		/// 下单时间
		/// </summary>		
		
		public  DateTime? OrderDate { get; set; }
		
		/// <summary>
		/// 更新时间
		/// </summary>		
		
		public  DateTime? UpdateDate { get; set; }
		
		/// <summary>
		/// 支付时间
		/// </summary>		
		
		public  DateTime? PayDate { get; set; }
		
		/// <summary>
		/// 发货时间
		/// </summary>		
		
		public  DateTime? ShippingDate { get; set; }
		
		/// <summary>
		/// 交易完成时间
		/// </summary>		
		
		public  DateTime? FinishDate { get; set; }
		
		/// <summary>
		/// 用户编号
		/// </summary>		
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 订单总金额
		/// </summary>		
		
		public  decimal OrderTotal { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal ProductTotal { get; set; }
		
		/// <summary>
		/// 支付方式名称
		/// </summary>		
		
		public  string PayType { get; set; }
		
		/// <summary>
		/// 支付编号
		/// </summary>		
		
		public  string PayID { get; set; }
		
		/// <summary>
		/// 订单类型
		/// </summary>		
		
		public  int? OrderType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProductID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ProductName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Address { get; set; }
		
		/// <summary>
		/// skuid
		/// </summary>		
		
		public  string SkuID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Tags { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SkuContent { get; set; }
		
		/// <summary>
		/// 收货人
		/// </summary>		
		
		public  string ShippName { get; set; }
		
		/// <summary>
		/// 收获人手机号
		/// </summary>		
		
		public  string ShippPhone { get; set; }
		
		/// <summary>
		/// 商品价格
		/// </summary>		
		
		public  decimal ProductPrice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ProductPicture { get; set; }
		
		/// <summary>
		/// 收货地址
		/// </summary>		
		
		public  string AddressName { get; set; }
		
		/// <summary>
		/// 产品的销售价
		/// </summary>		
		
		public  decimal SalePrice { get; set; }
		
		/// <summary>
		/// 退款单号
		/// </summary>		
		
		public  string RefundID { get; set; }
		
		/// <summary>
		/// 退款金额
		/// </summary>		
		
		public  decimal RefundMoney { get; set; }
		
		/// <summary>
		/// 退款时间
		/// </summary>		
		
		public  DateTime? RefundDate { get; set; }
		
		/// <summary>
		/// 退款备注
		/// </summary>		
		
		public  string RefundAbout { get; set; }
		
		/// <summary>
		/// 商品数量
		/// </summary>		
		
		public  int? ProductCount { get; set; }
		
		/// <summary>
		/// 商品类型
		/// </summary>		
		
		public  int? ProductType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ShopID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Integral { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal Cash { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ClientID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ExpressNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ExpressType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ExpressName { get; set; }
		
		/// <summary>
		/// 是否包邮
		/// </summary>		
		
		public  bool IsFree { get; set; }
		
		/// <summary>
		/// 运费模板
		/// </summary>		
		
		public  int? FTID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FullName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserTaskID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? TaskID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? TaskPreferentialType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal PreferentialMoney { get; set; }
		
		/// <summary>
		/// 下单类型
		/// </summary>		
		
		public  int? PlaceOrderType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Province { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string City { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string District { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SellerRemark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BuyerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? TaskType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? TaskMethod { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool HasRefund { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? RefundIntegra { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal FeeMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OpenID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PersonnelUID { get; set; }
		
		/// <summary>
		/// 核销人
		/// </summary>		
		
		public  int? WriteUserID { get; set; }
		
		/// <summary>
		/// 核销时间
		/// </summary>		
		
		public  DateTime? WriteTime { get; set; }
		
		/// <summary>
		/// 核销人姓名
		/// </summary>		
		
		public  string WriteFullName { get; set; }
		
		/// <summary>
		/// 优惠金额
		/// </summary>		
		
		public  decimal CouponTotal { get; set; }
		
		/// <summary>
		/// 物流状态
		/// </summary>		
		
		public  string ExpressStatus { get; set; }
		
		/// <summary>
		/// 最新物流信息
		/// </summary>		
		
		public  string ExpressNewInfo { get; set; }
		
		/// <summary>
		/// 是否评价
		/// </summary>		
		
		public  bool IsAssess { get; set; }
		
		/// <summary>
		/// 售后状态与退单状态一致
		/// </summary>		
		
		public  int? RefundStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsPackage { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? Commission { get; set; }
		
		/// <summary>
		/// 团购商品是否可以邮寄
		/// </summary>		
		
		public  bool IsGroupExpress { get; set; }
		
		/// <summary>
		/// 支付单号
		/// </summary>		
		
		public  string OrderPayID { get; set; }
		   
    }



	/// <summary>
	/// 优惠金额 - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_OrderItems
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? OrderItemGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OrderID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SkuID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProductID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SKU { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal ProductPrice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SkuContent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProductCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal TotalMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal RefundAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsRefund { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal CostPrice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal Weight { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ProductName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? RefundCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal SalePrice { get; set; }
		
		/// <summary>
		/// 优惠金额
		/// </summary>		
		
		public  decimal CouponMoney { get; set; }
		
		/// <summary>
		/// 核销人
		/// </summary>		
		
		public  int? WriteUserID { get; set; }
		
		/// <summary>
		/// 核销时间
		/// </summary>		
		
		public  DateTime? WriteTime { get; set; }
		
		/// <summary>
		/// 核销人姓名
		/// </summary>		
		
		public  string WriteFullName { get; set; }
		
		/// <summary>
		/// 团购券券码
		/// </summary>		
		
		public  string CouponNumber { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal PayCash { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PayIntegral { get; set; }
		
		/// <summary>
		/// 退款单号
		/// </summary>		
		
		public  string RefundID { get; set; }
		
		/// <summary>
		/// 退款金额
		/// </summary>		
		
		public  decimal RefundMoney { get; set; }
		
		/// <summary>
		/// 退款时间
		/// </summary>		
		
		public  DateTime? RefundDate { get; set; }
		
		/// <summary>
		/// 退款备注
		/// </summary>		
		
		public  string RefundAbout { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? RefundIntegral { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? Commission { get; set; }
		   
    }



	/// <summary>
	/// 1为订单2为退单 - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_OrderLog
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OrderID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Picture { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ToUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string StatusName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Title { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Type { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? SysUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FullName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SysFullName { get; set; }
		
		/// <summary>
		/// 1为订单2为退单
		/// </summary>		
		
		public  int? LogType { get; set; }
		   
    }



	/// <summary>
	/// 物流状态 - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_OrderPackage
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? PackageGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OrderID { get; set; }
		
		/// <summary>
		/// 物流状态
		/// </summary>		
		
		public  string ExpressStatus { get; set; }
		
		/// <summary>
		/// 最新物流信息
		/// </summary>		
		
		public  string ExpressNewInfo { get; set; }
		
		/// <summary>
		/// 快递单号
		/// </summary>		
		
		public  string ExpressNo { get; set; }
		
		/// <summary>
		/// 快递商编号
		/// </summary>		
		
		public  string ExpressType { get; set; }
		
		/// <summary>
		/// 快递商名称
		/// </summary>		
		
		public  string ExpressName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_OrderPackageItems
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? PackageGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? OrderItemID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Count { get; set; }
		   
    }



	/// <summary>
	/// 1为退款2为退货3为换货 - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_OrderReturn
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ReturnID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OrderID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ReturnPayType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal ReturnTotal { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal ReturnMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ReturnIntegral { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? ReturnDate { get; set; }
		
		/// <summary>
		/// 1为退款2为退货3为换货
		/// </summary>		
		
		public  int? ReturnType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ReturnReason { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AuditReason { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ExpressNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ExpressType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ExpressName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ReceiverProvince { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ReceiverCity { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ReceiverDistrict { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ReceiverMobile { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ReceiverAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ReceiverName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Pictures { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PayRefundID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CompanyGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal RefundCash { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? RefundIntegral { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ReturnAddressID { get; set; }
		   
    }



	/// <summary>
	/// 退款金额 - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_OrderReturnItems
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ReturnID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? OrderItemID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProductID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SkuID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SKU { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal ProductPrice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProductCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SkuContent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ProductName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ChangeProductID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ChangeSkuID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ChangeSKU { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ChangeProductCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal ChangeProductPrice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ChangeSkuContent { get; set; }
		
		/// <summary>
		/// 退款金额
		/// </summary>		
		
		public  decimal RefundMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal RefundCash { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? RefundIntegral { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_PictureAssort
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int AssortID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? AssortGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ParentID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? ParentGID { get; set; }
		
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
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AssortName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ShopID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_Pictures
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? AssortID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ShopID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Picture { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		   
    }



	/// <summary>
	/// 分类 - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_Product
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ProductID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CompanyGID { get; set; }
		
		/// <summary>
		/// 分类
		/// </summary>		
		
		public  int? CateID { get; set; }
		
		/// <summary>
		/// 产品名称
		/// </summary>		
		
		public  string ProductName { get; set; }
		
		/// <summary>
		/// 产品类型
		/// </summary>		
		
		public  int? ProductType { get; set; }
		
		/// <summary>
		/// 状态
		/// </summary>		
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 商品详情
		/// </summary>		
		
		public  string ProductContent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 销售数量
		/// </summary>		
		
		public  int? SaleCount { get; set; }
		
		/// <summary>
		/// 库存数量
		/// </summary>		
		
		public  int? ProductCount { get; set; }
		
		/// <summary>
		/// 是否开启sku
		/// </summary>		
		
		public  bool HasSKU { get; set; }
		
		/// <summary>
		/// 排序
		/// </summary>		
		
		public  int? Sort { get; set; }
		
		/// <summary>
		/// 列表图片
		/// </summary>		
		
		public  string Picture { get; set; }
		
		/// <summary>
		/// 详情图片
		/// </summary>		
		
		public  string ShowPicture { get; set; }
		
		/// <summary>
		/// 商品说明
		/// </summary>		
		
		public  string Description { get; set; }
		
		/// <summary>
		/// 单位
		/// </summary>		
		
		public  string Unit { get; set; }
		
		/// <summary>
		/// 产品编号
		/// </summary>		
		
		public  string ProductCode { get; set; }
		
		/// <summary>
		/// 商品价格
		/// </summary>		
		
		public  decimal ProductPrice { get; set; }
		
		/// <summary>
		/// 成本价
		/// </summary>		
		
		public  decimal CostPrice { get; set; }
		
		/// <summary>
		/// 售价
		/// </summary>		
		
		public  decimal MarketPrice { get; set; }
		
		/// <summary>
		/// 首页图片
		/// </summary>		
		
		public  string HomePicture { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TextIcon { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IconColor { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ShopID { get; set; }
		
		/// <summary>
		/// 运费模板
		/// </summary>		
		
		public  int? FTID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? BrandID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ProductTabs { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? Weight { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Barcode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string VideoFile { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsIntegral { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsVirtual { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? StartTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EndTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Tabs { get; set; }
		
		/// <summary>
		/// 商品简介
		/// </summary>		
		
		public  string Introduction { get; set; }
		
		/// <summary>
		/// 用于数据标记
		/// </summary>		
		
		public  string DataTabs { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal PromotionPrice { get; set; }
		
		/// <summary>
		/// 展示数据使用
		/// </summary>		
		
		public  string CouponName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PromotionSkuID { get; set; }
		
		/// <summary>
		/// 用户获取限制
		/// </summary>		
		
		public  int? UserLimitCount { get; set; }
		
		/// <summary>
		/// 代金券金额
		/// </summary>		
		
		public  decimal CouponMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CateIDS { get; set; }
		
		/// <summary>
		/// 佣金
		/// </summary>		
		
		public  decimal? Commission { get; set; }
		
		/// <summary>
		/// 是否开启佣金
		/// </summary>		
		
		public  bool IsCommission { get; set; }
		
		/// <summary>
		/// 团购商品是否可以邮寄
		/// </summary>		
		
		public  bool IsGroupExpress { get; set; }
		   
    }



	/// <summary>
	/// 虚拟产品的编号属于哪家平台 - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_ProductItems
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProductID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Number { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? StartTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EndTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 虚拟产品的编号属于哪家平台
		/// </summary>		
		
		public  string Platform { get; set; }
		   
    }



	/// <summary>
	/// 是福利还是任务 - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_ProductTask
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int TaskID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TaskName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProductID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SkuID { get; set; }
		
		/// <summary>
		/// 是福利还是任务
		/// </summary>		
		
		public  int? TaskType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProductCount { get; set; }
		
		/// <summary>
		/// 已被抢的数量
		/// </summary>		
		
		public  int? UseCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 任务方式
		/// </summary>		
		
		public  int? TaskMethod { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? StartTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EndTime { get; set; }
		
		/// <summary>
		/// 完成任务的次数
		/// </summary>		
		
		public  int? ContentCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SkuContent { get; set; }
		
		/// <summary>
		/// 优惠方式
		/// </summary>		
		
		public  int? PreferentialType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal PreferentialMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? JobID { get; set; }
		
		/// <summary>
		/// 任何岗位，指定岗位
		/// </summary>		
		
		public  int? ShareJobType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CompanyGID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_ProductTaskItems
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? TaskID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProductID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SkuID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SkuContent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Integral { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? RewardType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProductCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Picture { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Weight { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UseCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Sort { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_ProductTypes
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int TypeID { get; set; }
		
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
		
		public  string TypeName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string About { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CancelAbout { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Type { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Note { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsUpImage { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ImageCount { get; set; }
		   
    }



	/// <summary>
	/// 已完成数量 - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_ProductUserTask
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? TaskID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? StartTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EndTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 已完成数量
		/// </summary>		
		
		public  int? HaveCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 原因
		/// </summary>		
		
		public  string Reason { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProductID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ProductName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SkuID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SkuContent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProductCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? TaskItemID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Integral { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FullName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Picture { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_ProductUserTaskItems
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? FromID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserTaskID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? TaskMethod { get; set; }
		   
    }



	/// <summary>
	/// 设置方式1为打折2为促销价 - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_Promotion
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int PromotionID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? PromotionGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PromotionName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? StartTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EndTime { get; set; }
		
		/// <summary>
		/// 设置方式1为打折2为促销价
		/// </summary>		
		
		public  int? PromotionType { get; set; }
		
		/// <summary>
		/// 是否启用
		/// </summary>		
		
		public  bool Enable { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ShopID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? ShopGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CompanyGID { get; set; }
		   
    }



	/// <summary>
	/// 是否启用 - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_PromotionProduct
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PromotionID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? PromotionGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProductID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SkuID { get; set; }
		
		/// <summary>
		/// 是否启用
		/// </summary>		
		
		public  bool Enable { get; set; }
		
		/// <summary>
		/// 限购数量
		/// </summary>		
		
		public  int? LimitCount { get; set; }
		
		/// <summary>
		/// 促销价
		/// </summary>		
		
		public  decimal PromotionMoney { get; set; }
		
		/// <summary>
		/// 折扣
		/// </summary>		
		
		public  decimal? PromotionDiscount { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_PromotionTask
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CompanyGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? StartTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EndTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Enabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal TargetMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal HaveMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? TaskType { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_ReturnAddress
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
		
		public  int? ShopID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Province { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string City { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string District { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Address { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Contact { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Phone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		   
    }



	/// <summary>
	/// 输出数据使用 - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_ShoppingCart
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int CartID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProductID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProductCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SkuID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SKU { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SkuContent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal PromotionPrice { get; set; }
		
		/// <summary>
		/// 输出数据使用
		/// </summary>		
		
		public  string CouponName { get; set; }
		
		/// <summary>
		/// 输出数据使用
		/// </summary>		
		
		public  Guid? CouponGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsCheck { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_SKUItems
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true)]
		public  string SkuID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int AttributeID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ValueID { get; set; }
		   
    }



	/// <summary>
	/// 重量 - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_SKUs
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true)]
		public  string SkuID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProductID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SKU { get; set; }
		
		/// <summary>
		/// 重量
		/// </summary>		
		
		public  decimal? Weight { get; set; }
		
		/// <summary>
		/// 库存
		/// </summary>		
		
		public  int? Stock { get; set; }
		
		/// <summary>
		/// 库存警戒线
		/// </summary>		
		
		public  int? WarningStock { get; set; }
		
		/// <summary>
		/// 成本价
		/// </summary>		
		
		public  decimal CostPrice { get; set; }
		
		/// <summary>
		/// 售价
		/// </summary>		
		
		public  decimal SalePrice { get; set; }
		
		/// <summary>
		/// 冻结库存
		/// </summary>		
		
		public  int? FreezeStock { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Barcode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Picture { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Sort { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Enabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal PromotionPrice { get; set; }
		
		/// <summary>
		/// 用于数据标记
		/// </summary>		
		
		public  string DataTabs { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_UserAddress
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Phone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Province { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string City { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string District { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Address { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsDefault { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		   
    }



	/// <summary>
	/// 营业时间 - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_UserShop
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ShopID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? ShopGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ShopName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserID { get; set; }
		
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
		
		public  DateTime? EndTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ShopLevel { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal Money { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ShopType { get; set; }
		
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
		
		public  string Address { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  double? Latitude { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  double? Longitude { get; set; }
		
		/// <summary>
		/// 营业时间
		/// </summary>		
		
		public  string BusinessTime { get; set; }
		
		/// <summary>
		/// 设施
		/// </summary>		
		
		public  string Facilities { get; set; }
		
		/// <summary>
		/// 服务
		/// </summary>		
		
		public  string Services { get; set; }
		
		/// <summary>
		/// 环境
		/// </summary>		
		
		public  string Ambience { get; set; }
		
		/// <summary>
		/// 营业许可证
		/// </summary>		
		
		public  string Permission { get; set; }
		
		/// <summary>
		/// 人均消费
		/// </summary>		
		
		public  int? PerCapita { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Province { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string City { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string District { get; set; }
		
		/// <summary>
		/// 店铺小程序二维码
		/// </summary>		
		
		public  string Qrcode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Picture { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ShowPicture { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Contact { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_V_CouponItems
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CouponNumber { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? GID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CouponID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CouponGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CouponName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CouponType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal CouponMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? CouponDiscount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProductID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SkuID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProductCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? ExchangeTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FullName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? WriteUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? WriteTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WriteFullName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ShopID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string MainCouponName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? MainCouponType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal MainCouponMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? MainCouponDiscount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? MainProductID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string MainSkuID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? MainProductCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CouponStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? StartTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EndTime { get; set; }
		
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
		
		public  int? CouponCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserLimitCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CouponSetType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal LimitMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? LimitNumber { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? ReceiveStartTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? ReceiveEndTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OrderID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsStack { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsOpen { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CouponUseType { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_V_CouponProduct
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CouponID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CouponGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProductID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SkuID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ProductName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Picture { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal ProductPrice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CouponName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? StartTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EndTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsOpen { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsStack { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CouponUseType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CouponStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal CouponMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? CouponDiscount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CouponType { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_V_IntegralLog
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Integral { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OrderID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? IntegralType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
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
		
		public  int? FromUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? FromJobID { get; set; }
		
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
		
		public  int? ToLZUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RefundID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PartnerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? NowIntegral { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FullName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FromFullName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsShopPromotion { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_V_Order
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string OrderID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? OrderGID { get; set; }
		
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
		
		public  int? PartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? PartnerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? OrderStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? OrderDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? UpdateDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? PayDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? ShippingDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? FinishDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal OrderTotal { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal ProductTotal { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PayType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PayID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? OrderType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProductID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ProductName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Address { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SkuID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Tags { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SkuContent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ShippName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ShippPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal ProductPrice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ProductPicture { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AddressName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal SalePrice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RefundID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal RefundMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? RefundDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RefundAbout { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProductCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProductType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ShopID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Integral { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal Cash { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ClientID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ExpressNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ExpressType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ExpressName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsFree { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? FTID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FullName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserTaskID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? TaskID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? TaskPreferentialType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal PreferentialMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PlaceOrderType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Province { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string City { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string District { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SellerRemark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BuyerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? TaskType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? TaskMethod { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool HasRefund { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? RefundIntegra { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal FeeMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OpenID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PersonnelUID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? WriteUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? WriteTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WriteFullName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CouponGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CouponID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CouponName { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_V_OrderItems
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OrderID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SkuID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProductID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SKU { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal ProductPrice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SkuContent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProductCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal TotalMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal RefundAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsRefund { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal CostPrice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal Weight { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ProductName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Picture { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ProductCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? RefundCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsIntegral { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool HasSKU { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? StartTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EndTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal SalePrice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal CouponMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? WriteUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? WriteTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WriteFullName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CouponNumber { get; set; }
		
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
		
		public  int? ShopID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? OrderType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? OrderStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Integral { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal Cash { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal OrderTotal { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal PayCash { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PayIntegral { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RefundID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal RefundMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? RefundDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RefundAbout { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? RefundIntegral { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? OrderDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProductType { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_V_OrderPackageItems
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? PackageGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? OrderItemID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Count { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProductID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SkuContent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal ProductPrice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal TotalMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ProductName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsRefund { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Picture { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OrderID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_V_OrderReturnItems
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ReturnID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProductID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SkuID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SKU { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal ProductPrice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProductCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ProductName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Picture { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ChangeProductID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ChangeSkuID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ChangeSKU { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ChangeProductCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal ChangeProductPrice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ChangeSkuContent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SkuContent { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_V_Pictures
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? AssortID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ShopID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Picture { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CompanyGID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_V_Product
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ProductID { get; set; }
		
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
		
		public  int? CateID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ProductName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProductType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ProductContent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? SaleCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProductCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool HasSKU { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Sort { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Picture { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ShowPicture { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Description { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Unit { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ProductCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal ProductPrice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal CostPrice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal MarketPrice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string HomePicture { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TextIcon { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IconColor { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ShopID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CateName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ShopName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsFree { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FreightTemplateName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? FTID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? BrandID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ProductTabs { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsIntegral { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string VideoFile { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Barcode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? Weight { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsVirtual { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? StartTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EndTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Tabs { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Introduction { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string DataTabs { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal PromotionPrice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CouponName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PromotionSkuID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CateIDS { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal CouponMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserLimitCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? Commission { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsCommission { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsGroupExpress { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_V_ProductTask
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int TaskID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TaskName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProductID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SkuID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? TaskType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProductCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UseCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? TaskMethod { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? StartTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EndTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ProductName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProductStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool HasSKU { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Picture { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ContentCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SkuContent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PreferentialType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal PreferentialMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CompanyGID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_V_ProductUserTask
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? TaskID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? StartTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EndTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? HaveCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TaskName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProductID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SkuID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? TaskType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProductCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UseCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? TaskStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? TaskMethod { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? TaskStartTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? TaskEndTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ProductName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ProductPicture { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Reason { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ContentCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SkuContent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PreferentialType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal PreferentialMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? JobID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ShareJobType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserProductID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserProductName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserSkuID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserSkuContent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? TaskItemID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Integral { get; set; }
		
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
		
		public  string ItemPicture { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FullName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Picture { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_V_PromotionProduct
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PromotionID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? PromotionGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProductID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SkuID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Enable { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? LimitCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal PromotionMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PromotionName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? StartTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EndTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool PromotionEnable { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ShopID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? ShopGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ProductName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProductType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? SaleCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProductCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool HasSKU { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Picture { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ProductCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsIntegral { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsVirtual { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SKU { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Stock { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal SalePrice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SkuPicture { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool SkuEnabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal ProductPrice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CateID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PromotionType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? PromotionDiscount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CompanyGID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_V_PromotionTask
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CompanyGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? StartTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EndTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Enabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal TargetMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal HaveMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? TaskType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FullName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsShopPromotion { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_V_ShoppingCart
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int CartID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProductID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProductCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SkuID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SKU { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SkuContent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ProductName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Picture { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool HasSKU { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal ProductPrice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? SaleCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CateID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Stock { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal CostPrice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal SalePrice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? Weight { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ShopID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CateName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsFree { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FreightTemplateName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? FTID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsVirtual { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsIntegral { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProductType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Enabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal PromotionPrice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CouponName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CouponGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsCheck { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? Commission { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsCommission { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string DataTabs { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_V_SKUItems
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string SkuID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int AttributeID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ValueID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AttributeName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ValueName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Image { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsDelete { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool ValueIsDelete { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_V_UserOrder
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string OrderID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? OrderGID { get; set; }
		
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
		
		public  int? PartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? PartnerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? OrderStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? OrderDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? UpdateDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? PayDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? ShippingDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? FinishDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal OrderTotal { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal ProductTotal { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PayType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PayID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? OrderType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProductID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ProductName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Address { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SkuID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Tags { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SkuContent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ShippName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ShippPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal ProductPrice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ProductPicture { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AddressName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal SalePrice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RefundID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal RefundMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? RefundDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RefundAbout { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProductCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProductType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ShopID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Integral { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal Cash { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ClientID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ExpressNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ExpressType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ExpressName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsFree { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? FTID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FullName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserTaskID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? TaskID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? TaskPreferentialType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal PreferentialMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PlaceOrderType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Province { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string City { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string District { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SellerRemark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BuyerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? TaskType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? TaskMethod { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool HasRefund { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? RefundIntegra { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal FeeMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OpenID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PersonnelUID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? WriteUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? WriteTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WriteFullName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal CouponTotal { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Sex { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Age { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_V_Visit
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? UserGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CompanyGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserAgent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IPAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string City { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? StartTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EndTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Seconds { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FromPage { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? FromType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Search { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Keyword { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Sex { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Age { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CateID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ProductName { get; set; }
		   
    }



	/// <summary>
	/// 秒数 - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Shop")]
	public partial class Shop_Visit
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? UserGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CompanyGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserAgent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IPAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string City { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? StartTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EndTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProductID { get; set; }
		
		/// <summary>
		/// 秒数
		/// </summary>		
		
		public  int? Seconds { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FromPage { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? FromType { get; set; }
		
		/// <summary>
		/// 搜索内容
		/// </summary>		
		
		public  string Search { get; set; }
		
		/// <summary>
		/// 搜索的关键词
		/// </summary>		
		
		public  string Keyword { get; set; }
		   
    }



}


