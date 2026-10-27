using LZ.Common;
using LZ.Core;
using LZ.Core.Filters;
using LZ.Core.Interface;
using LZ.Core.Redis;
using LZ.Finance.Service.Interface;
using LZ.Model;
using LZ.Model.Dtos.FinanceDo;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.ModelExtensions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;

namespace LZ.Finance.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PaymentOrderController : ApiControllerBase
    {
        private readonly IPaymentOrderService _service;
        private readonly IPaymentAccountService _paymentAccountService;
        private readonly IRedisService _redisService;
        private readonly IPaymentAdminService _paymentAdminService;
        private readonly IPaymentAdminSignUnitService _paymentAdminSignUnitService;
        private readonly IPaymentSignUnitService _paymentSignUnitService;
        private readonly IPaymentApplicationService _paymentApplicationService;
        private readonly ITotpService _totpService;
        private readonly ISalaryCustomerService _salaryCustomerService;
        private readonly ISalaryRecordService _salaryRecordService;
        private readonly IWebHostEnvironment _env;

        public PaymentOrderController(
            IPaymentOrderService service,
            IPaymentAccountService paymentAccountService,
            IRedisService redisService,
            IPaymentAdminService paymentAdminService,
            IPaymentAdminSignUnitService paymentAdminSignUnitService,
            IPaymentSignUnitService paymentSignUnitService,
            IPaymentApplicationService paymentApplicationService,
            ITotpService totpService,
            ISalaryCustomerService salaryCustomerService,
            ISalaryRecordService salaryRecordService,
            IWebHostEnvironment env)
        {
            _service = service;
            _paymentAccountService = paymentAccountService;
            _redisService = redisService;
            _paymentAdminService = paymentAdminService;
            _paymentAdminSignUnitService = paymentAdminSignUnitService;
            _paymentSignUnitService = paymentSignUnitService;
            _paymentApplicationService = paymentApplicationService;
            _totpService = totpService;
            _salaryCustomerService = salaryCustomerService;
            _salaryRecordService = salaryRecordService;
            _env = env;
        }

        [HttpGet]
        public async Task<IActionResult> PageList(string customerName, string signUnitName, string billsPayOrderNo, string businessID, PaymentOrderTransStatusEnumType? status, [FromQuery] PagedModelBase query)
        {
            var signUnitIds = await GetViewableSignUnitIds();
            var exp = Expressionable.Create<Live_PaymentOrder>();
            exp.And(f => signUnitIds.Contains(f.PaymentSignUnitID));
            exp.AndIF(CurrentUser.PlatformType != PlatformType.Admin && CurrentUser.PlatformType != PlatformType.Api, f => f.CompanyID == CurrentUser.CompanyID);
            exp.AndIF(!string.IsNullOrWhiteSpace(customerName), f => f.CustomerName.Contains(customerName));
            exp.AndIF(!string.IsNullOrWhiteSpace(signUnitName), f => f.SignUnitName.Contains(signUnitName));
            exp.AndIF(!string.IsNullOrWhiteSpace(billsPayOrderNo), f => f.BillsPayOrderNo.Contains(billsPayOrderNo));
            exp.AndIF(!string.IsNullOrWhiteSpace(businessID), f => f.BusinessID.Contains(businessID));
            exp.AndIF(status.HasValue, f => f.Status == status.Value);
            var models = await _service.PageList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }

        [HttpGet]
        public async Task<IActionResult> PageDetailList(Guid paymentOrderID, string name, string cardID, PaymentOrderTransStatusEnumType? status, [FromQuery] PagedModelBase query)
        {
            if (paymentOrderID == Guid.Empty)
            {
                throw new ErrorCodeException(-1, "支付订单不能为空");
            }

            var order = await _service.Get(f => f.ID == paymentOrderID);
            if (order == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            EnsureCompanyReadable(order.CompanyID);
            await EnsureCanView(order);

            var exp = Expressionable.Create<Live_PaymentOrderDetail>();
            exp.And(f => f.PaymentOrderID == paymentOrderID);
            exp.AndIF(!string.IsNullOrWhiteSpace(name), f => f.Name.Contains(name));
            exp.AndIF(!string.IsNullOrWhiteSpace(cardID), f => f.CardID.Contains(cardID));
            exp.AndIF(status.HasValue, f => f.Status == status.Value);
            var models = await _service.PageDetailList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }

        [HttpGet]
        public async Task<IActionResult> Get(Guid id)
        {
            var model = await _service.GetNav(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            EnsureCompanyReadable(model.CompanyID);
            await EnsureCanView(model);
            return Ok(model);
        }

        /// <summary>
        /// 根据当前订单获取可用付款账户（同公司、同签约单位、已启用）
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ListAvailableAccounts(Guid id)
        {
            var order = await _service.Get(f => f.ID == id);
            if (order == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            EnsureCompanyReadable(order.CompanyID);
            await EnsureCanView(order);
            if (order.PaymentSignUnitID == Guid.Empty)
            {
                throw new ErrorCodeException(-1, "支付订单未关联签约单位");
            }

            var accounts = await _paymentAccountService.ListView(f =>
                !f.IsDelete
                && f.IsEnable
                && f.UnitName == order.SignUnitName
                && f.UnitNo == order.SignUnitNo
                && f.CompanyID == order.CompanyID);
            return Ok(accounts ?? new List<Live_V_PaymentAccount>());
        }

        /// <summary>
        /// 根据签约单位名称、编号、公司获取可用付款账户（已启用、未删除）
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ListAvailableAccountsByUnit(string unitName, string unitNo, int companyId)
        {
            if (string.IsNullOrWhiteSpace(unitName))
            {
                throw new ErrorCodeException(-1, "签约单位名称不能为空");
            }
            if (string.IsNullOrWhiteSpace(unitNo))
            {
                throw new ErrorCodeException(-1, "签约单位编号不能为空");
            }
            if (companyId <= 0)
            {
                throw new ErrorCodeException(-1, "公司不能为空");
            }
            EnsureCompanyReadable(companyId);

            var signUnitName = unitName.Trim();
            var signUnitNo = unitNo.Trim();
            //var signUnitIds = await GetViewableSignUnitIds();
            var accounts = await _paymentAccountService.ListView(f =>
                !f.IsDelete
                && f.IsEnable
                && f.UnitName == signUnitName
                && f.UnitNo == signUnitNo
                && f.CompanyID == companyId);
            var result = (accounts ?? new List<Live_V_PaymentAccount>())
                .Where(f => f.PaymentSignUnitID != Guid.Empty )
                .ToList();
            return Ok(result);
        }

        /// <summary>
        /// 新增支付订单：按签约单位名称匹配默认付款账户
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Add(PaymentOrderAddDto dto)
        {
            if (dto == null)
            {
                throw new ErrorCodeException(-1, "支付订单不能为空");
            }
            if (CurrentUser.CompanyID <= 0)
            {
                throw new ErrorCodeException(-1, "当前用户公司不能为空");
            }
            if (string.IsNullOrWhiteSpace(dto.SigningUnitName))
            {
                throw new ErrorCodeException(-1, "签约单位不能为空");
            }
            dto.CompanyID = CurrentUser.CompanyID;

            var signUnitName = dto.SigningUnitName.Trim();
            var manageableSignUnitIds = await GetManageableSignUnitIds();
            var accounts = await _paymentAccountService.ListView(f =>
                !f.IsDelete
                && f.IsEnable
                && f.UnitName == signUnitName
                && f.CompanyID == dto.CompanyID);
            if (accounts == null || accounts.Count == 0)
            {
                throw new ErrorCodeException(-1, $"签约单位【{signUnitName}】没有可用的付款账户");
            }

            var allowedAccounts = accounts
                .Where(f => f.PaymentSignUnitID != Guid.Empty && manageableSignUnitIds.Contains(f.PaymentSignUnitID))
                .ToList();
            if (allowedAccounts.Count == 0)
            {
                throw new ErrorCodeException(-1, "无权管理此签约单位的支付订单");
            }

            var account = allowedAccounts.FirstOrDefault(f => f.IsDefault) ?? allowedAccounts.First();
            dto.PaymentAccountID = account.ID;
            try
            {
                DbScoped.SugarScope.BeginTran();
                var model = await _service.Add(dto);
                DbScoped.SugarScope.CommitTran();
                return Ok(new { paymentOrderID = model.ID, model.TotalCount, model.TotalAmount });
            }
            catch (Exception ex)
            {
                DbScoped.SugarScope.RollbackTran();
                throw new ErrorCodeException(-1, ex.Message, ex);
            }
        }

        /// <summary>
        /// 根据预览支付订单生成正式订单
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddFromPre(PrePaymentOrderDto dto)
        {
            if (dto == null)
            {
                throw new ErrorCodeException(-1, "支付订单不能为空");
            }
            if (CurrentUser.CompanyID <= 0)
            {
                throw new ErrorCodeException(-1, "当前用户公司不能为空");
            }
            if (string.IsNullOrWhiteSpace(dto.SignUnitName))
            {
                throw new ErrorCodeException(-1, "签约单位不能为空");
            }
            if (!dto.PaymentAccountID.HasValue || dto.PaymentAccountID.Value == Guid.Empty)
            {
                throw new ErrorCodeException(-1, "付款账户不能为空");
            }
            if (string.IsNullOrWhiteSpace(dto.BusinessID))
            {
                throw new ErrorCodeException(-1, "业务单号不能为空");
            }

            //dto.CompanyID = CurrentUser.CompanyID;
            EnsureCompanyReadable(dto.CompanyID);

            var cacheKey = $"PaymentOrder_AddFromPre_{dto.BusinessID}";
            var locked = await _redisService.SetAsync(cacheKey, dto.BusinessID, TimeSpan.FromSeconds(5), CSRedis.RedisExistence.Nx);
            if (!locked)
            {
                throw new ErrorCodeException(-1, "请求过快,请5秒后再操作");
            }

          

            Live_SalaryCustomerExtension salary = null;
            Live_SalaryRecord delayRecord = null;
            if (dto.DataType == TransactionDataTypeEnumType.客户工资)
            {
                if (!Guid.TryParse(dto.BusinessID, out var salaryId) || salaryId == Guid.Empty)
                {
                    throw new ErrorCodeException(-1, "客户工资记录不能为空");
                }
                salary = await _salaryCustomerService.GetTransNav(f => f.ID == salaryId);
                if (salary == null)
                {
                    throw new ErrorCodeException(-1, "你要读取的数据不存在");
                }
                if (salary.CompanyID.HasValue)
                {
                    EnsureCompanyReadable(salary.CompanyID.Value);
                }
                var salaryRecords = salary.SetPushOrder();
                dto.CustomerName = salary.CustomerName;
                dto.CustomerNo = salary.CustomerCompanyNo;
                dto.SignUnitName = salary.SigningUnitCompanyName;
                dto.Records = salaryRecords.Select(f => new PrePaymentOrderDetailDto
                {
                    BusinessID = f.ID.ToString(),
                    Name = f.Name,
                    CardID = f.IdCardNo,
                    BankName = f.Bank,
                    BankNo = f.BankNo,
                    TransAmount = f.ActualSalarySendCardAmount,
                }).ToList();
            }
            else if (dto.DataType == TransactionDataTypeEnumType.暂缓异常工资)
            {
                if (!Guid.TryParse(dto.BusinessID, out var recordId) || recordId == Guid.Empty)
                {
                    throw new ErrorCodeException(-1, "工资记录不能为空");
                }
                delayRecord = await _salaryRecordService.Get(f => f.ID == recordId);
                if (delayRecord == null)
                {
                    throw new ErrorCodeException(-1, "你要读取的数据不存在");
                }
                if (delayRecord.IsCashSend)
                {
                    throw new ErrorCodeException(-1, "现金发放无法生成支付订单");
                }
                if (delayRecord.IsBatchSend)
                {
                    throw new ErrorCodeException(-1, "批量发放无法生成支付订单");
                }
                if (delayRecord.ActualSalarySendCardAmount <= 0)
                {
                    throw new ErrorCodeException(-1, "实发金额必须大于0");
                }
                if (delayRecord.Status != SalaryRecordSendStatusEnumType.待发放)
                {
                    throw new ErrorCodeException(-1, "非【待发放】记录无法执行此操作");
                }
                if (delayRecord.IsPushToOrder)
                {
                    throw new ErrorCodeException(-1, "该记录已推送到支付订单，无法重复推送");
                }
                if (delayRecord.IsSendSuccess)
                {
                    throw new ErrorCodeException(-1, "该记录已发放成功，无法重复推送");
                }
                if (delayRecord.Name != delayRecord.BankUserName)
                {
                    throw new ErrorCodeException(-1, "银行卡所属人与员工姓名不一致");
                }
                if (string.IsNullOrWhiteSpace(delayRecord.Name)
                    || string.IsNullOrWhiteSpace(delayRecord.IdCardNo)
                    || string.IsNullOrWhiteSpace(delayRecord.Bank)
                    || string.IsNullOrWhiteSpace(delayRecord.BankNo))
                {
                    throw new ErrorCodeException(-1, "银行卡或人员信息不完整");
                }

                var recordView = await _salaryRecordService.GetView(f => f.ID == recordId);
                if (recordView == null)
                {
                    throw new ErrorCodeException(-1, "你要读取的数据不存在");
                }
                if (string.IsNullOrWhiteSpace(recordView.SigningUnitCompanyName))
                {
                    throw new ErrorCodeException(-1, "签约单位不能为空");
                }

                dto.CustomerName = recordView.CustomerName;
                dto.CustomerNo = recordView.CustomerCompanyNo;
                dto.SignUnitName = recordView.SigningUnitCompanyName;
                dto.Records = new List<PrePaymentOrderDetailDto>
                {
                    new PrePaymentOrderDetailDto
                    {
                        BusinessID = delayRecord.ID.ToString(),
                        Name = delayRecord.Name,
                        CardID = delayRecord.IdCardNo,
                        BankName = delayRecord.Bank,
                        BankNo = delayRecord.BankNo,
                        TransAmount = delayRecord.ActualSalarySendCardAmount,
                    }
                };
            }

            try
            {
                DbScoped.SugarScope.BeginTran();
                var model = await _service.AddFromPre(dto);
                if (salary != null)
                {
                    var updateSuc = await _salaryCustomerService.UpdateTransNav(salary);
                    if (!updateSuc)
                    {
                        throw new ErrorCodeException(-1, "客户工资记录更新失败");
                    }
                }
                if (delayRecord != null)
                {
                    delayRecord.IsPushToOrder = true;
                    delayRecord.IsSingleSend = true;
                    var updateSuc = await _salaryRecordService.Update(delayRecord);
                    if (!updateSuc)
                    {
                        throw new ErrorCodeException(-1, "工资记录更新失败");
                    }
                }
                DbScoped.SugarScope.CommitTran();
                return Ok(new { paymentOrderID = model.ID, model.TotalCount, model.TotalAmount });
            }
            catch (Exception ex)
            {
                DbScoped.SugarScope.RollbackTran();
                throw new ErrorCodeException(-1, ex.Message, ex);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update(PaymentOrderSaveDto dto)
        {
            if (dto == null || dto.ID == Guid.Empty)
            {
                throw new ErrorCodeException(-1, "支付订单不能为空");
            }

            var exist = await _service.GetNav(f => f.ID == dto.ID);
            if (exist == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            EnsureCompanyReadable(exist.CompanyID);
            var manageableSignUnitIds = await GetManageableSignUnitIds();
            if (exist.PaymentSignUnitID == Guid.Empty)
            {
                throw new ErrorCodeException(-1, "支付订单未关联签约单位");
            }
            if (!manageableSignUnitIds.Contains(exist.PaymentSignUnitID))
            {
                throw new ErrorCodeException(-1, "无权管理此签约单位的支付订单");
            }
            if (exist.Status != PaymentOrderTransStatusEnumType.待支付)
            {
                throw new ErrorCodeException(-1, "非【待支付】状态，无法修改！");
            }

            exist.PayChannel = dto.PayChannel;
            exist.PayType = dto.PayType;
            exist.PayUse = dto.PayUse;
            exist.WhetherUrgent = dto.WhetherUrgent;
            exist.Purpose = dto.Purpose;
            exist.Remark = dto.Remark;
            exist.PaymentDate = dto.PaymentDate;
            exist.UpdateTime = DateTime.Now;
            exist.Timestamp = dto.Timestamp;

            if (!dto.PaymentAccountID.HasValue || dto.PaymentAccountID.Value == Guid.Empty)
            {
                throw new ErrorCodeException(-1, "付款账户不能为空");
            }

            var accounts = await _paymentAccountService.ListView(f => f.ID == dto.PaymentAccountID.Value);
            var account = accounts?.FirstOrDefault();
            if (account == null)
            {
                throw new ErrorCodeException(-1, "付款账户不存在");
            }
            if (!account.IsEnable)
            {
                throw new ErrorCodeException(-1, "付款账户未启用");
            }
            if (!account.CompanyID.HasValue || account.CompanyID.Value != exist.CompanyID)
            {
                throw new ErrorCodeException(-1, "付款账户不属于当前订单公司");
            }
            if (account.PaymentApplicationID == Guid.Empty)
            {
                throw new ErrorCodeException(-1, "付款账户未关联支付应用");
            }
            if (account.PaymentSignUnitID == Guid.Empty)
            {
                throw new ErrorCodeException(-1, "付款账户未关联签约单位");
            }
            if (!IsSameSignUnitName(account.UnitName, exist.SignUnitName))
            {
                throw new ErrorCodeException(-1, "付款账户签约单位与订单不一致");
            }
            if (string.IsNullOrWhiteSpace(account.UnitNo))
            {
                throw new ErrorCodeException(-1, "付款账户未配置签约单位编号");
            }
            if (string.IsNullOrWhiteSpace(account.UseOrgName))
            {
                throw new ErrorCodeException(-1, "付款账户未配置付款机构名称");
            }
            if (string.IsNullOrWhiteSpace(account.AppName))
            {
                throw new ErrorCodeException(-1, "付款账户未配置支付应用信息");
            }
            var application = await _paymentApplicationService.Get(f => f.ID == account.PaymentApplicationID && !f.IsDelete);
            if (application == null)
            {
                throw new ErrorCodeException(-1, "支付应用不存在");
            }
            if (string.IsNullOrWhiteSpace(account.AppID))
            {
                throw new ErrorCodeException(-1, "付款账户未配置支付应用编号");
            }

            exist.PaymentAccountID = account.ID;
            exist.PayAccountNo = account.AccountNo ?? string.Empty;
            exist.PayOrgName = account.UseOrgName.Trim();
            exist.PayOrgCode = account.UseOrgCode ?? string.Empty;
            exist.BusinessTypeName = account.BusinessTypeName ?? string.Empty;
            exist.PaymentApplicationID = application.ID;
            exist.AppName = account.AppName;
            exist.AppID = account.AppID;
            exist.PaymentSignUnitID = account.PaymentSignUnitID;
            exist.SignUnitNo = account.UnitNo;

            if (!manageableSignUnitIds.Contains(exist.PaymentSignUnitID))
            {
                throw new ErrorCodeException(-1, "无权管理此签约单位的支付订单");
            }
            await _service.Update(exist);
            return Ok();
        }

        //[HttpDelete]
        //public async Task<IActionResult> Delete([FromForm] Guid id)
        //{
        //    var exist = await _service.Get(f => f.ID == id);
        //    if (exist == null)
        //    {
        //        throw new ErrorCodeException(-1, "你要读取的数据不存在");
        //    }
        //    EnsureCompanyReadable(exist.CompanyID);
        //    await EnsureCanManage(exist);
        //    try
        //    {
        //        DbScoped.SugarScope.BeginTran();
        //        await _service.DeleteNav(id);
        //        DbScoped.SugarScope.CommitTran();
        //    }
        //    catch (Exception ex)
        //    {
        //        DbScoped.SugarScope.RollbackTran();
        //        throw new ErrorCodeException(-1, ex.Message, ex);
        //    }
        //    return Ok();
        //}

        /// <summary>
        /// 支付交易：提交后推送到银行接口
        /// </summary>
        [AesRasRequestHandleFilter]
        [HttpPost]
        public async Task<IActionResult> SubmitPay(PaymentOrderSubmitPayDto dto)
        {
            ValidateSubmitPayDto(dto);
            var cacheKey = $"PaymentOrder_SubmitPay_{dto.ID}";
            var isb = await _redisService.SetAsync(cacheKey, dto.ID.ToString(), TimeSpan.FromSeconds(60), CSRedis.RedisExistence.Nx);
            if (!isb)
            {
                throw new ErrorCodeException(-1, "请求过快,请稍后再操作");
            }

            var model = await _service.GetNav(f => f.ID == dto.ID);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            EnsureCompanyReadable(model.CompanyID);
            if (!model.PaymentAccountID.HasValue || model.PaymentAccountID.Value == Guid.Empty)
            {
                throw new ErrorCodeException(-1, "付款账户不能为空");
            }
            if (model.PaymentApplicationID == Guid.Empty)
            {
                throw new ErrorCodeException(-1, "支付订单未关联支付应用");
            }
            await EnsurePaymentAccountReadyToPay(model);
            var admin = await GetRequiredPaymentAdmin(model.PaymentApplicationID, model.CompanyID);
            await EnsureCanPay(admin, model);
            await ValidatePaymentTotp(admin, dto.TotpCode, model.PaymentApplicationID);

            model.Timestamp = dto.Timestamp;

            await _service.SubmitPay(model);
            return Ok();
        }

        /// <summary>
        /// 订单作废（仅待支付可作废；客户工资来源会回滚推送标记）
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> SetCancel([FromForm] Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ErrorCodeException(-1, "支付订单不能为空");
            }

            var cacheKey = $"PaymentOrder_SetCancel_{id}";
            var isb = await _redisService.SetAsync(cacheKey, id.ToString(), TimeSpan.FromSeconds(5), CSRedis.RedisExistence.Nx);
            if (!isb)
            {
                throw new ErrorCodeException(-1, "请求过快,请5秒后再操作");
            }

            var model = await _service.GetNav(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            EnsureCompanyReadable(model.CompanyID);
            await EnsureCanManage(model);

            try
            {
                DbScoped.SugarScope.BeginTran();
                await _service.Cancel(model);
                DbScoped.SugarScope.CommitTran();
            }
            catch (Exception ex)
            {
                DbScoped.SugarScope.RollbackTran();
                throw new ErrorCodeException(-1, ex.Message, ex);
            }
            return Ok();
        }

        private static void ValidateSubmitPayDto(PaymentOrderSubmitPayDto dto)
        {
            if (dto == null || dto.ID == Guid.Empty)
            {
                throw new ErrorCodeException(-1, "支付订单不能为空");
            }
            if (dto.Timestamp == null || dto.Timestamp.Length == 0)
            {
                throw new ErrorCodeException(-1, "版本数据不能为空");
            }
            if (string.IsNullOrWhiteSpace(dto.TotpCode))
            {
                throw new ErrorCodeException(-1, "支付令牌不能为空");
            }
        }

        private async Task<List<Guid>> GetViewableSignUnitIds()
        {
            var admins = await GetCurrentCompanyPaymentAdmins();
            if (admins.Count == 0)
            {
                throw new ErrorCodeException(-1, "无权查看支付订单");
            }

            var adminIds = admins.Select(f => f.ID).ToList();
            var permissions = await _paymentAdminSignUnitService.List(f =>
                adminIds.Contains(f.PaymentAdminID) && f.IsCanView);
            if (permissions == null || permissions.Count == 0)
            {
                throw new ErrorCodeException(-1, "无权查看支付订单");
            }

            return permissions.Select(f => f.PaymentSignUnitID).Distinct().ToList();
        }

        private async Task EnsureCanView(Live_PaymentOrder model)
        {
            if (model.PaymentSignUnitID == Guid.Empty)
            {
                throw new ErrorCodeException(-1, "无权查看支付订单");
            }
            var signUnitIds = await GetViewableSignUnitIds();
            if (!signUnitIds.Contains(model.PaymentSignUnitID))
            {
                throw new ErrorCodeException(-1, "无权查看此签约单位的支付订单");
            }
        }

        private async Task EnsureCanManage(Live_PaymentOrder model)
        {
            if (model.PaymentSignUnitID == Guid.Empty)
            {
                throw new ErrorCodeException(-1, "支付订单未关联签约单位");
            }

            var signUnitIds = await GetManageableSignUnitIds();
            if (!signUnitIds.Contains(model.PaymentSignUnitID))
            {
                throw new ErrorCodeException(-1, "无权管理此签约单位的支付订单");
            }
        }

        private async Task<List<Guid>> GetManageableSignUnitIds()
        {
            var admins = await GetCurrentCompanyPaymentAdmins();
            if (admins.Count == 0)
            {
                throw new ErrorCodeException(-1, "无权管理支付订单");
            }

            var adminIds = admins.Select(f => f.ID).ToList();
            var permissions = await _paymentAdminSignUnitService.List(f =>
                adminIds.Contains(f.PaymentAdminID) && f.IsCanManage);
            if (permissions == null || permissions.Count == 0)
            {
                throw new ErrorCodeException(-1, "无权管理支付订单");
            }

            return permissions.Select(f => f.PaymentSignUnitID).Distinct().ToList();
        }

        private async Task EnsureCanPay(Live_PaymentAdmin admin, Live_PaymentOrder model)
        {
            var signUnitId = await ResolvePaymentSignUnitId(model);
            var canPay = await _paymentAdminSignUnitService.Exist(f =>
                f.PaymentAdminID == admin.ID
                && f.PaymentSignUnitID == signUnitId
                && f.IsCanPay);
            if (!canPay)
            {
                throw new ErrorCodeException(-1, "无权支付此签约单位");
            }
        }

        private async Task<Guid> ResolvePaymentSignUnitId(Live_PaymentOrder model)
        {
            if (model.PaymentSignUnitID != Guid.Empty)
            {
                return model.PaymentSignUnitID;
            }

            if (model.PaymentAccountID.HasValue && model.PaymentAccountID.Value != Guid.Empty)
            {
                var account = await _paymentAccountService.Get(f => f.ID == model.PaymentAccountID.Value);
                if (account == null)
                {
                    throw new ErrorCodeException(-1, "付款账户不存在");
                }
                if (!account.IsEnable)
                {
                    throw new ErrorCodeException(-1, "付款账户未启用");
                }
                if (account.PaymentSignUnitID != Guid.Empty)
                {
                    return account.PaymentSignUnitID;
                }
            }

            if (string.IsNullOrWhiteSpace(model.SignUnitNo) && string.IsNullOrWhiteSpace(model.SignUnitName))
            {
                throw new ErrorCodeException(-1, "支付订单未关联签约单位");
            }

            Live_PaymentSignUnit signUnit = null;
            if (!string.IsNullOrWhiteSpace(model.SignUnitNo))
            {
                signUnit = await _paymentSignUnitService.Get(f =>
                    f.UnitNo == model.SignUnitNo
                    && f.PaymentApplicationID == model.PaymentApplicationID
                    && f.IsEnable);
            }
            if (signUnit == null && !string.IsNullOrWhiteSpace(model.SignUnitName))
            {
                signUnit = await _paymentSignUnitService.Get(f =>
                    f.UnitName == model.SignUnitName
                    && f.PaymentApplicationID == model.PaymentApplicationID
                    && f.IsEnable);
            }
            if (signUnit == null)
            {
                throw new ErrorCodeException(-1, "签约单位不存在或未启用");
            }
            return signUnit.ID;
        }

        private async Task EnsurePaymentAccountReadyToPay(Live_PaymentOrder model)
        {
            if (!model.PaymentAccountID.HasValue || model.PaymentAccountID.Value == Guid.Empty)
            {
                throw new ErrorCodeException(-1, "付款账户不能为空");
            }
            if (model.PaymentSignUnitID == Guid.Empty)
            {
                throw new ErrorCodeException(-1, "支付订单未关联签约单位");
            }

            var paymentAccountId = model.PaymentAccountID.Value;
            var accounts = await _paymentAccountService.ListView(f => f.ID == paymentAccountId);
            var account = accounts?.FirstOrDefault();
            if (account == null)
            {
                throw new ErrorCodeException(-1, "付款账户不存在");
            }
            if (account.IsDelete)
            {
                throw new ErrorCodeException(-1, "付款账户已删除");
            }
            if (!account.IsEnable)
            {
                throw new ErrorCodeException(-1, "付款账户未启用");
            }
            if (!account.CompanyID.HasValue || account.CompanyID.Value != model.CompanyID)
            {
                throw new ErrorCodeException(-1, "付款账户不属于当前订单公司");
            }
            if (account.PaymentSignUnitID == Guid.Empty)
            {
                throw new ErrorCodeException(-1, "付款账户未关联签约单位");
            }
            if (account.PaymentSignUnitID != model.PaymentSignUnitID)
            {
                throw new ErrorCodeException(-1, "付款账户签约单位与订单不一致");
            }
            if (account.PaymentApplicationID == Guid.Empty)
            {
                throw new ErrorCodeException(-1, "付款账户未关联支付应用");
            }
            if (account.PaymentApplicationID != model.PaymentApplicationID)
            {
                throw new ErrorCodeException(-1, "付款账户支付应用与订单不一致");
            }
            if (!IsSameSignUnitName(account.UnitName, model.SignUnitName))
            {
                throw new ErrorCodeException(-1, "付款账户签约单位与订单不一致");
            }
        }

        private static bool IsSameSignUnitName(string accountUnitName, string orderSignUnitName)
        {
            if (string.IsNullOrWhiteSpace(accountUnitName) || string.IsNullOrWhiteSpace(orderSignUnitName))
            {
                return false;
            }
            return string.Equals(accountUnitName.Trim(), orderSignUnitName.Trim(), StringComparison.Ordinal);
        }

        private async Task<Live_PaymentAdmin> GetRequiredPaymentAdmin(Guid paymentApplicationID, int orderCompanyID)
        {
            if (paymentApplicationID == Guid.Empty)
            {
                throw new ErrorCodeException(-1, "支付订单未关联支付应用");
            }

            var application = await _paymentApplicationService.Get(f => f.ID == paymentApplicationID && !f.IsDelete);
            if (application == null)
            {
                throw new ErrorCodeException(-1, "支付应用不存在");
            }
            if (!application.IsEnable)
            {
                throw new ErrorCodeException(-1, "支付应用未启用");
            }
            var currentCompanyGid = CurrentUser.CompanyGID ?? Guid.Empty;
            if (application.CompanyID != CurrentUser.CompanyID || application.CompanyGID != currentCompanyGid)
            {
                throw new ErrorCodeException(-1, "当前用户公司与支付应用公司不匹配");
            }
            if (application.CompanyID != orderCompanyID)
            {
                throw new ErrorCodeException(-1, "支付应用公司与订单公司不匹配");
            }

            var admin = await _paymentAdminService.GetWithSecretkey(f =>
                f.UserID == CurrentUser.UserID
                && f.PaymentApplicationID == application.ID
                && !f.IsDelete);
            if (admin == null)
            {
                throw new ErrorCodeException(-1, "非法的支付用户！");
            }
            if (admin.IsLock)
            {
                throw new ErrorCodeException(-1, "当前支付帐号已被锁定，无法执行当前操作！");
            }
            if (!admin.IsEnable)
            {
                throw new ErrorCodeException(-1, "当前支付帐号未启用，无法执行当前操作！");
            }
            if (!admin.IsValidate)
            {
                throw new ErrorCodeException(-1, "当前支付帐号验证未通过，无法执行当前操作！");
            }
            return admin;
        }

        private async Task ValidatePaymentTotp(Live_PaymentAdmin admin, string totpCode, Guid paymentApplicationID)
        {
            var totpCacheKey = $"PaymentOrder_SubmitPay_Totp_{CurrentUser.UserID}_{paymentApplicationID}_{totpCode}";
            var isTotpExist = await _redisService.SetAsync(totpCacheKey, "动态支付令牌", TimeSpan.FromSeconds(300), CSRedis.RedisExistence.Nx);
            if (!isTotpExist)
            {
                throw new ErrorCodeException(-1, "请求重放检测：TOTP 动态令牌 已被使用！");
            }

            var isSuc = _totpService.ValidateCode(admin.SecretKey, totpCode);
            if (!isSuc)
            {
                admin.FailTimes = (admin.FailTimes ?? 0) + 1;
                if (admin.FailTimes <= 5)
                {
                    admin.FailMsg = $"支付失败：【支付令牌错误】，当前失败次数：【{admin.FailTimes}】，失败次数超过5次，账号将被锁定！";
                }
                else
                {
                    admin.FailMsg = $"支付失败：【支付令牌错误】，当前失败次数：【{admin.FailTimes}】,账号已被锁定！";
                    admin.IsLock = true;
                    admin.LockTime = DateTime.Now;
                }
                admin.UpdateTime = DateTime.Now;
                await _paymentAdminService.Update(admin);
                throw new ErrorCodeException(-1, admin.FailMsg);
            }

            if ((admin.FailTimes ?? 0) > 0)
            {
                admin.FailTimes = 0;
                admin.FailMsg = string.Empty;
                admin.UpdateTime = DateTime.Now;
                await _paymentAdminService.Update(admin);
            }
        }

        private async Task<List<Live_PaymentAdmin>> GetCurrentCompanyPaymentAdmins()
        {
            var appIds = await GetCurrentCompanyApplicationIds();
            if (appIds.Count == 0)
            {
                return new List<Live_PaymentAdmin>();
            }

            var admins = await _paymentAdminService.List(f =>
                f.UserID == CurrentUser.UserID
                && f.IsEnable
                && !f.IsDelete
                && appIds.Contains(f.PaymentApplicationID));
            return admins ?? new List<Live_PaymentAdmin>();
        }

        private async Task<List<Guid>> GetCurrentCompanyApplicationIds()
        {
            var apps = await _paymentApplicationService.List(f => f.CompanyID == CurrentUser.CompanyID && !f.IsDelete);
            if (apps == null || apps.Count == 0)
            {
                return new List<Guid>();
            }
            return apps.Select(f => f.ID).Where(id => id != Guid.Empty).ToList();
        }

        private void EnsureCompanyReadable(int companyID)
        {
            if (companyID != CurrentUser.CompanyID)
            {
                throw new ErrorCodeException(-1, "无权访问其他公司的数据");
            }
        }



        [AllowAnonymous]
        [AesRasRequestHandleFilter]
        [HttpPost]
        public IActionResult TestAesDecrypt(PaymentOrderSubmitPayDto request)
        {
            //string privatePem = System.IO.File.ReadAllText(Path.Combine(_env.ContentRootPath, "RsaKeys", "rsa_private_key_pkcs8.pem"));
            //byte[] aesKey = CryptoHelper.RsaUnwrapAesKey(privatePem, request.WrappedKey);
            //string plainText = CryptoHelper.AesGcmDecrypt(aesKey, request.Cipher);
            return Ok(request);
        }



        [AllowAnonymous]
        [HttpGet]
        public IActionResult TestAesDecrypt1()
        {
            //string privatePem = System.IO.File.ReadAllText(Path.Combine(_env.ContentRootPath, "RsaKeys", "rsa_private_key_pkcs8.pem"));
            //byte[] aesKey = CryptoHelper.RsaUnwrapAesKey(privatePem, request.WrappedKey);
            //string plainText = CryptoHelper.AesGcmDecrypt(aesKey, request.Cipher);
            throw new ErrorCodeException(-1, "5566");
        }
    }
}
