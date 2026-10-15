using AutoMapper;
using LZ.Core;
using LZ.Finance.Service.Interface;
using LZ.Finance.Service.Services;
using LZ.Model;
using LZ.Model.Dtos.SettlementDtos;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Senparc.CO2NET.Extensions;
using SqlSugar;

namespace LZ.Finance.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    public class SettlementController : ApiControllerBase
    {

        private readonly ISettlementService   _settlementService;
        private readonly IMapper    _mapper;

        public SettlementController(ISettlementService settlementService, IMapper mapper)
        {
            _settlementService = settlementService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> PageList(string customerName,string issuerName, [FromQuery] PagedModelBase query)
        {
            Console.WriteLine(  Guid.Empty);
            var exp = Expressionable.Create<Model.Live_V_Settlement>();
            exp.AndIF(CurrentUser.PlatformType != PlatformType.Admin && CurrentUser.PlatformType != PlatformType.Api, f => f.CompanyID == CurrentUser.CompanyID);

            exp.AndIF(!string.IsNullOrWhiteSpace(customerName), f => f.CustomerName.Contains(customerName));
            exp.AndIF(!string.IsNullOrWhiteSpace(issuerName), f => f.IssuerName.Contains(issuerName));
            var models = await _settlementService.PageList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }
        [HttpPost]
        public async Task<IActionResult> Add(SettlementManageDto modeldto)
        {
            var model = _mapper.Map<Live_Settlement>(modeldto);
            model.ID = Guid.NewGuid();
            model.ApplyDate = DateTime.Today;
            model.CreateTime = DateTime.Now;
            await _settlementService.Add(model);
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> AddList(List<Live_Settlement> list)
        {

            await _settlementService.Add(list);
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> Get(Guid id)
        {
            var model = await _settlementService.Get(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return Ok(model);
        }
        [HttpPut]
        public async Task<IActionResult> JoinUpdate([FromForm] Guid id, [FromForm] string remark, [FromForm] string description, [FromForm] decimal? totalAmount, [FromForm] int? insuranceStatus)
        {
            var model = await _settlementService.Get(id);
            if (!string.IsNullOrEmpty(remark))
            {
                model.Remark = remark;
            }
            if (!string.IsNullOrEmpty(description))
            {
                model.Description = description;
            }
            if (totalAmount.HasValue)
            {
                model.TotalAmount = totalAmount.Value;
            }
            if (insuranceStatus.HasValue)
            {
                model.InsuranceStatus = insuranceStatus;
            }
            await _settlementService.Update(model);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] SettlementManageDto modelDto)
        {
            var model = await _settlementService.Get(modelDto.ID.Value);
            model= _mapper.Map(modelDto, model);
            await _settlementService.Update(model);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] Guid id)
        {
            await _settlementService.Delete(id);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> JoinDelete(Guid id)
        {
            //接口删除
            await _settlementService.Delete(id);
            return Ok();
        }
    }
}
