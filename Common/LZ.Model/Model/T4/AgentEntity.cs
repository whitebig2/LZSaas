



 


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
		
		public  Guid? AgentGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AgentName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string AgentVersion { get; set; }
		
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
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? ModelGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string Prompt { get; set; }
		   
    }



	/// <summary>
	///  - 实体层
	/// </summary>
	[Serializable]
	[TenantAttribute("Agent")]
	public partial class AI_Models
    {
		
		/// <summary>
		/// 
		/// </summary>		
		[SugarColumn(IsNullable=false,IsPrimaryKey=true,IsIdentity=true)]
		public  int ModelID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  Guid? ModelGID { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  string ModelName { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? Status { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  int? ModelType { get; set; }
		
		/// <summary>
		/// 
		/// </summary>		
		
		public  DateTime? CreateTime { get; set; }
		   
    }



}


