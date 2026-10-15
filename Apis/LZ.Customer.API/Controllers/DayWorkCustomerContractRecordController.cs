using LZ.ApiService.Interface;
using LZ.Core.Attributes;
using LZ.Core.Interface;
using LZ.Customer.Service.Interface;
using LZ.Customer.Service.Services;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.FxqModel;
using Microsoft.AspNetCore.Mvc;

namespace LZ.Customer.API.Controllers
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    [PlatformLimit(Model.Enums.PlatformType.LZMini)]
    [PlatformLimit(Model.Enums.PlatformType.Api)]
    public class DayWorkCustomerContractRecordController(IAuthService authService, IFxqDevService fxqDevService, IContractSignerService contractSignerService, IAliyunOSSService aliyunOSSService, IDayWorkCustomerContractRecordService dayWorkCustomerContractRecordService, IJobsDayWorkService jobsDayWorkService, IFxqService fxqService, IContractFileConfigService contractFileConfigService,ICustomerService customerService) : CustomerControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetCustomerContract(int id)
        {
            var model = await dayWorkCustomerContractRecordService.Get(t => t.CustomerID == id && (t.Status == 1 || t.Status == 2));
            if (model == null)
            {
                throw new ErrorCodeException(-1, "合同查询不到");
            }

            return Ok(new {model});
        }
        [HttpPost]
        public async Task<IActionResult> CustomerContractSign([FromForm] int id)
        {
            string ali = "https://oss.cn-labor.cn";
            //var customer = await customerService.Get(customerID);
            var contract = await dayWorkCustomerContractRecordService.Get(id);
            var signCompanyList = await contractSignerService.List(t => t.Idno == contract.CompanyNo || t.Idno == "91330212MA2CH79K4J");
            var templateContract = await contractFileConfigService.GetNav(t => t.ContractEnumType == ContractEnumType.灵活用工客户合同);
            
            var jia = signCompanyList.Find(t => t.Idno == contract.CompanyNo);
            var yi = signCompanyList.Find(t => t.Idno == "91330212MA2CH79K4J");
            if(jia==null || yi==null)
            {
                throw new ErrorCodeException(-1, "甲方或者乙方查询不到盖章文件");
            }
            string contractBase64 = "";
            using (HttpClient client = new HttpClient())
            {
                var imageBytes = await client.GetByteArrayAsync(contract.TemplateContract);
                contractBase64 = Convert.ToBase64String(imageBytes);
            }
            KeywordSigner[] keys = new KeywordSigner[2];
            keys[0] = new KeywordSigner(jia.Name, jia.Idno, jia.OssUrl, templateContract.Signers[0].SignKeyword, 120, null);
            keys[1] = new KeywordSigner(yi.Name, yi.Idno, yi.OssUrl, "乙方（盖章）", 120, null);
            var result = await fxqDevService.CombinePdfSigning(contractBase64, keys);
            contract.ContractTransactionId = result.Data.TransactionId;
            contract.ContractHash = result.Data.Hash;
            contract.SignPath = jia.OssUrl;
            contract.SignerIdNo = jia.Idno;
            contract.SignerName = jia.Name;
            contract.SignKeyword = templateContract.Signers[0].SignKeyword;
            contract.Status = 2;
            using (var ms = new MemoryStream(Convert.FromBase64String(result.Data.PdfBase64)))
            {

                var ressd = await aliyunOSSService.UploadFile(ms, "客户合同盖章.pdf", true, false, "temp");
                if (ressd.Code != 0)
                {
                    throw new ErrorCodeException(-1, "上传盖章合同错误");
                }
                contract.CompanyContract = ali + ressd.Data.ToString();
            }
            await dayWorkCustomerContractRecordService.Update(contract);
            return Ok(new { keys});
        }
    }
}
