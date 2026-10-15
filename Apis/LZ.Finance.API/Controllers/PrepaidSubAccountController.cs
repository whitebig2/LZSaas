using LZ.Common;
using LZ.Core;
using LZ.Finance.Service.Interface;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.IExcelTemplate.profit;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Quartz.Util;
using Senparc.CO2NET.Extensions;
using SqlSugar;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace LZ.Finance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PrepaidSubAccountController : ApiControllerBase
    {

        private readonly IPrepaidSubAccounttService _service;
        private readonly IFinanceOtherService _otherService;

        public PrepaidSubAccountController(IPrepaidSubAccounttService service, IFinanceOtherService otherService)
        {
            _service = service;
            _otherService = otherService;
        }

        [HttpGet]
        public async Task<IActionResult> PageList(string signUnitName,string subAccountNo,PrepaidSubAccountStatusEnumType? status, [FromQuery] PagedModelBase query)
        {
            var exp = Expressionable.Create<Model.Live_PrepaidSubAccount>();
            exp.AndIF(!string.IsNullOrWhiteSpace(signUnitName), f => f.SignUnitName.Contains(signUnitName));
            exp.AndIF(!string.IsNullOrWhiteSpace(subAccountNo), f => f.SubAccountNo.Contains(subAccountNo));
            exp.AndIF(status.HasValue, f => f.Status==status.Value);
            var models = await _service.PageList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }


        [HttpPost]
        public async Task<IActionResult> Add(Model.Live_PrepaidSubAccount model)
        {
            model.CreateTime = DateTime.Now;
            model.LastSendTime = DateTime.Now;
            model.Balance = 0;
            var existModel = await _service.Get(f => f.SubAccountNo == model.SubAccountNo || f.SignUnitName == model.SignUnitName);
            if (existModel != null)
            {
                throw new ErrorCodeException(-1, "预发账号或签章单位已存在，无法重复创建");
            }
            await _service.Add(model);
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var model = await _service.Get(f=>f.ID==id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return Ok(model); 
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_PrepaidSubAccount model)
        {
            var existModel = await _service.Get(f =>f.ID!=model.ID&& (f.SubAccountNo == model.SubAccountNo || f.SignUnitName == model.SignUnitName));
            if (model == null)
            {
                throw new ErrorCodeException(-1, "预发账号或签章单位已存在，无法修改");
            }
            await _service.Update(model);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm]int id)
        {
            await _service.Delete(f=>f.ID==id);
            return Ok();
        }
        
        [HttpGet]
        public async Task<IActionResult> GetTransRecordBalance(int id)
        {
         var  model=   await _service.Get(f => f.ID == id);

            DateTime dateFrom = DateTime.Today.AddDays(-30);
            DateTime dateTo = DateTime.Today.AddDays(1);
            if (!string.IsNullOrWhiteSpace(model.SubAccountNo))
            {
                var resp = await _otherService.EcpSignalTransactionPageList(dateFrom, dateTo, model.SubAccountNo, 1, 1);
                if (resp != null && resp.Result != null)
                {
                    var record = resp.Result.Data.FirstOrDefault();
                    if (record != null) {
                        model.Balance = Convert.ToDecimal(record.balance);
                        if (model.Balance > 5000)
                        {
                            model.Status = PrepaidSubAccountStatusEnumType.余额充足;
                        }
                        await _service.Update(model);

                        return Ok(record);
                    }

                }
                else
                {
                    throw new ErrorCodeException(-1, "余额信息获取失败");
                }
            }
            return Ok();
        }
    }
}
