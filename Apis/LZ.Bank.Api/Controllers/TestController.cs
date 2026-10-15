using com.sun.security.ntlm;
using java.security;
using java.time.format;
using LZ.ApiService.Interface;
using LZ.Bank.Service.Interface;
using LZ.Bank.Service.Services;
using LZ.Common;
using LZ.Core;
using LZ.Core.Attributes;
using LZ.Core.EventData;
using LZ.Model;
using LZ.Model.Bank.Apip;
using LZ.Model.Bank.CZY.ParamSetModels;
using LZ.Model.Enums;
using LZ.Model.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using sun.awt.image;
using System.Runtime.InteropServices;
using System.Text;
using static QRCoder.PayloadGenerator;

namespace LZ.Bank.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [AllowAnonymous]
    public class TestController : ApiControllerBase
    {
        private readonly ILogger<TestController> _logger;

        private readonly IWebHostEnvironment _env;
        private readonly IAliyunCommonService _aliyunCommonService;
        private readonly IFxqService _fxqService;
        private readonly ICzyService _cayservice;
        private readonly IBankTransactionService _transService;
        private readonly IConfiguration _configuration;
        private readonly ICzyApipService _czyApipService;
        public TestController(ILogger<TestController> logger, IWebHostEnvironment env, IAliyunCommonService aliyunCommonService, IFxqService fxqService, ICzyService cayservice, IBankTransactionService transService, IConfiguration configuration, ICzyApipService czyApipService)
        {
            _logger = logger;
            _env = env;
            _aliyunCommonService = aliyunCommonService;
            _fxqService = fxqService;
            _cayservice = cayservice;
            _transService = transService;
            _configuration = configuration;
            _czyApipService = czyApipService;
        }


        [HttpPost("TestAesTest")]
        public IActionResult TestAesTest()
        {
            const string plainText = "Hello";
            byte[] aesKey = CryptoHelper.GenerateAesKey();
            string cipher = CryptoHelper.AesGcmEncrypt(aesKey, plainText);

            string publicPem = System.IO.File.ReadAllText(Path.Combine(_env.ContentRootPath, "RsaKeys", "rsa_public_key.pem"));
            string privatePem = System.IO.File.ReadAllText(Path.Combine(_env.ContentRootPath, "RsaKeys", "rsa_private_key_pkcs8.pem"));
            string wrappedKey = CryptoHelper.RsaWrapAesKey(publicPem, aesKey);
            byte[] unwrappedKey = CryptoHelper.RsaUnwrapAesKey(privatePem, wrappedKey);
            string decrypted = CryptoHelper.AesGcmDecrypt(unwrappedKey, cipher);
            // 提交测试

            return Ok(new
            {
                plainText,
                cipher,
                wrappedKey,
                decrypted,
                rsaRoundTrip = unwrappedKey.AsSpan().SequenceEqual(aesKey)
            });
        }

        public record AesDecryptTestRequest(string Cipher, string WrappedKey);

        [HttpPost("TestAesDecrypt")]
        public IActionResult TestAesDecrypt(AesDecryptTestRequest request)
        {
            string privatePem = System.IO.File.ReadAllText(Path.Combine(_env.ContentRootPath, "RsaKeys", "rsa_private_key_pkcs8.pem"));
            byte[] aesKey = CryptoHelper.RsaUnwrapAesKey(privatePem, request.WrappedKey);
            string plainText = CryptoHelper.AesGcmDecrypt(aesKey, request.Cipher);
            return Ok(new { plainText });
        }

        [HttpPost("TestAesGmc")]
        public async Task<IActionResult> TestAesGmc()
        {
            byte[] keys = AesGcmHelper.GenerateSecureKey();
            string str1 = "Hello";

          var str2=  AesGcmHelper.Encrypt(str1, keys);


          var str3=  AesGcmHelper.Decrypt(str2, keys);





            return Ok(str3);
        }


        [HttpPost("T0001")]
        public async Task<IActionResult> T0001()
        {
            var res = await _czyApipService.QueryTransactionHistory(AccountQhisdRequestV1.AccountQhisdRequestParamSet.Sample());
            return Ok(res);
        }

        [HttpPost("T0004")]
        public async Task<IActionResult> T0004()
        {
            var res = await _czyApipService.QueryPaymentStatus(QueryPaymentStatusRequestV1.QueryPaymentStatusRequestParamSet.Sample());
            return Ok(res);
        }

        [HttpPost("T0005")]
        public async Task<IActionResult> T0005()
        {
            var res = await _czyApipService.QueryAccountInfo(AccountQacctinfRequestV1.AccountQacctinfRequestParamSet.Sample());
            return Ok(res);
        }

        [HttpPost("T0006")]
        public async Task<IActionResult> T0006()
        {
            var res = await _czyApipService.QueryAccountBalance(AccountQaccbalRequestV1.AccountQaccbalRequestParamSet.Sample());
            return Ok(res);
        }

        [HttpPost("T0014")]
        public async Task<IActionResult> T0014()
        {
            var res = await _czyApipService.QueryBankBranch(BankQuerybranchRequestV1.BankQuerybranchRequestParamSet.Sample());
            return Ok(res);
        }

        [HttpPost("T0023")]
        public async Task<IActionResult> T0023()
        {
            var res = await _czyApipService.QueryClaimResult(ClaimQryclmrstRequestV1.ClaimQryclmrstRequestParamSet.Sample());
            return Ok(res);
        }

        [HttpPost("T0039")]
        public async Task<IActionResult> T0039()
        {
            var res = await _czyApipService.QuerySingleElectronicReceipt(QuerySingleElectronicReceiptRequestV1.QuerySingleElectronicReceiptRequestParamSet.Sample());
            return Ok(res);
        }

        [HttpPost("T1002")]
        public async Task<IActionResult> T1002()
        {
            var res = await _czyApipService.PaymentOrdinaryPay(PaymentOrdinarypayRequestV1.PaymentOrdinarypayRequestParamSet.Sample());
            return Ok(res);
        }

        [HttpPost("T1003")]
        public async Task<IActionResult> T1003()
        {
            var res = await _czyApipService.BillBatchPrivate(BillBatchPrivateRequestV1.BillBatchPrivateRequestParamSet.Sample());
            return Ok(res);
        }

        [HttpPost("T1018")]
        public async Task<IActionResult> T1018()
        {
            var res = await _czyApipService.PaymentBatchPay(PaymentBatchpayRequestV1.PaymentBatchpayRequestParamSet.Sample());
            return Ok(res);
        }

        [HttpPost("T1019")]
        public async Task<IActionResult> T1019()
        {
            var res = await _czyApipService.QueryBankReconciliation(QueryBankreconciliationRequestV1.QueryBankreconciliationRequestParamSet.Sample());
            return Ok(res);
        }

        [HttpPost("T1022")]
        public async Task<IActionResult> T1022()
        {
            var res = await _czyApipService.PaymentCommonPay(PaymentCommonpayRequestV1.PaymentCommonpayRequestParamSet.Sample());
            return Ok(res);
        }

        [HttpPost("T1028")]
        public async Task<IActionResult> T1028()
        {
            var res = await _czyApipService.QueryElectronicReceipt(BillQueryelectronicreceiptRequestV1.BillQueryelectronicreceiptRequestParamSet.Sample());
            return Ok(res);
        }





        [HttpGet]
        public async Task<IActionResult> GetEnv()
        {
            AesCBCHelper.GenerateKeyAndIV(out var keyByte,out var ivByte);
            return Ok(new
            {
                keyByte = Convert.ToHexString(keyByte),
                ivByte = Convert.ToHexString(ivByte),
            });
        }

        public record Info(string data, string iv, string key, string signature, long timestamp, string nonce);
        [CzyTransactionFilter]
        [HttpPost]
        public async Task<IActionResult> Test(Info payload)
        {

            RsaPemHelper rsaHelper = new RsaPemHelper("MIICdgIBADANBgkqhkiG9w0BAQEFAASCAmAwggJcAgEAAoGBAMi59ewr1fTijNbjjh82TI1LafJ7a4HGkw8GhV5GOglq1/dMLXLFaIKD5AL8Pcbbft15DwYcOyNCHukbNl2ov4GxI03gTlpy8Y2qD8QeCMusnD0V0qdJikBfjqFT6bnnxpjxDghuoq9RWAz6exAvEg4fdEMRK+W5OnhnvQcAPwbtAgMBAAECgYAYbpIhagCD6yNrcI02/Vv6Asjli+I9DJePx6JqJDAiP1AEONB7XM3ovPb+6gb28naRrI+Ild0Y4I50nyxR4B1XC5wTjBk3nPE3TZarS2fT/pK6z3PwIuspzxEikykTrBCc055ligXLDBqE8TOY+HKTs0W1clRmza7Nq8SNOgc6gQJBAO/VO6fKKfZtoHZtKfgy5Nd2U8J/psbuVI1Bo+z9OR4Z1Q6DCuuKk/4IYdSIhvMkQIN4PHz6QAaxo7SwICdUmo0CQQDWQd7+801VDB5G7GsIsRxyJv2Bcuo17lfFwSSsai6H4Ge1Ycdb5hrQtagDHCd6x7R0LUvdZrk9J1K9UBIvdDXhAkBQIOX1gU3Tz3y1FHPZQOfENPx3Fz8ZZ7dIGo4yN8hFjkvotU+IspwPW62T3Kl5wrrrMdjDyLhrhB412EJl4ZZhAkEAsNjiBTIxrHc6LnRJzHzT6+z+5fBCHY3DaPkJGj7ydTjmoTI56UK+83JdYWPFunRIIHSa3/4/+5gPlS8YM48KQQJAcSOfhS9OyjSaZOBK1tx22ruyEV4W5ovA6uCbp3kTiLwxoWDYXmin8h/UGVTcGclm+4HGQd7wA22QN1dkB2UJ2g==", "");
            var aesKeyHex = rsaHelper.Decrypt(payload.key);
            Console.WriteLine($"aesKeyHex: {aesKeyHex}");
            // 机密密文
            var ss = AesCBCHelper.Decrypt(payload.data,Convert.FromHexString(aesKeyHex), Convert.FromHexString(payload.iv));
            // 验签
            string content = $"{payload.data}{payload.iv}{payload.timestamp}{payload.nonce}";

           var isb=  AesCBCHelper.VerifyHmac(content, payload.signature, Convert.FromHexString(aesKeyHex));
            return Ok(ss);
        }
        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;
            using var outputStream = new MemoryStream();
            // 假设字体文件在项目根目录的 Fonts 文件夹下
            string fontPath = Path.Combine(_env.WebRootPath, "Fonts", "msyh.ttf");
            // 检查字体文件是否存在，防止 Linux 路径大小写敏感问题
            if (!System.IO.File.Exists(fontPath))
            {
                // 生产环境建议记录日志
                throw new FileNotFoundException("Font file not found: " + fontPath);
            }

            try
            {
                ImageSharp.AddTextWatermark(
                    memoryStream,
                    outputStream,
                    "本人授权单位使用本人信息代申领工商银行借记卡",
                    fontPath,
                    fontSize: 30,
                    colorHex: "#80FFFFFF" // 半透明白色
                                          //colorHex: "#000000" // 半透明白色
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Watermark processing failed: {ex.Message}");
            }

            outputStream.Position = 0;
            return File(outputStream.ToArray(), "image/jpeg", "watermarked.jpg");
        }
    }
}
