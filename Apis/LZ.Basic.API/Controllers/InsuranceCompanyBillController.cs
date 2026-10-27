using LZ.Basic.Service.Interface;
using LZ.Basic.Service.Services;
using LZ.Model.Exceptions;
using LZ.Model;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using LZ.Core.Attributes;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Http.HttpResults;
using LZ.Core.Interface;
using LZ.Core.Services;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
using Consul;
using Org.BouncyCastle.Crypto.Operators;
using LZ.Model.Enums;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Runtime.CompilerServices;

namespace LZ.Basic.API.Controllers
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.LZMini)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    public class InsuranceCompanyBillController(IInsuranceCompanyBillService insuranceCompanyBillService,IHttpService httpService) : BasicControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] Model.PagedModel.BasicPagedModel query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Live_V_InsuranceCompanyBill>();
            if (user.PlatformType == Model.Enums.PlatformType.Admin || user.PlatformType == Model.Enums.PlatformType.Api)
            {
                expable.AndIF(query.CompanyID.HasValue && query.CompanyID > 0, it => it.CompanyID == query.CompanyID);
            }
            //else
            //{
            //    expable.And(it => it.CompanyID == CurrentUser.CompanyID);
            //}
            if (!string.IsNullOrEmpty(query.Name))
            {
                expable.And(t => t.EmployeeCompanyName.Contains(query.Name));
            }
            if (!string.IsNullOrEmpty(query.Types))
            {
                var arr = query.Types.Split(",");
                expable.And(t => arr.Contains(t.ProcessStatus.ToString()));
            }
            if(!string.IsNullOrEmpty(query.BillMonth))
            {
                var date = Convert.ToDateTime(query.BillMonth);
                expable.And(t => t.BillMonth == date);
            }
            if(query.Status.HasValue)
            {
                expable.And(t => t.ProcessStatus == query.Status);
            }
            var exp = expable.ToExpression();
            
            var models = await insuranceCompanyBillService.List(exp, query, "CompanyID desc,ID desc");
            return Ok(new { items = models, count = query.Count });
        }
        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var model = await insuranceCompanyBillService.Get(id);
            try
            {
                var dic = new Dictionary<string, string>();
                dic.Add("id", model.CompanyInvoiceId.ToString());
                var insuracne = await httpService.GetAsync(ServiceNames.FinanceService, "/api/Invoice/get", dic);
                Model.Live_Invoice invoice = new Live_Invoice();
                if (!string.IsNullOrEmpty(insuracne.Data.ToString()))
                {
                    var ii = JsonConvert.DeserializeObject(insuracne.Data.ToString()) as JObject;
                    invoice = JsonConvert.DeserializeObject<Model.Live_Invoice>(ii.ToString());
                }
                return Ok(new { model, invoice });
            }
            catch(Exception e)
            {
                throw new ErrorCodeException(-1, "没有生成发票");
            }
           
            //return await userService.Get(CurrentUser.UserID);
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            if (!await insuranceCompanyBillService.ExistID(id))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            var model = await insuranceCompanyBillService.Get(id);
            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (!model.CompanyInvoiceId.HasValue)
            {
                throw new ErrorCodeException(-1, "该账单没有关联发票");
            }
            dic.Add("id", model.CompanyInvoiceId.Value.ToString());
            await httpService.DeleteAsync(ServiceNames.FinanceService, "/api/Invoice/JoinDelete", dic);
            await insuranceCompanyBillService.Delete(id);
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> BatchEmployeeCreate([FromForm] DateTime date)
        {
            var cid = CurrentUser.CompanyID;
            var item = await insuranceCompanyBillService.BatchEmployeeCreate(date,cid);
            var datajson = JsonConvert.SerializeObject(item.list);
            var obj = await httpService.PostModelAsync(ServiceNames.FinanceService, "/api/Invoice/AddList", datajson);
            
            return Ok(new {items = obj,msg = item.msg});
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus([FromForm] int id, [FromForm] int? status, [FromForm] bool isOK,[FromForm] string name, [FromForm] decimal actualAmount, [FromForm] string remark)
        {
            var objs = new Dictionary<string, object>();
            objs.Add("ID", id);
            if(status.HasValue)
            {
                objs.Add("ProcessStatus", status.Value);
            }
            string a = "";
            if (!string.IsNullOrEmpty(name))
            {
                var model = await insuranceCompanyBillService.Get(id);
                Dictionary<string, string> dic = new Dictionary<string, string>();
                bool isPond = false;
                if (!model.CompanyInvoiceId.HasValue)
                {
                    throw new ErrorCodeException(-1, "该账单没有关联发票");
                }
                dic.Add("id", model.CompanyInvoiceId.Value.ToString());
                if (name== "isInvoicePaid")//发票已开具
                {
                    objs.Add(name, isOK);
                    dic.Add("status", ((int)InvoiceStatusEnumType.已开具).ToString());
                    
                }
                else if (name == "push")
                {
                    a += "aa";
                    dic.Add("status", ((int)InvoiceStatusEnumType.待开具).ToString());
                }
                else if (name == "back")
                {
                    dic.Add("status", ((int)InvoiceStatusEnumType.待锁定).ToString());
                }

                else if (name == "edit")
                {
                    objs.Add("ActualAmount", actualAmount);
                    objs.Add("Remark", remark);
                    dic.Add("totalAmount", actualAmount.ToString());
                    dic.Add("remark", remark);
                }
                else if (name == "pond")
                {
                    dic.Add("status", ((int)InvoiceStatusEnumType.冲红).ToString());
                    isPond = true;
                }
                //var datajson = JsonConvert.SerializeObject(dic);
                if (isPond)
                {

                    await httpService.PutAsync(ServiceNames.FinanceService, "/api/Invoice/UpdateStatus", dic);
                }
                else
                {
                    a += "bb";
                    await httpService.PutAsync(ServiceNames.FinanceService, "/api/Invoice/JoinUpdate", dic);
                }

            }
            
            var count = await insuranceCompanyBillService.DictionarySet(objs);
            return Ok(new {a});
        }

        [HttpGet]
        public async Task<IActionResult> CompanyBillExcel(int id)
        {
            var model = await insuranceCompanyBillService.Get(id);
            var result = await insuranceCompanyBillService.CompanyBillExcel(model);
            string base64String = Convert.ToBase64String(result);
            return Ok(new { base64String, fileName = model.EmployeeCompanyName + "-公司账单", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }

        [HttpGet]
        public async Task<IActionResult> PayNoticeExcel(int id)
        {
            var model = await insuranceCompanyBillService.Get(id);
            var result = insuranceCompanyBillService.PayNoticeExcel(model);
            string base64String = Convert.ToBase64String(result);
            return Ok(new { base64String, fileName = model.EmployeeCompanyName + "-付款通知书", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }

    }
}
