
using Aliyun.OSS.Util;
using LZ.ApiService.Interface;
using LZ.ApiService.Services;
using LZ.Common;
using LZ.Core.CommonHandle;
using LZ.Core.EventData;
using LZ.Core.EventData.Contract;
using LZ.Core.EventData.FxqEventData;
using LZ.Core.Interface;
using LZ.Core.RabbitMQ;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Core.Services;
using LZ.Customer.Service.EventHandler.FxqEventHandler;
using LZ.Customer.Service.Interface;
using LZ.Customer.Service.Services;
using LZ.Model;
using LZ.Model.Dtos;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.FxqModel;
using LZ.Model.IExcelTemplate;
using LZ.Model.ModelExtensions;
using LZ.Model.Options;
using LZ.Model.PagedModel;
using LZ.Model.Response;
using MathNet.Numerics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NPOI.POIFS.FileSystem;
using NPOI.SS.Formula.Functions;
using Quartz.Util;
using Senparc.CO2NET.Extensions;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
using SqlSugar;
using System.Net.Http;

namespace LZ.Customer.API.Controllers
{
    [AllowAnonymous]
    public class ContractRecordController(
        IEventPublisher eventPublisher, 
        ICustomerService customerService,
        IContractRecordService service,
        IAliyunOSSService aliyunOSSService,
        IFxqService fxqService,
        ICustomerOtherService otherService,
        ISettingsService settingsService,
        IAuthService authService,
        IContractSignerService  signerService,
        IContractFileConfigService  fileConfigService,
        IEnterpriseService  enterpriseService,
        IFxqDevService  fxqDevService,
        ICustomerContractConfigService customerContractConfigService,
        ICaAuthIdentityService caAuthIdentityService
        ) : CustomerControllerBase
    {

        public record Student(string Zhiwu, string Phone);
        public record SigningAndCooperationUnitQuery(bool? IsSigningUnit, bool? IsCooperationUnit);

        [HttpGet]
        public async Task<IActionResult> PageList(string name, string customerName, string idCardNo, SignStatusEnumType? status, ContractEnumType? contractType,bool? isTimeout, [FromQuery] PagedModelBase query)
        {

            var dt = new DateTime(2025, 5, 1);
            var user = CurrentUser;


            var expable = Expressionable.Create<Live_V_ContractRecord>();

            //expable.AndIF(CurrentUser.PlatformType != PlatformType.Admin&& CurrentUser.PlatformType != PlatformType.Api, f => f.CompanyID == CurrentUser.CompanyID);
            //expable.AndIF(CurrentUser.PlatformType != PlatformType.Admin && CurrentUser.PlatformType != PlatformType.Api, f => f.CompanyID == CurrentUser.CompanyID);
            if (user.PlatformType == Model.Enums.PlatformType.MiniAdmin)
            {
                expable.And(it => it.ContractType == ContractEnumType.入职合同 || it.ContractType == ContractEnumType.离职合同 || it.ContractType == ContractEnumType.电子工资条查看即确认通知);
                var info = await authService.UserInfo<Model.SYS_CompanyUser>();
                if (info != null && info.UserType == 2)
                {
                    expable.And(t => t.PartnerID == info.PartnerID);
                }
            }
            else if (user.PlatformType == Model.Enums.PlatformType.LZMini)
            {

                dt = DateTime.Today.AddDays(-3);

                var umodel = await authService.UserInfo<Model.Live_Personnel>();
                expable.And(UserEntryHandle.SetPersonnelAuth<Model.Live_V_ContractRecord>(umodel));

                expable.And(it => (it.ContractType == ContractEnumType.入职合同 && it.UserEntryStatus == 2) || (it.UserEntryStatus == 4 && it.ContractType == ContractEnumType.离职合同 && it.LeaveDate != null && it.LeaveDate.Value >= dt));

                if (contractType.HasValue && contractType.Value == ContractEnumType.入职合同)
                {
                    if (isTimeout.HasValue && isTimeout.Value)
                    {
                        expable.And(f => f.EntryDate.Value <= DateTime.Today.AddDays(-30));
                    }
                    else
                    {
                        expable.And(f => f.EntryDate.Value > DateTime.Today.AddDays(-30));
                    }
                }


            }
            else if (user.PlatformType != Model.Enums.PlatformType.Admin)
            {
                expable.And(it => it.ContractType == ContractEnumType.入职合同 || it.ContractType == ContractEnumType.离职合同);
            }
            else {
                expable.And(it => it.ContractType == ContractEnumType.入职合同 || it.ContractType == ContractEnumType.离职合同);
            }
            if (user.CompanyType == CompanyType.Party.ToString().ToLower())//人力公司授权甲方公司
            {
                expable.And(it => it.CompanyID == user.CompanyID || SqlFunc.Subqueryable<Model.Live_CompanyAuthorize>().Where(s => s.CustomerID == it.CustomerID && s.ToCompanyID == user.CompanyID && s.Enabled).Any());
            }
            else if (user.PlatformType != Model.Enums.PlatformType.Admin)
            {
                expable.And(it => it.CompanyID == user.CompanyID);
            }
            expable.AndIF(!string.IsNullOrWhiteSpace(name), it => it.EmpName.Contains(name));
            expable.AndIF(!string.IsNullOrWhiteSpace(customerName), it => it.CustomerName.Contains(customerName));
            expable.AndIF(!string.IsNullOrWhiteSpace(idCardNo), it => it.EmpIdCardNo.Contains(idCardNo));
            expable.AndIF(status.HasValue, it => it.Status == (int)status);
            expable.AndIF(contractType.HasValue, it => it.ContractType == contractType.Value);
       
            var exp = expable.ToExpression();
            var models = await service.PageList(exp, query);
            return Ok(new { items = models, count = query.Count });
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> DownloadFileBitch([FromForm] int year, [FromForm] string name)
        {
            //按年份批量导出劳动合同和声明
            var expable = Expressionable.Create<Live_V_ContractRecord>();
            expable.And(t => t.ContractType == ContractEnumType.入职合同 && t.CustomerName.Contains(name) && !string.IsNullOrEmpty(t.SignedFilePath));
            expable.And(t => SqlFunc.Subqueryable<Model.Live_UserEntry>().Where(s => s.ID == t.UserEntryID && s.Status == 2 && s.EntryTime.HasValue && s.EntryTime.Value.Year == year).Any());
            var list = await service.ListView(expable.ToExpression());
            var f = Directory.GetCurrentDirectory();
            string fPath = "/uploadfiles/pdf/" + year;
            string FilePath = f + fPath;
            //if (!Directory.Exists(FilePath))
            //{
            //    Directory.CreateDirectory(FilePath);
            //}
            
            foreach(var x in list)
            {
                var userFile = FilePath + '/' + x.UserName;
                if (!Directory.Exists(userFile))
                {
                    Directory.CreateDirectory(userFile);
                }
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");

                using var response = await httpClient.GetAsync(x.SignedFilePath, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                using var inputStream = await response.Content.ReadAsStreamAsync();
                using var outputStream = new FileStream(userFile + string.Format("/劳动合同_{0}.pdf",x.ID), FileMode.Create, FileAccess.Write, FileShare.None, 8192, FileOptions.Asynchronous);
                await inputStream.CopyToAsync(outputStream);

                using var httpClient1 = new HttpClient();
                httpClient1.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");

                using var response1 = await httpClient.GetAsync(x.SmSignedFilePath, HttpCompletionOption.ResponseHeadersRead);
                response1.EnsureSuccessStatusCode();

                using var inputStream1 = await response1.Content.ReadAsStreamAsync();
                using var outputStream1 = new FileStream(userFile + string.Format("/员工声明 _{0}.pdf", x.ID), FileMode.Create, FileAccess.Write, FileShare.None, 8192, FileOptions.Asynchronous);
                await inputStream1.CopyToAsync(outputStream1);

            }

            //if (list.Count > 0)
            //    {
            //    try
            //    {
                   
            //    }
            //    catch (Exception e)
            //    {
            //        return Ok(new { msg = e.ToString(), path = FilePath, url = list[0].SignedFilePath });
            //    }
            //}
           
            
            
            
            return Ok(new {items = list});

        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ContractDownloadFileBitch([FromForm] string name)
        {
            //导出第三方人力公司的全部员工的入职合同、离职合同，导出的时候能不能入职合同一个文件夹，离职合同一个文件夹。
            var expable = Expressionable.Create<Live_V_ContractRecord>();
            expable.And(t => (t.ContractType == ContractEnumType.入职合同 || t.ContractType == ContractEnumType.离职合同) && t.CustomerName.Contains(name) && !string.IsNullOrEmpty(t.SignedFilePath));
            expable.And(t => SqlFunc.Subqueryable<Model.Live_UserEntry>().Where(s => s.ID == t.UserEntryID && (s.Status == 2 || s.Status == 4)).Any());
            var list = await service.ListView(expable.ToExpression());
            var f = Directory.GetCurrentDirectory();
            string fPath = "/uploadfiles/pdf/入离职合同";
            string FilePath = f + fPath;

            foreach (var x in list)
            {
                string fileName = x.ContractType == null? "未知" : x.ContractType.Value.ToString();
                var userFile = FilePath + '/' + x.UserName;
                if (!Directory.Exists(userFile))
                {
                    Directory.CreateDirectory(userFile);
                }
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");

                using var response = await httpClient.GetAsync(x.SignedFilePath, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                using var inputStream = await response.Content.ReadAsStreamAsync();
                using var outputStream = new FileStream(userFile + string.Format("/{2}_{1}_{0}.pdf", x.ID,x.CreateTime.Value.ToString("yyyyMMdd"), x.UserName + fileName), FileMode.Create, FileAccess.Write, FileShare.None, 8192, FileOptions.Asynchronous);
                await inputStream.CopyToAsync(outputStream);

                //using var httpClient1 = new HttpClient();
                //httpClient1.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");

                //using var response1 = await httpClient.GetAsync(x.SmSignedFilePath, HttpCompletionOption.ResponseHeadersRead);
                //response1.EnsureSuccessStatusCode();

                //using var inputStream1 = await response1.Content.ReadAsStreamAsync();
                //using var outputStream1 = new FileStream(userFile + string.Format("/员工声明 _{0}.pdf", x.ID), FileMode.Create, FileAccess.Write, FileShare.None, 8192, FileOptions.Asynchronous);
                //await inputStream1.CopyToAsync(outputStream1);

            }

            //if (list.Count > 0)
            //    {
            //    try
            //    {

            //    }
            //    catch (Exception e)
            //    {
            //        return Ok(new { msg = e.ToString(), path = FilePath, url = list[0].SignedFilePath });
            //    }
            //}




            return Ok(new { items = list });

        }





        [HttpGet]
        public async Task<IActionResult> ExportQueryList(string name, string customerName, string idCardNo, SignStatusEnumType? status, ContractEnumType? contractType)
        {
            var dt = new DateTime(2025, 5, 1);
            var user = CurrentUser;


            var expable = Expressionable.Create<Live_V_ContractRecord>();

            //expable.AndIF(CurrentUser.PlatformType != PlatformType.Admin&& CurrentUser.PlatformType != PlatformType.Api, f => f.CompanyID == CurrentUser.CompanyID);
            //expable.AndIF(CurrentUser.PlatformType != PlatformType.Admin && CurrentUser.PlatformType != PlatformType.Api, f => f.CompanyID == CurrentUser.CompanyID);
            if (CurrentUser.PlatformType == Model.Enums.PlatformType.MiniAdmin)
            {
                expable.And(f => f.ContractType == ContractEnumType.入职合同 || f.ContractType == ContractEnumType.离职合同 || f.ContractType == ContractEnumType.电子工资条查看即确认通知);
                var info = await authService.UserInfo<Model.SYS_CompanyUser>();
                if (info != null && info.UserType == 2)
                {
                    expable.And(t => t.PartnerID == info.PartnerID);
                }
            }
            else if (user.PlatformType == Model.Enums.PlatformType.LZMini)
            {

                dt = DateTime.Today.AddDays(-3);

                var umodel = await authService.UserInfo<Model.Live_Personnel>();
                if (umodel.UserType == (int)PersonnelType.Headed)
                {
                    expable.And(it => it.PerUserID == user.UserID || it.PartnerID == umodel.PartnerID);
                }
                else if (umodel.UserType == (int)PersonnelType.Factory)
                {
                    expable.And(it => it.FactoryUserID == user.UserID);
                }
                else
                {
                    expable.And(it => it.PerUserID == user.UserID);
                }

                expable.And(f => (f.ContractType == ContractEnumType.入职合同 && f.UserEntryStatus == 2) || (f.UserEntryStatus == 4 && f.ContractType == ContractEnumType.离职合同 && f.LeaveDate != null && f.LeaveDate.Value >= dt));

            }
            else if (user.PlatformType != Model.Enums.PlatformType.Admin)
            {
                expable.And(f => f.ContractType == ContractEnumType.入职合同 || f.ContractType == ContractEnumType.离职合同);
                expable.And(it => it.CompanyID == user.CompanyID);
            }
            else
            {
                expable.And(f => f.ContractType == ContractEnumType.入职合同 || f.ContractType == ContractEnumType.离职合同);
            }

            expable.AndIF(!string.IsNullOrWhiteSpace(name), f => f.EmpName.Contains(name));
            expable.AndIF(!string.IsNullOrWhiteSpace(customerName), f => f.CustomerName.Contains(customerName));
            expable.AndIF(!string.IsNullOrWhiteSpace(idCardNo), f => f.EmpIdCardNo.Contains(idCardNo));
            expable.AndIF(status.HasValue, f => f.Status == (int)status);
            expable.AndIF(contractType.HasValue, f => f.ContractType == contractType.Value);
            var exp = expable.ToExpression();
            var dataList = await service.ListView(expable.ToExpression());
          
            var exportDataList = dataList.Select(f => new ContractRecordExcelTemplate()
            {
                EmpName = f.EmpName,
                IdCardNo = f.EmpIdCardNo,
                AutoLeaveFilePath = f.AutoLeaveFilePath,
                ContractStartToEndDate = $"{f.ContractStartDate?.ToString("yyyy-MM-dd")}至{f.ContractEndDate?.ToString("yyyy-MM-dd")}",
                ContractTypeName = f.ContractType?.ToString(),
                CreateTime = f.CreateTime?.ToString("yyyy-MM-dd HH:mm:ss"),
                CustomerName = f.CustomerName,
                ReadySigningDate = f.ReadySigningDate?.ToString("yyyy-MM-dd HH:mm:ss"),
                SignedTime = f.SignedTime?.ToString("yyyy-MM-dd HH:mm:ss"),
                SigningUnitName = f.SigningUnitName,
                SmsCode = f.SmsCode,
                SmTemplateFilePath = f.SmTemplateFilePath,
                StatusName = ((SignStatusEnumType)f.Status).ToString(),
                TemplateFilePath = f.TemplateFilePath,
                 PdfFilePath = f.PdfFilePath,
                  SignedFilePath = f.SignedFilePath,

            }).ToList();
            var fileBytes = NpoiExcelHelper.WriteExcel(exportDataList, "合同记录");
            string base64String = Convert.ToBase64String(fileBytes);
            return Ok(new { base64String, fileName = "合同记录.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }




        [HttpGet]
        public async Task<IActionResult> List([FromQuery] PagedModelBase query)
        {
            var expable = Expressionable.Create<Model.Live_ContractRecord>();
            var exp = expable.ToExpression();
            var models = await service.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            var uploadRes = await aliyunOSSService.UploadFile(file);
            uploadRes.Code = 200;
            return Ok(uploadRes);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Live_ContractRecord model)
        {
            model.CreateTime = DateTime.Now;
            model.ID = Guid.NewGuid();
            await service.Add(model);
            return Ok();
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<Live_ContractRecordExtension> Get(Guid id)
        {
            var model = await service.GetNav(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }

        [HttpGet]
        public async Task<Live_ContractRecord> GetByName(string name, ContractEnumType? contractType,string cardId="", string phone = "")
        {
            if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(cardId) && string.IsNullOrWhiteSpace(phone))
            {
                throw new ErrorCodeException(-1, "查询参数信息缺失");
            }

            var exp= Expressionable.Create<Live_ContractRecord>();
            exp.And(f => f.EmpName == name);
            exp.AndIF(contractType.HasValue, f => f.ContractType == contractType.Value);
            exp.AndIF(!string.IsNullOrEmpty(cardId), f => f.UserIdCardNo== cardId);
            exp.AndIF(!string.IsNullOrEmpty(phone), f => f.UserPhone == phone);
            var model = await service.Get(exp.ToExpression());
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_ContractRecord model)
        {
            if (!await service.Exist(model.ID))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            await service.Update(model);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] Guid id)
        {
            if (!await service.Exist(id))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            await service.DeleteNav(id);
            return Ok();
        }
        [AllowAnonymous]
        [HttpPut]
        public async Task<IActionResult> ResetStatus(Guid id)
        {
            if (!await service.Exist(id))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            var model = await service.Get(id);
            model.Status = Model.Enums.SignStatusEnumType.待生成;
            model.Remark = string.Empty;
            model.ContractOssUrl = string.Empty;
            model.SignedTime = null;
            model.SignedOssUrl = string.Empty;
            model.SendSmsTime = null;
            model.SmSignedFilePath = string.Empty;
            model.SmPdfFilePath = string.Empty;
            model.SmWordFilePath = string.Empty;

            model.SignedFilePath = string.Empty;
            model.PdfFilePath = string.Empty;
            model.WordFilePath = string.Empty;

            await service.Update(model);
            return Ok();
        }


        //[HttpPut]
        //public async Task<IActionResult> SendSms([FromForm] Guid id)
        //{

        //    var model = await service.Get(id);
        //    if (model==null)
        //    {
        //        throw new ErrorCodeException(-1, "你要读取的数据不存在");
        //    }

        //    if (model.ContractType== ContractEnumType.入职声明)
        //    {
        //        throw new ErrorCodeException(-1, "入职声明记录无法发送短信");
        //    }

        //    List<SignStatusEnumType> statusEnum = new List<SignStatusEnumType>() {
        //         SignStatusEnumType.待发送短信,
        //         SignStatusEnumType.短信发送失败,
        //          SignStatusEnumType.待签署,
        //          SignStatusEnumType.签署失败,
        //        };

        //    if (!statusEnum.Contains(model.Status.Value))
        //    {
        //        throw new ErrorCodeException(-1, "此状态无法发送短信");
        //    }
        //    UserEntryContractSendSmsEventData sendSmsEventData = new UserEntryContractSendSmsEventData(model.ID);
        //    eventPublisher.Publish(sendSmsEventData);
        //    return Ok();
        //}




        


        [HttpGet]
        public async Task<IActionResult> GetEntryContractRecordByUserEntryID(int userEntryID)
        {
            List<ContractEnumType> contractTypes = new List<ContractEnumType>() { ContractEnumType.入职声明, ContractEnumType.入职合同, ContractEnumType.离职合同 };

            List<Live_ContractRecord> records = new List<Live_ContractRecord>();
            List<GetSigningRecordByUserIDResponse> responses = new List<GetSigningRecordByUserIDResponse>();
            var exp = Expressionable.Create<Live_ContractRecord>();
            //exp.And(f =>
            //f.Status == SignStatusEnumType.待签署
            //|| f.Status == SignStatusEnumType.签署失败
            //|| f.Status == SignStatusEnumType.待存储合同
            //|| f.Status == SignStatusEnumType.存储合同失败
            //|| f.Status == SignStatusEnumType.待存证
            //|| f.Status == SignStatusEnumType.存证完成
            //|| f.Status == SignStatusEnumType.存证失败);
            exp.And(f => userEntryID == f.UserEntryID);
            exp.And(f => contractTypes.Contains(f.ContractType.Value));
            records = await service.List(exp.ToExpression());
            return Ok(records);
        }

        public record SigningRecord(string FilePath,string Title);
        public record GetSigningRecordByUserIDResponse(Guid Id,  string CustomerName, string StatusName, List<SigningRecord> records,string  StartDate,string EndDate);
        /// <summary>
        ///  获取员工入职离职合同
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="signStatus"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<RequestResultModel> GetEntryContractRecordByUserID(int userID)
        {
            RequestResultModel result = new RequestResultModel(200, "Success", true);

            var setting =await settingsService.GetMasterSettings();
            List<ContractEnumType> contractTypes = new List<ContractEnumType>() { ContractEnumType.电子工资条查看即确认通知, ContractEnumType.入职合同, ContractEnumType.离职合同 };
            var userEntryResp = await otherService.GetUserEntryListByUserID(userID);
            var records = await service.List(f => f.UserID == userID&&f.Status>= SignStatusEnumType.待签署);
            List<GetSigningRecordByUserIDResponse> contractData = new List<GetSigningRecordByUserIDResponse>();
            if (records != null && records.Any())
            {
                records = records.OrderBy(f => f.CreateTime).ToList();
                foreach (var item in records)
                {
                    var caIdentity = await caAuthIdentityService.Get(f => f.CardID == item.EmpIdCardNo && f.Status == CaAuthIdentityStatusEnumType.认证成功);
                    //if (caIdentity == null)
                    //{
                    //    //throw new ErrorCodeException(-1, "请到小程序【我的】-【实名认证】页面完成认证再继续操作！");
                    //    return Ok(new { IsAuth = false, Message = "请到小程序【我的】-【实名认证】页面完成认证再继续操作！" });



                    //}

                    if (caIdentity == null)
                    {
                        //throw new ErrorCodeException(200, "请到小程序【我的】-【实名认证】页面完成认证再继续操作！");
                        //return Ok(new { IsAuth=false, Message = "请到小程序【我的】-【实名认证】页面完成认证再继续操作！" });
                        result.Data = false;
                        result.Message = "请到小程序【我的】-【实名认证】页面完成认证再继续操作！";

                        return result;
                    }
                    if (item.Status < SignStatusEnumType.待存储合同)
                    {
                        List<SigningRecord> signingRecords = new List<SigningRecord>();
                        if (item.ContractType == ContractEnumType.入职合同)
                        {

                            string title = "入职合同";
                            if (item.CustomerName == "舜宇集团有限公司")
                            {
                                title = "入职须知";
                            }
                            signingRecords.Add(new SigningRecord(item.SmPdfFilePath.Replace("https://lz-uplod.oss-cn-shanghai.aliyuncs.com", setting.AliBucketDomain), "入职声明"));
                            signingRecords.Add(new SigningRecord(item.PdfFilePath.Replace("https://lz-uplod.oss-cn-shanghai.aliyuncs.com", setting.AliBucketDomain), title));
                        }
                        else if (item.ContractType == ContractEnumType.离职合同)
                        {
                            signingRecords.Add(new SigningRecord(item.PdfFilePath.Replace("https://lz-uplod.oss-cn-shanghai.aliyuncs.com", setting.AliBucketDomain), "离职合同"));
                        }
                        else if (item.ContractType == ContractEnumType.电子工资条查看即确认通知)
                        {
                            signingRecords.Add(new SigningRecord(item.PdfFilePath.Replace("https://lz-uplod.oss-cn-shanghai.aliyuncs.com", setting.AliBucketDomain), "电子工资条查看即确认通知"));
                        }
                        contractData.Add(new GetSigningRecordByUserIDResponse(item.ID, item.CustomerName, "待签署", signingRecords, item.ContractStartDate.Value.ToString("yyyy-MM-dd"), item.ContractEndDate.Value.ToString("yyyy-MM-dd")));
                    }
                    else
                    {
                        List<SigningRecord> signingRecords = new List<SigningRecord>();
                        if (item.ContractType == ContractEnumType.入职合同)
                        {

                            string title = "入职合同";
                            if (item.CustomerName == "舜宇集团有限公司")
                            {
                                title = "入职须知";
                            }
                            signingRecords.Add(new SigningRecord(item.SmSignedFilePath.Replace("https://lz-uplod.oss-cn-shanghai.aliyuncs.com", setting.AliBucketDomain), "入职声明"));

                            signingRecords.Add(new SigningRecord(item.SignedFilePath.Replace("https://lz-uplod.oss-cn-shanghai.aliyuncs.com", setting.AliBucketDomain), title));

                        }
                        else if (item.ContractType == ContractEnumType.离职合同)
                        {
                            signingRecords.Add(new SigningRecord(item.SignedFilePath.Replace("https://lz-uplod.oss-cn-shanghai.aliyuncs.com", setting.AliBucketDomain), "离职合同"));

                        }
                        else if (item.ContractType == ContractEnumType.电子工资条查看即确认通知)
                        {
                            signingRecords.Add(new SigningRecord(item.SignedFilePath.Replace("https://lz-uplod.oss-cn-shanghai.aliyuncs.com", setting.AliBucketDomain), "电子工资条查看即确认通知"));
                        }

                        contractData.Add(new GetSigningRecordByUserIDResponse(item.ID, item.CustomerName, "已签署", signingRecords, item.ContractStartDate.Value.ToString("yyyy-MM-dd"), item.ContractEndDate.Value.ToString("yyyy-MM-dd")));
                    }
                }
            }

            result.Items = contractData;
            return result;
            //return Ok(contractData);
        }


        public record SigningRequest(Guid Id,string Name,string SmsCode);
        /// <summary>
        ///  入离职合同签署
        /// </summary>
        /// <param name="batchNo"></param>
        /// <param name="name"></param>
        /// <param name="smsCode"></param>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> EntryContractRecordSigning([FromForm]Guid id, [FromForm]string name, [FromForm] string smsCode)
        {
            

            if (id==Guid.Empty)
            {
                throw new ErrorCodeException(-1, "合同文件标识缺失");
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ErrorCodeException(-1, "姓名不能为空");
            }
            if (string.IsNullOrWhiteSpace(smsCode))
            {
                throw new ErrorCodeException(-1, "验证码不能为空");
            }

            List<ContractEnumType> contractTypes = new List<ContractEnumType>() { ContractEnumType.入职声明, ContractEnumType.入职合同, ContractEnumType.离职合同 };
            var record = await service.Get(f => f.ID == id);
            if (record == null)
            {
                throw new ErrorCodeException(-1, "合同文件不存在");
            }
            if (record.EmpName !=name.Trim())
            {
                throw new ErrorCodeException(-1, "姓名信息与合同签署人姓名不匹配");
            }
            if (record.SmsCode != smsCode.Trim())
            {
                throw new ErrorCodeException(-1, "验证码与合同验证码不匹配");
            }


            //var caIdentity = await caAuthIdentityService.Get(f => f.CardID == record.EmpIdCardNo && f.Status == CaAuthIdentityStatusEnumType.认证成功);
            //if (caIdentity == null)
            //{
            //    //throw new ErrorCodeException(-1, "请到小程序【我的】-【实名认证】页面完成认证再继续操作！");
            //    return Ok(new { IsAuth = false, Message = "请到小程序【我的】-【实名认证】页面完成认证再继续操作！" });
            //}


            string empOssUrl = string.Empty;

        
            var signer = await signerService.Get(f => f.Name == record.EmpName && f.IsEnabled);
            if (signer!=null)
            {
                empOssUrl = signer.OssUrl;
            }

            if (!string.IsNullOrWhiteSpace(empOssUrl))
            {
                var empBase64 = await fxqService.DownloadFile(signer.OssUrl);
                if (string.IsNullOrWhiteSpace(empBase64))
                {
                    empOssUrl = string.Empty;
                }
            }

         
            if (string.IsNullOrWhiteSpace(empOssUrl))
            {
                var resp = await fxqService.ElectronicSeal(record.EmpName, 0);
                if (resp != null && resp.Code == 10000)
                {
                    var setting = await settingsService.GetMasterSettings();
                    var fileBase64 = Convert.FromBase64String(resp.Data.Replace("data:image/png;base64,", ""));
                    using (var ms = new MemoryStream(fileBase64))
                    {
                        var ossResp = await aliyunOSSService.UploadFile(ms, $"{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.png", true, false);
                        if (ossResp.Code == 0)
                        {
                            empOssUrl = setting.AliBucketDomain + ossResp.Data;
                        }
                        else
                        {
                            throw new ErrorCodeException(-1, "电子签名获取成功，上传阿里云Oss 失败！");
                        }
                    }
                }
                else
                {
                    throw new ErrorCodeException(-1, $"电子签名获取失败，异常信息：{resp.Msg}");
                }
            }

            if (string.IsNullOrWhiteSpace(empOssUrl))
            {
                throw new ErrorCodeException(-1, $"电子签名生成异常");
            }


            if (signer != null&&signer.OssUrl != empOssUrl)
            {

                signer.OssUrl = empOssUrl;
               await signerService.Update(signer);

            }
            record.EmpOssUrl = empOssUrl;

            if (string.IsNullOrWhiteSpace(record.SigningUnitOssUrl))
            {
                var signingUnitsigner = await signerService.Get(f => f.Name == record.SigningUnitName && f.IsEnabled);

                if (signingUnitsigner == null)
                {
                    throw new ErrorCodeException(-1, "企业未认证，请联系管理员");
                }
                else {
                    record.SigningUnitOssUrl = signingUnitsigner.OssUrl;
                }
            }




            if (record.ContractType == ContractEnumType.入职合同)
            {
                List<KeywordSigner> keywordSigners = new List<KeywordSigner>();
                //签署入职声明
                if (!string.IsNullOrWhiteSpace(record.EmpSignKeyword) && !string.IsNullOrWhiteSpace(record.SmPdfFilePath) && string.IsNullOrWhiteSpace(record.SmSignedFilePath))
                {
                    if (record.CustomerName == "舜宇集团有限公司")
                    {
                        keywordSigners.Add(new KeywordSigner(record.EmpName, record.EmpIdCardNo, record.EmpOssUrl, "声明人：", 120, null));
                    }
                    else
                    {
                        keywordSigners.Add(new KeywordSigner(record.EmpName, record.EmpIdCardNo, record.EmpOssUrl, "乙方：（签 章）", 120, null));
                    }
                    if (record.IsSmNeedSigningUnitSignKeyword)
                    {
                        keywordSigners.Add(new KeywordSigner(record.SigningUnitName, record.SigningUnitCompanyNo, record.SigningUnitOssUrl, "甲方：（盖 章）", 120, null));
                    }
                    var smResp = await fxqService.KeywordPdfContractSign(record.SmPdfFilePath, keywordSigners.ToArray(), 1, null, null, new SignDate(1, null, null, null));
                    if (smResp != null)
                    {
                        if (smResp.Code == 10000)
                        {
                            record.SmSignedFilePath = smResp.Data;
                            record.SignedTime = DateTime.Now;
                        }
                        else
                        {
                            throw new ErrorCodeException(-1, $"签章失败，异常信息：{smResp?.Msg}");
                        }
                    }
                    else
                    {
                        throw new ErrorCodeException(-1, "签署异常，请稍后再试");
                    }
                }
            }


            if (!string.IsNullOrWhiteSpace(record.SigningUnitSignKeyword) && !string.IsNullOrWhiteSpace(record.PdfFilePath) && string.IsNullOrWhiteSpace(record.SignedFilePath))
            {
                List<KeywordSigner> keywordSigners = new List<KeywordSigner>();
        
                keywordSigners.Add(new KeywordSigner(record.EmpName, record.EmpIdCardNo, record.EmpOssUrl, record.EmpSignKeyword, 120, null));
                keywordSigners.Add(new KeywordSigner(record.SigningUnitName, record.SigningUnitCompanyNo, record.SigningUnitOssUrl, record.SigningUnitSignKeyword, 120, record.SigningUnitSignPageHeight ?? null));

                var smResp = await fxqService.KeywordPdfContractSign(record.PdfFilePath, keywordSigners.ToArray(), 1, null, null, new SignDate(1, null, null, null));

                if (smResp != null)
                {
                    if (smResp.Code == 10000)
                    {
                        record.SignedFilePath = smResp.Data;
                        record.SignedTime = DateTime.Now;
                    }
                    else
                    {
                        throw new ErrorCodeException(-1, $"签章失败，异常信息：{smResp?.Msg}");
                    }
                }
                else
                {
                    throw new ErrorCodeException(-1, "签署异常，请稍后再试");
                }
            }

            UserContractStatusEventData eventData = new UserContractStatusEventData() { EntryID=record.UserEntryID.Value,
            ContractDate=record.ContractDate,
            ContractEndDate=record.ContractEndDate.Value,
            ContractStartDate=record.ContractStartDate.Value,
            };


            if (!string.IsNullOrWhiteSpace(record.SmSignedFilePath))
            {
               var pdfFileBase64 = await fxqService.DownloadFile(record.SmSignedFilePath);
                if (!string.IsNullOrWhiteSpace(pdfFileBase64))
                {

                    using (var ms = new MemoryStream(Convert.FromBase64String(pdfFileBase64)))
                    {
                        var ossResp = await aliyunOSSService.UploadFile(ms, $"{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.pdf", true, false, "ContractSigned");
                        if (ossResp.Code == 0)
                        {
                            record.SmSignedFilePath = "https://oss.cn-labor.cn" + ossResp.Data;
                        }
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(record.SignedFilePath))
            {
                var pdfFileBase64 = await fxqService.DownloadFile(record.SignedFilePath);
                if (!string.IsNullOrWhiteSpace(pdfFileBase64))
                {

                    using (var ms = new MemoryStream(Convert.FromBase64String(pdfFileBase64)))
                    {
                        var ossResp = await aliyunOSSService.UploadFile(ms, $"{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.pdf", true, false, "ContractSigned");
                        if (ossResp.Code == 0)
                        {
                            record.SignedFilePath = "https://oss.cn-labor.cn" + ossResp.Data;
                        }
                    }
                }
            }

            if (record.ContractType == ContractEnumType.入职合同)
            {
                if (!string.IsNullOrWhiteSpace(record.SmSignedFilePath) && !string.IsNullOrWhiteSpace(record.SignedFilePath))
                {
                    record.Status = SignStatusEnumType.待存证;
                    eventData.Status = ContractStatus.EntryComplete;
                }
                else
                {
                    record.Status = SignStatusEnumType.签署失败;
                    eventData.Status = ContractStatus.EntryCreateError;
                }
                await service.Update(record);
                eventPublisher.Publish(eventData);

            }
            else if (record.ContractType == ContractEnumType.离职合同)
            {
                if (!string.IsNullOrWhiteSpace(record.SignedFilePath))
                {
                    record.Status = SignStatusEnumType.待存证;
                    eventData.Status = ContractStatus.LeaveComplete;
                }
                else
                {
                    record.Status = SignStatusEnumType.签署失败;
                    eventData.Status = ContractStatus.LeaveCreateError;
                }
                await service.Update(record);
                eventPublisher.Publish(eventData);
            }
            else {
                if (!string.IsNullOrWhiteSpace(record.SignedFilePath))
                {
                    record.Status = SignStatusEnumType.待存证;
                }
                else
                {
                    record.Status = SignStatusEnumType.签署失败;
                }
                await service.Update(record);
            }
          
            return Ok(record);
        }

       

        [AllowAnonymous]
        [HttpPut]
        public async Task<IActionResult> DevEmpSign([FromForm] int userEntryID )
        {
            //var entry = await fileConfigService.GetUserEntry(f => f.ID == userEntryID);
            //if (entry == null)
            //{
            //    throw new ErrorCodeException(-1, "无数据！");
            //}
            //var config = await customerContractConfigService.GetByCustomerGID(entry.CustomerGID.Value);
            //if (config == null)
            //{
            //    throw new ErrorCodeException(-1, "合同配置文件未配置！");
            //}
            //Live_ContractFileConfigExtension tempFile = null;
            //if (entry.Status == 2)
            //{
            //    tempFile = await fileConfigService.GetNav(f => f.ID == config.EntryFileID.Value);
            //    if (tempFile == null)
            //    {
            //        throw new ErrorCodeException(-1, "入职合同模板缺失！");
            //    }
            //}
            //else
            //{
            //    tempFile = await fileConfigService.GetNav(f => f.ID == config.LeaveFileID.Value);
            //    if (tempFile == null)
            //    {
            //        throw new ErrorCodeException(-1, "离职合同模板缺失！");
            //    }
            //}





            //var docxFileBase64 = await fxqDevService.DownloadFile(tempFile.FilePath);
            //if (string.IsNullOrWhiteSpace(docxFileBase64))
            //{
            //    throw new ErrorCodeException(-1, "合同文件下载失败");
            //}

            //var cus = await customerService.GetView(f => f.CustomerGID == entry.CustomerGID.Value);

            //var signUnit = enterpriseService.Get(f => f.ID == cus.SigningUnitGID);
            //var empRealRecord = await signerService.Get(f => f.Idno == entry.CardID && !string.IsNullOrWhiteSpace(f.OssUrl));
            //var signUnitRealRecord = await signerService.Get(f => f.Idno == cus.SigningUnitCompanyNo && !string.IsNullOrWhiteSpace(f.OssUrl));

            //string empImg = string.Empty;
            //string signUnitImg = string.Empty;
            ////if (empRealRecord != null)
            ////{
            ////    empImg = empRealRecord.OssUrl;
            ////}


            //   var res=await fxqDevService.ElectronicSeal(entry.Name);
            //    empImg= res.Data;

            //if (signUnitRealRecord != null)
            //{
            //    signUnitImg = signUnitRealRecord.OssUrl;
            //}
            //var keywords = new Dictionary<string, string>();
            //keywords.Add("PartyA", cus.SigningUnitName);
            //keywords.Add("LegalPersonName", signUnit.Result.LegalPerson ?? "未设置");
            //keywords.Add("LegalPersonAddress", cus.signingUnitAddress ?? "未设置");
            //keywords.Add("PartyB", entry.Name);
            //keywords.Add("Sex", entry.Sex ?? "未设置");
            //keywords.Add("Phone", entry.Phone ?? "未设置");
            //keywords.Add("IdCardNo", entry.CardID);
            //keywords.Add("JobCity", cus.JobCity ?? "未设置");
            //keywords.Add("JobName", entry.WorkName ?? "未设置");
            //keywords.Add("PlaceOfResidence", entry.Address ?? "未设置");
            //keywords.Add("ContractStartDate", entry.EntryTime.Value.ToString("yyyy-MM-dd"));
            //keywords.Add("ContractEndDate", entry.EntryTime.Value.AddMonths(config.SignMonths.Value).ToString("yyyy-MM-dd"));
            //keywords.Add("ProbationPeriod", entry.EntryTime.Value.AddMonths(config.TrialMonths.Value).ToString("yyyy-MM-dd"));
            //keywords.Add("RegisteredResidence", entry.IdcardAddress ?? "未设置");
            //if (entry.LeaveTime.HasValue)
            //{
            //    keywords.Add("LeaveDate", entry.LeaveTime.Value.ToString("yyyy-MM-dd"));
            //}
            //keywords.Add("EducationName", entry.Education ?? "未设置");
            //keywords.Add("EmergencyContact", entry.EmergencyContact ?? "未设置");
            //keywords.Add("EmergencyContactPhone", entry.EmergencyContactPhone ?? "未设置");
            //keywords.Add("IdCardAddress", entry.IdcardAddress);

            //keywords.Add("TrialStartDate", entry.EntryTime.Value.ToString("yyyy-MM-dd"));

            //keywords.Add("TrialEndDate", entry.EntryTime.Value.AddMonths(config.TrialMonths.Value).ToString("yyyy-MM-dd"));

            //keywords = keywords.OrderBy(f => f.Key).Where(f => !string.IsNullOrWhiteSpace(f.Value)).ToDictionary(f => f.Key, f => f.Value);

            //var contractBase64 = await fxqDevService.DocxKeywordReplace(docxFileBase64, keywords, 0);

          var record=await  service.Get(f => f.ID == Guid.Parse("E80E1B70-C580-4A56-8EBA-5F173F5A7C99"));
            List<KeywordSigner> keywordSigners = new List<KeywordSigner>();


           var signer1= await signerService.GetSigner(record.SigningUnitName, record.SigningUnitCompanyNo, true);

            var signer2 = await signerService.GetSigner(record.EmpName, record.EmpIdCardNo, false);

            keywordSigners.Add(new KeywordSigner(record.SigningUnitName, record.SigningUnitCompanyNo, signer1.OssUrl, record.SigningUnitSignKeyword, 120, 120));

            keywordSigners.Add(new KeywordSigner(record.EmpName, record.EmpIdCardNo, signer2.OssUrl, record.EmpSignKeyword, 120, null));


            var pdfFile = await fxqDevService.DownloadFile(record.SmPdfFilePath);
            var signRes = await fxqDevService.KeywordPdfContractSign(pdfFile, keywordSigners.ToArray(), 1, null, null, new SignDate(1, null, null, null));

            if (signRes != null && signRes.Code == 10000)
            {
                return Ok(signRes.Data);
            }
            else
            {

                throw new ErrorCodeException(-1, signRes.Msg);
            }



        }
        #region 电子合同网页版接口



        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> WebSignRecord(Guid id)
        {
            //var miText = AesGcmHelper.Encrypt("hello", Convert.FromBase64String(aesOptions.Value.Key));
            //Console.WriteLine(miText);
            //var mingText = AesGcmHelper.Decrypt(miText, Convert.FromBase64String(aesOptions.Value.Key));
            var record = await service.Get(f => f.ID == id);
            ContractRecordSignResponse response = new ContractRecordSignResponse()
            {
                KeyID = record.ID.ToString(),
                Age = record.EmpAge?.ToString() ?? string.Empty,
                ContractEndDate = record.ContractEndDate.Value.ToString("yyyy年MM月dd日") ?? string.Empty,
                ContractStartDate = record.ContractStartDate.Value.ToString("yyyy年MM月dd日") ?? string.Empty,
                CustomerName = record.CustomerName,
                EntryDate = record.EntryDate?.ToString("yyyy年MM月dd日") ?? string.Empty,
                HtUrl = record.PdfFilePath,
                IdCardNo = record.EmpIdCardNo,
                LeaveDate = record.LeaveDate?.ToString("yyyy年MM月dd日") ?? string.Empty,
                Name = record.EmpName,
                Phone = record.EmpPhone,
                Sex = record.EmpSex,
                SignUnitName = record.SigningUnitName,
                SmUrl = record.SmPdfFilePath ?? string.Empty,
                StatusName = string.Empty,
                ContractTypeName = record.ContractType?.ToString() ?? string.Empty,
            };
            if (!string.IsNullOrWhiteSpace(record.SignedFilePath))
            {
                response.HtUrl = record.SignedFilePath;
                response.StatusName = "已签署";
            }
            else
            {
                response.StatusName = "待签署";
            }

            if (!string.IsNullOrWhiteSpace(record.SmSignedFilePath))
            {
                response.SmUrl = record.SmSignedFilePath;
            }


            if (!string.IsNullOrWhiteSpace(response.SmUrl)&& response.StatusName== "待签署")
            {
                response.SmBase64 = await fxqService.DownloadFile(response.SmUrl);
            }
            else
            {
                response.SmBase64 = string.Empty;
            }
            if (!string.IsNullOrWhiteSpace(response.HtUrl)&& response.StatusName == "待签署")
            {
                response.HtBase64 = await fxqService.DownloadFile(response.HtUrl);
            }
            else
            {
                response.HtBase64 = string.Empty;
            }
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> DownloadPdf([FromForm] Guid id,bool isHt)
        {
            var record = await service.Get(f => f.ID == id);
            string fileUrl = string.Empty;

            if (isHt)
            {
                fileUrl = record.PdfFilePath;
                if (!string.IsNullOrWhiteSpace(record.SignedFilePath))
                {
                    fileUrl = record.SignedFilePath;
                }
            }
            else {
                fileUrl = record.SmPdfFilePath;
                if (!string.IsNullOrWhiteSpace(record.SmSignedFilePath))
                {
                    fileUrl = record.SmSignedFilePath;
                }
            }
            var base64String = await fxqService.DownloadFile(fileUrl);
            Console.WriteLine(base64String);
            return Ok(new { base64String, fileName = $"文件.pdf", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });


            //return File(Convert.FromBase64String(base64String), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "文件.pdf");

        }
        #endregion

    }
}
