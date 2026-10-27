using LZ.Core;
using LZ.Core.Attributes;
using LZ.Core.Services;
using LZ.Model.Enums;
using LZ.Model;
using LZ.Model.Exceptions;
using LZ.Users.Service.Interface;
using LZ.Users.Service.Services;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using LZ.Core.Interface;

namespace LZ.Users.API.Controllers
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.LZMini)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    public class InjuryFinancialWriteOffController(IInjuryFinancialWriteOffService injuryFinancialWriteOff,IInjuryInfoService injuryInfoService, IAuthService authService,IInjuryAdvanceAccountService injuryAdvanceAccountService,IInjuryApplyRecordService injuryApplyRecordService) : UsersControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] Model.PagedModel.UserPagedModel query)
        {
            var user = CurrentUser;
            var cid = CurrentUser.CompanyID;
            var expable = Expressionable.Create<Model.Live_V_InjuryFinancialWriteOff>();
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
                expable.And(t => t.PartnerName == query.UserName);
            }
            if(query.Status.HasValue)
            {
                expable.And(t => t.Status == query.Status);
            }
            if (query.From == "lzmini")
            {
                expable.And(t => t.Status!=3);
            }
            await SetPersonnelOperateAuth(expable);
            var exp = expable.ToExpression();
            var models = await injuryFinancialWriteOff.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }
        /// <summary>
        /// 设置招聘专员的显示权限(只展示有操作权限的人员)
        /// </summary>
        /// <param name="expable"></param>
        /// <returns></returns>
        private async Task SetPersonnelOperateAuth(Expressionable<Live_V_InjuryFinancialWriteOff> expable)
        {
            var cuser = CurrentUser;
            if (cuser.PlatformType == PlatformType.LZMini)
            {
                var user = await authService.UserInfo<Model.Live_Personnel>();
                if (!user.UserType.HasValue)
                {
                    user.UserType = (int)Model.Enums.PersonnelType.Personnel;
                }
                var type = (Model.Enums.PersonnelType)user.UserType;
                switch (type)
                {
                    case PersonnelType.Personnel:
                    case PersonnelType.View:
                        {
                            expable.And(it => SqlFunc.Subqueryable<Model.Live_PersonnelItems>().Where(s => s.CID == it.CustomerID && s.UserID == user.UserID).Any());
                        }
                        break;
                    case PersonnelType.Platform:
                    case PersonnelType.Factory:
                        {
                            expable.And(it => SqlFunc.Subqueryable<Model.Live_V_PersonnelItems>().Where(s => s.CID == it.CustomerID && s.UserID == user.UserID && s.OperateModel == (int)OperateModel.Routine).Any() || it.FactoryUserID == user.UserID);
                        }
                        break;
                    case PersonnelType.Headed:
                        {
                            expable.And(it => it.CustomerPartnerID == user.PartnerID || it.FactoryUserID == user.UserID);
                        }
                        break;
                    case PersonnelType.Manage:
                        break;
                    default:
                        expable.And(it => it.PerUserID == user.UserID);
                        break;
                }
            }
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Model.Live_InjuryFinancialWriteOff model)
        {
            var user = CurrentUser;
            int cid = 0;
            Guid? cgid = Guid.Parse("00000000-0000-0000-0000-000000000000");
            if (CurrentUser.PlatformType == Model.Enums.PlatformType.LZMini)
            {
                cid = user.CompanyID;
                cgid = user.CompanyGID;
            }
            var expable = Expressionable.Create<Model.Live_InjuryFinancialWriteOff>();
            expable.And(t => t.CompanyID == cid);
            expable.And(t => t.InjuryID == model.InjuryID);
            if (await injuryFinancialWriteOff.Exist(expable.ToExpression()))
            {
                throw new ErrorCodeException(-1, "该标签已存在!");
            }
            model.CompanyID = cid;
            model.CompanyGID = cgid;
            model.Status = 1;
            model.CreateTime = DateTime.Now;
            await injuryFinancialWriteOff.Add(model);
            return Ok();
        }
        [HttpGet]
        public async Task<Model.Live_InjuryFinancialWriteOff> Get(int id)
        {
            return await injuryFinancialWriteOff.Get(id);
            //return await userService.Get(CurrentUser.UserID);
        }

        [HttpGet]
        public async Task<Model.Live_V_InjuryFinancialWriteOff> View(int id)
        {
            return await injuryFinancialWriteOff.View(id);
            //return await userService.Get(CurrentUser.UserID);
        }
        [HttpGet]
        public async Task<Model.Live_InjuryFinancialWriteOff> GetByInjuryID(int id)
        {
            return await injuryFinancialWriteOff.GetByInjuryID(id);
            //return await userService.Get(CurrentUser.UserID);
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_InjuryFinancialWriteOff model)
        {
            if (!await injuryFinancialWriteOff.ExistID(model.ID))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            await injuryFinancialWriteOff.Update(model);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Auditing([FromForm] int id, [FromForm] int status, [FromForm] string msg)
        {
            if (!await injuryFinancialWriteOff.ExistID(id))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            var model = await injuryFinancialWriteOff.Get(id);
            var info = await injuryInfoService.Get((int)model.InjuryID);
           
            //审核通过
            if (status == 3 && model.Status==2)
            {
                var account = await injuryAdvanceAccountService.Get(t => t.PartnerID == info.PartnerID);
                if (account == null)
                {
                    throw new ErrorCodeException(-1, "该服务商未创建可垫付账号");
                }
                var m = model.Money + info.ReturnBackMoneyTotalAmount;
                if (info.LoanTotalAmount > m)
                {
                    throw new ErrorCodeException(-1, "还款金额加发票金额不足");
                }
                info.RepaymentTotalAmount = model.Money;
                var loans = await injuryApplyRecordService.List(t => t.InjuryID == info.ID && t.AdvanceAccountID == account.ID, new Model.PagedModel.UserPagedModel() { Page = 0 });
                var money = loans.Sum(t => t.Money);// 借款成功后借款单中保有工伤垫付id，方便判断还款时候是否要回滚金额，老数据不用回滚
                if (money > 0)
                {
                    account.AvailableAmount = account.AvailableAmount + money;
                    account.UsedAmount = account.UsedAmount - money;
                    await injuryAdvanceAccountService.Update(account);
                }
                await injuryInfoService.Update(info);
            }
            model.Status = status;
            await injuryFinancialWriteOff.Update(model);
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> UpdateStatus([FromForm] int id, [FromForm] int status, [FromForm] string msg)
        {
            if (!await injuryFinancialWriteOff.ExistID(id))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            await injuryFinancialWriteOff.UpdateStatus(id, status, msg);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            if (!await injuryFinancialWriteOff.ExistID(id))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            await injuryFinancialWriteOff.Delete(id);
            return Ok();
        }
    }
}
