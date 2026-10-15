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
    public class InjuryRepayController(IInjuryRepayService injuryRepayService, IInjuryInfoService injuryInfoService,IFileUploadService fileUploadService,IAuthService authService) : UsersControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] Model.PagedModel.UserPagedModel query)
        {
            var user = CurrentUser;
            var cid = CurrentUser.CompanyID;
            var expable = Expressionable.Create<Model.Live_V_InjuryRepay>();
            if (user.PlatformType == Model.Enums.PlatformType.Admin || user.PlatformType == Model.Enums.PlatformType.Api)
            {
                expable.AndIF(query.CompanyID.HasValue && query.CompanyID > 0, it => it.CompanyID == query.CompanyID);
            }
            if (query.CID.HasValue)
            {
                expable.And(t => t.InjuryID == query.CID);
            }
            if (query.IsUser.HasValue)
            {
                expable.And(t => t.UserEntryInjuryStatus == 1);
            }
            if (query.From == "lzmini")
            {
                expable.And(t => t.ProcessStatus != 8 && t.ProcessStatus != 9);
            }

            
            var exp = expable.ToExpression();
            var models = await injuryRepayService.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromForm] int id)
        {
            var user = CurrentUser;
            int cid = 0;
            Guid? cgid = Guid.Parse("00000000-0000-0000-0000-000000000000");
            if (CurrentUser.PlatformType == Model.Enums.PlatformType.LZMini)
            {
                cid = user.CompanyID;
                cgid = user.CompanyGID;
            }
            var expable = Expressionable.Create<Model.Live_InjuryRepay>();
            //首次还款时候，需要统计所有上传发票的金额作为销账金额
            var info = await injuryInfoService.Get(id);
            if (info.ReturnBackMoneyTotalAmount == 0)
            {
                Model.PagedModel.UserPagedModel query = new Model.PagedModel.UserPagedModel();
                query.Page = 0;
                var fileExpable = Expressionable.Create<Model.Live_FileUpload>();
                fileExpable.And(t => t.CompanyID == cid && t.TypeKey == 6 && t.FID == id);
                var files = await fileUploadService.List(fileExpable.ToExpression(), query);
                foreach(var f in files)
                {
                    var resObj = JsonConvert.DeserializeObject(f.FileContentJSON) as JObject;
                    if(resObj!=null)
                    {
                        decimal money = resObj["money"]!=null? resObj["money"].ToDecimal():0;
                        info.ReturnBackMoneyTotalAmount += money;
                    }
                    
                }
                if (info.ReturnBackMoneyTotalAmount > 0)
                {
                    //await injuryInfoService.Update(info);
                }
            }
            
            var repay = await injuryRepayService.View(0, "1,2,4", cid);
            if (repay!=null)
            {
                throw new ErrorCodeException(-1, "该员工已创建还款单,审核通过后才可继续创建");
            }
            Model.Live_InjuryRepay model = new Model.Live_InjuryRepay();
            model.CompanyID = cid;
            model.CompanyGID = cgid;
            model.Status = 1;
            model.InjuryID = id;
            model.UserID = user.UserID;
            model.LoanTotalAmount = info.LoanTotalAmount;
            model.SpendTotalAmount = info.ReturnBackMoneyTotalAmount;
            model.RepaymentTotalAmount = info.RepaymentTotalAmount;
            model.CreateTime = DateTime.Now;
            model.Bank = info.SealCompanyBank;
            model.BankAccount = info.SealCompanyBankNo;
            long rid = await injuryRepayService.Add(model);
            

            return Ok(new {rid});
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus([FromForm] int id, [FromForm] int status, [FromForm] string msg)
        {
            if (!await injuryRepayService.ExistID(id))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            await injuryRepayService.UpdateStatus(id, status, msg);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Auditing([FromForm] int id, [FromForm] int status, [FromForm] string msg)
        {
            if (!await injuryRepayService.ExistID(id))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            var loan = await injuryRepayService.View(id,"");
            //审核通过
            if (status == 2)
            {
                //生成借款单
            }
            await injuryRepayService.UpdateStatus(id, status, msg);
            return Ok();
        }

        [HttpGet]
        public async Task<Model.Live_InjuryRepay> Get(int id)
        {
            return await injuryRepayService.Get(id);
            //return await userService.Get(CurrentUser.UserID);
        }
        [HttpGet]
        public async Task<Model.Live_V_InjuryRepay> View(int id)
        {
            return await injuryRepayService.View(id,"");
            //return await userService.Get(CurrentUser.UserID);
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_InjuryRepay model)
        {
            if (!await injuryRepayService.ExistID(model.ID))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            await injuryRepayService.Update(model);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            if (!await injuryRepayService.ExistID(id))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            await injuryRepayService.Delete(id);
            return Ok();
        }

        

    }
}
