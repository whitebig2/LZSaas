//using LZ.Core;
//using LZ.Core.Interface;
//using LZ.Core.Services;
//using LZ.Finance.Service.Interface;
//using LZ.Finance.Service.Services;
//using LZ.Model;
//using LZ.Model.Dtos;
//using LZ.Model.Dtos.BankDo;
//using LZ.Model.Enums;
//using LZ.Model.Exceptions;
//using LZ.Model.ModelExtensions;
//using LZ.Model.PagedModel;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.OpenApi.Extensions;
//using Newtonsoft.Json;
//using NPOI.OpenXmlFormats.Spreadsheet;
//using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
//using SqlSugar;
//using SqlSugar.IOC;

//namespace LZ.Finance.API.Controllers
//{
//    [Route("api/[controller]/[action]")]
//    [ApiController]
//    [AllowAnonymous]
//    public class SalaryTransactionController : ApiControllerBase
//    {

//        private readonly ISalaryCustomerService _salaryCustomerService;
//        private readonly ISalaryTransactionService _service;
//        private readonly ISalaryRecordService  _salaryRecordService;
//        private readonly IHttpService  _httpService;

//        public SalaryTransactionController(ISalaryRecordService salaryRecordService, ISalaryTransactionService service, IHttpService httpService, ISalaryCustomerService salaryCustomerService)
//        {
//            _salaryRecordService = salaryRecordService;
//            _service = service;
//            _httpService = httpService;
//            _salaryCustomerService = salaryCustomerService;
//        }

//        [HttpGet]
//        public async Task<IActionResult> PageList(string customerName, string signUnitName,SalaryTransactionStatusEnumType? status , [FromQuery] PagedModelBase query)
//        {
//            Console.WriteLine(Guid.Empty);
//            var exp = Expressionable.Create<Model.Live_SalaryTransaction>();
//            exp.AndIF(CurrentUser.PlatformType != PlatformType.Admin && CurrentUser.PlatformType != PlatformType.Api, f => f.CompanyID == CurrentUser.CompanyID);
//            exp.AndIF(!string.IsNullOrWhiteSpace(customerName), f => f.CustomerName.Contains(customerName));
//            exp.AndIF(!string.IsNullOrWhiteSpace(signUnitName), f => f.SignUnitName.Contains(signUnitName));
//            exp.AndIF(status.HasValue, f => f.Status==status.Value);
//            var models = await _service.PageList(exp.ToExpression(), query);
//            return Ok(new { items = models, count = query.Count });
//        }

//        [HttpGet]
//        public async Task<IActionResult> PageRecordList(Guid? id,string name,string cardID, TransactionStatusEnumType? status,[FromQuery] PagedModelBase query)
//        {
//            var exp = Expressionable.Create<Live_SalaryTransactionRecord>();
//            exp.AndIF(id.HasValue && id != Guid.Empty, f => f.SalaryTransactionID == id.Value);
//            exp.AndIF(!string.IsNullOrWhiteSpace(name), f => f.Name.Contains(name));
//            exp.AndIF(!string.IsNullOrWhiteSpace(cardID), f => f.CardID.Contains(cardID));
//            var models = await _service.PageRecordList(exp.ToExpression(), query);
//            return Ok(new { items = models, count = query.Count });
//        }

//        [HttpPost]
//        public async Task<IActionResult> Delete([FromForm] Guid id)
//        {
//            await _service.DeleteNav(id);
//            return Ok();
//        }


//        [HttpPost]
//        public async Task<IActionResult> TransactionResultNotice(BankTransactionResultNoticeDto dto)
//        {
//            try
//            {
//                List<TransactionStatusEnumType> status = new List<TransactionStatusEnumType>() { TransactionStatusEnumType.作废, TransactionStatusEnumType.已发放, TransactionStatusEnumType.发放失败 };
//                if (!status.Contains(dto.TransStatus))
//                {
//                    throw new ErrorCodeException(-1, $"不接受此通知类型：【{dto.TransStatus.ToString()}】");
//                }

//                if (!dto.Records.Any( f=>status.Contains(f.RecordTransStatus)))
//                {
//                    throw new ErrorCodeException(-1, $"交易记录存在未知的交易类型");
//                }
//                #region 推送记录状态同步
//                var modelNav = await _service.GetNav(f => f.ID == Guid.Parse(dto.TransactionID));


//                // 只有待核对支付结果才能处理数据，否则 直接返回成功响应，防止回调定时任务重复执行
//                if (modelNav.Status != SalaryTransactionStatusEnumType.待核对支付结果)
//                {
//                    return Ok();
//                }

//                // 获取企业工资
//                var salaryNav = await _salaryCustomerService.GetTransNav(f => f.ID == Guid.Parse(modelNav.BusinessID));

