//using LZ.Bank.Service.Interface;
//using LZ.Bank.Service.Services;
//using LZ.Core;
//using LZ.Core.Redis;
//using LZ.Model;
//using LZ.Model.Dtos.BankDo;
//using LZ.Model.Enums;
//using LZ.Model.Exceptions;
//using LZ.Model.PagedModel;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
//using SqlSugar;

//namespace LZ.Bank.Api.Controllers
//{
//    [Route("api/[controller]/[action]")]
//    [ApiController]
//    [AllowAnonymous]
//    public class BankTransactionController : ApiControllerBase
//    {

//        private readonly IRedisService _redisService;
//        private readonly IBankTransactionService _service;
//        private readonly IBankCompanyService _companyService;
//        private readonly IBankCompanyConfigService _configServices;
//        private readonly IBankUserCompanyService _userCompanyService;
//        private readonly IBankUserService _bankUserService;
//        private readonly IBankAccountService _bankAccountService;

//        public BankTransactionController(IBankTransactionService service, IRedisService redisService, IBankCompanyService companyService, IBankCompanyConfigService configServices, IBankUserCompanyService userCompanyService, IBankUserService bankUserService, IBankAccountService bankAccountService)
//        {
//            _redisService = redisService;
//            _service = service;
//            _companyService = companyService;
//            _configServices = configServices;
//            _userCompanyService = userCompanyService;
//            _bankUserService = bankUserService;
//            _bankAccountService = bankAccountService;
//        }
//        [HttpGet]
//        public async Task<IActionResult> PageList(string customerName, string signUnitName, BankTransactionStatusEnumType? status, [FromQuery] PagedModelBase query)
//        {
//            Console.WriteLine(Guid.Empty);
//            var exp = Expressionable.Create<Model.Live_BankTransaction>();
//            //exp.AndIF(CurrentUser.PlatformType != PlatformType.Admin && CurrentUser.PlatformType != PlatformType.Api, f => f.CompanyID == CurrentUser.CompanyID);
//            exp.AndIF(!string.IsNullOrWhiteSpace(customerName), f => f.CustomerName.Contains(customerName));
//            exp.AndIF(!string.IsNullOrWhiteSpace(signUnitName), f => f.SignUnitName.Contains(signUnitName));
//            exp.AndIF(status.HasValue, f => f.Status == status.Value);
//            var models = await _service.PageList(exp.ToExpression(), query);
//            return Ok(new { items = models, count = query.Count });
//        }

//        [HttpGet]
//        public async Task<IActionResult> PageRecordList(Guid? id, string name, string bank,string bankNo, TransactionStatusEnumType? status, [FromQuery] PagedModelBase query)
//        {
//            Console.WriteLine(Guid.Empty);
//            var exp = Expressionable.Create < Model.Live_BankTransactionRecord>();
//            //exp.AndIF(CurrentUser.PlatformType != PlatformType.Admin && CurrentUser.PlatformType != PlatformType.Api, f => f.CompanyID == CurrentUser.CompanyID);
//            exp.AndIF(id.HasValue, f => f.BankTransactionID==id.Value);
//            exp.AndIF(!string.IsNullOrWhiteSpace(name), f => f.ReceiveAccountName.Contains(name));
//            exp.AndIF(!string.IsNullOrWhiteSpace(bank), f => f.ReceiveBranchName.Contains(bank));
//            exp.AndIF(!string.IsNullOrWhiteSpace(bankNo), f => f.ReceiveAccountNo.Contains(bankNo));
//            exp.AndIF(status.HasValue, f => f.RecordTransStatus == status.Value);
//            var models = await _service.PageRecordList(exp.ToExpression(), query);
//            return Ok(new { items = models, count = query.Count });
//        }

