using LZ.Common;
using LZ.Core;
using LZ.Core.Attributes;
using LZ.Core.CommonHandle;
using LZ.Core.EventData;
using LZ.Core.Interface;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Core.Services;
using LZ.Finance.Service.Interface;
using LZ.Finance.Service.Services;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.IExcelTemplate;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using NPOI.OpenXmlFormats;
using NPOI.SS.Formula.Functions;
using Senparc.CO2NET.Extensions;
using SqlSugar;
using System.Threading.Tasks;

namespace LZ.Finance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PrepaidApplyRecordController : ApiControllerBase
    {
        private readonly IPrepaidSubAccounttService   _subAccounttService;
        private readonly IPrepaidPartnerAccountService   _prepaidPartnerAccountService;
        private readonly IPrepaidApplyRecordService  _prepaidApplyRecordService;
        private readonly IHttpService _httpService;
        private readonly IAuthService _authService;
        private readonly IEventPublisher _publisher;

        private readonly ISendMQService _sendMQService;

        public PrepaidApplyRecordController(IPrepaidApplyRecordService prepaidApplyRecordService, IPrepaidPartnerAccountService prepaidPartnerAccountService, IHttpService httpService, IAuthService authService, IEventPublisher publisher, ISendMQService sendMQService, IPrepaidSubAccounttService subAccounttService)
        {
            _prepaidApplyRecordService = prepaidApplyRecordService;
            _prepaidPartnerAccountService = prepaidPartnerAccountService;
            _httpService = httpService;
            _authService = authService;
            _publisher = publisher;
            _sendMQService = sendMQService;
            _subAccounttService = subAccounttService;
        }
        [HttpGet]
        public async Task<IActionResult> PageList(int? applyType, int? structureID, string partnerName,string accountPartnerName, string name, string signUnitName, string customerName, string idCardNo, string applyAmount, PrepaidRecordStatusEnumType? status, PrepaidRepayStatusEnumType? repayStatus, BadRepayStatusEnumType? badRepayStatus, bool? isFinance,bool? isMin,DateTime? sendTime, [FromQuery] UserPagedModel query)
        {


            List<string> cmbcNameList = new List<string>() {
                //"宁波莱博友翌供应链管理有限公司",
                //"宁波新莱博友盟供应链管理有限公司",
                "宁波新莱博金浩供应链管理有限公司",
                "宁波蓝鑫人力资源有限公司",
                "宁波莱鑫人力资源有限公司"
            };

            var user = CurrentUser;
            var exp = Expressionable.Create<Model.Live_V_PrepaidApplyRecordExtension>();
            //exp.AndIF(!string.IsNullOrEmpty(query.Type), it => it.ApplyType == Convert.ToInt32(query.Type));
            exp.AndIF(!string.IsNullOrWhiteSpace(accountPartnerName), it => it.AccountPartnerName.Contains(accountPartnerName));
            exp.AndIF(!string.IsNullOrWhiteSpace(partnerName), it => it.PartnerName.Contains(partnerName));
            exp.AndIF(!string.IsNullOrWhiteSpace(signUnitName), it => it.SigningUnitName.Contains(signUnitName));
            exp.AndIF(!string.IsNullOrWhiteSpace(customerName), it => it.CustomerName.Contains(customerName));
            exp.AndIF(!string.IsNullOrWhiteSpace(name), it => it.Name.Contains(name));
            exp.AndIF(!string.IsNullOrWhiteSpace(idCardNo), it => it.IdCardNo.Contains(idCardNo));
            exp.AndIF(!string.IsNullOrWhiteSpace(applyAmount), it => it.ApplyAmount.ToString() == applyAmount);
            exp.AndIF(sendTime.HasValue, it => it.SendTime!=null&&it.SendTime.Value.Date==sendTime.Value);
            exp.AndIF(applyType.HasValue, it => it.ApplyType == applyType);
            exp.AndIF(structureID.HasValue, it => it.StructureID == structureID);
            exp.AndIF(query.CID.HasValue, it => it.CustomerID == query.CID);
            exp.AndIF(repayStatus.HasValue, it => it.RepayStatus == repayStatus.Value);
            exp.AndIF(badRepayStatus.HasValue, it => it.BadRepayStatus == (int)badRepayStatus.Value);
            //exp.AndIF(isFinance.HasValue, it => (it.SendType != null&& it.SendType.Value==1&& it.CashStatus == PrepaidCashStatusEnumType.已签署) ||(it.SigningUnitName!= "宁波莱鑫人力资源有限公司" && it.SigningUnitName!= "宁波蓝鑫人力资源有限公司"));
            exp.AndIF(isFinance.HasValue, it => (it.SendType != null&& it.SendType.Value==1&& it.CashStatus == PrepaidCashStatusEnumType.已签署) || !cmbcNameList.Contains(it.SigningUnitName));

            if (user.PlatformType == Model.Enums.PlatformType.XFZMini)
            {
                exp.And(it => it.UserID == user.UserID);
            }
            else if (user.PlatformType == Model.Enums.PlatformType.LZMini)
            {
                var umodel = await _authService.UserInfo<Model.Live_Personnel>();
                if(query.From=="daywork" && umodel.UserGID.HasValue && umodel.UserType==8)
                {
                    exp.And(it => it.UserIDS.Contains(umodel.UserGID.Value.ToString()));

                }
                exp.And(UserEntryHandle.SetPersonnelAuth<Model.Live_V_PrepaidApplyRecordExtension>(umodel));
                exp.AndIF(isMin.HasValue && isMin.Value, it => it.Status == PrepaidRecordStatusEnumType.待发放 && it.CashStatus == PrepaidCashStatusEnumType.待签署);
                if(isMin.HasValue && isMin.Value)
                {
                    status = null;
                }

            }
            else if (user.PlatformType == Model.Enums.PlatformType.MiniAdmin)
            {
                var info = await _authService.UserInfo<Model.SYS_CompanyUser>();
                if (info != null && info.UserType == 2)
                {
                    exp.And(it => it.PartnerID == info.PartnerID|| it.PartnerGID==info.PartnerGID);
                }
                if (query.From == "daywork")
                {
                    exp.And(it => SqlFunc.Subqueryable<Model.Live_Customer>().Where(s => s.CustomerGID == it.CustomerGID && s.AttendanceType == 2).Any());
                }
            }
            if (user.CompanyType == CompanyType.Party.ToString().ToLower())//人力公司授权甲方公司
            {
                exp.And(it => it.CompanyID == user.CompanyID || SqlFunc.Subqueryable<Model.Live_CompanyAuthorize>().Where(s => s.CustomerID == it.CustomerID && s.ToCompanyID == user.CompanyID && s.Enabled).Any());
            }
           else if (user.PlatformType != Model.Enums.PlatformType.Admin)
            {
                exp.And(it => it.CompanyID == user.CompanyID);
            }
            if (query.StartTime.HasValue)
            {
                exp.And(it => it.ShiftStartTime>= query.StartTime);
            }
            if (query.EndTime.HasValue)
            {
                query.EndTime = query.EndTime.Value.AddDays(1);
                exp.And(it => it.ShiftStartTime < query.EndTime);
            }
            exp.AndIF(status.HasValue, it => it.Status == status.Value);
            var models = await _prepaidApplyRecordService.PageViewList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }

        [HttpGet]
        public async Task<IActionResult> ExportQueryList(int? applyType,string partnerName ,string accountPartnerName,string name, string signUnitName, string customerName, string idCardNo, string applyAmount, PrepaidRecordStatusEnumType? status, PrepaidRepayStatusEnumType? repayStatus, BadRepayStatusEnumType? badRepayStatus, bool? isFinance,DateTime? sendTime, [FromQuery] UserPagedModel query)
        {


            List<string> cmbcNameList = new List<string>() {
                //"宁波莱博友翌供应链管理有限公司",
                //"宁波新莱博友盟供应链管理有限公司",
                "宁波新莱博金浩供应链管理有限公司",
                "宁波蓝鑫人力资源有限公司",
                "宁波莱鑫人力资源有限公司"
            };

            //PrepaidApplyRecordExcelTemplate
            var user = CurrentUser;
            var exp = Expressionable.Create<Model.Live_V_PrepaidApplyRecord>();
            exp.AndIF(!string.IsNullOrEmpty(query.Type), it => it.ApplyType == Convert.ToInt32(query.Type));

            exp.AndIF(!string.IsNullOrWhiteSpace(accountPartnerName), f => f.AccountPartnerName.Contains(accountPartnerName));
            exp.AndIF(!string.IsNullOrWhiteSpace(partnerName), f => f.PartnerName.Contains(partnerName));
            exp.AndIF(!string.IsNullOrWhiteSpace(signUnitName), f => f.SigningUnitName.Contains(signUnitName));
            exp.AndIF(!string.IsNullOrWhiteSpace(customerName), f => f.CustomerName.Contains(customerName));
            exp.AndIF(!string.IsNullOrWhiteSpace(name), f => f.Name.Contains(name));
            exp.AndIF(!string.IsNullOrWhiteSpace(idCardNo), f => f.IdCardNo.Contains(idCardNo));
            exp.AndIF(!string.IsNullOrWhiteSpace(applyAmount), f => f.ApplyAmount.ToString() == applyAmount);
            exp.AndIF(sendTime.HasValue, it => it.SendTime != null && it.SendTime.Value.Date == sendTime.Value);
            exp.AndIF(applyType.HasValue, it => it.ApplyType == applyType);
            exp.AndIF(status.HasValue, f => f.Status == status.Value);
            exp.AndIF(repayStatus.HasValue, f => f.RepayStatus == repayStatus.Value);
            exp.AndIF(badRepayStatus.HasValue, f => f.BadRepayStatus == (int)badRepayStatus.Value);
            //exp.AndIF(isFinance.HasValue, f => (f.SendType != null && f.SendType.Value == 1 && f.CashStatus == PrepaidCashStatusEnumType.已签署) || (f.SigningUnitName != "宁波莱鑫人力资源有限公司" && f.SigningUnitName != "宁波蓝鑫人力资源有限公司"));
            exp.AndIF(isFinance.HasValue, it => (it.SendType != null && it.SendType.Value == 1 && it.CashStatus == PrepaidCashStatusEnumType.已签署) || !cmbcNameList.Contains(it.SigningUnitName));

            if (user.PlatformType == Model.Enums.PlatformType.XFZMini)
            {
                exp.And(it => it.UserID == user.UserID);
            }
            else if (user.PlatformType == Model.Enums.PlatformType.LZMini)
            {
                var umodel = await _authService.UserInfo<Model.Live_Personnel>();
                if (umodel.UserType == (int)PersonnelType.Headed)
                {
                    exp.And(it => it.PerUserID == user.UserID || it.PartnerID == umodel.PartnerID);
                }
                else
                {
                    exp.And(it => it.PerUserID == user.UserID);
                }
            }
            else if (user.PlatformType != Model.Enums.PlatformType.Admin)
            {
                exp.And(it => it.CompanyID == user.CompanyID);
            }
            if (query.From == "daywork")
            {
                exp.And(it => SqlFunc.Subqueryable<Model.Live_Customer>().Where(s => s.CustomerGID == it.CustomerGID && s.AttendanceType == 2).Any());
            }
            var dataList = await _prepaidApplyRecordService.ListView(exp.ToExpression());

            var applyIdList = dataList.Select(f => f.ID).ToList();

           var repaymentRecordList=await _prepaidApplyRecordService.ListRepaymentListView(f => applyIdList.Contains(f.PrepaidApplyRecordID));

            var repaymentDataList = repaymentRecordList.Select(f => new PrepaidRepaymentRecordExcelTemplate()
            {
                AccountPartnerName = f.AccountPartnerName,
                ApplyAmount = f.ApplyAmount?? 0,
                CustomerName = f.CustomerName,
                IdCardNo = f.IdCardNo,
                Name = f.Name,
                RepaymentAmount = f.TotalAmount,
                RepaymentTime = f.RepaymentTime?.ToString("yyyy-MM-dd"),
                RepayStatusName = f.RepayStatus?.ToString(),
                SendTime = f.SendTime?.ToString("yyyy-MM-dd"),
            }).ToList();






            var exportDataList = dataList.Select(f => new PrepaidApplyRecordExcelTemplate()
            {
                ApplyAmount = f.ApplyAmount,
                Bank = f.Bank.ToString(),
                BankNo = f.BankNo.ToString(),
                BankUserName = f.SendType == 1 ? f.BankUserName : f.Name,
                CustomerName = f.CustomerName,
                Name = f.Name,
                CreateTime = f.CreateTime.Value.ToString("yyyy-MM-dd"),
                //PartnerName = f.CusPartnerName,
                SendTypeName = f.SendType == 1 ? "现金发放" : "代发",
                SigningUnitName = f.SigningUnitName,
                StatusName = f.Status.ToString(),
                RepayStatusName = f.RepayStatus.ToString(),
                SendTime = f.SendTime.HasValue ? f.SendTime.Value.ToString("yyyy-MM-dd") : string.Empty,
                AccountPartnerName = f.AccountPartnerName,
                IsCashSend = f.SendType == 1 ? "是" : "否",
                IdCardNo = f.IdCardNo,
                Phone = f.Phone,
                CashStatusName = f.CashStatus?.ToString(),
                BadRepayAmount = f.BadRepayAmount ?? 0,
                BadRepayStatusName = ((BadRepayStatusEnumType)f.BadRepayStatus.Value).ToString(),
                BadRepayTime = f.BadRepayTime?.ToString("yyyy-MM-dd HH:mm:ss")

            }).ToList();

            var fileBytes = NpoiExcelHelper.WriteExcel(exportDataList, "预发工资申请记录", repaymentDataList, "预发还款记录");
            string base64String = Convert.ToBase64String(fileBytes);
            return Ok(new { base64String, fileName = "预发工资申请记录.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }
        



        [HttpGet]
        public async Task<IActionResult> List()
        {
            var exp = Expressionable.Create<Model.Live_V_PrepaidApplyRecord>();
            var models = await _prepaidApplyRecordService.List(exp.ToExpression(), new PagedModelBase() { Page = 0 });
            return Ok(models);
        }
        [HttpGet]
        public async Task<IActionResult> Get(Guid id)
        {
            var model = await _prepaidApplyRecordService.Get(f=>f.ID==id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return Ok(model);
        }

       public record PrepaidApplyRecordUpdateRequest(Guid id,string Bank,string BankNo);
        [HttpPut]
        public async Task<IActionResult> Update(PrepaidApplyRecordUpdateRequest request)
        {
           var model=await _prepaidApplyRecordService.Get(f => f.ID == request.id);

            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }

            if (model.Status!= PrepaidRecordStatusEnumType.待审核)
            {
                throw new ErrorCodeException(-1, "非待审核状态无法修改");
            }

            model.Bank=request.Bank;
            model.BankNo=request.BankNo;

            await _prepaidApplyRecordService.Update(model);
            return Ok();
        }


    
        [AllowAnonymous]
        [HttpPut]
        public async Task<IActionResult> UpdateStatus([FromForm]Guid id,[FromForm] PrepaidRecordStatusEnumType status)
        {
            var model = await _prepaidApplyRecordService.Get(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            model.Status = status;

            if (model.Status == PrepaidRecordStatusEnumType.已发放)
            {
                // 现金发放
                if (model.SendType != null && model.SendType == 1)
                {
                    if (model.CashStatus != PrepaidCashStatusEnumType.已签署 || string.IsNullOrWhiteSpace(model.SignedFilePath))
                    {
                        throw new ErrorCodeException(-1, "现金签收单未签署，无法执行此操作");
                    }
                }
                model.RepayStatus = PrepaidRepayStatusEnumType.待还款;
                model.SendTime = DateTime.Now;

                var applyViewRecord = await _prepaidApplyRecordService.GetView(f => f.ID == model.ID);

                if (model.ApplyType == PrepaidApplyEnumType.日结工资申请)
                {
                    _sendMQService.SendStatisticsMessage("{\"keyID\":\""+ applyViewRecord.AccountPartnerGID + "\",\"type\":\"dailySalary\",\"accountPartnerName\":\"" + applyViewRecord.AccountPartnerName + "\",\"transactionAmount\":\"" + applyViewRecord.ApplyAmount.ToString() + "\"}");
                }
                else
                {
                    _sendMQService.SendStatisticsMessage("{{\"keyID\":\""+ applyViewRecord.AccountPartnerGID + "\",\"type\":\"prepaidSalary\",\"accountPartnerName\":\"" + applyViewRecord.AccountPartnerName + "\",\"transactionAmount\":\"" + applyViewRecord.ApplyAmount.ToString() + "\"}");

                }


            }
            else if (model.Status == PrepaidRecordStatusEnumType.发放失败)
            {
                model.RepayStatus = PrepaidRepayStatusEnumType.无;
            }
            await _prepaidApplyRecordService.Update(model);
            return Ok();
        }


       /// <summary>
       /// 日结工资  待审核批量改成 待发放
       /// </summary>
       /// <param name="structureID"></param>
       /// <param name="customerID"></param>
       /// <param name="startDate"></param>
       /// <param name="endDate"></param>
       /// <returns></returns>
       /// <exception cref="ErrorCodeException"></exception>
        [HttpPut]
        public async Task<IActionResult> UpdateStatusDaySalary([FromForm] int? structureID, [FromForm] Guid customerID, [FromForm] DateTime startDate, [FromForm] DateTime endDate)
        {
            var exp = Expressionable.Create<Live_PrepaidApplyRecord>();
            exp.And(f=>f.Status==  PrepaidRecordStatusEnumType.待审核);
            exp.And(f=>f.ApplyType== PrepaidApplyEnumType.日结工资申请);
            if(structureID.HasValue)
            {
                exp.And(f => f.StructureID == structureID);
            }
            else
            {
                var umodel = await _authService.UserInfo<Model.Live_Personnel>();
                if(umodel.UserType==8 && umodel.UserGID.HasValue)
                {
                    exp.And(f => SqlFunc.Subqueryable<Model.Live_AttendanceStructure>().Where(s => s.UserIDS.Contains(umodel.UserGID.ToString()) && s.ParentID==0 && s.ID == f.StructureID).Any());
                }
            }
            exp.And(f => f.CustomerGID == customerID);

            exp.And(f=> f.ShiftStartTime!=null&& f.ShiftStartTime>=startDate.Date&&f.ShiftStartTime<=endDate.Date);
            var modelList = await _prepaidApplyRecordService.List(f =>f.CustomerGID== customerID&& f.StructureID==structureID&& f.Status==   PrepaidRecordStatusEnumType.待审核&&  f.ApplyType ==  PrepaidApplyEnumType.日结工资申请);
            //if (modelList == null || modelList.Count <= 0)
            //{
                
            //    throw new ErrorCodeException(-1, "待审核记录不存在，无法执行此操作");
            //}
            //else {

            //    foreach (var item in modelList)
            //    {
            //        item.Status = PrepaidRecordStatusEnumType.待发放;
            //    }
            //    await _prepaidApplyRecordService.Update(modelList);
            //}
            return Ok(new { items= modelList });
        }


        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm]Guid id)
        {
            await _prepaidApplyRecordService.Delete(id);
            return Ok();
        }

        #region 申请预发工资
       

        /// <summary>
        /// 预发申请
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Apply(Model.Live_PrepaidApplyRecord m)
        {
            //var now =DateTime.Now;
            //if(now>=Convert.ToDateTime("2026-2-14 00:00:00") && now <= Convert.ToDateTime("2026-2-21 8:59:59"))
            //{
            //    throw new ErrorCodeException(-1, "抱歉，系统将于2026-2-14至2026-2-21 9点暂停申请预发工资");
            //}
            var user = CurrentUser;
            var model = await _httpService.GetAsync<Model.Live_V_UserEntry>(ServiceNames.UsersService, "/api/entry/view?id=" + m.UserEntryID);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "入职信息不存在！");
            }
            if (model.Status!=(int)UserStatus.Entry)
            {
                throw new ErrorCodeException(-1, "您未入职不可申请！");
            }
            if (string.IsNullOrEmpty(model.CardID))
            {
                throw new ErrorCodeException(-1, "身份信息不存在！");
            }
            Live_WhiteList? whiteModel;
           if (m.CompanyID.HasValue)
            {
                whiteModel = await _httpService.GetAsync<Model.Live_WhiteList>(ServiceNames.UsersService, "/api/whitelist/cardget?cardid=" + model.CardID + "&companyId=" + m.CompanyID);
            }
            else
            {
                whiteModel = await _httpService.GetAsync<Model.Live_WhiteList>(ServiceNames.UsersService, "/api/whitelist/cardget?cardid=" + model.CardID + "&companyId=" + user.CompanyID);
            }
                
            if (whiteModel==null)//非现金
            {
                m.SendType = 0;
                if (string.IsNullOrEmpty(model.BankNumber) || string.IsNullOrEmpty(model.BankName))
                {
                    throw new ErrorCodeException(-1, "银行卡缺失不可申请！");
                }
            }
            else//现金
            {
                if (string.IsNullOrEmpty(model.AgentBank)|| string.IsNullOrEmpty(model.AgentBankNo))
                {
                    throw new ErrorCodeException(-1, "代理人银行卡信息缺失！");
                }
                m.SendType = 1;
            }
            Guid? pid = model.PartnerGID;
            if(pid==null)
            {
                throw new ErrorCodeException(-1, "服务商信息缺失！");
            }

            var prepaidAccount = await _prepaidPartnerAccountService.Get(f => f.IsEnabled && f.PartnerGID == pid.Value);
            if (prepaidAccount == null)
            {
                throw new ErrorCodeException(-1, "服务商账户信息不存在！");
            }
            //签章单位 是指定的签章单位
            try
            {
                if (prepaidAccount.SigningUnitID != null && prepaidAccount.SigningUnitID.Value != Guid.Empty && prepaidAccount.SigningUnitID == model.SigningUnitGID)
                {
                    if (prepaidAccount.SpecialPartnerGID != null && prepaidAccount.SpecialPartnerGID.Value != Guid.Empty)
                    {
                        prepaidAccount = await _prepaidPartnerAccountService.Get(f => f.IsEnabled && f.PartnerGID == prepaidAccount.SpecialPartnerGID.Value);
                    }
                }
            }
            catch(Exception e)
            {
                throw new ErrorCodeException(-1, "没有签章单位");
            }
          
            if (m.ApplyType == PrepaidApplyEnumType.预发工资申请)
            {
                var noStatus = new PrepaidRecordStatusEnumType[] { PrepaidRecordStatusEnumType.已发放, PrepaidRecordStatusEnumType.作废, PrepaidRecordStatusEnumType.审核退回, PrepaidRecordStatusEnumType.发放失败 };
                if(await _prepaidApplyRecordService.Exist(it=>it.ApplyType== PrepaidApplyEnumType.预发工资申请 && it.UserID==user.UserID && !noStatus.Contains(it.Status)))
                {
                    throw new ErrorCodeException(-1, "您已有一条记录待汇款中，请结束后再申请！");
                }
                if (model.LimitMoney > 0)
                {
                    var status = new PrepaidRecordStatusEnumType[] { PrepaidRecordStatusEnumType.已发放, PrepaidRecordStatusEnumType.待审核, PrepaidRecordStatusEnumType.待发放 };
                    var sumMoney = await _prepaidApplyRecordService.SumMoney(it => it.ApplyType == PrepaidApplyEnumType.预发工资申请 && it.PartnerID == model.PartnerID && status.Contains(it.Status) && it.UserID == user.UserID && it.RepayStatus!=PrepaidRepayStatusEnumType.还款完成);
                    if (sumMoney+m.ApplyAmount > model.LimitMoney)
                    {
                        throw new ErrorCodeException(-1, "您累计申请预付金额已超限！");
                    }
                }
                if (model.EntryDays > 0 && (DateTime.Now - model.EntryTime.Value).Days < model.EntryDays)
                {
                    throw new ErrorCodeException(-1, $"您的入职天数未达到{model.EntryDays}天！");
                }
            }

            Live_PrepaidApplyRecord applyRecord = new Live_PrepaidApplyRecord()
            {
                ID = Guid.NewGuid(),
                ApplyAmount = m.ApplyAmount,
                Bank =m.SendType==0? model.BankName:model.AgentBank,
                BankNo = m.SendType == 0 ? model.BankNumber : model.AgentBankNo,
                BankUserName= m.SendType == 1 ? model.AgentName : "",
                CreateTime = DateTime.Now,
                CustomerGID = model.CustomerGID.Value,
                CustomerID = model.CustomerID.Value,
                IdCardNo = model.CardID,
                StructureID = m.StructureID.HasValue? m.StructureID.Value :null,
                PartnerID = model.PartnerID.Value,
                PartnerGID = model.PartnerGID.Value,
                Name = model.Name,
                PrepaidPartnerAccountID = prepaidAccount.ID,
                SigningUnitID = model.SigningUnitGID.HasValue? model.SigningUnitGID.Value :Guid.Parse("46779570-0131-4393-BE8B-0C0F34C214BD"),
                UserEntryID = model.ID,
                ApplyType = m.ApplyType,
                DayWorkType = m.DayWorkType,
                //Status = m.ApplyType==PrepaidApplyEnumType.日结工资申请?PrepaidRecordStatusEnumType.待结算:PrepaidRecordStatusEnumType.待审核,
                Status = PrepaidRecordStatusEnumType.待审核,
                CompanyGID = model.CompanyGID.Value,
                CompanyID = model.CompanyID.Value,
                UserID = model.UserID,
                ShiftEndTime=m.ShiftEndTime,
                ShiftStartTime = m.ShiftStartTime,
                ShiftHours = m.ShiftHours,
                ShiftMoney = m.ShiftMoney,
                LunchHours = m.LunchHours,
                NightShiftSubsidy = m.NightShiftSubsidy,
                DinnerHours = m.DinnerHours,
                MoneyHours = m.MoneyHours,
                PerUserID = model.PerUserID,
                WorkType = m.WorkType,
                SendType = m.SendType,
                CashStatus = PrepaidCashStatusEnumType.无,
                PdfFilePath = string.Empty,
                SignedFilePath = string.Empty,
                Phone = model.Phone,
                RepayStatus = PrepaidRepayStatusEnumType.无,
                IsSinglePushSuc = false,
                RepayTotalAmount = 0,
                Msg = string.Empty,
                Remark = string.Empty,
                AuditTime = null,
                 BadRepayAmount = 0,
                  BadRepayStatus =  BadRepayStatusEnumType.无,
                   BadRepayTime=null,
                    IsBadRepay=false,
                     IsNotice=false,
                      SendTime = null,  
            };
            if(!string.IsNullOrEmpty(m.JobNo))
            {
                applyRecord.JobNo = m.JobNo;
            }
            if (!string.IsNullOrEmpty(m.EntryNo))
            {
                applyRecord.EntryNo = m.EntryNo;
            }
            var count =  await _prepaidApplyRecordService.Add(applyRecord);
            if(count>0 && m.ApplyType == PrepaidApplyEnumType.预发工资申请)
            {
                var record = new PartnerSendNoticeEventData();
                record.PartnerID = model.PartnerID.Value;
                record.UserType = PersonnelType.Headed;
                record.Title = "预付工资审核提醒";
                record.Message = $"{model.Name}申请了{m.ApplyAmount}元的预付工资";
                record.Page = "/pageInterview/interview/usermoney";
                _publisher.Publish(record);
            }
            return Ok();
        }

        public record PrepaidAuditRequest(Guid Id, PrepaidRecordStatusEnumType status, string Remark);


        [HttpGet]
        public async Task<IActionResult> PreAudit(Guid id)
        {
            var model = await _prepaidApplyRecordService.GetView(f => f.ID == id);
            var subAccount = await _subAccounttService.Get(f => f.IsEnable && f.SignUnitName == model.SigningUnitName);
            if (subAccount != null)
            {
                if (subAccount.Balance < subAccount.MinBalance)
                {
                    return Ok(new { code = 0, IsWarning = true, title = "账户余额不足", message = "预发账户余额不足，已通知财务充值，充值完成后将立即发放！" });
                }
                else
                {
                    return Ok(new { code = 0, IsWarning = false, title = "账户余额充足", message = "账户余额充足" });
                }
            }
            else {
                return Ok(new { code = 0, IsWarning = false, title = "账户余额充足", message = "账户余额充足" });
            }
            
        }

        /// <summary>
        /// 预发审核
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> Audit(PrepaidAuditRequest request)
        {
            var model = await _prepaidApplyRecordService.Get(f=>f.ID==request.Id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "申请记录信息不存在");
            }
            if (model.Status!= PrepaidRecordStatusEnumType.待审核 && model.Status != PrepaidRecordStatusEnumType.待结算)
            {
                throw new ErrorCodeException(-1, "非待审核或非待结算状态无法执行此操作");
            }
            if (PrepaidRecordStatusEnumType.待审核 == request.status)
            {
                await _prepaidApplyRecordService.ChangeStatus(request.Id, request.status);
            }
            else if (PrepaidRecordStatusEnumType.待结算 == request.status)
            {
                await _prepaidApplyRecordService.ChangeStatus(request.Id, request.status);
            }
            else if (PrepaidRecordStatusEnumType.待发放 == request.status)
            {
                model.Status = request.status;
                var hav = await _prepaidApplyRecordService.ApplyRecordAudit(model);
                if (!hav)
                {
                    throw new ErrorCodeException(-1, "审核失败！");
                }
            }
            else
            {
                model.Status = Model.Enums.PrepaidRecordStatusEnumType.审核退回;
                model.Remark = request.Remark;
                await _prepaidApplyRecordService.Update(model);
            }


           
            return Ok();
        }
        /// <summary>
        /// 修改金额
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> ChangeApplyAmount([FromForm]Guid id, [FromForm] decimal money)
        {
            var model = await _prepaidApplyRecordService.Get(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, $"申请记录信息不存在");
            }
            if (model.Status != PrepaidRecordStatusEnumType.待审核)
            {
                throw new ErrorCodeException(-1, $"非待审核状态不可修改金额");
            }
            model.ApplyAmount = money;
            await _prepaidApplyRecordService.ChangeApplyAmount(id, model.ApplyAmount);
            return Ok(new {model });
        }
        /// <summary>
        /// 修改时长
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> ChangeApplyHour([FromForm] Guid id, [FromForm] decimal hour)
        {
            var model = await _prepaidApplyRecordService.Get(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, $"申请记录信息不存在");
            }
            if (model.Status != PrepaidRecordStatusEnumType.待审核)
            {
                throw new ErrorCodeException(-1, $"非待审核状态不可修改金额");
            }
            model.MoneyHours = hour;
            decimal money = model.ShiftMoney.HasValue ? model.ShiftMoney.Value : 0M;
            model.ApplyAmount = decimal.Round(hour * money, 2) + model.NightShiftSubsidy ?? 0M;
            await _prepaidApplyRecordService.ChangeApplyAmount(id, model.ApplyAmount, hour);
            return Ok(new { model });
        }
        /// <summary>
        /// 取消预发
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> Cancel([FromForm] Guid id)
        {
            var model = await _prepaidApplyRecordService.Get(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, $"申请记录信息不存在");
            }
            if (model.Status != PrepaidRecordStatusEnumType.待审核)
            {
                throw new ErrorCodeException(-1, $"非待审核状态不可修改金额");
            }
            await _prepaidApplyRecordService.ChangeStatus(id, PrepaidRecordStatusEnumType.作废);
            return Ok();
        }
        #endregion

        #region 预发异常还款


        /// <summary>
        ///  手动还款完成
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> UpdateBadRepayStatus([FromForm] Guid id, [FromForm] BadRepayStatusEnumType badRepayStatus)
        {
            var model = await _prepaidApplyRecordService.GetNav(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }

            if (model.Status != PrepaidRecordStatusEnumType.已发放)
            {
                throw new ErrorCodeException(-1, "未发放记录无法执行此操作！");
            }

            if (model.RepayStatus == PrepaidRepayStatusEnumType.还款完成)
            {
                throw new ErrorCodeException(-1, "还款已完成无法执行此操作！");
            }

            if (badRepayStatus == BadRepayStatusEnumType.异常还款成)
            {
                decimal repaidAmount = 0;
                if (model.RepaymentRecords != null && model.RepaymentRecords.Any())
                {
                    repaidAmount = model.RepaymentRecords.Sum(f => f.TotalAmount);
                }
                // 待还款金额
                decimal pedingRepayAmount = model.ApplyAmount - repaidAmount;

                if (pedingRepayAmount <= 0)
                {
                    throw new ErrorCodeException(-1, "待还款金额小于0，无法执行此操作！");
                }

                if (pedingRepayAmount > 0)
                {
                    model.AddRepaymentRecord(Guid.Empty, Guid.Empty, model.UserEntryID, pedingRepayAmount, "客服人工处理还款");
                }
                model.BadRepayStatus = badRepayStatus;
                model.BadRepayAmount = pedingRepayAmount;
                model.IsBadRepay = true;
                model.RepayStatus = PrepaidRepayStatusEnumType.还款完成;
                model.BadRepayTime=DateTime.Now;
                await _prepaidApplyRecordService.UpdateNav(model);
            }
            else {
                model.BadRepayStatus = badRepayStatus;
                await _prepaidApplyRecordService.Update(model);
            }

         
            return Ok();
        }


        /// <summary>
        ///  手动还款完成
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> RepayCompleted([FromForm] Guid id)
        {
            var model = await _prepaidApplyRecordService.GetNav(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }

            if (model.Status!= PrepaidRecordStatusEnumType.已发放)
            {
                throw new ErrorCodeException(-1, "未发放记录无法执行此操作！");
            }

            if (model.RepayStatus == PrepaidRepayStatusEnumType.还款完成)
            {
                throw new ErrorCodeException(-1, "还款已完成无法执行此操作！");
            }
            decimal repaidAmount = 0;
            if (model.RepaymentRecords != null && model.RepaymentRecords.Any())
            {
                repaidAmount = model.RepaymentRecords.Sum(f => f.TotalAmount);
            }
            // 待还款金额
            decimal pedingRepayAmount = model.ApplyAmount- repaidAmount;

            if (pedingRepayAmount<=0)
            {
                throw new ErrorCodeException(-1, "待还款金额小于0，无法执行此操作！");
            }

            if (pedingRepayAmount>0)
            {
             var  record=  model.AddRepaymentRecord(Guid.Empty, Guid.Empty, model.UserEntryID, pedingRepayAmount, "客服人工处理还款");
                record.IsSuc=true;
            }

            model.BadRepayAmount = pedingRepayAmount;
            model.IsBadRepay = true;
            model.RepayStatus = PrepaidRepayStatusEnumType.还款完成;
            await _prepaidApplyRecordService.UpdateNav(model);
            return Ok();
        }



        #endregion

        #region 手动还款


        /// <summary>
        ///  手动还款完成
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPut]
        public async Task<IActionResult> RepayAnyAmount([FromForm] Guid id, [FromForm] decimal repayAnyAmount, [FromForm] DateTime repayTime)
        {
            var model = await _prepaidApplyRecordService.GetNav(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }

            if (model.Status != PrepaidRecordStatusEnumType.已发放)
            {
                throw new ErrorCodeException(-1, "未发放记录无法执行此操作！");
            }

            if (model.RepayStatus == PrepaidRepayStatusEnumType.还款完成)
            {
                throw new ErrorCodeException(-1, "还款已完成无法执行此操作！");
            }
            decimal repaidAmount = 0;
            if (model.RepaymentRecords != null && model.RepaymentRecords.Any())
            {
                repaidAmount = model.RepaymentRecords.Sum(f => f.TotalAmount);
            }
            // 待还款金额
            decimal pedingRepayAmount = model.ApplyAmount - repaidAmount;

            if (pedingRepayAmount <= 0)
            {
                throw new ErrorCodeException(-1, "待还款金额小于0，无法执行此操作！");
            }

            if (pedingRepayAmount > 0&& repayAnyAmount <= pedingRepayAmount)
            {
                model.AddRepaymentRecord(Guid.Empty, Guid.Empty, model.UserEntryID, repayAnyAmount, "手动处理还款");

             var record=   model.RepaymentRecords.OrderByDescending(f => f.CreateTime).First();
                record.RepaymentTime = repayTime;
                record.CreateTime = repayTime;
                record.IsSuc=true;
            }

            if (pedingRepayAmount == repayAnyAmount)
            {
                model.RepayStatus = PrepaidRepayStatusEnumType.还款完成;
            }
            else {
                model.RepayStatus = PrepaidRepayStatusEnumType.还款失败;
            }


         
            await _prepaidApplyRecordService.UpdateNav(model);
            return Ok();
        }
        #endregion





    }
}
