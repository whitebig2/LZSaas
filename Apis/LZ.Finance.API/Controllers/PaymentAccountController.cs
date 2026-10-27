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
    public class PaymentAccountController : ApiControllerBase
    {
        private readonly IPaymentAccountService _service;
        private readonly IPaymentSignUnitService _signUnitService;
        private readonly IPaymentApplicationService _applicationService;

        public PaymentAccountController(
            IPaymentAccountService service,
            IPaymentSignUnitService signUnitService,
            IPaymentApplicationService applicationService)
        {
            _service = service;
            _signUnitService = signUnitService;
            _applicationService = applicationService;
        }

        [HttpGet]
        public async Task<IActionResult> PageList(string accountName, string accountNo, Guid? paymentSignUnitID, Guid? paymentApplicationID, bool? isEnable, [FromQuery] PagedModelBase query)
        {
            var exp = Expressionable.Create<Live_V_PaymentAccount>();
            exp.And(f => !f.IsDelete);
            exp.AndIF(!string.IsNullOrWhiteSpace(accountName), f => f.AccountName.Contains(accountName));
            exp.AndIF(!string.IsNullOrWhiteSpace(accountNo), f => f.AccountNo.Contains(accountNo));
            exp.AndIF(paymentSignUnitID.HasValue && paymentSignUnitID.Value != Guid.Empty, f => f.PaymentSignUnitID == paymentSignUnitID.Value);
            exp.AndIF(paymentApplicationID.HasValue && paymentApplicationID.Value != Guid.Empty, f => f.PaymentApplicationID == paymentApplicationID.Value);
            exp.AndIF(isEnable.HasValue, f => f.IsEnable == isEnable.Value);
            var models = await _service.PageList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }

        [HttpGet]
        public async Task<IActionResult> List(string accountName, Guid? paymentSignUnitID, Guid? paymentApplicationID, string unitName, string AppName, string queryType)
        {
            var exp = Expressionable.Create<Live_V_PaymentAccount>();
            exp.And(f => f.IsEnable);
            exp.AndIF(!string.IsNullOrWhiteSpace(accountName), f => f.AccountName.Contains(accountName));
            exp.AndIF(!string.IsNullOrWhiteSpace(unitName), f => f.UnitName.Contains(unitName));
            exp.AndIF(!string.IsNullOrWhiteSpace(AppName), f => f.AppName.Contains(AppName));
            exp.AndIF(paymentSignUnitID.HasValue && paymentSignUnitID.Value != Guid.Empty, f => f.PaymentSignUnitID == paymentSignUnitID.Value);
            exp.AndIF(paymentApplicationID.HasValue && paymentApplicationID.Value != Guid.Empty, f => f.PaymentApplicationID == paymentApplicationID.Value);
            if (string.IsNullOrWhiteSpace(queryType) || !string.Equals(queryType, "all", StringComparison.OrdinalIgnoreCase))
            {
                exp.And(f => f.CompanyID == CurrentUser.CompanyID);
            }
            var models = await _service.ListView(exp.ToExpression());
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
        public async Task<IActionResult> Add(Live_PaymentAccount dto)
        {
            var application = await EnsureRelationValid(dto.PaymentSignUnitID, dto.PaymentApplicationID);

            var model = new Live_PaymentAccount
            {
                ID = Guid.NewGuid(),
                PaymentSignUnitID = dto.PaymentSignUnitID,
                PaymentApplicationID = application.ID,
                AccountName = dto.AccountName?.Trim(),
                AccountNo = dto.AccountNo?.Trim(),
                IsEnable = dto.IsEnable,
                IsDelete = false,
                DeleteTime = null,
                UseOrgName = dto.UseOrgName?.Trim(),
                UseOrgCode = dto.UseOrgCode?.Trim(),
                UseOrgKey = dto.UseOrgKey?.Trim(),
                BusinessTypeName = dto.BusinessTypeName?.Trim(),
                CreateTime = DateTime.Now,
                UpdateTime = null
            };

            await ValidateModel(model, null);
            await _service.Add(model);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update(Live_PaymentAccount dto)
        {
            var model = await _service.Get(f => f.ID == dto.ID && !f.IsDelete);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }

            await EnsureExistingApplicationOwned(model.PaymentApplicationID);
            var application = await EnsureRelationValid(dto.PaymentSignUnitID, dto.PaymentApplicationID);

            model.PaymentSignUnitID = dto.PaymentSignUnitID;
            model.PaymentApplicationID = application.ID;
            model.AccountName = dto.AccountName?.Trim();
            model.AccountNo = dto.AccountNo?.Trim();
            model.IsEnable = dto.IsEnable;
            model.UseOrgName = dto.UseOrgName?.Trim();
            model.UseOrgCode = dto.UseOrgCode?.Trim();
            model.UseOrgKey = dto.UseOrgKey?.Trim();
            model.BusinessTypeName = dto.BusinessTypeName?.Trim();
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

        private async Task<Live_PaymentApplication> EnsureRelationValid(Guid paymentSignUnitID, Guid paymentApplicationID)
        {
            if (paymentSignUnitID == Guid.Empty)
            {
                throw new ErrorCodeException(-1, "签约单位不能为空");
            }
            if (paymentApplicationID == Guid.Empty)
            {
                throw new ErrorCodeException(-1, "支付应用不能为空");
            }

            var signUnit = await _signUnitService.Get(f => f.ID == paymentSignUnitID && !f.IsDelete);
            if (signUnit == null)
            {
                throw new ErrorCodeException(-1, "签约单位不存在");
            }
            if (!signUnit.IsEnable)
            {
                throw new ErrorCodeException(-1, "签约单位未启用");
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
            if (signUnit.PaymentApplicationID == Guid.Empty || signUnit.PaymentApplicationID != application.ID)
            {
                throw new ErrorCodeException(-1, "签约单位与支付应用不匹配");
            }
            EnsureCurrentUserMatchApplication(application);
            return application;
        }

        private async Task EnsureExistingApplicationOwned(Guid paymentApplicationID)
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

        private async Task ValidateModel(Live_PaymentAccount model, Guid? excludeId)
        {
            if (string.IsNullOrWhiteSpace(model.AccountName))
            {
                throw new ErrorCodeException(-1, "账户名称不能为空");
            }
            if (string.IsNullOrWhiteSpace(model.AccountNo))
            {
                throw new ErrorCodeException(-1, "账户号不能为空");
            }
            if (string.IsNullOrWhiteSpace(model.BusinessTypeName))
            {
                throw new ErrorCodeException(-1, "业务类型名称不能为空");
            }

            var noExist = excludeId.HasValue
                ? await _service.Exist(f => f.AccountNo == model.AccountNo && f.PaymentApplicationID == model.PaymentApplicationID && !f.IsDelete && f.ID != excludeId.Value)
                : await _service.Exist(f => f.AccountNo == model.AccountNo && f.PaymentApplicationID == model.PaymentApplicationID && !f.IsDelete);
            if (noExist)
            {
                throw new ErrorCodeException(-1, $"账户号：【{model.AccountNo}】已存在，无法重复创建！");
            }
        }
    }
}
