using LZ.Core.Services;
using LZ.Model;
using LZ.Model.Exceptions;
using LZ.Users.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using SqlSugar;
using LZ.Core.Interface;
using LZ.Model.Enums;
using LZ.Model.ModelExtensions;
using System.Collections.Generic;
using Senparc.CO2NET.Cache.Redis;
using LZ.Core.Attributes;
using LZ.Users.Service.Services;
using System.Text.RegularExpressions;

namespace LZ.Users.API.Controllers.Injury
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.LZMini)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    public class InjuryElectronicDocumentsController(IInjuryDocumentLabelService injuryDocumentLabelService,IInjuryElectronicDocumentsService injuryElectronicDocumentsService, IHttpService httpService,IInjuryInfoService injuryInfoService,IBusinessInsuranceRecordService businessInsuranceRecordService,IBusinessInsuranceOrderService businessInsuranceOrderService,IBusinessGuaranteeService businessGuaranteeService) : UsersControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> LabelList(int injuryID)
        {
            var info = await injuryInfoService.Get(injuryID);
            //var expable = Expressionable.Create<Model.Live_BusinessInsuranceRecord>();
            //if (!string.IsNullOrEmpty(info.UserCardNo))
            //{
            //    expable.And(it => it.IdCardNo == info.UserCardNo);
            //}
            //expable.And(it => it.IsMain);
            //expable.AndIF(info.InjuryTime.HasValue, t => t.EndDate.HasValue && t.StartDate < info.InjuryTime && t.EndDate.Value.AddDays(1) > info.InjuryTime);
            //var exp = expable.ToExpression();
            //var Insurance = await businessInsuranceRecordService.Get(exp);
            if (string.IsNullOrEmpty(info.InsuranceId))
            {
                throw new ErrorCodeException(-1, "未设置保险订单号");
            }
            var insuranceOrder = await businessInsuranceOrderService.Get(t => t.InsuranceId == info.InsuranceId);
            if (insuranceOrder == null)
            {
                throw new ErrorCodeException(-1, "保险订单号不存在");
            }
            string name = insuranceOrder.InsuranceCompany;

            //var main = Insurances.Where(t => t.IsMain == true).FirstOrDefault();
            //if (string.IsNullOrEmpty(main.InsuranceCompany))
            //{
            //    throw new ErrorCodeException(-1, "该员工没有主保险");
            //}
            var label = await injuryDocumentLabelService.GetByName(name);
            if(label==null)
            {
                throw new ErrorCodeException(-1, "未找到匹配的保险配置");
            }
            string ids = "";
            if (!string.IsNullOrEmpty(label.WorkInjuryReportFileID))
            {
                ids = label.WorkInjuryReportFileID;
            }
            if(!string.IsNullOrEmpty(label.NoticeLetterFileID))
            {
                ids += "," + label.NoticeLetterFileID;
            }
            if (!string.IsNullOrEmpty(label.AuthorizationLetterFileID))
            {
                ids += "," + label.AuthorizationLetterFileID;
            }
            if (!string.IsNullOrEmpty(label.CompensationAgreementLetterFileID))
            {
                ids += "," + label.CompensationAgreementLetterFileID;
            }
            if (!string.IsNullOrEmpty(label.CompensationAgreementDisabilityLetterFileID))
            {
                ids += "," + label.CompensationAgreementDisabilityLetterFileID;
            }


            var paras = new Dictionary<string, string>();
            paras.Add("Page", "0");
            paras.Add("IDS", ids);
            var obj = await httpService.GetAsync(ServiceNames.CustomerService, "/api/ContractFileConfig/list", paras);
            List<Model.Live_ContractFileConfig> contracts = new List<Live_ContractFileConfig>();
            JObject? aa11;
            if (!string.IsNullOrEmpty(obj.Data.ToString()))
            {
                var ii = JsonConvert.DeserializeObject(obj.Data.ToString()) as JObject;
                contracts = JsonConvert.DeserializeObject<List<Model.Live_ContractFileConfig>>(ii["items"].ToString());
                aa11 = ii;
            }
            if(contracts==null)
            {
                throw new ErrorCodeException(-1, "未找到保险合同");
            }
            Model.PagedModel.UserPagedModel query = new Model.PagedModel.UserPagedModel();
            query.Page = 0;
            //var labels = await injuryDocumentLabelService.List(expable.ToExpression(), query);

            var documentExpable = Expressionable.Create<Model.Live_InjuryElectronicDocuments>();
            documentExpable.And(t => t.InjuryID == injuryID);
            var documents = await injuryElectronicDocumentsService.List(documentExpable.ToExpression()
                , query);
            List<Model.Model.InjuryDocument> models = new List<Model.Model.InjuryDocument>();
            foreach (var x in contracts)
            {
                Model.Model.InjuryDocument model = new Model.Model.InjuryDocument();
                model.ID = x.ID;
                model.InjuryID = injuryID;
                model.Name = x.Name;
                model.TypeName = x.ContractEnumType.ToString();
                model.isOK = false;
                var item = documents.Where(t => t.ContractFileConfigID == x.ID.ToString()).FirstOrDefault();
                if (item != null)
                {
                    model.ElectronicDocumentID = item.ID;
                    model.isOK = true;
                    model.Path = item.Path;
                }
                models.Add(model);
            }
            return Ok(new {items = models });
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Model.Live_InjuryElectronicDocuments model)
        {
            
            await injuryElectronicDocumentsService.Add(model);
            return Ok();
        }

        //[HttpPost]
        //public async Task<IActionResult> CreateDocument([FromBody] Model.Model.InjuryDocument model)
        //{
        //    var info = await injuryInfoService.Get(model.InjuryID);
        //    Dictionary<string, string> Keywords = new Dictionary<string, string>();
        //    Keywords.Add("workInjuryTime", "工伤日期123");// JsonSerializer.Serialize(dictionary)
        //    string jsonString = JsonConvert.SerializeObject(model);

        //    Dictionary<string, string> paras = new Dictionary<string, string>();
        //    paras.Add("CustomerGID", info.CustomerGID.ToString());
        //    paras.Add("ContractFileID", model.ID.ToString());
        //    paras.Add("Keywords", jsonString);



        //    Live_ContractRecordExtension contractRecord = null;
        //    var obj = await httpService.GetAsync(ServiceNames.CustomerService, "/api/fxqApp/generatePdfContractByFileID", paras);
        //    List<Model.Live_ContractFileConfig> contracts = new List<Live_ContractFileConfig>();
        //    if (!string.IsNullOrEmpty(obj.Data.ToString()))
        //    {
        //        var ii = JsonConvert.DeserializeObject(obj.Data.ToString()) as JObject;
        //        contractRecord = JsonConvert.DeserializeObject<Live_ContractRecordExtension>(ii["items"].ToString());
        //    }
        //    return Ok();
        //}

        [HttpPost]
        public async Task<IActionResult> CreateDocument([FromForm] int injuryID, [FromForm] string gid)
        {
            var info = await injuryInfoService.GetView(injuryID);

            try
            {
                //var insuracneDic = new Dictionary<string, string>();
                //insuracneDic.Add("customerName", info.CustomerName);
                //insuracneDic.Add("name", info.UserName);
                //if (info.InjuryTime.HasValue)
                //{
                //    insuracneDic.Add("time", info.InjuryTime.ToString());
                //}
                //insuracneDic.Add("main", "true");
                //var result = await httpService.GetAsync(ServiceNames.UsersService, "/api/BusinessInsuracneRecord/get", insuracneDic);
                //Model.Live_BusinessInsuranceRecord insurance = new Live_BusinessInsuranceRecord();
                //if (!string.IsNullOrEmpty(result.Data.ToString()))
                //{
                //    insurance = JsonConvert.DeserializeObject<Model.Live_BusinessInsuranceRecord>(result.Data.ToString());
                //}
                var expable = Expressionable.Create<Model.Live_BusinessInsuranceRecord>();
                if (!string.IsNullOrEmpty(info.UserCardNo))
                {
                    expable.And(it => it.IdCardNo == info.UserCardNo);
                }
                expable.And(it => it.IsMain);
                expable.AndIF(info.InjuryTime.HasValue, t => t.EndDate.HasValue &&  t.StartDate < info.InjuryTime && t.EndDate.Value.AddDays(1) > info.InjuryTime);
                var exp = expable.ToExpression();
                var insurance = await businessInsuranceRecordService.Get(exp);
                var insuranceOrder = await businessInsuranceOrderService.Get(t => t.InsuranceId == info.InsuranceId);
                if (insuranceOrder == null)
                {
                    throw new ErrorCodeException(-1, "保险订单号不存在");
                }
                var guarantee = await businessGuaranteeService.Get(t => t.InsuranceId == info.InsuranceId);
                Dictionary<string, string> dic = new Dictionary<string, string>();
                if (guarantee!=null)
                {
                    dic.Add("insuranceType", guarantee.PlanName ?? "");
                }
                var totalCompensationAmount = info.StaffLossOfWorkingTime + info.StaffHospitalizationAllowance + info.StaffMedicalExpense;
                int monthinday = DateTime.DaysInMonth(info.InjuryTime.Value.Year, info.InjuryTime.Value.Month);
                string period = info.InjuryTime.Value.ToString("yyyy年MM月01日") + "至" + info.InjuryTime.Value.ToString("yyyy年MM月"+monthinday+"日");
                
                dic.Add("agentBankUserName", info.AgentName?? "");
                dic.Add("amountChargeForLossOfWorkingTime", info.StaffLossOfWorkingTime.ToString());
                dic.Add("amountHospitalizationAllowance", info.StaffHospitalizationAllowance.ToString());
                dic.Add("amountMedicalExpenses", info.StaffMedicalExpense.ToString());
                dic.Add("amountOneTimeSubsidy", info.StaffOneTimePackAmount.ToString());
                dic.Add("disabilityAssessed", info.IsDisabilityAssessed?"是":"否");
                dic.Add("estimatedLosses", info.AdvanceApplyAmount.ToString() + "元");
                dic.Add("insurancePeriod", period);
                dic.Add("insurancePolicyNo", insuranceOrder.InsuranceNo);//保险保单号
                
                dic.Add("signatureCompany", info.SealCompanyName);
                dic.Add("signatureCompanyAddress", info.SealCompanyAddress);
                dic.Add("signatureCompanyBank", info.SealCompanyBank);
                dic.Add("signatureCompanyBankNo", info.SealCompanyBankNo);
                dic.Add("totalCompensationAmount", totalCompensationAmount.ToString());
                dic.Add("totalCompensationAmountWords", LZ.Common.MoneyTypeChange.MoneyToChinese(totalCompensationAmount));
                dic.Add("workInjuryEmployeeBank", info.UserBank?? "");
                dic.Add("workInjuryEmployeeBankNo", info.UserBankNo??"");
                
                dic.Add("workInjuryEmployeeCompanyName", info.CustomerName??"");
                dic.Add("workInjuryEmployeeIdcardNo", info.UserCardNo??"");
                dic.Add("workInjuryEmployeeName", info.UserName??"");
                dic.Add("workInjuryEmployeePhone", info.UserPhone??"");
                dic.Add("workInjuryEmployeeRegisteredResidence", info.UserRegisteredResidence ?? "");
                dic.Add("workInjuryEmployeeType", info.UserType);
                string report = "";
                if(insuranceOrder.InsuranceCompany== "阳光保险公司")
                {
                    dic.Add("workInjuryLocation", info.CustomerName);
                    dic.Add("workInjuryEmployeeCompanyAddress", info.CustomerName ?? "");
                    //report = info.UserName + "工作于" + info.CustomerName + ",在" + (info.HospitalName ?? "医院名称") + "就诊," + (info.Diagnosis ?? "诊断结果");
                    report = info.UserName + info.InjuryDescription + ",在" + (info.HospitalName ?? "医院名称") + "就诊,诊断为：" + (info.Diagnosis ?? "诊断结果");
                    //report = info.UserName + "工作于" + info.CustomerName + ",在" + (info.HospitalName ?? "医院名称") + "就诊," + (info.Diagnosis ?? "诊断结果");
                }
                else if (insuranceOrder.InsuranceCompany == "大地财险")
                {
                    dic.Add("workInjuryLocation",info.Province+info.City+info.District);
                    dic.Add("workInjuryEmployeeCompanyAddress", info.Province + info.City + info.District);
                    dic.Add("staffMedicalExpense", info.StaffMedicalExpense.ToString()+"元");
                    dic.Add("staffLossOfWorkingTime", info.StaffLossOfWorkingTime.ToString() + "元");
                    dic.Add("staffHospitalizationMealAllowance", info.StaffHospitalizationMealAllowance.ToString() + "元");
                    dic.Add("staffHospitalizationAllowance", info.StaffHospitalizationAllowance.ToString() + "元");
                    report = info.UserName + "工作期间" + (info.Diagnosis ?? "诊断结果");
                }
                report = Regex.Replace(report, @"[\r\n]", "");
                dic.Add("workInjuryReport", report);

                dic.Add("workInjuryTime", info.InjuryTime.Value.ToString("yyyy年MM月dd日"));
                dic.Add("workInjuryType", info.InjuryType);
                foreach(var x in dic)
                {
                    if(x.Value==null)
                    {
                        return Ok(new { msg = x.Key+"参数为空,验签失败" });
                    }
                }
                var newDic = dic.Where(t=>t.Value!=null && t.Value!="").OrderBy(t => t.Key).ToDictionary();
                
                var model = new { CustomerGID = info.CustomerGID.ToString(), ContractFileID = gid, Keywords = newDic };
                var modeljson = JsonConvert.SerializeObject(model);
                //paras.Add("request", aajson);
                Live_ContractRecordExtension contractRecord = null;
                var obj = await httpService.PostModelAsync(ServiceNames.CustomerService, "/api/fxqApi/generatePdfContractByFileID", modeljson);
                Model.Live_InjuryElectronicDocuments document = new Live_InjuryElectronicDocuments();
                List<Model.Live_ContractFileConfig> contracts = new List<Live_ContractFileConfig>();
                if (obj.Data!=null)
                {
                    var ii = JsonConvert.DeserializeObject(obj.Data.ToString()) as JObject;
                    contractRecord = JsonConvert.DeserializeObject<Live_ContractRecordExtension>(ii.ToString());
                   
                    document.CompanyID = info.CompanyID;
                    document.CompanyGID = info.CompanyGID;
                    document.InjuryID = info.ID;
                    document.Path = contractRecord.ContractOssUrl;
                    document.ContractFileConfigID = gid;
                    await injuryElectronicDocumentsService.Add(document);
                    return Ok(new { items = document, data = newDic });
                }
                else
                {
                    return Ok(new { msg = obj.Message,data = newDic });
                }
                
            }
            catch(Exception e)
            {
                throw new ErrorCodeException(-1, e.ToString());
            }
            

           
            //ContractSigner 盖章信息
            //List<object> singers = new List<object>();
            //var signer = new { Name = info.SealCompanyName, IdNo = info.SealCompanyNo, IsCompany = true, SignerPartyType = SignerPartyEnumType.第一当事人, IsNeedSendSms = false, SmsPhone = "", sealOssFilePath = "" };
            //singers.Add(signer);
           
            
           
          
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            if (!await injuryElectronicDocumentsService.ExistID(id))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            await injuryElectronicDocumentsService.Delete(id);
            return Ok();
        }

    }
}
