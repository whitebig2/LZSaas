using LZ.Basic.Service.Departments;
using LZ.Basic.Service.Interface;
using LZ.Core.Attributes;
using LZ.Model;
using LZ.Model.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LZ.Basic.API.Controllers;

[Route("api/basic/departments")]
[PlatformLimit(PlatformType.Admin)]
[PlatformLimit(PlatformType.MiniAdmin)]
public class DepartmentController(IDepartmentService departments) : BasicControllerBase
{
    [HttpGet]
    //[AllowAnonymous]
    public async Task<IActionResult> List([FromQuery] int? companyId, CancellationToken ct)
    {
        var id = CurrentUser.PlatformType == PlatformType.Admin ? companyId : CurrentUser.CompanyID;
        if (id is not > 0)
            return BadRequest("公司ID不能为空");
        return Ok(await departments.GetDeptTreeAsync(id.Value, ct));
    }

    [HttpGet("{deptGid:guid}")]
    //[AllowAnonymous]
    public async Task<IActionResult> Get(Guid deptGid, CancellationToken ct)
    {
        var dept = await departments.GetAsync(deptGid, ct);
        return dept == null || !CanAccess(dept) ? NotFound() : Ok(dept);
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] DepartmentRecord dept, CancellationToken ct)
    {
        if (CurrentUser.PlatformType != PlatformType.Admin)
        {
            dept.CompanyID = CurrentUser.CompanyID;
            dept.CompanyGID = CurrentUser.CompanyGID;
        }
        else if (dept.CompanyID is not > 0)
            return BadRequest("公司ID不能为空");

        await departments.AddAsync(dept, ct);
        return CreatedAtAction(nameof(Get), new { deptGid = dept.DeptGID }, dept);
    }

    [HttpPut("{deptGid:guid}")]
    public async Task<IActionResult> Update(Guid deptGid, [FromBody] DepartmentRecord dept, CancellationToken ct)
    {
        var existing = await departments.GetAsync(deptGid, ct);
        if (existing == null || !CanAccess(existing))
            return NotFound();
        dept.DeptGID = deptGid;
        return await departments.UpdateAsync(dept, ct) ? NoContent() : NotFound();
    }

    [HttpDelete("{deptGid:guid}")]
    public async Task<IActionResult> Delete(Guid deptGid, CancellationToken ct)
    {
        var existing = await departments.GetAsync(deptGid, ct);
        if (existing == null || !CanAccess(existing))
            return NotFound();
        if (!await departments.CanDeleteAsync(deptGid, ct))
            return Conflict("部门下存在子部门或关联预算记录，不能删除");
        return await departments.DisableAsync(deptGid, ct) ? NoContent() : Conflict("部门无法删除");
    }

    private bool CanAccess(DepartmentRecord dept) =>
        CurrentUser.PlatformType == PlatformType.Admin || dept.CompanyID == CurrentUser.CompanyID;
}
