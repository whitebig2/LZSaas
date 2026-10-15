using LZ.Core.EventData;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using LZ.Users.API;
using LZ.Users.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Quartz.Util;
using Senparc.CO2NET.Extensions;
using SqlSugar;
using SqlSugar.IOC;
using System.Security.Policy;

namespace LZ.Customer.API.Controllers
{

    public class BusinessInsurancePushController : UsersControllerBase
    {

        private readonly IBusinessInsuranceRecordService  _recordService;
        private readonly IBusinessInsurancePushService _pushService;

        public BusinessInsurancePushController(IBusinessInsurancePushService pushService, IBusinessInsuranceRecordService recordService)
        {
            _pushService = pushService;
            _recordService = recordService;
        }

        [HttpGet]
        public async Task<IActionResult> PageList(string insuranceCompany,string insuranceId,string planName,[FromQuery] PagedModelBase query)
        {
            var expable = Expressionable.Create<Model.Live_V_BusinessInsurancePush>();

            expable.AndIF(!string.IsNullOrWhiteSpace(insuranceCompany), f => f.InsuranceCompany.Contains(insuranceCompany));
            expable.AndIF(!string.IsNullOrWhiteSpace(insuranceId), f => f.InsuranceId.Contains(insuranceId));
            expable.AndIF(!string.IsNullOrWhiteSpace(planName), f => f.PlanName.Contains(planName));

             var exp = expable.ToExpression();
            var models = await _pushService.PageList(exp, query);
            return Ok(new { items = models, count = query.Count });
        }

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] PagedModelBase query)
        {
            var expable = Expressionable.Create<Model.Live_BusinessInsurancePush>();
            var models = await _pushService.List(expable.ToExpression());
            return Ok(new { items = models, count = models.Count });
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get(Guid id)
        {
            var model = await _pushService.GetNav(f=>f.ID==id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return Ok(model);
        }
        //[HttpPut]
        //public async Task<IActionResult> Update([FromBody] Model.Live_BusinessInsurancePush model)
        //{
            
        //    await _pushService.Update(model);
        //    return Ok();
        //}
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] Guid id)
        {
            var model = await _pushService.GetNav(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            if (model.AddPushStatus== InsurancePushStatusEnumType.已推送|| model.DelPushStatus== InsurancePushStatusEnumType.已推送)
            {
                throw new ErrorCodeException(-1, "已推送记录禁止删除");
            }
            var records=await  _recordService.List(f => f.BusinessInsurancePushID == model.ID);
            try
            {
                DbScoped.SugarScope.BeginTran();
                if (records.Any())
                {
                    foreach (var item in records)
                    {
                        if (item.Status == BusinessInsuranceStatusEnumType.待增员)
                        {
                            item.Status = BusinessInsuranceStatusEnumType.待增员匹配;
                            item.AddMatchRecordID = null;
                        }
                        else if (item.Status == BusinessInsuranceStatusEnumType.待减员)
                        {
                            item.Status = BusinessInsuranceStatusEnumType.待减员匹配;
                            item.DelMatchRecordID = null;
                        }
                    }
                    await _recordService.Update(records);
                }
                await _pushService.DeleteNav(model);
                DbScoped.SugarScope.CommitTran();
            }
            catch (Exception ex)
            {
                DbScoped.SugarScope.RollbackTran();
                throw ex;
            }
            return Ok();
        }


    }
}
