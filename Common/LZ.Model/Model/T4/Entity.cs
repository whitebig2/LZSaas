



 


using System;
using SqlSugar;
using LZ.Model.Enums;
namespace LZ.Model
{	

	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class AI_Knowledge
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
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
		
		public  string Indexs { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Content { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? Sroce { get; set; }
		
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
	/// AccountReceivableStatusEnumType - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_AccountReceivable
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
		public  Guid CustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid PartnerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string PartnerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid CooperationUnitGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CooperationUnitName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal PaidAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal PendingAmount { get; set; }
		
		/// <summary>
		/// AccountReceivableStatusEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  AccountReceivableStatusEnumType Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Description { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid InvoiceID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string AnnexFileName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string AnnexFilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal InvoiceTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SfWithholdAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid YlBillID { get; set; }
		
		/// <summary>
		/// AccountReceivableDataSourceTypeEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  AccountReceivableDataSourceTypeEnumType DataSourceType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RecipientName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IssuerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string LegalCustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CompanyID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_AccountReceivableRecord
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
		public  Guid AccountReceivableID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal PaidAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string ActionUserName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ActionUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? PaidTime { get; set; }
		   
    }



	/// <summary>
	/// ActionPasswordStatusEnumType - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_ActionPassword
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
		public  bool IsEnable { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Pwd { get; set; }
		
		/// <summary>
		/// ActionPasswordStatusEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  ActionPasswordStatusEnumType Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string IdCardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Phone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string SmsCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int UserID { get; set; }
		   
    }



	/// <summary>
	/// AdviceNoteStatusEnumType - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_AdviceNote
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserEntryID { get; set; }
		
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
		
		public  string IdCardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EntryDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? LeaveDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? SmsSendTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SmsContent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? ContractStartDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? ContractEndDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SigningUnitName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ContractMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WorkName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WorkClass { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? ContractRecordID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SignedFilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Msg { get; set; }
		
		/// <summary>
		/// AdviceNoteStatusEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  AdviceNoteStatusEnumType Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Phone { get; set; }
		   
    }



	/// <summary>
	/// 请假，补卡，加班，提前请假 - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_AttendanceApply
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
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? RecordID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Content { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FileInfo { get; set; }
		
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
		
		public  string Reply { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AuditedID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Sort { get; set; }
		
		/// <summary>
		/// 请假，补卡，加班，提前请假
		/// </summary>		
		
		public  DateTime? ApplyDate { get; set; }
		
		/// <summary>
		/// 上班，下班，加班上班，加班下班
		/// </summary>		
		
		public  int? Type { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? StructureID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ManageGIDS { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? Number { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? SignDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ApplyType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? DefaultTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? DayType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Message { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? AuditStructure { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string StartTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EndTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsDay { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? StartDayType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? EndDayType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? UpTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? OffTime { get; set; }
		
		/// <summary>
		/// 请假或者加班的类型
		/// </summary>		
		
		public  string InfoTypeName { get; set; }
		
		/// <summary>
		/// 加班或请假的类型
		/// </summary>		
		
		public  int? InfoType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? EntryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TypeName { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_AttendanceApplyLog
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
		
		public  string UserName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? AuditedID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AuditedName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Content { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ApplyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string From { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Type { get; set; }
		   
    }



	/// <summary>
	/// 调休 - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_AttendanceAskForLeaveRecord
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
		
		public  int? CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Year { get; set; }
		
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
		
		public  string CardID { get; set; }
		
		/// <summary>
		/// 调休
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int CompensatoryLeave { get; set; }
		
		/// <summary>
		/// 年休假
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int YearLeave { get; set; }
		
		/// <summary>
		/// 工伤假
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int InjuryLeave { get; set; }
		
		/// <summary>
		/// 婚嫁
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int MarryLeave { get; set; }
		
		/// <summary>
		/// 丧假
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int BereavementLeave { get; set; }
		
		/// <summary>
		/// 产检假
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int PrenatalCheckLeave { get; set; }
		
		/// <summary>
		/// 产假
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int MaternityLeave { get; set; }
		
		/// <summary>
		/// 陪产假
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int PrenatalLeave { get; set; }
		
		/// <summary>
		/// 流产假
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int MiscarriageLeave { get; set; }
		
		/// <summary>
		/// 哺乳假
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int LactationLeave { get; set; }
		
		/// <summary>
		/// 陪护假
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int AccompanyLeave { get; set; }
		
		/// <summary>
		/// 事假
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int PersonalLeave { get; set; }
		
		/// <summary>
		/// 病假
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int SickLeave { get; set; }
		
		/// <summary>
		/// 调休已请
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int CompensatoryLeaveUsed { get; set; }
		
		/// <summary>
		/// 年休已请
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int YearLeaveUsed { get; set; }
		
		/// <summary>
		/// 工伤假已请
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int InjuryLeaveUsed { get; set; }
		
		/// <summary>
		/// 婚嫁已请
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int MarryLeaveUsed { get; set; }
		
		/// <summary>
		/// 丧假已请
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int BereavementLeaveUsed { get; set; }
		
		/// <summary>
		/// 产检假已请
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int PrenatalCheckLeaveUsed { get; set; }
		
		/// <summary>
		/// 产假已请
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int MaternityLeaveUsed { get; set; }
		
		/// <summary>
		/// 陪产假已请
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int PrenatalLeaveUsed { get; set; }
		
		/// <summary>
		/// 流产假已请
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int MiscarriageLeaveUsed { get; set; }
		
		/// <summary>
		/// 哺乳假已请
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int LactationLeaveUsed { get; set; }
		
		/// <summary>
		/// 陪护假已请
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int AccompanyLeaveUsed { get; set; }
		
		/// <summary>
		/// 事假已请
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int PersonalLeaveUsed { get; set; }
		
		/// <summary>
		/// 病假已请
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int SickLeaveUsed { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? EntryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_AttendanceCalendar
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
		
		public  DateTime? Calendar { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsHoliday { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsWork { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Year { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Month { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Day { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WeekDay { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? DayEnum { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Content { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_AttendanceConfig
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? GID { get; set; }
		
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
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ManageID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Type { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  double? Latitude { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  double? Longitude { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AddressName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string City { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int Range { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Days { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? OnTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? OffTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Enable { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Count { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Content { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Holiday { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Elastic { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? LunchHours { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? DinnerHours { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? ShiftMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? NightShiftSubsidy { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string LunchTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string DinnerTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? TimeRule { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string DayWorkType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ApplicationCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsApplicationLimit { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? RuleType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string LocationJson { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_AttendanceDayWorkApply
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
		
		public  string UserName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Title { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string StartTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EndTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? WorkHour { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? SignTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? StructureID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string DayWorkType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? Price { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? Money { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? Subsidy { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Path { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Content { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? DayType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Message { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EntryNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? EntryID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_AttendanceDetail
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
		[SugarColumn(IsNullable=false)]
		public  int ConfigID { get; set; }
		
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
		
		public  int? Hour { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Minute { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Content { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsAuto { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Sort { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Title { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 上班/下班
		/// </summary>		
		
		public  string TimeType { get; set; }
		
		/// <summary>
		/// 打卡时间展示
		/// </summary>		
		
		public  string TimeShow { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? DayType { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_AttendancePrepaidApplyCustomer
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? GID { get; set; }
		
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
		
		public  int? CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BillMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? StructureID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string StructureName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? ShiftHours { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? ShiftPrice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? ShiftMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserIDS { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserNames { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? DayPrice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? DayHours { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? DayMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? NightHours { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? NightPrice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? NightMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? InvoiceID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_AttendancePrepaidApplyRecord
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? GID { get; set; }
		
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
		
		public  int? StructureID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string StructureName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BillMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? BillDate { get; set; }
		
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
		
		public  int? EntryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CardID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Phone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? ShiftHours { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? ShiftPrice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? ShiftMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PrepaidApplyCustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EntryNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? JobPrice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? JobMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string DayWorkType { get; set; }
		   
    }



	/// <summary>
	/// 所在车间 - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_AttendanceRecord
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
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? AttendanceDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? DetailID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string StatusName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Type { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TypeName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AddressName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Content { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ManageContent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Message { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string MobileDevice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Title { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TimeShow { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? OnTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? OffTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Sort { get; set; }
		
		/// <summary>
		/// 所在车间
		/// </summary>		
		
		public  int? DepartmentID { get; set; }
		
		/// <summary>
		/// 考勤时间是否是次日
		/// </summary>		
		
		public  int? DayType { get; set; }
		
		/// <summary>
		/// 考勤时间(针对哪一天的考勤)
		/// </summary>		
		
		public  DateTime? SignDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ApplyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? WorkHour { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FileInfo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? DefaultTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? LeaveHour { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? StructureID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? AttendanceType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EntryNo { get; set; }
		
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
		
		public  int? EntryID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_AttendanceStructure
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? DepartmentID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? AttendanceID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ParentID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Enable { get; set; }
		
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
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserIDS { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserNames { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? NavID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? AttendanceType { get; set; }
		   
    }



	/// <summary>
	/// BankTypeEnumType - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_BankAccount
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
		public  string AccountName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string AccountNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsEnable { get; set; }
		
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
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsDelete { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? DeleteTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string DirectCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AcctKey { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AcctNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AcctName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AcctOpenDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CurrencyCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CurrencyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AcctStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OrgKey { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OrgCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OrgName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UseOrgKey { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UseOrgCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UseOrgName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AuthorizedOrgKey { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AuthorizedOrgCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AuthorizedOrgName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string LegalPersonOrgKey { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string LegalPersonOrgCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string LegalPersonOrgName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AcctProvinceCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AcctProvinceName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AcctCityCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AcctCityName { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_BankCompany
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
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CompanyNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsEnable { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsDelete { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? DeleteTime { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_BankCompanyConfig
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
		public  string ConfigTitle { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid BankCompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int BankType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid MainBankAccountID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid PrepaidBankAccountID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid OtherBankAccountID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsDefault { get; set; }
		
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
		
		public  DateTime? DeleteTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_BankCustomer
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
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CustomerNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string SignUnitName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string SignUnitNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsEnable { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsDelete { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? DeleteTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int CompanyID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_BankElectronicReceipt
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FileName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ReceiptInnerNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ReceiptBatchNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BillsPayOrderNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RelBillsCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		   
    }



	/// <summary>
	/// OpenAccountStatusEnumType - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_BankOpenAccount
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
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Sex { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Phone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CardID { get; set; }
		
		/// <summary>
		/// OpenAccountStatusEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  OpenAccountStatusEnumType Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? SendDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Address { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CardType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CardStartDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CardEndDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CardFrontPath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CardBackPath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? BirthDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string OpenAreaNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string OpenWebNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int BatchNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PdfPath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SignedPath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SmsCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SmsContent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? SmsSendTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? SignedTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FailMsg { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int BankType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CompanyAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int UserEntryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CustomerNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CompanyName { get; set; }
		
		/// <summary>
		/// CommonSignStatusEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  CommonSignStatusEnumType SignStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WatermarkFrontPath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WatermarkBackPath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmpSealKey { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FileHash { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmpSealPath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Content { get; set; }
		   
    }



	/// <summary>
	/// 外部系统单据流水号 - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_BankTransaction
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 外部系统单据流水号
		/// </summary>		
		
		public  string SerialNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string BillsPayOrderNo { get; set; }
		
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
		
		public  string PayOrgCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string PayOrgName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
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
		public  string PayCurrencyType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
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
		
		public  string PayUse { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Purpose { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TotalAmount { get; set; }
		
		/// <summary>
		/// BankTransactionStatusEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  BankTransactionStatusEnumType Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TransStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? TransTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TransMsg { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CustomerNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string SignUnitName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string SignUnitNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CompanyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int SucCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int FailCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int TotalCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string BusinessID { get; set; }
		
		/// <summary>
		/// BankTypeEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  BankTypeEnumType BankType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FailMsg { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TransactionID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsSendNotice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int SendNoticeCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? NoticeTime { get; set; }
		
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
		
		public  string FailReason { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RelPaymentDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PayAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RefundDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RelBillsCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RefundAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RecordRelBillsCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Summary { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? BankCompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid BankCompanyConfigID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? PushTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PushResultCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PushResultMsg { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? LockTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int DataType { get; set; }
		   
    }



	/// <summary>
	/// TransactionStatusEnumType - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_BankTransactionRecord
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
		public  Guid BankTransactionID { get; set; }
		
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
		
		public  string ReceiveOrgCode { get; set; }
		
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
		/// TransactionStatusEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  TransactionStatusEnumType RecordTransStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RecordTransMsg { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? RecordTransTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SalaryRecordID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RecordBusinessID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CardID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RecordPayStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RecordPayStatusName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RecordFailReasons { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RecordRelPaymentDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RecordPayAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RecordRefundAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RecordRefundDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RecordTransactionID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RecordRelBillsCode { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_BankUser
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
		public  int UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string SecretKey { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsEnable { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsDelete { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? DeleteTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsValidate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsLock { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? LockTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FailMsg { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int FailTimes { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_BankUserCompany
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
		public  bool IsDetail { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsPay { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsExecute { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsEnable { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid BankCompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid BankUserID { get; set; }
		   
    }



	/// <summary>
	/// 0-身份证，目前仅支持身份证类型 - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_BankWallet
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? GID { get; set; }
		
		/// <summary>
		/// 0-身份证，目前仅支持身份证类型
		/// </summary>		
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CertType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CertNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ValidityPeriod { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SignDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Address { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BindMedium { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Mobile { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Occupation { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 融e聚电子账户账号
		/// </summary>		
		
		public  string MediumId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OpenAcctErrorCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OpenAcctErrorMsg { get; set; }
		
		/// <summary>
		/// 协议开通类型 1:【小额免密】 2:【新型无卡】
		/// </summary>		
		
		public  int? ProtocolType { get; set; }
		
		/// <summary>
		/// 签约协议状态 0-未签约, 1-已签约
		/// </summary>		
		
		public  int? MediumProStatus { get; set; }
		
		/// <summary>
		/// 签约协议编号
		/// </summary>		
		
		public  string MediumProId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FailMsg { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SmsSendNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OutUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Code { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Msg { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CloseCorpSerno { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PhoneMsgNo { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_BlackList
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? GID { get; set; }
		
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
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IdentityCard { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Content { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string LabelIDS { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string LabelNames { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Method { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsRemind { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_BlackListLabel
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
		
		public  string Title { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Sort { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_BudgetApplyRecord
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
	[TenantAttribute("Main")]
	public partial class Live_BudgetApplyRecordDetail
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsIdentity=true)]
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
	[TenantAttribute("Main")]
	public partial class Live_BudgetConfig
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
	[TenantAttribute("Main")]
	public partial class Live_BudgetManage
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
	[TenantAttribute("Main")]
	public partial class Live_BudgetMonthReport
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? BudgetReportId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string MainTitle { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Title { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int DataType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int QuarterType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BusinessIncome { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_BudgetRechargeLog
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
		
		public  int? CID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Year { get; set; }
		
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
		
		public  string Content { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal Money { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PartnerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CostName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Title { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string GenerateName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Type { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OtherGenerateName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OtherCostName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OtherPartnerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OtherTitle { get; set; }
		
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
		public  decimal HeadOfficeUsedAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal AddAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal DecreaseAmount { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_BudgetReport
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
		
		public  int? PartnerID { get; set; }
		
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
		
		public  int? Year { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_BudgetReportConfig
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
		
		public  int? CID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Title { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ItemType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CostType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CostName { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_BudgetReportDetail
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
		
		public  int? BudgetReportID { get; set; }
		
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
		
		public  int? BudgetReportConfigID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Title { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal Money { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Year { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Month { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? QuarterType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string QuarterName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? MonthType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string MonthName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_BusinessApiOrderDetail
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int Id { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? GenerateDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? Cqbrq { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal Cbf { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Geentname { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Cname { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Proname { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Cgygz { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Cidnum { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Csjygdw { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Cage { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Czylb { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? Czbrq { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Geentid { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Cldhtdw { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string InsuranceId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int UserEntryId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int UserEntryStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ActionType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string InsuranceCompany { get; set; }
		   
    }



	/// <summary>
	/// UserEntryPlanStatusEnumType - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_BusinessGuarantee
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int Id { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? InsuranceMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? StartDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? EndDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string InsuranceCompany { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string PlanName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int PlanNameId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string IdCardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string SignUnitName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string WorkName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string WorkClass { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string InsuranceId { get; set; }
		
		/// <summary>
		/// UserEntryPlanStatusEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  UserEntryPlanStatusEnumType Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int UserEntryId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? EntryTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string UserEntryStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsMonthInsurance { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsEnabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsDelTag { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid DelRecordID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsMatchRecord { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? GenerateDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int SortNo { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_BusinessInsuranceConfig
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
		public  Guid CustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid InsuranceSigningUnitID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string InsuranceCompany { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Title { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Plan3 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Plan4 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Plan5 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsDefaultPush { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsMain { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsEnabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WorkNames { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceCompanyOther { get; set; }
		   
    }



	/// <summary>
	/// BusInsuranceInitialStatusEnumType - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_BusinessInsuranceInitialRecord
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
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string IdCardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string InsuranceSigningUnitName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string WorkName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string WorkClass { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? EntryDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string InsuranceCompany { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string PlanName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string InsuranceId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// BusInsuranceInitialStatusEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  BusInsuranceInitialStatusEnumType Status { get; set; }
		
		/// <summary>
		/// BusInsuranceInitialDataTypeEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  BusInsuranceInitialDataTypeEnumType DataType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Msg { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int UserEntryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? InsuranceMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsMain { get; set; }
		
		/// <summary>
		/// BusInsuranceInitialActionTypeEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  BusInsuranceInitialActionTypeEnumType ActionType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsDefaultPush { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int PlanNameId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ResultMsg { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? StartDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EndDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsDayOrder { get; set; }
		   
    }



	/// <summary>
	/// InsuranceOrderEnumType - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_BusinessInsuranceOrder
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
		public  string InsuranceCompany { get; set; }
		
		/// <summary>
		/// InsuranceOrderEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  InsuranceOrderEnumType OrderType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string InsuranceId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string InsuranceNo { get; set; }
		
		/// <summary>
		/// BusinessInsuranceStatusEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  BusinessInsuranceStatusEnumType Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsDefaultPush { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid SigningUnitID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? StartDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? EndDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SigningUnitNames { get; set; }
		   
    }



	/// <summary>
	/// InsuranceActionEnumType - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_BusinessInsuranceOrderEmpoyee
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
		public  Guid BusinessInsuranceOrderID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string IdCardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Sex { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int Age { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string JobType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string JobLevelType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CertificateType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string SigningUnitName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string JobCompanyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string PlanName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? StartDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? EndDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal InsuranceAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Remark { get; set; }
		
		/// <summary>
		/// InsuranceActionEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  InsuranceActionEnumType ActionType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// InsuranceNameRecordStatusEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  InsuranceNameRecordStatusEnumType Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string FromType { get; set; }
		
		/// <summary>
		/// InsuranceMethodEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  InsuranceMethodEnumType MethodType { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_BusinessInsuranceOrderPlan
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
		public  Guid BusinessInsuranceOrderID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int GnteeId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string GnteeName { get; set; }
		   
    }



	/// <summary>
	/// InsurancePushStatusEnumType - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_BusinessInsurancePush
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
		public  string InsuranceId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? AddPushTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int AddRecordCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string AddIndecaseId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? DelPushTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int DelRecordCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string DelIndecaseId { get; set; }
		
		/// <summary>
		/// InsurancePushStatusEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  InsurancePushStatusEnumType AddPushStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int AddFailCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int DelFailCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string PlanName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int PlanNameId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int AddResCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string AddResMsg { get; set; }
		
		/// <summary>
		/// InsurancePushStatusEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  InsurancePushStatusEnumType DelPushStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int DelResCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string DelResMsg { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceCompany { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_BusinessInsurancePushRecord
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
		public  string Custemername { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Custemernum { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Custemersex { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Custemerage { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Guyuangongzhong { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Sjygdw { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Ldhtdw { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Zhiyeleibei { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Custemertype { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid BusinessInsuracneRecordID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid BusinessInsurancePushID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int GnteeId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string GnteeName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsAdd { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsFail { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int UserEntryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsMain { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsCompleted { get; set; }
		   
    }



	/// <summary>
	/// BusinessInsuranceStatusEnumType - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_BusinessInsuranceRecord
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
		public  int UserEntryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid CustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid InsuranceSigningUnitID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string InsuranceSigningUnitName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsDayOrder { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string InsuranceCompany { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string OrderNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string InsuranceNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string PlanName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Sex { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int Age { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string IdCardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? EntryDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? LeaveDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string WorkName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string WorkClass { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsExpired { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? AddSysTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? AddPushTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? AddMatchRecordID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? DelSysTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? DelPushTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? DelMatchRecordID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? StartDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? EndDate { get; set; }
		
		/// <summary>
		/// BusinessInsuranceStatusEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  BusinessInsuranceStatusEnumType Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsMain { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsDefaultPush { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int PlanNameId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? BusinessInsurancePushID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IndecaseId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? PushTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PushCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PushMsg { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_BusinessInsuranceWorkNamePlan
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
		public  Guid BusinessInsuranceConfigID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string WorkName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string PlanName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string InsuranceCompany { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsEnable { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_BusinessModel
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int BusinessModelID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? BusinessModel { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CustomerGID { get; set; }
		
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
		
		public  DateTime? CreatrTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Enable { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_BusInsuracneOrder
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
		public  string InsuranceCompany { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string OrderNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PolicyNum { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? OrderMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string PlanName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int PlanNameID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SignUnitNames { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsEnable { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? StartDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EndDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsDayOrder { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_BusInsuracneOrderPlan
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
		public  string PlanName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal PlanPriceAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int BusInsuranceOrderID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int PlanNameID { get; set; }
		   
    }



	/// <summary>
	/// BusInsuranceApiPushStatusEnumType - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_BusInsuranceApiPush
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
		public  string InsuranceCompany { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? RecordDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string PushType { get; set; }
		
		/// <summary>
		/// BusInsuranceApiPushStatusEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  BusInsuranceApiPushStatusEnumType Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int AddCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int DelCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ConfirmUrl { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		   
    }



	/// <summary>
	/// BusInsuranceApiPushDetailStatusEnumType - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_BusInsuranceApiPushDetail
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
		public  Guid BusInsuranceApiPushID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CardID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int Age { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Sex { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Phone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string OrderNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string PlanName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string WorkName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string WorkClass { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string StartDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsSocial { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsWorkHeight { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FailMsg { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string InsuranceCompanyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string InsuranceWorkName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string InsuranceWorkClass { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int AddBusinessID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int DelBusinessID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string DelName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string DelCardID { get; set; }
		
		/// <summary>
		/// BusInsuranceApiPushDetailStatusEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  BusInsuranceApiPushDetailStatusEnumType Status { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_BusInsuranceCompany
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
		public  string InsuranceCompany { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsEnable { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsLimitEntry { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsWorkDay { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  TimeSpan StartTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  TimeSpan EndTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		   
    }



	/// <summary>
	/// BusInsuranceDayMatchRecordStatusEnumType - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_BusInsuranceDayMatchRecord
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
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string SignUnitName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OrderNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string InsuranceCompany { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string WorkName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WorkClass { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string PlanName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int PlanNameID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CardID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int UserEntryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsAddRecord { get; set; }
		
		/// <summary>
		/// BusInsuranceDayMatchRecordStatusEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  BusInsuranceDayMatchRecordStatusEnumType Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? RecordDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string InsuranceWorkName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceWorkClass { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? StartDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EndDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Msg { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int DelMatchRecordID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int Age { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Sex { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PushResult { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IndecaseId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? PlanNamePrice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Weight { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string DelMatchRecordName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string DelMatchRecordCardID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string DelMatchRecordPlanName { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_BusInsurancePlanName
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
		public  string PlanName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsurancePlanName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsEnable { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		   
    }



	/// <summary>
	/// BusInsuranceRecordStatusEnumType - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_BusInsuranceRecord
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
		public  string SignUnitName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Sex { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int Age { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CardID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string WorkName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string WorkClass { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PolicyNum { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OrderNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string PlanName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PlanNameID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string InsuranceCompany { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? StartDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EndDate { get; set; }
		
		/// <summary>
		/// BusInsuranceRecordStatusEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  BusInsuranceRecordStatusEnumType Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? InsuranceMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AddPushID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string DelPushID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? MatchDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsDelMatch { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsManuallyInsurance { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? PlanNamePrice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Weight { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string DelMatchRecordName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string DelMatchRecordCardID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string DelMatchRecordPlanName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? DelMatchRecordPlanNamePrice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceWorkName { get; set; }
		   
    }



	/// <summary>
	/// InsuranceTypeEnumType - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_BusInsuranceWorkPlan
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
		public  string WorkName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string PlanName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string InsuranceCompany { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsEnable { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceWorkName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceWorkClass { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsUseBusInsurance { get; set; }
		
		/// <summary>
		/// InsuranceTypeEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  InsuranceTypeEnumType InsuranceType { get; set; }
		   
    }



	/// <summary>
	/// CaAuthIdentityStatusEnumType - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_CaAuthIdentity
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
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Phone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CardID { get; set; }
		
		/// <summary>
		/// CaAuthIdentityStatusEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  CaAuthIdentityStatusEnumType Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AgreeUrl { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsAgree { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AgreeVerifyId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AgreeRedictUrl { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? AgreeTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FaceOrderId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FaceVerifyId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FaceUrl { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FaceRedictUrl { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FaceRescode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FaceResNewSignature { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FaceResH5faceId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FaceResLiveRate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OriCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OrderNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string LiveRate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Similarity { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OccurredTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Success { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Photo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Video { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BizSeqNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TransactionTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AuthType { get; set; }
		
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
		
		public  string AuthUrl { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? UserGid { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? AuthTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FaceVideoPath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FacePhotoPath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsApp { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SealPath { get; set; }
		
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
		
		public  string LegalPersonVerifyId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsCompany { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SmsMsgId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SmsTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SmsPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FaceAgeCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FaceAgeAppKey { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FaceAgeState { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FaceAgeTimestamp { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FaceAgeApplyId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AppName { get; set; }
		   
    }



	/// <summary>
	/// CashSignStatusEnumType - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_CashSignRecord
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
		
		public  string Sex { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Age { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CardID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CashType { get; set; }
		
		/// <summary>
		/// CashSignStatusEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  CashSignStatusEnumType Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? ExpirationDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CompanyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PartnerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SignUnitName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int PartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TempPath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PdfPath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SignedPath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int UserEntryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int SignType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string SealPath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SmsCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SmsContent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SmsSendTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CashHash { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Phone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SmsMsgID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FailMsg { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? SignTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsEnable { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SignUnitCompanyNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SignUnitSealPath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SignUnitSealKey { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SealKey { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? GID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_CityPartner
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
		public  string Title { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string City { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string AgentName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string AgentIdCardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string AgentPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsEnabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string ProfitPartnerGIDArr { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Type { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int TaskNum { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_CityPartnerBill
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
		public  DateTime? BillMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid CityPartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CityPartnerTitle { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CityPartnerAgentName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal InsuranceRebateAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal LaborRebateAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal OutsourcingRebateAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SalesmanAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal LaborProfitAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RecruitmentProfitAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ProfitPartnerGIDArr { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CityPartnerCity { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SaleBill { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_CityPartnerEmpRecord
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
		public  Guid CityPartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CityPartnerTitle { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CityPartnerAgentName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid PerPartnerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string PerPartnerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid CustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int UserEntryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string IdCardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? EntryDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? LeaveDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RebateAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? RecordMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CompanyCityPartnerTitle { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal HQRebateAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int PlanStatus1 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int CityPartnerBillID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_CityPartnerSalaryRecord
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
		public  Guid CityPartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CityPartnerTitle { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid PerPartnerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string PerPartnerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid CustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CardID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitSalaryAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ServiceAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? TaxMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? RecordMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? EntryDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? LeaveDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int UserEntryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RebateAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SalesmanPercent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Salesman { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string BusinessModelName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int BusinessModel { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CustomerPaidTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal HQRebateAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int CityPartnerBillID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_CitySalesman
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
		public  string City { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string IdCardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Phone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string BankName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string BankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsEnabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_ClientMyTemplate
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
		
		public  int? TempID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ClientID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AppConfig { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsDefault { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_ClientNoticeTemplate
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ClientID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? NoticeID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TempID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_ClientPage
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ClientID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Moulds { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PageType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Style { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Title { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? TempID { get; set; }
		   
    }



	/// <summary>
	/// 应用类别蓝招，幸福职 - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_Clients
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? GID { get; set; }
		
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
		
		public  string Type { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Enabled { get; set; }
		
		/// <summary>
		/// 应用类别蓝招，幸福职
		/// </summary>		
		
		public  string AppType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AppID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AppSecret { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Token { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PrivateKeyPath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ProjectPath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ReleaseStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Version { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string HomePage { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AppConfig { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? ReleaseTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string MediaID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsDefault { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FxqAppName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FxqAppAuthUrl { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_ClientShortLink
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ClientID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ShortLinkType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ShortLinkUrl { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PageUrl { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PageTitle { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsPermanent { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_ClientTemplate
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
		
		public  string AppType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PageConfig { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsDefault { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_ClientTemplatePage
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Moulds { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PageType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Style { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Title { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? TempID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_CompanyAuthorize
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
		
		public  int? ToCompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Enabled { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_CompanyPerFunction
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true)]
		public  int CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int PFID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Sort { get; set; }
		   
    }



	/// <summary>
	/// ContractEnumType - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_ContractFileConfig
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FileName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FirstPartyKeyword { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SecondPartyKeyword { get; set; }
		
		/// <summary>
		/// ContractEnumType
		/// </summary>		
		
		public  ContractEnumType? ContractEnumType { get; set; }
		
		/// <summary>
		/// SignEnumType
		/// </summary>		
		
		public  SignEnumType? SignEnumType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsEnabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsDefault { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ImageOssUrl { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsOss { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsNeedSignDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsNeedPageSeal { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Description { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsKeywordReplace { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsNeedSigningUnitSignKeyword { get; set; }
		   
    }



	/// <summary>
	/// SignerPartyEnumType - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_ContractFileSigner
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsCompany { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsEnabled { get; set; }
		
		/// <summary>
		/// SignerPartyEnumType
		/// </summary>		
		
		public  SignerPartyEnumType? SignerPartyType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SignKeyword { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsPageSign { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsSignTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid ContractFileConfigID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PageSignHeight { get; set; }
		   
    }



	/// <summary>
	/// ContractEnumType - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_ContractRecord
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TemplateOssUrl { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ContractOssUrl { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SignedOssUrl { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsNeedKeywordReplace { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string KeywordReplaceContent { get; set; }
		
		/// <summary>
		/// ContractEnumType
		/// </summary>		
		
		public  ContractEnumType? ContractType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? SignedTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? RelateID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? RelateGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? RStatus { get; set; }
		
		/// <summary>
		/// SignStatusEnumType
		/// </summary>		
		
		public  SignStatusEnumType? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FxqSignedOssUrl { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FxqTransactionId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FxqHash { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? BatchNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserEntryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserIdCardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerCompanyNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SigningUnitGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SigningUnitName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SigningUnitCompanyNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? StartDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EndDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? ContractStartDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? ContractEndDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? SigningTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? SendSmsTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SmsCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SmsContent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? ReadySigningDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsRetire { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsNotice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmpName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmpIdCardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmpSex { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? EmpAge { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? TrialMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ContractMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EntryDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? LeaveDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobCity { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string NativePlace { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SigningUnitAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SigningUnitLegaPperson { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SigningUnitPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SigningUnitSignKeyword { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmpSignKeyword { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmergencyContactName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmergencyContactPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Education { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobContent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmpNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmpPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmpOssUrl { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SigningUnitOssUrl { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WordOssUrl { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsSuc { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SigningUnitLegalPerson { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool SigningUnitSignDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? SigningUnitSignPageHeight { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool EmpSignDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? EmpPageHeight { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SmTemplateFilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SmWordFilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SmPdfFilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SmSignedFilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TemplateFilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WordFilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PdfFilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SignedFilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string HomeAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string HtHash { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SmHash { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsAdviceNote { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AutoLeaveFilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsAutoLeave { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AutoLeaveSmsContent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? AutoLeaveSmsSendTime { get; set; }
		
		/// <summary>
		/// AutoLeaveStatusEnumType
		/// </summary>		
		
		public  AutoLeaveStatusEnumType? AutoLeaveStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? ContractDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? TrialStartDate { get; set; }
		
		/// <summary>
		/// 试用结束时间
		/// </summary>		
		
		public  DateTime? TrialEndDate { get; set; }
		
		/// <summary>
		/// 试用描述
		/// </summary>		
		
		public  string TrialDescription { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? RetireDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsRenewal { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AutoLeaveHash { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? SmsSendFailTimes { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsSmNeedSigningUnitSignKeyword { get; set; }
		
		/// <summary>
		/// SignTypeEnumType
		/// </summary>		
		
		public  SignTypeEnumType? SignType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string College { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CollegeMajor { get; set; }
		   
    }



	/// <summary>
	/// SignerPartyEnumType - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_ContractRecordSigner
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? ContractRecordID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? ContractSignerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SignKeyword { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SignerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SignerIdno { get; set; }
		
		/// <summary>
		/// SignerPartyEnumType
		/// </summary>		
		
		public  SignerPartyEnumType? SignPartyType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SignOssUrl { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SmsPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SmsContent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? SmsSendTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SmsCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FxqSmsId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FxqSmsTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PageSignHeight { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsContainSignDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CustomerGID { get; set; }
		   
    }



	/// <summary>
	/// RealNameEnumType - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_ContractSigner
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Idno { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OssUrl { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? Expired { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsCompany { get; set; }
		
		/// <summary>
		/// RealNameEnumType
		/// </summary>		
		
		public  RealNameEnumType? RealNameType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? RealNameTime { get; set; }
		
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
		
		public  bool IsRealNameSuc { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Phone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string LegalPerson { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string LegalPersonIdCardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ApplyID { get; set; }
		   
    }



	/// <summary>
	/// 客服 - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_Customer
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CompanyGID { get; set; }
		
		/// <summary>
		/// 客服
		/// </summary>		
		
		public  int? ServiceUserID { get; set; }
		
		/// <summary>
		/// 驻厂人员
		/// </summary>		
		
		public  int? FactoryUserID { get; set; }
		
		/// <summary>
		/// 经营模式
		/// </summary>		
		
		public  int? OperateModel { get; set; }
		
		/// <summary>
		/// 入职模式
		/// </summary>		
		
		public  int? EntryModel { get; set; }
		
		/// <summary>
		/// 服务商，联营
		/// </summary>		
		
		public  int? PartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? PartnerGID { get; set; }
		
		/// <summary>
		/// 服务商，联营
		/// </summary>		
		
		public  int? JointPartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? JointPartnerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Enabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Tel { get; set; }
		
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
		
		public  string QQ { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remarks { get; set; }
		
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
		/// 账单日
		/// </summary>		
		
		public  int? BillingDay { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Annex { get; set; }
		
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
		
		public  string FromType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FromNames { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Tabs { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Type { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Logo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  double? Latitude { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  double? Longitude { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Picture { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RecruitTel { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Coordinate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Scale { get; set; }
		
		/// <summary>
		/// 行业
		/// </summary>		
		
		public  int? Industry { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? Money { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string About { get; set; }
		
		/// <summary>
		/// 试招聘
		/// </summary>		
		
		public  bool IsTry { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Structure { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Months { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Recommend { get; set; }
		
		/// <summary>
		/// 街道
		/// </summary>		
		
		public  string Street { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RecruitAnnex { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? DayMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? NoDayMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserDayMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? UserMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? NoUserDayMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string GroupNotice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobProvince { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobCity { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobDistrict { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsAdvance { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AdvanceMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Physical { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Social { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Accumul { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BatteryCar { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InterviewProcess { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ContactName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BusLine { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PhysicalMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PhysicalDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SocialMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AccumulMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool DirectEntry { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string MonthCommission { get; set; }
		
		/// <summary>
		/// 合伙人
		/// </summary>		
		
		public  int? CUID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EditMoneyTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ShowMonthMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CompanyNo { get; set; }
		
		/// <summary>
		/// 商务主体
		/// </summary>		
		
		public  string BusinessEntity { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InvoiceName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InvoiceType { get; set; }
		
		/// <summary>
		/// 是否备案综合工时
		/// </summary>		
		
		public  bool IsSynthetical { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Bank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BankAccount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string invoiceRemarks { get; set; }
		
		/// <summary>
		/// 服务费
		/// </summary>		
		
		public  decimal? ServiceMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? PersonnelMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserMonths { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ServiceMoneyAbout { get; set; }
		
		/// <summary>
		/// 是否长期联营
		/// </summary>		
		
		public  bool IsLongJoint { get; set; }
		
		/// <summary>
		/// 日结工结算类型
		/// </summary>		
		
		public  int? DailyType { get; set; }
		
		/// <summary>
		/// 日结工结算周期
		/// </summary>		
		
		public  int? DailyDays { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string DayShiftTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string NightShiftTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string DayStartTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string DayEndTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string NightStartTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string NightEndTime { get; set; }
		
		/// <summary>
		/// 白班时长
		/// </summary>		
		
		public  decimal? DayShiftHours { get; set; }
		
		/// <summary>
		/// 夜班时长
		/// </summary>		
		
		public  decimal? NightShiftHours { get; set; }
		
		/// <summary>
		/// 白班工价
		/// </summary>		
		
		public  decimal? DayShiftMoney { get; set; }
		
		/// <summary>
		/// 夜班工价
		/// </summary>		
		
		public  decimal? NightShiftMoney { get; set; }
		
		/// <summary>
		/// 午饭工时
		/// </summary>		
		
		public  decimal? LunchHours { get; set; }
		
		/// <summary>
		/// 晚饭工时
		/// </summary>		
		
		public  decimal? DinnerHours { get; set; }
		
		/// <summary>
		/// 夜班补贴
		/// </summary>		
		
		public  decimal? NightShiftSubsidy { get; set; }
		
		/// <summary>
		/// 是否日结工
		/// </summary>		
		
		public  bool IsDayOver { get; set; }
		
		/// <summary>
		/// 是否跟随客户地址
		/// </summary>		
		
		public  bool IsUserAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EntryWorkClass { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EntryWorkName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EntryFromType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CooperationUnitGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SigningUnitGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? JointAuditStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PartnerIDS { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PartnerNames { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JointObjType { get; set; }
		
		/// <summary>
		/// 与甲方客户的关联码
		/// </summary>		
		
		public  string RelatedCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ToCompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? ToCompanyGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? BusinessModel { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AgentName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AgentIdCardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AgentBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AgentBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool SociaEnabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SocialFee { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string LegalCustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WorkNames { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? BusinessType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BusinessTypeName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BaseType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WagesValueAddedTax { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WagesAdditionalTax { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? Tax { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SalesMan { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsSaleCustomer { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SaleContent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? SaleStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ServiceValueAddedTax { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ServiceAdditionalTax { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool InvoiceCombine { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WagesInvoiceType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WagesInvoiceName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ServiceInvoiceType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ServiceInvoiceName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string VicePartnerIDS { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string VicePartnerIDNames { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsUserContact { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmergencyContact { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmergencyContactPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string VideoFile { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? AttendanceType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? ShiftPrice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? Wages { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? Charge { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? LunchHour { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? DinnerHour { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Post { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WorkTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WorkContent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Requirement { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Cycle { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WXName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WXPasswordSalt { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? TimeRule { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsAuth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsReal { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AuthorizeFilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BankName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string VerifyAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string VerifyCode { get; set; }
		
		/// <summary>
		/// 认证人姓名
		/// </summary>		
		
		public  string VerifyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string VerifyCardID { get; set; }
		
		/// <summary>
		/// 法人姓名
		/// </summary>		
		
		public  string LegalName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? VerifyType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CitySalesmanID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? CitySalesmanPercent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsUseBusInsurance { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsUseDeathInsurance { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? GroupID { get; set; }
		   
    }



	/// <summary>
	/// SignTypeEnumType - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_CustomerContractConfig
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
		public  Guid CustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsEnabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? EntryFileID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? LeaveFileID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? ReEntryFileID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? ReLeaveFileID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? DelayDays { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? SignMonths { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? TrialMonths { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? EntryDeclareFileID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CashFileID { get; set; }
		
		/// <summary>
		/// SignTypeEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  SignTypeEnumType SignType { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_CustomerContractFile
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
		public  Guid CustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? JoinFileID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? LeaveFileID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? ReJoinFileID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? ReLeaveFileID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CashFileID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsEnabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_CustomerGroup
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int GroupID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? GroupGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string GroupName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Enable { get; set; }
		
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
	[TenantAttribute("Main")]
	public partial class Live_CustomerJobs
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true)]
		public  int CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int EntryJobID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsDefault { get; set; }
		   
    }



	/// <summary>
	/// 客户 - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_CustomerServiceCharge
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 客户
		/// </summary>		
		
		public  int? CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CustomerGID { get; set; }
		
		/// <summary>
		/// 开票公司
		/// </summary>		
		
		public  int? InvoiceCustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? InvoiceCustomerGID { get; set; }
		
		/// <summary>
		/// 收票公司
		/// </summary>		
		
		public  int? ReceiptCustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? ReceiptCustomerGID { get; set; }
		
		/// <summary>
		/// 所属工资月份
		/// </summary>		
		
		public  DateTime? SalaryMonth { get; set; }
		
		/// <summary>
		/// 开票时间
		/// </summary>		
		
		public  DateTime? InvoiceDate { get; set; }
		
		/// <summary>
		/// 发工资时间
		/// </summary>		
		
		public  DateTime? SendDate { get; set; }
		
		/// <summary>
		/// 开票金额
		/// </summary>		
		
		public  decimal? InvoiceAmount { get; set; }
		
		/// <summary>
		/// 工资增值税额
		/// </summary>		
		
		public  decimal? SalaryVAT { get; set; }
		
		/// <summary>
		/// 工资附加税额
		/// </summary>		
		
		public  decimal? SalarySurtax { get; set; }
		
		/// <summary>
		/// 服务费
		/// </summary>		
		
		public  decimal? ServiceCharge { get; set; }
		
		/// <summary>
		/// 服务费增值税额
		/// </summary>		
		
		public  decimal? ServiceVAT { get; set; }
		
		/// <summary>
		/// 服务费附加税额
		/// </summary>		
		
		public  decimal? ServiceSurtax { get; set; }
		
		/// <summary>
		/// 社保公积金
		/// </summary>		
		
		public  decimal? SocialAmount { get; set; }
		
		/// <summary>
		/// 人数
		/// </summary>		
		
		public  int? PeopleCount { get; set; }
		
		/// <summary>
		/// 合计金额
		/// </summary>		
		
		public  decimal? TotalAmount { get; set; }
		
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
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CompanyGID { get; set; }
		
		/// <summary>
		/// 是否预估
		/// </summary>		
		
		public  bool IsPredict { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PerMoneyDayCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? NowMoneyDayCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SalaryCustomerID { get; set; }
		
		/// <summary>
		/// 平均工资
		/// </summary>		
		
		public  decimal? AverageSalary { get; set; }
		
		/// <summary>
		/// 平均服务费
		/// </summary>		
		
		public  decimal? AverageService { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_DayWorkCustomerContractRecord
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? GID { get; set; }
		
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
		
		public  int? CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ContractFileConfigID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SignPath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SignKeyword { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SignerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SignerIdNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CompanyContract { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TemplateContract { get; set; }
		
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
		
		public  string CompanyNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ContractTransactionId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ContractHash { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_DayWorkUserContractFile
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? JobDayWorkID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EntryNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CardID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmploymentStatement { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string StatementTransactionId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string StatementHash { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmploymentContract { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ContractTransactionId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ContractHash { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TemplateStatement { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TemplateContract { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_DayWorkUserContractRecord
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? GID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CustomerID { get; set; }
		
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
		
		public  string SignPath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SignKeyword { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SignerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SignerIdNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? FileID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Message { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SendTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? EntryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Code { get; set; }
		   
    }



	/// <summary>
	/// BusInsuranceRecordStatusEnumType - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_DeathInsuranceRecord
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
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? InsuranceStartDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? InsuranceEndDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? ApplyDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string InsuranceName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PlanName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CardID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Passport { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string ChangeType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? StartDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? EndDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string WorkName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsHasSocialInsurance { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SignUnitName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SignUnitAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// BusInsuranceRecordStatusEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  BusInsuranceRecordStatusEnumType Status { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_Enterprise
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
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CompanyNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string LegalPerson { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Phone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Address { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string BankName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string BankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsEnabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsSealCompany { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsBusinessCooperation { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsDeleted { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsRealName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsSigningUnit { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsCooperationUnit { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsInvoiceUnit { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string LegalPersonCardID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_EntryJob
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
		
		public  string Type { get; set; }
		
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
		
		public  bool Enabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsInsurance { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_FileUpload
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
		
		public  int? FID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? FGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Path { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? TypeKey { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TypeValue { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FileName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FileLength { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FileContentJSON { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? SID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_FxqElectronicSeal
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ColorType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? FontType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ShapeType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OssUrl { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FileExtension { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? ExpirationDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsCompanySeal { get; set; }
		   
    }



	/// <summary>
	/// OpenAccountProcessStatusEnumType - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_IcbcOpenAccountRecord
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
		public  Guid BatchNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid BusinessID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? SendDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string OpenZono { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string OpenBrno { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? BirthDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Sex { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CardType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CardID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CardEndDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CardStartDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Address { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Phone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string FilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string ExcelFileName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string ZipFileName { get; set; }
		
		/// <summary>
		/// OpenAccountProcessStatusEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  OpenAccountProcessStatusEnumType Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ResCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ResMsg { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CardFrontPath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CardBackPath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FailMsg { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int FailCount { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_IcbcOpenLog
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? BatchNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Code { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Msg { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? GID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Title { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_InjuryAdvanceAccount
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
		
		public  int? PartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? PartnerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? BasePercent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? FloatPercent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? TotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? UsedAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? AvailableAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool Enabled { get; set; }
		   
    }



	/// <summary>
	/// 借款/审核 - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_InjuryAdvanceAccountLog
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
		
		public  int? PartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? PartnerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? Money { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? TotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? UsedAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? AvailableAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Type { get; set; }
		
		/// <summary>
		/// 借款/审核
		/// </summary>		
		
		public  string TypeName { get; set; }
		
		/// <summary>
		/// 操作者
		/// </summary>		
		
		public  string Name { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_InjuryApplyRecord
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
		
		public  int? InjuryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? InjuryGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PerUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Content { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Bank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BankAccount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Path { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal Money { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Type { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? MoneyType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? PayTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? AdvanceAccountID { get; set; }
		   
    }



	/// <summary>
	/// 累计缴纳金额 - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_InjuryClaimRatio
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
		
		public  int? PartnerID { get; set; }
		
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
		
		public  int? CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Type { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TypeName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 累计缴纳金额
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal PremiumReceivedAmount { get; set; }
		
		/// <summary>
		/// 累计已决理赔金额
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ResolvedClaimAmount { get; set; }
		
		/// <summary>
		/// 累计未决理赔金额
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal OutstandingClaimAmount { get; set; }
		
		/// <summary>
		/// 理赔率
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ClaimRatio { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int InjuryNumber { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int InsuranceNumber { get; set; }
		
		/// <summary>
		/// 工伤发生率
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal InjuryRate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? SignTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Year { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TotalAmount { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_InjuryDocumentLabel
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? GUID { get; set; }
		
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
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WorkInjuryReportFileID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WorkInjuryReportFileName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string NoticeLetterFileID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string NoticeLetterFileName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AuthorizationLetterFileID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AuthorizationLetterFileName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CompensationAgreementLetterFileID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CompensationAgreementLetterFileName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CompensationAgreementDisabilityLetterFileID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CompensationAgreementDisabilityLetterFileName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceCompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceCompanyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CompanyApplyLoanFileID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CompanyApplyLoanFileName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PersonApplyLoanFileID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PersonApplyLoanFileName { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_InjuryElectronicDocuments
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
		
		public  int? InjuryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? DocumentLabelID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Path { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ContractFileConfigID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SignPath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? LoanID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string signKeyword { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string signerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string signerIdno { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_InjuryFinancialWriteOff
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
		
		public  int? InjuryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserEntryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserCardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal LoanTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RepaymentTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ReturnBackMoneyTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal WriteOffTotalAmount { get; set; }
		
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
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? AdvanceAccountID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal Money { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Bank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BankAccount { get; set; }
		   
    }



	/// <summary>
	/// 借款总金额 - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_InjuryInfo
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? InjuryGID { get; set; }
		
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
		
		public  int? PerUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? UserGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserEntryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserCardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserSex { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserRegisteredResidence { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserWorkType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InjuryDescription { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? InjuryTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InjuryAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InjuryType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InjuryLevel { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? AdvanceApplyAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PartnerName { get; set; }
		
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
		
		public  string NoticeLetterFilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AuthorizationLetterFilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CompensationAgreementLetterFilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CompensationAgreementDisabilityLetterFilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? InjuryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProcessStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SealCompanyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SealCompanyGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SealCompanyAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SealCompanyPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SealCompanyBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SealCompanyBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SealCompanyNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationCompanyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CooperationCompanyGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsurancePeriod1 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsurancePolicyNo1 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceType1 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceCompany1 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? InsuranceStartDate1 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? InsuranceEndDate1 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsurancePeriod2 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsurancePolicyNo2 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceType2 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceCompany2 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? InsuranceStartDate2 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? InsuranceEndDate2 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AgentName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AgentIdcardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AgentBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AgentBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsDisabilityAssessed { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WorkInjuryReportFilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FailMsg { get; set; }
		
		/// <summary>
		/// 借款总金额
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal LoanTotalAmount { get; set; }
		
		/// <summary>
		/// 还款总金额
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RepaymentTotalAmount { get; set; }
		
		/// <summary>
		/// 销账总金额
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ReturnBackMoneyTotalAmount { get; set; }
		
		/// <summary>
		/// 员工医疗费
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal StaffMedicalExpense { get; set; }
		
		/// <summary>
		/// 员工误工费
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal StaffLossOfWorkingTime { get; set; }
		
		/// <summary>
		/// 员工住院津贴
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal StaffHospitalizationAllowance { get; set; }
		
		/// <summary>
		/// 员工住院伙食补助费
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal StaffHospitalizationMealAllowance { get; set; }
		
		/// <summary>
		/// 员工交通费
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal StaffTransportationCosts { get; set; }
		
		/// <summary>
		/// 员工护工费
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal StaffNursingFees { get; set; }
		
		/// <summary>
		/// 员工辅助器具费
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal StaffAssistiveDevices { get; set; }
		
		/// <summary>
		/// 员工一次性伤残补助金
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal StaffOneTimeDisabilityAllowance { get; set; }
		
		/// <summary>
		/// 员工一次性就业补助金
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal StaffOneTimeEmploymentSubsidy { get; set; }
		
		/// <summary>
		/// 员工一次性医疗补助金
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal StaffOneTimeMedicalSubsidy { get; set; }
		
		/// <summary>
		/// 员工三个一次性打包费用
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal StaffOneTimePackAmount { get; set; }
		
		/// <summary>
		/// 员工一次性工亡补助金
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal StaffOneTimeWorkDeathAllowance { get; set; }
		
		/// <summary>
		/// 员工鉴定费
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal StaffAppraisalCosts { get; set; }
		
		/// <summary>
		/// 员工其他费用
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal StaffOtherCosts { get; set; }
		
		/// <summary>
		/// 员工工伤赔付小计
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal StaffSubtotal { get; set; }
		
		/// <summary>
		/// 员工定期费用
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal StaffPeriodicFee { get; set; }
		
		/// <summary>
		/// 员工支付周期
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int StaffPaymentCycle { get; set; }
		
		/// <summary>
		/// 员工定期费用合计
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal StaffPeriodicTotalAmount { get; set; }
		
		/// <summary>
		/// 员工工伤费用合计支付
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal StaffTotalAmount { get; set; }
		
		/// <summary>
		/// 保险医疗费
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal InsuranceMedicalExpense { get; set; }
		
		/// <summary>
		/// 保险误工费
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal InsuranceLossOfWorkingTime { get; set; }
		
		/// <summary>
		/// 保险住院津贴
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal InsuranceHospitalizationAllowance { get; set; }
		
		/// <summary>
		/// 保险住院伙食补助费
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal InsuranceHospitalizationMealAllowance { get; set; }
		
		/// <summary>
		/// 保险交通费
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal InsuranceTransportationCosts { get; set; }
		
		/// <summary>
		/// 保险护工费
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal InsuranceNursingFees { get; set; }
		
		/// <summary>
		/// 保险辅助器具费
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal InsuranceAssistiveDevices { get; set; }
		
		/// <summary>
		/// 保险一次性伤残补助金
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal InsuranceOneTimeDisabilityAllowance { get; set; }
		
		/// <summary>
		/// 保险一次性就业补助金
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal InsuranceOneTimeEmploymentSubsidy { get; set; }
		
		/// <summary>
		/// 保险一次性医疗补助金
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal InsuranceOneTimeMedicalSubsidy { get; set; }
		
		/// <summary>
		/// 保险三个一次性打包费用
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal InsuranceOneTimePackAmount { get; set; }
		
		/// <summary>
		/// 保险一次性工亡补助金
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal InsuranceOneTimeWorkDeathAllowance { get; set; }
		
		/// <summary>
		/// 保险鉴定费
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal InsuranceAppraisalCosts { get; set; }
		
		/// <summary>
		/// 保险其它费用
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal InsuranceOtherCosts { get; set; }
		
		/// <summary>
		/// 保险工伤赔付小计
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal InsuranceSubtotal { get; set; }
		
		/// <summary>
		/// 保险定期费用
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal InsurancePeriodicFee { get; set; }
		
		/// <summary>
		/// 保险支付周期
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int InsurancePaymentCycle { get; set; }
		
		/// <summary>
		/// 保险定期费用合计
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal InsurancePeriodicTotalAmount { get; set; }
		
		/// <summary>
		/// 保险工伤费用合计支付
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal InsuranceTotalAmount { get; set; }
		
		/// <summary>
		/// 风险金医疗费
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RiskMedicalExpense { get; set; }
		
		/// <summary>
		/// 风险金误工费
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RiskLossOfWorkingTime { get; set; }
		
		/// <summary>
		/// 保险金住院津贴
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RiskHospitalizationAllowance { get; set; }
		
		/// <summary>
		/// 风险金住院伙食补助费
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RiskHospitalizationMealAllowance { get; set; }
		
		/// <summary>
		/// 风险金交通费
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RiskTransportationCosts { get; set; }
		
		/// <summary>
		/// 风险金护工费
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RiskNursingFees { get; set; }
		
		/// <summary>
		/// 风险金辅助器具费
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RiskAssistiveDevices { get; set; }
		
		/// <summary>
		/// 风险金一次性伤残补助金
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RiskOneTimeDisabilityAllowance { get; set; }
		
		/// <summary>
		/// 风险金一次性就业补助金
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RiskOneTimeEmploymentSubsidy { get; set; }
		
		/// <summary>
		/// 风险金一次性医疗补助金
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RiskOneTimeMedicalSubsidy { get; set; }
		
		/// <summary>
		/// 风险金三个一次性打包费用
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RiskOneTimePackAmount { get; set; }
		
		/// <summary>
		/// 风险金一次性工亡补助金
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RiskOneTimeWorkDeathAllowance { get; set; }
		
		/// <summary>
		/// 风险金鉴定费
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RiskAppraisalCosts { get; set; }
		
		/// <summary>
		/// 风险金其它费用
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RiskOtherCosts { get; set; }
		
		/// <summary>
		/// 风险金工伤赔付小计
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RiskSubtotal { get; set; }
		
		/// <summary>
		/// 风险金定期费用
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RiskPeriodicFee { get; set; }
		
		/// <summary>
		/// 风险金支付周期
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int RiskPaymentCycle { get; set; }
		
		/// <summary>
		/// 风险金定期费用合计
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RiskPeriodicTotalAmount { get; set; }
		
		/// <summary>
		/// 风险金工伤费用合计支付
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RiskTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsArbitration { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? DocumentLabelType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? ShowTime { get; set; }
		
		/// <summary>
		/// 诊断结果
		/// </summary>		
		
		public  string Diagnosis { get; set; }
		
		/// <summary>
		/// 就诊医院
		/// </summary>		
		
		public  string HospitalName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InjuryPart { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobItem { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EditTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsPerson { get; set; }
		
		/// <summary>
		/// 是否骨折
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsFracture { get; set; }
		
		/// <summary>
		/// 预估赔付金额
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EstimatedClaim { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal AverageWage { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ProvincialAverageWage { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_InjuryLoan
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
		
		public  int? InjuryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? Money { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Path { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BillPath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? AddTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Type { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Content { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_InjuryPayment
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
		
		public  int? InjuryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PerUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Year { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? ApplicationTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal PaymentAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CostName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PaymentSubject { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Purpose { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserCardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PayeeName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PayeeBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PayeeBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? PayTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Message { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_InjuryRepay
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
		
		public  int? InjuryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? InjuryGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Content { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Bank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BankAccount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RepaymentTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal LoanTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SpendTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Path { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RepayMoney { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_InsuranceCompanyBill
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
		
		public  int? RecordCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? EmployeeCompanyId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? EmployeeCompanyGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmployeeCompanyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmployeeCompanyAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmployeeCompanyPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmployeeCompanyNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmployeeCompanyBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmployeeCompanyBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CooperationUnitGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TotalServiceFee { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TotalSheBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TotalGongJiJin { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? BillMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeeSumGongJiJin { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitSumGongJiJin { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeeSumSheBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitSumSheBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProcessStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsInvoicePaid { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsInvoicePrinted { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CompanyInvoiceId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? ActualAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_InsuranceCustomerConfig
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Title { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceCompany { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? InsuranceSigningUnitID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string LevelThreePlan { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string LevelFourPlan { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string LevelFivePlan { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsCommon { get; set; }
		
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
		
		public  bool IsMain { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_InsuranceEmployee
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? EmployeeGID { get; set; }
		
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
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IdcardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IdcardAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Sex { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Nation { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Phone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Education { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? EmployeeCompanyId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? EmployeeCompanyGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ShemeType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? YlSchemeId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? StartDateGongJiJin { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EndDateGongJiJin { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? StartDateSheBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EndDateSheBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Province { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ProvinceCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string City { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CityCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Enable { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? YlSupplierIdSheBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BaseAmountYangLao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BaseAmountYiLiao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BaseAmountShiYe { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BaseAmountShengYu { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BaseAmountGongShang { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BaseAmountCanBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitPercentYangLao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitPercentYiLiao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitPercentShiYe { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitPercentShengYu { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitPercentGongShang { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitPercentCanBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeePercentYangLao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeePercentYiLiao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeePercentShiYe { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeePercentShengYu { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeePercentGongShang { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeePercentCanBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeeAmountCanBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeeAmountDaBingYiLiao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeeAmountYiLiaoBuChong { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitAmountCanBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitAmountDaBingYiLiao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitAmountYiLiaoBuChong { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? YlSupplierIdGongJiJin { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BaseAmountGongJiJin { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeePercentGongJiJin { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitPercentGongJiJin { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? YlRule { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? TimeNode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProcessStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BaseDifferenceAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BaseRemark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsCustomize { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_InsuranceEmployeeBill
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
		
		public  int? PartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? BillMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IdcardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IdcardAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Sex { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Nation { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Phone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Education { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? EmployeeCompanyId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? EmployeeCompanyGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmployeeCompanyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeeCompanySocialFee { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool EmployeeCompanySocialEnabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmployeeCompanyAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmployeeCompanyPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmployeeCompanNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmployeeCompanyBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmployeeCompanyBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProcessStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ShemeType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? YlSchemeId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? StartDateGongJiJin { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EndDateGongJiJin { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? StartDateSheBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EndDateSheBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Province { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ProvinceCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string City { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CityCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Enable { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? YlSupplierIdSheBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BaseAmountYangLao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BaseAmountYiLiao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BaseAmountShiYe { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BaseAmountShengYu { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BaseAmountGongShang { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BaseAmountCanBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitPercentYangLao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitPercentYiLiao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitPercentShiYe { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitPercentShengYu { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitPercentGongShang { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitPercentCanBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeePercentYangLao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeePercentYiLiao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeePercentShiYe { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeePercentShengYu { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeePercentGongShang { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeePercentCanBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeeAmountCanBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeeAmountDaBingYiLiao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeeAmountYiLiaoBuChong { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitAmountCanBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitAmountDaBingYiLiao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitAmountYiLiaoBuChong { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? YlSupplierIdGongJiJin { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BaseAmountGongJiJin { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeePercentGongJiJin { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitPercentGongJiJin { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? YlRule { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? TimeNode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ServiceBaseAmountGongJiJin { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ServiceBaseAmountSheBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitAmountYangLao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeeAmountYangLao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitAmountYiLiao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeeAmountYiLiao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitAmountShiYe { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeeAmountShiYe { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitAmountShengYu { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeeAmountShengYu { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitAmountGongShang { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeeAmountGongShang { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitAmountCountCanBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeeAmountCountCanBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitAmountGongJiJin { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeeAmounGongJiJin { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ServiceFeeAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeeSumSheBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitSumSheBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TotalAmountSheBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TotalAmountGongJiJin { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CooperationUnitGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationUnitName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationUnitPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationUnitAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationUnitBankName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationUnitBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationUnitNo { get; set; }
		   
    }



	/// <summary>
	/// InsuranceNameRecordStatusEnumType - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_InsuranceOderNameRecord
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? InsuranceOrderID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IdCardNo { get; set; }
		
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
		
		public  string JobType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? JobLevelType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CertificateType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SigningUnitName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobCompanyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FromType { get; set; }
		
		/// <summary>
		/// InsuranceNameRecordStatusEnumType
		/// </summary>		
		
		public  InsuranceNameRecordStatusEnumType? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PlanName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? InsuracneStartDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? InsuracneEndDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? InsuranceAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// InsuranceActionEnumType
		/// </summary>		
		
		public  InsuranceActionEnumType? ActionType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// InsuranceMethodEnumType
		/// </summary>		
		
		public  InsuranceMethodEnumType? MethodType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsAddEmp { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserEntryID { get; set; }
		   
    }



	/// <summary>
	/// InsuranceOrderEnumType - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_InsuranceOrder
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
		public  string InsuranceCompany { get; set; }
		
		/// <summary>
		/// InsuranceOrderEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  InsuranceOrderEnumType OrderType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string OrderNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string InsuranceNo { get; set; }
		
		/// <summary>
		/// BusinessInsuranceStatusEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  BusinessInsuranceStatusEnumType Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsCommon { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SigningUnitID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? StartDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? EndDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_InsuranceOrderPlan
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
		public  Guid InsuranceOrderID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int GnteeId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string GnteeName { get; set; }
		   
    }



	/// <summary>
	/// InsurancePushStatusEnumType - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_InsurancePush
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
		public  string OrderNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? AddPushTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int AddRecordCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string AddIndecaseId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? DelPushTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int DelRecordCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string DelIndecaseId { get; set; }
		
		/// <summary>
		/// InsurancePushStatusEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  InsurancePushStatusEnumType Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int AddFailCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int DelFailCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int GnteeId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string GnteeName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid InsuranceOrderPlanID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? AddResCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AddResMsg { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? DelResCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string DelResMsg { get; set; }
		
		/// <summary>
		/// InsurancePushStatusEnumType
		/// </summary>		
		
		public  InsurancePushStatusEnumType? AddPushStatus { get; set; }
		
		/// <summary>
		/// InsurancePushStatusEnumType
		/// </summary>		
		
		public  InsurancePushStatusEnumType? DelPushStatus { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_InsurancePushRecord
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
		public  string Custemername { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Custemernum { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Custemersex { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Custemerage { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Guyuangongzhong { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Sjygdw { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Ldhtdw { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Zhiyeleibei { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Custemertype { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid UserEntryInsuracneRecordID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid InsurancePushID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsAdd { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsPushSuc { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserEntryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? BusinessInsuranceRecordID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsMain { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsApiPush { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? PushTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsPushing { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_InsuranceScheme
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SchemeGID { get; set; }
		
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
		
		public  string Province { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ProvinceCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string City { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CityCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? YlSupplierIdSheBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BaseAmountYangLao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BaseAmountYiLiao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BaseAmountShiYe { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BaseAmountShengYu { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BaseAmountGongShang { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BaseAmountCanBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitPercentYangLao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitPercentYiLiao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitPercentShiYe { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitPercentShengYu { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitPercentGongShang { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitPercentCanBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeePercentYangLao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeePercentYiLiao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeePercentShiYe { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeePercentShengYu { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeePercentGongShang { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeePercentCanBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeeAmountCanBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeeAmountDaBingYiLiao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeeAmountYiLiaoBuChong { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitAmountCanBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitAmountDaBingYiLiao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitAmountYiLiaoBuChong { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? YlSupplierIdGongJiJin { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BaseAmountGongJiJin { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeePercentGongJiJin { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitPercentGongJiJin { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? YlRule { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? TimeNode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Enable { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BaseDifferenceAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BaseRemark { get; set; }
		   
    }



	/// <summary>
	/// 是否有收费单位 - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_InsuranceSupplier
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SupplierGID { get; set; }
		
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
		[SugarColumn(IsNullable=false)]
		public  bool Enable { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ServiceAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ParentId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsCombine { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmployeeCompanyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? EmployeeCompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmployeeCompanyNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmployeeCompanyBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmployeeCompanyBankNo { get; set; }
		
		/// <summary>
		/// 是否有收费单位
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsJoint { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JointCompanyName { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_InsuranceSupplierBill
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
		
		public  int? ProcessStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? RecordCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? YlSupplierId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? YlSupplierGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string YlSupplierName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InvoiceCompanyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SupplierCompanyNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SupplierCompanyBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SupplierCompanyBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CooperationUnitGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TotalServiceFee { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TotalSheBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TotalGongJiJin { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? BillMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeeSumGongJiJin { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitSumGongJiJin { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeeSumSheBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitSumSheBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsInvoicePaid { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsInvoicePrinted { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SheetId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CompanyInvoiceId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsPaidAlready { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? SheBaoBill { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? GongJiJinBill { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? ActualAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsSettlementPass { get; set; }
		   
    }



	/// <summary>
	/// InvoiceTypeEnumType - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_Invoice
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
		public  Guid CustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string IssuerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string IssuerCompanyNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string IssuerPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string IssuerAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string IssuerBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string IssuerBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string RecipientName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string RecipientCompanyNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string RecipientPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string RecipientAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string RecipientBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string RecipientBankNo { get; set; }
		
		/// <summary>
		/// InvoiceTypeEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  InvoiceTypeEnumType InvoiceType { get; set; }
		
		/// <summary>
		/// PrintTemplateEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  PrintTemplateEnumType BusinessType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Title { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsAddedTax { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsAdditionalTax { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal AddedTaxAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal AdditionalTaxAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal AddedTaxDiscountAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal AdditionalTaxDiscountAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ChangeAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsLocked { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsIssued { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int RecordCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ServiceFeeAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Description { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? ApplyTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? LockedTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? IssuedTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? SalaryMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid IssuerUnitID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid RecipientUnitID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal AddedTaxPointAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal AdditionalTaxPointAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal InvoiceAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal AddedTaxSumAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal AdditionTaxSumAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SfAddedTaxAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SfAddedTaxPointAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SfPersonalTaxAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SfPersonalTaxPointAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsSf { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SfOtherPaidAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SfSocialAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SfTaxRefundAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsCancel { get; set; }
		
		/// <summary>
		/// InvoiceStatusEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  InvoiceStatusEnumType Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SalaryCustomerID { get; set; }
		
		/// <summary>
		/// 除了企业工资发票和新建发票，其他渠道过来的发票只有开具和冲红功能
		/// </summary>		
		
		public  bool IsSpecial { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string LegalCustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
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
		
		public  int? CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ApplyUserName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ApplyUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ApplyUserFullName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? SalaryAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? SalaryRecordCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? SerivceFeeRecordCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? BothRecordCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsReset { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_InvoiceAttachFile
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
		public  Guid InvoiceID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FileName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FileOssUrl { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_InvoiceCompany
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
		public  string CompanyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CompanyNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string LegalPerson { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Address { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Phone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Bank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string BankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsEnabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int UserID { get; set; }
		   
    }



	/// <summary>
	/// InvoiceTypeEnumType - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_InvoiceConfig
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
		public  string Title { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string InvoiceTitle { get; set; }
		
		/// <summary>
		/// InvoiceTypeEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  InvoiceTypeEnumType InvoiceType { get; set; }
		
		/// <summary>
		/// PrintTemplateEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  PrintTemplateEnumType PrintTemplate { get; set; }
		
		/// <summary>
		/// InvoiceDataTypeEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  InvoiceDataTypeEnumType DataType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsIncludeSalary { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsIncludeServiceFee { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid CompanyGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TaxPointSalaryValueAdded { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TaxPointSalaryAdditional { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TaxPointServiceFeeValueAdded { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TaxPointServiceFeeAdditional { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsEnabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CustomerGID { get; set; }
		   
    }



	/// <summary>
	/// InvoiceTaxDataTypeEnumType - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_InvoiceTaxRecord
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
		public  Guid InvoiceID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int TaxType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BaseAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ValueAddedTaxPointPercent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ValueAddedAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal AdditionalTaxPointPercent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal AdditionalTaxAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// InvoiceTaxDataTypeEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  InvoiceTaxDataTypeEnumType DataType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ValueAddedTaxAmount { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_JobCandidatePool
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true)]
		public  int JobID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal Score { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int Rank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ScoreDetail { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WorkTypeHit { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? CredibilityScore { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  byte SeekStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string DistanceText { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string LastEntryWorkName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  byte CertMatch { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  long BatchID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CalcTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  byte SourceType { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_JobClass
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? GID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Enabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Sort { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsTag { get; set; }
		   
    }



	/// <summary>
	/// 综合工资 - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_Jobs
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Title { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Tabs { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TabNames { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? JCID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Sex { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Ages { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Address { get; set; }
		
		/// <summary>
		/// 综合工资
		/// </summary>		
		
		public  int? MinMoney { get; set; }
		
		/// <summary>
		/// 综合工资
		/// </summary>		
		
		public  int? MaxMoney { get; set; }
		
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
		
		public  string WorkTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Payday { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Structure { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WorkMode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WorkContent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Foot { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Hostel { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RestMode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Coverall { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Workshop { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Intensity { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BusLine { get; set; }
		
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
		
		public  string Remarks { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserAsk { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? UpTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? MinAge { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? MaxAge { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? SynTime { get; set; }
		
		/// <summary>
		/// 岗位优先
		/// </summary>		
		
		public  bool First { get; set; }
		
		/// <summary>
		/// 需求数量
		/// </summary>		
		
		public  int? PCount { get; set; }
		
		/// <summary>
		/// 保招数量
		/// </summary>		
		
		public  int? ECount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Flow { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string NewMoney { get; set; }
		
		/// <summary>
		/// 绩效
		/// </summary>		
		
		public  string JobAchieve { get; set; }
		
		/// <summary>
		/// 岗位补贴
		/// </summary>		
		
		public  string JobSubsidy { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string NightSubsidy { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string HostelCondition { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OtherAsk { get; set; }
		
		/// <summary>
		/// 全勤
		/// </summary>		
		
		public  string Regularity { get; set; }
		
		/// <summary>
		/// 高温补贴
		/// </summary>		
		
		public  string HighSubsidy { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobAge { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WorkTimeAbout { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool MinShow { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ShowTags { get; set; }
		
		/// <summary>
		/// 佣金
		/// </summary>		
		
		public  decimal? Commission { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CommissionDays { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SalaryStructure { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SpecialTag { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Number { get; set; }
		
		/// <summary>
		/// 幸福职展示
		/// </summary>		
		
		public  bool XFZShow { get; set; }
		
		/// <summary>
		/// 劳务经纪人展示
		/// </summary>		
		
		public  bool LWShow { get; set; }
		
		/// <summary>
		/// 是否匿名展示
		/// </summary>		
		
		public  bool AnonyShow { get; set; }
		
		/// <summary>
		/// 匿名展示的单位名称
		/// </summary>		
		
		public  string AnonyCompany { get; set; }
		
		/// <summary>
		/// 是否优质
		/// </summary>		
		
		public  bool IsQuality { get; set; }
		
		/// <summary>
		/// 是否日结工
		/// </summary>		
		
		public  bool IsDayOver { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsJoint { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? AttendancePlan { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? NightAttendancePlan { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Integrals { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SubTitle { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EndTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Distance { get; set; }
		
		/// <summary>
		/// 技能类型(1普工2技工)
		/// </summary>		
		
		public  int? SkillType { get; set; }
		
		/// <summary>
		/// 技能
		/// </summary>		
		
		public  string JobSkills { get; set; }
		   
    }



	/// <summary>
	/// 日结岗位编号 - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_JobsDayWork
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 日结岗位编号
		/// </summary>		
		
		public  string JobNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Title { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Tabs { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TabNames { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Sex { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Ages { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Address { get; set; }
		
		/// <summary>
		/// 综合工资
		/// </summary>		
		
		public  int? MinMoney { get; set; }
		
		/// <summary>
		/// 综合工资
		/// </summary>		
		
		public  int? MaxMoney { get; set; }
		
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
		
		public  string WorkTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WorkContent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Requirement { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Post { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RestMode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Coverall { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Workshop { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Intensity { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BusLine { get; set; }
		
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
		
		public  string Remarks { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserAsk { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? UpTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? MinAge { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? MaxAge { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? SynTime { get; set; }
		
		/// <summary>
		/// 岗位优先
		/// </summary>		
		
		public  bool First { get; set; }
		
		/// <summary>
		/// 需求数量
		/// </summary>		
		
		public  int? PCount { get; set; }
		
		/// <summary>
		/// 保招数量
		/// </summary>		
		
		public  int? ECount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Flow { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string NewMoney { get; set; }
		
		/// <summary>
		/// 绩效
		/// </summary>		
		
		public  string JobAchieve { get; set; }
		
		/// <summary>
		/// 岗位补贴
		/// </summary>		
		
		public  string JobSubsidy { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string NightSubsidy { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string HostelCondition { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OtherAsk { get; set; }
		
		/// <summary>
		/// 全勤
		/// </summary>		
		
		public  string Regularity { get; set; }
		
		/// <summary>
		/// 高温补贴
		/// </summary>		
		
		public  string HighSubsidy { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobAge { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WorkTimeAbout { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool MinShow { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ShowTags { get; set; }
		
		/// <summary>
		/// 佣金
		/// </summary>		
		
		public  decimal? Commission { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CommissionDays { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SalaryStructure { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SpecialTag { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Number { get; set; }
		
		/// <summary>
		/// 幸福职展示
		/// </summary>		
		
		public  bool XFZShow { get; set; }
		
		/// <summary>
		/// 劳务经纪人展示
		/// </summary>		
		
		public  bool LWShow { get; set; }
		
		/// <summary>
		/// 是否匿名展示
		/// </summary>		
		
		public  bool AnonyShow { get; set; }
		
		/// <summary>
		/// 匿名展示的单位名称
		/// </summary>		
		
		public  string AnonyCompany { get; set; }
		
		/// <summary>
		/// 是否优质
		/// </summary>		
		
		public  bool IsQuality { get; set; }
		
		/// <summary>
		/// 是否日结工
		/// </summary>		
		
		public  bool IsDayOver { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsJoint { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? AttendancePlan { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? NightAttendancePlan { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? TotalNumber { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? StaffNumber { get; set; }
		   
    }



	/// <summary>
	/// 秒数 - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_JobsVisit
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
		
		public  int? JobID { get; set; }
		
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



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_JobVideo
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? GID { get; set; }
		
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
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Path { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? JobID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ApprovalName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ApprovalType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? AuditTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Message { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Content { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PlanName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? InsuranceType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceCompany { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? InsuranceID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceName { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_JointAudit
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
		
		public  int? CustomerID { get; set; }
		
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
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? AuditTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? AuditUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PartnerIDS { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PartnerNames { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AuditRemarks { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? OperateModel { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JointObjType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? JointCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_LabelManagement
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
		
		public  int? PartnerID { get; set; }
		
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
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Type { get; set; }
		
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
		
		public  string Content { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Path { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_Labor
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? UserGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FullName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserName { get; set; }
		
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
		
		public  string Phone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Picture { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string NickName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Enabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Sex { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string City { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Province { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Qrcode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? Money { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? LastTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string LastIP { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_LaborOpenIds
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true)]
		public  int UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int FromID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string OpenIdType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OpenId { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_LaborToken
    {
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true)]
		public  string Token { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FromType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EndTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserAgent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IPAddress { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_LeaveReason
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
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Options { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Sort { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Type { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_Message
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
		
		public  int? PersonnelID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Content { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Type { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Title { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Reply { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EditTime { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_NoticeTemplate
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
		
		public  string TID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string KidList { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Type { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AppType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SceneDesc { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Fields { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Keywords { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_Notification
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
		
		public  int? PartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? PartnerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Content { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Type { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TypeName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Title { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserIDS { get; set; }
		   
    }



	/// <summary>
	/// 合作伙伴 - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_Partner
    {
		
		/// <summary>
		/// 合作伙伴
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int PartnerID { get; set; }
		
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
		
		public  bool Enabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CIDS { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CNames { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? EntryDays { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? LimitMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PartnerType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PartnerLevel { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsBudget { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CityPartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsBusinessPartner { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Integral { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_PartnerCustomer
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true)]
		public  int CID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int PID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_PaymentAccount
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
		public  Guid PaymentSignUnitID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid PaymentApplicationID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string AccountName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string AccountNo { get; set; }
		
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
		
		public  DateTime? DeleteTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? UpdateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UseOrgKey { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UseOrgCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string UseOrgName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string BusinessTypeName { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_PaymentAdmin
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
		public  int UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid UserGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string SecretKey { get; set; }
		
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
		
		public  DateTime? DeleteTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsValidate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsLock { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? LockTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FailMsg { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? FailTimes { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? UpdateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string RealName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string DispalyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid PaymentApplicationID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_PaymentAdminSignUnit
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
		public  Guid PaymentAdminID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid PaymentSignUnitID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsCanView { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsCanPay { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsCanManage { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_PaymentApplication
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
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int BankType { get; set; }
		
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
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsDefault { get; set; }
		   
    }



	/// <summary>
	/// PaymentOrderTransStatusEnumType - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_PaymentOrder
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
		public  string CompanyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string SignUnitName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string SignUnitNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CustomerNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string BusinessID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int SalaryType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int TotalCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TotalAmount { get; set; }
		
		/// <summary>
		/// PaymentOrderTransStatusEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  PaymentOrderTransStatusEnumType Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? PushTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FailMsg { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int PushCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? TransTime { get; set; }
		
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
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// TransactionDataTypeEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  TransactionDataTypeEnumType DataType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string BillsPayOrderNo { get; set; }
		
		/// <summary>
		/// CzyPayTypeEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  CzyPayTypeEnumType PayType { get; set; }
		
		/// <summary>
		/// CzyPayChannelEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  CzyPayChannelEnumType PayChannel { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool WhetherUrgent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string PayAccountNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
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
		public  string BusinessTypeName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? PaymentDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? UpdateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? PaymentAccountID { get; set; }
		
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
		
		public  string RelPaymentDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FailReasons { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PayFailReason { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RefundAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RefundDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AppName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AppID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid PaymentApplicationID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid PaymentSignUnitID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  byte[] Timestamp { get; set; }
		   
    }



	/// <summary>
	/// PaymentOrderTransStatusEnumType - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_PaymentOrderDetail
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
		public  string BusinessID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CardID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string BankName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string BankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TransAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? TransTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid PaymentOrderID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string BillsPayOrderNo { get; set; }
		
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
		
		public  string PayFailReason { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RefundAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RefundDate { get; set; }
		
		/// <summary>
		/// PaymentOrderTransStatusEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  PaymentOrderTransStatusEnumType Status { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_PaymentSignUnit
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
		public  string UnitName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string UnitNo { get; set; }
		
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
		
		public  DateTime? DeleteTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? UpdateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid PaymentApplicationID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_Personnel
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? UserGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FullName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserName { get; set; }
		
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
		
		public  string Phone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Picture { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string NickName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Enabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Sex { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string City { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Province { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CIDS { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CNames { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Qrcode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PartnerIDS { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PartnerNames { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? LastTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string LastIP { get; set; }
		
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
		
		public  string PartnerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool AddJoin { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsBindMP { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserFroms { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PrivateQrcode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ClientQrcode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsDefault { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JDYNumber { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_PersonnelFunction
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
		
		public  int? Type { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Number { get; set; }
		
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
		
		public  string ToPage { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Icon { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Unit { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CountField { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IconSize { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IconClass { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IconColor { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsPublic { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserTypes { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserTypeNames { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CutTypes { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_PersonnelFunctionItems
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PFID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CompanyID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_PersonnelItems
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? GID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Qrcode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Enabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsJoin { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsManage { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JDYDataID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JDYJson { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? BusinessModel { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_PersonnelItemsLog
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PersonnelID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PersonnelName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Content { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_PersonnelOpenIds
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true)]
		public  int UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int FromID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string OpenIdType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OpenId { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_PersonnelToken
    {
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true)]
		public  string Token { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FromType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EndTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserAgent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IPAddress { get; set; }
		   
    }



	/// <summary>
	/// PrepaidApplyEnumType - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_PrepaidApplyRecord
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
		[SugarColumn(IsNullable=false)]
		public  int UserEntryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid PrepaidPartnerAccountID { get; set; }
		
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
		[SugarColumn(IsNullable=false)]
		public  Guid CustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid SigningUnitID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string IdCardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string BankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Bank { get; set; }
		
		/// <summary>
		/// PrepaidApplyEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  PrepaidApplyEnumType ApplyType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ApplyAmount { get; set; }
		
		/// <summary>
		/// PrepaidRecordStatusEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  PrepaidRecordStatusEnumType Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? ShiftEndTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? ShiftStartTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? ShiftHours { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? ShiftMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? LunchHours { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? NightShiftSubsidy { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? DinnerHours { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? MoneyHours { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PerUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? WorkType { get; set; }
		
		/// <summary>
		/// PrepaidRepayStatusEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  PrepaidRepayStatusEnumType RepayStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RepayTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsSinglePushSuc { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? AuditTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Msg { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? SendType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BankUserName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PdfFilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SignedFilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Phone { get; set; }
		
		/// <summary>
		/// PrepaidCashStatusEnumType
		/// </summary>		
		
		public  PrepaidCashStatusEnumType? CashStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? SendTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsNotice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsBadRepay { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? BadRepayAmount { get; set; }
		
		/// <summary>
		/// BadRepayStatusEnumType
		/// </summary>		
		
		public  BadRepayStatusEnumType? BadRepayStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? BadRepayTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string DayWorkType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? StructureID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? RepayCompleteTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EntryNo { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_PrepaidInterestRecord
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
		public  Guid PrepaidPartnerAccountID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid PrepaidApplyRecordID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid PartnerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid CustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string PartnerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? BillMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ApplyAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal PendingAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RepaymentAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BasePercent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal FloatPercent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ActualPercent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int Days { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string IdCardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? StartDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? EndDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int UserEntryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? SendDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal MainAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal MainInterestAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? RepaymentDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_PrepaidInterestReport
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
		public  Guid PrepaidPartnerAccountID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid PartnerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string PartnerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int BillYear { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal PendingYearAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal PendingMonthAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal InterestYearAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal InterestMonthAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BasePercent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal FloatPercent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? BillMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? ApplyYearAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? ApplyMonthAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		   
    }



	/// <summary>
	/// PrepaidPaymentEnumType - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_PrepaidPartnerAccount
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
		public  Guid PartnerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal CashAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TotalAmount { get; set; }
		
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
		public  bool IsEnabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BadAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  byte[] Timestamp { get; set; }
		
		/// <summary>
		/// PrepaidPaymentEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  PrepaidPaymentEnumType PaymentType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BasePercent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal FloatPercent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SigningUnitID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SpecialPartnerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CompanyID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_PrepaidRepaymentRecord
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
		public  Guid PrepaidApplyRecordID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid SalaryCustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid SalaryRecordID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? RepaymentTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsSuc { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int UserEntryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsRetrunToAccount { get; set; }
		   
    }



	/// <summary>
	/// PrepaidSubAccountStatusEnumType - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_PrepaidSubAccount
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
		public  string SignUnitName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? LastSendTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal Balance { get; set; }
		
		/// <summary>
		/// PrepaidSubAccountStatusEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  PrepaidSubAccountStatusEnumType Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsEnable { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string SubAccountNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal MinBalance { get; set; }
		   
    }



	/// <summary>
	/// ProfitBusInsuranceCountEnunType - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_ProfitConfig
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
		public  Guid CustomerGID { get; set; }
		
		/// <summary>
		/// ProfitBusInsuranceCountEnunType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int BusInsuranceCountType { get; set; }
		
		/// <summary>
		/// ProfitAgentCountEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  ProfitAgentCountEnumType AgentCountType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TaxPointPercent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal DifferenceBaseAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ProfitTaxPointPercent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BusInsuranceAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BusInsurance5Amount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RiskAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BusinessAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal DeductionAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SubsidyAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal PreBusInsuranceTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal PreRiskAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsHasInvoice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsEnabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal AgentPaidPercent { get; set; }
		
		/// <summary>
		/// 新业务开发费比例
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BusinessPercent { get; set; }
		
		/// <summary>
		/// 联营驻厂管理费
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal JoinManagePercent { get; set; }
		
		/// <summary>
		/// 客情费比例
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal CustomerThingPercent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SocialRepayRiskBaseAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal DeathRiskAmount { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_ProfitManage
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
		public  Guid PartnerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string PartnerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? ProfitMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? TaxMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int RecordCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int UnitSalaryServiceFeeRecordCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int InJobRecordCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitSalaryTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitSocialTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ServiceFeeTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BusInsuranceCurrentTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BusInsuranceLastTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BusInsuranceNextTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BusInsuranceSumAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RiskTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal AgentPaidTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal AgentPaidTaxAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal DisabilityTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BusinessTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal DeductionAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SubsidyAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal GrossAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal GrossContainTaxAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid SigningUnitID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string SigningUnitName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string SigningUnitBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string SigningUnitBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string SigningUnitPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string SigningUnitAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string SigningUnitNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string FilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string FileName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ServiceFeeRecordCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BusInsuranceBalanceTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? ProfitAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string DeductionRemark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SubsidyRemark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? CustomerThingTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? JoinManageTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? InterestTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal JoinManagePaidAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal JoinManageIncomeAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SocialRepayRiskAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CityPartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal DeathRiskTotalAmount { get; set; }
		   
    }



	/// <summary>
	/// 残保金 - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_ProfitManageRecord
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
		public  Guid CustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid PartnerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string PartnerNamer { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? JoinPartnerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JoinPartnerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string IdCardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? ProfitMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? TaxMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid SigningUnitID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string SigningUnitName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string WorkClass { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string WorkName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string PlanName1 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string PlanName2 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? EntryDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? LeaveDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BusInsuranceCurrentAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BusInsuranceLastAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BusInsuranceNextAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BusInsuranceSumAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BusInsuranceBalanceAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RiskCurrentAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int InJobDays { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitSalaryTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ServiceFeeTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitSocialTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? StartDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? EndDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int Days { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsInJob { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid GrossMangeID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsContainSalary { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Msg { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsRiskAndBusInsurance { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal AgentPaidTaxAmount { get; set; }
		
		/// <summary>
		/// 残保金
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal DisabilityAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BusinessAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal JoinManageAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal CustomerThingAmount { get; set; }
		
		/// <summary>
		/// 待缴税费
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal AgentPaidAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsAddMatchSuc { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsDelMatchSuc { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string FromType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string RealPartnerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsDayMatch { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SocialRepayRiskAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Salesman { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CityPartnerTitle { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? RealPartnerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CityPartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CustomerBusinessModel { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerBusinessModelName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BusInsuranceBaseAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SocialRepayRiskBaseAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsRetire { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? RetireDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Age { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Sex { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal DeathRiskCurrentAmount { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_ProfitPartnerConfig
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
		public  decimal DeductionAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SubsidyAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string DeductionRemark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SubsidyRemark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid PartnerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int PartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsEnabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_ProfitSigningUnitReport
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
		public  DateTime? BillMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid SigningUnitID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string SigningUnitName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsBusInsurancePaid { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsRiskPaid { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsRiskPrint { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsBusinessPaid { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsBusinessPrint { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? BusInsuranceBlanceAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? BusInsuranceSheetID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? BusinessSheetID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? BusinessInvoiceID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? RiskSheetID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? RiskInvoiceID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BusInsuranceAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RiskAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BusinessAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsPush { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_ProfitSummary
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? BillMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? ProfitAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? CityPartnerAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? TotalAmount { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_RegionAddress
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ParentCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Code { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Level { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		   
    }



	/// <summary>
	/// CashSendStatusEnumType - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_SalaryCashSendRecord
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SalaryCustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SalaryRecordID { get; set; }
		
		/// <summary>
		/// CashSendStatusEnumType
		/// </summary>		
		
		public  CashSendStatusEnumType? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Reason { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? SendAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AgentName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AgentIdCardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AgentBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AgentBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SucFilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SignerOssUrl { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SignKeyword { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? SignTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? FailRecordID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Msg { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CashHash { get; set; }
		
		/// <summary>
		/// SalaryCashAuditStatusEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  SalaryCashAuditStatusEnumType AuditStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CashType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CashRemark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? DelayRecordID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AuditFailMsg { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FromType { get; set; }
		
		/// <summary>
		/// SalaryCashAgentStatusEnumType
		/// </summary>		
		
		public  SalaryCashAgentStatusEnumType? AgentStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AgentTransPath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? AgentTransTime { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_SalaryConfig
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true)]
		public  Guid Id { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CompanyGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsVerifyEmpInfo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsCalculateTax { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsVerifyPaySlip { get; set; }
		
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
		
		public  Guid? CustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AgentName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AgentIdCardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AgentBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AgentBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsMatchZeroSalary { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsCostSettlement { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsSalarySettlement { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsSalaryYearRepayment { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CombineCompanyGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsMatchCmbcBankCard { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsContractDelay { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsNeedSalarySlip { get; set; }
		   
    }



	/// <summary>
	/// 是否平台垫付 - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_SalaryCustomer
    {
		
		/// <summary>
		/// 是否平台垫付
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string SealCompanyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal CompanySocialSecurityAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ManagementFeeAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal CompanySendSalaryAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal CompanyDeductionAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal DeductionNoTaxAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SubsidyAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SubsidyNoTaxAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ServiceFeeAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ActualSalaryAmount { get; set; }
		
		/// <summary>
		/// SalaryStatusEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  SalaryStatusEnumType Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? SalaryMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? TaxMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? SendMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string WorkShop { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int SalaryBatchType { get; set; }
		
		/// <summary>
		/// SalarySendTypeEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  SalarySendTypeEnumType SalarySendType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid CustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CustomerCompanyNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CustomerCompanyBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CustomerCompanyBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CustomerCompanyAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CustomerCompanyPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string SigningUnitCompanyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string SigningUnitCompanyNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string SigningUnitCompanyBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string SigningUnitCompanyBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string SigningUnitCompanyAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string SigningUnitCompanyPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CooperationUnitCompanyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CooperationUnitCompanyNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CooperationUnitCompanyBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CooperationUnitCompanyBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CooperationUnitCompanyAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CooperationUnitCompanyPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal InnerServiceFeeAmount { get; set; }
		
		/// <summary>
		/// 企业应发合计，企业应发工资-企业扣款
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal CompanySendSalarySumAmount { get; set; }
		
		/// <summary>
		/// 应收账款合计金额
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal AccountsReceivableSumAmount { get; set; }
		
		/// <summary>
		/// 公司结算金额
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal CompanySettlementSumAmount { get; set; }
		
		/// <summary>
		/// SalaryTypeEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  SalaryTypeEnumType SalaryType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal MonthNetSalaryAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int RecordCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ServiceFeeRecordCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsCustomerPaid { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsCompanyPosted { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid SigningUnitID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid CooperationUnitID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsMatchZeroSalary { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsMainBatchSalary { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitManageSumAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitSalarySumAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitDeductionSumAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitSalaryTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ServiceFeeSumAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal MonthNetSalarySumAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SocialUnitSumAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ReceivableTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal CustomerBillTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitSettlementAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ServiceRecordCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ZeroSalaryCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ActualSalarySumAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BeforeTaxSalarySumAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsPushTrans { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string TransStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string TransStatusName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid TransNonce { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsCmbcSend { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int SalaryYear { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsPlatformPrepaid { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsCanCmbcSend { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsSalarySlip { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SalarySlipID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsHasOutInvoice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CombineCompanyGID { get; set; }
		
		/// <summary>
		/// 联合开票合并分组工资
		/// </summary>		
		
		public  bool IsCombineSalary { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CombineCustomerSalaryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsNoticeCompleted { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? WorkShopBatchNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsNoticeSendCompleted { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? SendTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? BudgetApplyRecordID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsCmbcSendFail { get; set; }
		
		/// <summary>
		/// SalaryApiSendStatusEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  SalaryApiSendStatusEnumType SalaryApiSendStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsLocked { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? LockedTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CustomerPaidTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? PushTransTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TransMsg { get; set; }
		
		/// <summary>
		/// SalaryPaymentStatusEnumType
		/// </summary>		
		
		public  SalaryPaymentStatusEnumType? PaymentStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsPushToOrder { get; set; }
		   
    }



	/// <summary>
	/// SalaryDelaySendStatusEnumType - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_SalaryDelaySendRecord
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SalaryCustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SalaryRecordID { get; set; }
		
		/// <summary>
		/// SalaryDelaySendStatusEnumType
		/// </summary>		
		
		public  SalaryDelaySendStatusEnumType? Status { get; set; }
		
		/// <summary>
		/// SalaryDelaySendTypeEnumType
		/// </summary>		
		
		public  SalaryDelaySendTypeEnumType? DelayType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? SendAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsNotice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsToCash { get; set; }
		   
    }



	/// <summary>
	/// SalaryFailRecordStatusEnumType - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_SalaryFailRecord
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
		public  Guid SalaryCustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid SalaryRecordID { get; set; }
		
		/// <summary>
		/// SalaryFailRecordStatusEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  SalaryFailRecordStatusEnumType Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Description { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string BankUserName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Bank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string BankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsNotice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsToCash { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ToBankUserName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ToBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ToBank { get; set; }
		
		/// <summary>
		/// SalaryFailTypeEnumType
		/// </summary>		
		
		public  SalaryFailTypeEnumType? FailType { get; set; }
		   
    }



	/// <summary>
	/// 实收企业工资合计=企业应发工资-企业扣款 - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_SalaryRecord
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
		public  Guid SalaryCustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string IdCardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CompanyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Workshop { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? SalaryMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? SendMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ServiceFeeAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitSalaryAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitDeductionAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SubsidyAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SubsidyNoTaxAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal DeductionNoTaxAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsDelayPayment { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SocialYiLiaoAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SocialYangLaoAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SocialShiYeAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SocialDaBingAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SocialGongJiJinAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SocialEmpSumAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SocialUnitSumAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal PlatformCommissionAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal PlatformWelfareAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PlatformDepartment { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SpZiNvAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SpJiaoYuAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SpDaiKuanAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SpZuJinAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SpShanYangAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SpYingErAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SpSumAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SpTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal InnerServiceFeeAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SpBasicTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TaxSalaryTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TaxSpZiNvTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TaxSpJiaoYuTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TaxSpDaiKuanTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TaxSpZuJinTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TaxSpShanYangTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TaxSpYingErTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TaxSpTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SocialYiLiaoTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SocialYangLaoTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SocialShiYeTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SocialDaBingTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SocialGongJiJinTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SocialEmpTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal MonthNetSalaryAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TaxSocialTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int TaxTimes { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BeforeTaxSalaryAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BeforeTaxSalaryTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal CompanyTotalAmount { get; set; }
		
		/// <summary>
		/// 实收企业工资合计=企业应发工资-企业扣款
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal CompanySalarySumAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Bank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string BankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string BankUserName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsCashSend { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int UserEntryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? EntryDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? LeaveDate { get; set; }
		
		/// <summary>
		/// 税后工资
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal AfterTaxAmount { get; set; }
		
		/// <summary>
		/// 预发工资
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal PreSendAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool VerifyInfoOk { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SpBasiclAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal LastSocialYiLiaoTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal LastSocialYangLaoTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal LastSocialShiYeTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal LastSocialDaBingTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal LastSocialGongJiJinTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal LastSocialEmpTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal LastBeforeTaxSalaryTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal LastTaxTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TaxTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TaxMonthAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitManageAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ActualSalaryAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal PrepaidRepaymentAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ActualSalarySendCardAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsDelayPaymentCancel { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsCashSignSuc { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Phone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsDelayPaymentSuc { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BankChangeFilePath { get; set; }
		
		/// <summary>
		/// SalaryDelaySendTypeEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  SalaryDelaySendTypeEnumType DelayType { get; set; }
		
		/// <summary>
		/// SalaryRecordSendTypeEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  SalaryRecordSendTypeEnumType SendType { get; set; }
		
		/// <summary>
		/// SalaryRecordSendStatusEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  SalaryRecordSendStatusEnumType Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int MonthWorkDays { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SettlementID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string TransStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string TransStatusName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsTransBatch { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int SalaryYear { get; set; }
		
		/// <summary>
		/// SalaryRecordTypeEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  SalaryRecordTypeEnumType SalaryRecordType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal DelayPaymentAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal PrepaidTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsBatchSend { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsBatchPushSuc { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string BatchTransStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string BatchTransStatusName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsSingleSend { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsSinglePushSuc { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TransDescription { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SalarySlipRecordID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsCombineSalary { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? SendTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsFailSalary { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TransMsg { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? ChangeAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ChangeRemark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ChangeHash { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? ChangeUpdateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsSplitSend { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsSplitSendSuc { get; set; }
		
		/// <summary>
		/// CmbcSendStatusEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  CmbcSendStatusEnumType CmbcSendStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SplitSendSucAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsPrepaidFail { get; set; }
		
		/// <summary>
		/// SalaryApiSendStatusEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  SalaryApiSendStatusEnumType BatchApiSendStatus { get; set; }
		
		/// <summary>
		/// SalaryApiSendStatusEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  SalaryApiSendStatusEnumType SingleApiSendStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SingleApiBusinessID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SingleApiSendApplyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsLocked { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? LockedTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsFailSalarySuc { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int TransCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsAutoLocked { get; set; }
		
		/// <summary>
		/// 住宿
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ZhuSuAmount { get; set; }
		
		/// <summary>
		/// 水电
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ShuiDianAmount { get; set; }
		
		/// <summary>
		/// 商保
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ShangBaoAmunt { get; set; }
		
		/// <summary>
		/// 体检
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TiJianAmunt { get; set; }
		
		/// <summary>
		/// 线下预支
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal XianXiaYuZhiAmount { get; set; }
		
		/// <summary>
		/// 其它扣除
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal QiTaKouChuAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CashTypeName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsSendSuccess { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsPushToOrder { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_SalarySlip
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
		public  Guid CustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? SalaryMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int TotalNum { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsNotice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string HeaderKV { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SalaryCustomerID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_SalarySlipRecord
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
		public  Guid SalarySlipID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsNotice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? SalaryMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string IdCardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string ContentKV { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SalaryRecordID { get; set; }
		   
    }



	/// <summary>
	/// DeductionEnumType - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_SalarySpecialDedcution
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
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string IdCardNo { get; set; }
		
		/// <summary>
		/// DeductionEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DeductionEnumType DeductionType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? StartMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? EndMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Relationship { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RelationName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RelationIdCardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string DeductionPercentage { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BaseAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal MonthAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? TaxCountMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WorkCity { get; set; }
		   
    }



	/// <summary>
	/// SalarySplitSendStatusEnumType - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_SalarySplitSend
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
		public  Guid SalaryRecordID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string IdCardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TotalAmount { get; set; }
		
		/// <summary>
		/// SalarySplitSendStatusEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  SalarySplitSendStatusEnumType Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Bank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string BankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string BankUserName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid PartnerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string PartnerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid CustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? SendTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SalaryDelaySendRecordID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal PendingSplitAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SendSucAmount { get; set; }
		   
    }



	/// <summary>
	/// SalarySplitSendRecordStatusEnumType - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_SalarySplitSendRecord
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
		public  Guid SalarySplitSendID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SplitAmount { get; set; }
		
		/// <summary>
		/// SalarySplitSendRecordStatusEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  SalarySplitSendRecordStatusEnumType Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? SendTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		   
    }



	/// <summary>
	/// SalaryTransactionStatusEnumType - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_SalaryTransaction
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
		public  string CompanyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string SignUnitName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string SignUnitNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CustomerNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string BusinessID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int SalaryType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int TotalCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TotalAmount { get; set; }
		
		/// <summary>
		/// SalaryTransactionStatusEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  SalaryTransactionStatusEnumType Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? PushTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TransMsg { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int PushCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? TransTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Summary { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Purpose { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// TransactionDataTypeEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  TransactionDataTypeEnumType DataType { get; set; }
		
		/// <summary>
		/// TransactionPayTypeEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  TransactionPayTypeEnumType PayType { get; set; }
		   
    }



	/// <summary>
	/// TransactionStatusEnumType - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_SalaryTransactionRecord
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
		public  Guid SalaryTransactionID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string RecordBusinessID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CardID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Bank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string BankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TransAmount { get; set; }
		
		/// <summary>
		/// TransactionStatusEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  TransactionStatusEnumType TransStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TransMsg { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? TransTime { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_Scheduler
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
		
		public  string JobName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobGroup { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Description { get; set; }
		
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
		
		public  bool IsRun { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Type { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TriggerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TriggerGroup { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Cron { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int Method { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Parameter { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_SchedulerLog
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Content { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		   
    }



	/// <summary>
	/// SettlementBusinessTypeEnumType - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_Settlement
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
		public  Guid CustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? IssuerUnitID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string IssuerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string IssuerCompanyNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string IssuerPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string IssuerAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string IssuerBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string IssuerBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? RecipientUnitID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string RecipientName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string RecipientCompanyNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string RecipientPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string RecipientAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string RecipientBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string RecipientBankNo { get; set; }
		
		/// <summary>
		/// SettlementBusinessTypeEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  SettlementBusinessTypeEnumType BusinessType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Title { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Description { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? ApplyDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? SalaryMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int SettlementType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SalaryCustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerCompanyNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? InsuranceStatus { get; set; }
		   
    }



	/// <summary>
	/// 活跃时间 - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_Staff
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? GID { get; set; }
		
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
		
		public  string Sex { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Age { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CardID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Address { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Tabs { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TabNames { get; set; }
		
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
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remarks { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? FromType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Native { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? UpTime { get; set; }
		
		/// <summary>
		/// 活跃时间
		/// </summary>		
		
		public  DateTime? AcTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Jobs { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobNames { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? MateTime { get; set; }
		
		/// <summary>
		/// 入职时间
		/// </summary>		
		
		public  DateTime? EntryTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? MinMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? MaxMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? RUID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? JobID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? LeaveTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? Money { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PUID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PCID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WeiXinNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WeiXinAbout { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Secret { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ComNames { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Step { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsPond { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? PondTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? ReceiveTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string LastContent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobOtherStreet { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobOtherCity { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobOtherProvince { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobOtherDistrict { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Recommend { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobOtherStreet2 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobOtherCity2 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobOtherProvince2 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobOtherDistrict2 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? Birthday { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Picture { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WXName { get; set; }
		
		/// <summary>
		/// 学历
		/// </summary>		
		
		public  string Education { get; set; }
		
		/// <summary>
		/// 求职状态
		/// </summary>		
		
		public  int? JobStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? WorkYear { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Nation { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Marital { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RegCity { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RegProvince { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobCity { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobProvince { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobDistrict { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobStreet { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UID { get; set; }
		
		/// <summary>
		/// 完善时间
		/// </summary>		
		
		public  DateTime? PerfectTime { get; set; }
		
		/// <summary>
		/// 完善人员
		/// </summary>		
		
		public  int? PerfectUID { get; set; }
		
		/// <summary>
		/// 面试状态
		/// </summary>		
		
		public  int? InStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OldPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool WeiXinCheck { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Level { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Tags { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PerType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Belong { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsSendView { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobAsk { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JsonTags { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? InDate { get; set; }
		
		/// <summary>
		/// 是否线下员工
		/// </summary>		
		
		public  bool IsOffline { get; set; }
		
		/// <summary>
		/// 经验
		/// </summary>		
		
		public  string Experience { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? OldRUID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WorkExperience { get; set; }
		   
    }



	/// <summary>
	/// 面试状态 - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_StaffContact
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string About { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? SID { get; set; }
		
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
		
		public  DateTime? EntryTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Title { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? JID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? LeaveTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsPond { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Step { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Cancel { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? AuditUID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? AuditStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? AuditTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AuditAbout { get; set; }
		
		/// <summary>
		/// 面试状态
		/// </summary>		
		
		public  int? InStatus { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_ThirdSalarySlip
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
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid CustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? SalaryMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string PartnerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int PartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid PartnerGID { get; set; }
		
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
		public  bool IsNotice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? NoticeTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_ThirdSalarySlipRecord
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
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string IdCardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SocialSubsidy { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? SalaryMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int UserEntryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsNotice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsReceive { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? NoticeTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? ReceiveTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string SlipContent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid ThirdSalarySlipID { get; set; }
		   
    }



	/// <summary>
	/// CmbcStatusEnumType - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_TransBatch
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
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string SigningUnitName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int TotalNum { get; set; }
		
		/// <summary>
		/// CmbcStatusEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  CmbcStatusEnumType Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string TransStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string TransStatusName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ConfirmTimes { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid TransNonce { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? TransTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid BusinessID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TransMsg { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RecordID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_TransBatchRecord
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
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string IdCardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Bank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string BankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string BankUserName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TransAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string TranStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string TranStatusName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid TransBatchID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid BusinessID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TransMsg { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RecordID { get; set; }
		   
    }



	/// <summary>
	/// CmbcTransBusinessTypeEnumType - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_TransSingleRecord
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
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string SigningUnitName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string IdCardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TransAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string TransStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string TransStatusName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? TransTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Bank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string BankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string BankUserName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsPrepaid { get; set; }
		
		/// <summary>
		/// CmbcTransBusinessTypeEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  CmbcTransBusinessTypeEnumType BusinessType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid BusinessID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ConfirmTimes { get; set; }
		
		/// <summary>
		/// CmbcStatusEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  CmbcStatusEnumType Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid Nonce { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TransRemark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RecordID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_UpdateRecord
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
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Content { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Title { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? UpdateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ServiceName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ServiceType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Vesion { get; set; }
		   
    }



	/// <summary>
	/// 经验 - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_User
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? UserGID { get; set; }
		
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
		
		public  string UserName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FullName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Password { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OpenID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UnionId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? FUID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string NickName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Picture { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Phone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ClientId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? BirthDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Qrcode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Enabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WeiXinNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? Money { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? SID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsPerfect { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Recommend { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Sex { get; set; }
		
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
		
		public  decimal? MinMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? MaxMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Age { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Jobs { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobNames { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? UpTime { get; set; }
		
		/// <summary>
		/// 经验
		/// </summary>		
		
		public  string Experience { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Education { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PerUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Type { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? EntryStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? JobStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CusID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? EntrySynchro { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsSign { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WeiXinOpenId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FromType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? SceneUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? SceneCusID { get; set; }
		
		/// <summary>
		/// 是否实名认证
		/// </summary>		
		
		public  bool IsReal { get; set; }
		
		/// <summary>
		/// 实名等级
		/// </summary>		
		
		public  int? RealLevel { get; set; }
		
		/// <summary>
		/// 是否日结工
		/// </summary>		
		
		public  bool IsDayOver { get; set; }
		
		/// <summary>
		/// 签到时间
		/// </summary>		
		
		public  DateTime? DayOverStartTime { get; set; }
		
		/// <summary>
		/// 日结工是否签到
		/// </summary>		
		
		public  bool IsDaySign { get; set; }
		
		/// <summary>
		/// 日结公司
		/// </summary>		
		
		public  int? DayOverCID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? DayOverEndTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? LastTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string LastIP { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CardID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? QrcodeGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WorkExperience { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsBlack { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IdcardFrontImage { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IdcardBackImage { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BankImage { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IdcardAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Nation { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Authority { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CardStartDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CardEndDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BankNumber { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BankName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BankID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BankCardName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BankCardType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string MaritalStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string MajorDiseases { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmergencyContact { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmergencyContactPhone { get; set; }
		
		/// <summary>
		/// 重新上传证件
		/// </summary>		
		
		public  int? UploadCardStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RealMessage { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IdPhoto { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomizeField { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EntryCustomizeField { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OldUserGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? EntryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BankCity { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BankProvince { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsPersonnel { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Integral { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsBindMP { get; set; }
		
		/// <summary>
		/// 是否接受跨区域上班
		/// </summary>		
		
		public  string IsCrossRegion { get; set; }
		
		/// <summary>
		/// 是否接受夜班
		/// </summary>		
		
		public  string IsNightShift { get; set; }
		
		/// <summary>
		/// 是否接受无尘服
		/// </summary>		
		
		public  string IsCleanroomSuit { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WorkSkill { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OtherWorkSkill { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobProvince { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobCity { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobDistrict { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? SignDate { get; set; }
		
		/// <summary>
		/// 是否可以参与商城购买
		/// </summary>		
		
		public  bool IsShopPay { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? TurntableCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? TurntableDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsCaAuth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ShopID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsShowInfo { get; set; }
		
		/// <summary>
		/// 是否商城推广员
		/// </summary>		
		
		public  bool IsShopPromotion { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? SkillType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobSkills { get; set; }
		
		/// <summary>
		/// 特种作业操作证
		/// </summary>		
		
		public  string SpecialCertificate { get; set; }
		
		/// <summary>
		/// 进入小程序时的省份
		/// </summary>		
		
		public  string MiniProvince { get; set; }
		
		/// <summary>
		/// 进入小程序时的城市
		/// </summary>		
		
		public  string MiniCity { get; set; }
		
		/// <summary>
		/// 进入小程序时的区域
		/// </summary>		
		
		public  string MiniDistrict { get; set; }
		
		/// <summary>
		/// 进入小程序时的地址
		/// </summary>		
		
		public  string MiniAddress { get; set; }
		
		/// <summary>
		/// 进入小程序时的坐标
		/// </summary>		
		
		public  double? Latitude { get; set; }
		
		/// <summary>
		/// 进入小程序时的坐标
		/// </summary>		
		
		public  double? Longitude { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Tabs { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SysTabs { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RegFromType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsRegPerfect { get; set; }
		   
    }



	/// <summary>
	/// 蓝领uid - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_UserEntry
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? GID { get; set; }
		
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
		
		public  string Sex { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Age { get; set; }
		
		/// <summary>
		/// 蓝领uid
		/// </summary>		
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? SID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? JID { get; set; }
		
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
		
		public  DateTime? EntryTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? LeaveTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 蓝招uid
		/// </summary>		
		
		public  int? PerUserID { get; set; }
		
		/// <summary>
		/// 入职经办人
		/// </summary>		
		
		public  int? HandUserID { get; set; }
		
		/// <summary>
		/// 劳务经纪人uid
		/// </summary>		
		
		public  int? LaborUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? Days { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? ComDays { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WorkName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WorkClass { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FromType { get; set; }
		
		/// <summary>
		/// 是否到期收费
		/// </summary>		
		
		public  bool IsExpire { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? ExpireDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsDayOver { get; set; }
		
		/// <summary>
		/// 签到时间
		/// </summary>		
		
		public  DateTime? DayOverStartTime { get; set; }
		
		/// <summary>
		/// 日结工是否签到
		/// </summary>		
		
		public  bool IsDaySign { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? DayOverEndTime { get; set; }
		
		/// <summary>
		/// 是否结束收费
		/// </summary>		
		
		public  bool EndCharge { get; set; }
		
		/// <summary>
		/// 合作伙伴
		/// </summary>		
		
		public  int? PartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? PartnerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CardID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? QrcodeGID { get; set; }
		
		/// <summary>
		/// 是否是小程序会员
		/// </summary>		
		
		public  bool IsUser { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ContractStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? ContractDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IntAbout { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? IntDate { get; set; }
		
		/// <summary>
		/// 经营模式
		/// </summary>		
		
		public  int? OperateModel { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsBlack { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IdcardFrontImage { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IdcardBackImage { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BankImage { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IdcardAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Nation { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Authority { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CardStartDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CardEndDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BankNumber { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BankName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BankID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BankCardName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BankCardType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string MaritalStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string MajorDiseases { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmergencyContact { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmergencyContactPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? InsureStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsureType { get; set; }
		
		/// <summary>
		/// 离职反馈
		/// </summary>		
		
		public  string LeaveAbout { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? InjuryStatus { get; set; }
		
		/// <summary>
		/// 联营公司的ID
		/// </summary>		
		
		public  int? JointPartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? JointPartnerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IdPhoto { get; set; }
		
		/// <summary>
		/// UserEntryInsuranceStatusEnumType
		/// </summary>		
		
		public  UserEntryInsuranceStatusEnumType? InsuranceStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool SocialMark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? SocialStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? SocialID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SocialGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsRetire { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomizeField { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PlanName1 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PlanName2 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remarks { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsStrike { get; set; }
		
		/// <summary>
		/// UserEntryPlanStatusEnumType
		/// </summary>		
		
		public  UserEntryPlanStatusEnumType? PlanStatus1 { get; set; }
		
		/// <summary>
		/// UserEntryPlanStatusEnumType
		/// </summary>		
		
		public  UserEntryPlanStatusEnumType? PlanStatus2 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? RetireDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? ContractStartDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? ContractEndDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsRenewal { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BankCity { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BankProvince { get; set; }
		
		/// <summary>
		/// 是否实名认证
		/// </summary>		
		
		public  bool IsReal { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string LastNote { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceCompany1 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceCompany2 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceId1 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceId2 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? StructureID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsManuallyInsurance { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsDayWork { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? JobDayWorkID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobDayWorkNo { get; set; }
		
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
		
		public  string Education { get; set; }
		
		/// <summary>
		/// InsuranceProcessEnumType
		/// </summary>		
		
		public  InsuranceProcessEnumType? InsuranceProcessStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceWorkName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceWorkClass { get; set; }
		
		/// <summary>
		/// InsuranceTypeEnumType
		/// </summary>		
		
		public  InsuranceTypeEnumType? InsuranceType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsUseInsurance { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsCaAuth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Tabs { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JDYDataID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? BusinessModel { get; set; }
		
		/// <summary>
		/// 技能类型(1普工2技工)
		/// </summary>		
		
		public  int? SkillType { get; set; }
		
		/// <summary>
		/// 技能
		/// </summary>		
		
		public  string JobSkills { get; set; }
		
		/// <summary>
		/// 特种作业操作证
		/// </summary>		
		
		public  string SpecialCertificate { get; set; }
		   
    }



	/// <summary>
	/// InsuranceSendStatusEnumType - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_UserEntryInsuracneRecord
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? InsuranceCustomerConfigID { get; set; }
		
		/// <summary>
		/// InsuranceSendStatusEnumType
		/// </summary>		
		
		public  InsuranceSendStatusEnumType? SendStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserEntryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? InsuranceOrderID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? StartDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EndDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PlanName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? JobLevelType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? InsuranceStartDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? InsuranceEndDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceCompany { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IdCardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Sex { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceSigningUnitName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Age { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsCommon { get; set; }
		
		/// <summary>
		/// InsuranceMatchEnumType
		/// </summary>		
		
		public  InsuranceMatchEnumType? AddMatchType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? AddMatchRecordID { get; set; }
		
		/// <summary>
		/// InsuranceMatchEnumType
		/// </summary>		
		
		public  InsuranceMatchEnumType? DelMatchType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? DelMatchRecordID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? InsuranceOrderPlanID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? BatchNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsMain { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsAdd { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OrderNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsComplete { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_UserEntryNote
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
		
		public  int? EntryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PersonnelID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remarks { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_UserEntryRecord
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
		
		public  int? EntryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string About { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? SID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? AuditUID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? AuditStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? AuditTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AuditAbout { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EntryTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Title { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? LeaveTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PerUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? LaborUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? OperateUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? OperateFrom { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OperateFullName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OperateFromName { get; set; }
		   
    }



	/// <summary>
	/// 入职经办人 - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_UserInterview
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
		
		public  int? PerUserID { get; set; }
		
		/// <summary>
		/// 入职经办人
		/// </summary>		
		
		public  int? HandUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? LaborUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		
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
		
		public  DateTime? EntryTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? ViewTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WorkName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WorkClass { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string About { get; set; }
		
		/// <summary>
		/// 合作伙伴
		/// </summary>		
		
		public  int? PartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? PartnerGID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_UserJobQrcode
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? JobID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Qrcode { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_UserLeaveReason
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? EntryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? LeaveID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string LeaveName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string LeaveValue { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_UserOpenIds
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int FromID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string OpenIdType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OpenId { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_UserProfile
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true)]
		public  int UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? UserGID { get; set; }
		
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
		
		public  string WorkTypePrefs { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WorkTypeTop { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CatePrefs { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? SalaryLevel { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ShiftPrefs { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? CredibilityScore { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int HiredCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int AppliedCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int QuickLeaveCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? AvgStayDays { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RiskClassMode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  byte SeekStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  byte MobilityType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? LastBehaviorTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string LastEntryWorkName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? LastCalcTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  long CalcVersion { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_UserToken
    {
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true)]
		public  string Token { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FromType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EndTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserAgent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IPAddress { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_AttendanceApply
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
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
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? RecordID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Content { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FileInfo { get; set; }
		
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
		
		public  string Reply { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AuditedID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Sort { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? ApplyDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Type { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? StructureID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ManageGIDS { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? Number { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? SignDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ApplyType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? DefaultTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? AttendanceID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AddressName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? DayType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? AttendanceType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? NightShiftSubsidy { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? ShiftMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? DinnerHours { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? LunchHours { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string LunchTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string DinnerTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? TimeRule { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string DayWorkType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Message { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? AuditStructure { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ParentID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string StartTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EndTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsDay { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? StartDayType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? EndDayType { get; set; }
		
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
		
		public  DateTime? UpTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? OffTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserIDS { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserNames { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? NavID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? InfoType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InfoTypeName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? EntryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TypeName { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_AttendanceAskForLeaveRecord
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
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
		
		public  int? CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Year { get; set; }
		
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
		
		public  string CardID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int CompensatoryLeave { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int YearLeave { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int InjuryLeave { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int MarryLeave { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int BereavementLeave { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int PrenatalCheckLeave { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int MaternityLeave { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int PrenatalLeave { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int MiscarriageLeave { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int LactationLeave { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int AccompanyLeave { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int PersonalLeave { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int SickLeave { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int CompensatoryLeaveUsed { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int YearLeaveUsed { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int InjuryLeaveUsed { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int MarryLeaveUsed { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int BereavementLeaveUsed { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int PrenatalCheckLeaveUsed { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int MaternityLeaveUsed { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int PrenatalLeaveUsed { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int MiscarriageLeaveUsed { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int LactationLeaveUsed { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int AccompanyLeaveUsed { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int PersonalLeaveUsed { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int SickLeaveUsed { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? EntryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EntryTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? LeaveTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Phone { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_AttendanceConfig
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? GID { get; set; }
		
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
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ManageID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Type { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  double? Latitude { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  double? Longitude { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AddressName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string City { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int Range { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Days { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? OnTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? OffTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Enable { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Count { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Content { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Holiday { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Elastic { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? LunchHours { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? DinnerHours { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? ShiftMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? NightShiftSubsidy { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string LunchTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? TimeRule { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string DinnerTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string DayWorkType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ApplicationCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsApplicationLimit { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? RuleType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string LocationJson { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_AttendancePrepaidApplyCustomer
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? GID { get; set; }
		
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
		
		public  int? CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BillMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? StructureID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string StructureName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? ShiftHours { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? ShiftPrice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? ShiftMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserIDS { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserNames { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? DayPrice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? DayHours { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? DayMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? NightHours { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? NightPrice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? NightMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? InvoiceID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Bank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BankAccount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CompanyNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Address { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Phone { get; set; }
		
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
		
		public  int? InvoiceStatus { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_AttendancePrepaidApplyRecord
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? GID { get; set; }
		
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
		
		public  int? StructureID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string StructureName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BillMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? BillDate { get; set; }
		
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
		
		public  int? EntryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CardID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Phone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? ShiftHours { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? ShiftPrice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? ShiftMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PrepaidApplyCustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserIDS { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserNames { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EntryNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? JobPrice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? JobMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string DayWorkType { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_AttendanceRecord
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
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
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? AttendanceDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? DetailID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string StatusName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Type { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TypeName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AddressName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Content { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ManageContent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Message { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string MobileDevice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Title { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TimeShow { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? OnTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? OffTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Sort { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? DepartmentID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? DayType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? SignDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ApplyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? WorkHour { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FileInfo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? DefaultTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? LeaveHour { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CardID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? StructureID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? AttendanceType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EntryNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? ShiftMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? NightShiftSubsidy { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserIDS { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserNames { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? Charge { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? ShiftPrice { get; set; }
		
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
		
		public  DateTime? ApplyApplyDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ApplyStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ApplyMessage { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ApplyFileInfo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ApplyContent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? ApplyNumber { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? EntryID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_AttendanceStructure
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? DepartmentID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? AttendanceID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ParentID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Enable { get; set; }
		
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
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserIDS { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserNames { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? NavID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? AttendanceType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? ShiftPrice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string DayWorkType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? TimeRule { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? NightShiftSubsidy { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? ShiftMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? LunchHours { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? DinnerHours { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ApplicationCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsApplicationLimit { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? RuleType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Holiday { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Expr1 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? Expr2 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Days { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_BankCompany
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CompanyNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsEnable { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CompanyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsDelete { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? DeleteTime { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_BankCompanyConfig
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string ConfigTitle { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid BankCompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int BankType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsDefault { get; set; }
		
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
		
		public  DateTime? DeleteTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CompanyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid MainBankAccountID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid PrepaidBankAccountID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid OtherBankAccountID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string MainAccountName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PrepaidAccountName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OtherAccountName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string MainAccountNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PrepaidAccountNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OtherAccountNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string MainAccountOrgCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PrepaidAccountOrgCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OtherAccountOrgCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string MainAccountOrgName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PrepaidAccountOrgName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OtherAccountOrgName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool MainAccountIsEnable { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool PrepaidAccountIsEnable { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool OtherAccountIsEnable { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BankCompanyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool CompanyIsEnable { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_BankCustomer
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid id { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CustomerNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string SignUnitName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string SignUnitNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsEnable { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsDelete { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? DeleteTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CompanyName { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_BankUser
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int UserID { get; set; }
		
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
		[SugarColumn(IsNullable=false)]
		public  bool IsDelete { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsEnable { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsValidate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FullName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CompanyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsLock { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? LockTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FailMsg { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int FailTimes { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_BlackList
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? GID { get; set; }
		
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
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IdentityCard { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Content { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string LabelIDS { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string LabelNames { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Method { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PartnerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsRemind { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_BudgetApplyRecordDetail
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
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
		
		public  int? ReceivePaymentCompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ApplyRecordPartnerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Description { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? ApplyAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? ApplyRecordCreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProcessStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ReceivePaymentCompanyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? PayTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CostType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Payer { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_BudgetConfig
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ID { get; set; }
		
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
		
		public  string PartnerName { get; set; }
		
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
		
		public  int? PartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PartnerNames { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PartnerIDS { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_BudgetManage
    {
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PartnerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
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
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsBudget { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_BusinessInsuranceConfig
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid CustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid InsuranceSigningUnitID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string InsuranceCompany { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Title { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Plan3 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Plan4 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Plan5 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsDefaultPush { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsMain { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsEnabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WorkNames { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceCompanyOther { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
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
		
		public  string InsuranceSigningUnitName { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_BusinessInsuranceOrder
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string InsuranceCompany { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int OrderType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string InsuranceId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string InsuranceNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsDefaultPush { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid SigningUnitID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? StartDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? EndDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SigningUnitNames { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SigningUnitName { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_BusinessInsuranceOrderEmpoyee
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid BusinessInsuranceOrderID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string IdCardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Sex { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int Age { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string JobType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string JobLevelType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CertificateType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string SigningUnitName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string JobCompanyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string PlanName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? StartDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? EndDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal InsuranceAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ActionType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string FromType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int MethodType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceCompany { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_BusinessInsuranceOrderPlan
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid BusinessInsuranceOrderID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int GnteeId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string GnteeName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? OrderType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? StartDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EndDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceCompany { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsDefaultPush { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SigningUnitNames { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceSingningUnitName { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_BusinessInsurancePush
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string InsuranceId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? AddPushTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int AddRecordCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string AddIndecaseId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? DelPushTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int DelRecordCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string DelIndecaseId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int AddPushStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int AddFailCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int DelFailCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string PlanName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int PlanNameId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int AddResCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string AddResMsg { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int DelPushStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int DelResCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string DelResMsg { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? StartDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EndDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? OrderType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceCompany { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_BusinessInsurancePushRecord
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Custemername { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Custemernum { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Custemersex { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Custemerage { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Guyuangongzhong { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Sjygdw { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Ldhtdw { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Zhiyeleibei { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Custemertype { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid BusinessInsuracneRecordID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid BusinessInsurancePushID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int GnteeId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string GnteeName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsAdd { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsFail { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int UserEntryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsMain { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsCompleted { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceCompany { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? OrderType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? StartDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EndDate { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_BusinessInsuranceRecord
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int UserEntryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid CustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid InsuranceSigningUnitID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string InsuranceSigningUnitName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsDayOrder { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string InsuranceCompany { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string OrderNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string InsuranceNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string PlanName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Sex { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int Age { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string IdCardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? EntryDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? LeaveDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string WorkName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string WorkClass { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsExpired { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? AddSysTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? AddPushTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? AddMatchRecordID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? DelSysTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? DelPushTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? DelMatchRecordID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? StartDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? EndDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsMain { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserEntryStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PlanStatus1 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PlanStatus2 { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_BusInsuracneOrderPlan
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PlanName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int PlanNameID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal PlanPriceAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceCompany { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OrderNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PolicyNum { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? OrderMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SignUnitNames { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsEnable { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? StartDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EndDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsDayOrder { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_CityPartnerReport
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Title { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string City { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string AgentName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string AgentIdCardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string AgentPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsEnabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string ProfitPartnerGIDArr { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int PartnerCounts { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int PartnerEntryCounts { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int CompanyEntryCounts { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int CustomerCounts { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int PartnerType1 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int PartnerType5 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int PartnerType8 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int PartnerType9 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int PartnerType10 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal MonthTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal YearTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int CompanyCounts { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_ClientNoticeTemplate
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ClientID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? NoticeID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TempID { get; set; }
		
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
		
		public  string AppType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Type { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AppID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AppSecret { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Token { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string KidList { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Fields { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Name { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_ClientPage
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ClientID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Moulds { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PageType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AppID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AppConfig { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Style { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Title { get; set; }
		
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
	[TenantAttribute("Main")]
	public partial class Live_V_Clients
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? GID { get; set; }
		
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
		
		public  string Type { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Enabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AppType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AppID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AppSecret { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Token { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CompanyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsDefault { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? ReleaseTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Version { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ReleaseStatus { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_ClientShortLink
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ClientID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ShortLinkType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ShortLinkUrl { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PageUrl { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Type { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AppType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PageTitle { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsPermanent { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_CompanyAuthorize
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ToCompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CompanyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ToCompanyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Enabled { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_CompanyPerFunction
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int PFID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Sort { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Number { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Enabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ToPage { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Icon { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Unit { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CountField { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Type { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PerSort { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IconSize { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IconClass { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IconColor { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_ContractRecord:ModelBasic
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TemplateOssUrl { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ContractOssUrl { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SignedOssUrl { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsNeedKeywordReplace { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string KeywordReplaceContent { get; set; }
		
		/// <summary>
		/// ContractEnumType
		/// </summary>		
		
		public  ContractEnumType? ContractType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? SignedTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? RelateID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? RelateGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? RStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FxqSignedOssUrl { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FxqTransactionId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FxqHash { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? BatchNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserEntryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserIdCardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerCompanyNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SigningUnitGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SigningUnitName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SigningUnitCompanyNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? StartDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EndDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? ContractStartDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? ContractEndDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? SigningTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? SendSmsTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SmsCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SmsContent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? ReadySigningDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public override int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsRetire { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsNotice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmpName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmpIdCardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmpSex { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? EmpAge { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? TrialMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ContractMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EntryDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? LeaveDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobCity { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string NativePlace { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SigningUnitAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SigningUnitLegaPperson { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SigningUnitPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SigningUnitSignKeyword { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmpSignKeyword { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmergencyContactName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmergencyContactPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Education { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobContent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmpNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmpPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmpOssUrl { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SigningUnitOssUrl { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WordOssUrl { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsSuc { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SigningUnitLegalPerson { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool SigningUnitSignDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? SigningUnitSignPageHeight { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool EmpSignDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? EmpPageHeight { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SmTemplateFilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SmWordFilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SmPdfFilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SmSignedFilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TemplateFilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WordFilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PdfFilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SignedFilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string HomeAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string HtHash { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SmHash { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsAdviceNote { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AutoLeaveFilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsAutoLeave { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AutoLeaveSmsContent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? AutoLeaveSmsSendTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? AutoLeaveStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? ContractDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? TrialStartDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? TrialEndDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TrialDescription { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? RetireDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsRenewal { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public override int? CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CompanyGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public override int? CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? FactoryUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public override int? PartnerID { get; set; }
		
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
		
		public  int? UserPartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public override int? PerUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? HandUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserEntryStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public override int? PerPartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public override int? CustomerPartnerID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_Customer
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
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
		
		public  int? ServiceUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? FactoryUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? OperateModel { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Enabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Tel { get; set; }
		
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
		
		public  string QQ { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remarks { get; set; }
		
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
		
		public  int? BillingDay { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Annex { get; set; }
		
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
		
		public  string FromType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FromNames { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Tabs { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Type { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Logo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  double? Latitude { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  double? Longitude { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Picture { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RecruitTel { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Coordinate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Scale { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Industry { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? Money { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string About { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsTry { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Structure { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Months { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Recommend { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Street { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RecruitAnnex { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? DayMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? NoDayMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserDayMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? UserMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? NoUserDayMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string GroupNotice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobProvince { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobCity { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobDistrict { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsAdvance { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AdvanceMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Physical { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Social { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Accumul { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BatteryCar { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InterviewProcess { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ContactName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BusLine { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PhysicalMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PhysicalDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SocialMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AccumulMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool DirectEntry { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string MonthCommission { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CUID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EditMoneyTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ShowMonthMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CompanyNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BusinessEntity { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InvoiceName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InvoiceType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsSynthetical { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Bank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BankAccount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string invoiceRemarks { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? ServiceMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? PersonnelMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserMonths { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ServiceMoneyAbout { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsLongJoint { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? DailyType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? DailyDays { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string DayShiftTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string NightShiftTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? DayShiftHours { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? NightShiftHours { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? DayShiftMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? NightShiftMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? LunchHours { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? DinnerHours { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? NightShiftSubsidy { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsDayOver { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsUserAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ServiceName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ServicePhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? EntryModel { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CooperationUnitGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SigningUnitGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SigningUnitName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SigningUnitCompanyNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string signingUnitAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SigningUnitPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SigningUnitBankName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SigningUnitBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationUnitName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationUnitCompanyNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationUnitAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationUnitPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationUnitBankName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationUnitBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AgentBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AgentBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AgentIdCardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AgentName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PartnerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JointPartnerName { get; set; }
		
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
		[SugarColumn(IsNullable=false)]
		public  decimal SocialFee { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool SociaEnabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string LegalCustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BaseType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BusinessTypeName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? BusinessType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WorkNames { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? Tax { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SalesMan { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsSaleCustomer { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SaleContent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? SaleStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WagesValueAddedTax { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WagesAdditionalTax { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ServiceInvoiceName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ServiceInvoiceType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WagesInvoiceName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WagesInvoiceType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool InvoiceCombine { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ServiceAdditionalTax { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ServiceValueAddedTax { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string VicePartnerIDS { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string VicePartnerIDNames { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? AttendanceType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? ShiftPrice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? Wages { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? Charge { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? LunchHour { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? DinnerHour { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Post { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WorkTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WorkContent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Requirement { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Cycle { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WXName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WXPasswordSalt { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? TimeRule { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CitySalesmanID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? CitySalesmanPercent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CitySalesmanName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CityPartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CityPartnerTitle { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? BusinessModel { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsUseBusInsurance { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsUseDeathInsurance { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string GroupName { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_CustomerContractConfig
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid CustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsEnabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? EntryFileID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? LeaveFileID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? ReEntryFileID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? ReLeaveFileID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? DelayDays { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? SignMonths { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? TrialMonths { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? EntryDeclareFileID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CashFileID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int SignType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EntryDeclareFilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EntryFilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string LeaveFilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ReEntryFilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ReLeaveFilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CashFilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CustomerID { get; set; }
		
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
		
		public  string SigningUnitName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SigningUnitNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SigningUnitLegalPerson { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SigningUnitPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SigningUnitAddress { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_CustomerContractFile
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid CustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? JoinFileID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? LeaveFileID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? ReJoinFileID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? ReLeaveFileID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CashFileID { get; set; }
		
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
		
		public  string JoinFileName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JoinFilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string LeaveFileName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string LeaveFilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ReJoinFileName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ReJoinFilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ReLeaveFileName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ReLeaveFilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CashFileName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CashFilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_CustomerJobs
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int EntryJobID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Type { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Enabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsDefault { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_CustomerServiceCharge
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? InvoiceCustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? InvoiceCustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ReceiptCustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? ReceiptCustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? SalaryMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? InvoiceDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? SendDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? InvoiceAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? SalaryVAT { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? SalarySurtax { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? ServiceCharge { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? ServiceVAT { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? ServiceSurtax { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? SocialAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PeopleCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? TotalAmount { get; set; }
		
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
		
		public  string InvoiceCustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ReceiptCustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsPredict { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_CustomerUnit
    {
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? BusinessType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BusinessTypeName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CitySalesmanID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? PartnerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PartnerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CityPartnerTitle { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_DayWorkUserContractRecord
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? GID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CustomerID { get; set; }
		
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
		
		public  string SignPath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SignKeyword { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SignerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SignerIdNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? FileID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Message { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SendTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? EntryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TemplateStatement { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TemplateContract { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmploymentContract { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmploymentStatement { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CardID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? JobDayWorkID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EntryNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Code { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_InjuryAdvanceAccount
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
		
		public  int? PartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? PartnerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? BasePercent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? FloatPercent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? TotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? UsedAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? AvailableAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PartnerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool Enabled { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_InjuryApplyRecord
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
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
		
		public  int? InjuryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Content { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Bank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BankAccount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Path { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal Money { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Type { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProcessStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InjuryType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InjuryLevel { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? InjuryTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? LoanTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? RepaymentTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? ReturnBackMoneyTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserSex { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Age { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserEntryInjuryStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? MoneyType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ApplyRecordPerUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FullName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PartnerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SealCompanyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SealCompanyBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SealCompanyBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AgentName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AgentBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AgentBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserCardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AgentIdcardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? InjuryGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? FactoryUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EntryTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? HandUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PerUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CustomerPartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? PayTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? AdvanceAccountID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsPerson { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_InjuryClaimRatio
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
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
		
		public  int? PartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? PartnerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Type { get; set; }
		
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
		[SugarColumn(IsNullable=false)]
		public  decimal PremiumReceivedAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ResolvedClaimAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal OutstandingClaimAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ClaimRatio { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int InjuryNumber { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal InsuranceNumber { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal InjuryRate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? SignTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CompanyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PartnerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Year { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? TotalAmount { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_InjuryFinancialWriteOff
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
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
		
		public  int? InjuryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserEntryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserCardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal LoanTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RepaymentTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ReturnBackMoneyTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal WriteOffTotalAmount { get; set; }
		
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
		
		public  string PartnerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProcessStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AgentName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AgentIdcardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AgentBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AgentBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CustomerPartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? FactoryUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PerUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? InjuryTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? InjuryLoanTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? InjuryRepaymentTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? InjuryReturnBackMoneyTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsPerson { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? AdvanceAccountID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal Money { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Bank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BankAccount { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_InjuryInfo:ModelBasic
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public override int? CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CompanyGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? UserGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserEntryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserCardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserSex { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserRegisteredResidence { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserWorkType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InjuryDescription { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? InjuryTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InjuryAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InjuryType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? AdvanceApplyAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public override int? CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PartnerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public override int? PartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? PartnerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string NoticeLetterFilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AuthorizationLetterFilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CompensationAgreementLetterFilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CompensationAgreementDisabilityLetterFilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? InjuryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProcessStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SealCompanyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SealCompanyAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SealCompanyPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SealCompanyBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SealCompanyBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SealCompanyNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationCompanyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsurancePeriod1 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsurancePolicyNo1 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceType1 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceCompany1 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? InsuranceStartDate1 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? InsuranceEndDate1 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsurancePeriod2 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsurancePolicyNo2 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceType2 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceCompany2 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? InsuranceStartDate2 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? InsuranceEndDate2 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AgentName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AgentIdcardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AgentBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AgentBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsDisabilityAssessed { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WorkInjuryReportFilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FailMsg { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal LoanTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RepaymentTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ReturnBackMoneyTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal StaffMedicalExpense { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal StaffLossOfWorkingTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal StaffHospitalizationAllowance { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal StaffHospitalizationMealAllowance { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal StaffTransportationCosts { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal StaffNursingFees { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal StaffAssistiveDevices { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal StaffOneTimeDisabilityAllowance { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal StaffOneTimeEmploymentSubsidy { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal StaffOneTimeMedicalSubsidy { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal StaffOneTimePackAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal StaffOneTimeWorkDeathAllowance { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal StaffAppraisalCosts { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal StaffOtherCosts { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal StaffSubtotal { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal StaffPeriodicFee { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int StaffPaymentCycle { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal StaffPeriodicTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal StaffTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal InsuranceMedicalExpense { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal InsuranceLossOfWorkingTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal InsuranceHospitalizationAllowance { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal InsuranceHospitalizationMealAllowance { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal InsuranceTransportationCosts { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal InsuranceNursingFees { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal InsuranceAssistiveDevices { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal InsuranceOneTimeDisabilityAllowance { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal InsuranceOneTimeEmploymentSubsidy { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal InsuranceOneTimeMedicalSubsidy { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal InsuranceOneTimePackAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal InsuranceOneTimeWorkDeathAllowance { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal InsuranceAppraisalCosts { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal InsuranceOtherCosts { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal InsuranceSubtotal { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal InsurancePeriodicFee { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int InsurancePaymentCycle { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal InsurancePeriodicTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal InsuranceTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RiskMedicalExpense { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RiskLossOfWorkingTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RiskHospitalizationAllowance { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RiskHospitalizationMealAllowance { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RiskTransportationCosts { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RiskNursingFees { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RiskAssistiveDevices { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RiskOneTimeDisabilityAllowance { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RiskOneTimeEmploymentSubsidy { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RiskOneTimeMedicalSubsidy { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RiskOneTimePackAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RiskOneTimeWorkDeathAllowance { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RiskAppraisalCosts { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RiskOtherCosts { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RiskSubtotal { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RiskPeriodicFee { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int RiskPaymentCycle { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RiskPeriodicTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RiskTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public override int? PerUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? HandUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? FactoryUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserEntryInjuryStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public override int? UserID { get; set; }
		
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
		
		public  string InjuryLevel { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? InjuryPerUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SealCompanyGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CooperationCompanyGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EntryTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsArbitration { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public override int? CustomerPartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ToCompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? ToCompanyGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? InjuryGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? ShowTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? DocumentLabelType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public override int? PerPartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Diagnosis { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string HospitalName { get; set; }
		
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
		
		public  string InsuranceId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsPerson { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InjuryPart { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobItem { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EditTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsFracture { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EstimatedClaim { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal AverageWage { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ProvincialAverageWage { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_InjuryLoan
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
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
		
		public  int? InjuryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? Money { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Path { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BillPath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? AddTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Type { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Content { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserEntryInjuryStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PartnerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Age { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserSex { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserCardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProcessStatus { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_InjuryPayment
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
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
		
		public  int? InjuryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PerUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Year { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? ApplicationTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal PaymentAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CostName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PaymentSubject { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Purpose { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserCardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PayeeName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PayeeBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PayeeBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProcessStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PartnerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? PayTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Message { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_InjuryRepay
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
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
		
		public  int? InjuryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Content { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Bank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BankAccount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RepaymentTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal LoanTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SpendTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Path { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RepayMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserEntryInjuryStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserSex { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Age { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProcessStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? InjuryGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PartnerID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_InsuranceCompanyBill
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
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
		
		public  int? RecordCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? EmployeeCompanyId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmployeeCompanyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmployeeCompanyNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmployeeCompanyBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmployeeCompanyBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TotalServiceFee { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TotalSheBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TotalGongJiJin { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? BillMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeeSumGongJiJin { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitSumGongJiJin { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeeSumSheBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitSumSheBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProcessStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsInvoicePaid { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsInvoicePrinted { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ProcessName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CooperationUnitGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? EmployeeCompanyGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmployeeCompanyAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmployeeCompanyPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CompanyInvoiceId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? ActualAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_InsuranceCustomerConfig
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Title { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceCompany { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? InsuranceSigningUnitID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string LevelThreePlan { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string LevelFourPlan { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string LevelFivePlan { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsCommon { get; set; }
		
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
		
		public  bool IsMain { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SigningUnitName { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_InsuranceEmployee
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
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
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IdcardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IdcardAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Sex { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Nation { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Phone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Education { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? EmployeeCompanyId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ShemeType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? YlSchemeId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? StartDateGongJiJin { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EndDateGongJiJin { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? StartDateSheBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EndDateSheBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Province { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ProvinceCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string City { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CityCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Enable { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? YlSupplierIdSheBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BaseAmountYangLao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BaseAmountYiLiao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BaseAmountShiYe { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BaseAmountShengYu { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BaseAmountGongShang { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BaseAmountCanBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitPercentYangLao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitPercentYiLiao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitPercentShiYe { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitPercentShengYu { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitPercentGongShang { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitPercentCanBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeePercentYangLao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeePercentYiLiao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeePercentShiYe { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeePercentShengYu { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeePercentGongShang { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeePercentCanBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeeAmountCanBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeeAmountDaBingYiLiao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeeAmountYiLiaoBuChong { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitAmountCanBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitAmountDaBingYiLiao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitAmountYiLiaoBuChong { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? YlSupplierIdGongJiJin { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BaseAmountGongJiJin { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeePercentGongJiJin { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitPercentGongJiJin { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? YlRule { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? TimeNode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProcessStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? SheBaoServiceAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? GongJiJinServiceAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BaseRemark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsCustomize { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SchemeName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SheBaoSuplierName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string GongJiJinSuplierName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BaseDifferenceAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CooperationUnitGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationUnitName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationUnitCompanyNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationUnitPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationUnitAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationUnitBankName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationUnitBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? EmployeeGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? EmployeeCompanyGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerBankAccount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerCompanyNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool CustomerSociaEnabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? CustomerSocialFee { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PartnerID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_InsuranceEmployeeBill
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
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
		
		public  DateTime? BillMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IdcardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IdcardAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Sex { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Nation { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Phone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Education { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? EmployeeCompanyId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmployeeCompanyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ProcessStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ShemeType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? YlSchemeId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? StartDateGongJiJin { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EndDateGongJiJin { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? StartDateSheBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EndDateSheBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Province { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ProvinceCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string City { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CityCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Enable { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? YlSupplierIdSheBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BaseAmountYangLao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BaseAmountYiLiao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BaseAmountShiYe { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BaseAmountShengYu { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BaseAmountGongShang { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BaseAmountCanBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitPercentYangLao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitPercentYiLiao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitPercentShiYe { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitPercentShengYu { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitPercentGongShang { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitPercentCanBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeePercentYangLao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeePercentYiLiao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeePercentShiYe { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeePercentShengYu { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeePercentGongShang { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeePercentCanBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeeAmountCanBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeeAmountDaBingYiLiao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeeAmountYiLiaoBuChong { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitAmountCanBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitAmountDaBingYiLiao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitAmountYiLiaoBuChong { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? YlSupplierIdGongJiJin { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BaseAmountGongJiJin { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeePercentGongJiJin { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitPercentGongJiJin { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? YlRule { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? TimeNode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ServiceBaseAmountGongJiJin { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ServiceBaseAmountSheBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitAmountYangLao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeeAmountYangLao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitAmountYiLiao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeeAmountYiLiao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitAmountShiYe { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeeAmountShiYe { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitAmountShengYu { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeeAmountShengYu { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitAmountGongShang { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeeAmountGongShang { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitAmountCountCanBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeeAmountCountCanBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitAmountGongJiJin { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeeAmounGongJiJin { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ServiceFeeAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeeSumSheBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitSumSheBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TotalAmountSheBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TotalAmountGongJiJin { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SheBaoName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string GongJiJinName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SchemeName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string shemeTypeName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RuleName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ProcessName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationUnitName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CooperationUnitGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationUnitPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationUnitAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationUnitBankName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationUnitBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? EmployeeCompanyGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeeCompanySocialFee { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmployeeCompanyAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmployeeCompanyPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmployeeCompanNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmployeeCompanyBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmployeeCompanyBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool EmployeeCompanySocialEnabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationUnitNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PartnerID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_InsuranceOrder
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string InsuranceCompany { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int OrderType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string OrderNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string InsuranceNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsCommon { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SigningUnitID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? StartDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? EndDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SigningUnitName { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_InsuranceOrderPlan
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid InsuranceOrderID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int GnteeId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string GnteeName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceCompany { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? OrderType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OrderNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsCommon { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? StartDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EndDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SigningUnitName { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_InsurancePush
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string OrderNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? AddPushTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int AddRecordCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string AddIndecaseId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? DelPushTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int DelRecordCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string DelIndecaseId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int AddFailCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int DelFailCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int GnteeId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string GnteeName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid InsuranceOrderPlanID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? AddResCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AddResMsg { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? DelResCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string DelResMsg { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? AddPushStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? DelPushStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? InsuranceOrderID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceCompany { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? StartDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EndDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsCommon { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SigningUnitName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? OrderType { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_InsurancePushRecord
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Custemername { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Custemernum { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Custemersex { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Custemerage { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Guyuangongzhong { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Sjygdw { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Ldhtdw { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Zhiyeleibei { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Custemertype { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid UserEntryInsuracneRecordID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid InsurancePushID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsAdd { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsPushSuc { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserEntryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? BusinessInsuranceRecordID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsMain { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsApiPush { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? PushTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsPushing { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OrderNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string GnteeName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceCompany { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_InsuranceScheme
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
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
		
		public  string Province { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ProvinceCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string City { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CityCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? YlSupplierIdSheBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BaseAmountYangLao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BaseAmountYiLiao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BaseAmountShiYe { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BaseAmountShengYu { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BaseAmountGongShang { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BaseAmountCanBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitPercentYangLao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitPercentYiLiao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitPercentShiYe { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitPercentShengYu { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitPercentGongShang { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitPercentCanBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeePercentYangLao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeePercentYiLiao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeePercentShiYe { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeePercentShengYu { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeePercentGongShang { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeePercentCanBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeeAmountCanBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeeAmountDaBingYiLiao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeeAmountYiLiaoBuChong { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitAmountCanBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitAmountDaBingYiLiao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitAmountYiLiaoBuChong { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? YlSupplierIdGongJiJin { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BaseAmountGongJiJin { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeePercentGongJiJin { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitPercentGongJiJin { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? YlRule { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? TimeNode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Enable { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BaseDifferenceAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BaseRemark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? SheBaoServiceAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? GongJiJinServiceAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SheBaoSupplierName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string GongJiJinSupplierName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SchemeGID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_InsuranceSupplier
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
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
		[SugarColumn(IsNullable=false)]
		public  bool Enable { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ServiceAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ParentId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsCombine { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmployeeCompanyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? EmployeeCompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmployeeCompanyNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmployeeCompanyBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmployeeCompanyBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SupplierGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsJoint { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JointCompanyName { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_InsuranceSupplierBill
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
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
		
		public  int? ProcessStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? RecordCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? YlSupplierId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string YlSupplierName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CooperationUnitGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TotalServiceFee { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TotalSheBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TotalGongJiJin { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? BillMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeeSumGongJiJin { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitSumGongJiJin { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal EmployeeSumSheBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitSumSheBao { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsInvoicePaid { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsInvoicePrinted { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsPaidAlready { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? SheBaoBill { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? GongJiJinBill { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ProcessName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SupplierCompanyNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SupplierCompanyBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SupplierCompanyBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? YlSupplierGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CompanyInvoiceId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SheetId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? ActualAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InvoiceCompanyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsSettlementPass { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_Invoice
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid CustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string IssuerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string IssuerCompanyNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string IssuerPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string IssuerAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string IssuerBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string IssuerBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string RecipientName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string RecipientCompanyNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string RecipientPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string RecipientAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string RecipientBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string RecipientBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int InvoiceType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int BusinessType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Title { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsAddedTax { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsAdditionalTax { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal AddedTaxAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal AdditionalTaxAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal AddedTaxDiscountAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal AdditionalTaxDiscountAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ChangeAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsLocked { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsIssued { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int RecordCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ServiceFeeAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Description { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? ApplyTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? LockedTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? IssuedTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? SalaryMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid IssuerUnitID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid RecipientUnitID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal AddedTaxPointAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal AdditionalTaxPointAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal InvoiceAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal AddedTaxSumAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal AdditionTaxSumAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SfAddedTaxAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SfAddedTaxPointAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SfPersonalTaxAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SfPersonalTaxPointAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsSf { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SfOtherPaidAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SfSocialAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SfTaxRefundAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsCancel { get; set; }
		
		/// <summary>
		/// InvoiceStatusEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  InvoiceStatusEnumType Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SalaryCustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsSpecial { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string LegalCustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
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
		
		public  int? CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ApplyUserName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ApplyUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ApplyUserFullName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? SalaryAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? SalaryRecordCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? SerivceFeeRecordCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? BothRecordCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsReset { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationUnitName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CooperationUnitID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CustomerID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_InvoiceConfig
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Title { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string InvoiceTitle { get; set; }
		
		/// <summary>
		/// InvoiceTypeEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  InvoiceTypeEnumType InvoiceType { get; set; }
		
		/// <summary>
		/// PrintTemplateEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  PrintTemplateEnumType PrintTemplate { get; set; }
		
		/// <summary>
		/// InvoiceDataTypeEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  InvoiceDataTypeEnumType DataType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsIncludeSalary { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsIncludeServiceFee { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TaxPointSalaryValueAdded { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TaxPointSalaryAdditional { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TaxPointServiceFeeValueAdded { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TaxPointServiceFeeAdditional { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsEnabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CusomerStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string LegalCustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CompanyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CompanyGID { get; set; }
		
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
		
		public  int? CustomerID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_Jobs
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Title { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Tabs { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TabNames { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? JCID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Sex { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Ages { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Address { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? MinMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? MaxMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WorkTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Payday { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Structure { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WorkMode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WorkContent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Foot { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Hostel { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RestMode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Coverall { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Workshop { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Intensity { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remarks { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserAsk { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? UpTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? MinAge { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? MaxAge { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Flow { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool First { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string NewMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobAchieve { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobSubsidy { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string NightSubsidy { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string HostelCondition { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OtherAsk { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Regularity { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string HighSubsidy { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobAge { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WorkTimeAbout { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool MinShow { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ShowTags { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? Commission { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CommissionDays { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SalaryStructure { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SpecialTag { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Number { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool XFZShow { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool LWShow { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool AnonyShow { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AnonyCompany { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsQuality { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
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
		
		public  int? ServiceUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CustomerStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  double? Latitude { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  double? Longitude { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? FactoryUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? OperateModel { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AccumulMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SocialMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PhysicalDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PhysicalMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BusLine { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ContactName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InterviewProcess { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BatteryCar { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Accumul { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Social { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Physical { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AdvanceMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsAdvance { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobDistrict { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobCity { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobProvince { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string GroupNotice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsJoint { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ServiceFullName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Phone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string VideoFile { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsDayOver { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? LunchHours { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? ShiftMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? AttendancePlan { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? NightAttendancePlan { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? DinnerHours { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? NightShiftMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? NightShiftSubsidy { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? AttendanceType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Integrals { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ServicePrivateQrcode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SubTitle { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EndTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Distance { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? SkillType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobSkills { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_JobsDayWork
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Title { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Tabs { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TabNames { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Sex { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Ages { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Address { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? MinMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? MaxMoney { get; set; }
		
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
		
		public  string WorkTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WorkContent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RestMode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Coverall { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Workshop { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Intensity { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BusLine { get; set; }
		
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
		
		public  string Remarks { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserAsk { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? UpTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? MinAge { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? MaxAge { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? SynTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool First { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ECount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Flow { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string NewMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobAchieve { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobSubsidy { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string NightSubsidy { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string HostelCondition { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OtherAsk { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Regularity { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string HighSubsidy { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobAge { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WorkTimeAbout { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool MinShow { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ShowTags { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? Commission { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CommissionDays { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SalaryStructure { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SpecialTag { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Number { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool XFZShow { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool LWShow { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool AnonyShow { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AnonyCompany { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsQuality { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsDayOver { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsJoint { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? AttendancePlan { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? NightAttendancePlan { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? AttendanceType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string VideoFile { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ServiceFullName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Phone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? TotalNumber { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? StaffNumber { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ServiceUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? FactoryUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? OperateModel { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Requirement { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Post { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? LunchHours { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? DinnerHours { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? ShiftMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? ShiftSubsidy { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? NightLunchHours { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? NightDinnerHours { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? NightShiftMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? NightShiftSubsidy { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? PartnerGID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_JobVideo
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? GID { get; set; }
		
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
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Path { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ApprovalName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ApprovalType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? AuditTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Message { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Content { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PartnerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PlanName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? InsuranceType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceCompany { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_JointAudit
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PartnerID { get; set; }
		
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
		
		public  DateTime? AuditTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? AuditUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PartnerIDS { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PartnerNames { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AuditRemarks { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? OperateModel { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PartnerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AuditFullName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JointObjType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? JointCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FullName { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_PartnerCustomer
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int CID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int PID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
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
		
		public  int? OperateModel { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? JointAuditStatus { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_PaymentAccount
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid PaymentSignUnitID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid PaymentApplicationID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string AccountName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string AccountNo { get; set; }
		
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
		
		public  DateTime? DeleteTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? UpdateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UseOrgKey { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UseOrgCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string UseOrgName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string BusinessTypeName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UnitName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UnitNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AppName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? BankType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CompanyGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CompanyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsDefault { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AppID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_PaymentAdmin
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid UserGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string SecretKey { get; set; }
		
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
		
		public  DateTime? DeleteTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsValidate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsLock { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? LockTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FailMsg { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? FailTimes { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? UpdateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string RealName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string DispalyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid PaymentApplicationID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AppName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? BankType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CompanyGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CompanyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CompanyID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_PaymentSignUnit
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string UnitName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string UnitNo { get; set; }
		
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
		
		public  DateTime? DeleteTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? UpdateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid PaymentApplicationID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AppName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? BankType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CompanyGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CompanyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CompanyID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_Personnel
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? UserGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FullName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserName { get; set; }
		
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
		
		public  string Phone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Picture { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string NickName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Enabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Sex { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string City { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Province { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CIDS { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CNames { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Qrcode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PartnerIDS { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? LastTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PartnerNames { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string LastIP { get; set; }
		
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
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_PersonnelItems
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
		
		public  int? CID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Qrcode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Enabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FullName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? GID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsUserAddress { get; set; }
		
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
		
		public  Guid? CompanyGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? OperateModel { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? EntryModel { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EntryWorkClass { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EntryWorkName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EntryFromType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CustomerPartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? JointPartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CustomerPartnerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? JointPartnerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? PartnerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsManage { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PartnerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsUserContact { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmergencyContact { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmergencyContactPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? UserGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsUseBusInsurance { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JDYDataID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? BusinessModel { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_PrepaidApplyRecord:ModelBasic
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public override int? CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CompanyGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int UserEntryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid PrepaidPartnerAccountID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public override int? PartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid CustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid SigningUnitID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string IdCardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsSinglePushSuc { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? AuditTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string BankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Bank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ApplyType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ApplyAmount { get; set; }
		
		/// <summary>
		/// PrepaidRecordStatusEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  PrepaidRecordStatusEnumType Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? ShiftEndTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? ShiftStartTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? ShiftHours { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? ShiftMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? LunchHours { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? NightShiftSubsidy { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? DinnerHours { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? MoneyHours { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public override int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public override int? PerUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? WorkType { get; set; }
		
		/// <summary>
		/// PrepaidRepayStatusEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  PrepaidRepayStatusEnumType RepayStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsNotice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RepayTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PartnerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Picture { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public override int? CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? PartnerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? EntryStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EntryTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SigningUnitName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? SendType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BankUserName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PdfFilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SignedFilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Phone { get; set; }
		
		/// <summary>
		/// PrepaidCashStatusEnumType
		/// </summary>		
		
		public  PrepaidCashStatusEnumType? CashStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? SendTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? FactoryUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AccountPartnerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? AccountPartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? AccountPartnerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CusPartnerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CusPartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CusPartnerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsBadRepay { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? BadRepayAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? BadRepayStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? BadRepayTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PaymentType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public override int? PerPartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int HandUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public override int? CustomerPartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Msg { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string DayWorkType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? StructureID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserIDS { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? RepayCompleteTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EntryNo { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_PrepaidInterestReport
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid PrepaidPartnerAccountID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid PartnerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string PartnerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int BillYear { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal PendingYearAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal PendingMonthAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal InterestYearAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal InterestMonthAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BasePercent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal FloatPercent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? BillMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? ApplyYearAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? ApplyMonthAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
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
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_PrepaidPartnerAccount
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid PartnerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal CashAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TotalAmount { get; set; }
		
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
		public  bool IsEnabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BadAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  byte[] Timestamp { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int PaymentType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BasePercent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal FloatPercent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SigningUnitID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SpecialPartnerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PartnerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SigningUnitName { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_PrepaidRepaymentRecord
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid PrepaidApplyRecordID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid SalaryCustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid SalaryRecordID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? RepaymentTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsSuc { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int UserEntryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsRetrunToAccount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IdCardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? ApplyAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? SendTime { get; set; }
		
		/// <summary>
		/// PrepaidRepayStatusEnumType
		/// </summary>		
		
		public  PrepaidRepayStatusEnumType? RepayStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? PrepaidPartnerAccountID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ApplyType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? AccountPartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? AccountPartnerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AccountPartnerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_ProfitConfig
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid CustomerGID { get; set; }
		
		/// <summary>
		/// ProfitBusInsuranceCountEnunType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int BusInsuranceCountType { get; set; }
		
		/// <summary>
		/// ProfitAgentCountEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  ProfitAgentCountEnumType AgentCountType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TaxPointPercent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal DifferenceBaseAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ProfitTaxPointPercent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BusInsuranceAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BusInsurance5Amount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RiskAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BusinessAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal DeductionAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SubsidyAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal PreBusInsuranceTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal PreRiskAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsHasInvoice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsEnabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal AgentPaidPercent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BusinessPercent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal JoinManagePercent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal CustomerThingPercent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SocialRepayRiskBaseAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal DeathRiskAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
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
		
		public  string SalesMan { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? BusinessModel { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string BusinessModelName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CityPartnerID { get; set; }
		
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
		
		public  string PartnerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SigningUnitName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SigningUnitGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SigningUnitBanNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SigningUnitBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SigningUnitLegalPerson { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SigningUnitPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SigningUnitAddress { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_ProfitManage
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid PartnerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string PartnerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? ProfitMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? TaxMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int RecordCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int UnitSalaryServiceFeeRecordCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int InJobRecordCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitSalaryTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitSocialTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ServiceFeeTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BusInsuranceCurrentTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BusInsuranceLastTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BusInsuranceNextTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BusInsuranceSumAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal RiskTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal AgentPaidTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal AgentPaidTaxAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal DisabilityTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BusinessTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal DeductionAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SubsidyAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal GrossAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal GrossContainTaxAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid SigningUnitID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string SigningUnitName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string SigningUnitBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string SigningUnitBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string SigningUnitPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string SigningUnitAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string SigningUnitNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string FilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string FileName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ServiceFeeRecordCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BusInsuranceBalanceTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? ProfitAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string DeductionRemark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SubsidyRemark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? CustomerThingTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? JoinManageTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? InterestTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal JoinManagePaidAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal JoinManageIncomeAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SocialRepayRiskAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CityPartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal DeathRiskTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PartnerID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_ProfitPartnerConfig
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal DeductionAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SubsidyAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string DeductionRemark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SubsidyRemark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid PartnerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int PartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsEnabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PartnerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CompanyGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CompanyID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_SalaryCashSendRecord:ModelBasic
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SalaryCustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SalaryRecordID { get; set; }
		
		/// <summary>
		/// CashSendStatusEnumType
		/// </summary>		
		
		public  CashSendStatusEnumType? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Reason { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? SendAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AgentName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AgentIdCardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AgentBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AgentBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SucFilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SignerOssUrl { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SignKeyword { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? SignTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? FailRecordID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Msg { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int AuditStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CashType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CashRemark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public override int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Name { get; set; }
		
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
		
		public  string BankUserName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IdCardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Phone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserEntryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SettlementID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? SendTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SigningUnitCompanyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? SalaryMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? SalaryCustomerStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public override int? CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CusAgentBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CusAgentBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CusAgentIdCardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CusAgentName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public override int? PartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? FactoryUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public override int? PerUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserEntryPartnerType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? UserEntryPartnerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserEntryPartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsCustomerPaid { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsDelayPayment { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsCashSend { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public override int? CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public override int? PerPartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public override int? CustomerPartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int HandUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? DelayRecordID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AuditFailMsg { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FromType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AgentTransPath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? AgentStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? AgentTransTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? SalaryRecordStatus { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_SalaryConfig
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid Id { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsVerifyEmpInfo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsVerifyPaySlip { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsCalculateTax { get; set; }
		
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
		
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CompanyGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string LegalCustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AgentBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AgentBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AgentName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CompanyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsCostSettlement { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsMatchZeroSalary { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsSalarySettlement { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsSalaryYearRepayment { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsMatchCmbcBankCard { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CombineCompanyGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsContractDelay { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsNeedSalarySlip { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_SalaryCustomer
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string SealCompanyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal CompanySocialSecurityAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ManagementFeeAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal CompanySendSalaryAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal CompanyDeductionAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal DeductionNoTaxAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SubsidyAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SubsidyNoTaxAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ServiceFeeAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ActualSalaryAmount { get; set; }
		
		/// <summary>
		/// SalaryStatusEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  SalaryStatusEnumType Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? SalaryMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? TaxMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? SendMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string WorkShop { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int SalaryBatchType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int SalarySendType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid CustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CustomerCompanyNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CustomerCompanyBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CustomerCompanyBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CustomerCompanyAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CustomerCompanyPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string SigningUnitCompanyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string SigningUnitCompanyNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string SigningUnitCompanyBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string SigningUnitCompanyBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string SigningUnitCompanyAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string SigningUnitCompanyPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CooperationUnitCompanyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CooperationUnitCompanyNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CooperationUnitCompanyBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CooperationUnitCompanyBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CooperationUnitCompanyAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CooperationUnitCompanyPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal InnerServiceFeeAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal CompanySendSalarySumAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal AccountsReceivableSumAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal CompanySettlementSumAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int SalaryType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal MonthNetSalaryAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int RecordCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ServiceFeeRecordCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsCustomerPaid { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsCompanyPosted { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid SigningUnitID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid CooperationUnitID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsMatchZeroSalary { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsMainBatchSalary { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitManageSumAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitSalarySumAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitDeductionSumAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitSalaryTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ServiceFeeSumAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal MonthNetSalarySumAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SocialUnitSumAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ReceivableTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal CustomerBillTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitSettlementAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ServiceRecordCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ZeroSalaryCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ActualSalarySumAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BeforeTaxSalarySumAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsPushTrans { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string TransStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string TransStatusName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid TransNonce { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsCmbcSend { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int SalaryYear { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsPlatformPrepaid { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsCanCmbcSend { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsSalarySlip { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SalarySlipID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsHasOutInvoice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CombineCompanyGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsCombineSalary { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CombineCustomerSalaryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsNoticeCompleted { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? WorkShopBatchNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsNoticeSendCompleted { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? SendTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? BudgetApplyRecordID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsCmbcSendFail { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int SalaryApiSendStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? PartnerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string LegalCustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PartnerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? LockedTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsLocked { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CustomerPaidTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? PushTransTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TransMsg { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PaymentStatus { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_SalaryDelaySendRecord:ModelBasic
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SalaryCustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SalaryRecordID { get; set; }
		
		/// <summary>
		/// SalaryDelaySendStatusEnumType
		/// </summary>		
		
		public  SalaryDelaySendStatusEnumType? Status { get; set; }
		
		/// <summary>
		/// SalaryDelaySendTypeEnumType
		/// </summary>		
		
		public  SalaryDelaySendTypeEnumType? DelayType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? SendAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsNotice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsToCash { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IdCardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Phone { get; set; }
		
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
		
		public  string BankUserName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SettlementID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public override int? CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? SalaryMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserEntryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ContractStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BankName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BankNumber { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserEntryName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public override int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public override int? PerUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? FactoryUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public override int? PartnerID { get; set; }
		
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
		
		public override int? CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public override int? PerPartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int HandUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public override int? CustomerPartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? SalaryRecordStatus { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_SalaryFailRecord:ModelBasic
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid SalaryCustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid SalaryRecordID { get; set; }
		
		/// <summary>
		/// SalaryFailRecordStatusEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  SalaryFailRecordStatusEnumType Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Description { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string BankUserName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Bank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string BankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsNotice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsToCash { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SalaryBankUserName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SalaryBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SalaryBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? SalaryMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? ActualSalarySendCardAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IdCardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Phone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public override int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserEntryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SettlementID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public override int? CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public override int? PerUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserEntryPartnerType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? UserEntryPartnerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserEntryPartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public override int? PartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? PartnerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? FactoryUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsCustomerPaid { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsDelayPayment { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsCashSend { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public override int? CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public override int? PerPartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public override int? CustomerPartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int HandUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ToBankUserName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ToBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ToBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? SalaryRecordStatus { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_SalaryRecord
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid SalaryCustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string IdCardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CompanyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Workshop { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? SalaryMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? SendMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ServiceFeeAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitSalaryAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitDeductionAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SubsidyAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SubsidyNoTaxAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal DeductionNoTaxAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsDelayPayment { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SocialYiLiaoAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SocialYangLaoAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SocialShiYeAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SocialDaBingAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SocialGongJiJinAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SocialEmpSumAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SocialUnitSumAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal PlatformCommissionAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal PlatformWelfareAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PlatformDepartment { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SpZiNvAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SpJiaoYuAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SpDaiKuanAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SpZuJinAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SpShanYangAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SpYingErAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SpSumAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SpTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal InnerServiceFeeAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SpBasicTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TaxSalaryTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TaxSpZiNvTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TaxSpJiaoYuTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TaxSpDaiKuanTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TaxSpZuJinTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TaxSpShanYangTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TaxSpYingErTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TaxSpTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SocialYiLiaoTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SocialYangLaoTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SocialShiYeTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SocialDaBingTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SocialGongJiJinTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SocialEmpTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal MonthNetSalaryAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TaxSocialTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int TaxTimes { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BeforeTaxSalaryAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BeforeTaxSalaryTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal CompanyTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal CompanySalarySumAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Bank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string BankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string BankUserName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsCashSend { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int UserEntryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? EntryDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? LeaveDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal AfterTaxAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal PreSendAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool VerifyInfoOk { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SpBasiclAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal LastSocialYiLiaoTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal LastSocialYangLaoTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal LastSocialShiYeTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal LastSocialDaBingTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal LastSocialGongJiJinTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal LastSocialEmpTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal LastBeforeTaxSalaryTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal LastTaxTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TaxTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TaxMonthAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitManageAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ActualSalaryAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal PrepaidRepaymentAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ActualSalarySendCardAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsDelayPaymentCancel { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsCashSignSuc { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Phone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsDelayPaymentSuc { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BankChangeFilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int DelayType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int SendType { get; set; }
		
		/// <summary>
		/// SalaryRecordSendStatusEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  SalaryRecordSendStatusEnumType Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int MonthWorkDays { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SettlementID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string TransStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string TransStatusName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsTransBatch { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int SalaryYear { get; set; }
		
		/// <summary>
		/// SalaryRecordTypeEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  SalaryRecordTypeEnumType SalaryRecordType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal DelayPaymentAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal PrepaidTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsBatchSend { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsBatchPushSuc { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string BatchTransStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string BatchTransStatusName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsSingleSend { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsSinglePushSuc { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TransDescription { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SalarySlipRecordID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsCombineSalary { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? SendTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsFailSalary { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TransMsg { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? ChangeAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ChangeRemark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ChangeHash { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? ChangeUpdateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerCompanyAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerCompanyBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerCompanyBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerCompanyNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerCompanyPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SigningUnitID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SigningUnitCompanyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SigningUnitCompanyAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SigningUnitCompanyBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SigningUnitCompanyBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SigningUnitCompanyNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SigningUnitCompanyPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationUnitCompanyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationUnitCompanyAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationUnitCompanyBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationUnitCompanyBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationUnitCompanyNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationUnitCompanyPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CooperationUnitID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? SalaryBatchType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CombineCompanyGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsCustomerPaid { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? SalaryCompanyStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? TaxMonth { get; set; }
		
		/// <summary>
		/// SalaryStatusEnumType
		/// </summary>		
		
		public  SalaryStatusEnumType? SalaryStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsNoticeCompleted { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsNoticeSendCompleted { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? SalarySendTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? PartnerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsSplitSend { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsSplitSendSuc { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsCanCmbcSend { get; set; }
		
		/// <summary>
		/// CmbcSendStatusEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  CmbcSendStatusEnumType CmbcSendStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SplitSendSucAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsPrepaidFail { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int BatchApiSendStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int SingleApiSendStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SingleApiBusinessID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SingleApiSendApplyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsLocked { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? LockedTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsFailSalarySuc { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int TransCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsAutoLocked { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool SalaryCustomerIsLocked { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CustomerPaidTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AgentName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AgentIdCardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AgentBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AgentBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsSendSuccess { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsPushToOrder { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_SalaryRecordSlipRecord
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid SalaryCustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string IdCardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CompanyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Workshop { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? SalaryMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? SendMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ServiceFeeAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitSalaryAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitDeductionAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SubsidyAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SubsidyNoTaxAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal DeductionNoTaxAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsDelayPayment { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SocialYiLiaoAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SocialYangLaoAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SocialShiYeAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SocialDaBingAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SocialGongJiJinAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SocialEmpSumAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SocialUnitSumAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal PlatformCommissionAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal PlatformWelfareAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PlatformDepartment { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SpZiNvAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SpJiaoYuAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SpDaiKuanAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SpZuJinAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SpShanYangAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SpYingErAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SpSumAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SpTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal InnerServiceFeeAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SpBasicTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TaxSalaryTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TaxSpZiNvTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TaxSpJiaoYuTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TaxSpDaiKuanTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TaxSpZuJinTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TaxSpShanYangTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TaxSpYingErTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TaxSpTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SocialYiLiaoTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SocialYangLaoTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SocialShiYeTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SocialDaBingTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SocialGongJiJinTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SocialEmpTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal MonthNetSalaryAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TaxSocialTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int TaxTimes { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BeforeTaxSalaryAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal BeforeTaxSalaryTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal CompanyTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal CompanySalarySumAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Bank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string BankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string BankUserName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsCashSend { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int UserEntryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? EntryDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? LeaveDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal AfterTaxAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal PreSendAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool VerifyInfoOk { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SpBasiclAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal LastSocialYiLiaoTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal LastSocialYangLaoTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal LastSocialShiYeTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal LastSocialDaBingTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal LastSocialGongJiJinTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal LastSocialEmpTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal LastBeforeTaxSalaryTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal LastTaxTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TaxTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TaxMonthAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal UnitManageAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ActualSalaryAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal PrepaidRepaymentAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal ActualSalarySendCardAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsDelayPaymentCancel { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsCashSignSuc { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Phone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsDelayPaymentSuc { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BankChangeFilePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int DelayType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int SendType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int MonthWorkDays { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SettlementID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string TransStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string TransStatusName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsTransBatch { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int SalaryYear { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int SalaryRecordType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal DelayPaymentAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal PrepaidTotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsBatchSend { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsBatchPushSuc { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string BatchTransStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string BatchTransStatusName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsSingleSend { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsSinglePushSuc { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TransDescription { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SalarySlipRecordID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsCombineSalary { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? SendTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsFailSalary { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TransMsg { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ContentKV { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string HeaderKV { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? TaxMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? SalaryBatchType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? SalaryCustomerStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? ChangeAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ChangeRemark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ChangeHash { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? ChangeUpdateTime { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_SalarySlip
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid CustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? SalaryMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int TotalNum { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsNotice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string HeaderKV { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SalaryCustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? SalaryCustomerStatus { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_SalarySpecialDedcution
    {
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string IdCardNo { get; set; }
		
		/// <summary>
		/// DeductionEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DeductionEnumType DeductionType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? StartMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? EndMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RelationName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RelationIdCardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? MonthAmount { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_SalarySplitSend
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid SalaryRecordID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string IdCardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Bank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string BankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string BankUserName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid PartnerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string PartnerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid CustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
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
		
		public  DateTime? SendTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SalaryDelaySendRecordID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal PendingSplitAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SendSucAmount { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class live_V_SalarySplitSendRecord
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid SalarySplitSendID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SplitAmount { get; set; }
		
		/// <summary>
		/// SalarySplitSendRecordStatusEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  SalarySplitSendRecordStatusEnumType Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? SendTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// SalarySplitSendStatusEnumType
		/// </summary>		
		
		public  SalarySplitSendStatusEnumType? SalarySplitSendStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Name { get; set; }
		
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
		
		public  string BankUserName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? FactoryUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? PartnerGID { get; set; }
		
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
		
		public  int? PerUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IdCardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? TaxMonth { get; set; }
		
		/// <summary>
		/// SalaryStatusEnumType
		/// </summary>		
		
		public  SalaryStatusEnumType? SalaryCustomerStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SigningUnitCompanyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? SalaryMonth { get; set; }
		
		/// <summary>
		/// SalaryRecordSendStatusEnumType
		/// </summary>		
		
		public  SalaryRecordSendStatusEnumType? SalaryRecordStatus { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_Settlement
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid CustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? IssuerUnitID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string IssuerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string IssuerCompanyNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string IssuerPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string IssuerAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string IssuerBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string IssuerBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? RecipientUnitID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string RecipientName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string RecipientCompanyNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string RecipientPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string RecipientAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string RecipientBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string RecipientBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int BusinessType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TotalAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Title { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Description { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? ApplyDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? SalaryMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int SettlementType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SalaryCustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerCompanyNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? InsuranceStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CompanyID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_ThirdSalarySlipRecord
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string IdCardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal SocialSubsidy { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? SalaryMonth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int UserEntryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsNotice { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsReceive { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? NoticeTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? ReceiveTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string SlipContent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid ThirdSalarySlipID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PartnerName { get; set; }
		
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
		
		public  int? CompanyID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_TransBatchRecord
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string IdCardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Bank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string BankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string BankUserName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal TransAmount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string TranStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string TranStatusName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid TransBatchID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid BusinessID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TransMsg { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RecordID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TransStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string TransStatusName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_User
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? UserGID { get; set; }
		
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
		
		public  string UserName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FullName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Password { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OpenID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UnionId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? FUID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string NickName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Picture { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Phone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ClientId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? BirthDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Qrcode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Enabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WeiXinNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? Money { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? SID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsPerfect { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Recommend { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Sex { get; set; }
		
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
		
		public  int? Type { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? JobStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CusID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsSign { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WeiXinOpenId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FromType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? SceneCusID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsReal { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsDayOver { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? DayOverStartTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsDaySign { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? DayOverCID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? DayOverEndTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? LastTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string LastIP { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PersonnelName { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_UserEntry:ModelBasic
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public override int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? SID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public override int? CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? JID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EntryTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? LeaveTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public override int? PerUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? HandUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? LaborUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? Days { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? ComDays { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WorkName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WorkClass { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FromType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsExpire { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? ExpireDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsDayOver { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool EndCharge { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public override int? PartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? PartnerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PersonnelName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PartnerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string LaborName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string HandName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Picture { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? QrcodeGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsUser { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ContractStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? FactoryUserID { get; set; }
		
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
		
		public  string Sex { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Age { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IntAbout { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? OperateModel { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsBlack { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? IntDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? ContractDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IdcardFrontImage { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IdcardBackImage { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BankImage { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IdcardAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Nation { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Authority { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CardStartDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CardEndDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BankNumber { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BankName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BankID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BankCardName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BankCardType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string MaritalStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string MajorDiseases { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmergencyContact { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmergencyContactPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CardID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public override int? CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CompanyGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? InjuryStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PlanName1 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PlanName2 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? UserGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RealMessage { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SigningUnitGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CooperationUnitGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SignName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SignAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SignBankName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SignBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationBankName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CooperationBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SignCompanyNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SignPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public override int? CustomerPartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CompanyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PartnerType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PartnerLevel { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ToCompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IdPhoto { get; set; }
		
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
		
		public  int? BusinessModel { get; set; }
		
		/// <summary>
		/// UserEntryInsuranceStatusEnumType
		/// </summary>		
		
		public  UserEntryInsuranceStatusEnumType? InsuranceStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? EntryDays { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string DayStartTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string DayEndTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string NightStartTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string NightEndTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? DayOverStartTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsDaySign { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? DayOverEndTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  double? Latitude { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  double? Longitude { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? JointPartnerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  decimal? LimitMoney { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool SocialMark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? SocialStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? SocialID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsRetire { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Education { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JointPartnerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AgentName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AgentIdCardNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AgentBank { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AgentBankNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomizeField { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SocialGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public override int? PerPartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? PerCityPartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? BirthDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? JointPartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? EntryModel { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsStrike { get; set; }
		
		/// <summary>
		/// UserEntryPlanStatusEnumType
		/// </summary>		
		
		public  UserEntryPlanStatusEnumType? PlanStatus2 { get; set; }
		
		/// <summary>
		/// UserEntryPlanStatusEnumType
		/// </summary>		
		
		public  UserEntryPlanStatusEnumType? PlanStatus1 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remarks { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? RetireDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? ContractStartDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? ContractEndDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsRenewal { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BankProvince { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BankCity { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ToCompanyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string LastNote { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceCompany1 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceCompany2 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceId1 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceId2 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsReal { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? StructureID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsManuallyInsurance { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string StructureName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? AttendanceType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsDayWork { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? JobDayWorkID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobDayWorkNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? AttendancePlan { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? NightAttendancePlan { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerProvince { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerCity { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? InsuranceProcessStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? GID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? InsuranceType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsCaAuth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Tabs { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JDYDataID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserBusinessModel { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? SkillType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JobSkills { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SpecialCertificate { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_UserEntryContract
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CardID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IdcardAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Phone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmergencyContact { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string EmergencyContactPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WorkName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WorkClass { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EntryTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? LeaveTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsRetire { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Education { get; set; }
		
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
		
		public  int? CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerCity { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerCompanyNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? SigningUnitID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SigningUnitName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SigningUnitCompanyNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SigningUnitLegalPerson { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SigningUnitPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SigningUnitAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsRenewal { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? ContractEndDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? ContractStartDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? RetireDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? ContractDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomizeField { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_UserEntryInfo
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		
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
		
		public  string CardID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Phone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IdcardBackImage { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IdcardFrontImage { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CardStartDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CardEndDate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EntryTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CompanyNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CompanyAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string HomeAddress { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_UserEntryInsuracneStatus
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CardID { get; set; }
		
		/// <summary>
		/// UserEntryPlanStatusEnumType
		/// </summary>		
		
		public  UserEntryPlanStatusEnumType? PlanStatus1 { get; set; }
		
		/// <summary>
		/// UserEntryPlanStatusEnumType
		/// </summary>		
		
		public  UserEntryPlanStatusEnumType? PlanStatus2 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PlanName1 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PlanName2 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceCompany1 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceCompany2 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsUseInsurance { get; set; }
		
		/// <summary>
		/// InsuranceProcessEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  InsuranceProcessEnumType InsuranceProcessStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WorkName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EntryTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? LeaveTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsManuallyInsurance { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceWorkName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceWorkClass { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string WorkClass { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceId1 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InsuranceId2 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CustomerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsUseBusInsurance { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool CusEnable { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsUseDeathInsurance { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SignUnitName { get; set; }
		
		/// <summary>
		/// InsuranceTypeEnumType
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  InsuranceTypeEnumType InsuranceType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CustomerGID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_UserEntryNote
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
		
		public  int? EntryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PersonnelID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remarks { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FullName { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_UserEntryRecord
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
		
		public  int? EntryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string About { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? SID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? AuditUID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? AuditStatus { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? AuditTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AuditAbout { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EntryTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Title { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? LeaveTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PerUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? LaborUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FullName { get; set; }
		
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
		
		public  string PerFullName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CardID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? OperateUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? OperateFrom { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OperateFullName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OperateFromName { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_UserEntryUnit
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CardID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EntryTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? LeaveTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CustomerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CustomerID { get; set; }
		
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
		
		public  int? PlanStatus1 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CustomerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SalesMan { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PerPartnerName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CityPartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? PerPartnerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PerPartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CityPartnerTitle { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CityPartnerAgentName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CompanyCityPartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CompanyCityPartnerTitle { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_UserOpenIdsFxq
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int FromID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OpenId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string OpenIdType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? UserGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FxqAppName { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_V_WhiteList
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? GID { get; set; }
		
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
		
		public  int? LabelID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string LabelName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Content { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IdentityCard { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool Enabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EndTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PartnerName { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_VoteItems
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
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ClassName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_VoteUser
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int UserID { get; set; }
		
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
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string LikeProduct { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_WeiXinMaterial
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ClientID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string MediaID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Path { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Title { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? MediaType { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_WeiXinMenu
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
		
		public  int? PID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Keyword { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Url { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Sort { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? MType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string MiniAppID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PagePath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? FromType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ClientID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_WeiXinReply
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
		
		public  string Keyword { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ReplyType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsLike { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ReplyContent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Sort { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ClientID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class Live_WhiteList
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? GID { get; set; }
		
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
		
		public  int? LabelID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string LabelName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Content { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IdentityCard { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool Enabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EndTime { get; set; }
		   
    }



	/// <summary>
	/// 与甲方客户的关联码 - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class SYS_Company
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid CompanyGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CompanyName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CutType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Industry { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Tel { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Phone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Contact { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Address { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BusinessLicense { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? MUID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FromType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Config { get; set; }
		
		/// <summary>
		/// 与甲方客户的关联码
		/// </summary>		
		
		public  string RelatedCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Password { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string MiniPhone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string MiniName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CityPartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Products { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CompanyNo { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PersonnelNeeds { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string JDYForms { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class SYS_CompanyDictionary
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true)]
		public  Guid GUID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? PID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string NO { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Enabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Sort { get; set; }
		
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
	[TenantAttribute("Main")]
	public partial class SYS_CompanyFunction
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true)]
		public  int CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid FGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OperateName { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class SYS_CompanyFunctionMenu
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
		public  Guid FGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? PID { get; set; }
		
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
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Icon { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Url { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Enabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OperateNumber { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Sort { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Number { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OperateName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Component { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Hidden { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool HideChildren { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Redirect { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class SYS_CompanyRoles
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true)]
		public  Guid RGID { get; set; }
		
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
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Enabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Number { get; set; }
		   
    }



	/// <summary>
	/// 合作伙伴 - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class SYS_CompanyUser
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid UserGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? DeptGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CompanyGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CompanyID { get; set; }
		
		/// <summary>
		/// 合作伙伴
		/// </summary>		
		
		public  int? PartnerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? PartnerGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserPassword { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PasswordSalt { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? LoingCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? LastTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string LastIP { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Enabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Sort { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FullName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Picture { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RoleIDS { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RoleNames { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RoleNumbers { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Phone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PersonnelID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? PersonnelGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ShopID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class SYS_CompanyUserRoles
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true)]
		public  Guid RGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int UserID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class SYS_Content
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CID { get; set; }
		
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
		
		public  string Contents { get; set; }
		
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
		
		public  bool Commend { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsTop { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsAudit { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ShowPicture { get; set; }
		
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
		
		public  string Annex { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ShowCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Url { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ClassName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Field3 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Field4 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Field5 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Field6 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Field7 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Field8 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Field9 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Field10 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Field11 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Field12 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Field13 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Field14 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Field15 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Field16 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Field17 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Field18 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Field19 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Field20 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Field2 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Field1 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Profiles { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class SYS_ContentClass
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
		
		public  int? PID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ListType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Sort { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Commend { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsTop { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsAudit { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Picture { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Banner { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Contents { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Annex { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Url { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remarks { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ListName { get; set; }
		
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
	[TenantAttribute("Main")]
	public partial class SYS_ContentClassFiles
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FileName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? PID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? InputType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Sort { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Value { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class SYS_CustomerUserToken
    {
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true)]
		public  string Token { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FromType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EndTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserAgent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IPAddress { get; set; }
		   
    }



	/// <summary>
	/// 是否不限制预发工资 - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class SYS_Department
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true)]
		public  Guid DeptGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsIdentity=true)]
		public  int DeptID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? PID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string DeptName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? DeptType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Enabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Sort { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CustomerID { get; set; }
		
		/// <summary>
		/// 是否不限制预发工资
		/// </summary>		
		
		public  bool Unlimited { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CompanyNames { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CompanyGIDS { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class SYS_Dictionary
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsIdentity=true)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true)]
		public  Guid GUID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? PID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string NO { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Enabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Sort { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Type { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class SYS_FunctionMenu
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
		public  Guid FGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? PID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Icon { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Url { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Enabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OperateNumber { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Sort { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Number { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OperateName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Component { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Hidden { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool HideChildren { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Redirect { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsPublic { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string CutTypes { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Products { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class SYS_ManageRoles
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true)]
		public  Guid RGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int UserID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class SYS_ManageUser
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? UserGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? DeptGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserPassword { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FullName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PasswordSalt { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? LoingCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? LastTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string LastIP { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Enabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Sort { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string DeptName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RGIDS { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Picture { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Phone { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RoleIDS { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RoleNames { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string RoleNumbers { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserType { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class SYS_ManageUserToken
    {
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true)]
		public  string Token { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FromType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EndTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserAgent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IPAddress { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class SYS_OperationLog
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
		
		public  int? Platform { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Content { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Json { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string IpAddress { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string UserAgent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Method { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Code { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CompanyID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FullName { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class SYS_Roles
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true)]
		public  Guid RGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool Enabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Number { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsPublic { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? CompanyGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CompanyID { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class SYS_RolesFunciton
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true)]
		public  Guid FGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid RGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OperateNumber { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class SYS_SiteSettings
    {
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Key { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Value { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class SYS_Tokens
    {
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true)]
		public  string Token { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FromType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? EndTime { get; set; }
		
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
		
		public  int? PlatformType { get; set; }
		
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
		
		public  string CompanyType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string FullName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ShopID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserType { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Main")]
	public partial class SYS_V_Content
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CID { get; set; }
		
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
		
		public  string Contents { get; set; }
		
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
		
		public  bool Commend { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsTop { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  bool IsAudit { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ShowPicture { get; set; }
		
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
		
		public  string Annex { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ShowCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Url { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ClassName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Field3 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Field4 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Field5 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Field6 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Field7 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Field8 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Field9 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Field10 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Field2 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Field1 { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Name { get; set; }
		   
    }



}


