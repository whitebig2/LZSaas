using LZ.Basic.Service.Interface;
using LZ.Core.Attributes;
using LZ.Model;
using LZ.Model.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LZ.Basic.API.Controllers;

[PlatformLimit(PlatformType.Admin)]
[PlatformLimit(PlatformType.MiniAdmin)]
public class DepartmentController(IDepartmentService departmentService) : BasicControllerBase
{
    [HttpGet]
    //[AllowAnonymous]
    public async Task<IActionResult> List([FromQuery] int? companyId, CancellationToken ct)
    {
        var id = CurrentUser.PlatformType == PlatformType.Admin ? companyId : CurrentUser.CompanyID;
        if (id is not > 0)
            return BadRequest("公司ID不能为空");
        var deptData = await departmentService.GetDeptTreeAsync(id.Value, ct);
        return ToCamelCase(Ok(deptData));
    }

    [HttpGet("{deptGid:guid}")]
    //[AllowAnonymous]
    public async Task<IActionResult> Get(Guid deptGid, CancellationToken ct)
    {
        var dept = await departmentService.GetAsync(deptGid, ct);
        return dept == null || !CanAccess(dept) ? NotFound() : ToCamelCase(Ok(dept));
    }

    [HttpPost]
    //[AllowAnonymous]
    public async Task<IActionResult> Add([FromBody] Live_Partner dept, CancellationToken ct)
    {
        if (CurrentUser.PlatformType != PlatformType.Admin)
        {
            dept.CompanyID = CurrentUser.CompanyID;
            dept.CompanyGID = CurrentUser.CompanyGID;
        }
        else if (dept.CompanyID is not > 0)
            return BadRequest("公司ID不能为空");
        dept.PartnerGID = Guid.NewGuid();
        await departmentService.AddAsync(dept, ct);
        var result = CreatedAtAction(nameof(Get), new { deptGid = dept.PartnerGID.Value }, dept);
        result.StatusCode = (int)HttpStatusCode.OK;
        return result;
    }

    [HttpPut("{deptGid:guid}")]
    public async Task<IActionResult> Update(Guid deptGid, [FromBody] Live_Partner dept, CancellationToken ct)
    {
        var existing = await departmentService.GetAsync(deptGid, ct);
        if (existing == null || !CanAccess(existing))
            return NotFound();
        dept.PartnerGID = deptGid;
        return await departmentService.UpdateAsync(dept, ct) ? Ok() : NotFound();
    }

    [HttpDelete("{deptGid:guid}")]
    public async Task<IActionResult> Delete(Guid deptGid, CancellationToken ct)
    {
        var existing = await departmentService.GetAsync(deptGid, ct);
        if (existing == null || !CanAccess(existing))
            return NotFound();
        if (!await departmentService.CanDeleteAsync(deptGid, ct))
            return Conflict("部门下存在子部门或关联预算记录，不能删除");
        return await departmentService.DeleteAsync(deptGid, ct) ? Ok() : Conflict("部门无法删除");
    }

    private bool CanAccess(Live_Partner dept) =>
        CurrentUser.PlatformType == PlatformType.Admin || dept.CompanyID == CurrentUser.CompanyID;
}
