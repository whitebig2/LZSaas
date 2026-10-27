using LZ.Clients.API;
using LZ.Core.Attributes;
using LZ.Core.Interface;
using LZ.Core.Services;
using LZ.Finance.Service.Interface;
using LZ.Finance.Service.Services;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.Enums.Attendance;
using LZ.Model.Enums.Insurance;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using MathNet.Numerics;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Senparc.Weixin.MP.AdvancedAPIs.MerChant;
using SqlSugar;
using System.Collections.Generic;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LZ.Finance.API.Controllers
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.LZMini)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    public class AttendancePrepaidApplyCustomerController(IAttendancePrepaidApplyCustomerService attendancePrepaidApplyCustomerService, IHttpService httpService, IPrepaidApplyRecordService prepaidApplyRecordService, IAttendancePrepaidApplyRecordService attendancePrepaidApplyRecordService, IAuthService authService,IInvoiceService invoiceService) : FinanceControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] FinancePageModel query, string jobNo)
        {
            var user = CurrentUser;
            var exp = Expressionable.Create<Model.Live_V_AttendancePrepaidApplyCustomer>();
            if (CurrentUser.PlatformType == Model.Enums.PlatformType.Admin || CurrentUser.PlatformType == Model.Enums.PlatformType.Api)
            {
                exp.AndIF(query.CompanyID.HasValue && query.CompanyID > 0, it => it.CompanyID == query.CompanyID);
            }
            else
            {
                exp.And(t => t.CompanyID == user.CompanyID);
            }
            if (CurrentUser.PlatformType == Model.Enums.PlatformType.LZMini)
            {
                query.Order = "status asc";
                var umodel = await authService.UserInfo<Model.Live_Personnel>();
                if (umodel.UserType == 8)
                {
                    exp.And(it => it.UserIDS.Contains(umodel.UserGID.Value.ToString()));
                }
            }
            if(!string.IsNullOrEmpty(query.BillMonth))
            {
                exp.And(t => t.BillMonth == query.BillMonth);
            }
            if (!string.IsNullOrEmpty(jobNo))
            {
                exp.And(t => t.JobNo.Contains(jobNo));
            }
            //if (!string.IsNullOrEmpty(query.Types))
            //{
            //    var arr = query.Types.Split(",");
            //    exp.And(t => arr.Contains(t.ProcessStatus.ToString()));
            //}
            exp.AndIF(!string.IsNullOrEmpty(query.Name), t => t.CustomerName.Contains(query.Name));
            //exp.AndIF(!string.IsNullOrEmpty(payer), t => t.Payer.Contains(query.UserName));
            //exp.AndIF(!string.IsNullOrEmpty(query.UserName), t => t.UserName.Contains(query.UserName));
            //exp.AndIF(!string.IsNullOrEmpty(query.PartnerName), t => t.PartnerName.Contains(query.PartnerName));
            //exp.AndIF(!string.IsNullOrEmpty(query.Description), t => t.Description.Contains(query.Description));
            //exp.AndIF(query.Money.HasValue, t => t.ApplyAmount == query.Money);
            //exp.AndIF(query.StartTime.HasValue, t => t.CreateTime >= query.StartTime);
            //exp.AndIF(query.EndTime.HasValue, t => t.CreateTime >= query.EndTime);
            //exp.AndIF(query.ProcessStatus.HasValue, t => t.ProcessStatus == query.ProcessStatus);
            //exp.AndIF(query.Year.HasValue, t => t.Year >= query.Year);
            var models = await attendancePrepaidApplyCustomerService.List(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }

        [HttpPost]
        public async Task<IActionResult> BatchCreate([FromForm] DateTime date)
        {
            var user = CurrentUser;
            DateTime start = date;
            DateTime end = date.AddMonths(1);
            Model.PagedModel.FinancePageModel query = new Model.PagedModel.FinancePageModel();
            query.Page = 0;
            var expable = Expressionable.Create<Model.Live_V_PrepaidApplyRecord>();

            var paras = new Dictionary<string, string>();
            //paras.Add("CompanyID", user.CompanyID.ToString());
            paras.Add("CompanyID", "1");
            paras.Add("AttendanceType", "2");
            paras.Add("StartTime", start.ToString());
            paras.Add("EndTime", end.ToString());
            paras.Add("type", ((int)AttendanceTypeEnum.下班).ToString());//日结工下班卡
            var obj = await httpService.GetAsync(ServiceNames.UsersService, "/api/AttendanceRecord/list", paras);//获取某月份所有日结打卡记录，用来判断需要统计哪些客户的结算单
            List<Live_V_AttendanceRecord> records = new List<Live_V_AttendanceRecord>();
            if (!string.IsNullOrEmpty(obj.Data.ToString()))
            {
                var ii = JsonConvert.DeserializeObject(obj.Data.ToString()) as JObject;
                records = JsonConvert.DeserializeObject<List<Live_V_AttendanceRecord>>(ii["items"].ToString());
            }
            else
            {
                throw new ErrorCodeException(-1, "查询日结客户失败");
            }

            if (CurrentUser.PlatformType == Model.Enums.PlatformType.Admin || CurrentUser.PlatformType == Model.Enums.PlatformType.Api)
            {
                expable.AndIF(query.CompanyID.HasValue && query.CompanyID > 0, it => it.CompanyID == query.CompanyID);
            }
            else
            {
                expable.And(it => it.CompanyID == user.CompanyID);
            }
            
            expable.And(t => t.ApplyType == 1 && t.Status == Model.Enums.PrepaidRecordStatusEnumType.已发放 && t.ShiftStartTime.HasValue && t.ShiftStartTime > start && t.ShiftStartTime < end);
            List<Live_AttendancePrepaidApplyRecord> userList = new List<Live_AttendancePrepaidApplyRecord>();
            var prepaidApplyRecords = await prepaidApplyRecordService.List(expable.ToExpression(), query);
            var prepaidApplyCustomers = await attendancePrepaidApplyCustomerService.List(t => t.BillMonth == date.ToString("yyyy-MM") && t.CompanyID == user.CompanyID, new FinancePageModel() { Page = 0 });

            


            if (records.Count > 0)
            {
                var dics = records.Select(p => p.CustomerID).Distinct();

                foreach (var dic in dics)
                {
                    var usersRecords = records.Where(t => t.CustomerID == dic).ToList();//该客户单位下所有的下班卡考勤记录（一天的时长在下班卡记录中）
                    if (usersRecords.Count == 0) continue;
                    //var structuresCustomer = structures.Where(t => t.CustomerID == x.CustomerID).ToList();
                    var subCustomers = prepaidApplyCustomers.Where(t => t.CustomerID == dic).ToList();
                    if (subCustomers.Count > 0) continue;//该客户单位已生成结算单
                    var sub = prepaidApplyRecords.Where(t => t.CustomerID == dic).ToList();//该客户所有的日结预发
                    if (sub.Count == 0) continue;//预发记录中没有该客户单位
                    Live_AttendancePrepaidApplyCustomer customer = new Live_AttendancePrepaidApplyCustomer();
                    List<Live_AttendancePrepaidApplyRecord> subList = new List<Live_AttendancePrepaidApplyRecord>();
                    customer.GID = Guid.NewGuid();
                    customer.CreateTime = DateTime.Now;
                    customer.CompanyID = usersRecords[0].CompanyID;
                    customer.CompanyGID = usersRecords[0].CompanyGID;
                    customer.CustomerID = usersRecords[0].CustomerID;
                    customer.CustomerName = usersRecords[0].CustomerName;
                    customer.BillMonth = date.ToString("yyyy-MM");
                    customer.Status = 1;
                    //customer.StructureID = x.ID;
                    customer.ShiftHours = 0M;
                    customer.ShiftPrice = usersRecords[0].Charge;
                    customer.UserIDS = usersRecords[0].UserIDS;
                    customer.UserNames = usersRecords[0].UserNames;
                    customer.JobNo = usersRecords[0].JobNo;
                    customer.ShiftMoney = 0M;
                    customer.DayHours = 0M;
                    customer.DayPrice = 0M;
                    customer.DayMoney = 0M;
                    customer.NightHours = 0M;
                    customer.NightMoney = 0M;
                    customer.NightPrice = 0M;
                    var userArrar = sub.Select(p => p.UserID).Distinct();//该客户下日结预发的不同员工集合
                    foreach (var arr in userArrar)
                    {
                        //var applyrecords = sub.Where(t =>  t.UserID== arr).ToList();
                        var jobArray = sub.Where(t => t.UserID == arr).ToList().Select(t => t.EntryNo).Distinct();//一个员工不同岗位分别统计
                        foreach(var jarr in jobArray)
                        {
                            if (string.IsNullOrEmpty(jarr)) continue;
                            //白班统计
                            var days = sub.Where(t => t.UserID == arr && t.EntryNo == jarr && t.DayWorkType=="白班").ToList();
                            if(days.Count>0)
                            {
                                Live_AttendancePrepaidApplyRecord item = new Live_AttendancePrepaidApplyRecord();
                                item.GID = Guid.NewGuid();
                                item.CompanyID = customer.CompanyID;
                                item.CompanyGID = customer.CompanyGID;
                                item.CustomerID = customer.CustomerID;
                                item.CustomerName = customer.CustomerName;
                                item.StructureID = customer.StructureID;
                                item.StructureName = customer.StructureName;
                                item.BillMonth = customer.BillMonth;
                                item.UserID = days[0].UserID;
                                item.EntryID = days[0].UserEntryID;
                                item.UserName = days[0].Name;
                                item.CardID = days[0].IdCardNo;
                                item.Phone = days[0].Phone;
                                item.CreateTime = DateTime.Now;
                                item.JobNo = days[0].JobNo;
                                item.EntryNo = days[0].EntryNo;
                                item.ShiftMoney = 0M;
                                item.ShiftPrice = usersRecords[0].Charge ?? 0M;
                                item.JobPrice = days[0].ShiftMoney ?? 0M;//日结配置的工资(元/小时)
                                item.ShiftHours = days.Sum(t => t.MoneyHours);
                                item.DayWorkType = "白班";
                                if (!item.ShiftHours.HasValue) item.ShiftHours = 0M;
                                item.ShiftMoney = decimal.Round(item.ShiftHours.Value * item.ShiftPrice.Value, 2);
                                item.JobMoney = decimal.Round(item.ShiftHours.Value * item.JobPrice.Value, 2);
                                customer.ShiftHours += item.ShiftHours;
                                customer.ShiftMoney += item.ShiftMoney;
                                customer.DayPrice = item.JobPrice;
                                customer.DayHours += item.ShiftHours;
                                customer.DayMoney += item.JobMoney;
                                subList.Add(item);
                            }

                            //夜班统计
                            var nights = sub.Where(t => t.UserID == arr && t.EntryNo==jarr && t.DayWorkType == "夜班").ToList();
                            if(nights.Count>0)
                            {
                                Live_AttendancePrepaidApplyRecord item = new Live_AttendancePrepaidApplyRecord();
                                item.GID = Guid.NewGuid();
                                item.CompanyID = customer.CompanyID;
                                item.CompanyGID = customer.CompanyGID;
                                item.CustomerID = customer.CustomerID;
                                item.CustomerName = customer.CustomerName;
                                item.StructureID = customer.StructureID;
                                item.StructureName = customer.StructureName;
                                item.BillMonth = customer.BillMonth;
                                item.UserID = nights[0].UserID;
                                item.EntryID = nights[0].UserEntryID;
                                item.UserName = nights[0].Name;
                                item.CardID = nights[0].IdCardNo;
                                item.Phone = nights[0].Phone;
                                item.CreateTime = DateTime.Now;
                                item.JobNo = nights[0].JobNo;
                                item.EntryNo = nights[0].EntryNo;
                                item.ShiftPrice = usersRecords[0].Charge ?? 0M;
                                item.ShiftMoney = 0M;
                                item.JobPrice = nights[0].ShiftMoney ?? 0M;
                                item.ShiftHours = nights.Sum(t => t.MoneyHours);
                                item.DayWorkType = "夜班";
                                if (!item.ShiftHours.HasValue) item.ShiftHours = 0M;
                                item.ShiftMoney += decimal.Round(item.ShiftHours.Value * item.ShiftPrice.Value, 2);
                                item.JobMoney = decimal.Round(item.ShiftHours.Value * item.JobPrice.Value, 2);
                                customer.ShiftHours += item.ShiftHours;
                                customer.ShiftMoney += item.ShiftMoney;
                                customer.NightPrice = item.JobPrice;
                                customer.NightHours += item.ShiftHours;
                                customer.NightMoney += item.JobMoney;
                                subList.Add(item);
                            }
                            
                        }
                        
                    }

                    var id = await attendancePrepaidApplyCustomerService.Add(customer);
                    if (id > 0)
                    {
                        foreach (var s in subList)
                        {
                            s.PrepaidApplyCustomerID = id;
                            userList.Add(s);
                        }
                    }
                }
            }
            if (userList.Count > 0)
            {
                await attendancePrepaidApplyRecordService.Add(userList);
            }
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> Audit([FromForm] int id, [FromForm] int status)
        {
            var cid = CurrentUser.CompanyID;
            if (CurrentUser.PlatformType == Model.Enums.PlatformType.Admin)
            {
                cid = 0;
            }
            if (status == (int)AttendancePrepaidApplyCustomerStatusEnum.已审核)
            {
                var model = await attendancePrepaidApplyCustomerService.GetView(id);
                Model.Live_Invoice invo = new Live_Invoice();
                List<Live_Invoice> invos = new List<Live_Invoice>();
                invo.ID = Guid.NewGuid();
                if (model.CustomerGID.HasValue)
                {
                    invo.CustomerGID = model.CustomerGID.Value;
                }
                invo.CompanyID = cid;
                invo.CustomerName = model.CustomerName;
                //invo.IssuerName = model.CustomerName;//甲方
                //invo.IssuerCompanyNo = model.CompanyNo;
                //invo.IssuerPhone = model.Phone;
                //invo.IssuerAddress = model.Address;
                //invo.IssuerBank = model.Bank;
                //invo.IssuerBankNo = model.BankAccount;
                invo.IssuerName = "宁波莱博网络科技有限公司";
                invo.IssuerCompanyNo = "91330212MA2CH79K4J";
                invo.IssuerPhone = "0574-28856666";
                invo.IssuerAddress = "浙江省宁波高新区贵驷街道贵安路10号209-2054";
                invo.IssuerBank = "中国民生银行股份有限公司宁波东部新城支行";
                invo.IssuerBankNo = "630410756";
                invo.ApplyUserID = 8;
                invo.ApplyUserName = "lzadmin";
                invo.RecipientName = model.CustomerName;//乙方
                invo.RecipientCompanyNo = model.CompanyNo;
                invo.RecipientPhone = model.Phone;
                invo.RecipientAddress = model.Address;
                invo.RecipientBank = model.Bank;
                invo.RecipientBankNo = model.BankAccount;
                invo.InvoiceType = InvoiceTypeEnumType.ESpecialInvoice;
                invo.BusinessType = PrintTemplateEnumType.外部发票;
                invo.Title = "人力资源服务费";
                invo.IsAddedTax = false;
                invo.IsAdditionalTax = false;
                invo.AddedTaxAmount = 0;
                invo.AdditionalTaxAmount = 0;
                invo.AddedTaxDiscountAmount = 0;
                invo.AdditionalTaxDiscountAmount = 0;
                invo.ChangeAmount = 0;
                invo.IsLocked = false;
                invo.IsIssued = false;
                invo.RecordCount = 0;
                invo.ServiceFeeAmount = 0;
                invo.Description = string.Format("{0}{1}日结结算费用：{2}元",model.CustomerName,model.JobNo, model.ShiftMoney.HasValue?model.ShiftMoney.Value.ToString("#0.######"):0);
                invo.Remark = "";
                invo.ApplyTime = DateTime.Now;
                invo.CreateTime = DateTime.Now;
                invo.LockedTime = DateTime.Now;
                invo.IssuerUnitID = model.CustomerGID.Value;
                invo.RecipientUnitID = Guid.Parse("46779570-0131-4393-BE8B-0C0F34C214BD");
                invo.AddedTaxPointAmount = 0;
                invo.AdditionalTaxPointAmount = 0;
                invo.InvoiceAmount = model.ShiftMoney.HasValue? model.ShiftMoney.Value:0;
                invo.TotalAmount = model.ShiftMoney.HasValue ? model.ShiftMoney.Value : 0;
                invo.AddedTaxSumAmount = 0;
                invo.AdditionTaxSumAmount = 0;
                invo.SfAddedTaxAmount = 0;
                invo.SfAddedTaxPointAmount = 0;
                invo.SfPersonalTaxAmount = 0;
                invo.SfPersonalTaxPointAmount = 0;
                invo.IsSf = false;
                invo.SfOtherPaidAmount = 0;
                invo.SfSocialAmount = 0;
                invo.SfTaxRefundAmount = 0;
                invo.PartnerGID = model.PartnerGID.HasValue? model.PartnerGID.Value:null;
                invo.IsCancel = false;
                invo.Status = InvoiceStatusEnumType.待开具;
                invos.Add(invo);
                var datajson = JsonConvert.SerializeObject(invos);
                var msg = await httpService.PostModelAsync(ServiceNames.FinanceService, "/api/Invoice/AddList", datajson);
                
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("id", model.ID);
                dic.Add("Status", status);
                dic.Add("InvoiceID", invo.ID);
                await attendancePrepaidApplyCustomerService.DictionarySet(dic);
                return Ok(new { msg});

            }

            return Ok();

        }

        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var model = await attendancePrepaidApplyCustomerService.Get(id);
            if(model.InvoiceID.HasValue)
            {
                var invoice = await invoiceService.Get(model.InvoiceID.Value);
                return Ok(new { model, invoice });
            }
            return Ok(new { model });
        }

        [HttpGet]
        public async Task<IActionResult> BillList([FromQuery] FinancePageModel query)
        {



            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            await attendancePrepaidApplyCustomerService.Delete(id);
            await attendancePrepaidApplyRecordService.Delete(t=>t.PrepaidApplyCustomerID==id);
            return Ok();
        }

        //[HttpPost]
        //public async Task<IActionResult> BatchDelete([FromForm] DateTime start, [FromForm] DateTime end, [FromForm] DateTime end)
        //{

        //}


        [HttpGet]
        public async Task<IActionResult> BillExcel(string billMonth,int userType,int customerID)
        {
            List<Live_V_AttendancePrepaidApplyRecord> records = new List<Live_V_AttendancePrepaidApplyRecord>();
            var exp = Expressionable.Create<Model.Live_V_AttendancePrepaidApplyRecord>();
            exp.And(t => t.CustomerID == customerID && t.BillMonth == billMonth);
            //exp.AndIF(structureID.HasValue,t => t.StructureID == structureID);
            //var umodel = await authService.UserInfo<Model.Live_Personnel>();
            //if(umodel.UserType==8)
            //{
            //    exp.And(it => it.UserIDS.Contains(umodel.UserGID.Value.ToString()));
            //}
            var list = await attendancePrepaidApplyRecordService.List(exp.ToExpression(), new FinancePageModel() { Page = 0 });
            var result = attendancePrepaidApplyCustomerService.ExportExcel(list);
            string base64String = Convert.ToBase64String(result);
            return Ok(new { base64String, fileName = billMonth  + "-日结结算账单", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }

    }
}
