using LZ.Company.Service.Interface;
using LZ.Company.Service.Services;
using LZ.Core;
using LZ.Core.Attributes;
using LZ.Core.Redis;
using LZ.Model;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace LZ.Company.API.Controllers
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    public class CompanyFunctionController(ICompanyFunctionService companyFunctionService) : CompanyControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery]BasicPagedModel query)
        {
            var expable = Expressionable.Create<Model.SYS_CompanyFunction>();
            expable.And(it => it.CompanyID == query.CompanyID);
            var exp= expable.ToExpression();
            var models = await companyFunctionService.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] List<Model.SYS_CompanyFunction> models)
        {
            await companyFunctionService.Add(models);
            return Ok();
        }
    }
}
