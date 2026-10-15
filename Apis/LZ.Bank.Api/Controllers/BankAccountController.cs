//using LZ.Bank.Service.Interface;
//using LZ.Bank.Service.Services;
//using LZ.Core;
//using LZ.Model;
//using LZ.Model.Bank.CZY;
//using LZ.Model.Bank.CZY.ParamSetModels;
//using LZ.Model.Bank.CZY.Response;
//using LZ.Model.Dtos.BankDo;
//using LZ.Model.Enums;
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
//    public class BankAccountController : ApiControllerBase
//    {
//        private readonly IBankAccountService _service;
//        private readonly ICzyService _czyService;
//        private readonly IWebHostEnvironment _env;

//        public BankAccountController(IBankAccountService service, ICzyService czyService, IWebHostEnvironment env)
//        {
//            _service = service;
//            _czyService = czyService;
//            _env = env;
//        }

//        [HttpGet]
//        public async Task<IActionResult> PageList(string name,  [FromQuery] PagedModelBase query)
//        {
//            var exp = Expressionable.Create<Model.Live_BankAccount>();
//            exp.And(f=>!f.IsDelete);
//            exp.AndIF(!string.IsNullOrWhiteSpace(name), f => f.AccountName.Contains(name));
//            var models = await _service.PageList(exp.ToExpression(), query);
//            return Ok(new { items = models, count = query.Count });
//        }

//        [HttpGet]
//        public async Task<IActionResult> List(BankTypeEnumType? bankType)
//        {
//            var exp = Expressionable.Create<Model.Live_BankAccount>();
//            exp.And(f => !f.IsDelete);
//            exp.And(f => f.IsEnable);
//            exp.AndIF(bankType.HasValue ,f => f.BankType==bankType.Value);
//            var models = await _service.List(exp.ToExpression());
//            return Ok(models);
//        }

//        [HttpPost]
//        public async Task<IActionResult> Add(Live_BankAccount dto)
//        {
            
//            var isExist = await _service.Exist(f => f.AccountNo == dto.AccountNo && !f.IsDelete);
//            if (isExist)
//            {
//                throw new ErrorCodeException(-1, $"【{dto.AccountNo}】已存在，无法重复创建！");
//            }

//            #region 查询账户信息

//            // 获取账户余额
//            T0005ParamSetItem accountReq = new T0005ParamSetItem()
//            {
//                AcctNo = dto.AccountNo,
//                CurrencyCode = "CNY",
//            };
//            var accountRes = await _czyService.QueryAccountInfo(accountReq);
//            var resBody = accountRes.Cmscloud.Body.Data;

//            if (resBody.ResultCode == "0000")
//            {
//                var accountInfo = resBody.ResultSet.FirstOrDefault();
//                var model = new Model.Live_BankAccount()
//                {
//                    CreateTime = DateTime.Now,
//                    AccountName = accountInfo.AcctName,
//                    AccountNo = accountInfo.AcctNo,
//                    BankType = dto.BankType,
//                    Remark = dto.Remark,
//                    DeleteTime = null,
//                    ID = Guid.NewGuid(),
//                    IsDelete = false,
//                    IsEnable = dto.IsEnable,
//                    CompanyID = CurrentUser.CompanyID,
//                    DirectCode = accountInfo.DirectCode,
//                    AcctCityCode = accountInfo.AcctCityCode,
//                    AcctCityName = accountInfo.AcctCityName,
//                    AcctKey = accountInfo.AcctKey,
//                    AcctName = accountInfo.AcctName,
//                    AcctNo = accountInfo.AcctNo,
//                    AcctOpenDate = accountInfo.AcctOpenDate,
//                    AcctProvinceCode = accountInfo.AcctProvinceCode,
//                    AcctProvinceName = accountInfo.AcctProvinceName,
//                    AcctStatus = accountInfo.AcctStatus,
//                    AuthorizedOrgCode = accountInfo.AuthorizedOrgCode,
//                    AuthorizedOrgKey = accountInfo.AuthorizedOrgKey,
//                    AuthorizedOrgName = accountInfo.AuthorizedOrgName,
//                    CurrencyCode = accountInfo.CurrencyCode,
//                    CurrencyName = accountInfo.CurrencyName,
//                    LegalPersonOrgCode = accountInfo.LegalPersonOrgCode,
//                    LegalPersonOrgKey = accountInfo.LegalPersonOrgKey,
//                    LegalPersonOrgName = accountInfo.LegalPersonOrgName,
//                    OrgCode = accountInfo.OrgCode,
//                    OrgKey = accountInfo.OrgKey,
//                    OrgName = accountInfo.OrgName,
//                    UseOrgCode = accountInfo.UseOrgCode,
//                    UseOrgKey = accountInfo.UseOrgKey,
//                    UseOrgName = accountInfo.UseOrgName,
//                };
//                await _service.Add(model);
//            }
//            else {

