using LZ.Core.Attributes;
using LZ.Customer.Service.Interface;
using LZ.Customer.Service.Services;
using LZ.Model.Enums.Sale;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using Microsoft.AspNetCore.Mvc;
using LZ.Core.Services;
using LZ.Core.Interface;
using LZ.ApiService.Interface;
using LZ.Model.FxqModel;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using LZ.ApiService.Services;
using LZ.Common;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using SqlSugar;
using static Quartz.Logging.OperationName;
using Senparc.Weixin.WxOpen.Entities;
using LZ.Core.EventData;
using LZ.Core.RabbitMQ;
using LZ.Core.RabbitMQ.EventBus;
using NPOI.SS.Formula.Functions;
using LZ.Model.PagedModel;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using LZ.Model.Enums.DayWork;

namespace LZ.Customer.API.Controllers
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.XFZMini)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    [PlatformLimit(Model.Enums.PlatformType.LZMini)]
    [PlatformLimit(Model.Enums.PlatformType.Api)]
    public class DayWorkUserContractRecordController(IDayWorkUserContractRecordService dayWorkUserContractRecordService, IAuthService authService, IFxqDevService fxqDevService, IContractSignerService contractSignerService, IAliyunOSSService aliyunOSSService, ISettingsService settingsService, IDayWorkUserContractFileService dayWorkUserContractFileService, IJobsDayWorkService jobsDayWorkService, IFxqService fxqService, IContractFileConfigService contractFileConfigService, IEventPublisher eventPublisher,IDayWorkCustomerContractRecordService dayWorkCustomerContractRecordService) : CustomerControllerBase
    {

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] PagedModelBase query,int userEntryID,string jobNo, string entryNo)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Live_V_DayWorkUserContractRecord>();
            if (query.CID.HasValue)
            {
                expable.And(it => it.CustomerID == query.CID);
            }
            if(!string.IsNullOrEmpty(query.UserName))
            {
                expable.And(it => it.UserName.Contains(query.UserName));
            }
            if (!string.IsNullOrEmpty(query.Name))
            {
                expable.And(it => it.CustomerName.Contains(query.Name));
            }
            if(userEntryID>0)
            {
                expable.And(t => t.EntryID == userEntryID);
            }
            if (!string.IsNullOrEmpty(jobNo))
            {
                expable.And(it => it.JobNo.Contains(jobNo));
            }
            if (!string.IsNullOrEmpty(entryNo))
            {
                expable.And(it => it.EntryNo.Contains(entryNo));
            }
            var exp = expable.ToExpression();
            var models = await dayWorkUserContractRecordService.List(exp,query);
            return Ok(new { items = models });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> AddWX([FromForm] int jobID, [FromForm] int fileID, [FromForm] int entryID, [FromForm] string message, [FromForm] string sendTime)
        {
            try
            {
                string ali = "https://oss.cn-labor.cn";
                var job = await jobsDayWorkService.Get(jobID);
                var file = await dayWorkUserContractFileService.Get(fileID);
                Model.Live_DayWorkUserContractRecord model = new Model.Live_DayWorkUserContractRecord();
                model.GID = Guid.NewGuid();
                model.CreateTime = DateTime.Now;
                model.CustomerID = job.CustomerID;
                model.FileID = file.ID;
                model.UserID = file.UserID;
                model.UserName = file.UserName;
                var signUserInfo = await contractSignerService.Get(t => t.Idno == file.CardID);
                if (signUserInfo == null)
                {
                    return Ok(new { code = 1, msg = "员工签章获取失败" });
                }
                model.SignPath = signUserInfo.OssUrl;
                model.SignerIdNo = signUserInfo.Idno;
                model.SignerName = signUserInfo.Name;
                model.EntryID = entryID;
                model.Message = message;
                model.SendTime = sendTime;
                model.Status = (int)DayWorkEntryContractTypeEnum.已盖章;
                var templateContract = await contractFileConfigService.GetNav(t => t.ContractEnumType == ContractEnumType.灵活用工员工入职合同);


                #region 入职后合同和声明同时盖章加保存

                //合同盖章
                string contractBase64 = "";

                if (templateContract.Signers.Count > 0)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        var imageBytes = await client.GetByteArrayAsync(file.TemplateContract);
                        contractBase64 = Convert.ToBase64String(imageBytes);
                    }
                    KeywordSigner userSign = new KeywordSigner(signUserInfo.Name, signUserInfo.Idno, signUserInfo.OssUrl, templateContract.Signers[0].SignKeyword, 120, null);
                    var signCompanyInfo = await contractSignerService.Get(t => t.Idno == "91330212MA2CH79K4J");
                    KeywordSigner companySign = new KeywordSigner(signCompanyInfo.Name, signCompanyInfo.Idno, signCompanyInfo.OssUrl, "乙方（盖章）", 120, null);
                    KeywordSigner[] keys = new KeywordSigner[2];
                    keys[0] = userSign;
                    keys[1] = companySign;
                    var contract = await fxqDevService.CombinePdfSigning(contractBase64, keys);
                    file.ContractTransactionId = contract.Data.TransactionId;
                    file.ContractHash = contract.Data.Hash;
                    using (var ms = new MemoryStream(Convert.FromBase64String(contract.Data.PdfBase64)))
                    {

                        var ressd = await aliyunOSSService.UploadFile(ms, file.EntryNo + "合同.pdf", true, false, "temp");
                        if (ressd.Code != 0)
                        {
                            return Ok(new { code = 1, msg = "上传盖章合同错误" });
                        }
                        file.EmploymentContract = ali + ressd.Data.ToString();
                    }
                }
                else
                {
                    //合同不用盖章
                    file.EmploymentContract = file.TemplateContract;
                }


                // 声明盖章
                var templateStatement = await contractFileConfigService.GetNav(t => t.ContractEnumType == ContractEnumType.灵活用工入职须知);
                string statementBase64 = "";

                if (templateStatement.Signers.Count > 0)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        var imageBytes = await client.GetByteArrayAsync(file.TemplateStatement);
                        statementBase64 = Convert.ToBase64String(imageBytes);
                    }
                    KeywordSigner stateSign = new KeywordSigner(signUserInfo.Name, signUserInfo.Idno, signUserInfo.OssUrl, templateStatement.Signers[0].SignKeyword, 120, null);
                    KeywordSigner[] keys1 = new KeywordSigner[1];
                    keys1[0] = stateSign;
                    var statement = await fxqDevService.CombinePdfSigning(statementBase64, keys1);
                    file.StatementTransactionId = statement.Data.TransactionId;
                    file.StatementHash = statement.Data.Hash;
                    using (var ms1 = new MemoryStream(Convert.FromBase64String(statement.Data.PdfBase64)))
                    {

                        var ressd = await aliyunOSSService.UploadFile(ms1, file.EntryNo + "声明.pdf", true, false, "temp");
                        if (ressd.Code != 0)
                        {
                            return Ok(new { code = 1, msg = "上传盖章声明错误" });
                        }
                        file.EmploymentStatement = ali + ressd.Data.ToString();
                    }
                }
                else
                {
                    //声明不用盖章
                    file.EmploymentStatement = file.TemplateStatement;
                }



                #endregion
                await dayWorkUserContractFileService.Update(file);

                await jobsDayWorkService.ChangeNumber(job.ID, 1);




                var index = await dayWorkUserContractRecordService.Add(model);
                return Ok(new { code = 0, msg = "盖章成功" });
            }
            catch (Exception e)
            {
                return Ok(new { code = 4, msg = e.ToString() });
            }

        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ChangeNumber([FromForm] int jobID, [FromForm] int number)
        {
            await jobsDayWorkService.ChangeNumber(jobID, number);
            return Ok();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<Model.Live_DayWorkUserContractFile> Get(int id)
        {
            var model = await dayWorkUserContractFileService.Get(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }

        [HttpPost]
        [PlatformLimit(Model.Enums.PlatformType.LZMini)]
        [PlatformLimit(Model.Enums.PlatformType.XFZMini)]
        public async Task<IActionResult> AddContractFile([FromForm] int customerID, [FromForm] string contractPath, [FromForm] string notePath, [FromForm] string no, [FromForm] int jid)
        {
            // 暂时没用到！！
            var umodel = await authService.UserInfo<Model.Live_User>();
            Model.Live_DayWorkUserContractFile file = new Model.Live_DayWorkUserContractFile();
            file.UserID = umodel.UserID;
            file.CardID = umodel.CardID;
            file.TemplateStatement = notePath;
            file.JobDayWorkID = jid;
            file.CreateTime = DateTime.Now;
            file.TemplateContract = contractPath;
            file.EntryNo = no;
            file.CustomerID = customerID;
            file.UserName = umodel.FullName;
            string contractBase64 = "";
            using (HttpClient client = new HttpClient())
            {
                var imageBytes = await client.GetByteArrayAsync(contractPath);
                contractBase64 = Convert.ToBase64String(imageBytes);
            }
            var signUserInfo = await contractSignerService.Get(t => t.Idno == file.CardID);
            if (signUserInfo == null)
            {
                throw new ErrorCodeException(-1, "没有生成签章");
            }
            var templateContract = await contractFileConfigService.GetNav(t => t.ContractEnumType == ContractEnumType.灵活用工员工入职合同);
            //var templateStatement = await contractFileConfigService.GetNav(t => t.ContractEnumType == ContractEnumType.灵活用工入职须知);
            if (templateContract.Signers.Count > 0)
            {
                KeywordSigner userSign = new KeywordSigner(signUserInfo.Name, signUserInfo.Idno, signUserInfo.OssUrl, "甲方（盖章）", 120, null);
                var signCompanyInfo = await contractSignerService.Get(t => t.Idno == "91330212MA2CH79K4J");
                KeywordSigner companySign = new KeywordSigner(signCompanyInfo.Name, signCompanyInfo.Idno, signCompanyInfo.OssUrl, "乙方（盖章）", 120, null);
                KeywordSigner[] keys = new KeywordSigner[2];
                keys[0] = userSign;
                keys[1] = companySign;
                var contract = await fxqService.CombinePdfSigning(contractBase64, keys);
                file.ContractTransactionId = contract.Data.TransactionId;
                file.ContractHash = contract.Data.Hash;
                using (var ms = new MemoryStream(Convert.FromBase64String(contract.Data.PdfBase64)))
                {

                    var ressd = await aliyunOSSService.UploadFile(ms, no + ".pdf", true, false, "temp");
                    if (ressd.Code != 0)
                    {
                        throw new ErrorCodeException(-1, "上传盖章合同错误");
                    }
                    file.EmploymentContract = ressd.Data.ToString();
                }
                await dayWorkUserContractFileService.Add(file);
            }





            return Ok(new { model = file });
        }

        [HttpPost]
        [PlatformLimit(Model.Enums.PlatformType.LZMini)]
        [PlatformLimit(Model.Enums.PlatformType.XFZMini)]
        public async Task<IActionResult> DayWorkContractCreate([FromForm] int id)
        {
            var now = DateTime.Now;

            var job = await jobsDayWorkService.GetView(id);

            if (job == null)
            {
                throw new ErrorCodeException(-1, "日结岗位不存在！");
            }
            var user = CurrentUser;
            var umodel = await authService.UserInfo<Model.Live_User>();
            var model = await dayWorkUserContractFileService.Get(t => t.CustomerID == job.CustomerID && t.UserID == umodel.UserID && t.CreateTime.HasValue && t.CreateTime.Value.ToString("yyyy-MM-dd") == now.ToString("yyyy-MM-dd"));
            string ali = "https://oss.cn-labor.cn";
            var templateContract = await contractFileConfigService.Get(t => t.ContractEnumType == ContractEnumType.灵活用工员工入职合同);
            var templateStatement = await contractFileConfigService.Get(t => t.ContractEnumType == ContractEnumType.灵活用工入职须知);
            if (templateContract == null || templateStatement == null)
            {
                throw new ErrorCodeException(-1, "合同模板或者声明模板不存在");
            }
            string ossUrl = templateContract.FilePath;//入职合同
            string ossStatementUrl = templateStatement.FilePath;//入职声明
            string jobNo = "";
            string contractBase64 = "";
            string statementBase64 = "";

            if (model == null)
            {
                string statebase64 = "";
                using (HttpClient client = new HttpClient())
                {
                    //文件地址转成base64
                    var imageBytes = await client.GetByteArrayAsync(ossStatementUrl);
                    statebase64 = Convert.ToBase64String(imageBytes);
                }

                string docxBase64 = "";
                using (HttpClient client = new HttpClient())
                {
                    var imageBytes = await client.GetByteArrayAsync(ossUrl);
                    docxBase64 = Convert.ToBase64String(imageBytes);
                }
                var customerContract = await dayWorkCustomerContractRecordService.Get(t => t.CustomerID == job.CustomerID && t.Status == 2);
                if(customerContract==null)
                {
                    throw new ErrorCodeException(-1, "该岗位未签订合同,请联系客服");
                }
                var today = DateTime.Today;
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("card", umodel.CardID ?? "");
                dic.Add("name", umodel.FullName ?? "");
                dic.Add("syear", today.Year.ToString());
                dic.Add("smonth", today.Month.ToString());
                dic.Add("sday", today.Day.ToString());
                if(customerContract.EndTime.HasValue)
                {
                    dic.Add("eyear", customerContract.EndTime.Value.Year.ToString());
                    dic.Add("emonth", customerContract.EndTime.Value.Month.ToString());
                    dic.Add("eday", customerContract.EndTime.Value.Day.ToString());
                }
                
                dic.Add("jobno", job.JobNo);
                string amount = "";
                if(job.ShiftMoney.HasValue)
                {
                    amount += "白班工价:" + job.ShiftMoney.Value.ToString("#0.######") + "元/小时";
                }
                if(job.NightShiftMoney.HasValue)
                {
                    if (!string.IsNullOrEmpty(amount))
                    {
                        amount += ",";
                    }
                    amount += "夜班工价:" + job.NightShiftMoney.Value.ToString("#0.######") + "元/小时";
                }
                dic.Add("amount", amount);
                var newDic = dic.Where(t => t.Value != null && t.Value != "").OrderBy(t => t.Key).ToDictionary();
                var pathBase = await fxqService.CombineDocxKeywordToPdf(docxBase64, newDic);

                if (pathBase.Code != 200)
                {
                    //return Ok(new { code = 1, msg = "12312", newDic });
                    throw new ErrorCodeException(-1, "生成协议报错");
                }
                contractBase64 = pathBase.Data;
                jobNo = job.JobNo + '-' + Tool.RandomName();


                #region 上传合同模板并且上传盖章合同，保存到Live_DayWorkUserContractFile
                Model.Live_DayWorkUserContractFile file = new Model.Live_DayWorkUserContractFile();
                using (var ms = new MemoryStream(Convert.FromBase64String(pathBase.Data)))
                {
                    //文件流上传到服务器
                    var ressd = await aliyunOSSService.UploadFile(ms, jobNo + ".pdf", true, false, "temp");
                    if (ressd.Code != 0)
                    {
                        throw new ErrorCodeException(-1, "上传盖章合同错误");
                    }
                    file.TemplateContract = ali + ressd.Data.ToString();
                }
                file.UserID = umodel.UserID;
                file.CardID = umodel.CardID;
                file.JobDayWorkID = id;
                file.CreateTime = DateTime.Now;
                file.EntryNo = jobNo;
                file.CustomerID = job.CustomerID;
                file.UserName = umodel.FullName;


                var stateBase = await fxqService.DocxToPdf(statebase64);
                using (var ms1 = new MemoryStream(Convert.FromBase64String(stateBase.Data)))
                {

                    var ressd = await aliyunOSSService.UploadFile(ms1, jobNo + ".pdf", true, false, "temp");
                    if (ressd.Code != 0)
                    {
                        throw new ErrorCodeException(-1, "上传盖章合同错误");
                    }
                    file.TemplateStatement = ali + ressd.Data.ToString();
                }
                statementBase64 = stateBase.Data;
                //var signUserInfo = await contractSignerService.Get(t => t.Idno == "341221200408203783");
                //if (signUserInfo == null)
                //{
                //    throw new ErrorCodeException(-1, "没有生成签章");
                //}
                //KeywordSigner userSign = new KeywordSigner(signUserInfo.Name, signUserInfo.Idno, signUserInfo.OssUrl, "甲方（盖章）", 120, null);
                //var signCompanyInfo = await contractSignerService.Get(t => t.Idno == "341221200408203783");
                //KeywordSigner companySign = new KeywordSigner(signCompanyInfo.Name, signCompanyInfo.Idno, signCompanyInfo.OssUrl, "乙方（盖章）", 120, null);
                //KeywordSigner[] keys = new KeywordSigner[2];
                //keys[0] = userSign;
                //keys[1] = companySign;
                //var contract = await fxqService.CombinePdfSigning(pathBase.Data, keys);
                //file.ContractTransactionId = contract.Data.TransactionId;
                //file.ContractHash = contract.Data.Hash;
                //using (var ms1 = new MemoryStream(Convert.FromBase64String(contract.Data.PdfBase64)))
                //{

                //    var ressd = await aliyunOSSService1.UploadFile(ms1, jobNo + ".pdf", true, false, "temp");
                //    if (ressd.Code != 0)
                //    {
                //        throw new ErrorCodeException(-1, "上传盖章合同错误");
                //    }
                //    file.EmploymentContract = ali + ressd.Data.ToString();
                //}
                var index = await dayWorkUserContractFileService.Add(file);
                file.ID = index;







                #endregion


                return Ok(new { code = 1, msg = "生成当天合同", model = file, contractBase64, statementBase64 });
            }
            else
            {
                using (HttpClient client = new HttpClient())
                {
                    //文件地址转成base64
                    var imageBytes = await client.GetByteArrayAsync(model.TemplateStatement);
                    statementBase64 = Convert.ToBase64String(imageBytes);
                }
                using (HttpClient client = new HttpClient())
                {
                    var imageBytes = await client.GetByteArrayAsync(model.TemplateContract);
                    contractBase64 = Convert.ToBase64String(imageBytes);
                }

                jobNo = model.EntryNo;
                return Ok(new { code = 2, msg = "当天已生成过", model, contractBase64, statementBase64 });
            }

        }

        [HttpPost]
        [PlatformLimit(Model.Enums.PlatformType.LZMini)]
        [PlatformLimit(Model.Enums.PlatformType.XFZMini)]
        public async Task<IActionResult> SmsSend([FromForm] string verify, [FromForm] string phone)
        {
            string content = "宁波莱博网络有限公司,日结工入职验证码:" + verify;
            var result = await fxqService.SmsSend(phone, content);
            if (result.Code != 10000)
            {
                throw new ErrorCodeException(-1, "发送验证码失败");
            }
            return Ok(new { content, time = result.Data.Time });
            
            //try
            //{
                
            //}
            //catch (Exception e)
            //{
            //    return Ok(new { msg = e.ToString(), content, result });
            //}
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<Model.Live_V_DayWorkUserContractRecord> GetView(int? entry,int? status)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Live_V_DayWorkUserContractRecord>();
            if (entry.HasValue)
            {
                expable.And(it => it.EntryID==entry);
            }
            if (status.HasValue)
            {
                expable.And(it => it.Status == status);
            }
            var exp = expable.ToExpression();
            var model = await dayWorkUserContractRecordService.GetView(exp);
            return model;
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            if (!await jobsDayWorkService.Exist(id, CurrentUser.CompanyID))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            await dayWorkUserContractRecordService.Delete(id);
            return Ok();
        }

    }
}
