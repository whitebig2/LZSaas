



 


using System;
using SqlSugar;
using LZ.Model.Enums;
namespace LZ.Model
{	

	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Agent")]
	public partial class AI_Agent
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int AgentID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid AgentGID { get; set; }
		
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
		public  string Code { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Avatar { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Description { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string SystemPrompt { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PersonaTemplate { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ModelID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ConfigJson { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string PublishJson { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  byte Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool AllowInvoke { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CreateUserID { get; set; }
		
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
		public  bool IsDeleted { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Agent")]
	public partial class AI_AgentModel
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ModelID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string ProviderKey { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string ModelCode { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string DisplayName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Capabilities { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int MaxInTokens { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int MaxOutTokens { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal PriceIn { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal PriceOut { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsDefault { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int SortOrder { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  byte Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? UpdateTime { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Agent")]
	public partial class AI_AgentSkill
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int AgentSkillID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int AgentID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int SkillID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool Enabled { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ParamOverrideJson { get; set; }
		
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
	[TenantAttribute("Agent")]
	public partial class AI_Attachment
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int AttachmentID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid AttachmentGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int SessionID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? MessageGID { get; set; }
		
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
		public  int UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string FileUrl { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string FileName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string FileType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int FileSize { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Kind { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OcrRef { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  byte Status { get; set; }
		
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
	[TenantAttribute("Agent")]
	public partial class AI_CardType
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int CardTypeID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string CardType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string DisplayName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string ComponentKey { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Platforms { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int SchemaVersion { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  byte Status { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Agent")]
	public partial class AI_CompanyAgentConfig
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ConfigID { get; set; }
		
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
		
		public  int? MaxAgents { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? MaxSessionsPerUser { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  long TokenQuotaDay { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ModelIds { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ExtraJson { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CreateUserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? UpdateTime { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Agent")]
	public partial class AI_InvokeLog
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  long InvokeLogID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid TraceID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ChainJson { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int CallerAgentID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int CalleeAgentID { get; set; }
		
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
		public  int UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string InputSummary { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string OutputSummary { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  byte Depth { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int Duration { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  byte Status { get; set; }
		
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
	[TenantAttribute("Agent")]
	public partial class AI_Knowledge
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int KnowledgeID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid KnowledgeGID { get; set; }
		
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
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Description { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string BailianKBId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ChunkSize { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int Overlap { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int TopK { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  double ScoreThreshold { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  byte Status { get; set; }
		
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
		public  bool IsDeleted { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Agent")]
	public partial class AI_KnowledgeDoc
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int KnowledgeDocID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid KnowledgeDocGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int KnowledgeID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string FileUrl { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string FileName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string FileType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int PageSize { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ChunkCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  byte Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ErrorMsg { get; set; }
		
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
		public  bool IsDeleted { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Agent")]
	public partial class AI_LLMProvider
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ProviderID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string ProviderKey { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string DisplayName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string BaseUrl { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ApiKeyEnc { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string ChatPath { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  byte Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Remark { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? UpdateTime { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Agent")]
	public partial class AI_McpServer
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int McpServerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid McpServerGID { get; set; }
		
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
		public  string Name { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string EndpointUrl { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  byte[] AuthHeadersEnc { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool PassUserContext { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ToolsCacheJson { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  byte Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? LastSyncTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CreateUserID { get; set; }
		
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
		public  bool IsDeleted { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Agent")]
	public partial class AI_Memory
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int MemoryID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid MemoryGID { get; set; }
		
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
		
		public  int? AgentID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  byte Scope { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  byte MemoryType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Content { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Summary { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  byte Importance { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool IsPinned { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  byte[] Embedding { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string VecProvider { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string VecRef { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? ExpireTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? SourceSessionID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? CreateUserID { get; set; }
		
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
		public  bool IsDeleted { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Agent")]
	public partial class AI_Message
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  long MessageID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid MessageGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int SessionID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Role { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Content { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Extra { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  byte Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? PendingRequestId { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int Tokens { get; set; }
		
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
	[TenantAttribute("Agent")]
	public partial class AI_PlatformConfig
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ConfigID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int MaxAgents { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int MaxSessionsPerUser { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  long TokenQuotaDay { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ModelIds { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ExtraJson { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? UpdateTime { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Agent")]
	public partial class AI_Session
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int SessionID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid SessionGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int AgentID { get; set; }
		
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
		public  int UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  byte PlatformType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Title { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Summary { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ModelID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  byte Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? LastActiveTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int MsgCount { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  long TotalTokens { get; set; }
		
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
		public  bool IsDeleted { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Agent")]
	public partial class AI_Skill
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int SkillID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  Guid SkillGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Name { get; set; }
		
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
		public  byte Kind { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  byte Permission { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ServiceName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Path { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string HttpMethod { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  bool CheckCompany { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  string Platforms { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? McpServerID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string McpToolName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ParamsJson { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ResultCardType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  byte Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? UpdateTime { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Agent")]
	public partial class AI_TokenLog
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  long TokenLogID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int AgentID { get; set; }
		
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
		public  int UserID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int ModelID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int InputTokens { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int OutputTokens { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  int Duration { get; set; }
		
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
	[TenantAttribute("Agent")]
	public partial class AI_UsageStat
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int StatID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? StatDate { get; set; }
		
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
		public  int AgentID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  long InputTokens { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  long OutputTokens { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  decimal Cost { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false)]
		public  DateTime? CreateTime { get; set; }
		   
    }



}


