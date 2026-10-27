//using LZ.Core.EventData;
//using LZ.Core.RabbitMQ.EventBus;
//using LZ.Customer.Service.Interface;
//using LZ.Customer.Service.Services;
//using LZ.Model.Enums;
//using LZ.Model.Exceptions;
//using LZ.Model.PagedModel;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Senparc.CO2NET.Extensions;
//using SqlSugar;
//using System.Security.Policy;

//namespace LZ.Customer.API.Controllers
//{


    
//    public class InsurancePushRecordController(IInsurancePushRecordService recordService ) : CustomerControllerBase
//    {

//        [HttpGet]
//        public async Task<IActionResult> PageList([FromQuery] PagedModelBase query)
//        {
//            var expable = Expressionable.Create<Model.Live_V_InsurancePushRecord>();
//            var exp = expable.ToExpression();
//            var models = await recordService.PageList(exp, query);
//            return Ok(new { items = models, count = query.Count });
//        }

//        [HttpGet]
//        public async Task<IActionResult> List([FromQuery] PagedModelBase query)
//        {
//            var expable = Expressionable.Create<Model.Live_InsurancePushRecord>();
//            var models = await recordService.List(expable.ToExpression());
//            return Ok(new { items = models, count = models.Count });
//        }




//        [HttpGet]
//        [AllowAnonymous]
//        public async Task<IActionResult> Get(Guid id)
//        {
//            var model = await recordService.Get(f=>f.ID==id);
//            if (model == null)
//            {
//                throw new ErrorCodeException(-1, "你要读取的数据不存在");
//            }
//            return Ok(model);
//        }
      
//        [HttpDelete]
//        public async Task<IActionResult> Delete([FromForm] Guid id)
//        {
//            var model = await recordService.Get(f => f.ID == id);
//            if (model == null)
//            {
//                throw new ErrorCodeException(-1, "你要读取的数据不存在");
//            }
//            await recordService.Delete(id);
//            return Ok();
//        }
//    }
//}
