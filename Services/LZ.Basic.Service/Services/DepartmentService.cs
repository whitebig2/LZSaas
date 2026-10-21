using LZ.Basic.Service.Departments;
using LZ.Basic.Service.Interface;
using LZ.Core.Interface;
using LZ.Model.Exceptions;
using SqlSugar;
using SqlSugar.IOC;

namespace LZ.Basic.Service.Services;

public class DepartmentService : IDepartmentService, IScopedDependency
{
    private ISqlSugarClient Departments => DbScoped.SugarScope.GetConnection("Main");
    private ISqlSugarClient Finance => DbScoped.SugarScope.GetConnection("Finance");

    public Task<List<DepartmentRecord>> GetDeptTreeAsync(int companyId, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        return Departments.Queryable<DepartmentRecord>()
            .Where(d => d.CompanyID == companyId)
            .OrderBy(d => d.Sort)
            .OrderBy(d => d.DeptID)
            .ToListAsync();
    }

    public Task<DepartmentRecord?> GetAsync(Guid deptGID, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        return Departments.Queryable<DepartmentRecord>().FirstAsync(d => d.DeptGID == deptGID)!;
    }

    public async Task<int> AddAsync(DepartmentRecord dept, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        Validate(dept);
        if (dept.CompanyID is not > 0)
            throw new ErrorCodeException(-1, "公司ID不能为空");

        await ValidateParentAsync(dept, ct);
        dept.DeptGID = Guid.NewGuid();
        dept.DeptID = 0;
        dept.CreateTime = DateTime.Now;
        dept.Enabled ??= false;
        return await Departments.Insertable(dept).ExecuteCommandAsync();
    }

    public async Task<bool> UpdateAsync(DepartmentRecord dept, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        Validate(dept);
        var existing = await GetAsync(dept.DeptGID, ct);
        if (existing == null)
            return false;

        dept.CompanyID = existing.CompanyID;
        await ValidateParentAsync(dept, ct);
        if (dept.PID.HasValue)
        {
            var ancestor = dept.PID;
            var visited = new HashSet<Guid>();
            while (ancestor.HasValue && ancestor.Value != Guid.Empty)
            {
                if (!visited.Add(ancestor.Value) || ancestor.Value == dept.DeptGID)
                    throw new ErrorCodeException(-1, "上级部门不能是当前部门或其下级部门");
                var parent = await GetAsync(ancestor.Value, ct);
                ancestor = parent?.PID;
            }
        }

        return await Departments.Updateable<DepartmentRecord>()
            .SetColumns(d => new DepartmentRecord
            {
                PID = dept.PID,
                DeptName = dept.DeptName,
                DeptType = dept.DeptType,
                Enabled = dept.Enabled,
                Sort = dept.Sort
            })
            .Where(d => d.DeptGID == dept.DeptGID && d.CompanyID == existing.CompanyID)
            .ExecuteCommandAsync() > 0;
    }

    public async Task<bool> DisableAsync(Guid deptGID, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        if (!await CanDeleteAsync(deptGID, ct))
            return false;
        return await Departments.Updateable<DepartmentRecord>()
            .SetColumns(d => d.Enabled == false)
            .Where(d => d.DeptGID == deptGID)
            .ExecuteCommandAsync() > 0;
    }

    public async Task<bool> CanDeleteAsync(Guid deptGID, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        var dept = await GetAsync(deptGID, ct);
        if (dept == null)
            return false;
        if (await Departments.Queryable<DepartmentRecord>().AnyAsync(d => d.PID == deptGID))
            return false;
        return !await Finance.Queryable<DepartmentBudgetReference>()
            .AnyAsync(b => b.PartnerID == dept.DeptID || b.PartnerGID == dept.DeptGID);
    }

    private static void Validate(DepartmentRecord dept)
    {
        if (string.IsNullOrWhiteSpace(dept.DeptName) || dept.DeptName.Length > 50)
            throw new ErrorCodeException(-1, "部门名称不能为空且不能超过50个字符");
        dept.DeptName = dept.DeptName.Trim();
        if (dept.PID == Guid.Empty)
            dept.PID = null;
    }

    private async Task ValidateParentAsync(DepartmentRecord dept, CancellationToken ct)
    {
        if (!dept.PID.HasValue)
            return;
        var parent = await GetAsync(dept.PID.Value, ct);
        if (parent == null || parent.CompanyID != dept.CompanyID)
            throw new ErrorCodeException(-1, "上级部门不存在或不属于当前公司");
    }
}