//                throw new ErrorCodeException(-1, resBody.ResultMsg);
            
//            }
//            #endregion

     
//            return Ok();
//        }
//        [HttpGet]
//        public async Task<IActionResult> Get(Guid id)
//        {
//            var model = await _service.Get(f=>f.ID==id);
//            if (model == null)
//            {
//                throw new ErrorCodeException(-1, "你要读取的数据不存在");
//            }
//            return Ok(model);
//        }
//        [HttpPut]
//        public async Task<IActionResult> Update(Live_BankAccount dto)
//        {
//            var model = await _service.Get(f => f.ID == dto.ID);
//            if (model == null)
//            {
//                throw new ErrorCodeException(-1, "你要读取的数据不存在");
//            }
//            //model.AccountName = dto.AccountName.Trim();
//            //model.AccountNo = dto.AccountNo.Trim();
//            model.IsEnable = dto.IsEnable;

//            var isExist = await _service.Exist(f => (f.AccountNo == model.AccountNo || f.AccountName == model.AccountName)&&f.ID!=dto.ID && !f.IsDelete);
//            if (isExist)
//            {
//                throw new ErrorCodeException(-1, $"【{dto.AccountName}或{dto.AccountNo}】已存在，操作失败！");
//            }

//            await _service.Update(model);
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


//        /// <summary>
//        /// 查看账户信息
//        /// </summary>
//        /// <param name="id"></param>
//        /// <returns></returns>
//        [AllowAnonymous]
//        [HttpGet]
//        public async Task<IActionResult> QueryAccountDetail(Guid id)
//        {
//            //var model = await _service.Get(f => f.ID == id);
//            //T0005ParamSetItem req = new T0005ParamSetItem() { AcctNo = model.AcctNo, AcctName = model.AcctName };
//            //var res = await _czyService.QueryAccountInfo(req);
//            string resPath = Path.Combine(_env.WebRootPath, "CzyTest", "T0005Result.txt");
//            var json = System.IO.File.ReadAllText(resPath);
//            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<CzyCmscloud<T0005Response>>(json);
//            return Ok(result.Cmscloud.Body.Data.ResultSet.FirstOrDefault());
//        }
//        /// <summary>
//        /// 查询账户余额
//        /// </summary>
//        /// <param name="id"></param>
//        /// <returns></returns>
//        [AllowAnonymous]
//        [HttpGet]
//        public async Task<IActionResult> QueryAccountBalanceDetail(Guid id)
//        {
//            //var model = await _service.Get(f => f.ID == id);
//            //T0005ParamSetItem req = new T0005ParamSetItem() { AcctNo = model.AcctNo, AcctName = model.AcctName };
//            //var res = await _czyService.QueryAccountInfo(req);
//            string resPath = Path.Combine(_env.WebRootPath, "CzyTest", "T0006Result.txt");
//            var json = System.IO.File.ReadAllText(resPath);
//            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<CzyCmscloud<T0006Response>>(json);
//            return Ok(result.Cmscloud.Body.Data.ResultSet.FirstOrDefault());
//        }




//    }
//}
