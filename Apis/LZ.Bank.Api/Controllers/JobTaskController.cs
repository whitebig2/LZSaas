using LZ.ApiService.Interface;
using LZ.Bank.Service.Interface;
using LZ.Bank.Service.Services;
using LZ.Common;
using LZ.Core.Interface;
using LZ.Core.Redis;
using LZ.Core.Services;
using LZ.Model;
using LZ.Model.Bank.Apip;
using LZ.Model.Bank.CZY.BusinessModels;
using LZ.Model.Bank.CZY.ParamSetModels;
using LZ.Model.Dtos.FinanceDo;
using LZ.Model.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;

namespace LZ.Bank.Api.Controllers
{
    public class JobTaskController : BankControllerBase
    {
        private readonly IIcbcOpenAccountRecordService _icbcOpenAccountRecordService;
        private readonly IBankIcbcService _bankIcbcService;
        private readonly IHttpService _httpService;
        private readonly ICzyService _czyService;
        private readonly IBankAccountService _accountService;
        private readonly IAliyunOSSService _ossService;
        private readonly IBankElectronicReceiptService _bankElectronicReceiptService;
        private readonly IBankTransactionService _bankTransactionService;
        private readonly ICzyPayOrderService _payOrderService;
        private readonly ICzyApipService _czyApipService;
        private readonly IRedisService _redisService;
        public JobTaskController(IIcbcOpenAccountRecordService icbcOpenAccountRecordService, IBankIcbcService bankIcbcService, IHttpService httpService, ICzyService czyService, IBankAccountService accountService, IAliyunOSSService ossService, IBankElectronicReceiptService bankElectronicReceiptService, IBankTransactionService bankTransactionService, ICzyPayOrderService payOrderService, ICzyApipService czyApipService, IRedisService redisService)
        {
            _icbcOpenAccountRecordService = icbcOpenAccountRecordService;
            _bankIcbcService = bankIcbcService;
            _httpService = httpService;
            _czyService = czyService;
            _accountService = accountService;
            _ossService = ossService;
            _bankElectronicReceiptService = bankElectronicReceiptService;
            _bankTransactionService = bankTransactionService;
            _payOrderService = payOrderService;
            _czyApipService = czyApipService;
            _redisService = redisService;
        }

