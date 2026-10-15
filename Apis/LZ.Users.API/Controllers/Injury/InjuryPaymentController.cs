using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Users.Service.Interface;
using LZ.Users.Service.Services;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using LZ.Core;
using LZ.Core.Interface;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using LZ.Core.Attributes;
using LZ.Core.Services;
using LZ.Model.PagedModel;

namespace LZ.Users.API.Controllers
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.LZMini)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    public class InjuryPaymentController(IInjuryPaymentService injuryPaymentService, IAuthService authService) : UsersControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] Model.PagedModel.UserPagedModel query)
        {
            var user = CurrentUser;
            var cid = CurrentUser.CompanyID;
            var expable = Expressionable.Create<Model.Live_V_InjuryPayment>();
            if (user.PlatformType == Model.Enums.PlatformType.Admin || user.PlatformType == Model.Enums.PlatformType.Api)
            {
                expable.AndIF(query.CompanyID.HasValue && query.CompanyID > 0, it => it.CompanyID == query.CompanyID);
            }
            //else
            //{
            //    expable.And(it => it.CompanyID == CurrentUser.CompanyID);
            //}
            if (!string.IsNullOrEmpty(query.UserName))
            {
                expable.And(t => t.UserName == query.UserName);
            }
            if (query.CID.HasValue)
            {
                expable.And(t => t.InjuryID == query.CID);
            }
            if (query.Status.HasValue)
            {
                expable.And(t => t.Status == query.Status);
            }
            if (query.From == "lzmini")
            {
                expable.And(t => t.ProcessStatus != 8 && t.ProcessStatus != 9);
            }
            var exp = expable.ToExpression();
            var models = await injuryPaymentService.List(exp, query,"createTime desc");
            return Ok(new { items = models, count = query.Count });
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Model.Live_InjuryPayment model)
        {
            var user = CurrentUser;
            int cid = 0;
            Guid? cgid = Guid.Parse("00000000-0000-0000-0000-000000000000");
            if (CurrentUser.PlatformType == Model.Enums.PlatformType.LZMini || CurrentUser.PlatformType == Model.Enums.PlatformType.MiniAdmin)
            {
                cid = user.CompanyID;
                cgid = user.CompanyGID;
            }
            model.CompanyID = cid;
            model.CompanyGID = cgid;
            model.Status = 1;
            model.CreateTime = DateTime.Now;
            model.Year = DateTime.Now.Year.ToString();
            await injuryPaymentService.Add(model);
            return Ok();
        }
        [HttpGet]
        public async Task<Model.Live_InjuryPayment> Get(int id)
        {
            return await injuryPaymentService.Get(id);
            //return await userService.Get(CurrentUser.UserID);
        }
        [HttpGet]
        public async Task<Model.Live_InjuryPayment> GetByInjuryID(int id)
        {
            return await injuryPaymentService.GetByInjuryID(id); 
            //return await userService.Get(CurrentUser.UserID);
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_InjuryPayment model)
        {
            if (!await injuryPaymentService.ExistID(model.ID))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            await injuryPaymentService.Update(model);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus([FromForm] int id, [FromForm] int status, [FromForm] string msg)
        {
            if (!await injuryPaymentService.ExistID(id))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            if(status==2)
            {
                var model = await injuryPaymentService.Get(id);
                model.Status = status;
                // model.Remark = msg;
                if(!string.IsNullOrEmpty(msg))
                {
                    model.Message = msg;
                }
                model.PayTime = DateTime.Now;
                await injuryPaymentService.Update(model);
            }
            else
            {
                await injuryPaymentService.UpdateStatus(id, status, msg);
            }
                
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            if (!await injuryPaymentService.ExistID(id))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            await injuryPaymentService.Delete(id);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> ExportExcel([FromQuery] UserPagedModel query)
        {
            var expable = Expressionable.Create<Model.Live_V_InjuryPayment>();
            var user = CurrentUser;
            query.Page = 0;
            
            if (!string.IsNullOrEmpty(query.UserName))
            {
                expable.And(t => t.UserName == query.UserName);
            }
            if (query.CID.HasValue)
            {
                expable.And(t => t.InjuryID == query.CID);
            }
            if (query.Status.HasValue)
            {
                expable.And(t => t.Status == query.Status);
            }
            if (query.From == "lzmini")
            {
                expable.And(t => t.ProcessStatus != 8 && t.ProcessStatus != 9);
            }
            var models = await injuryPaymentService.List(expable.ToExpression(), query, "createTime desc");




            var result = injuryPaymentService.ExportExcel(models);
            string base64String = Convert.ToBase64String(result);
            return Ok(new { base64String, fileName = "工伤支付信息.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }

    }
}
