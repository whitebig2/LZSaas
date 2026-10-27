using LZ.Common;
using LZ.Core;
using LZ.Finance.Service.Interface;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.ExcelTemplateModel;
using LZ.Model.Exceptions;
using LZ.Model.IExcelTemplate;
using LZ.Model.ModelExtensions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.XSSF.UserModel;
using Quartz.Util;
using Senparc.CO2NET.Extensions;
using SqlSugar;
using SqlSugar.IOC;
using System.Linq;

namespace LZ.Finance.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    public class ThirdSalarySlipController : ApiControllerBase
    {

        private readonly IPlatformNoticeService   _platformNoticeService    ;
        private readonly IUserEntryService   _entryService    ;
        private readonly IFinanceOtherService  _otherService    ;
        private readonly IThirdSalarySlipService  _thirdSalarySlipService    ;

        public ThirdSalarySlipController(IThirdSalarySlipService thirdSalarySlipService, IFinanceOtherService otherService, IUserEntryService entryService, IPlatformNoticeService platformNoticeService)
        {
            _thirdSalarySlipService = thirdSalarySlipService;
            _otherService = otherService;
            _entryService = entryService;
            _platformNoticeService = platformNoticeService;
        }

        [HttpGet]
        public async Task<IActionResult> PageList(string partnerName, string customerName, DateTime? salaryMonth, [FromQuery] PagedModelBase query)
        {
            var exp = Expressionable.Create<Model.Live_ThirdSalarySlip>();
            exp.AndIF(CurrentUser.PlatformType != PlatformType.Admin && CurrentUser.PlatformType != PlatformType.Api, f => f.CompanyID == CurrentUser.CompanyID);
            exp.AndIF(!string.IsNullOrWhiteSpace(customerName), f => f.CustomerName.Contains(customerName));
            exp.AndIF(!string.IsNullOrWhiteSpace(partnerName), f => f.PartnerName.Contains(partnerName));
            exp.AndIF(salaryMonth.HasValue, f => f.SalaryMonth== salaryMonth.Value);
            var models = await _thirdSalarySlipService.PageList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }


        [HttpGet]
        public async Task<IActionResult> PageRecordList(string customerName, string name, string idCardNo, DateTime? salaryMonth, Guid? thirdSalarySlipID, [FromQuery] PagedModelBase query)
        {
            var exp = Expressionable.Create<Model.Live_ThirdSalarySlipRecord>();
            exp.AndIF(thirdSalarySlipID.HasValue, f => f.ThirdSalarySlipID == thirdSalarySlipID.Value);
            exp.AndIF(!string.IsNullOrWhiteSpace(name), f => f.Name.Contains(name));
            exp.AndIF(!string.IsNullOrWhiteSpace(idCardNo), f => f.IdCardNo.Contains(idCardNo));
            exp.AndIF(!string.IsNullOrWhiteSpace(customerName), f => f.CustomerName.Contains(customerName));
            exp.AndIF(salaryMonth.HasValue, f => f.SalaryMonth == salaryMonth.Value);
            var models = await _thirdSalarySlipService.PageRecordList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }



        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> UploadSalarySlip(IFormFile file)
        {
            using (MemoryStream ms = new MemoryStream())
            {

                List<Live_ThirdSalarySlipRecord> records = new List<Live_ThirdSalarySlipRecord>();
                Live_ThirdSalarySlipRecord record = null;

                file.CopyTo(ms);
                var kvList = NpoiExcelHelper.ReadExcelKV(ms);
                if (kvList == null || kvList.Count <= 0)
                {
                    throw new ErrorCodeException(-1, "数据信息读取失败");
                }

                int customerIndex = 0;
                int salaryMonthIndex = 0;
                int nameIndex = 0;
                int idCardNoIndex = 0;
                int socialIndex = 0;

                // 或表头
                var header = kvList.First();


               var sameKey= header.GroupBy(f => f.Value).Where(f => f.Count() > 1).ToList();

                if (sameKey!=null&& sameKey.Count>0)
                {
                    throw new ErrorCodeException(-1, $"表头信息重复：{string.Join("、", sameKey.Select(f=>f.Key))}");
                }


                // 客户单位Index

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


                if (header.Any(f => f.Value == "社保补贴"))
                {
                    socialIndex= header.First(f => f.Value == "身份证号码").Key;
                }
                else {
                    throw new ErrorCodeException(-1, "表头信息缺少【社保补贴】");
                }
                var empRecordList = kvList.Where(f => f != header).ToList();
                foreach (var item in empRecordList)
                {

                    Dictionary<string, string> dssItem = new Dictionary<string, string>();
                    record = new Live_ThirdSalarySlipRecord()
                    {
                        CreateTime = DateTime.Now,
                        ID = Guid.NewGuid(),

                    };
                    record.CustomerName = item[customerIndex];
                    record.Name = item[nameIndex];
                    record.IdCardNo = item[idCardNoIndex];

                    if (string.IsNullOrWhiteSpace(record.CustomerName))
                    {
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(record.IdCardNo))
                    {
                        throw new ErrorCodeException(-1, "身份证号码不能为空");
                    }

                   



                    if (DateTime.TryParse(item[salaryMonthIndex], out DateTime salaryMonth))
                    {
                        record.SalaryMonth = salaryMonth;
                    }
                    if (decimal.TryParse(item[salaryMonthIndex], out decimal socialSubsidy))
                    {
                        record.SocialSubsidy = socialSubsidy;
                    }
                    foreach (var headerItem in header)
                    {

                        if (item.ContainsKey(headerItem.Key))
                        {
                            dssItem.Add(headerItem.Value, item[headerItem.Key]);

                        }
                        else {
                            dssItem.Add(headerItem.Value, string.Empty);
                        }
                    }
                    record.SlipContent=Newtonsoft.Json.JsonConvert.SerializeObject(dssItem);

                    records.Add(record);

                }

                foreach (var item in records)
                {
                    var userEntry = await _entryService.GetView(f => f.CardID == item.IdCardNo.Trim() && f.UserID != null && f.CustomerName == item.CustomerName);
                    if (userEntry != null)
                    {
                        item.UserEntryID = userEntry.ID;
                        item.UserID = userEntry.UserID.Value;
                    }
                }
                if (records.Count != records.Select(f => f.IdCardNo).Distinct().Count())
                {
                    var sameIdCardNos = records.Select(f => f.IdCardNo).GroupBy(f => f).Where(f => f.Count() > 1).Select(f => f.Key);
                    throw new ErrorCodeException(-1, $"检测到身份证号码重复：{string.Join("、", sameIdCardNos)}");
                }

                //if (records.Any(f=> f.UserID<=0))
                //{
                //    throw new ErrorCodeException(-1, $"员工信息匹配失败，：{string.Join("、", records.Where(f => f.UserID <= 0).Select(f=>f.Name))}");
                //}



                var customerGroup= records.GroupBy(f => new { f.CustomerName, f.SalaryMonth }).ToList() ;
                if (customerGroup==null|| customerGroup.Count<=0)
                {
                    throw new ErrorCodeException(-1, "工资条记录获取失败，请检查Excel文件是否为空或者存在隐藏表单");
                }
                if (customerGroup.Count>1)
                {
                    throw new ErrorCodeException(-1, "只能上传一组客户单位与工资所属月的数据，检测到多组数据");
                }

                Live_ThirdSalarySlipExtension salarySlip = new Live_ThirdSalarySlipExtension()
                {
                    CreateTime = DateTime.Now,
                    SalaryMonth = customerGroup.First().Key.SalaryMonth,
                    CustomerName = customerGroup.First().Key.CustomerName,
                    Records= records,
                };

                var cusResp = await _otherService.GetCustomerByNameAsync(salarySlip.CustomerName);

                if (cusResp != null && cusResp.Data != null)
                {
                    Live_V_Customer cus = cusResp.Data;

                    salarySlip.CustomerName = cus.CustomerName;
                    salarySlip.CustomerGID = cus.CustomerGID ?? Guid.Empty;
                    salarySlip.PartnerName = cus.PartnerName;
                    salarySlip.PartnerGID = cus.PartnerGID ?? Guid.Empty;
                    salarySlip.PartnerID = cus.PartnerID ?? 0;
                    salarySlip.CompanyID = cus.CompanyID ?? 0;
                    salarySlip.CompanyGID = cus.CompanyGID ?? Guid.Empty;
                    await _thirdSalarySlipService.AddNav(salarySlip);
                }
                else
                {
                    throw new ErrorCodeException(-1, $"客户单位信息获取失败：{salarySlip.CustomerName}");
                }

                return Ok(records);

            }
        }


        /// <summary>
        /// 发放通知
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        [HttpPut]
        public async Task<IActionResult> BatchNotice([FromForm] Guid id)
        {
            var model = await _thirdSalarySlipService.GetNav(f => f.ID == id);
            if (model==null)
            {
                throw new ErrorCodeException(-1, "数据信息获取失败");
            }
            if (model.IsNotice)
            {
                throw new ErrorCodeException(-1, "批量通知无法重新执行");
            }
            if (model.Records != null && model.Records.Count > 0)
            {
                foreach (var record in model.Records)
                {

                    if (record.UserID<=0)
                    {
                        continue;
                    }
                    if (record.IsNotice == false)
                    {
                        record.IsNotice = true;
                        record.NoticeTime = DateTime.Now;
                    }
                    _platformNoticeService.EmpSalarySlipNotice(record.UserID, record.SalaryMonth.Value, 0, record.CustomerName);
                }
            }
            else
            {
                throw new ErrorCodeException(-1, "没有需要推送的记录");
            }
            model.IsNotice = true;
            model.NoticeTime = DateTime.Now;
            await _thirdSalarySlipService.UpdateNav(model);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> NoticeRecord([FromForm] Guid id, [FromForm] Guid recordID)
        {
            var model = await _thirdSalarySlipService.GetNav(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "数据信息获取失败");
            }
       
            if (model.Records != null && model.Records.Count > 0)
            {
               var record= model.Records.FirstOrDefault(f => f.ID == recordID);
                if (record != null )
                {

                    if (record.UserID <= 0)
                    {
                        throw new ErrorCodeException(-1, "线下员工无法发送通知！");
                    }

                    _platformNoticeService.EmpSalarySlipNotice(record.UserID, record.SalaryMonth.Value, 0, record.CustomerName);
                    record.IsNotice = true;
                    record.NoticeTime = DateTime.Now;
                    await _thirdSalarySlipService.UpdateNav(model);

                }
                else {
                    throw new ErrorCodeException(-1, "数据信息获取失败");
                }
            }
            else
            {
                throw new ErrorCodeException(-1, "没有需要推送的记录");
            }
            return Ok();
        }


        [HttpDelete]
        public async Task<IActionResult> DeleteRecord([FromForm] Guid id, [FromForm] Guid recordID)
        {
            var model = await _thirdSalarySlipService.GetNav(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "数据信息获取失败");
            }

            if (model.Records != null && model.Records.Count > 0)
            {
                var record = model.Records.FirstOrDefault(f => f.ID == recordID);
                if (record != null && record.UserID > 0)
                {
                    model.Records.Remove(record);
                   await _thirdSalarySlipService.UpdateNav(model);

                }
                else
                {
                    throw new ErrorCodeException(-1, "数据信息获取失败");
                }
            }
            else
            {
                throw new ErrorCodeException(-1, "没有需要推送的记录");
            }
            return Ok();
        }




        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] Guid id)
        {
            var modelNav = await _thirdSalarySlipService.GetNav(f => f.ID == id);
            await _thirdSalarySlipService.DeleteNav(modelNav);
            return Ok();
        }



        [HttpPost]
        public async Task<IActionResult> ExportRecord([FromForm]Guid id)
        {

            var modelNav = await _thirdSalarySlipService.GetNav(f => f.ID == id);
            List<Dictionary<string, string>> excelData = new List<Dictionary<string, string>>();
            foreach (var record in modelNav.Records)
            {
                var recordDss = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(record.SlipContent);
                excelData.Add(recordDss);
            }

            var fileBytes = NpoiExcelHelper.WriteExcelKV(excelData);

            //return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet","11.xlsx");
      
            string base64String = Convert.ToBase64String(fileBytes);
            return Ok(new { base64String, fileName = "工资条.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }




    }
}
