using SqlSugar;

namespace LZ.Basic.Service.Departments;

// The generated SYS_Department entity does not contain the reimbursement department columns.
[SugarTable("SYS_Department")]
public class DepartmentRecord
{
    [SugarColumn(IsPrimaryKey = true)]
    public Guid DeptGID { get; set; }

    [SugarColumn(IsIdentity = true)]
    public int DeptID { get; set; }

    public int? CompanyID { get; set; }
    public Guid? CompanyGID { get; set; }
    public Guid? PID { get; set; }
    public string? DeptName { get; set; }
    public int? DeptType { get; set; }
    public bool? Enabled { get; set; }
    public DateTime? CreateTime { get; set; }
    public int? Sort { get; set; }
    public Guid? ExpReportID { get; set; }
}

[SugarTable("FIN_Rembi__BudgetManage")]
internal class DepartmentBudgetReference
{
    public int? PartnerID { get; set; }
    public Guid? PartnerGID { get; set; }
}
