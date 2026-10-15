using LZ.Core;
using LZ.Finance.Service.Interface;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace LZ.Finance.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PaymentApplicationController : ApiControllerBase
    {
        private readonly IPaymentApplicationService _service;

        public PaymentApplicationController(IPaymentApplicationService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> PageList(string appName, string appId, bool? isEnable, bool? isDefault, int? bankType, [FromQuery] PagedModelBase query)
        {
            var exp = Expressionable.Create<Live_PaymentApplication>();
            exp.And(f => !f.IsDelete);
            exp.AndIF(!string.IsNullOrWhiteSpace(appName), f => f.AppName.Contains(appName));
            exp.AndIF(!string.IsNullOrWhiteSpace(appId), f => f.AppID.Contains(appId));
            exp.AndIF(isEnable.HasValue, f => f.IsEnable == isEnable.Value);
            exp.AndIF(isDefault.HasValue, f => f.IsDefault == isDefault.Value);
            exp.AndIF(bankType.HasValue, f => f.BankType == bankType.Value);
            var models = await _service.PageList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }

        [HttpGet]
        public async Task<IActionResult> List(string appName, int? bankType, string queryType)
        {
            var exp = Expressionable.Create<Live_PaymentApplication>();
            exp.And(f => f.IsEnable);
            exp.And(f => f.CompanyID==CurrentUser.CompanyID);
            exp.AndIF(!string.IsNullOrWhiteSpace(appName), f => f.AppName.Contains(appName));
            exp.AndIF(bankType.HasValue, f => f.BankType == bankType.Value);
          
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
        public async Task<IActionResult> Add(Live_PaymentApplication dto)
        {
            var model = new Live_PaymentApplication
            {
                ID = Guid.NewGuid(),
                AppName = dto.AppName?.Trim(),
                AppID = dto.AppID?.Trim(),
                Remark = dto.Remark,
                BankType = dto.BankType,
                CompanyID = CurrentUser.CompanyID,
                CompanyGID = CurrentUser.CompanyGID ?? Guid.Empty,
                CompanyName = dto.CompanyName?.Trim(),
                IsEnable = dto.IsEnable,
                IsDefault = dto.IsDefault,
                IsDelete = false,
                CreateTime = DateTime.Now,
                DeleteTime = null,
                UpdateTime = null
            };

            EnsureCompanyMatch(model);
            await ValidateModel(model, null);
            if (model.IsDefault)
            {
                await _service.ClearOtherDefault(model.CompanyID, model.ID);
            }
            await _service.Add(model);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update(Live_PaymentApplication dto)
        {
            var model = await _service.Get(f => f.ID == dto.ID && !f.IsDelete);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }

            EnsureCompanyMatch(model);
            if (dto.CompanyID != model.CompanyID || dto.CompanyGID != model.CompanyGID)
            {
                throw new ErrorCodeException(-1, "公司新增后不允许更改");
            }

            model.AppName = dto.AppName?.Trim();
            model.AppID = dto.AppID?.Trim();
            model.Remark = dto.Remark;
            model.BankType = dto.BankType;
            model.IsEnable = dto.IsEnable;
            model.IsDefault = dto.IsDefault;
            model.UpdateTime = DateTime.Now;

            await ValidateModel(model, dto.ID);
            if (model.IsDefault)
            {
                await _service.ClearOtherDefault(model.CompanyID, model.ID);
            }
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
            model.IsEnable = !model.IsEnable;
            model.UpdateTime = DateTime.Now;
            await _service.Update(model);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> SetDefault([FromForm] Guid id)
        {
            var model = await _service.Get(f => f.ID == id && !f.IsDelete);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "数据不存在");
            }
            if (!model.IsEnable)
            {
                throw new ErrorCodeException(-1, "支付应用未启用，无法设为默认");
            }
            model.IsDefault = true;
            model.UpdateTime = DateTime.Now;
            await _service.ClearOtherDefault(model.CompanyID, model.ID);
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
            model.IsEnable = false;
            model.IsDelete = true;
            model.DeleteTime = DateTime.Now;
            model.UpdateTime = DateTime.Now;
            await _service.Update(model);
            return Ok();
        }

        private void EnsureCompanyMatch(Live_PaymentApplication dto)
        {
            var currentCompanyGid = CurrentUser.CompanyGID ?? Guid.Empty;
            if (dto.CompanyID != CurrentUser.CompanyID || dto.CompanyGID != currentCompanyGid)
            {
                throw new ErrorCodeException(-1, "公司信息不匹配，禁止操作");
            }
        }

        private async Task ValidateModel(Live_PaymentApplication model, Guid? excludeId)
        {
            if (string.IsNullOrWhiteSpace(model.AppName))
            {
                throw new ErrorCodeException(-1, "应用名称不能为空");
            }
            if (string.IsNullOrWhiteSpace(model.AppID))
            {
                throw new ErrorCodeException(-1, "APP编号不能为空");
            }
            if (model.BankType == (int)BankTypeEnumType.未知)
            {
                throw new ErrorCodeException(-1, "银行类型不能为空");
            }
            if (string.IsNullOrWhiteSpace(model.CompanyName))
            {
                throw new ErrorCodeException(-1, "公司名称不能为空");
            }

            var nameExist = excludeId.HasValue
                ? await _service.Exist(f => f.AppName == model.AppName  && f.CompanyID == model.CompanyID && !f.IsDelete && f.ID != excludeId.Value)
                : await _service.Exist(f => f.AppName == model.AppName && f.CompanyID == model.CompanyID && !f.IsDelete);
            if (nameExist)
            {
                throw new ErrorCodeException(-1, $"应用名称：【{model.AppName}】已存在，无法重复创建！");
            }

            var appIdExist = excludeId.HasValue
                ? await _service.Exist(f => f.AppID == model.AppID  && f.CompanyID == model.CompanyID && !f.IsDelete && f.ID != excludeId.Value)
                : await _service.Exist(f => f.AppID == model.AppID  && f.CompanyID == model.CompanyID && !f.IsDelete);
            if (appIdExist)
            {
                throw new ErrorCodeException(-1, $"APP编号：【{model.AppID}】已存在，无法重复创建！");
            }
        }
    }
}
