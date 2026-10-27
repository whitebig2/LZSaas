using LZ.Core.Interface;
using LZ.Model.Exceptions;
using LZ.Users.Service.Interface;
using LZ.Users.Service.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using SqlSugar;
using LZ.Core;
using LZ.Model.Enums;
using System.IO;
using NPOI.SS.Formula.Functions;
using System.Security.Cryptography;
using Senparc.Weixin.WxOpen.Entities;
using NPOI.SS.UserModel;
using LZ.Core.Attributes;
using LZ.Core.Services;
using LZ.Model;
using LZ.Model.ModelExtensions;
using LZ.ApiService.Services;
using LZ.Model.FxqModel;
using LZ.ApiService.Interface;
using System.Security.Policy;
using System;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Core.EventData;
using LZ.Core.CommonHandle;

namespace LZ.Users.API.Controllers
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.LZMini)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    public class InjuryApplyRecordController(IInjuryApplyRecordService injuryApplyRecordService, IInjuryInfoService injuryInfoService, IFileUploadService fileUploadService,IInjuryFinancialWriteOffService injuryFinancialWriteOffService, IHttpService httpService, IInjuryDocumentLabelService injuryDocumentLabelService, IInjuryElectronicDocumentsService injuryElectronicDocumentsService,IFxqService fxqService, IAuthService authService,IBusinessInsuranceRecordService businessInsuranceRecordService, IEventPublisher eventPublisher, ISendMQService sendMQService, IBusinessInsuranceOrderService businessInsuranceOrderService, IBusinessGuaranteeService businessGuaranteeService,IInjuryAdvanceAccountService injuryAdvanceAccountService) : UsersControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] Model.PagedModel.UserPagedModel query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Live_V_InjuryApplyRecord>();
            if (user.PlatformType == Model.Enums.PlatformType.Admin || user.PlatformType == Model.Enums.PlatformType.Api)
            {
                expable.AndIF(query.CompanyID.HasValue && query.CompanyID > 0, it => it.CompanyID == query.CompanyID);
            }
            //else
            //{
            //    expable.And(it => it.CompanyID == CurrentUser.CompanyID);
            //}
            if (query.CID.HasValue)
            {
                expable.And(t => t.InjuryID == query.CID);
            }
            if (query.IsUser.HasValue)
            {
                expable.And(t => t.UserEntryInjuryStatus == 1);
            }
            if (!string.IsNullOrEmpty(query.CustomerName))
            {
                expable.And(t => t.CustomerName.Contains(query.CustomerName));
            }
            if (!string.IsNullOrEmpty(query.UserName))
            {
                expable.And(t => t.UserName.Contains(query.UserName));
            }
            if(query.Status.HasValue)
            {
                expable.And(t => t.Status == query.Status);
            }
            if (query.From == "loan")
            {
                expable.And(t => t.ProcessStatus != 9 && t.ProcessStatus != 2);
            }
            else if (query.From == "repay")
            {
                expable.And(t=>t.Status !=3);//审核通过不显示
            }
            else if (query.From == "loanlist")
            {
                expable.And(t =>t.ProcessStatus != 9 && t.ProcessStatus != 2);
            }
            if (!string.IsNullOrEmpty(query.Type))
            {
                expable.And(t => t.MoneyType == Convert.ToInt32(query.Type));
            }
            expable.And(t => t.CustomerName != null);
            //var puser = await authService.UserInfo<Model.Live_Personnel>();
            //expable.And(UserEntryHandle.SetPersonnelAuth<Model.Live_V_InjuryApplyRecord>(puser));

            //expable.And(UserEntryHandle.SetPersonnelOperateAuth<Model.Live_V_InjuryApplyRecord>(puser));
            if(user.PlatformType == Model.Enums.PlatformType.LZMini)
            {
                var puser = await authService.UserInfo<Model.Live_Personnel>();
                if (!puser.UserType.HasValue)
                {
                    puser.UserType = (int)Model.Enums.PersonnelType.Personnel;
                }
                var type = (Model.Enums.PersonnelType)puser.UserType;
                switch (type)
                {
                    case PersonnelType.Personnel:
                    case PersonnelType.View:
                        {
                            expable.And(t => SqlFunc.Subqueryable<Model.Live_PersonnelItems>().Where(s => s.CID == t.CustomerID && s.UserID == puser.UserID).Any());
                        }
                        break;
                    case PersonnelType.Factory:
                        {
                            expable.And(t => SqlFunc.Subqueryable<Model.Live_PersonnelItems>().Where(s => s.CID == t.CustomerID && s.UserID == puser.UserID && s.IsManage).Any());
                        }
                        break;
                    case PersonnelType.Headed:
                        {
                            expable.And(t => SqlFunc.Subqueryable<Model.Live_PersonnelItems>().Where(s => s.CID == t.CustomerID && s.UserID == puser.UserID && s.IsManage).Any() || t.CustomerPartnerID == puser.PartnerID);
                        }
                        break;
                    case PersonnelType.Manage:
                        break;
                    case PersonnelType.Daywork:
                        {
                            expable.And(t => SqlFunc.Subqueryable<Model.Live_PersonnelItems>().Where(s => s.CID == t.CustomerID && s.UserID == puser.UserID).Any());
                        }
                        break;
                    case PersonnelType.DayworkManage:
                        {
                            expable.And(t => SqlFunc.Subqueryable<Model.Live_PersonnelItems>().Where(s => s.CID == t.CustomerID && s.UserID == puser.UserID).Any());
                        }
                        break;
                    default:
                        expable.And(t => t.PerUserID == puser.UserID);
                        break;
                }
            }
            
            //await SetPersonnelOperateAuth(expable, puser);
            var exp = expable.ToExpression();

            var models = await injuryApplyRecordService.List(exp, query, "CreateTime desc");
            return Ok(new { items = models, count = query.Count });
        }
        
        [HttpPost]
        public async Task<IActionResult> RepayAdd([FromForm] int id)
        {

            Guid? cgid = Guid.Parse("00000000-0000-0000-0000-000000000000");
            int cid = 0;
            var expable = Expressionable.Create<Model.Live_BudgetApplyRecord>();
            if (CurrentUser.PlatformType == Model.Enums.PlatformType.LZMini || CurrentUser.PlatformType == Model.Enums.PlatformType.MiniAdmin)
            {
                cgid = CurrentUser.CompanyGID;
                cid = CurrentUser.CompanyID;
            }
            var info = await injuryInfoService.Get(id);
            var repay = await injuryApplyRecordService.View(id,info.ID, "1,2,4", (int)InjuryApplyType.还款单, cid);
            if (repay != null)
            {
                throw new ErrorCodeException(-1, "该员工已创建还款单,审核通过后才可继续创建");
            }
            Model.Live_InjuryApplyRecord model = new Model.Live_InjuryApplyRecord();
            model.CompanyID = cid;
            model.CompanyGID = cgid;
            model.Status = 1;
            model.InjuryID = id;
            model.MoneyType = (int)InjuryApplyType.还款单;
            model.Remark = "申请还款单";
            model.PerUserID = CurrentUser.UserID;
            model.CreateTime = DateTime.Now;
            model.Bank = info.SealCompanyBank;
            model.BankAccount = info.SealCompanyBankNo;
            model.CustomerID = info.CustomerID;
            long rid = await injuryApplyRecordService.Add(model);


            // 首次还款时候，需要统计所有上传发票的金额作为销账金额


            if (info.ReturnBackMoneyTotalAmount == 0)
            {
                Model.PagedModel.UserPagedModel query = new Model.PagedModel.UserPagedModel();
                query.Page = 0;
                var fileExpable = Expressionable.Create<Model.Live_FileUpload>();
                fileExpable.And(t => t.CompanyID == cid && t.TypeKey == 6 && t.FID == id);
                var files = await fileUploadService.List(fileExpable.ToExpression(), query);
                foreach (var f in files)
                {
                    var resObj = JsonConvert.DeserializeObject(f.FileContentJSON) as JObject;
                    if (resObj != null)
                    {
                        decimal money = resObj["money"] != null ? resObj["money"].ToDecimal() : 0;
                        info.ReturnBackMoneyTotalAmount += money;
                    }

                }
                if (info.ReturnBackMoneyTotalAmount > 0)
                {
                    await injuryInfoService.Update(info);
                }
            }




            return Ok(new { rid });
        }

        [HttpGet]
        public async Task<IActionResult> LoanTest()
        {
            var info = await injuryInfoService.Get(625);
            List<object> singers = new List<object>();

            var dic = new Dictionary<string, string>();
            dic.Add("customerName", info.CustomerName ?? "");
            dic.Add("name", info.UserName ?? "");
            dic.Add("agentBank", info.AgentBank ?? "");
            dic.Add("agentBankNo", info.AgentBankNo ?? "");
            dic.Add("agentIdcardNo", info.AgentIdcardNo ?? "");
            dic.Add("amountApply", "8000");
            dic.Add("amountApplyWords", LZ.Common.MoneyTypeChange.MoneyToChinese(8000));
            dic.Add("signatureCompany", info.SealCompanyName ?? "");
            dic.Add("signatureCompanyAddress", info.SealCompanyAddress ?? "");
            dic.Add("signatureCompanyPhone", info.SealCompanyPhone ?? "");
            dic.Add("signatureCompanyBank", info.SealCompanyBank ?? "");
            dic.Add("signatureCompanyBankNo", info.SealCompanyBankNo ?? "");
            dic.Add("workInjuryEmployeeIdcardNo", info.UserCardNo ?? "");
            dic.Add("workInjuryEmployeeName", info.UserName ?? "");
            var newDic = dic.Where(t => t.Value != null && t.Value != "").OrderBy(t => t.Key).ToDictionary();
            var label = await injuryDocumentLabelService.GetByName("阳光保险公司");
            string gid = label.PersonApplyLoanFileID;
            string itemjson = "";
            var signer = new { Name = info.AgentName, IdNo = info.AgentIdcardNo, IsCompany = false, SignerPartyType = SignerPartyEnumType.第二当事人, IsNeedSendSms = false, SmsPhone = "", sealOssFilePath = "" };
            singers.Add(signer);
            var item = new { CustomerGID = info.CustomerGID.ToString(), ContractFileID = gid, Keywords = newDic, Signers = singers };
            itemjson = JsonConvert.SerializeObject(item);

            try
            {
                Live_ContractRecordExtension contractRecord = null;
                var obj = await httpService.PostModelAsync(ServiceNames.CustomerService, "/api/fxqApi/generatePdfContractByFileID", itemjson);
                List<Model.Live_ContractFileConfig> contracts = new List<Live_ContractFileConfig>();
                if (obj.Data == null)
                {
                    throw new ErrorCodeException(-1, obj.Message.ToString());
                }
                var ii = JsonConvert.DeserializeObject(obj.Data.ToString()) as JObject;
                //var ii = JsonConvert.DeserializeObject(obj.Data.ToString()) as JObject;
                contractRecord = JsonConvert.DeserializeObject<Live_ContractRecordExtension>(ii.ToString());
                return Ok(new { contractRecord, msg = "OK", newDic });
            }
            catch(Exception e)
            {
                return Ok(new {  msg = e.ToString(), newDic });
            }

        }

        [HttpPost]
        public async Task<IActionResult> LoanAdd([FromBody] Model.Live_InjuryApplyRecord model)
        {
            var user = CurrentUser;
            int cid = 0;
            Guid? cgid = Guid.Parse("00000000-0000-0000-0000-000000000000");
            var expable = Expressionable.Create<Model.Live_InjuryApplyRecord>();
            if (user.PlatformType == Model.Enums.PlatformType.LZMini || user.PlatformType == Model.Enums.PlatformType.MiniAdmin)
            {
                cid = user.CompanyID;
                cgid = user.CompanyGID;
            }
            expable.And(t => t.CompanyID == cid);
            expable.And(t => t.InjuryID == model.InjuryID);
            expable.And(t => t.MoneyType == (int)InjuryApplyType.借款单);
            expable.And(t => t.Status == 1);
            if (await injuryApplyRecordService.Exist(expable.ToExpression()))
            {
                throw new ErrorCodeException(-1, "已存在借款单未签名");
            }
            if(!model.InjuryID.HasValue)
            {
                throw new ErrorCodeException(-1, "未关联工伤id");
            }
            var info = await injuryInfoService.Get(model.InjuryID.Value);
            bool isPerson = info.SealCompanyName.Trim() == "宁波璟霖供应链管理有限公司" || info.SealCompanyName.Trim() == "宁波莱博友翌供应链管理有限公司" || info.SealCompanyName.Trim() == "宁波莱鑫人力资源有限公司" || info.SealCompanyName.Trim() == "宁波蓝鑫人力资源有限公司" || info.SealCompanyName.Trim() == "宁波新莱博友盟供应链管理有限公司" || info.SealCompanyName.Trim() == "台州莱博友翌供应链管理有限公司";
            var account = await injuryAdvanceAccountService.Get(t => t.PartnerID == info.PartnerID);
            if (account == null)
            {
                throw new ErrorCodeException(-1, "该服务商未创建可垫付账号");
            }
            if (account.AvailableAmount < model.Money)
            {
                throw new ErrorCodeException(-1, "可用余额为:" + account.AvailableAmount);
            }

            //var expable_Record = Expressionable.Create<Model.Live_BusinessInsuranceRecord>();
            //if (!string.IsNullOrEmpty(info.CustomerName))
            //{
            //	expable_Record.And(it => it.CustomerName == info.CustomerName);
            //}
            //if (!string.IsNullOrEmpty(info.UserName))
            //{
            //	expable_Record.And(it => it.Name == info.UserName);
            //}
            //expable_Record.And(it => it.IsMain);
            //// expable_Record.AndIF(info.InjuryTime.HasValue, t => t.StartDate < info.InjuryTime && t.EndDate > info.InjuryTime);
            //var exp = expable.ToExpression();
            //var Insurance = await businessInsuranceRecordService.Get(expable_Record.ToExpression());
            //         if(Insurance==null)
            //         {
            //             throw new ErrorCodeException(-1, "没有配置主保险");
            //         }
            var expable_insurance = Expressionable.Create<Model.Live_BusinessInsuranceRecord>();
            if (!string.IsNullOrEmpty(info.UserCardNo))
            {
                expable_insurance.And(it => it.IdCardNo == info.UserCardNo);
            }
            expable_insurance.And(it => it.IsMain);
            expable_insurance.AndIF(info.InjuryTime.HasValue, t => t.EndDate.HasValue && t.StartDate < info.InjuryTime && t.EndDate.Value.AddDays(1) > info.InjuryTime);
            var exp_insurance = expable_insurance.ToExpression();
            var insurance = await businessInsuranceRecordService.Get(exp_insurance);
            var insuranceOrder = await businessInsuranceOrderService.Get(t => t.InsuranceId == info.InsuranceId);
            string insuranceCompany = "阳光保险公司";
            if (insuranceOrder != null)
            {
                insuranceCompany = insuranceOrder.InsuranceCompany;
                //throw new ErrorCodeException(-1, "保险订单号不存在");
            }
            
            //var guarantee = await businessGuaranteeService.Get(t => t.InsuranceId == info.InsuranceId);
            //string name = guarantee.InsuranceCompany;
			var label = await injuryDocumentLabelService.GetByName(insuranceCompany);
            string gid = !isPerson ? label.CompanyApplyLoanFileID : label.PersonApplyLoanFileID;
            if (string.IsNullOrEmpty(gid))
            {
                throw new ErrorCodeException(-1, "工伤配置中没有设置借款单");
            }

            model.CompanyID = info.CompanyID;
            model.CompanyGID = info.CompanyGID;
            model.Status = 1;
            model.PerUserID = user.UserID;
            model.CreateTime = DateTime.Now;
            model.Type = (int)InjuryLoanType.公司借款;
            model.MoneyType = (int)InjuryApplyType.借款单;
            model.Remark = "申请借款单";
            model.CustomerID = info.CustomerID;
            if (isPerson)
            {
                model.Status = 1;
                model.Type = (int)InjuryLoanType.个人借款;
            }
            var loan = await injuryApplyRecordService.LoanAdd(model);
            List<object> singers = new List<object>();

			var dic = new Dictionary<string, string>();
			dic.Add("customerName", info.CustomerName ?? "");
			dic.Add("name", info.UserName ?? "");
            dic.Add("agentBank", info.AgentBank ?? "");
            dic.Add("agentBankNo", info.AgentBankNo ?? "");
            dic.Add("agentIdcardNo", info.AgentIdcardNo ?? "");
            dic.Add("amountApply", model.Money.ToString());
            dic.Add("amountApplyWords", LZ.Common.MoneyTypeChange.MoneyToChinese(model.Money));
            dic.Add("signatureCompany", info.SealCompanyName ?? "");
            dic.Add("signatureCompanyAddress", info.SealCompanyAddress ?? "");
            dic.Add("signatureCompanyPhone", info.SealCompanyPhone ?? "");
            dic.Add("signatureCompanyBank", info.SealCompanyBank ?? "");
            dic.Add("signatureCompanyBankNo", info.SealCompanyBankNo ?? "");
            dic.Add("workInjuryEmployeeIdcardNo", info.UserCardNo ?? "");
            dic.Add("workInjuryEmployeeName", info.UserName ?? "");
            var newDic =  dic.Where(t=>t.Value!=null && t.Value!="").OrderBy(t => t.Key).ToDictionary();
            string itemjson = "";
            try
            {
                if (isPerson)
                {
                    var signer = new { Name = info.AgentName, IdNo = info.AgentIdcardNo, IsCompany = false, SignerPartyType = SignerPartyEnumType.第二当事人, IsNeedSendSms = false, SmsPhone = "", sealOssFilePath = "" };
                    singers.Add(signer);
                    var item = new { CustomerGID = info.CustomerGID.ToString(), ContractFileID = gid, Keywords = newDic, Signers = singers };
                    itemjson = JsonConvert.SerializeObject(item);
                    if(string.IsNullOrEmpty(info.AgentIdcardNo))
                    {
                        throw new ErrorCodeException(-1, "代理身份证为空");
                    }
                    RealName2EventData eventData = new RealName2EventData(info.AgentName, info.AgentIdcardNo);
                    eventPublisher.Publish(eventData);
                }
                else
                {
                    var signer = new { Name = info.SealCompanyName, IdNo = info.SealCompanyNo, IsCompany = true, SignerPartyType = SignerPartyEnumType.第一当事人, IsNeedSendSms = false, SmsPhone = "", sealOssFilePath = "" };
                    singers.Add(signer);
                    var item = new { CustomerGID = info.CustomerGID.ToString(), ContractFileID = gid, Keywords = newDic, Signers = singers };
                    itemjson = JsonConvert.SerializeObject(item);
                }
            }
            catch(Exception e)
            {
                await injuryApplyRecordService.Delete(loan.ID);
                throw new ErrorCodeException(-1, "代理信息不全"+e.ToString());
            }
            try
            {
                Live_ContractRecordExtension contractRecord = null;
                var obj = await httpService.PostModelAsync(ServiceNames.CustomerService, "/api/fxqApi/generatePdfContractByFileID", itemjson);
                List<Model.Live_ContractFileConfig> contracts = new List<Live_ContractFileConfig>();
                if (obj.Data == null)
                {
                    await injuryApplyRecordService.Delete(loan.ID);
                    throw new ErrorCodeException(-1, obj.Message.ToString());
                }
                var ii = JsonConvert.DeserializeObject(obj.Data.ToString()) as JObject;
                //var ii = JsonConvert.DeserializeObject(obj.Data.ToString()) as JObject;
                contractRecord = JsonConvert.DeserializeObject<Live_ContractRecordExtension>(ii.ToString());
                Model.Live_InjuryElectronicDocuments document = new Live_InjuryElectronicDocuments();
                document.CompanyID = info.CompanyID;
                document.CompanyGID = info.CompanyGID;
                document.InjuryID = info.ID;
                document.Path = contractRecord.ContractOssUrl;
                document.ContractFileConfigID = gid;
                document.LoanID = loan.ID;
                if (contractRecord.Signers != null)
                {
                    document.SignPath = contractRecord.Signers[0].SignOssUrl;
                    document.signerIdno = contractRecord.Signers[0].SignerIdno;
                    document.signKeyword = contractRecord.Signers[0].SignKeyword;
                    document.signerName = contractRecord.Signers[0].SignerName;
                }

                await injuryElectronicDocumentsService.Add(document);

                await injuryInfoService.Update(info);
                var objs = new Dictionary<string, object>();
                objs.Add("ID", loan.ID);
                objs.Add("Path", document.Path);
                loan.Path = document.Path;
                var count = await injuryApplyRecordService.DictionarySet(objs);

                var snedItem = new
                {
                    type = "injury-loanModel",
                    customerName = loan.CustomerName,
                    agentName = loan.AgentName,
                    userName = loan.UserName,
                    partnerName = loan.PartnerName,
                    userCardNo = loan.UserCardNo,
                    money = loan.Money
                };
                var json = JsonConvert.SerializeObject(snedItem);
                sendMQService.SendStatisticsMessage(json);

               

                return Ok(new { items = loan, document });
            }
            catch(Exception e)
            {
                throw new ErrorCodeException(-1, "合同生成报错" + e.ToString());
            }

            
        }

        [HttpPost]
        public async Task<IActionResult> SignContract([FromBody] Model.Live_InjuryElectronicDocuments document)
        {
            var loan = await injuryApplyRecordService.Get(document.LoanID.Value);
            var info = await injuryInfoService.Get(loan.InjuryID.Value);

            bool isPerson = info.SealCompanyName.Trim() == "宁波璟霖供应链管理有限公司" || info.SealCompanyName.Trim() == "宁波莱博友翌供应链管理有限公司" || info.SealCompanyName.Trim() == "宁波莱鑫人力资源有限公司" || info.SealCompanyName.Trim() == "宁波蓝鑫人力资源有限公司" || info.SealCompanyName.Trim() == "宁波新莱博友盟供应链管理有限公司" || info.SealCompanyName.Trim() == "台州莱博友翌供应链管理有限公司"; 
            List<KeywordSigner> keys = new List<KeywordSigner>();
            KeywordSigner key = new KeywordSigner(document.signerName.Trim(), document.signerIdno.Trim(), document.SignPath, document.signKeyword, 120, null);
            keys.Add(key);

            if (!isPerson)
            {
                //如果没有匹配的关键词，则会报错，个人借款不需要公司章
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("id", "62D8CB3A-C717-42B9-B496-7D558C972C50");
                var obj = await httpService.GetAsync(ServiceNames.CustomerService, "/api/ContractSigner/get", dic);
                Live_ContractSigner singer = new Live_ContractSigner();
                if (obj.Data != null)
                {
                    var ii = JsonConvert.DeserializeObject(obj.Data.ToString()) as JObject;
                    //var ii = JsonConvert.DeserializeObject(obj.Data.ToString()) as JObject;
                    singer = JsonConvert.DeserializeObject<Live_ContractSigner>(ii.ToString());
                    KeywordSigner jia = new KeywordSigner(singer.Name, singer.Idno, singer.OssUrl, "甲方签署", 120, null);
                    keys.Add(jia);
                }
            }
            
            var signResp = await fxqService.KeywordPdfContractSign(document.Path, keys.ToArray(), 1, null, null, new SignDate(1, null, null, null));
            if (signResp != null && signResp.Code == 10000)
            {
                var objs = new Dictionary<string, object>();
                objs.Add("ID", loan.ID);
                objs.Add("Path", signResp.Data);
                objs.Add("Status", 2);
                var count = await injuryApplyRecordService.DictionarySet(objs);
            }
            else
            {
                throw new ErrorCodeException(-1, "盖章错误:"+signResp.Code +"-"+ signResp.Msg);
            }
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus([FromForm] int id, [FromForm] int status, [FromForm] string msg)
        {
            if (!await injuryApplyRecordService.ExistID(id))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            await injuryApplyRecordService.UpdateStatus(id, status, msg);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Auditing([FromForm] int id, [FromForm] int status, [FromForm] string msg)
        {
            if (!await injuryApplyRecordService.ExistID(id))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            var model = await injuryApplyRecordService.Get(id);
            var info = await injuryInfoService.Get((int)model.InjuryID);
            model.Status = status;
            var account = await injuryAdvanceAccountService.Get(t => t.PartnerID == info.PartnerID);
           
            //审核通过
            if (model.MoneyType == 1)
            {
                model.PayTime = DateTime.Now;
                info.LoanTotalAmount += model.Money;
                info.IsPerson = (model.Type == (int)InjuryLoanType.个人借款);
                if (account == null)
                {
                    throw new ErrorCodeException(-1, "该服务商未创建可垫付账号");
                }

                if (account.AvailableAmount < model.Money)
                {
                    throw new ErrorCodeException(-1, "可用余额为:" + account.AvailableAmount);
                }
                account.AvailableAmount = account.AvailableAmount - model.Money;
                account.UsedAmount = account.UsedAmount + model.Money;
                model.AdvanceAccountID = account.ID;// 借款成功后借款单中保有工伤垫付id，方便判断还款时候是否要回滚金额，老数据不用回滚
                await injuryAdvanceAccountService.Update(account);
                await injuryInfoService.Update(info);

            }
            else if (model.MoneyType == 2)
            {
                if (account == null)
                {
                    throw new ErrorCodeException(-1, "该服务商未创建可垫付账号");
                }
                if (info.LoanTotalAmount > model.Money)
                {
                    throw new ErrorCodeException(-1, "借款金额大于还款金额");
                }
                info.RepaymentTotalAmount = model.Money;
                var loans = await injuryApplyRecordService.List(t => t.InjuryID == info.ID && t.AdvanceAccountID == account.ID, new Model.PagedModel.UserPagedModel() { Page = 0 });
                var money = loans.Sum(t => t.Money);// 借款成功后借款单中保有工伤垫付id，方便判断还款时候是否要回滚金额，老数据不用回滚
                if (money > 0)
                {
                    account.AvailableAmount = account.AvailableAmount + money;
                    account.UsedAmount = account.UsedAmount - money;
                    await injuryAdvanceAccountService.Update(account);
                }
                await injuryInfoService.Update(info);
            }
            await injuryApplyRecordService.Update(model);
            return Ok();
        }

        [HttpGet]
        public async Task<Model.Live_InjuryApplyRecord> Get(int id)
        {
            return await injuryApplyRecordService.Get(id);
            //return await userService.Get(CurrentUser.UserID);
        }
        [HttpGet]
        public async Task<IActionResult> View(int id)
        {
            var model = await injuryApplyRecordService.View(id,0, "");
            var document = await injuryElectronicDocumentsService.GetByLoan(id);
            return Ok(new { items = model, document });
            //return await userService.Get(CurrentUser.UserID);
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_InjuryApplyRecord model)
        {
            if (!await injuryApplyRecordService.ExistID(model.ID))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            await injuryApplyRecordService.Update(model);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            if (!await injuryApplyRecordService.ExistID(id))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            await injuryApplyRecordService.Delete(id);
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> ExPortExcel([FromQuery] Model.PagedModel.UserPagedModel query)
        {
            var cid = CurrentUser.CompanyID;
            var expable = Expressionable.Create<Model.Live_V_InjuryApplyRecord>();
            
            expable.And(t => t.ProcessStatus != 0 && t.ProcessStatus!=9);
            var list = await injuryApplyRecordService.List(expable.ToExpression(), query);
            var result = await injuryApplyRecordService.ExportExcel(list);
            string base64String = Convert.ToBase64String(result);
            return Ok(new { base64String, fileName = "工伤申请记录.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }

        [HttpGet]
        public async Task<IActionResult> SaveFile([FromForm] int id)
        {
            var model = await fileUploadService.Get(id);
            FileStream fs = new FileStream(model.Path, FileMode.Open, FileAccess.Read); //将图片以文件流的形式进行保存
            BinaryReader br = new BinaryReader(fs);
            byte[] imgBytesIn = br.ReadBytes((int)fs.Length); //将流读入到字节数组中
            return Ok(new { imgBytesIn, fileName = model.FileName, mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }
    }
}
