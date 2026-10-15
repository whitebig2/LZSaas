//using LZ.Core.Attributes;
//using LZ.Core.EventData.FxqEventData;
//using LZ.Customer.Service.Interface;
//using LZ.Customer.Service.Services;
//using LZ.Model.PagedModel;
//using Microsoft.AspNetCore.Mvc;
//using LZ.Core.EventData.Insurance;
//using LZ.Core.RabbitMQ.EventBus;
//using SqlSugar;
//using System.Security.Policy;
//using LZ.Core.EventData;
//using LZ.Model.Exceptions;
//using LZ.Model.Enums;

//namespace LZ.Customer.API.Controllers
//{
//    [PlatformLimit(Model.Enums.PlatformType.Admin)]
//    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
//    [PlatformLimit(Model.Enums.PlatformType.LZMini)]
//    public class UserEntryInsuracneRecordController(IUserEntryInsuracneRecordService userEntryInsuracneRecordService) : CustomerControllerBase
//    {

//        [HttpGet]
//        public async Task<IActionResult> PageList([FromQuery] PagedModelBase query)
//        {
//            var expable = Expressionable.Create<Model.Live_UserEntryInsuracneRecord>();
//            var exp = expable.ToExpression();
//            var models = await userEntryInsuracneRecordService.PageList(exp, query);
//            return Ok(new { items = models, count = query.Count });
//        }


//        [HttpGet]
//        public async Task<IActionResult> List([FromQuery] UserPagedModel query)
//        {
//            var user = CurrentUser;
//            var expable = Expressionable.Create<Model.Live_UserEntryInsuracneRecord>();
//            if (!string.IsNullOrEmpty(query.CustomerName))
//            {
//                expable.And(it => it.CustomerName==query.CustomerName);
//            }
//            if (!string.IsNullOrEmpty(query.Name))
//            {
//                expable.And(it => it.Name==query.Name);
//            }
//            if (query.Main.HasValue)
//            {
//                expable.And(it => it.IsMain == query.Main);
//            }
//            expable.AndIF(query.Time.HasValue, t => t.InsuranceStartDate < query.Time && t.InsuranceEndDate > query.Time);
//            var exp = expable.ToExpression();
//            var models = await userEntryInsuracneRecordService.List(exp);
//            return Ok(new { items = models, count = query.Count });
//        }

//        [HttpGet]
//        public async Task<IActionResult> Get([FromQuery] UserPagedModel query)
//        {
//            var user = CurrentUser;
//            var expable = Expressionable.Create<Model.Live_UserEntryInsuracneRecord>();
//            if (!string.IsNullOrEmpty(query.CustomerName))
//            {
//                expable.And(it => it.CustomerName == query.CustomerName);
//            }
//            if (!string.IsNullOrEmpty(query.Name))
//            {
//                expable.And(it => it.Name == query.Name);
//            }
//            //if(query.Main.HasValue)
//            //{
//            //    expable.And(it => it.Name == query.Name);
//            //}
//            expable.AndIF(query.Time.HasValue, t => t.InsuranceStartDate < query.Time && t.InsuranceEndDate > query.Time);
//            var exp = expable.ToExpression();
//            var model = await userEntryInsuracneRecordService.Get(exp);
//            return Ok(model);
//        }

//        //[HttpPut]
//        //public async Task<IActionResult> Update([FromBody] Model.Live_Enterprise model)
//        //{
//        //    if (!await userEntryInsuracneRecordService.Exist(model.ID))
//        //    {
//        //        throw new ErrorCodeException(-1, "你要修改的数据不存在");
//        //    }
//        //    await userEntryInsuracneRecordService.Update(model);
//        //    return Ok();
//        //}


//        [HttpDelete]
//        public async Task<IActionResult> Delete([FromForm] Guid id)
//        {
//            await userEntryInsuracneRecordService.Delete(id);
//            return Ok();
//        }


//        [HttpDelete]
//        public async Task<IActionResult> UpdateStatus([FromForm] Guid id, [FromForm] InsuranceSendStatusEnumType status)
//        {
//            var model = await userEntryInsuracneRecordService.Get(f => f.ID == id);
//            if (model == null)
//            {
//                throw new ErrorCodeException(-1, "你要修改的数据不存在");
//            }
//            model.SendStatus = status;
//            await userEntryInsuracneRecordService.Update(model);
//            return Ok();
//        }


//    }
//}
