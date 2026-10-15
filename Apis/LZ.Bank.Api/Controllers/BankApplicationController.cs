
//using LZ.Bank.Service.Interface;
//using LZ.Common;
//using LZ.Core;
//using LZ.Model;
//using LZ.Model.Dtos.BankDo;
//using LZ.Model.Enums;
//using LZ.Model.Exceptions;
//using LZ.Model.Options;
//using LZ.Model.PagedModel;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Options;
//using SqlSugar;

//namespace LZ.Bank.Api.Controllers
//{
//    [Route("api/[controller]/[action]")]
//    [ApiController]
//    public class BankApplicationController : ApiControllerBase
//    {
//        private readonly IBankApplicationService _service;
//        private readonly BankAesOption _bankAesOption;

//        public BankApplicationController(IBankApplicationService service, IOptions<BankAesOption> bankAesOption)
//        {
//            _service = service;
//            _bankAesOption = bankAesOption.Value;
//        }

//        [HttpGet]
//        public async Task<IActionResult> PageList(string appName, string appId, bool? isEnable, BankTypeEnumType? bankType, [FromQuery] PagedModelBase query)
//        {
//            var exp = Expressionable.Create<Czy_BankApplication>();
//            exp.And(f => !f.IsDelete);
//            exp.AndIF(!string.IsNullOrWhiteSpace(appName), f => f.AppName.Contains(appName));
//            exp.AndIF(!string.IsNullOrWhiteSpace(appId), f => f.AppID.Contains(appId));
//            exp.AndIF(isEnable.HasValue, f => f.IsEnable == isEnable.Value);
//            exp.AndIF(bankType.HasValue, f => f.BankType == bankType.Value);
//            var models = await _service.PageList(exp.ToExpression(), query);
//            return Ok(new { items = models, count = query.Count });
//        }

//        [HttpGet]
//        public async Task<IActionResult> List(string appName, BankTypeEnumType? bankType, string queryType)
//        {
//            var exp = Expressionable.Create<Czy_BankApplication>();
//            exp.And(f => !f.IsDelete);
//            exp.And(f => f.IsEnable);
//            exp.AndIF(!string.IsNullOrWhiteSpace(appName), f => f.AppName.Contains(appName));
//            exp.AndIF(bankType.HasValue, f => f.BankType == bankType.Value);
//            if (string.IsNullOrWhiteSpace(queryType) || !string.Equals(queryType, "all", StringComparison.OrdinalIgnoreCase))
//            {
//                exp.And(f => f.CompanyID == CurrentUser.CompanyID);
//            }
//            var models = await _service.List(exp.ToExpression());
//            return Ok(models);
//        }

//        [HttpGet]
//        public async Task<IActionResult> Get(Guid id)
//        {
//            var model = await _service.Get(f => f.ID == id && !f.IsDelete);
//            if (model == null)
//            {
//                throw new ErrorCodeException(-1, "你要读取的数据不存在");
//            }
//            return Ok(model);
//        }

//        [HttpPost]
//        public async Task<IActionResult> Add(BankApplicationDto dto)
//        {
//            var model = BuildModel(dto);
//            model.ID = Guid.NewGuid();
//            model.CreateTime = DateTime.Now;
//            model.UpdateTime = null;
//            model.IsDelete = false;
//            model.DeleteTime = null;

//            EnsureCompanyMatch(dto);

//            model.AppPrivateKey = AesCBCHelper.Encrypt(dto.AppPrivateKey, _bankAesOption.KeyBytes, _bankAesOption.IvBytes);
//            model.AppEncryptKey = AesCBCHelper.Encrypt(dto.AppEncryptKey, _bankAesOption.KeyBytes, _bankAesOption.IvBytes);

//            await ValidateModel(model, null);
//            await _service.Add(model);
//            return Ok();
//        }

//        [HttpPut]
//        public async Task<IActionResult> Update(BankApplicationDto dto)
//        {
//            var model = await _service.Get(f => f.ID == dto.ID && !f.IsDelete);
//            if (model == null)
//            {
//                throw new ErrorCodeException(-1, "你要读取的数据不存在");
//            }

//            EnsureCompanyMatch(dto);
//            if (model.CompanyID != dto.CompanyID || model.CompanyGID != dto.CompanyGID)
//            {
//                throw new ErrorCodeException(-1, "无权修改其他公司的数据");
//            }

//            model.AppName = dto.AppName?.Trim();
//            model.AppID = dto.AppID?.Trim();
//            model.AppSignType = NormalizeSignType(dto.AppSignType);
//            model.AppCharset = string.IsNullOrWhiteSpace(dto.AppCharset) ? "UTF-8" : dto.AppCharset.Trim();
//            model.AppFormat = NormalizeFormat(dto.AppFormat);
//            model.AppPulicKey = dto.AppPulicKey;
//            model.AppEncryptType = dto.AppEncryptType?.Trim();
//            model.Remark = dto.Remark;
//            model.IsEnable = dto.IsEnable;
//            model.BankType = dto.BankType;
//            model.CompanyID = dto.CompanyID;
//            model.CompanyGID = dto.CompanyGID;
//            model.CompanyName = dto.CompanyName?.Trim();
//            model.UpdateTime = DateTime.Now;

