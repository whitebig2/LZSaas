using LZ.Core;
using LZ.Core.Interface;
using LZ.Finance.Service.Interface;
using LZ.Model;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Mvc;
using QRCoder;
using SqlSugar;

namespace LZ.Finance.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PaymentAdminController : ApiControllerBase
    {
        private readonly IPaymentAdminService _service;
        private readonly ITotpService _totpService;
        private readonly IPaymentApplicationService _applicationService;

        public PaymentAdminController(IPaymentAdminService service, ITotpService totpService, IPaymentApplicationService applicationService)
        {
            _service = service;
            _totpService = totpService;
            _applicationService = applicationService;
        }

        [HttpGet]
        public async Task<IActionResult> PageList(int? userId, bool? isEnable, bool? isValidate, bool? isLock, Guid? paymentApplicationID, [FromQuery] PagedModelBase query)
        {
            var exp = Expressionable.Create<Live_V_PaymentAdmin>();
            exp.And(f => !f.IsDelete);
            exp.AndIF(userId.HasValue && userId.Value > 0, f => f.UserID == userId.Value);
            exp.AndIF(isEnable.HasValue, f => f.IsEnable == isEnable.Value);
            exp.AndIF(isValidate.HasValue, f => f.IsValidate == isValidate.Value);
            exp.AndIF(isLock.HasValue, f => f.IsLock == isLock.Value);
            exp.AndIF(paymentApplicationID.HasValue && paymentApplicationID.Value != Guid.Empty, f => f.PaymentApplicationID == paymentApplicationID.Value);
            var models = await _service.PageList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }

        [HttpGet]
        public async Task<IActionResult> List(int? userId, Guid? paymentApplicationID, string queryType)
        {
            var exp = Expressionable.Create<Live_PaymentAdmin>();
            exp.And(f => !f.IsDelete);
            exp.And(f => f.IsEnable);
            exp.AndIF(userId.HasValue && userId.Value > 0, f => f.UserID == userId.Value);
            exp.AndIF(paymentApplicationID.HasValue && paymentApplicationID.Value != Guid.Empty, f => f.PaymentApplicationID == paymentApplicationID.Value);
            if (string.IsNullOrWhiteSpace(queryType) || !string.Equals(queryType, "all", StringComparison.OrdinalIgnoreCase))
            {
                var appIds = await GetCurrentCompanyApplicationIds();
                if (appIds.Count == 0)
                {
                    return Ok(new List<Live_PaymentAdmin>());
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
        public async Task<IActionResult> Add(Live_PaymentAdmin dto)
        {
            if (dto.UserID <= 0)
            {
                throw new ErrorCodeException(-1, "用户不能为空");
            }

            var application = await GetRequiredApplication(dto.PaymentApplicationID);
            var isExist = await _service.Exist(f => f.UserID == dto.UserID && f.PaymentApplicationID == application.ID && !f.IsDelete);
            if (isExist)
            {
                throw new ErrorCodeException(-1, "支付管理员已存在，无法重复创建！");
            }

            var model = new Live_PaymentAdmin
            {
                ID = Guid.NewGuid(),
                UserID = dto.UserID,
                UserGID = dto.UserGID == Guid.Empty ? Guid.NewGuid() : dto.UserGID,
                SecretKey = _totpService.GenerateSecretKey(),
                IsEnable = dto.IsEnable,
                IsDelete = false,
                DeleteTime = null,
                IsValidate = false,
                IsLock = false,
                LockTime = null,
                FailMsg = null,
                FailTimes = 0,
                CreateTime = DateTime.Now,
                UpdateTime = null,
                Remark = dto.Remark,
                RealName = dto.RealName?.Trim(),
                DispalyName = dto.DispalyName?.Trim(),
                PaymentApplicationID = application.ID,
            };
            await _service.Add(model);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update(Live_PaymentAdmin dto)
        {
            var model = await _service.Get(f => f.ID == dto.ID && !f.IsDelete);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }

            await EnsureExistingApplicationOwned(model.PaymentApplicationID);
            var application = await GetRequiredApplication(dto.PaymentApplicationID);

            model.Remark = dto.Remark;
            model.IsEnable = dto.IsEnable;
            model.RealName = dto.RealName?.Trim();
            model.DispalyName = dto.DispalyName?.Trim();
            model.PaymentApplicationID = application.ID;
            model.UpdateTime = DateTime.Now;
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

        [HttpPut]
        public async Task<IActionResult> SetLock([FromForm] Guid id)
        {
            var model = await _service.Get(f => f.ID == id && !f.IsDelete);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "数据不存在");
            }
            await EnsureExistingApplicationOwned(model.PaymentApplicationID);
            model.IsLock = !model.IsLock;
            model.LockTime = model.IsLock ? DateTime.Now : null;
            model.UpdateTime = DateTime.Now;
            await _service.Update(model);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> ValidateCode([FromForm] Guid id, [FromForm] string code)
        {
            var model = await _service.GetWithSecretkey(f => f.ID == id && !f.IsDelete);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "数据不存在");
            }
            await EnsureExistingApplicationOwned(model.PaymentApplicationID);
            if (model.IsValidate)
            {
                throw new ErrorCodeException(-1, "已验证通过，无法重复验证");
            }
            if (!_totpService.ValidateCode(model.SecretKey, code))
            {
                throw new ErrorCodeException(-1, "验证失败");
            }

            model.IsValidate = true;
            model.UpdateTime = DateTime.Now;
            await _service.Update(model);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GenerateQrCode(Guid id)
        {
            var model = await _service.GetWithSecretkey(f => f.ID == id && !f.IsDelete);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "数据不存在");
            }
            if (model.IsValidate)
            {
                throw new ErrorCodeException(-1, "已验证通过，不再显示授权二维码");
            }

            var application = await GetRequiredApplication(model.PaymentApplicationID);

            var qrUrl = $"otpauth://totp/{application.AppName}:{model.DispalyName}?secret={model.SecretKey}&issuer=PAY&algorithm=SHA1&digits=6&period=30";
            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(qrUrl, QRCodeGenerator.ECCLevel.Q);
            var pngQRCode = new PngByteQRCode(qrCodeData);
            var qrCodeImage = pngQRCode.GetGraphic(20);
            return Ok(Convert.ToBase64String(qrCodeImage));
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
            EnsureCurrentUserMatchApplication(application);
            return application;
        }

        private async Task EnsureExistingApplicationOwned(Guid paymentApplicationID)
        {
            if (paymentApplicationID == Guid.Empty)
            {
                throw new ErrorCodeException(-1, "支付管理员未绑定支付应用");
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
    }
}