//        [HttpGet]
//        public async Task<IActionResult> Get(Guid id)
//        {
//            var model = await _service.Get(f => f.ID == id);
//            return Ok(model);
//        }
//        /// <summary>
//        ///  重置
//        /// </summary>
//        /// <param name="id"></param>
//        /// <returns></returns>
//        [HttpPut]
//        public async Task<IActionResult> SetReset([FromForm] Guid id)
//        {
//            List<BankTransactionStatusEnumType> cancelStatus = new List<BankTransactionStatusEnumType>() { BankTransactionStatusEnumType.发放失败, BankTransactionStatusEnumType.发放成功, BankTransactionStatusEnumType.作废, BankTransactionStatusEnumType.已推送待查询 };
//            var model = await _service.GetNav(id);
//            model.LockTime = null;
//            if (cancelStatus.Contains(model.Status))
//            {
//                throw new ErrorCodeException(-1, "当前状态无法作废！");
//            }
//            await _service.ResetNav(model);
//            return Ok();
//        }
//        /// <summary>
//        /// 锁定
//        /// </summary>
//        /// <param name="dto"></param>
//        /// <returns></returns>
//        /// <exception cref="ErrorCodeException"></exception>
//        [HttpPut]
//        public async Task<IActionResult> SetLock(BankTransactionSetLockDto dto)
//        {
//            var model = await _service.Get(f => f.ID == dto.ID);
//            if (model.Status != BankTransactionStatusEnumType.待锁定)
//            {
//                throw new ErrorCodeException(-1, "当前状态无法执行此操作！");
//            }
//            var config = await _configServices.GetView(f => f.ID==dto.BankCompanyConfigID && f.IsEnable && f.CompanyIsEnable && f.MainAccountIsEnable && f.IsDefault && !f.IsDelete && f.CompanyID == CurrentUser.CompanyID);
//            if (config == null)
//            {
//                throw new ErrorCodeException(-1, $"配置信息不可用，代发失败！");
//            }
//            model.LockTime = DateTime.Now;
//            model.Status = BankTransactionStatusEnumType.待推送;
//            model.BankCompanyConfigID = config.ID;
//            model.PayAccountNo = config.MainAccountNo;
//            model.PayOrgName = config.MainAccountOrgName;
//            model.PayOrgCode = config.MainAccountOrgCode;
//            model.PaymentDate = dto.PaymentDate;
//            model.Summary = dto.Summary;
//            model.Purpose = dto.Purpose;
//            model.Remark = dto.Remark;
//            await _service.Update(model);
//            return Ok();
//        }
//        /// <summary>
//        /// 作废
//        /// </summary>
//        /// <param name="id"></param>
//        /// <returns></returns>
//        [HttpPut]
//        public async Task<IActionResult> SetCancel([FromForm] Guid id)
//        {
//            List<BankTransactionStatusEnumType> cancelStatus = new List<BankTransactionStatusEnumType>() { BankTransactionStatusEnumType.发放失败, BankTransactionStatusEnumType.发放成功, BankTransactionStatusEnumType.作废, BankTransactionStatusEnumType.已推送待查询, };
//            var model = await _service.GetNav(id);
//            if (cancelStatus.Contains(model.Status))
//            {
//                throw new ErrorCodeException(-1, "当前状态无法作废！");
//            }
//            await _service.CancelNav(model);
//            // 通知最终结果
//            await _service.NoticeResult(model);

//            return Ok();
//        }


//        #region 批量代发申请
//        /// <summary>
//        /// 批量代发申请
//        /// </summary>
//        /// <param name="dtos"></param>
//        /// <returns></returns>
//        /// <exception cref="ErrorCodeException"></exception>
//        [HttpPost]
//        public async Task<IActionResult> PayrollBatchApply(List<BankTransactionApplyDto> dtos)
//        {

//            try
//            {
//                List<Live_BankTransactionExtension> modes = new List<Live_BankTransactionExtension>();
//                Live_BankTransactionExtension model = null;
//                foreach (var item in dtos)
//                {

//                   var existModel=await _service.Get(f => f.TransactionID == item.TransactionID);
//                    if (existModel!=null)
//                    {
//                        //return Ok(new BankResponseDto { Success = false, Code = -1, Msg = "重复推送已过滤！" });
//                        return Ok("记录已存在，无法重复推送");
//                    }
//                    if (item.Records.Any(f => f.TransAmount <= 0))
//                    {
//                        throw new ErrorCodeException(-1, $"工资记录实发金额必须大于0!");
//                    }
//                    var bankCompany = await _companyService.Get(f => f.Name == item.SignUnitName && f.CompanyNo == item.SignUnitNo && f.IsEnable);
//                    if (bankCompany == null)
//                    {
//                        throw new ErrorCodeException(-1, $"未匹配到代发单位：【{item.SignUnitName}（{item.SignUnitNo}）】代发失败！");
//                    }
//                    var config = await _configServices.GetView(f => f.BankCompanyID == bankCompany.ID && f.IsEnable && f.IsDefault);
//                    if (config == null)
//                    {
//                        throw new ErrorCodeException(-1, $"代发默认配置缺失：【{item.SignUnitName}（{item.SignUnitNo}）】代发失败！");
//                    }

