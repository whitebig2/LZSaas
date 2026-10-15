using LZ.ApiService.Interface;
using LZ.Clients.API;
using LZ.Core.EventData;
using LZ.Core.Interface;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Core.Services;
using LZ.Finance.Service.Interface;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.FxqModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
using System.Collections.Generic;
using static LZ.Common.NpoiExcelHelper;

namespace LZ.Finance.API.Controllers
{
    [AllowAnonymous]
    public class ContractFileSigningController : FinanceControllerBase
    {

        private readonly ISalaryRecordService      _salaryRecordService;
        private readonly IAliyunOSSService     _aliyunOSSService;
        private readonly IFxqService    _fxqService;
        private readonly IFinanceOtherService   _financeOtherService;
        private readonly ISettingsService  _settingsService;
        private readonly ISalaryCashSendRecordService _cashSendRecordService;
        private readonly IPrepaidApplyRecordService _prepaidApplyRecordService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IHttpService _httpService;

        public ContractFileSigningController(ISalaryCashSendRecordService cashSendRecordService, IPrepaidApplyRecordService prepaidApplyRecordService, ISettingsService settingsService, IFinanceOtherService financeOtherService, IFxqService fxqService, IAliyunOSSService aliyunOSSService, ISalaryRecordService salaryRecordService, IEventPublisher eventPublisher, IHttpService httpService)
        {
            _cashSendRecordService = cashSendRecordService;
            _prepaidApplyRecordService = prepaidApplyRecordService;
            _settingsService = settingsService;
            _financeOtherService = financeOtherService;
            _fxqService = fxqService;
            _aliyunOSSService = aliyunOSSService;
            _salaryRecordService = salaryRecordService;
            _eventPublisher = eventPublisher;
            _httpService = httpService;
        }


        public record CashContractResponse(Guid Id, CashContractTypeEnumType CashType,string CustomerName,string Name,string IdCardNo,decimal TotalAmount,string FilePath,string SignedFilePath, bool IsNeedSmsCode, string Title, string note);
        [HttpGet]
        public async Task<IActionResult> ListByUserID(int userID)
        {
            List<CashContractResponse> list = new List<CashContractResponse>();
            var cashList = await _cashSendRecordService.ListView(f => f.Status == Model.Enums.CashSendStatusEnumType.待签署&&f.UserID==userID);
            var prepaidList = await _prepaidApplyRecordService.ListView(f => f.CashStatus ==  PrepaidCashStatusEnumType.待签署 && f.UserID == userID);



            Dictionary<string,string>dss= new Dictionary<string,string>();  
            dss.Add("userID", userID.ToString());

            var cashSignRecordList = await _httpService.GetAsync<List<Live_CashSignRecord>>(ServiceNames.CustomerService, $"/api/CashSignRecord/ListByUserID?userID={userID}",isItem:true );


            if (cashSignRecordList != null)
            {
                var cashContractFileList = cashSignRecordList.Take(5).Select(f => new CashContractResponse(f.GID.Value, CashContractTypeEnumType.工资现金签收单, f.CustomerName, f.Name, f.CardID, 0, f.PdfPath, f.SignedPath,true,"现金承诺函","无")).ToList();


                list.AddRange(cashContractFileList);
            }

            //var customers = await _httpService.GetAsync<List<Model.Live_Customer>>(ServiceNames.CustomerService, "/api/customer/list?page=0&companyId=" + user.CompanyID, isItem: true);

            string phone = string.Empty;
            if (cashList != null && cashList.Count > 0)
            {
                var cashContractFileList = cashList.Take(5).Select(f => new CashContractResponse(f.ID,  CashContractTypeEnumType.工资现金签收单, f.CustomerName, f.Name, f.IdCardNo, f.TotalAmount, f.FilePath, f.SucFilePath, IsNeedSmsCode: false, "现金签收单（工资）", $"{f.TotalAmount}元")).ToList();
                list.AddRange(cashContractFileList);
            }
            if (prepaidList != null && prepaidList.Count > 0)
            {
                var prepaidContractFileList = prepaidList.Take(5).Select(f => new CashContractResponse(f.ID,  CashContractTypeEnumType.预发现金签收单, f.CustomerName, f.Name, f.IdCardNo, f.ApplyAmount, f.PdfFilePath, f.SignedFilePath, IsNeedSmsCode: false, "现金签收单（预发）", $"{f.ApplyAmount}元"));
                list.AddRange(prepaidContractFileList);
            }

            if (list!=null&& list.Any())
            {
                // 为确保成功
                RealName2EventData realName2 = new RealName2EventData(list.First().Name, list.First().IdCardNo);
                _eventPublisher.Publish(realName2);
            }
            return Ok(list);
        }



