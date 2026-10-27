using LZ.Common;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Finance.Service.Interface;
using LZ.Finance.Service.Services;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.ExcelTemplateModel;
using LZ.Model.Exceptions;
using LZ.Model.IExcelTemplate;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.OpenXmlFormats;
using Senparc.CO2NET.Extensions;
using SqlSugar;
using SqlSugar.IOC;

namespace LZ.Finance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [AllowAnonymous]
    public class PrepaidInterestReportController : ApiControllerBase
    {

        private readonly IPrepaidApplyRecordService _applyRecordService   ;
        private readonly IPrepaidInterestRecordService _interestRecordService   ;
        private readonly IPrepaidInterestReportService _interestReportService   ;
        private readonly IAuthService _authService;

        public PrepaidInterestReportController(IPrepaidInterestReportService interestReportService, IAuthService authService, IPrepaidInterestRecordService interestRecordService, IPrepaidApplyRecordService applyRecordService)
        {
            _interestReportService = interestReportService;
            _authService = authService;
            _interestRecordService = interestRecordService;
            _applyRecordService = applyRecordService;
        }

        [HttpGet]
        public async Task<IActionResult> PageList(string partnerName,DateTime? billMonth, [FromQuery] PagedModelBase query)
        {
            var user = CurrentUser;

            var exp = Expressionable.Create<Model.Live_V_PrepaidInterestReport>();
            exp.AndIF(!string.IsNullOrWhiteSpace(partnerName), f => f.PartnerName.Contains(partnerName));
            exp.AndIF(billMonth.HasValue, f => f.BillMonth==billMonth.Value);
            exp.AndIF(CurrentUser.PlatformType != PlatformType.Admin && CurrentUser.PlatformType != PlatformType.Api, f => f.CompanyID == CurrentUser.CompanyID);

            if (CurrentUser.PlatformType == Model.Enums.PlatformType.MiniAdmin)
            {
                var info = await _authService.UserInfo<Model.SYS_CompanyUser>();
                if (info != null && info.UserType == 2)
                {
                    exp.And(t => t.PartnerID == info.PartnerID);
                }
            }

            var models = await _interestReportService.PageList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }


        [HttpGet]
        public async Task<IActionResult> ExportQueryList(string partnerName, DateTime? billMonth, [FromQuery] PagedModelBase query)
        {
            var user = CurrentUser;

            var exp = Expressionable.Create<Model.Live_V_PrepaidInterestReport>();
            exp.AndIF(!string.IsNullOrWhiteSpace(partnerName), f => f.PartnerName.Contains(partnerName));
            exp.AndIF(billMonth.HasValue, f => f.BillMonth == billMonth.Value);
            exp.AndIF(CurrentUser.PlatformType != PlatformType.Admin && CurrentUser.PlatformType != PlatformType.Api, f => f.CompanyID == CurrentUser.CompanyID);

            if (CurrentUser.PlatformType == Model.Enums.PlatformType.MiniAdmin)
            {
                var info = await _authService.UserInfo<Model.SYS_CompanyUser>();
                if (info != null && info.UserType == 2)
                {
                    exp.And(t => t.PartnerID == info.PartnerID);
                }
            }

            var models = await _interestReportService.ListView(exp.ToExpression());

            var exportData1 =   models.Select(f => new PrepaidInterestReportExcelTemplate()
            {
                ApplyMonthAmount = f.ApplyMonthAmount.Value,
                ApplyYearAmount = f.ApplyYearAmount.Value,
                BasePercent = f.BasePercent,
                BillMonth = f.BillMonth.Value.ToString("yyyy-MM-dd"),
                FloatPercent = f.FloatPercent,
                InterestMonthAmount = f.InterestMonthAmount,
                InterestYearAmount = f.InterestYearAmount,
                PartnerName = f.PartnerName,
            }
            ).ToList() ;


            var fileBytes = NpoiExcelHelper.WriteExcel(exportData1, "金融技术服务费");
            string base64String = Convert.ToBase64String(fileBytes);
            return Ok(new { base64String, fileName = "金融技术服务费.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }


        [HttpPost]
        public async Task<IActionResult> Add(Model.Live_PrepaidInterestReport model)
        {
            model.ID = Guid.NewGuid();
            model.CreateTime = DateTime.Now;
            await _interestReportService.Add(model);
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> Get(Guid id)
        {

            var model = await _interestReportService.Get(f=>f.ID==id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return Ok(model); 
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_PrepaidInterestReport model)
        {
            await _interestReportService.Update(model);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm]Guid id)
        {
            await _interestReportService.Delete(id);
            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> ExportReportDetailExcel([FromForm] Guid id)
        {
            var model = await _interestReportService.Get(f => f.ID == id);
            var recordList = await _interestRecordService.List(f => f.PrepaidPartnerAccountID == model.PrepaidPartnerAccountID);
            var applyIdList = recordList.Select(f => f.PrepaidApplyRecordID).Distinct().ToList();
            var applyRecordList = await _applyRecordService.ListView(f => applyIdList.Contains(f.ID));
            var exportData1 = new List<PrepaidInterestReportExcelTemplate>() {

            new PrepaidInterestReportExcelTemplate(){
             ApplyMonthAmount=model.ApplyMonthAmount.Value,
              ApplyYearAmount=model.ApplyYearAmount.Value,
               BasePercent=model.BasePercent,
                BillMonth=model.BillMonth.Value.ToString("yyyy-MM-dd"),
                 FloatPercent=model.FloatPercent,
                  InterestMonthAmount= model.InterestMonthAmount,
                   InterestYearAmount= model.InterestYearAmount,
                    PartnerName=model.PartnerName,

            }

            };
            var exportData2 = applyRecordList.Select(f => new PrepaidInterestApplyRecordExcelTemplate()
            {
                ApplyAmount = f.ApplyAmount,
                CustomerName = f.CustomerName,
                IdCardNo = f.IdCardNo,
                Name = f.Name,
                PartnerName = f.PartnerName,
                SendDate = f.SendTime.HasValue ? f.SendTime.Value.ToString("yyyy-MM-dd") : f.CreateTime.Value.ToString("yyyy-MM-dd"),
                Status = f.Status.ToString(),
            }).ToList();


            var exportData3 = recordList.Select(f => new PrepaidInterestRecordExcelTemplate()
            {
                PartnerName = f.PartnerName,
                ActualPercent = f.ActualPercent,
                ApplyAmount = f.ApplyAmount,
                BasePercent = f.BasePercent,
                BillMonth = f.BillMonth.Value.ToString("yyyy-MM-dd"),
                FloatPercent = f.FloatPercent,
                CustomerName = f.CustomerName,
                Days = f.Days,
                EndDate = f.EndDate.Value.ToString("yyyy-MM-dd"),
                MainAmount = f.MainAmount,
                MainInterestAmount = f.MainInterestAmount,
                Name = f.Name,
                RepaymentDate = f.RepaymentDate.HasValue? f.RepaymentDate.Value.ToString("yyyy-MM-dd"):"",
                SendDate = f.SendDate.Value.ToString("yyyy-MM-dd"),
                StartDate = f.StartDate.Value.ToString("yyyy-MM-dd"),

            }).ToList();

         
            var fileBytes = NpoiExcelHelper.WriteExcel(exportData1, "总账单", exportData2,"预发工资发放明细", exportData3,"预发工资还款明细");
            string base64String = Convert.ToBase64String(fileBytes);
            return Ok(new { base64String, fileName = "工资结算单.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }
    }
}
