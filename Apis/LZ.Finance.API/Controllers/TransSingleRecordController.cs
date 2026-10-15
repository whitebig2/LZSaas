using LZ.Core;
using LZ.Finance.Service.Interface;
using LZ.Finance.Service.Services;
using LZ.Model.Enums;
using LZ.Model;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Senparc.CO2NET.Extensions;
using SqlSugar;
using SqlSugar.IOC;
using Microsoft.AspNetCore.Authorization;

namespace LZ.Finance.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    public class TransSingleRecordController : ApiControllerBase
    {

        private readonly IFinanceOtherService  _otherService;
        private readonly ISalaryRecordService  _salaryRecordService;
        private readonly ITransSingleRecordService  _singleRecordService;
        private readonly IPrepaidApplyRecordService   _applyRecordService;

        public TransSingleRecordController(ITransSingleRecordService singleRecordService, ISalaryRecordService salaryRecordService, IFinanceOtherService otherService, IPrepaidApplyRecordService applyRecordService)
        {
            _singleRecordService = singleRecordService;
            _salaryRecordService = salaryRecordService;
            _otherService = otherService;
            _applyRecordService = applyRecordService;
        }

        [HttpGet]
        public async Task<IActionResult> PageList(string name,string idCardNo,string customerName,[FromQuery] PagedModelBase query)
        {
            var exp = Expressionable.Create<Model.Live_TransSingleRecord>();
            exp.AndIF(!string.IsNullOrWhiteSpace(name), f => f.Name.Contains(name));
            exp.AndIF(!string.IsNullOrWhiteSpace(idCardNo), f => f.IdCardNo.Contains(idCardNo));
            exp.AndIF(!string.IsNullOrWhiteSpace(customerName), f => f.CustomerName.Contains(customerName));
            var models = await _singleRecordService.PageList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }

        /// <summary>
        ///  人工确认 发放状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isSuc"></param>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        [HttpPut]
        public async Task<IActionResult> PaidStatus([FromForm] Guid id, [FromForm] bool isSuc)
        {
            var model = await _singleRecordService.Get(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            if (model.Status == Model.Enums.CmbcStatusEnumType.确认异常)
            {
                model.Status = Model.Enums.CmbcStatusEnumType.已确认;
                if (isSuc)
                {
                    model.TransStatus = "02";
                    model.TransStatusName = "人工确认交易成功";
                }
                else
                {
                    model.TransStatus = "03";
                    model.TransStatusName = "人工确认交易失败";
                }
            }
            else {

                throw new ErrorCodeException(-1, "此状态无需人工操作");
            }
            await _singleRecordService.Update(model);
            return Ok();
        }




        [HttpDelete]
        public async Task<IActionResult> Get(Guid id)
        {
            await _singleRecordService.Get(f => f.ID == id);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] Guid id)
        {
            await _singleRecordService.Delete(id);
            return Ok();
        }


    }
}