//                if (salaryNav.Status != SalaryStatusEnumType.PendingSend)
//                {
//                    return Ok();
//                }

//                modelNav.TransMsg = dto.TransMsg;
//                modelNav.TransTime = dto.TransTime;

//                if (dto.TransStatus == TransactionStatusEnumType.作废)
//                {
//                    modelNav.Status = SalaryTransactionStatusEnumType.作废;


//                    foreach (var item in modelNav.Records)
//                    {
//                        item.TransStatus = TransactionStatusEnumType.作废;
//                    }
//                }
//                else
//                {
//                    foreach (var item in dto.Records)
//                    {
//                        if (dto.Records.Any(f => f.RecordTransStatus == TransactionStatusEnumType.已发放))
//                        {
//                            modelNav.Status = SalaryTransactionStatusEnumType.支付成功;
//                        }
//                        else
//                        {
//                            modelNav.Status = SalaryTransactionStatusEnumType.支付失败;
//                        }
//                        var record = modelNav.Records.First(f => f.RecordBusinessID == item.RecordBusinessID);
//                        record.TransTime = item.RecordTransTime;
//                        record.TransStatus = item.RecordTransStatus;
//                        record.TransMsg = item.RecordTransMsg;
//                    }
//                }

//                #endregion

//                #region 企业工资状态同步
              

//                salaryNav.SendTime = modelNav.TransTime;
//                salaryNav.TransMsg = modelNav.TransMsg;
//                salaryNav.TransStatus = modelNav.Status.ToString();
//                salaryNav.TransStatusName = modelNav.Status.ToString();
           
//                if (modelNav.Status == SalaryTransactionStatusEnumType.支付成功)
//                {
//                    salaryNav.Status = SalaryStatusEnumType.SucessSend;
//                    foreach (var item in modelNav.Records)
//                    {
//                        var salaryRecord = salaryNav.Records.First(f => f.ID.ToString() == item.RecordBusinessID);
//                        if (item.TransStatus == TransactionStatusEnumType.已发放)
//                        {
//                            salaryRecord.Status = SalaryRecordSendStatusEnumType.已发放;
//                            salaryRecord.TransStatus = item.TransStatus.ToString();
//                            salaryRecord.TransStatusName = item.TransStatus.ToString();
//                            salaryRecord.TransMsg = item.TransMsg;
//                            salaryRecord.SendTime = item.TransTime;
//                        }
//                        else if (item.TransStatus == TransactionStatusEnumType.发放失败)
//                        {
//                            salaryRecord.Status = SalaryRecordSendStatusEnumType.发放异常;
//                            salaryRecord.TransStatus = item.TransStatus.ToString();
//                            salaryRecord.TransStatusName = item.TransStatus.ToString();
//                            salaryRecord.TransMsg = item.TransMsg;
//                            salaryRecord.SendTime = item.TransTime;
//                        }
                     
//                    }
//                }
//                else if (modelNav.Status == SalaryTransactionStatusEnumType.支付失败 || modelNav.Status == SalaryTransactionStatusEnumType.作废)
//                {

//                    salaryNav.Status = SalaryStatusEnumType.FailSend;

//                    foreach (var item in modelNav.Records)
//                    {
//                        var salaryRecord = salaryNav.Records.First(f => f.ID.ToString() == item.RecordBusinessID);
//                        salaryRecord.Status = SalaryRecordSendStatusEnumType.发放异常;
//                        salaryRecord.TransStatus = item.TransStatus.ToString();
//                        salaryRecord.TransStatusName = item.TransStatus.ToString();
//                        salaryRecord.TransMsg = item.TransMsg;
//                        salaryRecord.SendTime = item.TransTime;
//                    }
//                }

//                #endregion
//                DbScoped.SugarScope.BeginTran();
//                if (salaryNav.Status == SalaryStatusEnumType.FailSend)
//                {
//                    await _salaryCustomerService.UpdateTransNav(salaryNav);
//                }
//                else
//                {
//                    await _salaryCustomerService.BatchSendSuccess(salaryNav);
//                }
//                await _service.UpdateNav(modelNav);
//                DbScoped.SugarScope.CommitTran();
//                return Ok();
//            }
//            catch (Exception ex)
//            {
//                throw new ErrorCodeException(-1, ex.Message);
//            }
//        }

    


//        /// <summary>
//        ///  作废
//        ///  取消发放
//        /// </summary>
//        /// <param name="id"></param>
//        /// <returns></returns>
//        [HttpPut]
//        public async Task<IActionResult> SetCancel([FromForm] Guid id)
//        {
//            var salaryTransaction = await _service.GetNav(f => f.ID == id);

