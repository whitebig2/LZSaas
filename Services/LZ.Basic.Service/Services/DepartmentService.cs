using LZ.Basic.Service.Departments;
using LZ.Basic.Service.Interface;
using LZ.Common.Extensions;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Model;
using LZ.Model.Enums.Reimbusement;
using LZ.Model.Exceptions;
using Microsoft.AspNetCore.Mvc.Controllers;
using SqlSugar;
using SqlSugar.IOC;

namespace LZ.Basic.Service.Services;

public class DepartmentService : ServicesBase<Live_Partner>, IDepartmentService, IScopedDependency
{
    private ISqlSugarClient Departments => DbScoped.SugarScope.GetConnection("Main");
    private ISqlSugarClient Finance => DbScoped.SugarScope.GetConnection("Finance");

    public async Task<List<Live_Partner>> GetDeptTreeAsync(int companyId, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        return await base.Context.Queryable<Live_Partner>()
            .Where(d => d.CompanyID == companyId)
            .OrderBy(d => d.PartnerID)
            .ToListAsync();
    }

    public async Task<Live_Partner> GetAsync(Guid deptGID, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        return await base.Context.Queryable<Live_Partner>().FirstAsync(d => d.PartnerGID == deptGID)!;
    }

    public async Task<int> AddAsync(Live_Partner dept, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        Validate(dept);
        if (dept.CompanyID is not > 0)
            throw new ErrorCodeException(-1, "公司ID不能为空");

        await ValidateParentAsync(dept, ct);
        dept.DeptGID = Guid.NewGuid();
        dept.DeptID = 0;
        dept.CreateTime = DateTime.Now;
        dept.Enabled = false;
        return await base.Context.Insertable(dept).ExecuteCommandAsync();
    }

    public async Task<bool> UpdateAsync(Live_Partner dept, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        Validate(dept);
        var existing = await GetAsync(dept.PartnerGID.Value, ct);
        if (existing == null)
            return false;

        dept.CompanyID = existing.CompanyID;
        await ValidateParentAsync(dept, ct);
        if (dept.PartnerPID.HasValue)
        {
            var ancestor = dept.PartnerPID;
            var visited = new HashSet<Guid>();
            while (ancestor.HasValue && ancestor.Value != Guid.Empty)
            {
                if (!visited.Add(ancestor.Value) || ancestor.Value == dept.PartnerGID)
                    throw new ErrorCodeException(-1, "上级部门不能是当前部门或其下级部门");
                var parent = await GetAsync(ancestor.Value, ct);
                ancestor = parent?.PartnerPID;
            }
        }

        return await base.Context.Updateable<Live_Partner>()
            .SetColumns(d => new Live_Partner
            {
                PartnerPID = dept.PartnerPID,
                PartnerName = dept.PartnerName,
                PartnerType = dept.PartnerType,
                Enabled = dept.Enabled,
                IsBudget = dept.IsBudget,
                PartnerLevel = dept.PartnerLevel,
                DeptType = dept.DeptType,
                LimitMoney = dept.LimitMoney,
                EntryDays = dept.EntryDays,
                CityPartnerID = dept.CityPartnerID,
                CompanyGID = dept.CompanyGID,
                CompanyID = dept.CompanyID,
            })
            .Where(d => d.PartnerGID == dept.PartnerGID.Value && d.CompanyID == existing.CompanyID)
            .ExecuteCommandAsync() > 0;
    }

    public async Task<bool> DeleteAsync(Guid deptGID, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        if (!await CanDeleteAsync(deptGID, ct))
            return false;
        return await base.Context.Deleteable<Live_Partner>()
            .Where(d => d.PartnerGID == deptGID)
            .ExecuteCommandAsync() > 0;
    }

    public async Task<bool> CanDeleteAsync(Guid deptGID, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        var dept = await GetAsync(deptGID, ct);
        if (dept == null)
            return false;
        if (await base.Context.Queryable<Live_Partner>().AnyAsync(d => d.PartnerPID == deptGID))
            return false;
        //TODO:关于预算部分的判断暂时先不增加
        //return !await base.Context.Queryable<FIN_Rembi__BudgetManage>()
        //    .AnyAsync(b => b.PartnerID == dept.DeptID || b.PartnerGID == dept.DeptGID);

        return true;
    }

    private static void Validate(Live_Partner dept)
    {
        if (string.IsNullOrWhiteSpace(dept.PartnerName) || dept.PartnerName.Length > 50)
            throw new ErrorCodeException(-1, "部门名称不能为空且不能超过50个字符");
        dept.PartnerName = dept.PartnerName.Trim();
        if (dept.PartnerPID == Guid.Empty)
            dept.PartnerPID = null;
    }

    private async Task ValidateParentAsync(Live_Partner dept, CancellationToken ct)
    {
        if (!dept.PartnerPID.HasValue)
            return;
        var parent = await GetAsync(dept.PartnerPID.Value, ct);
        if (parent == null || parent.CompanyID != dept.CompanyID)
            throw new ErrorCodeException(-1, "上级部门不存在或不属于当前公司");
    }
}
