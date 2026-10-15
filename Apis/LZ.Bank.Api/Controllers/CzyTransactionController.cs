using LZ.Bank.Service.Interface;
using LZ.Bank.Service.Services;
using LZ.Core;
using LZ.Core.Attributes;
using LZ.Core.Filters;
using LZ.Core.Redis;
using LZ.Model;
using LZ.Model.Bank.CZY;
using LZ.Model.Bank.CZY.BaseModels;
using LZ.Model.Bank.CZY.BodyData;
using LZ.Model.Bank.CZY.NoticeModels;
using LZ.Model.Bank.CZY.ParamSetModels;
using LZ.Model.Dtos.BankDo;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Senparc.Weixin.WxOpen.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace LZ.Bank.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CzyTransactionController : ApiControllerBase
    {

        private readonly  IRedisService _redisService;
        private readonly  ICzyService _czyService;
        private readonly  IBankTransactionService _bankTransactionService;
        private readonly  IBankCompanyConfigService _configService;
        private readonly  IBankCompanyService _bankCompanyService;
        public CzyTransactionController(ICzyService czyService, IBankTransactionService bankTransactionService, IRedisService redisService, IBankCompanyConfigService configService, IBankCompanyService bankCompanyService)
        {
            _czyService = czyService;
            _bankTransactionService = bankTransactionService;
            _redisService = redisService;
            _configService = configService;
            _bankCompanyService = bankCompanyService;
        }


        #region 查询
        /// <summary>
        ///  交易记录
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> PageListHistoryRecords(int page=1)
        {
            T0001ParamSetItem item = new T0001ParamSetItem()
            {
                AcctNo = string.Empty,
                CurrencyCode = string.Empty,
                EndTransDate = string.Empty,
                StartTransDate = string.Empty,
                NextTag = $"{page}",
            };
            var res = await _czyService.QueryTransactionHistoryRecords(item);
            var bodyData = res.Cmscloud.Body.Data;
            int total = Convert.ToInt32(bodyData.TotalNum);
            return Ok(new { items = bodyData.ResultSet, count = total });
        }


        /// <summary>
        /// 应付单业务状态查询接口
        /// </summary>
        /// <param name="transactionID"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> QueryTransactionDetail(Guid transactionID)
        {
            var model = await _bankTransactionService.Get(f => f.ID == transactionID);
            T0004ParamSetItem req = new T0004ParamSetItem() { BillsPayOrderNo = model.BillsPayOrderNo };
            var res = await _czyService.QueryTransactionDetail(req);
            return Ok(res.Cmscloud.Body.Data);
        }


        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> QueryElectronicReceiptRecords(string transType, string queryType, string acctNo, DateTime? beginDate, DateTime? endDate,int page=0)
        {
            page = page--;
            T0028ParamSetItem req = new T0028ParamSetItem()
            {
                AcctNo = acctNo,
                BeginDate = (beginDate ?? DateTime.Today).ToString("yyyyMMdd"),
                EndDate = (endDate ?? DateTime.Today).ToString("yyyyMMdd"),
                CurrencyCode = "CNY",
                TransType = transType,
                QueryType = queryType,
                NextTag = page.ToString()
            };
            var res = await _czyService.QueryElectronicReceiptList(req);
            return Ok(res.Cmscloud.Body.Data);
        }






        #endregion
        #region 交易
        /// <summary>
        /// 工资代发-批量对私
        /// 适用于工资代发
        /// </summary>
        /// <param name="requestDto"></param>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        [CzyTransactionFilter]
        [HttpPost]
        public async Task<IActionResult> BatchTransactionToPersonApply(BankRequestDto requestDto)
        {
            var model = await _bankTransactionService.GetNav(f => f.ID == requestDto.TransactionID);
            if (model.Status != Model.Enums.BankTransactionStatusEnumType.待推送)
            {
                throw new ErrorCodeException(-1, $"非【待推送】状态无法执行此操作");
            }
            model.BankCompanyConfigID = requestDto.ConfigID;
            model.Remark = requestDto.Remark;
            model.Purpose = requestDto.Purpose;
            model.Summary = requestDto.Summary;
            model.PaymentDate = requestDto.PaymentDate;
            model.WhetherUrgent = requestDto.WhetherUrgent;
            model.PayChannel = requestDto.PayChannel;
            // 获取配置
            var config = await _configService.GetView(f => f.ID == model.BankCompanyConfigID);
            model.PayAccountNo = config.MainAccountNo;
            model.PayOrgName = config.MainAccountOrgName;
            model.PayOrgCode = config.MainAccountOrgCode;

            // 获取账户余额
            T0006ParamSetItem accountReq = new T0006ParamSetItem()
            {
                AcctNo = model.PayAccountNo,
                CurrencyCode = model.PayCurrencyType,
                EndDate = DateTime.Today.ToString("yyyy-MM-dd"),
                StartDate = DateTime.Today.ToString("yyyy-MM-dd"),
            };
            var accountRes = await _czyService.QueryAccountBalanceInfo(accountReq);
            var accountBodyData = accountRes.Cmscloud.Body.Data;

            if (accountBodyData.ResultCode == "0000")
            {
                var accountInfo = accountBodyData.ResultSet.FirstOrDefault();
                var usableBalance = Convert.ToDecimal(accountInfo.UsableBalance);
                if (usableBalance < model.TotalAmount)
                {
                    await _bankTransactionService.UpdateNav(model);
                    throw new ErrorCodeException(-1, $"当前可用余额：{usableBalance}元，小于代发金额，无法代发。");
                }
            }
            else
            {
                await _bankTransactionService.UpdateNav(model);
                throw new ErrorCodeException(-1, accountBodyData.ResultMsg);
            }




            // 
            model.Status = Model.Enums.BankTransactionStatusEnumType.已推送待查询;
            await _bankTransactionService.UpdateNav(model);
            T0003ParamSetItem req = new T0003ParamSetItem()
            {
                SerialNo = model.SerialNo,
                BillsPayOrderNo = model.BillsPayOrderNo,
                PayType = $"{(int)model.PayType}",// 工资代发-批量对私
                PaymentDate = (model.PaymentDate ?? DateTime.Today).ToString("yyyy-MM-dd"),
                BusinessTypeCode = null,
                BusinessTypeName = model.BusinessTypeName,
                PayOrgCode = model.PayOrgCode,
                PayOrgName = model.PayOrgName,
                PayAccountNo = model.PayAccountNo,//付款方银行账号
                PayChannel = ((int)model.PayChannel).ToString(),
                PayAmount = model.TotalAmount.ToString(),
                PayCurrencyName = model.PayCurrencyName,
                PayCurrencyType = model.PayCurrencyType,
                PayUse = model.Summary,
                Purpose = model.Purpose,
                Remark = model.Remark,
                WhetherUrgent = model.WhetherUrgent ? "1" : "2",
                BatchPrivateRecList = model.Records.Select(f => new T0003BatchPrivateRecItem()
                {
                    BillsPayOrderNo = f.BillsPayOrderNo,
                    PrivateAmount = f.PrivateAmount.ToString(),
                    ReceiveAccountName = f.ReceiveAccountName,
                    ReceiveAccountNo = f.ReceiveAccountNo,
                    ReceiveNature = "2",
                    ReceiveCurrencyCode = f.ReceiveCurrencyCode ?? "CNY",
                    ReceiveCurrencyName = f.ReceiveCurrencyName ?? "人民币",
                    ReceiveBranchName = f.ReceiveBranchName,
                }).ToList()
            };
            var transRes = await _czyService.TransBatchToPerson(req);

            var bodyData = transRes.Cmscloud.Body.Data;
            if (bodyData.ResultCode == "0000")
            {
                // 推送成功
                model.PushTime = DateTime.Now;
                model.Status = Model.Enums.BankTransactionStatusEnumType.已推送待查询;
            }
            else if (bodyData.ResultCode == "-1")
            {
                model.Status = Model.Enums.BankTransactionStatusEnumType.推送失败;
            }
            else
            {
                model.PushTime = DateTime.Now;
                model.Status = Model.Enums.BankTransactionStatusEnumType.发放失败;
            }
            model.PushResultCode = bodyData.ResultCode;
            model.PushResultMsg = bodyData.ResultMsg;
            await _bankTransactionService.UpdateNav(model);
            return Ok(requestDto);
        }
        /// <summary>
        ///  单笔代发
        ///  对公或者对私
        /// </summary>
        /// <param name="requestDto"></param>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        [CzyTransactionFilter]
        [HttpPost]
        public async Task<IActionResult> SingleTransactionApply(BankRequestDto requestDto )
        {
            var model = await _bankTransactionService.GetNav(f => f.ID == requestDto.TransactionID);
            if (model.Status != Model.Enums.BankTransactionStatusEnumType.待推送)
            {
                throw new ErrorCodeException(-1, $"非【待推送】状态无法执行此操作");
            }

            if (model.Records.Count>1)
            {
                throw new ErrorCodeException(-1, $"检测到多代发记录，无法使用单笔代发功能");
            }
            model.BankCompanyConfigID = requestDto.ConfigID;
            model.Remark = requestDto.Remark;
            model.Purpose = requestDto.Purpose;
            model.Summary = requestDto.Summary;
            model.PaymentDate = requestDto.PaymentDate;
            model.WhetherUrgent = requestDto.WhetherUrgent;
            model.PayChannel = requestDto.PayChannel;
            // 获取配置
            var config = await _configService.GetView(f => f.ID == model.BankCompanyConfigID);
            model.PayAccountNo = config.MainAccountNo;
            model.PayOrgName = config.MainAccountOrgName;
            model.PayOrgCode = config.MainAccountOrgCode;

            // 获取账户余额
            T0006ParamSetItem accountReq = new T0006ParamSetItem()
            {
                AcctNo = model.PayAccountNo,
                CurrencyCode = model.PayCurrencyType,
                EndDate = DateTime.Today.ToString("yyyy-MM-dd"),
                StartDate = DateTime.Today.ToString("yyyy-MM-dd"),
            };
            var accountRes = await _czyService.QueryAccountBalanceInfo(accountReq);
            var accountBodyData = accountRes.Cmscloud.Body.Data;

            if (accountBodyData.ResultCode == "0000")
            {
                var accountInfo = accountBodyData.ResultSet.FirstOrDefault();
                var usableBalance = Convert.ToDecimal(accountInfo.UsableBalance);
                if (usableBalance < model.TotalAmount)
                {
                    await _bankTransactionService.UpdateNav(model);
                    throw new ErrorCodeException(-1, $"当前可用余额：{usableBalance}元，小于代发金额，无法代发。");
                }
            }
            else
            {
                await _bankTransactionService.UpdateNav(model);
                throw new ErrorCodeException(-1, accountBodyData.ResultMsg);
            }




            // 
            model.Status = Model.Enums.BankTransactionStatusEnumType.已推送待查询;
            await _bankTransactionService.UpdateNav(model);
            T0002ParamSetItem req = new T0002ParamSetItem()
            {
         
                BillsPayOrderNo = model.BillsPayOrderNo,
                PayType = "1",// 工资代发-批量对私
                PaymentDate = (model.PaymentDate ?? DateTime.Today).ToString("yyyy-MM-dd"),
                BusinessTypeCode = null,
                BusinessTypeName = model.BusinessTypeName,
                PayOrgCode = model.PayOrgCode,
                PayOrgName = model.PayOrgName,
                PayAccountNo = model.PayAccountNo,//付款方银行账号
                PayChannel = ((int)model.PayChannel).ToString(),
                PayAmount = model.TotalAmount.ToString(),
                PayCurrencyName = model.PayCurrencyName,
                PayCurrencyType = model.PayCurrencyType,
                PayUse = model.Summary,
                Purpose = model.Purpose,
                Remark = model.Remark,
                WhetherUrgent = model.WhetherUrgent ? "1" : "2",
                PaymentRec = model.Records.Select(f => new PaymentRec()
                {

                    ReceiveAccountName = f.ReceiveAccountName,
                    ReceiveAccountNo = f.ReceiveAccountNo,
                    ReceiveNature = "2",
                    ReceiveCurrencyCode = f.ReceiveCurrencyCode ?? "CNY",
                    ReceiveCurrencyName = f.ReceiveCurrencyName ?? "",
                    ReceiveBranchName = f.ReceiveBranchName,
                }).First()
            };
            var transRes = await _czyService.TransSingle(req);

            var bodyData = transRes.Cmscloud.Body.Data;
            if (bodyData.ResultCode == "0000")
            {
                // 推送成功
                model.PushTime = DateTime.Now;
                model.Status = Model.Enums.BankTransactionStatusEnumType.已推送待查询;
            }
            else if (bodyData.ResultCode == "-1")
            {
                model.Status = Model.Enums.BankTransactionStatusEnumType.推送失败;
            }
            else
            {
                model.PushTime = DateTime.Now;
                model.Status = Model.Enums.BankTransactionStatusEnumType.发放失败;
            }
            model.PushResultCode = bodyData.ResultCode;
            model.PushResultMsg = bodyData.ResultMsg;
            await _bankTransactionService.UpdateNav(model);
            return Ok(requestDto);
        }


        [CzyTransactionFilter]
        [HttpPost]
        public async Task<IActionResult> BatchTransaction(BankRequestDto requestDto)
        {
            var model = await _bankTransactionService.GetNav(f => f.ID == requestDto.TransactionID);
            if (model.Status != Model.Enums.BankTransactionStatusEnumType.待推送)
            {
                throw new ErrorCodeException(-1, $"非【待推送】状态无法执行此操作");
            }
            model.BankCompanyConfigID = requestDto.ConfigID;
            model.Remark = requestDto.Remark;
            model.Purpose = requestDto.Purpose;
            model.Summary = requestDto.Summary;
            model.PaymentDate = requestDto.PaymentDate;
            model.WhetherUrgent = requestDto.WhetherUrgent;
            model.PayChannel = requestDto.PayChannel;
            // 获取配置
            var config = await _configService.GetView(f => f.ID == model.BankCompanyConfigID);
            model.PayAccountNo = config.MainAccountNo;
            model.PayOrgName = config.MainAccountOrgName;
            model.PayOrgCode = config.MainAccountOrgCode;

            // 获取账户余额
            T0006ParamSetItem accountReq = new T0006ParamSetItem()
            {
                AcctNo = model.PayAccountNo,
                CurrencyCode = model.PayCurrencyType,
                EndDate = DateTime.Today.ToString("yyyy-MM-dd"),
                StartDate = DateTime.Today.ToString("yyyy-MM-dd"),
            };
            var accountRes = await _czyService.QueryAccountBalanceInfo(accountReq);
            var accountBodyData = accountRes.Cmscloud.Body.Data;

            if (accountBodyData.ResultCode == "0000")
            {
                var accountInfo = accountBodyData.ResultSet.FirstOrDefault();
                var usableBalance = Convert.ToDecimal(accountInfo.UsableBalance);
                if (usableBalance < model.TotalAmount)
                {
                    await _bankTransactionService.UpdateNav(model);
                    throw new ErrorCodeException(-1, $"当前可用余额：{usableBalance}元，小于代发金额，无法代发。");
                }
            }
            else
            {
                await _bankTransactionService.UpdateNav(model);
                throw new ErrorCodeException(-1, accountBodyData.ResultMsg);
            }




            // 
            model.Status = Model.Enums.BankTransactionStatusEnumType.已推送待查询;
            await _bankTransactionService.UpdateNav(model);
            T0018ParamSetItem req = new T0018ParamSetItem()
            {
                SerialNo = model.SerialNo,
                BillsPayOrderNo = model.BillsPayOrderNo,
                PayType = "24",// 工资代发-批量对私
                PaymentDate = (model.PaymentDate ?? DateTime.Today).ToString("yyyy-MM-dd"),
                BusinessTypeCode = null,
                BusinessTypeName = model.BusinessTypeName,
                PayOrgCode = model.PayOrgCode,
                PayOrgName = model.PayOrgName,
                PayAccountNo = model.PayAccountNo,//付款方银行账号
                PayChannel = ((int)model.PayChannel).ToString(),
                 SumPayAmount = model.TotalAmount.ToString(),
                PayCurrencyName = model.PayCurrencyName,
                PayCurrencyType = model.PayCurrencyType,
                PayUse = model.Summary,
                 
                Remark = model.Remark,
                WhetherUrgent = model.WhetherUrgent ? "1" : "2",
                PaymentRecList = model.Records.Select(f => new T0018PaymentRec()
                {
                    ReceiveAccountName = f.ReceiveAccountName,
                    ReceiveAccountNo = f.ReceiveAccountNo,
                    ReceiveNature = "2",
                    ReceiveCurrencyCode = f.ReceiveCurrencyCode ?? "CNY",
                    ReceiveCurrencyName = f.ReceiveCurrencyName ?? "",
                    ReceiveBranchName = f.ReceiveBranchName,
                    BillsPayOrderNo = f.BillsPayOrderNo,
                    Postscript = model.Purpose,
                    PrivateAmount = f.PrivateAmount.ToString(),
                    Summary = model.Summary
                }).ToList()
            };
            var transRes = await _czyService.TransBatch(req);

            var bodyData = transRes.Cmscloud.Body.Data;
            if (bodyData.ResultCode == "0000")
            {
                // 推送成功
                model.PushTime = DateTime.Now;
                model.Status = Model.Enums.BankTransactionStatusEnumType.已推送待查询;
            }
            else if (bodyData.ResultCode == "-1")
            {
                model.Status = Model.Enums.BankTransactionStatusEnumType.推送失败;
            }
            else
            {
                model.PushTime = DateTime.Now;
                model.Status = Model.Enums.BankTransactionStatusEnumType.发放失败;
            }
            model.PushResultCode = bodyData.ResultCode;
            model.PushResultMsg = bodyData.ResultMsg;
            await _bankTransactionService.UpdateNav(model);
            return Ok(requestDto);
        }
        #endregion
        #region 通知
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ReceiveTransactionNotice()
        {
            var str = new StreamReader(Request.Body).ReadToEnd();
            Common.StaticLog.Info($"财资云通知：{str}");

            var noticeModel = Newtonsoft.Json.JsonConvert.DeserializeObject<CzyCmscloud<TransactionNoticeCommonRequest>>(str);
         
            var head = noticeModel.Cmscloud.Head;
          
            if (head.MesgNo == "T0003")
            {
                var bodyData = Newtonsoft.Json.JsonConvert.DeserializeObject<T0003Notice>(noticeModel.Cmscloud.Body.Data);
                var transModel = await _bankTransactionService.GetNav(f => f.BillsPayOrderNo == bodyData.BillsPayOrderNo);
                transModel.PayStatus = bodyData.PayStatus;
                transModel.PayStatusName = bodyData.PayStatusName;
                transModel.PayAmount = bodyData.PayAmount;
                transModel.RelPaymentDate = bodyData.RelPaymentDate;
                transModel.RelBillsCode = bodyData.RelBillsCode;
                transModel.FailReason = bodyData.FailReasons;
                //transModel.RefundDate = bodyData.RefundDate;
                //transModel.RefundAmount = bodyData.RefundAmount;
                foreach (var item in bodyData.BatchList)
                {
                    //var record = transModel.Records.FirstOrDefault(f => f.BillsPayOrderNo == item.);
                    //if (record != null)
                    //{
                    //    record.RecordPayStatus = bodyData.PayStatus;
                    //    record.RecordPayStatusName = bodyData.PayStatusName;
                    //    record.RecordPayAmount = bodyData.PayAmount;
                    //    record.RecordRelPaymentDate = bodyData.RelPaymentDate;
                    //    record.RecordRelBillsCode = bodyData.RelBillsCode;
                    //    record.RecordFailReasons = bodyData.FailReasons;
                    //    record.RecordRefundDate = bodyData.RefundDate;
                    //    record.RecordRefundAmount = bodyData.RefundAmount;
                    //}
                }
                await _bankTransactionService.UpdateNav(transModel);
            }

            return Ok(str);
        }
        #endregion

       
    }
}