        [HttpPost]
        public async Task<IActionResult> Signing([FromForm] Guid id, [FromForm] CashContractTypeEnumType cashType,[FromForm]string smsCode, [FromForm] string empName)
        {
            //List<CashContractResponse> list = new List<CashContractResponse>();
             //list.Add(new CashContractResponse(Guid.Empty, CashContractTypeEnumType.工资现金签收单, "测试单位", "郑康", "320722199203033956", 99.99m, "https://oss.cn-labor.cn/files/20250503/6974ab1f93cd4c049ebc72cdf8ed48aa.pdf", string.Empty));
            var setting = await _settingsService.GetMasterSettings();
            Live_SalaryCashSendRecord cashSendRecord = null;
            Live_PrepaidApplyRecord applyRecord = null;
          Live_CashSignRecord cashSignRecord = null;
            string name=string.Empty;
            string filePath=string.Empty;
            string idCardNo = string.Empty;
            string signedFilePath = string.Empty;
            string failMsg = string.Empty;
            //name = list.FirstOrDefault().Name;
            //filePath = list.FirstOrDefault().FilePath;
            //idCardNo = list.FirstOrDefault().IdCardNo;
            //signedFilePath = string.Empty;
            //failMsg = string.Empty;



            if (cashType == CashContractTypeEnumType.工资现金签收单)
            {
                cashSendRecord = await _cashSendRecordService.Get(f => f.ID == id);

                if (cashSendRecord != null)
                {
                    var salaryRecord = await _salaryRecordService.Get(f => f.ID == cashSendRecord.SalaryRecordID);
                    name = salaryRecord.Name;
                    filePath = cashSendRecord.FilePath;
                    idCardNo = salaryRecord.IdCardNo;
                }
                else
                {

                    cashSignRecord = await _httpService.GetAsync<Live_CashSignRecord>(ServiceNames.CustomerService, $"/api/CashSignRecord/Get?gid={id.ToString()}", isItem: false);

                    if (cashSignRecord.Name!=empName.Trim())
                    {
                        throw new ErrorCodeException(-1, "姓名信息不匹配");
                    }

                    if (cashSignRecord.SmsCode!=smsCode.Trim())
                    {
                        throw new ErrorCodeException(-1, "验证码信息不匹配");
                    }

                    name = cashSignRecord.Name;
                    filePath = cashSignRecord.PdfPath;
                    idCardNo = cashSignRecord.CardID;
                }

            }
            else
            {
                applyRecord = await _prepaidApplyRecordService.Get(f => f.ID == id);
                name = applyRecord.Name;
                filePath = applyRecord.PdfFilePath;
                idCardNo = applyRecord.IdCardNo;
            }
            Live_ContractSigner signerInfo = null;
            var signerResp = await _financeOtherService.GetContractSignerByIdno(idCardNo);
            if (signerResp != null && signerResp.Code == 200)
            {
                signerInfo = signerResp.Data;
            }
            if (signerInfo == null)
            {
                throw new ErrorCodeException(-1, "电子签章信息缺失");
            }
            KeywordSigner signer = null;
            if (cashSignRecord != null)
            {
                signer = new KeywordSigner(name, idCardNo, signerInfo.OssUrl, cashSignRecord.SealKey, 120, null);
            }
            else { 
                signer = new KeywordSigner(name, idCardNo, signerInfo.OssUrl, "签章", 120, null);
            }

            var signResp = await _fxqService.KeywordPdfContractSign(filePath, new KeywordSigner[] { signer }, 1, null, null, new SignDate(1, null, null, null));
            if (signResp != null )
            {

                if (signResp.Code == 10000)
                {
                    signedFilePath = signResp.Data;
                }
                else
                {
                    throw new ErrorCodeException(-1, $"签署异常：{signResp.Msg}");
                }
            }
            else {
                throw new ErrorCodeException(-1, $"为获取到正确响应，请稍后再试");
            }
          
            try
            {
                if (!string.IsNullOrWhiteSpace(signedFilePath))
                {
                    var base64 = await _fxqService.DownloadFile(signedFilePath);
                    var fileBase64 = Convert.FromBase64String(base64);
                    using (var ms = new MemoryStream(fileBase64))
                    {
                        var ossResp = await _aliyunOSSService.UploadFile(ms, "11.pdf", true, false);
                        if (ossResp.Code == 0)
                        {
                            signedFilePath = setting.AliBucketDomain + ossResp.Data;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                failMsg = $"上传oss异常，异常信息：{ex.Message}";
            }


            if (!string.IsNullOrWhiteSpace(signedFilePath))
            {
                if (cashSignRecord != null)
                {
                    cashSignRecord.Status =  CashSignStatusEnumType.待存证;
                    cashSignRecord.SignedPath = signedFilePath;
                }
                else
                if (cashSendRecord != null)
                {
                    cashSendRecord.Status = CashSendStatusEnumType.已签署;
                    cashSendRecord.Msg = failMsg;
                    cashSendRecord.SucFilePath = signedFilePath;
                }
                else
                {
                    applyRecord.SignedFilePath = signedFilePath;
                    applyRecord.CashStatus = PrepaidCashStatusEnumType.已签署;
                }

            }
            else
            {

                if (cashSignRecord != null)
                {
                    cashSignRecord.Status = CashSignStatusEnumType.待签署;
                    cashSignRecord.FailMsg = failMsg;
                }
                else if (cashSendRecord != null)
                {
                    cashSendRecord.Status = CashSendStatusEnumType.待签署;
                    cashSendRecord.Msg = failMsg;
                }
                else
                {
                    applyRecord.CashStatus = PrepaidCashStatusEnumType.待签署;
                    applyRecord.Msg = failMsg;
                }
            }

            if (cashSignRecord != null)
            {
                //[FromForm] Guid gid, [FromForm] CashSignStatusEnumType status, [FromForm] string signedPath,  [FromForm] string failMsg
                Dictionary<string, string> dss = new Dictionary<string, string>();
                dss.Add("gid", cashSignRecord.GID.ToString());
                dss.Add("status", ((int)cashSignRecord.Status).ToString());
                dss.Add("signedPath", cashSignRecord.SignedPath);
                dss.Add("failMsg", cashSignRecord.FailMsg);
                var res = await _httpService.PutAsync(ServiceNames.CustomerService, $"/api/CashSignRecord/UpdateSignInfo", dss);

            }
            else
            if (cashSendRecord != null)
            {
              await  _cashSendRecordService.Update(cashSendRecord);
                var salaryRecord = await _salaryRecordService.Get(f => f.ID == cashSendRecord.SalaryRecordID);
                if (salaryRecord != null)
                {
                    salaryRecord.IsCashSignSuc = true;

                    _salaryRecordService.VerifyPendingSend(salaryRecord);

                    await _salaryRecordService.Update(salaryRecord);
                }
            }
            else
            {
                await _prepaidApplyRecordService.Update(applyRecord);
            }
            return Ok();
        }

    }
}