        [HttpPost]
        public async Task<IActionResult> OpenCardApplyJob()
        {
            var models = await _icbcOpenAccountRecordService.List(t => t.Status == OpenAccountProcessStatusEnumType.待开户);
            var arr = models.Select(t => t.BatchNo).Distinct().ToList();
            List<Live_IcbcOpenAccountRecord> list = new List<Live_IcbcOpenAccountRecord>();
            foreach (var x in arr)
            {
                var subs = models.Where(t => t.BatchNo == x).ToList();
                if (subs.Count > 0)
                {
                    var excelResult = await _bankIcbcService.OpenAccountApplyAsync(subs[0].FilePath, subs[0].ExcelFileName);
                    if (excelResult.Retcode != "0000")
                    {
                        foreach (var sub in subs)
                        {
                            sub.Status = OpenAccountProcessStatusEnumType.报表上传失败;
                            sub.ResCode = excelResult.Retcode;
                            sub.ResMsg = excelResult.Retmsg;
                            list.Add(sub);
                        }
                        continue;
                    }
                    var imgresult = await _bankIcbcService.OpenAccountUploadImageApplyAsync(subs[0].FilePath, subs[0].ZipFileName, DateTime.Now);
                    if (imgresult.Retcode != "0000")
                    {
                        foreach (var sub in subs)
                        {
                            sub.Status = OpenAccountProcessStatusEnumType.影像资料上传失败;
                            sub.ResCode = imgresult.Retcode;
                            sub.ResMsg = imgresult.Retmsg;
                            list.Add(sub);
                        }
                        continue;
                    }
                    foreach (var sub in subs)
                    {
                        sub.Status = OpenAccountProcessStatusEnumType.待查询开户结果;
                        sub.ResCode = "0000";
                        sub.ResMsg = "上传成功";
                        list.Add(sub);
                    }

                }

            }
            await _icbcOpenAccountRecordService.Update(list);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> OpenCardQueryJob()
        {
            var models = await _icbcOpenAccountRecordService.List(t => t.Status == OpenAccountProcessStatusEnumType.待查询开户结果 && t.FailCount < 6);
            foreach (var x in models)
            {
                var result = await _bankIcbcService.OpenAccountQueryDetailAsync(x.Name, x.CardID, x.SendDate.Value);
                Console.WriteLine("银行卡查询结果返回：" + JsonConvert.SerializeObject(result));
                x.ResCode = result.Retcode;
                x.ResMsg = result.Retmsg;
                if (result.Retcode == "0000")
                {
                    x.Status = OpenAccountProcessStatusEnumType.完成;
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    dic.Add("name", x.Name);
                    dic.Add("cardID", x.CardID);
                    dic.Add("status", x.Status.ToString());
                    var json = JsonConvert.SerializeObject(dic);
                    var send = await _httpService.PostAsync(ServiceNames.UsersService, "/api/BankOpenAccount/OpenCardQuery", json);
                    Console.WriteLine("银行卡申请通过接口返回：" + JsonConvert.SerializeObject(send));
                    if (send.Code != 200)
                    {
                        x.FailMsg = send.Message ?? "开卡申请失败";
                    }
                }
                else
                {
                    x.FailCount++;
                }


            }
            await _icbcOpenAccountRecordService.Update(models);
            return Ok();
        }




        [HttpPost]
        public async Task<IActionResult> BankElectronicReceiptJob()
        {
            var accounts = await _accountService.List(f => f.IsEnable);
            List<Live_BankElectronicReceipt> records = new List<Live_BankElectronicReceipt>();
            Live_BankElectronicReceipt record = null;
            T0028ParamSetItem req;
            bool hasNextTag = true;
            string nextTag = "0";
            foreach (var accountItem in accounts)
            {
                hasNextTag = true;
                nextTag = "0";
                while (hasNextTag)
                {
                    req = new T0028ParamSetItem()
                    {
                        AcctNo = accountItem.AcctNo,
                        BeginDate = DateTime.Today.ToString("yyyy-MM-dd"),
                        EndDate = DateTime.Today.ToString("yyyy-MM-dd"),
                        CurrencyCode = "CNY",
                        NextTag = $"{nextTag}",
                    };
                    var res = await _czyService.QueryElectronicReceiptList(req);
                    var bodyData = res.Cmscloud.Body.Data;
                    if (bodyData.ResultSet != null && bodyData.ResultSet.Count > 0)
                    {

                        foreach (var item in bodyData.ResultSet)
                        {
                            var isExist = await _bankElectronicReceiptService.Exist(f => f.RelBillsCode == item.RelBillsCode);
                            if (!isExist)
                            {
                                record = new Live_BankElectronicReceipt()
                                {
                                    BillsPayOrderNo = item.BillsPayOrderNo,
                                    CreateTime = DateTime.Now,
                                    FileName = item.FileName,
                                    ReceiptBatchNo = item.ReceiptBatchNo,
                                    ReceiptInnerNo = item.ReceiptInnerNo,
                                    ID = Guid.NewGuid(),
                                    RelBillsCode = item.RelBillsCode,
                                };
                                using (var ms = new MemoryStream(Convert.FromBase64String(item.FileContent)))
                                {
                                    string objectKey = $"BankPay/ElectronicReceipt/{item.FileName}";
                                    var ossRes = await _ossService.UploadOssFile(ms, $"BankPay/ElectronicReceipt/{item.FileName}");
                                    if (ossRes.Code == 200)
                                    {
                                        record.FilePath = objectKey;
                                    }
                                }
                                records.Add(record);
                            }
                        }
                    }

                    if (string.IsNullOrWhiteSpace(bodyData.NextTag) || bodyData.NextTag == "0")
                    {
                        hasNextTag = false;
                    }
                    nextTag = bodyData.NextTag;
                }
            }
            if (records.Count > 0)
            {
                await _bankElectronicReceiptService.Add(records);
            }
            return Ok();
        }



        [HttpPost]
        public async Task<IActionResult> BankTransactionResultJob()
        {

            var models = await _bankTransactionService.List(f => f.Status == BankTransactionStatusEnumType.已推送待查询 && f.IsSendNotice == false);
            foreach (var modelItem in models)
            {
                T0004ParamSetItem reqItem = new T0004ParamSetItem() { BillsPayOrderNo = modelItem.BillsPayOrderNo };
                var modelRes = await _czyService.QueryTransactionDetail(reqItem);
            }

            return Ok();
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> PushToCzyJob()
        {
            // 多并发限制
            var cacheKey = "Bank_JobTask_PushToCzyJob";
            var isb = await _redisService.SetAsync(cacheKey, $"批次号:{Guid.NewGuid().ToString()}", 3600, CSRedis.RedisExistence.Nx);
            if (!isb)
            {
                return Ok("上一个任务未执行结束,稍后再试!");
            }
            try
            {
                var list = await _payOrderService.ListNav(f => f.PushStatus == CzyPushStatusEnumType.待推送 && f.PushTime == null);
            foreach (var modelItem in list)
            {
                if (modelItem.Records == null || modelItem.Records.Count == 0)
                {
                    modelItem.ReturnCode = "-1";
                    modelItem.ReturnMsg = "支付明细不能为空";
                    modelItem.PushStatus = CzyPushStatusEnumType.推送失败;
                    await _payOrderService.Update(modelItem);
                }
                else
                {
                    try
                    {

                            var dt = DateTime.Now;
                            await _payOrderService.UpdatePushTime(modelItem.ID, dt);


                        ApipResponse res = null;
                        List<PaymentFailResultItem> resultSet = null;

                        if (modelItem.PayType == CzyPayTypeEnumType.批量对私)
                        {
                            var paramSet = new BillBatchPrivateRequestV1.BillBatchPrivateRequestParamSet
                            {
                                BillsPayOrderNo = modelItem.BillsPayOrderNo,
                                PayType = ((int)modelItem.PayType).ToString(),
                                PaymentDate = (modelItem.PaymentDate ?? DateTime.Today).ToString("yyyy-MM-dd"),
                                BusinessTypeCode = modelItem.BusinessTypeCode,
                                BusinessTypeName = modelItem.BusinessTypeName,
                                PayOrgCode = modelItem.PayOrgCode,
                                PayOrgName = modelItem.PayOrgName,
                                PayAccountNo = modelItem.PayAccountNo,
                                PayChannel = ((int)modelItem.PayChannel).ToString(),
                                PayAmount = modelItem.PayAmount.ToString("0.##"),
                                PayCurrencyType = string.IsNullOrWhiteSpace(modelItem.PayCurrencyType) ? "CNY" : modelItem.PayCurrencyType,
                                PayCurrencyName = string.IsNullOrWhiteSpace(modelItem.PayCurrencyName) ? "人民" : modelItem.PayCurrencyName,
                                WhetherUrgent = modelItem.WhetherUrgent ? "1" : "2",
                                Remark = modelItem.Remark,
                                PayUse = modelItem.PayUse,
                                CreateByCode = modelItem.CreateByCode,
                                CreateByName = modelItem.CreateByName,
                                Purpose = modelItem.Purpose,
                                BatchPrivateRecList = modelItem.Records.Select(f => new BillBatchPrivateRequestV1.BillBatchPrivateRequestBatchPrivateRec
                                {
                                    BillsPayOrderNo = f.BillsPayOrderNo,
                                    PrivateAmount = f.PrivateAmount.ToString("0.##"),
                                    ReceiveAccountNo = f.ReceiveAccountNo,
                                    ReceiveAccountName = f.ReceiveAccountName,
                                    ReceiveAreaNameOfProvince = f.ReceiveAreaNameOfProvince,
                                    ReceiveAreaNameOfCity = f.ReceiveAreaNameOfCity,
                                    ReceiveBranchCode = f.ReceiveBranchCode,
                                    ReceiveBranchName = f.ReceiveBranchName,
                                    ReceiveNature = string.Empty,
                                    ReceiveCurrencyCode = string.IsNullOrWhiteSpace(f.ReceiveCurrencyCode) ? "CNY" : f.ReceiveCurrencyCode,
                                    ReceiveCurrencyName = string.IsNullOrWhiteSpace(f.ReceiveCurrencyName) ? "人民" : f.ReceiveCurrencyName,
                                }).ToList()
                            };

                            var batchRes = await _czyApipService.BillBatchPrivate(paramSet);
                            res = batchRes;
                            resultSet = batchRes?.ResultSet;
                        }
                        else if (modelItem.PayType == CzyPayTypeEnumType.普通付款)
                        {
                            if (modelItem.Records.Count != 1)
                            {
                                throw new Exception("普通付款只能有一条支付明细");
                            }

                            var rec = modelItem.Records.First();
                            var paramSet = new PaymentOrdinarypayRequestV1.PaymentOrdinarypayRequestParamSet
                            {
                                BillsPayOrderNo = modelItem.BillsPayOrderNo,
                                PayType = ((int)modelItem.PayType).ToString(),
                                PaymentDate = (modelItem.PaymentDate ?? DateTime.Today).ToString("yyyy-MM-dd"),
                                BusinessTypeCode = modelItem.BusinessTypeCode,
                                BusinessTypeName = modelItem.BusinessTypeName,
                                PayOrgCode = modelItem.PayOrgCode,
                                PayOrgName = modelItem.PayOrgName,
                                PayAccountNo = modelItem.PayAccountNo,
                                PayChannel = ((int)modelItem.PayChannel).ToString(),
                                PayAmount = modelItem.PayAmount.ToString("0.##"),
                                PayCurrencyType = string.IsNullOrWhiteSpace(modelItem.PayCurrencyType) ? "CNY" : modelItem.PayCurrencyType,
                                PayCurrencyName = string.IsNullOrWhiteSpace(modelItem.PayCurrencyName) ? "人民" : modelItem.PayCurrencyName,
                                WhetherUrgent = modelItem.WhetherUrgent ? "1" : "2",
                                Remark = modelItem.Remark,
                                PayUse = modelItem.PayUse,
                                Purpose = modelItem.Purpose,
                                PaymentRec = new PaymentOrdinarypayRequestV1.PaymentOrdinarypayRequestPaymentRec
                                {
                                    ReceiveAccountNo = rec.ReceiveAccountNo,
                                    ReceiveAccountName = rec.ReceiveAccountName,
                                    ReceiveAreaNameOfProvince = rec.ReceiveAreaNameOfProvince,
                                    ReceiveAreaNameOfCity = rec.ReceiveAreaNameOfCity,
                                    ReceiveBranchCode = rec.ReceiveBranchCode,
                                    ReceiveBranchName = rec.ReceiveBranchName,
                                        ReceiveNature = string.Empty,
                                    ReceiveCurrencyCode = string.IsNullOrWhiteSpace(rec.ReceiveCurrencyCode) ? "CNY" : rec.ReceiveCurrencyCode,
                                    ReceiveCurrencyName = string.IsNullOrWhiteSpace(rec.ReceiveCurrencyName) ? "人民" : rec.ReceiveCurrencyName,
                                }
                            };

                            var ordinaryRes = await _czyApipService.PaymentOrdinaryPay(paramSet);
                            res = ordinaryRes;
                            resultSet = ordinaryRes?.ResultSet;
                        }
                        else if (modelItem.PayType == CzyPayTypeEnumType.批量付款)
                        {
                            var paramSet = new PaymentBatchpayRequestV1.PaymentBatchpayRequestParamSet
                            {
                                BillsPayOrderNo = modelItem.BillsPayOrderNo,
                                PayType = ((int)modelItem.PayType).ToString(),
                                PaymentDate = (modelItem.PaymentDate ?? DateTime.Today).ToString("yyyy-MM-dd"),
                                BusinessTypeCode = modelItem.BusinessTypeCode,
                                BusinessTypeName = modelItem.BusinessTypeName,
                                PayOrgCode = modelItem.PayOrgCode,
                                PayOrgName = modelItem.PayOrgName,
                                PayAccountNo = modelItem.PayAccountNo,
                                PayChannel = ((int)modelItem.PayChannel).ToString(),
                                SumPayAmount = modelItem.PayAmount.ToString("0.##"),
                                PayCurrencyType = string.IsNullOrWhiteSpace(modelItem.PayCurrencyType) ? "CNY" : modelItem.PayCurrencyType,
                                PayCurrencyName = string.IsNullOrWhiteSpace(modelItem.PayCurrencyName) ? "人民" : modelItem.PayCurrencyName,
                                WhetherUrgent = modelItem.WhetherUrgent ? "1" : "2",
                                Remark = modelItem.Remark,
                                PayUse = modelItem.PayUse,
                                PaymentRecList = modelItem.Records.Select(f => new PaymentBatchpayRequestV1.PaymentBatchpayRequestPaymentRec
                                {
                                    BillsPayOrderNo = f.BillsPayOrderNo,
                                    PrivateAmount = f.PrivateAmount.ToString("0.##"),
                                    ReceiveAccountNo = f.ReceiveAccountNo,
                                    ReceiveAccountName = f.ReceiveAccountName,
                                    ReceiveAreaNameOfProvince = f.ReceiveAreaNameOfProvince,
                                    ReceiveAreaNameOfCity = f.ReceiveAreaNameOfCity,
                                    ReceiveBranchCode = f.ReceiveBranchCode,
                                    ReceiveBranchName = f.ReceiveBranchName,
                                    ReceiveNature = string.Empty,
                                    ReceiveCurrencyCode = string.IsNullOrWhiteSpace(f.ReceiveCurrencyCode) ? "CNY" : f.ReceiveCurrencyCode,
                                    ReceiveCurrencyName = string.IsNullOrWhiteSpace(f.ReceiveCurrencyName) ? "人民" : f.ReceiveCurrencyName,
                                    Summary = modelItem.PayUse,
                                    Postscript = modelItem.Purpose,
                                }).ToList()
                            };

                            var batchPayRes = await _czyApipService.PaymentBatchPay(paramSet);
                            res = batchPayRes;
                            resultSet = batchPayRes?.ResultSet;
                        }
                        else
                        {
                            throw new Exception($"不支持的付款类型：{(int)modelItem.PayType}");
                        }

                        modelItem.ReturnCode = res?.ReturnCode.ToString();
                        modelItem.ReturnMsg = res?.ReturnMsg;
                        modelItem.PushTime = DateTime.Now;
                        if (res != null && res.IsSuccess)
                        {
                            modelItem.PushStatus = CzyPushStatusEnumType.推送成功;
                            await _payOrderService.Update(modelItem);
                        }
                        else
                        {
                            modelItem.PushStatus = CzyPushStatusEnumType.推送失败;
                            if (resultSet != null && resultSet.Count > 0)
                            {
                                modelItem.ReturnFailReasons = JsonConvert.SerializeObject(resultSet);
                                foreach (var fail in resultSet)
                                {
                                    var detail = modelItem.Records.FirstOrDefault(f => f.BillsPayOrderNo == fail.BillsPayOrderNo);
                                    if (detail != null)
                                    {
                                        detail.FailReasons = fail.FailReasons;
                                    }
                                }
                                await _payOrderService.UpdateNav(modelItem);
                            }
                            else
                            {
                                await _payOrderService.Update(modelItem);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                            // 只捕获异常
                        modelItem.ReturnCode = "-1";
                        modelItem.ReturnMsg = ex.Message;
                        modelItem.PushStatus = CzyPushStatusEnumType.推送失败;
                        modelItem.PushTime = DateTime.Now;
                        await _payOrderService.Update(modelItem);
                            // 记录异常
                            StaticLog.Debug($"Bank_推送失败_id:{modelItem.ID.ToString()}" + ex.Message, ex);
                    }
                }

                var noticeJson = JsonConvert.SerializeObject(new PaymentOrderPushNoticeDto
                {
                    RelBillsCode = modelItem.RelBillsCode,
                    BillsPayOrderNo = modelItem.BillsPayOrderNo,
                    IsSuccess = modelItem.PushStatus == CzyPushStatusEnumType.推送成功,
                    ReturnCode = modelItem.ReturnCode,
                    ReturnMsg = modelItem.ReturnMsg,
                    PushTime = modelItem.PushTime
                });
                await _httpService.PostAsync(ServiceNames.FinanceService, "/api/BusNotice/PaymentOrderPushNotice", noticeJson);
            }
            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            {
                // 无论成功与否都释放
                await _redisService.DeleteAsync(cacheKey);
            }
            return Ok();
        }
    }
}
