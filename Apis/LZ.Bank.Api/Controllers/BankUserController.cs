//using LZ.Bank.Service.Interface;
//using LZ.Core;
//using LZ.Core.Interface;
//using LZ.Model;
//using LZ.Model.Dtos.BankDo;
//using LZ.Model.Exceptions;
//using LZ.Model.PagedModel;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.DataProtection;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using QRCoder;
//using Senparc.Weixin.TenPay.V2;
//using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
//using SqlSugar;
//using static QRCoder.PayloadGenerator;

//namespace LZ.Bank.Api.Controllers
//{
//    [Route("api/[controller]/[action]")]
//    [ApiController]
//    public class BankUserController : ApiControllerBase
//    {

//        private readonly IBankUserService _service;
//        private readonly ITotpService _totpService;
//        private readonly IWebHostEnvironment _env;
//        private readonly IBankUserCompanyService _userCompanyService ;

//        public BankUserController(IBankUserService service, ITotpService totpService, IWebHostEnvironment env, IBankUserCompanyService userCompanyService)
//        {
//            _service = service;
//            _totpService = totpService;
//            _env = env;
//            _userCompanyService = userCompanyService;
//        }

//        [HttpGet]
//        public async Task<IActionResult> PageList(string userName, string fullName, [FromQuery] PagedModelBase query)
//        {
//            var exp = Expressionable.Create<Model.Live_V_BankUser>();
//            exp.And(f => !f.IsDelete);
//            exp.And(f => f.CompanyID == CurrentUser.CompanyID);
//            exp.AndIF(!string.IsNullOrWhiteSpace(userName), f => f.UserName.Contains(userName));
//            exp.AndIF(!string.IsNullOrWhiteSpace(fullName), f => f.FullName.Contains(fullName));
//            var models = await _service.PageList(exp.ToExpression(), query);
//            return Ok(new { items = models, count = query.Count });
//        }

//        [HttpGet]
//        public async Task<IActionResult> List(string userName, string fullName, string queryType)
//        {
//            var exp = Expressionable.Create<Model.Live_V_BankUser>();
//            exp.And(f => !f.IsDelete);
//            exp.AndIF(!string.IsNullOrWhiteSpace(userName), f => f.UserName.Contains(userName));
//            exp.AndIF(!string.IsNullOrWhiteSpace(fullName), f => f.FullName.Contains(fullName));
//            exp.And(f => f.CompanyID == CurrentUser.CompanyID);

//            var models = await _service.ListView(exp.ToExpression());
//            return Ok(models);
//        }

//        [HttpPost]
//        public async Task<IActionResult> Add(BankUserDto dto)
//        {
//            var model = new Model.Live_BankUser()
//            {
//                CreateTime = DateTime.Now,
//                DeleteTime = null,
//                ID = Guid.NewGuid(),
//                IsDelete = false,
//                IsEnable = dto.IsEnable,
//                SecretKey = _totpService.GenerateSecretKey(),
//                UserID = dto.UserID
//            };

//            var isExist = await _service.Exist(f => f.UserID == model.UserID && !f.IsDelete);
//            if (isExist)
//            {
//                throw new ErrorCodeException(-1, $"支付账户已存在，无法重复创建！");
//            }

//            await _service.Add(model);
//            return Ok();
//        }
//        [HttpGet]
//        public async Task<Model.Live_V_BankUser> Get(Guid id)
//        {
//            var model = await _service.GetView(f => f.ID == id);
//            if (model == null)
//            {
//                throw new ErrorCodeException(-1, "你要读取的数据不存在");
//            }
//            return model;
//        }
//        [HttpPut]
//        public async Task<IActionResult> SetEnable([FromForm]Guid id)
//        {
//            var model = await _service.Get(f => f.ID == id);
//            model.IsEnable = !model.IsEnable;
//            await _service.Update(model);
//            return Ok();
//        }
//        [AllowAnonymous]
//        [HttpPut]
//        public async Task<IActionResult> ValidateCode([FromForm] Guid id, [FromForm] string code)
//        {
//            var model = await _service.GetWithSecretkey(f => f.ID == id);
//            if (model.IsValidate)
//            {
//                throw new ErrorCodeException(-1, "已验证通过，无法重复验证");
//            }
//            var isb = _totpService.ValidateCode(model.SecretKey, code);
//            if (isb)
//            {
//                model.IsValidate = true;
//            }
//            else
//            {
//                throw new ErrorCodeException(-1,"验证失败");
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

//            model.IsDelete = true;
//            model.DeleteTime = DateTime.Now;



//            await _service.Update(model);
//            return Ok();
//        }


//        [AllowAnonymous]
//        [HttpGet]
//        public async Task<IActionResult> GenerateQrCode(Guid id)
//        {
//            var modelView = await _service.GetView(f => f.ID== id);
//            if (modelView.IsValidate)
//            {
//                throw new ErrorCodeException(-1, "已验证通过，不再显示授权二位码");
//            }
//            var model = await _service.GetWithSecretkey(f => f.ID == id);
//            var qrUrl = $"otpauth://totp/XFZ:{modelView.UserName}?secret={model.SecretKey}&issuer=XFZ&algorithm=SHA1&digits=6&period=30";
//            // 1. 创建生成器
//            QRCodeGenerator qrGenerator = new QRCodeGenerator();
//            // 2. 创建二维码数据 (ECCLevel: 纠错级别 L, M, Q, H)
//            QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrUrl, QRCodeGenerator.ECCLevel.Q);
//            // 3. 生成 PNG 字节数组 (跨平台兼容性好，不依赖 System.Drawing)
//            PngByteQRCode pngQRCode = new PngByteQRCode(qrCodeData);
//            byte[] qrCodeImage = pngQRCode.GetGraphic(20); // 20 是像素模块大小
//            //return File(qrCodeImage,"image/png");
//            return Ok(Convert.ToBase64String(qrCodeImage));
//        }


//        [AllowAnonymous]
//        [HttpPost]
//        public async Task<IActionResult> UpdateUserCompany(BankUserCompanyReqDto req)
//        {
//            var records = await _userCompanyService.List(f => f.BankUserID == req.BankUserID);
//            await _userCompanyService.Delete(f => f.BankUserID == req.BankUserID);
//            var userCompanyList = req.CompanyIds.Select(f => new Live_BankUserCompany()
//            {
//                BankUserID = req.BankUserID,
//                BankCompanyID = f,
//                CreateTime = DateTime.Now,
//                ID = Guid.NewGuid(),
//                IsDetail = true,
//                IsEnable = true,
//                IsExecute = true,
//                IsPay = true,
//            }).ToList();
//            await _userCompanyService.Add(userCompanyList);
//            return Ok();
//        }

//        [AllowAnonymous]
//        [HttpGet]
//        public async Task<IActionResult> ListUserCompany(Guid id)
//        {
//            var records = await _userCompanyService.List(f => f.BankUserID == id);
//            return Ok(records);
        
//        }





//    }
//}
