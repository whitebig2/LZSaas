using Microsoft.AspNetCore.Mvc;
using LZ.Model.Exceptions;
using SqlSugar;
using LZ.Company.Service.Interface;
using LZ.Core.Attributes;
using LZ.Model.PagedModel;
using LZ.Company.Service.Services;

namespace LZ.Company.API
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    [PlatformLimit(Model.Enums.PlatformType.LZMini)]
    public class CompanyAuthorizeController(ICompanyAuthorizeService companyAuthorizeService,ICompanyService companyService) : CompanyControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] UserPagedModel query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Live_V_CompanyAuthorize>();
            if(query.From=="kq")
            {
                expable.And(it => it.ToCompanyID == user.CompanyID);
            }
            else
            {
                if (user.PlatformType == Model.Enums.PlatformType.MiniAdmin)
                {
                    expable.And(it => it.CompanyID == user.CompanyID);
                }
                else if (user.PlatformType == Model.Enums.PlatformType.LZMini)
                {
                    expable.And(it => it.ToCompanyID == user.CompanyID);
                }
                else
                {
                    if (query.CompanyID.HasValue)
                    {
                        expable.And(it => it.CompanyID == query.CompanyID);
                    }
                }
            }
            var exp = expable.ToExpression();
            var models = await companyAuthorizeService.List(exp, query);
            if (user.PlatformType == Model.Enums.PlatformType.LZMini)
            {
                models = models.DistinctBy(it => it.CompanyID).ToList();
            }
                return Ok(new { items = models, count = query.Count });
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Model.Live_CompanyAuthorize model)
        {
            if(await companyAuthorizeService.Exist(it=>it.CompanyID==model.CompanyID && it.ToCompanyID == model.ToCompanyID && it.CustomerID==model.CustomerID))
            {
                throw new ErrorCodeException(-1, "该授权已存在");
            }
            model.CreateTime = DateTime.Now;
            await companyAuthorizeService.Add(model);
            return Ok();
        }
        [OperationLogAction("授权客户：{customerId}授权给{id}")]
        [HttpPost]
        public async Task<IActionResult> Auth([FromForm] Guid id, [FromForm] int customerId)
        {
            var info = await companyService.Get(id);
            if (info == null)
            {
                throw new ErrorCodeException(-1, "该公司信息不存在");
            }
            var user = CurrentUser;
            if (await companyAuthorizeService.Exist(it => it.CompanyID == user.CompanyID && it.ToCompanyID == info.CompanyID && it.CustomerID == customerId))
            {
                throw new ErrorCodeException(-1, "该授权已存在");
            }
            var model = new Model.Live_CompanyAuthorize();
            model.CreateTime = DateTime.Now;
            model.CompanyID = user.CompanyID;
            model.ToCompanyID = info.CompanyID;
            model.CustomerID = customerId;
            model.Enabled = true;
            await companyAuthorizeService.Add(model);
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> CustomerGet(int cid)
        {
            var model = await companyAuthorizeService.Get(t => t.CustomerID == cid);
            return Ok(model);
        }


        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            if (!await companyAuthorizeService.Delete(id))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            return Ok();
        }
    }
}