//            if (!string.IsNullOrWhiteSpace(dto.AppPrivateKey))
//            {
//                model.AppPrivateKey = AesCBCHelper.Encrypt(dto.AppPrivateKey, Convert.FromHexString(_bankAesOption.Key), Convert.FromHexString(_bankAesOption.Iv));
//            }
//            if (!string.IsNullOrWhiteSpace(dto.AppEncryptKey))
//            {
//                model.AppEncryptKey = AesCBCHelper.Encrypt(dto.AppEncryptKey, Convert.FromHexString(_bankAesOption.Key), Convert.FromHexString(_bankAesOption.Iv));
//            }
//            await ValidateModel(model, dto.ID);
//            await _service.Update(model);
//            return Ok();
//        }

//        [HttpPut]
//        public async Task<IActionResult> SetEnable([FromForm] Guid id)
//        {
//            var model = await _service.Get(f => f.ID == id && !f.IsDelete);
//            if (model == null)
//            {
//                throw new ErrorCodeException(-1, "数据不存在");
//            }
//            model.IsEnable = !model.IsEnable;
//            model.UpdateTime = DateTime.Now;
//            await _service.Update(model);
//            return Ok();
//        }

//        [HttpDelete]
//        public async Task<IActionResult> Delete([FromForm] Guid id)
//        {
//            var model = await _service.Get(f => f.ID == id && !f.IsDelete);
//            if (model == null)
//            {
//                throw new ErrorCodeException(-1, "数据不存在");
//            }
//            model.IsEnable = false;
//            model.IsDelete = true;
//            model.DeleteTime = DateTime.Now;
//            model.UpdateTime = DateTime.Now;
//            await _service.Update(model);
//            return Ok();
//        }

//        private Czy_BankApplication BuildModel(BankApplicationDto dto)
//        {
//            return new Czy_BankApplication
//            {
//                AppName = dto.AppName?.Trim(),
//                AppID = dto.AppID?.Trim(),
//                AppSignType = NormalizeSignType(dto.AppSignType),
//                AppPrivateKey = dto.AppPrivateKey,
//                AppCharset = string.IsNullOrWhiteSpace(dto.AppCharset) ? "UTF-8" : dto.AppCharset.Trim(),
//                AppFormat = NormalizeFormat(dto.AppFormat),
//                AppPulicKey = dto.AppPulicKey,
//                AppEncryptType = dto.AppEncryptType?.Trim(),
//                AppEncryptKey = dto.AppEncryptKey,
//                Remark = dto.Remark,
//                IsEnable = dto.IsEnable,
//                BankType = dto.BankType,
//                CompanyID = dto.CompanyID,
//                CompanyGID = dto.CompanyGID,
//                CompanyName = dto.CompanyName?.Trim()
//            };
//        }

//        private async Task ValidateModel(Czy_BankApplication model, Guid? excludeId)
//        {
//            if (string.IsNullOrWhiteSpace(model.AppName))
//            {
//                throw new ErrorCodeException(-1, "应用名称不能为空");
//            }
//            if (string.IsNullOrWhiteSpace(model.AppID))
//            {
//                throw new ErrorCodeException(-1, "APP编号不能为空");
//            }
//            if (model.BankType == BankTypeEnumType.未知)
//            {
//                throw new ErrorCodeException(-1, "银行类型不能为空");
//            }
//            if (string.IsNullOrWhiteSpace(model.CompanyName))
//            {
//                throw new ErrorCodeException(-1, "公司名称不能为空");
//            }

//            var nameExist = excludeId.HasValue
//                ? await _service.Exist(f => f.AppName == model.AppName && f.BankType == model.BankType && f.CompanyID == model.CompanyID && !f.IsDelete && f.ID != excludeId.Value)
//                : await _service.Exist(f => f.AppName == model.AppName && f.BankType == model.BankType && f.CompanyID == model.CompanyID && !f.IsDelete);
//            if (nameExist)
//            {
//                throw new ErrorCodeException(-1, $"应用名称：【{model.AppName}】已存在，无法重复创建！");
//            }

//            var appIdExist = excludeId.HasValue
//                ? await _service.Exist(f => f.AppID == model.AppID && f.BankType == model.BankType && f.CompanyID == model.CompanyID && !f.IsDelete && f.ID != excludeId.Value)
//                : await _service.Exist(f => f.AppID == model.AppID && f.BankType == model.BankType && f.CompanyID == model.CompanyID && !f.IsDelete);
//            if (appIdExist)
//            {
//                throw new ErrorCodeException(-1, $"APP编号：【{model.AppID}】已存在，无法重复创建！");
//            }
//        }

//        private void EnsureCompanyMatch(BankApplicationDto dto)
//        {
//            var currentCompanyGid = CurrentUser.CompanyGID ?? Guid.Empty;
//            if (dto.CompanyID != CurrentUser.CompanyID || dto.CompanyGID != currentCompanyGid)
//            {
//                throw new ErrorCodeException(-1, "公司信息不匹配，禁止操作");
//            }
//        }

//        private static string NormalizeSignType(string signType)
//        {
//            return string.IsNullOrWhiteSpace(signType) ? "RSA2" : signType.Trim();
//        }

//        private static string NormalizeFormat(string format)
//        {
//            var value = string.IsNullOrWhiteSpace(format) ? "json" : format.Trim();
//            if (!string.Equals(value, "json", StringComparison.OrdinalIgnoreCase))
//            {
//                throw new ErrorCodeException(-1, "请求参数格式仅支持json");
//            }
//            return "json";
//        }
//    }
//}