//            List<SalaryTransactionStatusEnumType> status = new List<SalaryTransactionStatusEnumType>() { SalaryTransactionStatusEnumType.待核对支付结果, SalaryTransactionStatusEnumType.支付成功, SalaryTransactionStatusEnumType.支付失败 };

//            if (status.Contains(salaryTransaction.Status))
//            {
//                throw new ErrorCodeException(-1, "此状态无法执行此操作！");
//            }

//            // 已推送过则判断是否推送成功
//            if (salaryTransaction.PushCount > 0)
//            {
//                var res = await _httpService.GetAsync<Live_BankTransaction>(ServiceNames.BankService, $"/api/BankTransaction/GeByTransactionID?transactionID={id.ToString()}");
//                if (res != null)
//                {
//                    throw new ErrorCodeException(-1, "检测到已推送记录，无法执行此操作！");
//                }
//            }
//            salaryTransaction.Status = SalaryTransactionStatusEnumType.作废;
//            DbScoped.Sugar.BeginTran();
//            await _service.UpdateNav(salaryTransaction);
//            // 将企业工资重置倒 待发放状态
//            await _salaryCustomerService.RollbackToPendingSend(Guid.Parse(salaryTransaction.BusinessID));
//            DbScoped.Sugar.CommitTran();
//            return Ok();
//        }


//        #region MyRegion

//        /// <summary>
//        ///  推送到财资云
//        /// </summary>
//        /// <param name="id"></param>
//        /// <returns></returns>
//        /// <exception cref="ErrorCodeException"></exception>
//        [HttpPut]
//        public async Task<IActionResult> PushToBank([FromForm] Guid id)
//        {
//            var salaryTransaction = await _service.GetNav(f => f.ID == id);


//            if (salaryTransaction.Status != SalaryTransactionStatusEnumType.待支付)
//            {
//                throw new ErrorCodeException(-1, "非【待支付】状态，无法执行此操作！");
//            }
//            #region 推送财资云等待发放
//            var bankTransaction = await _httpService.GetAsync<Live_BankTransaction>(ServiceNames.BankService, $"/api/BankTransaction/GeByTransactionID?transactionID={id.ToString()}");
//            if (bankTransaction != null)
//            {
//                throw new ErrorCodeException(-1, "检测到已推送记录，无法执行此操作！");
//            }
//            BankTransactionApplyDto appDto = new BankTransactionApplyDto()
//            {
//                BusinessID = salaryTransaction.BusinessID,
//                TransactionID = salaryTransaction.ID.ToString(),
//                CustomerName = salaryTransaction.CustomerName,
//                CustomerNo = salaryTransaction.CustomerNo,
//                PayDate = DateTime.Today,
//                Summary = salaryTransaction.Summary ?? "工资代发",
//                Purpose = salaryTransaction.Purpose,
//                Remark = salaryTransaction.Remark,
//                SignUnitName = salaryTransaction.SignUnitName,
//                SignUnitNo = salaryTransaction.SignUnitNo,
//                TotalAmount = salaryTransaction.TotalAmount,
//                TotalCount = salaryTransaction.TotalCount,
//            };
//            appDto.Records = salaryTransaction.Records.Select(f => new BankTransactionRecordApplyDto()
//            {
//                Bank = f.Bank,
//                BankNo = f.BankNo,
//                CardID = f.CardID,
//                Name = f.Name,
//                RecordBusinessID = f.RecordBusinessID,
//                RecordTransactionID = f.ID.ToString(),
//                TransAmount = f.TransAmount
//            }).ToList();
//            salaryTransaction.PushCount += 1;
//            salaryTransaction.PushTime = DateTime.Now;
//            string json = JsonConvert.SerializeObject(new List<BankTransactionApplyDto> { appDto });
//            var res = await _httpService.PostAsync<string>(ServiceNames.BankService, "/api/BankTransaction/PayrollBatchApply", json);
//            salaryTransaction.TransMsg = string.Empty;
//            if (res != null)
//            {
//                if (res.Code == 200)
//                {
//                    salaryTransaction.Status = SalaryTransactionStatusEnumType.待核对支付结果;
//                    salaryTransaction.TransMsg = string.Empty;
//                }
//                else
//                {
//                    salaryTransaction.Status = SalaryTransactionStatusEnumType.支付失败;
//                    salaryTransaction.TransMsg = res.Message;
//                }
//                await _service.Update(salaryTransaction);
//            }
//            else
//            {
//                salaryTransaction.TransMsg = "未能获取到正确的响应信息，请稍后再试！";
//                await _service.Update(salaryTransaction);
//                throw new ErrorCodeException(-1, salaryTransaction.TransMsg);
//            }
//            #endregion
//            return Ok();
//        }
//        #endregion

//    }
//}
