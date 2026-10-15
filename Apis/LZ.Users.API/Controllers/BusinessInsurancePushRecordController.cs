using LZ.Core.EventData;
using LZ.Core.RabbitMQ.EventBus;
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
using System.Security.Policy;

namespace LZ.Customer.API.Controllers
{

    public class BusinessInsurancePushRecordController : UsersControllerBase
    {

        private readonly IBusinessInsurancePushRecordService _pushRecordService;

        public BusinessInsurancePushRecordController(IBusinessInsurancePushRecordService pushRecordService)
        {
            _pushRecordService = pushRecordService;
        }

        [HttpGet]
        public async Task<IActionResult> PageList(string name, string idCardNo, [FromQuery] PagedModelBase query)
        {
            var expable = Expressionable.Create<Model.Live_BusinessInsurancePushRecord>();

            expable.AndIF(!string.IsNullOrWhiteSpace(name), f => f.Custemername.Contains(name));
            expable.AndIF(!string.IsNullOrWhiteSpace(idCardNo), f => f.Custemernum.Contains(idCardNo));
            var exp = expable.ToExpression();
            var models = await _pushRecordService.PageList(exp, query);
            return Ok(new { items = models, count = query.Count });
        }

       

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get(Guid id)
        {
            var model = await _pushRecordService.Get(f=>f.ID==id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return Ok(model);
        }
    
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] Guid id)
        {
            var model = await _pushRecordService.Get(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            await _pushRecordService.Delete(id);
            return Ok();
        }


    }
}
