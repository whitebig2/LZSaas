using LZ.Basic.Service.Interface;
using LZ.Basic.Service.Services;
using LZ.Core.Attributes;
using LZ.Core.Interface;
using LZ.Core.Services;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.SS.Formula.Functions;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
using SqlSugar;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LZ.Basic.API.Controllers
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.LZMini)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    public class InsuranceSupplierBillController(IInsuranceSupplierBillService insuranceSupplierBillService, IInsuranceEmployeeBillService insuranceEmployeeBillService,IHttpService httpService) : BasicControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] Model.PagedModel.BasicPagedModel query)
        {
            var user = CurrentUser;
            var cid = user.CompanyID;
            var expable = Expressionable.Create<Model.Live_V_InsuranceSupplierBill>();
            if (user.PlatformType == Model.Enums.PlatformType.Admin || user.PlatformType == Model.Enums.PlatformType.Api)
            {
                expable.AndIF(query.CompanyID.HasValue && query.CompanyID > 0, it => it.CompanyID == query.CompanyID);
            }
            
            if (!string.IsNullOrEmpty(query.Name))
            {
                expable.And(t => t.YlSupplierName.Contains(query.Name));
            }
            if (!string.IsNullOrEmpty(query.Types))
            {
                var arr = query.Types.Split(",");
                expable.And(t => arr.Contains(t.ProcessStatus.ToString()));
            }
            if(!string.IsNullOrEmpty(query.MonthType))
            {
                var date = Convert.ToDateTime(query.BillMonth);
                switch(query.MonthType)
                {
                    case "1":
                        expable.And(t =>t.BillMonth== date);
                        break;
                    case "2":
                        expable.And(t => t.SheBaoBill == date);
                        break;
                    case "3":
                        expable.And(t => t.GongJiJinBill == date);
                        break;
                }
            }
            var exp = expable.ToExpression();

            var models = await insuranceSupplierBillService.List(exp, query, "CompanyID desc,ID desc");
            return Ok(new { items = models, count = query.Count });
        }
        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var model =  await insuranceSupplierBillService.Get(id);

            var dic = new Dictionary<string, string>();
            if(!model.CompanyInvoiceId.HasValue)
            {
                throw new ErrorCodeException(-1, "没有关联的发票");
            }
            if (!model.SheetId.HasValue)
            {
                throw new ErrorCodeException(-1, "没有关联的审批单");
            }
            dic.Add("id", model.CompanyInvoiceId.Value.ToString());
            var result = await httpService.GetAsync(ServiceNames.FinanceService, "/api/Invoice/get", dic);
            Model.Live_Invoice invoice = new Live_Invoice();
            if (!string.IsNullOrEmpty(result.Data.ToString()))
            {
                var ii = JsonConvert.DeserializeObject(result.Data.ToString()) as JObject;
                invoice = JsonConvert.DeserializeObject<Model.Live_Invoice>(ii.ToString());
            }
            else
            {
                throw new ErrorCodeException(-1, "获取发票数据失败");
            }
            var dic1 = new Dictionary<string, string>();
            dic1.Add("id", model.SheetId.Value.ToString());
            var result1 = await httpService.GetAsync(ServiceNames.FinanceService, "/api/Settlement/get", dic1);
            Model.Live_Settlement settle = new Live_Settlement();
            if (!string.IsNullOrEmpty(result1.Data.ToString()))
            {
                var ii = JsonConvert.DeserializeObject(result1.Data.ToString()) as JObject;
                settle = JsonConvert.DeserializeObject<Model.Live_Settlement>(ii.ToString());
            }
            else
            {
                throw new ErrorCodeException(-1, "获取审核单数据失败");
            }

            return Ok(new { model, invoice, settle });

            //return await userService.Get(CurrentUser.UserID);
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            if (!await insuranceSupplierBillService.ExistID(id))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            var model = await insuranceSupplierBillService.Get(id);
            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (model.CompanyInvoiceId.HasValue)
            {
                dic.Add("id", model.CompanyInvoiceId.Value.ToString());
                await httpService.DeleteAsync(ServiceNames.FinanceService, "/api/Invoice/JoinDelete", dic);
            }
           

            Dictionary<string, string> dic1 = new Dictionary<string, string>();
            if (model.SheetId.HasValue)
            {
                dic1.Add("id", model.SheetId.Value.ToString());
                await httpService.DeleteAsync(ServiceNames.FinanceService, "/api/Settlement/JoinDelete", dic1);
            }
            




            await insuranceSupplierBillService.Delete(id);
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> UpdateStatus([FromForm] int id, [FromForm] int? status, [FromForm] bool isOK, [FromForm] string name, [FromForm] decimal actualAmount, [FromForm] string remark)
        {
            var objs = new Dictionary<string, object>();
            objs.Add("ID", id);
            if (status.HasValue && status>0)
            {

                objs.Add("ProcessStatus", status.Value);
            }
            if (!string.IsNullOrEmpty(name))
            {
                var model = await insuranceSupplierBillService.Get(id);
                Dictionary<string, string> dic = new Dictionary<string, string>();

                if (!model.CompanyInvoiceId.HasValue)
                {
                    throw new ErrorCodeException(-1, "该账单没有关联发票");
                }
                dic.Add("id", model.CompanyInvoiceId.Value.ToString());
                bool isPond = false;
                if (name == "isInvoicePaid")//发票已开具
                {
                    objs.Add(name, isOK);
                    dic.Add("status", ((int)InvoiceStatusEnumType.已开具).ToString());
                }
                else if (name == "push")
                {
                    dic.Add("status", ((int)InvoiceStatusEnumType.待开具).ToString());
                }
                else if (name == "back")
                {
                    dic.Add("status", ((int)InvoiceStatusEnumType.待锁定).ToString());
                }
                else if (name == "pond")
                {
                    dic.Add("status", ((int)InvoiceStatusEnumType.冲红).ToString());
                    isPond = true;
                }
                else if (name == "edit")
                {

                    objs.Add("ActualAmount", actualAmount);
                    objs.Add("Remark", remark);
                    dic.Add("totalAmount", actualAmount.ToString());
                    dic.Add("remark", remark);
                    if (model.SheetId.HasValue)
                    {
                        Dictionary<string, string> dic_settle = new Dictionary<string, string>();
                        dic_settle.Add("id", model.SheetId.Value.ToString());
                        dic_settle.Add("totalAmount", actualAmount.ToString());
                        dic_settle.Add("remark", remark);
                        await httpService.PutAsync(ServiceNames.FinanceService, "/api/Settlement/JoinUpdate", dic_settle);
                    }


                }
                else if (name == "isSettlementPass")
                {
                    objs.Add(name, isOK);
                    Dictionary<string, string> dic_settle = new Dictionary<string, string>();
                    dic_settle.Add("id", model.SheetId.Value.ToString());
                    dic_settle.Add("insuranceStatus", "2");
                    await httpService.PutAsync(ServiceNames.FinanceService, "/api/Settlement/JoinUpdate", dic_settle);

                }
                    //var datajson = JsonConvert.SerializeObject(dic);
                if(isPond)
                {
                    
                    await httpService.PutAsync(ServiceNames.FinanceService, "/api/Invoice/UpdateStatus", dic);
                }
                else
                {
                    await httpService.PutAsync(ServiceNames.FinanceService, "/api/Invoice/JoinUpdate", dic);
                }
                    
            }
            //if (!string.IsNullOrEmpty(name))
            //{
            //    objs.Add(name, isOK);
            //    if (name == "isInvoicePaid")//发票已开具
            //    {
            //        var model = await insuranceSupplierBillService.Get(id);
            //        Dictionary<string, string> dic = new Dictionary<string, string>();
            //        if (!model.CompanyInvoiceId.HasValue)
            //        {
            //            throw new ErrorCodeException(-1, "该账单没有关联发票");
            //        }
            //        dic.Add("id", model.CompanyInvoiceId.Value.ToString());
            //        dic.Add("status", ((int)InvoiceStatusEnumType.已开具).ToString());
            //        var datajson = JsonConvert.SerializeObject(dic);
            //        await httpService.PutAsync(ServiceNames.FinanceService, "/api/Invoice/UpdateStatus", dic);
            //    }
            //}

            var count = await insuranceSupplierBillService.DictionarySet(objs);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> BatchEmployeeCreate([FromForm] DateTime date)
        {
            try
            {
                List<Live_Invoice> invos = new List<Live_Invoice>();
                List<Live_Settlement> settles = new List<Live_Settlement>();
                var cid = CurrentUser.CompanyID;
                if (CurrentUser.PlatformType == Model.Enums.PlatformType.Admin)
                {
                    cid = 0;
                }
                var batchModel = await insuranceSupplierBillService.BatchEmployeeCreate(date, cid);
                var bills = batchModel.list;
                if (bills.Count > 0)
                {
                    foreach (var bill in bills)
                    {

                        Model.Live_Invoice invo = new Live_Invoice();

                        invo.ID = Guid.NewGuid();
                        bill.CompanyInvoiceId = invo.ID;
                        invo.CustomerGID = bill.CompanyGID.Value;
                        invo.CompanyID = cid;
                        invo.IssuerName = bill.InvoiceCompanyName;//甲方
                        invo.IssuerCompanyNo = bill.SupplierCompanyNo;
                        invo.IssuerPhone = "";
                        invo.IssuerAddress = "";
                        invo.IssuerBank = bill.SupplierCompanyBank;
                        invo.IssuerBankNo = bill.SupplierCompanyBankNo;
                        invo.ApplyUserID = 8;
                        invo.ApplyUserName = "lzadmin";
                        invo.RecipientName = bill.CooperationName;//乙方
                        invo.RecipientCompanyNo = bill.CooperationNo;
                        invo.RecipientPhone = bill.CooperationPhone;
                        invo.RecipientAddress = bill.CooperationAddress;
                        invo.RecipientBank = bill.CooperationBank;
                        invo.RecipientBankNo = bill.CooperationBankNo;
                        invo.InvoiceType = InvoiceTypeEnumType.ESpecialInvoice;
                        invo.BusinessType = PrintTemplateEnumType.社保公积金发票;
                        invo.Title = "人力资源服务*经纪人代理服务";
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
                        //invo.Description = string.Format("支付{0}社保{1}元,{4}公积金：{2}元,服务费{3}元", date.AddMonths(-1).ToString("yyyy年MM月"), bill.TotalSheBao.ToString("#0.######"), bill.TotalGongJiJin.ToString("#0.######"), bill.TotalServiceFee.ToString("#0.######"), date.ToString("MM月"));
                        invo.Description = string.Format("支付{0}社保:{1}元,{4}公积金：{2}元,服务费:{3}元", date.AddMonths(-1).ToString("yyyy年MM月"), Math.Round(bill.TotalSheBao, 2), Math.Round(bill.TotalGongJiJin, 2), Math.Round(bill.TotalServiceFee, 2), date.ToString("MM月"));
                        invo.Remark = "";
                        invo.ApplyTime = date;
                        invo.CreateTime = DateTime.Now;
                        invo.IssuerUnitID = bill.YlSupplierGID.Value;
                        invo.RecipientUnitID = bill.CooperationUnitGID.Value;
                        invo.AddedTaxPointAmount = 0;
                        invo.AdditionalTaxPointAmount = 0;
                        invo.InvoiceAmount = bill.TotalAmount;
                        invo.TotalAmount = bill.TotalAmount;
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
                        invo.IsCancel = false;
                        invo.Status = InvoiceStatusEnumType.待锁定;
                        invos.Add(invo);

                        Model.Live_Settlement settle = new Live_Settlement();
                        settle.ID = Guid.NewGuid();
                        bill.SheetId = settle.ID;
                        settle.CustomerGID = Guid.Parse("09043276-874C-4DBC-B858-ECE1C249471B");
                        settle.IssuerUnitID = bill.CooperationUnitGID;
                        settle.IssuerName = bill.CooperationName;
                        settle.IssuerCompanyNo = bill.CooperationNo;
                        settle.IssuerPhone = bill.CooperationPhone;
                        settle.IssuerAddress = "";
                        settle.IssuerBank = bill.CooperationBank;
                        settle.IssuerBankNo = bill.CooperationBankNo;


                        settle.RecipientUnitID = bill.YlSupplierGID;
                        settle.RecipientName = bill.InvoiceCompanyName;
                        settle.RecipientCompanyNo = bill.SupplierCompanyNo;
                        settle.RecipientPhone = "";
                        settle.RecipientAddress = "";
                        settle.RecipientBank = bill.SupplierCompanyBank;
                        settle.RecipientBankNo = bill.SupplierCompanyBankNo;
                        settle.BusinessType = SettlementBusinessTypeEnumType.友联审批单;
                        settle.InsuranceStatus = 1;
                        settle.TotalAmount = bill.TotalAmount;
                        settle.Title = "社保公积金";
                        settle.Description = string.Format("支付{0}社保:{1}元,{4}公积金：{2}元,服务费:{3}元", date.AddMonths(-1).ToString("yyyy年MM月"), Math.Round(bill.TotalSheBao, 2), Math.Round(bill.TotalGongJiJin, 2), Math.Round(bill.TotalServiceFee, 2), date.ToString("MM月"));
                        //settle.Description = string.Format("支付{0}社保公积金费用{1}元", date.ToString("yyyy年MM月"), bill.TotalAmount);
                        settle.CreateTime = DateTime.Now;
                        settle.ApplyDate = DateTime.Now;
                        settle.SettlementType = 1;
                        settle.CustomerName = bill.InvoiceCompanyName;
                        settle.CustomerCompanyNo = bill.SupplierCompanyNo;
                        settle.CustomerBank = bill.SupplierCompanyBank;
                        settle.CustomerBankNo = bill.SupplierCompanyBankNo;
                        settles.Add(settle);
                    }
                    //await Adds(bills);
                }
                await insuranceSupplierBillService.CheckAndSaveCompany(settles);
                var datajson = JsonConvert.SerializeObject(invos);
                var msg1= await httpService.PostModelAsync(ServiceNames.FinanceService, "/api/Invoice/AddList", datajson);
                var datajson1 = JsonConvert.SerializeObject(settles);
               var msg = await httpService.PostModelAsync(ServiceNames.FinanceService, "/api/Settlement/AddList", datajson1);
                await insuranceSupplierBillService.Adds(bills);
                return Ok(new { item = bills,msg=batchModel.msg });
            }
            catch(Exception ex)
            {
                throw new ErrorCodeException(-1, ex.ToString());
            }
            
            
        }

        [HttpGet]
        public async Task<IActionResult> SupplierBillExcel(int id)
        {
            var model = await insuranceSupplierBillService.Get(id);
            var result = await insuranceSupplierBillService.SupplierBillExcel(model);
            string base64String = Convert.ToBase64String(result);
            return Ok(new { base64String, fileName = model.YlSupplierName+"-供应商账单", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }



    }
}
