using LZ.ApiService.Interface;
using LZ.Invoice.Service.Interface;
using LZ.Model;
using LZ.Model.BaiWangModel;
using LZ.Model.Enums.Invoice;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace LZ.Invoice.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PartnerAuthCodeController : InvoiceControllerBase
    {
        private readonly IPartnerAuthCodeService _partnerAuthCodeService;
        private readonly IBaiWangService _baiWangService;

        public PartnerAuthCodeController(IPartnerAuthCodeService partnerAuthCodeService, IBaiWangService baiWangService)
        {
            _partnerAuthCodeService = partnerAuthCodeService;
            _baiWangService = baiWangService;
        }

        [HttpGet]
        public async Task<IActionResult> List(string taxNo, string appKey, string orgAuthCode, int? opeType, [FromQuery] PagedModelBase query)
        {
            var exp = Expressionable.Create<FIN_PartnerAuthCode>();
            exp.AndIF(!string.IsNullOrWhiteSpace(taxNo), f => f.TaxNo.Contains(taxNo));
            exp.AndIF(!string.IsNullOrWhiteSpace(appKey), f => f.AppKey.Contains(appKey));
            exp.AndIF(!string.IsNullOrWhiteSpace(orgAuthCode), f => f.OrgAuthCode.Contains(orgAuthCode));
            exp.AndIF(opeType.HasValue, f => f.OpeType == opeType);
            var models = await _partnerAuthCodeService.List(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Add(FIN_PartnerAuthCode model)
        {
            if (await _partnerAuthCodeService.Exist(t => t.TaxNo == model.TaxNo))
            {
                throw new ErrorCodeException(-1, "该税号已有记录，如果需要继续操作，请先删除之前的记录");
            }
            model.Status = (int)PartnerAuthCodeStatusEnum.待审核;
            model.CreateTime = DateTime.Now;
            await _partnerAuthCodeService.Add(model);
            List<long> relTypes = new List<long>();
            relTypes.Add(7);
            List<long> authTypes = new List<long>();
            authTypes.Add(2);
            //BaiWangOpenOrgRelApiRequest request = new BaiWangOpenOrgRelApiRequest()
            //{
            //    taxCode = taxNo,
            //    toTaxCode = toTaxCode,
            //    relTypes = relTypes,
            //    authTypes = authTypes,
            //    creater = creater,
            //    sourceFlag = 5,
            //    relUrlType = 2,
            //};
            BaiWangOpenOrgRelApiRequest request = new BaiWangOpenOrgRelApiRequest()
            {
                taxCode = "338888888888SMB",
                toTaxCode = model.TaxNo,
                relTypes = relTypes,
                authTypes = authTypes,
                creater = model.Creater,
                sourceFlag = 5,
                relUrlType = 2,
            };
            var result = await _baiWangService.OpenOrgRelApiAsync(request);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> AddList(List<FIN_PartnerAuthCode> models)
        {
            await _partnerAuthCodeService.Add(models);
            return Ok();
        }

        [HttpGet]
        public async Task<FIN_PartnerAuthCode> Get(int id)
        {
            var model = await _partnerAuthCodeService.Get(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] FIN_PartnerAuthCode model)
        {
            await _partnerAuthCodeService.Update(model);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            await _partnerAuthCodeService.Delete(id);
            return Ok();
        }
    }
}