//                    model = new Live_BankTransactionExtension()
//                    {
//                        ID = Guid.NewGuid(),
//                        BankType = BankTypeEnumType.工商银行,
//                        BillsPayOrderNo = item.TransactionID,
//                        BusinessTypeName = "批量代发",
//                        BusinessTypeCode = string.Empty,
//                        CreateTime = DateTime.Now,
//                        CustomerName = item.CustomerName,
//                        CustomerNo = item.CustomerNo,
//                        SignUnitName = item.SignUnitName,
//                        SignUnitNo = item.SignUnitNo,
//                        FailCount = 0,
//                        SucCount = 0,
//                        TotalCount = item.TotalCount,
//                        TotalAmount = item.TotalAmount,
//                        SerialNo = Guid.NewGuid().ToString(),
//                        Summary = item.Summary,
//                        Purpose = item.Purpose,
//                        Remark = item.Remark,
//                        TransMsg = string.Empty,
//                        PayCurrencyName = "CNY",
//                        PayCurrencyType = "人民币",
//                        PayChannel = CzyPayChannelEnumType.直联支付,
//                        PaymentDate = item.PayDate,
//                        PayOrgCode = item.SignUnitNo,
//                        PayOrgName = item.SignUnitName,
//                        PayType = CzyPayTypeEnumType.工资代发批量对私,
//                        Status = BankTransactionStatusEnumType.待锁定,
//                        WhetherUrgent = false,
//                        TransStatus = "",
//                        TransTime = null,
//                        CompanyID = 0,
//                        CompanyName = "幸福职",
//                        BusinessID = item.BusinessID,
//                        TransactionID = item.TransactionID,
//                        BankCompanyID = bankCompany.ID,
//                        BankCompanyConfigID = config.ID,
//                        PayAccountNo = config.MainAccountNo,
//                    };

//                    model.Records = item.Records.Select(f => new Live_BankTransactionRecord()
//                    {
//                        ID = Guid.NewGuid(),
//                        BillsPayOrderNo = f.RecordTransactionID,
//                        BankTransactionID = model.ID,
//                        PrivateAmount = f.TransAmount,
//                        ReceiveAccountName = f.Name,
//                        ReceiveAccountNo = f.BankNo,
//                        ReceiveBranchName = f.Bank,
//                        ReceiveCurrencyCode = "CNY",
//                        ReceiveCurrencyName = "人民币",
//                        ReceiveNature = string.Empty,
//                        ReceiveBranchCode = string.Empty,
//                        ReceiveAreaNameOfCity = string.Empty,
//                        ReceiveAreaNameOfProvince = string.Empty,
//                        ReceiveOrgCode = "",
//                        RecordTransMsg = string.Empty,
//                        RecordTransStatus = TransactionStatusEnumType.待发放,
//                        RecordTransTime = null,
//                        CardID = f.CardID,
//                        RecordTransactionID = f.RecordTransactionID,
//                        RecordBusinessID = f.RecordBusinessID,
                         
//                    }).ToList();
//                    model.TotalCount = model.Records.Count;
//                    model.TotalAmount = model.Records.Sum(f => f.PrivateAmount);
//                    modes.Add(model);
//                }
//                await _service.AddNav(modes);
//                return Ok("推送成功");
//            }
//            catch (Exception ex)
//            {
//                //return Ok(new BankResponseDto { Success = false, Code = -1, Msg = "操作失败：" + ex.Message });
//                throw new ErrorCodeException(-1, $"操作失败：{ex.Message}");

//            }
//        }



//        /// <summary>
//        /// 批量代发申请
//        /// </summary>
//        /// <param name="dtos"></param>
//        /// <returns></returns>
//        /// <exception cref="ErrorCodeException"></exception>
//        [HttpPut]
//        public async Task<IActionResult> PayrollTransactionResultNotice([FromForm] Guid id)
//        {
//            var model = await _service.GetNav(id);
//            if (model.IsSendNotice)
//            {
//                throw new ErrorCodeException(-1, "已通知记录，无法重复操作！");
//            }
//            bool isb = await _service.NoticeResult(model);
//            if (!isb)
//            {
//                throw new ErrorCodeException(-1, "通知失败！");
//            }
//            return Ok(isb);
//        }
//        #endregion

//        #region MyRegion
//        [HttpGet]
//        public async Task<IActionResult> GeByTransactionID(string transactionID)
//        {
//            var model = await _service.Get(f => f.TransactionID == transactionID);
//            return Ok(model);

//        }
//        #endregion

//    }
//}
