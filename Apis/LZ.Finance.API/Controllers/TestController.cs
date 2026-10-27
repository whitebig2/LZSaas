using Aliyun.OSS.Util;
using LZ.ApiService.Interface;
using LZ.Common;
using LZ.Core;
using LZ.Core.EventData;
using LZ.Core.EventData.FxqEventData;
using LZ.Core.EventData.Salary;
using LZ.Core.Interface;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Core.Redis;
using LZ.Core.Services;
using LZ.Finance.Service.Interface;
using LZ.Model;
using LZ.Model.Dtos;
using LZ.Model.Enums;
using LZ.Model.ExcelTemplateModel;
using LZ.Model.Exceptions;
using LZ.Model.IExcelTemplate;
using LZ.Model.IExcelTemplate.Banks;
using LZ.Model.ModelExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NPOI.SS.Formula.Functions;
using Polly;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
using System.Text;

namespace LZ.Finance.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    public class TestController : ControllerBase
    {
        public record Person(string Name,string Sex,int Age);

        private IHttpService   _httpService;
        private IAuthService  _authService;
        private ICurrentUser _currentUser;
        private IFxqService _fxqService;    
        private IRedisService  _redisService;
        private IAliyunOSSService   _aliyunOSSService;
        private IFinanceOtherService    _financeOtherService;
        private IEventPublisher _publisher    ;
        private ISalaryCustomerService _salaryCustomerService    ;
        private IFinanceOtherService _otherService    ;
        private ISalaryRecordService _salaryRecordService    ;
        private IBaiWangService _baiWangService;

        public TestController(IFxqService fxqService, IRedisService redisService, IAliyunOSSService aliyunOSSService, IFinanceOtherService financeOtherService, IEventPublisher publisher, ISalaryCustomerService salaryCustomerService, IFinanceOtherService otherService, ICurrentUser currentUser, IAuthService authService, IHttpService httpService, ISalaryRecordService salaryRecordService, IBaiWangService baiWangService)
        {
            _fxqService = fxqService;
            _redisService = redisService;
            _aliyunOSSService = aliyunOSSService;
            _financeOtherService = financeOtherService;
            _publisher = publisher;
            _salaryCustomerService = salaryCustomerService;
            _otherService = otherService;
            _currentUser = currentUser;
            _authService = authService;
            _httpService = httpService;
            _salaryRecordService = salaryRecordService;
            _baiWangService = baiWangService;
        }





        [HttpPost]
        public async Task<IActionResult> BaiwangTest()
        {

       

          //await  _baiWangService.TaxEnterpriseInfoQuery("", "");

            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> HttpServiceTest11(IFormFile file)
        {
            //using MemoryStream ms = new MemoryStream();
            //file.CopyTo(ms);
            //ms.Position = 0;    
            //await _aliyunOSSService.UploadOssFile(ms, $"ICBC/Plkh/{file.FileName}");

            var customerRes = await _httpService.GetAsync<Live_V_Customer>(ServiceNames.CustomerService, $"/api/Customer/GetByName?name=宁波海连科电气技术有限公司&companyID=1");

            return Ok(customerRes);
        }




        [HttpPost]
        public async Task<IActionResult> SalaryRecordOverTest(IFormFile file)
        {
            // 当月月初
            var taxMonth = DateTime.Now.Date.AddDays(1 - DateTime.Today.Day);
            HttpContext.Request.Headers.TryGetValue("authorization", out var value);
            List<SalaryRecordExportExcelTemplate> excelRecords = new List<SalaryRecordExportExcelTemplate>();

            using (MemoryStream ms = new MemoryStream())
            {
                file.CopyTo(ms);
                excelRecords = NpoiExcelHelper.ReadExcelFile<SalaryRecordExportExcelTemplate>(ms, "");
            }


            foreach (var item in excelRecords)
            {

                int batch = 0;

                for (int i = 0; i < 100; i++)
                {
                    if (item.SalaryBatchType.Contains(i.ToString()))
                    {
                        batch=i;
                        break;
                    }
                }

                var salaryMonth = DateTime.Parse(item.SalaryMonth);

               var record=await _salaryRecordService.GetView(f => f.SalaryBatchType == batch && f.CustomerName == item.CustomerName && f.SalaryMonth == salaryMonth && f.IdCardNo == item.IdCardNo && f.Name == item.Name && f.UnitSalaryAmount == item.UnitSalaryAmount);

                if (record == null)
                {
                    Console.WriteLine($"{item.Name}获取工资信息失败");
                    continue;
                }
                else
                    Console.WriteLine(item.Name);

                var model = await _salaryRecordService.Get(f => f.ID == record.ID);
                model.TaxTimes = item.TaxTimes;
                model.TaxMonthAmount = item.TaxMonthAmount;
                model.BeforeTaxSalaryTotalAmount = item.BeforeTaxSalaryTotalAmount;
                model.SpBasicTotalAmount = item.TaxSpBasicTotalAmount;
                model.TaxSalaryTotalAmount = item.TaxSalaryTotalAmount;
                model.TaxTotalAmount = item.TaxTotalAmount;
                model.LastTaxTotalAmount = item.LastTaxTotalAmount;

             await   _salaryRecordService.Update(model);


            }

            return Ok(excelRecords);
        }


        public record ListCustomerRecord(List<Live_V_Customer> items, int count);

        [HttpPost]
        public async Task<IActionResult> HttpServiceTest()
        {
          bool isb=  await _redisService.SetAsync("icbcSendLock", DateTime.Now.ToString("yyyy-MM-dd"), -1, CSRedis.RedisExistence.Nx);

            return Ok(isb);
        }


            [HttpPost]
        public async Task<IActionResult> YouZhengExcelTest()
        {
            //var fileBase64 = await _aliyunOSSService.DownloadFile("https://oss.cn-labor.cn//files/20251020/b01953efbe824d038fdff778cbe7edd3.xlsx");
            var fileBase64 = await _aliyunOSSService.DownloadFile(NpoiExcelHelper.YouZhengTempUri);
            YouZhengYinHangExcelTemplate zhaoShang = new YouZhengYinHangExcelTemplate()
            {
                BankNo = "6214855740102757",

                BankUserName = "刘伯川",
                Remark = "欧贝汽车5月",
                TransactionAmount = 1984.74m,
                BankNameNo = "BankNameNo",
                IdCardNo = "IdCardNo",
                IdType = "IdType",
                IsYouZhengBank = "IsYouZhengBank",
                SignNo = "SignNo"
            };

            YouZhengYinHangExcelTemplate zhaoShang1 = new YouZhengYinHangExcelTemplate()
            {
                BankNo = "6214855740102757",

                BankUserName = "刘伯川",
                Remark = "欧贝汽车5月",
                TransactionAmount = 1984.74m,
                BankNameNo = "BankNameNo",
                IdCardNo = "IdCardNo",
                IdType = "IdType",
                IsYouZhengBank = "IsYouZhengBank",
                SignNo = "SignNo"
            };
            var fileByteArry = NpoiExcelHelper.WriteYouZhengYinHang(new List<YouZhengYinHangExcelTemplate>() { zhaoShang, zhaoShang1 }, fileBase64);
            return File(fileByteArry, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "邮政银行.xlsx");
        }


        //[HttpPost]
        //public async Task<IActionResult> MinShengExcelTest()
        //{
        //    //var fileBase64 = await _aliyunOSSService.DownloadFile("https://oss.cn-labor.cn//files/20251020/c2dd428d73ff4cb68d2a8385cc2747e6.xls");

        //    var fileBase64 = await _aliyunOSSService.DownloadFile(NpoiExcelHelper.MinShengTempUri);
        //    MinShengYinHangItemExcelTemplate zhaoShang = new MinShengYinHangItemExcelTemplate()
        //    {
        //        BankNo = "6214855740102757",
        //        BankNameNo = "BankNameNo",
        //        BankUserName = "刘伯川",
        //        Remark = "欧贝汽车5月",
        //        TransactionAmount = 1984.74m,
        //        Summary = "1月工资"
        //    };

        //    MinShengYinHangItemExcelTemplate zhaoShang1 = new MinShengYinHangItemExcelTemplate()
        //    {
        //        BankNo = "6214855740102757",
              
        //        BankNameNo = "BankNameNo",
          
        //        BankUserName = "刘伯川",
        //        Remark = "欧贝汽车5月",
        //        TransactionAmount = 1984.74m,
           
        //        Summary = "1月工资",
                   
        //    };

        //    MinShengYinHangExcelTemplate minSheng = new MinShengYinHangExcelTemplate() { 
        //     Items=new List<MinShengYinHangItemExcelTemplate>() { zhaoShang , zhaoShang1 }
        //    };
        //    var fileByteArry = NpoiExcelHelper.WriteMinShengYinHang(minSheng, fileBase64);
        //    return File(fileByteArry, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "民生银行.xls");
        //}


        [HttpPost]
        public async Task<IActionResult> JianSheExcelTest()
        {
            //var fileBase64 = await _aliyunOSSService.DownloadFile("https://oss.cn-labor.cn//files/20251020/9b233d435c4342aca5dbd3490232a51b.xls");
            var fileBase64 = await _aliyunOSSService.DownloadFile(NpoiExcelHelper.JianSheTempUri);
            JianSheYinHangExcelTemplate zhaoShang = new JianSheYinHangExcelTemplate()
            {
                BankNo = "6214855740102757",
                BankName = "招商银行",
                BankNameNo = "BankNameNo",
                OrderNo = 1,
                BankUserName = "刘伯川",
                Remark = "欧贝汽车5月",
                TransactionAmount = 1984.74m,
                 OtherBankTag=string.Empty,
                  Summary="1月工资"
            };

            JianSheYinHangExcelTemplate zhaoShang1 = new JianSheYinHangExcelTemplate()
            {
                BankNo = "6214855740102757",
                BankName = "招商银行",
                BankNameNo = "BankNameNo",
                OrderNo = 1,
                BankUserName = "刘伯川",
                Remark = "欧贝汽车5月",
                TransactionAmount = 1984.74m,
                OtherBankTag = string.Empty,
                Summary = "1月工资"
            };
            var fileByteArry = NpoiExcelHelper.WriteJianSheYinHang(new List<JianSheYinHangExcelTemplate>() { zhaoShang, zhaoShang1 }, fileBase64);
            return File(fileByteArry, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "建设银行.xls");
        }

        [HttpPost]
        public async Task<IActionResult> ZhaoShangExcelTest()
        {
            //var fileBase64 = await _aliyunOSSService.DownloadFile("https://oss.cn-labor.cn//files/20251020/5d8fdd914c81463f94571b75f8d0bc62.xlsx");
            var fileBase64 = await _aliyunOSSService.DownloadFile(NpoiExcelHelper.ZhaoShangTempUri);
            ZhaoShangYinHangExcelTemplate zhaoShang = new ZhaoShangYinHangExcelTemplate()
            {
                BankNo = "6214855740102757",
                BankName = "招商银行",
                BankAddreess = "",
                BankUserName = "刘伯川",
                Remark = "欧贝汽车5月",
                TransactionAmount = 1984.74m
            };
            var fileByteArry = NpoiExcelHelper.WriteZhaoShangYinHang(new List<ZhaoShangYinHangExcelTemplate>() { zhaoShang }, fileBase64);
            return File(fileByteArry, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "招商银行.xlsx");
        }




        [HttpPost]
        public async Task<IActionResult> UploadExcelTest(IFormFile file)
        {




            


            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var excelRecordList = NpoiExcelHelper.ReadExcelKV(ms);
                return Ok(excelRecordList);
            }
        }



        [HttpPost]
        public async Task<IActionResult> SalaryTest()
        {

            var info = await _authService.UserInfo<Model.SYS_CompanyUser>();

            if (info == null)
            {
                throw new ErrorCodeException(-1, "授权信息获取失败");
            }

            // var resp = await _otherService.BatchTransactionDetail(Guid.Parse("8083C7E7-6F19-4EB1-8F0A-AA067CEC94FB"));

            //var  ss= Newtonsoft.Json.JsonConvert.SerializeObject(resp.Result.resultList.FirstOrDefault(f=>f.payeeAcctName== "卢艳"));


            //var resp = await _otherService.SingleTransactionDetail(Guid.Parse("61571BF0-1D54-489F-BB3C-352A405B739C"));
            return Ok(info);
        }


        [HttpPost]
        public async Task<IActionResult> CmbcTransaBatchJobEventData()
        {
            CmbcTransaBatchJobEventData eventData = new CmbcTransaBatchJobEventData();

            _publisher.Publish(eventData);
            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> CacheTets()
        {
            var context = HttpContext;
            var ip = context.Connection.RemoteIpAddress?.ToString();
            var cacheKey = $"{ip}_count11";
            var isb =await  _redisService.SetAsync(cacheKey, "11", TimeSpan.FromSeconds(5), CSRedis.RedisExistence.Nx);
            return Ok(isb);
        }


            [HttpPost]
        public async Task<IActionResult> tttt111(IFormFile file)
        {

            using (var ms=new MemoryStream())
            {
                file.CopyTo(ms);
                var kvList = NpoiExcelHelper.ReadExcelKV(ms);
                if (kvList==null|| kvList.Count<=0)
                {
                    //读取失败
                }

                // 或表头
                var header = kvList.First();
            



                int customerIndex = 0;
                int salaryMonthIndex = 0;
                int nameIndex = 0;
                int idCardNoIndex = 0;
                if (header.Any(f => f.Value == "客户单位"))
                {
                    customerIndex = header.First(f => f.Value == "客户单位").Key;
                }
                else
                {
                    throw new ErrorCodeException(-1, "表头信息缺少【客户单位】");
                }
                if (header.Any(f => f.Value == "工资所属月"))
                {
                    salaryMonthIndex = header.First(f => f.Value == "工资所属月").Key;
                }
                else
                {
                    throw new ErrorCodeException(-1, "表头信息缺少【工资所属月】");
                }
                if (header.Any(f => f.Value == "姓名"))
                {
                    nameIndex = header.First(f => f.Value == "姓名").Key;
                }
                else
                {
                    throw new ErrorCodeException(-1, "表头信息缺少【姓名】");
                }
                if (header.Any(f => f.Value == "身份证号码"))
                {
                    idCardNoIndex = header.First(f => f.Value == "身份证号码").Key;
                }
                else
                {
                    throw new ErrorCodeException(-1, "表头信息缺少【身份证号码】");
                }
                var headerJson = Newtonsoft.Json.JsonConvert.SerializeObject(header);
                var headerBytes = Encoding.UTF8.GetBytes(headerJson);
                var headerBase64 = Convert.ToBase64String(headerBytes);

                // 获取客户单位
                var content = kvList.Last();


                //Live_SalarySlipExtension salarySlip = new Live_SalarySlipExtension()
                //{
                //    Createtime = DateTime.Now,
                //    CustomerName = customerName,
                //    IsNotice = false,
                //    SalaryMonth = Convert.ToDateTime(salaryMonth),
                //    TotalNum = kvList.Count - 1,
                //    HeaderKV = headerBase64,
                //};
                kvList.RemoveAt(0);

                List<Live_SalarySlipRecord> records = new List<Live_SalarySlipRecord>();

                Live_SalarySlipRecord slipRecord;

                foreach (var kv in kvList)
                {
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(kv);
                    var jsonBytes = Encoding.UTF8.GetBytes(headerJson);
                    slipRecord = new Live_SalarySlipRecord()
                    {
                        CustomerName = kv[customerIndex],
                        SalaryMonth = Convert.ToDateTime(kv[salaryMonthIndex]),
                        IdCardNo = kv[idCardNoIndex],
                        Name = kv[nameIndex],
                        ContentKV = Convert.ToBase64String(headerBytes),
                        IsNotice = false,
                    };
                    records.Add(slipRecord);
                }

                var groups = records.GroupBy(f => new { f.CustomerName, f.SalaryMonth }).ToList();
                if (groups.Count > 1)
                {
                    throw new ErrorCodeException(-1, "不支持多客户单位或多工资所属月导入");
                }

                var group = groups.First();

                Live_SalarySlipExtension salarySlip = new Live_SalarySlipExtension()
                {

                    CreateTime = DateTime.Now,
                    CustomerName = group.Key.CustomerName,
                    IsNotice = false,
                    SalaryMonth = group.Key.SalaryMonth,
                    TotalNum = kvList.Count - 1,
                    HeaderKV = headerBase64,
                    CustomerGID = Guid.Empty
                };

                return Ok(salarySlip);

            }

        }



        [HttpPost]
        public async Task<IActionResult> tttt(string name)
        {

            //var entity = await _financeOtherService.SingleTransactionDetail(Guid.NewGuid());
            CmbcTransaBatchJobEventData cmbcTransaBatchJobEventData = new CmbcTransaBatchJobEventData();

            _publisher.Publish(cmbcTransaBatchJobEventData);


            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> ttt111t(string name)
        {

            //var entity = await _financeOtherService.SingleTransactionDetail(Guid.NewGuid());
            CmbcTransaSignleJobEventData cmbcTransaBatchJobEventData = new CmbcTransaSignleJobEventData();

            _publisher.Publish(cmbcTransaBatchJobEventData);


            return Ok();
        }

        

            [HttpPost]
        public async Task<IActionResult> GetEnterpriseByNameAsync(string name)
        {

          var entity=await  _financeOtherService.GetEnterpriseByNameAsync(name);
            return Ok(entity);
        }
        [HttpPost]
        public async Task<IActionResult> GetCustomerByNameAsync(string name)
        {

            var entity = await _financeOtherService.GetCustomerByNameAsync(name);
            return Ok(entity);
        }
        [HttpPost]
        public async Task<IActionResult> GetEnterpriseByIdAsync(Guid id)
        {
            var entity = await _financeOtherService.GetEnterpriseByIdAsync(id);
            return Ok(entity);
        }

        

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {

            var fileRes = await _aliyunOSSService.UploadFile(file);
            return Ok(fileRes);
        }


        [HttpGet]
        public async Task DeleteToken()
        {
          await _redisService.DeleteAsync("fxqAccessToken");
        }

        [HttpGet]
        public async Task KeywordReplace()
        {
            string path = @"C:\Users\SayHi\Desktop\放心版测试数据\关键字替换.docx";
            var fileBytes = System.IO.File.ReadAllBytes(path);
            var fileBase64 = Convert.ToBase64String(fileBytes);
            var none = Guid.NewGuid().ToString("N");
            Person person = new Person("张三", "男", 18);
            //var resp = await _fxqService.DocxKeywordReplace<Person>(fileBase64, none, person);
        }


        [HttpGet]
        public async Task<IActionResult> RealName2()
        {
            var resp = await _fxqService.IdentityAuthentication2("郑佳鑫", "500102200410061766");
            return Ok(resp);
        }


        [HttpGet]
        public async Task<IActionResult> PrepaidApplyTest()
        {

            PrepaidApplyEventData eventData = new PrepaidApplyEventData() { 
             ApplyAmount=1,
              ApplyType= Model.Enums.PrepaidApplyEnumType.日结工资申请,
               CreateTime= DateTime.Now,
                UserEntryID=2
            };
          _publisher.Publish(eventData);
            return Ok();
        }
    }
}
