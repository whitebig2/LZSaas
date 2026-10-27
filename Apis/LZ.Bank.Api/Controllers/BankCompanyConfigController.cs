//using LZ.Bank.Service.Interface;
//using LZ.Core;
//using LZ.Model;
//using LZ.Model.Dtos.BankDo;
//using LZ.Model.Exceptions;
//using LZ.Model.PagedModel;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using SqlSugar;
//using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

//namespace LZ.Bank.Api.Controllers
//{
//    [Route("api/[controller]/[action]")]
//    [ApiController]
//    [AllowAnonymous]
//    public class BankCompanyConfigController : ApiControllerBase
//    {
//        private readonly IBankCompanyConfigService _service;

//        public BankCompanyConfigController(IBankCompanyConfigService service)
//        {
//            _service = service;
//        }

//        [HttpGet]
//        public async Task<IActionResult> PageList(string name,  [FromQuery] PagedModelBase query)
//        {
//            var exp = Expressionable.Create<Model.Live_V_BankCompanyConfig>();
//            exp.And(f=>!f.IsDelete);
//            exp.AndIF(!string.IsNullOrWhiteSpace(name), f => f.BankCompanyName.Contains(name));
//            var models = await _service.PageList(exp.ToExpression(), query);
//            return Ok(new { items = models, count = query.Count });
//        }

//        [HttpGet]
//        public async Task<IActionResult> List(string name,Guid? bankCompanyID)
//        {
//            var exp = Expressionable.Create<Model.Live_V_BankCompanyConfig>();
//            exp.And(f => !f.IsDelete);
//            exp.And(f =>f.IsEnable);
//            exp.AndIF(!string.IsNullOrWhiteSpace(name), f => f.BankCompanyName.Contains(name));
//            exp.AndIF(bankCompanyID.HasValue&&bankCompanyID.Value!=Guid.Empty, f =>f.BankCompanyID==bankCompanyID.Value);
//            exp.And(f => f.CompanyID == CurrentUser.CompanyID);
//            var models = await _service.ListView(exp.ToExpression());
//            return Ok(models);
//        }
//        [HttpGet]
//        public async Task<IActionResult> Get(Guid id)
//        {
//            var model = await _service.Get(f => f.ID == id);
//            if (model == null)
//            {
//                throw new ErrorCodeException(-1, "你要读取的数据不存在");
//            }
//            return Ok(model);
//        }

//        [HttpPost]
//        public async Task<IActionResult> Add(Live_BankCompanyConfig dto)
//        {
//            var model = new Model.Live_BankCompanyConfig()
//            {
//                IsEnable = dto.IsEnable,
//                BankCompanyID = dto.BankCompanyID,
//                BankType = dto.BankType,
//                IsDefault = dto.IsDefault,
//                MainBankAccountID = dto.MainBankAccountID,
//                OtherBankAccountID = dto.OtherBankAccountID,
//                PrepaidBankAccountID = dto.PrepaidBankAccountID,
//                ConfigTitle = dto.ConfigTitle,
//                CreateTime = DateTime.Now,
//                DeleteTime = null,
//                ID = Guid.NewGuid(),
//                IsDelete = false,
//            };

//            if (model.IsDefault)
//            {
//               var isDefault= await _service.Exist(f => f.IsDefault&&f.BankCompanyID==model.BankCompanyID && !f.IsDelete);
//                if (isDefault)
//                {
//                    throw new ErrorCodeException(-1, $"默认配置已存在，无法重复创建！");
//                }
//            }
//            var isExist = await _service.Exist(f => f.ConfigTitle==model.ConfigTitle && !f.IsDelete);
//            if (isExist)
//            {
//                throw new ErrorCodeException(-1, $"配置名称：【{dto.ConfigTitle}】已存在，无法重复创建！");
//            }

//            await _service.Add(model);
//            return Ok();
//        }
        
//        [HttpPut]
//        public async Task<IActionResult> SetEnable([FromForm] Guid id)
//        {
//            var model = await _service.Get(f => f.ID == id);
//            model.IsEnable = !model.IsEnable;
//            await _service.Update(model);
//            return Ok();
//        }
//        [HttpPut]
//        public async Task<IActionResult> Update(Live_BankCompanyConfig dto)
//        {
//            var model = await _service.Get(f => f.ID == dto.ID);
//            if (model == null)
//            {
//                throw new ErrorCodeException(-1, "你要读取的数据不存在");
//            }
//            model.ConfigTitle = dto.ConfigTitle.Trim();
//            model.BankCompanyID = dto.BankCompanyID;
//            model.BankType = dto.BankType;
//            model.IsEnable = dto.IsEnable;
//            model.IsDefault = dto.IsDefault;
//            model. MainBankAccountID   = dto.MainBankAccountID;
//            model.PrepaidBankAccountID = dto.PrepaidBankAccountID;
//            model.OtherBankAccountID = dto.OtherBankAccountID;


//            if (model.IsDefault)
//            {
//                var isDefault = await _service.Exist(f => f.IsDefault && f.BankCompanyID == model.BankCompanyID && f.ID != dto.ID && !f.IsDelete);
//                if (isDefault)
//                {
//                    throw new ErrorCodeException(-1, $"默认配置已存在，无法重复创建！");
//                }
//            }

//            var isExist = await _service.Exist(f =>f.ConfigTitle==model.ConfigTitle &&f.ID!=dto.ID&& !f.IsDelete);
//            if (isExist)
//            {
//                throw new ErrorCodeException(-1, $"配置名称：【{dto.ConfigTitle}】已存在，无法重复创建！");
//            }

//            await _service.Update(model);
//            return Ok();
//        }
//        [HttpDelete]
//        public async Task<IActionResult> Delete([FromForm] Guid id)
//        {
//            var model = await _service.Get(f => f.ID == id);
//            if (model == null)
//            {
//                throw new ErrorCodeException(-1, "数据不存在");
//            }
//            model.IsEnable = false;
//            model.IsDelete = true;
//            model.DeleteTime= DateTime.Now;
//            await _service.Update(model);
//            return Ok();
//        }




//    }
//}
