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

//    public class InsurancePushController(IInsurancePushService insurancePushService,
//        IUserEntryInsuracneRecordService userEntryInsuracneRecordService,
//        IEventPublisher publisher
//        ) : CustomerControllerBase
//    {

//        [HttpGet]
//        public async Task<IActionResult> PageList([FromQuery] PagedModelBase query)
//        {
//            var expable = Expressionable.Create<Model.Live_V_InsurancePush>();
//            var exp = expable.ToExpression();
//            var models = await insurancePushService.PageViewList(exp, query);
//            return Ok(new { items = models, count = query.Count });
//        }

//        [HttpGet]
//        public async Task<IActionResult> List([FromQuery] PagedModelBase query)
//        {
//            var expable = Expressionable.Create<Model.Live_InsurancePush>();
//            var models = await insurancePushService.List(expable.ToExpression());
//            return Ok(new { items = models, count = models.Count });
//        }



//        [HttpGet]
//        [AllowAnonymous]
//        public async Task<IActionResult> GetView(Guid id)
//        {
//            var model = await insurancePushService.GetViewNav(f => f.ID == id);
//            if (model == null)
//            {
//                throw new ErrorCodeException(-1, "你要读取的数据不存在");
//            }
//            return Ok(model);
//        }

//        [HttpGet]
//        [AllowAnonymous]
//        public async Task<IActionResult> Get(Guid id)
//        {
//            var model = await insurancePushService.GetNav(f=>f.ID==id);
//            if (model == null)
//            {
//                throw new ErrorCodeException(-1, "你要读取的数据不存在");
//            }
//            return Ok(model);
//        }
//        //[HttpPut]
//        //public async Task<IActionResult> Update([FromBody] Model.Live_InsurancePush model)
//        //{
            
//        //    await insurancePushService.Update(model);
//        //    return Ok();
//        //}
//        [HttpDelete]
//        public async Task<IActionResult> Delete([FromForm] Guid id)
//        {
//            var model = await insurancePushService.GetNav(f => f.ID == id);
//            if (model == null)
//            {
//                throw new ErrorCodeException(-1, "你要读取的数据不存在");
//            }
//            await insurancePushService.DeleteNav(model);
//            return Ok();
//        }


//        [HttpPut]
//        public async Task<IActionResult> UpdateRecordPushSuccess([FromForm] Guid id,[FromForm]Guid recordID)
//        {
//            var model = await insurancePushService.GetNav(f => f.ID == id);
//            if (model == null)
//            {
//                throw new ErrorCodeException(-1, "你要读取的数据不存在");
//            }

//            var record = model.PushRecords.First(f => f.ID == recordID);
//            record.IsPushSuc = true;
//            record.Remark = "线下手动推送";
//            if (record.IsAdd)
//            {
//                model.AddFailCount = model.PushRecords.Count(f => f.IsAdd && !f.IsPushSuc);
//            }
//            else
//            {
//                model.DelFailCount = model.PushRecords.Count(f => !f.IsAdd && f.IsPushSuc == false);
//            }
//            var insuranceRecord = await userEntryInsuracneRecordService.Get(f => f.ID == record.UserEntryInsuracneRecordID);
//            // 获取
//            var sameBatchNoRecord = await userEntryInsuracneRecordService.List(f => f.BatchNo == insuranceRecord.BatchNo);

//            await insurancePushService.UpdateNav(model);
//            // 之后推送状态
//            UserInsuranceInsuranceStatusUpdateEventData eventData = new UserInsuranceInsuranceStatusUpdateEventData()
//            {
//                UserEntryID = insuranceRecord.UserEntryID.Value,
//                InsuranceStatus = UserEntryInsuranceStatusEnumType.已增员,
//            };
//            if (sameBatchNoRecord.Any(f => f.SendStatus == InsuranceSendStatusEnumType.增员异常))
//            {
//                eventData.InsuranceStatus = UserEntryInsuranceStatusEnumType.增员异常;
//                publisher.Publish(eventData);
//            }
//            else if (sameBatchNoRecord.Any(f => f.SendStatus == InsuranceSendStatusEnumType.减员异常))
//            {
//                // 减员异常
//                eventData.InsuranceStatus = UserEntryInsuranceStatusEnumType.减员异常;
//                publisher.Publish(eventData);
//            }
//            else if (sameBatchNoRecord.Any(f => f.SendStatus == InsuranceSendStatusEnumType.待增员))
//            {
//                //待增员
//                // 减员异常
//                eventData.InsuranceStatus = UserEntryInsuranceStatusEnumType.待增员;
//                publisher.Publish(eventData);

//            }
//            else if (sameBatchNoRecord.Any(f => f.SendStatus == InsuranceSendStatusEnumType.待减员))
//            {

//                eventData.InsuranceStatus = UserEntryInsuranceStatusEnumType.待减员;
//                publisher.Publish(eventData);
//            }
//            else if (sameBatchNoRecord.Any(f => f.SendStatus == InsuranceSendStatusEnumType.已增员))
//            {
//                eventData.InsuranceStatus = UserEntryInsuranceStatusEnumType.已增员;
//                publisher.Publish(eventData);

//            }
//            else if (sameBatchNoRecord.Any(f => f.SendStatus == InsuranceSendStatusEnumType.已减员))
//            {
//                eventData.InsuranceStatus = UserEntryInsuranceStatusEnumType.已减员;
//                publisher.Publish(eventData);
//            }
 
//            return Ok();
//        }
//    }
//}
