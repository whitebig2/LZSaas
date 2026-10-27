using LZ.Basic.Service.Interface;
using LZ.Basic.Service.Services;
using LZ.Core;
using LZ.Core.Attributes;
using LZ.Core.Interface;
using LZ.Core.Services;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace LZ.Basic.API.Controllers
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.LZMini)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    public class InsuranceSupplierController(IInsuranceSupplierService insuranceSupplierService, IAuthService authService) : BasicControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] BasicPagedModel query)
        {
            var user = CurrentUser;
            var com = await authService.UserInfo<Model.SYS_CompanyUser>();
            var expable = Expressionable.Create<Model.Live_V_InsuranceSupplier>();
            if (user.PlatformType == Model.Enums.PlatformType.Admin || user.PlatformType == Model.Enums.PlatformType.Api)
            {
                expable.AndIF(query.CompanyID.HasValue && query.CompanyID > 0, it => it.CompanyID == query.CompanyID);
            }
            if (user.CompanyType == CompanyType.Party.ToString().ToLower())
            {
                expable.And(it => it.CompanyID == user.CompanyID || SqlFunc.Subqueryable<Model.Live_CompanyAuthorize>().Where(s => s.CustomerID == it.EmployeeCompanyID && s.ToCompanyID == user.CompanyID && s.Enabled).Any());
            }
            else
            {
                expable.And(it => it.CompanyID == user.CompanyID);
            }
            if (!string.IsNullOrEmpty(query.Name))
            {
                expable.And(t => t.Name.Contains(query.Name));
            }
            if (query.Enable.HasValue)
            {
                expable.And(t => t.Enable == query.Enable);
            }
            if (query.IsCombine.HasValue)
            {
                expable.And(t => t.IsCombine == query.IsCombine);
            }
            if(query.CID.HasValue)
            {
                expable.And(t => t.ParentId == query.CID);
            }
            var exp = expable.ToExpression();
            var models = await insuranceSupplierService.List(exp, query, "CompanyID desc,ID desc");
            return Ok(new { items = models, count = query.Count, com });
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Model.Live_InsuranceSupplier model)
        {
            var user = CurrentUser;
            Guid? cgid = Guid.Parse("00000000-0000-0000-0000-000000000000");
            int cid = 0;
            var expable = Expressionable.Create<Model.Live_InsuranceSupplier>();
            if (user.PlatformType == Model.Enums.PlatformType.LZMini || user.PlatformType == Model.Enums.PlatformType.MiniAdmin)
            {
                cgid = user.CompanyGID;
                cid = user.CompanyID;
            }
            expable.And(t => t.CompanyID == cid);
            expable.And(t => t.Name == model.Name);
            if (await insuranceSupplierService.Exist(expable.ToExpression()))
            {
                throw new ErrorCodeException(-1, "该标签已存在!");
            }
            model.CompanyID = cid;
            model.CompanyGID = cgid;
            model.SupplierGID = Guid.NewGuid();
            model.CreateTime = DateTime.Now;
            await insuranceSupplierService.Add(model);
            return Ok();
        }
        [HttpGet]
        public async Task<Model.Live_InsuranceSupplier> Get(int id)
        {
            return await insuranceSupplierService.Get(id);
            //return await userService.Get(CurrentUser.UserID);
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_InsuranceSupplier model)
        {
            if (!await insuranceSupplierService.ExistID(model.ID))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            if(!model.Enable && model.IsCombine)
            {
                var expable = Expressionable.Create<Model.Live_InsuranceSupplier>();
                expable.And(t => t.ParentId == model.ID);
                var exp = expable.ToExpression();
                int count = await insuranceSupplierService.Count(exp);
                if(count > 0)
                {
                    return Ok(new {code=1,msg="有服务商联合,不能关闭"});
                }
            }
            await insuranceSupplierService.Update(model);
            return Ok(new { code = 0, msg = "更新成功" });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            if (!await insuranceSupplierService.ExistID(id))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            await insuranceSupplierService.Delete(id);
            return Ok();
        }
    }
}
