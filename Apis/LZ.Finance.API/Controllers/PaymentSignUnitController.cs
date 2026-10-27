using LZ.Core;
using LZ.Finance.Service.Interface;
using LZ.Model;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace LZ.Finance.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PaymentSignUnitController : ApiControllerBase
    {
        private readonly IPaymentSignUnitService _service;
        private readonly IPaymentApplicationService _applicationService;

        public PaymentSignUnitController(IPaymentSignUnitService service, IPaymentApplicationService applicationService)
        {
            _service = service;
            _applicationService = applicationService;
        }

        [HttpGet]
        public async Task<IActionResult> PageList(string unitName, string unitNo, bool? isEnable, Guid? paymentApplicationID, [FromQuery] PagedModelBase query)
        {
            var exp = Expressionable.Create<Live_V_PaymentSignUnit>();
            exp.And(f => !f.IsDelete);
            exp.AndIF(!string.IsNullOrWhiteSpace(unitName), f => f.UnitName.Contains(unitName));
            exp.AndIF(!string.IsNullOrWhiteSpace(unitNo), f => f.UnitNo.Contains(unitNo));
            exp.AndIF(isEnable.HasValue, f => f.IsEnable == isEnable.Value);
            exp.AndIF(paymentApplicationID.HasValue && paymentApplicationID.Value != Guid.Empty, f => f.PaymentApplicationID == paymentApplicationID.Value);
            var models = await _service.PageList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }

        [HttpGet]
        public async Task<IActionResult> List(string unitName, string unitNo, Guid? paymentApplicationID, string queryType)
        {
            var exp = Expressionable.Create<Live_PaymentSignUnit>();
            exp.And(f => f.IsEnable);
            exp.AndIF(!string.IsNullOrWhiteSpace(unitName), f => f.UnitName.Contains(unitName));
            exp.AndIF(!string.IsNullOrWhiteSpace(unitNo), f => f.UnitNo.Contains(unitNo));
            exp.AndIF(paymentApplicationID.HasValue && paymentApplicationID.Value != Guid.Empty, f => f.PaymentApplicationID == paymentApplicationID.Value);
            if (string.IsNullOrWhiteSpace(queryType) || !string.Equals(queryType, "all", StringComparison.OrdinalIgnoreCase))
            {
                var appIds = await GetCurrentCompanyApplicationIds();
                if (appIds.Count == 0)
                {
                    return Ok(new List<Live_PaymentSignUnit>());
                }
                exp.And(f => appIds.Contains(f.PaymentApplicationID));
            }
            var models = await _service.List(exp.ToExpression());
            return Ok(models);
        }

        [HttpGet]
        public async Task<IActionResult> Get(Guid id)
        {
            var model = await _service.Get(f => f.ID == id && !f.IsDelete);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return Ok(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(Live_PaymentSignUnit dto)
        {
            var application = await GetRequiredApplication(dto.PaymentApplicationID);

            var model = new Live_PaymentSignUnit
            {
                ID = Guid.NewGuid(),
                UnitName = dto.UnitName?.Trim(),
                UnitNo = dto.UnitNo?.Trim(),
                IsEnable = dto.IsEnable,
                IsDelete = false,
                DeleteTime = null,
                CreateTime = DateTime.Now,
                UpdateTime = null,
                PaymentApplicationID = application.ID,
            };

            await ValidateModel(model, null);
            await _service.Add(model);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update(Live_PaymentSignUnit dto)
        {
            var model = await _service.Get(f => f.ID == dto.ID && !f.IsDelete);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }

            await EnsureExistingApplicationOwned(model.PaymentApplicationID);
            var application = await GetRequiredApplication(dto.PaymentApplicationID);

            model.UnitName = dto.UnitName?.Trim();
            model.UnitNo = dto.UnitNo?.Trim();
            model.IsEnable = dto.IsEnable;
            model.PaymentApplicationID = application.ID;
            model.UpdateTime = DateTime.Now;

            await ValidateModel(model, dto.ID);
            await _service.Update(model);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> SetEnable([FromForm] Guid id)
        {
            var model = await _service.Get(f => f.ID == id && !f.IsDelete);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "数据不存在");
            }
            await EnsureExistingApplicationOwned(model.PaymentApplicationID);
            model.IsEnable = !model.IsEnable;
            model.UpdateTime = DateTime.Now;
            await _service.Update(model);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] Guid id)
        {
            var model = await _service.Get(f => f.ID == id && !f.IsDelete);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "数据不存在");
            }
            await EnsureExistingApplicationOwned(model.PaymentApplicationID);
            model.IsEnable = false;
            model.IsDelete = true;
            model.DeleteTime = DateTime.Now;
            model.UpdateTime = DateTime.Now;
            await _service.Update(model);
            return Ok();
        }

        private async Task<List<Guid>> GetCurrentCompanyApplicationIds()
        {
            var apps = await _applicationService.List(f => f.CompanyID == CurrentUser.CompanyID && !f.IsDelete);
            return apps.Select(f => f.ID).Where(id => id != Guid.Empty).ToList();
        }

        private async Task<Live_PaymentApplication> GetRequiredApplication(Guid paymentApplicationID)
        {
            if (paymentApplicationID == Guid.Empty)
            {
                throw new ErrorCodeException(-1, "支付应用不能为空");
            }
            var application = await _applicationService.Get(f => f.ID == paymentApplicationID && !f.IsDelete);
            if (application == null)
            {
                throw new ErrorCodeException(-1, "支付应用不存在");
            }
            if (!application.IsEnable)
            {
                throw new ErrorCodeException(-1, "支付应用未启用");
            }
            EnsureCurrentUserMatchApplication(application);
            return application;
        }

        private async Task EnsureExistingApplicationOwned(Guid paymentApplicationID)
        {
            if (paymentApplicationID == Guid.Empty)
            {
                throw new ErrorCodeException(-1, "签约单位未绑定支付应用");
            }
            var application = await _applicationService.Get(f => f.ID == paymentApplicationID && !f.IsDelete);
            if (application == null)
            {
                throw new ErrorCodeException(-1, "支付应用不存在");
            }
            EnsureCurrentUserMatchApplication(application);
        }

        private void EnsureCurrentUserMatchApplication(Live_PaymentApplication application)
        {
            var currentCompanyGid = CurrentUser.CompanyGID ?? Guid.Empty;
            if (application.CompanyID != CurrentUser.CompanyID || application.CompanyGID != currentCompanyGid)
            {
                throw new ErrorCodeException(-1, "当前用户公司与支付应用公司不匹配");
            }
        }

        private async Task ValidateModel(Live_PaymentSignUnit model, Guid? excludeId)
        {
            if (string.IsNullOrWhiteSpace(model.UnitName))
            {
                throw new ErrorCodeException(-1, "签约单位名称不能为空");
            }
            if (string.IsNullOrWhiteSpace(model.UnitNo))
            {
                throw new ErrorCodeException(-1, "签约单位编号不能为空");
            }

            var nameExist = excludeId.HasValue
                ? await _service.Exist(f => f.UnitName == model.UnitName && f.PaymentApplicationID == model.PaymentApplicationID && !f.IsDelete && f.ID != excludeId.Value)
                : await _service.Exist(f => f.UnitName == model.UnitName && f.PaymentApplicationID == model.PaymentApplicationID && !f.IsDelete);
            if (nameExist)
            {
                throw new ErrorCodeException(-1, $"签约单位名称：【{model.UnitName}】已存在，无法重复创建！");
            }

            var noExist = excludeId.HasValue
                ? await _service.Exist(f => f.UnitNo == model.UnitNo && f.PaymentApplicationID == model.PaymentApplicationID && !f.IsDelete && f.ID != excludeId.Value)
                : await _service.Exist(f => f.UnitNo == model.UnitNo && f.PaymentApplicationID == model.PaymentApplicationID && !f.IsDelete);
            if (noExist)
            {
                throw new ErrorCodeException(-1, $"签约单位编号：【{model.UnitNo}】已存在，无法重复创建！");
            }
        }
    }
}
