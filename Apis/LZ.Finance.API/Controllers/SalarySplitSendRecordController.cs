using LZ.Common;
using LZ.Core;
using LZ.Core.CommonHandle;
using LZ.Core.Interface;
using LZ.Finance.Service.Interface;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.ExcelTemplateModel;
using LZ.Model.Exceptions;
using LZ.Model.IExcelTemplate;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Senparc.CO2NET.Extensions;
using SqlSugar;
using SqlSugar.IOC;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace LZ.Finance.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    public class SalarySplitSendRecordController : ApiControllerBase
    {

        private readonly IFinanceOtherService  _otherService    ;
        private readonly ISalarySplitSendService  _splitSendService;
        private readonly ISalaryDelaySendRecordService  _delaySendRecordService;
        private readonly ISalaryRecordService  _salaryRecordService;
        private readonly IAuthService  _authService;

        public SalarySplitSendRecordController(ISalarySplitSendService splitSendService, ISalaryDelaySendRecordService delaySendRecordService, ISalaryRecordService salaryRecordService, IFinanceOtherService otherService, IAuthService authService)
        {
            _splitSendService = splitSendService;
            _delaySendRecordService = delaySendRecordService;
            _salaryRecordService = salaryRecordService;
            _otherService = otherService;
            _authService = authService;
        }

        [HttpGet]
        public async Task<IActionResult> PageList(string customerName,string signUnitName,string name, string idCardNo, SalarySplitSendRecordStatusEnumType? status,bool? isFinance,[FromQuery] PagedModelBase query)
        {
            var exp = Expressionable.Create<Model.live_V_SalarySplitSendRecord>();



            var user = CurrentUser;
            SYS_CompanyUser info = null;
            if (CurrentUser.PlatformType == Model.Enums.PlatformType.MiniAdmin)
            {
                 info = await _authService.UserInfo<Model.SYS_CompanyUser>();
                if (info != null && info.UserType == 2)
                {
                    exp.And(t => t.PartnerID == info.PartnerID);
                }
            }
            if (user.PlatformType == Model.Enums.PlatformType.XFZMini)
            {
                exp.And(it => it.UserID == user.UserID);
            }
            else if (user.PlatformType == Model.Enums.PlatformType.LZMini)
            {
                //var umodel = await _authService.UserInfo<Model.Live_Personnel>();
                //exp.And(UserEntryHandle.SetPersonnelAuth<Model.Live_V_SalaryDelaySendRecord>(umodel));
                //exp.And(f => f.Status != SalaryDelaySendStatusEnumType.已处理);
            }
            if (user.PlatformType != Model.Enums.PlatformType.Admin)
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

            exp.And(f=> f.SalaryRecordStatus == SalaryRecordSendStatusEnumType.已发放);
            exp.AndIF(!string.IsNullOrWhiteSpace(signUnitName), f => f.SigningUnitCompanyName.Contains(signUnitName));
            exp.AndIF(!string.IsNullOrWhiteSpace(customerName), f => f.CustomerName.Contains(customerName));
            exp.AndIF(!string.IsNullOrWhiteSpace(name), f => f.Name.Contains(name));
            exp.AndIF(!string.IsNullOrWhiteSpace(idCardNo), f => f.Name.Contains(idCardNo));
            exp.AndIF(status.HasValue, f => f.Status==status.Value);
            exp.AndIF(isFinance.HasValue, f =>f.Status!=  SalarySplitSendRecordStatusEnumType.取消发放);
            var models = await _splitSendService.PageRecordList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }


        [HttpGet]
        public async Task<IActionResult> ExportQueryList(string customerName,string signUnitName, string name, string idCardNo, SalarySplitSendRecordStatusEnumType? status, bool? isFinance)
        {
            var exp = Expressionable.Create<Model.live_V_SalarySplitSendRecord>();



            var user = CurrentUser;
            if (CurrentUser.PlatformType == Model.Enums.PlatformType.MiniAdmin)
            {
                var info = await _authService.UserInfo<Model.SYS_CompanyUser>();
                if (info != null && info.UserType == 2)
                {
                    exp.And(t => t.PartnerID == info.PartnerID);
                }
            }
            if (user.PlatformType == Model.Enums.PlatformType.XFZMini)
            {
                exp.And(it => it.UserID == user.UserID);
            }
            else if (user.PlatformType == Model.Enums.PlatformType.LZMini)
            {
                //var umodel = await _authService.UserInfo<Model.Live_Personnel>();
                //exp.And(UserEntryHandle.SetPersonnelAuth<Model.Live_V_SalaryDelaySendRecord>(umodel));
                //exp.And(f => f.Status != SalaryDelaySendStatusEnumType.已处理);
            }
            if (user.PlatformType != Model.Enums.PlatformType.Admin)
            {
                exp.And(it => it.CompanyID == user.CompanyID);
            }

            exp.And(f => f.SalaryRecordStatus == SalaryRecordSendStatusEnumType.已发放);
            exp.AndIF(!string.IsNullOrWhiteSpace(signUnitName), f => f.SigningUnitCompanyName.Contains(signUnitName));
            exp.AndIF(!string.IsNullOrWhiteSpace(customerName), f => f.CustomerName.Contains(customerName));
            exp.AndIF(!string.IsNullOrWhiteSpace(name), f => f.Name.Contains(name));
            exp.AndIF(!string.IsNullOrWhiteSpace(idCardNo), f => f.Name.Contains(idCardNo));
            exp.AndIF(status.HasValue, f => f.Status == status.Value);
            exp.AndIF(isFinance.HasValue, f => f.Status != SalarySplitSendRecordStatusEnumType.取消发放);



            var dataList =   await _splitSendService.ListRecordViewList(exp.ToExpression());

            var exportDataList = dataList.Select(f => new SalarySplitSendRecordExcelTemplate()
            {
                IdCardNo = f.IdCardNo,
                Bank = f.Bank,
                BankNo = f.BankNo,
                BankUserName = f.BankUserName,
                CustomerName = f.CustomerName,
                Name = f.Name,
                CreateTime = f.CreateTime.HasValue ? f.CreateTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : "",
                SendTime = f.SendTime.HasValue ? f.SendTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : "",
                TaxMonth = f.TaxMonth.HasValue ? f.TaxMonth.Value.ToString("yyyy-MM-dd HH:mm:ss") : "",
                SalaryMonth = f.TaxMonth.HasValue ? f.SalaryMonth.Value.ToString("yyyy-MM-dd HH:mm:ss") : "",
                SigningUnitCompanyName = f.SigningUnitCompanyName,
                SplitAmount = f.SplitAmount,
                Status = f.Status,
            }).ToList();
            var fileBytes = NpoiExcelHelper.WriteExcel(exportDataList, "部分发放记录");
            string base64String = Convert.ToBase64String(fileBytes);
            return Ok(new { base64String, fileName = "部分发放.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }




        [HttpGet]
        public async Task<IActionResult> GetView(Guid id)
        {
            var model = await _splitSendService.GetRecordView(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "记录获取失败");
            }
            return Ok(model);
        }

        #region 记录操作
        [HttpPut]
        public async Task<IActionResult> UpdateRecordStatus([FromForm] Guid id, [FromForm] Guid recordID, [FromForm] SalarySplitSendRecordStatusEnumType status)
        {

            var modelNav = await _splitSendService.GetNav(f => f.ID == id);
            modelNav.SetRecordStatus(recordID, status);
            var salaryRecord = await _salaryRecordService.Get(f => f.ID == modelNav.SalaryRecordID);
            if (salaryRecord == null)
            {
                throw new ErrorCodeException(-1, "关联的工资记录已被删除，无法执行此操作");
            }

            try
            {
                DbScoped.SugarScope.BeginTran();
                if (salaryRecord != null)
                {

                    if (modelNav.Records!=null)
                    {
                        salaryRecord.SplitSendSucAmount = modelNav.Records.Where(f => f.Status == SalarySplitSendRecordStatusEnumType.已发放).Sum(f => f.SplitAmount);
                        await _salaryRecordService.Update(salaryRecord);
                    }
                }
                await _splitSendService.UpdateNav(modelNav);

                DbScoped.SugarScope.CommitTran();
            }
            catch (Exception ex)
            {
                DbScoped.SugarScope.RollbackTran();
                throw new ErrorCodeException(-1,ex.Message);
            }
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> DeleteRecord([FromForm] Guid id, [FromForm] Guid recordID)
        {
            var modelNav = await _splitSendService.GetNav(f => f.ID == id);
            modelNav.DeleteRecord(recordID);
            await _splitSendService.UpdateNav(modelNav);
            return Ok();
        }
        #endregion


    }
}
