using AutoMapper;
using LZ.Common;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Finance.Service.Interface;
using LZ.Model;
using LZ.Model.Dtos.SettlementDtos;
using LZ.Model.Enums;
using LZ.Model.ExcelTemplateModel;
using LZ.Model.Exceptions;
using LZ.Model.IExcelTemplate;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Senparc.CO2NET.Extensions;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
using SqlSugar;
using System.Text;

namespace LZ.Finance.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    public class SalarySlipController : ApiControllerBase
    {

        private readonly IFinanceOtherService  _otherService   ;
        private readonly ISalarySlipService   _salarySlipService;
        private readonly IMapper    _mapper;
        private readonly IThirdSalarySlipService    _thirdSalarySlipService;
        private readonly IAuthService _authService;

        public SalarySlipController(ISalarySlipService salarySlipService, IMapper mapper, IFinanceOtherService otherService, IThirdSalarySlipService thirdSalarySlipService, IAuthService authService)
        {
            _salarySlipService = salarySlipService;
            _mapper = mapper;
            _otherService = otherService;
            _thirdSalarySlipService = thirdSalarySlipService;
            _authService = authService;
        }

        [HttpGet]
        public async Task<IActionResult> PageList([FromQuery] PagedModelBase query)
        {
            var exp = Expressionable.Create<Model.Live_V_SalarySlip>();

            var user = CurrentUser;
            SYS_CompanyUser info = null;

            if (user.PlatformType != PlatformType.Admin && user.PlatformType != PlatformType.Api)
            {
                if (info == null)
                {
                    info = await _authService.UserInfo<Model.SYS_CompanyUser>();
                }

                if (info != null && info.UserType == 4 && !string.IsNullOrWhiteSpace(info.RoleIDS) && info.RoleIDS.Contains("3AD4CA6E-AA01-4824-A223-8B671C41F557".ToLower()))
                {
                    exp.And(it => it.CompanyID == user.CompanyID || it.CustomerName == "宁波君禾清洁科技有限公司");
                }
                else
                {
                    exp.And(it => it.CompanyID == user.CompanyID);
                }
            }
          
            exp.AndIF(!string.IsNullOrWhiteSpace(query.Keyword), f => f.CustomerName.Contains(query.Keyword));
            var models = await _salarySlipService.PageList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }

        [HttpGet]
        public async Task<IActionResult> GetNav(Guid id)
        {
            var model = await _salarySlipService.GetNav(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return Ok(model);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] Guid id)
        {
            var model = await _salarySlipService.GetNav(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            await _salarySlipService.DeleteNav(model);
            return Ok();
        }

      
        [HttpPost]
        public async Task<IActionResult> UploadSalarySlip(IFormFile file)
        {

            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var kvList = NpoiExcelHelper.ReadExcelKV(ms);
                if (kvList == null || kvList.Count <= 0)
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
                kvList.RemoveAt(0);

                List<Live_SalarySlipRecord> records = new List<Live_SalarySlipRecord>();

                Live_SalarySlipRecord slipRecord;

                foreach (var kv in kvList)
                {
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(kv);
                    var jsonBytes = Encoding.UTF8.GetBytes(json);
                    slipRecord = new Live_SalarySlipRecord()
                    {
                        CustomerName = kv[customerIndex],
                        SalaryMonth = Convert.ToDateTime(kv[salaryMonthIndex]),
                        IdCardNo = kv[idCardNoIndex],
                        Name = kv[nameIndex],
                        ContentKV = Convert.ToBase64String(jsonBytes),
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
                    CustomerGID = Guid.Empty,
                    Records = records,
                };
              var resp=await  _otherService.GetCustomerByNameAsync(salarySlip.CustomerName.Trim());
                if (resp != null && resp.Code == 200)
                {
                    salarySlip.CustomerGID = resp.Data.CustomerGID.Value;
                }
                else {

                    throw new ErrorCodeException(-1, $"{salarySlip.CustomerName} 客户单位信息去获取失败");
                }

                await _salarySlipService.AddNav(salarySlip);
                return Ok();

            }

        }

        [HttpPost]
        public async Task<IActionResult> DownloadSalarySlip([FromForm]Guid id)
        {

            var model = await _salarySlipService.GetNav(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            Dictionary<int, string> headerDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, string>>(Encoding.UTF8.GetString(Convert.FromBase64String(model.HeaderKV)));
            List<Dictionary<int, string>> dss = new List<Dictionary<int, string>>();
            dss.Add(headerDic);
            foreach (var item in model.Records)
            {
                Dictionary<int, string> contentDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, string>>(Encoding.UTF8.GetString(Convert.FromBase64String(item.ContentKV)));
                dss.Add(contentDic);
            }
            var fileBytes = NpoiExcelHelper.WriteExcelKV(dss);
            string base64String = Convert.ToBase64String(fileBytes);
            return Ok(new { base64String, fileName = "工资条.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });

        }


        [HttpPost]
        public async Task<IActionResult> DownloadTemplate()
        {
            using var stream = new MemoryStream();
            NpoiExcelHelper.WriteExcelTemplateFile<SalarySlipRecordExcelTemplate>(stream, "工资条模板");
            byte[] fileBytes = stream.ToArray();
            string base64String = Convert.ToBase64String(fileBytes);
            return Ok(new { base64String, fileName = "工资条模板.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }


        [HttpGet]
        public async Task<IActionResult> GetByName(string name,DateTime? salaryMonth)
        {

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ErrorCodeException(-1,"姓名参数信息缺失");
            }

            if (salaryMonth==null)
            {
                throw new ErrorCodeException(-1, "工资所属月参数信息缺失");
            }


            var slipRecord = await _salarySlipService.GetSalarySlipRecord(f=>f.Name==name&&f.SalaryMonth.Value.ToString("yyyy-MM")== salaryMonth.Value.ToString("yyyy-MM"));
            if (slipRecord != null)
            {
                var slipModel = await _salarySlipService.Get(f => f.ID == slipRecord.SalarySlipID);
                if (slipModel != null)
                {
                    Dictionary<int, string> headerDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, string>>(Encoding.UTF8.GetString(Convert.FromBase64String(slipModel.HeaderKV)));
                    //List<Dictionary<int, string>> dss = new List<Dictionary<int, string>>();
                    //dss.Add(headerDic);

                    Dictionary<int, string> contentDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, string>>(Encoding.UTF8.GetString(Convert.FromBase64String(slipRecord.ContentKV)));

                    Dictionary<string, string> slipDic = new Dictionary<string, string>();
                    foreach (var d in headerDic)
                    {
                        if (!slipDic.ContainsKey(d.Value) && contentDic.ContainsKey(d.Key))
                        {
                            slipDic.Add(d.Value, contentDic[d.Key]);
                        }
                    }
                    if (slipDic.Count > 0)
                    {
                        return Ok(slipDic);
                    }
                }
            }
            var thirdRecord = await _thirdSalarySlipService.GetRecord(f => f.Name == name && f.SalaryMonth.Value.ToString("yyyy-MM") == salaryMonth.Value.ToString("yyyy-MM"));
            if (thirdRecord == null)
            {
                throw new ErrorCodeException(-1, "工资条信息未上传，获取信息失败！");
            }
            var thirdSlipDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(thirdRecord.SlipContent);
            return Ok(thirdSlipDic);
        }




    }
}
