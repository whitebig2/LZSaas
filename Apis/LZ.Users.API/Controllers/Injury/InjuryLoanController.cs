using LZ.ApiService.Interface;
using LZ.Core.Attributes;
using LZ.Core.Interface;
using LZ.Core.Services;
using LZ.Model.Enums;
using LZ.Model;
using LZ.Model.Exceptions;
using LZ.Users.Service.Interface;
using LZ.Users.Service.Services;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using LZ.Model.ModelExtensions;

namespace LZ.Users.API.Controllers
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.LZMini)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    public class InjuryLoanController (IInjuryLoanService injuryLoanService,IInjuryInfoService injuryInfoService,IHttpService httpService, IInjuryDocumentLabelService injuryDocumentLabelService, IInjuryElectronicDocumentsService injuryElectronicDocumentsService,IBusinessInsuranceRecordService businessInsuranceRecordService) : UsersControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] Model.PagedModel.UserPagedModel query)
        {
            var user = CurrentUser;
            var cid = CurrentUser.CompanyID;
            var expable = Expressionable.Create<Model.Live_V_InjuryLoan>();
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
            if (query.From == "lzmini")
            {
                expable.And(t => t.ProcessStatus != 8 && t.ProcessStatus != 9);
            }
            var exp = expable.ToExpression();
            var models = await injuryLoanService.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Model.Live_InjuryLoan model)
        {
            try
            {
                var user = CurrentUser;
                int cid = 0;
                Guid? cgid = Guid.Parse("00000000-0000-0000-0000-000000000000");
                if (user.PlatformType == Model.Enums.PlatformType.LZMini)
                {
                    cid = user.CompanyID;
                    cgid = user.CompanyGID;
                }

                var info = await injuryInfoService.Get(model.InjuryID.Value);
                bool isPerson = info.SealCompanyName == "宁波莱鑫人力资源有限公司" || info.SealCompanyName == "宁波蓝鑫人力资源有限公司";

				var expable = Expressionable.Create<Model.Live_BusinessInsuranceRecord>();
				if (!string.IsNullOrEmpty(info.CustomerName))
				{
					expable.And(it => it.CustomerName == info.CustomerName);
				}
				if (!string.IsNullOrEmpty(info.UserName))
				{
					expable.And(it => it.Name == info.UserName);
				}
				expable.And(it => it.IsMain);
				expable.AndIF(info.InjuryTime.HasValue, t => t.StartDate < info.InjuryTime && t.EndDate > info.InjuryTime);
				var exp = expable.ToExpression();
				var Insurance = await businessInsuranceRecordService.Get(exp);
				string name = Insurance.InsuranceCompany;
				//string name = "阳光保险公司";
				var label = await injuryDocumentLabelService.GetByName(name);

                string gid = !isPerson ? label.CompanyApplyLoanFileID : label.PersonApplyLoanFileID;

                if (!string.IsNullOrEmpty(gid))
                {
                    throw new ErrorCodeException(-1, "工伤配置中没有设置借款单");
                }
                model.CompanyID = cid;
                model.CompanyGID = cgid;
                model.Status = 2;
                model.UserID = user.UserID;
                model.AddTime = DateTime.Now;
                model.Type = ((int)InjuryLoanType.公司借款).ToString();
                var loadID = await injuryLoanService.Add(model);
                if (isPerson)
                {
                    model.Status = 1;
                    model.Type = ((int)InjuryLoanType.个人借款).ToString();
                    gid = label.PersonApplyLoanFileID;
                }
				var dic = new Dictionary<string, string>();
				dic.Add("customerName", info.CustomerName);
				dic.Add("name", info.UserName);
				Dictionary<string, string> fileDictionary = new Dictionary<string, string>();
                dic.Add("agentBank", info.AgentBank);
                dic.Add("agentBankNo", info.AgentBankNo);
                dic.Add("agentIdcardNo", info.AgentIdcardNo);
                dic.Add("amountApply", model.Money.ToString());
                dic.Add("amountApplyWords", LZ.Common.MoneyTypeChange.MoneyToChinese(model.Money.Value));

                dic.Add("signatureCompany", info.SealCompanyName);
                dic.Add("signatureCompanyAddress", info.SealCompanyAddress);
                dic.Add("signatureCompanyPhone", info.SealCompanyPhone);

                dic.Add("workInjuryEmployeeIdcardNo", info.UserCardNo);
                dic.Add("workInjuryEmployeeName", info.UserName);

                var item = new { CustomerGID = info.CustomerGID.ToString(), ContractFileID = gid, Keywords = dic };
                var itemjson = JsonConvert.SerializeObject(item);
                //paras.Add("request", aajson);
                Live_ContractRecordExtension contractRecord = null;
                var obj = await httpService.PostModelAsync(ServiceNames.CustomerService, "/api/fxqApi/generatePdfContractByFileID", itemjson);
                List<Model.Live_ContractFileConfig> contracts = new List<Live_ContractFileConfig>();
                if (!string.IsNullOrEmpty(obj.Data.ToString()))
                {
                    var ii = JsonConvert.DeserializeObject(obj.Data.ToString()) as JObject;
                    contractRecord = JsonConvert.DeserializeObject<Live_ContractRecordExtension>(ii.ToString());
                }
                Model.Live_InjuryElectronicDocuments document = new Live_InjuryElectronicDocuments();
                document.CompanyID = info.CompanyID;
                document.CompanyGID = info.CompanyGID;
                document.InjuryID = info.ID;
                document.Path = contractRecord.ContractOssUrl;
                document.ContractFileConfigID = gid;
                document.LoanID = loadID;
                await injuryElectronicDocumentsService.Add(document);


                info.LoanTotalAmount += model.Money.HasValue ? (decimal)model.Money : 0;
                await injuryInfoService.Update(info);
                return Ok(new {items=document});
            }
            catch(Exception e)
            {
                Common.StaticLog.Error(e.ToString());
                throw new ErrorCodeException(-1, e.ToString());
            }
           


            
        }
        [HttpPost]
        public async Task<IActionResult> AddAndGet([FromBody] Model.Live_InjuryLoan model)
        {
            var user = CurrentUser;
            int cid = 0;
            Guid? cgid = Guid.Parse("00000000-0000-0000-0000-000000000000");
            var expable = Expressionable.Create<Model.Live_InjuryLoan>();
            if (user.PlatformType == Model.Enums.PlatformType.LZMini)
            {
                cid = user.CompanyID;
                cgid = user.CompanyGID;
            }
            expable.And(t => t.CompanyID == cid);
            expable.And(t => t.InjuryID == model.InjuryID);
            if (await injuryLoanService.Exist(expable.ToExpression()))
            {
                throw new ErrorCodeException(-1, "该标签已存在!");
            }
            model.CompanyID = cid;
            model.CompanyGID = cgid;
            model.Status = 2;
            model.UserID = user.UserID;
            model.AddTime = DateTime.Now;
            model.Type = ((int)InjuryLoanType.公司借款).ToString();
            var loan = await injuryLoanService.AddAndGet(model);
            if (model.InjuryID.HasValue)
            {
                var info = await injuryInfoService.Get((int)model.InjuryID);
                if (info.SealCompanyName == "宁波莱鑫人力资源有限公司" || info.SealCompanyName == "宁波蓝鑫人力资源有限公司")
                {
                    model.Status = 1;
                    model.Type = ((int)InjuryLoanType.个人借款).ToString();
                }
                //info.LoanTotalAmount += model.Money.HasValue ? (decimal)model.Money : 0;
                //await injuryInfoService.Update(info);
            }

            



            return Ok(new { items = loan });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus([FromForm] int id, [FromForm] int status, [FromForm] string msg)
        {
            if (!await injuryLoanService.ExistID(id))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            await injuryLoanService.UpdateStatus(id, status, msg);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Auditing([FromForm] int id, [FromForm] int status, [FromForm] string msg)
        {
            if (!await injuryLoanService.ExistID(id))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            var loan = await injuryLoanService.View(id);
            //审核通过
            if(status==2)
            {
                //生成借款单
            }
            await injuryLoanService.UpdateStatus(id, status, msg);
            return Ok();
        }

        [HttpGet]
        public async Task<Model.Live_InjuryLoan> Get(int id)
        {
            return await injuryLoanService.Get(id);
            //return await userService.Get(CurrentUser.UserID);
        }
        [HttpGet]
        public async Task<Model.Live_V_InjuryLoan> View(int id)
        {
            return await injuryLoanService.View(id);
            //return await userService.Get(CurrentUser.UserID);
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_InjuryLoan model)
        {
            if (!await injuryLoanService.ExistID(model.ID))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            await injuryLoanService.Update(model);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            if (!await injuryLoanService.ExistID(id))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            await injuryLoanService.Delete(id);
            return Ok();
        }


        
    }
}
