using LZ.Core;
using LZ.Core.Interface;
using LZ.Core.Services;
using LZ.Finance.Service.Interface;
using LZ.Model;
using LZ.Model.Dtos.BankDo;
using LZ.Model.Dtos.FinanceDo;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.ModelExtensions;
using LZ.Model.PagedModel;
using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace LZ.Finance.Service.Services
{
    public class PaymentOrderService : ServicesBase<Live_PaymentOrder>, IPaymentOrderService, IScopedDependency
    {
        private readonly IHttpService _httpService;
        private readonly IPaymentAccountService _paymentAccountService;
        private readonly IPaymentApplicationService _paymentApplicationService;

        public PaymentOrderService(IHttpService httpService, IPaymentAccountService paymentAccountService, IPaymentApplicationService paymentApplicationService)
        {
            _httpService = httpService;
            _paymentAccountService = paymentAccountService;
            _paymentApplicationService = paymentApplicationService;
        }
        public async Task<List<Live_PaymentOrder>> PageList(Expression<Func<Live_PaymentOrder, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var list = await base.Context.Queryable<Live_PaymentOrder>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<List<Live_PaymentOrder>> List(Expression<Func<Live_PaymentOrder, bool>> exp)
        {
            return await base.Context.Queryable<Live_PaymentOrder>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }

        public async Task<List<Live_PaymentOrderExtension>> ListNav(Expression<Func<Live_PaymentOrderExtension, bool>> exp)
        {
            return await base.Context.Queryable<Live_PaymentOrderExtension>()
                .Includes(f => f.Records)
                .Where(exp)
                .OrderByDescending(it => it.CreateTime)
                .ToListAsync();
        }

        public async Task<Live_PaymentOrder> Get(Expression<Func<Live_PaymentOrder, bool>> exp)
        {
            return await base.Context.Queryable<Live_PaymentOrder>().Where(exp).FirstAsync();
        }

        public async Task<Live_PaymentOrderExtension> GetNav(Expression<Func<Live_PaymentOrderExtension, bool>> exp)
        {
            return await base.Context.Queryable<Live_PaymentOrderExtension>()
                .Includes(f => f.Records)
                .Where(exp)
                .FirstAsync();
        }

        public async Task<bool> Exist(Expression<Func<Live_PaymentOrder, bool>> exp)
        {
            return await base.Context.Queryable<Live_PaymentOrder>().AnyAsync(exp);
        }

        public async Task<Live_PaymentOrderExtension> Add(PaymentOrderAddDto dto)
        {
            ValidateAddDto(dto);
            var account = await ResolveAddAccount(dto);

            if (await Exist(f => f.BusinessID == dto.BusinessID
                && f.Status != PaymentOrderTransStatusEnumType.支付失败
                && f.Status != PaymentOrderTransStatusEnumType.取消支付))
            {
                throw new ErrorCodeException(-1, $"业务单号重复：{dto.BusinessID}");
            }

            var orderId = Guid.NewGuid();
            var now = DateTime.Now;
            var details = dto.Records.Select(f => new Live_PaymentOrderDetail
            {
                ID = Guid.NewGuid(),
                BusinessID = string.IsNullOrWhiteSpace(f.BusinessID) ? dto.BusinessID : f.BusinessID,
                Name = f.Name,
                CardID = f.CardID,
                BankName = f.BankName,
                BankNo = f.BankNo,
                TransAmount = f.TransAmount,
                TransTime = null,
                Status = PaymentOrderTransStatusEnumType.待支付,
                PaymentOrderID = orderId,
                BillsPayOrderNo = string.IsNullOrWhiteSpace(f.BillsPayOrderNo) ? Guid.NewGuid().ToString() : f.BillsPayOrderNo.Trim(),
            }).ToList();

            var billsPayOrderNo = string.IsNullOrWhiteSpace(dto.BillsPayOrderNo)
                ? Guid.NewGuid().ToString()
                : dto.BillsPayOrderNo.Trim();
            if (details.Any(f => f.BillsPayOrderNo == billsPayOrderNo))
            {
                if (!string.IsNullOrWhiteSpace(dto.BillsPayOrderNo))
                {
                    throw new ErrorCodeException(-1, "支付单号不能与明细支付单号相同");
                }
                billsPayOrderNo = Guid.NewGuid().ToString();
            }
            await EnsureBillsPayOrderNoUnique(billsPayOrderNo, details, null);

            var model = new Live_PaymentOrderExtension
            {
                ID = orderId,
                CompanyID = dto.CompanyID,
                CompanyName = account.CompanyName,
                SignUnitName = account.UnitName,
                SignUnitNo = account.UnitNo,
                CustomerName = dto.CustomerName,
                CustomerNo = dto.CustomerNo,
                BusinessID = dto.BusinessID,
                SalaryType = dto.SalaryType,
                TotalCount = details.Count,
                TotalAmount = details.Sum(f => f.TransAmount),
                Status = PaymentOrderTransStatusEnumType.待支付,
                PushTime = null,
                FailMsg = string.Empty,
                PushCount = 0,
                TransTime = null,
                PayUse = string.IsNullOrWhiteSpace(dto.PayUse) ? "工资" : dto.PayUse,
                Purpose = string.IsNullOrWhiteSpace(dto.Purpose) ? "工资" : dto.Purpose,
                Remark = dto.Remark ?? string.Empty,
                DataType = dto.DataType,
                BillsPayOrderNo = billsPayOrderNo,
                PayType = CzyPayTypeEnumType.普通付款,
                PayChannel =  CzyPayChannelEnumType.线下支付,
                WhetherUrgent = false,
                PaymentAccountID = account.ID,
                PayAccountNo = account.AccountNo,
                PayOrgName = account.UseOrgName.Trim(),
                PayOrgCode = account.UseOrgCode ?? string.Empty,
                BusinessTypeName = account.BusinessTypeName,
                PaymentApplicationID = account.PaymentApplicationID,
                PaymentSignUnitID = account.PaymentSignUnitID,
                AppName = account.AppName,
                AppID = account.AppID,
                PaymentDate = dto.PaymentDate ?? DateTime.Today,
                CreateTime = now,
                UpdateTime = now,
                Records = details,
            };

            switch (model.DataType)
            {
                case TransactionDataTypeEnumType.未知:
                    throw new ErrorCodeException(-1, "未知的数据类型");
                case TransactionDataTypeEnumType.客户工资:
                    model.PayType = CzyPayTypeEnumType.批量对私;
                    break;
                case TransactionDataTypeEnumType.暂缓异常工资:
                case TransactionDataTypeEnumType.预发工资:
                    model.PayType = CzyPayTypeEnumType.普通付款;
                    break;
                default:
                    throw new ErrorCodeException(-1, "未知的数据类型");
            }

            var insertSuc = await AddNav(model);
            if (!insertSuc)
            {
                throw new ErrorCodeException(-1, "支付订单创建失败");
            }
            return model;
        }

        public async Task<Live_PaymentOrderExtension> AddFromPre(PrePaymentOrderDto dto)
        {
            ValidatePreDto(dto);

            if (dto.PaymentSignUnitID == Guid.Empty)
            {
                throw new ErrorCodeException(-1, "此付款账户不能为空");
            }

            var accounts = await _paymentAccountService.ListView(f =>
                f.ID == dto.PaymentAccountID.Value
                && !f.IsDelete
                && f.IsEnable
                && f.CompanyID == dto.CompanyID);
            var account = accounts?.FirstOrDefault();
            if (account == null)
            {
                throw new ErrorCodeException(-1, "付款账户不存在或未启用");
            }
            if (account.UnitName != dto.SignUnitName || account.UnitNo != dto.SignUnitNo)
            {
                throw new ErrorCodeException(-1, "此付款账户不属于指定的签约单位");
            }

            if (!string.IsNullOrWhiteSpace(dto.SignUnitNo)
                && !string.Equals((account.UnitNo ?? string.Empty).Trim(), dto.SignUnitNo.Trim(), StringComparison.Ordinal))
            {
                throw new ErrorCodeException(-1, "付款账户签约单位编号与预览不一致");
            }

            if (await Exist(f => f.BusinessID == dto.BusinessID
                && f.Status != PaymentOrderTransStatusEnumType.支付失败
                && f.Status != PaymentOrderTransStatusEnumType.取消支付))
            {
                throw new ErrorCodeException(-1, $"业务单号重复：{dto.BusinessID}");
            }

            var orderId = Guid.NewGuid();
            var now = DateTime.Now;
            var details = dto.Records.Select(f => new Live_PaymentOrderDetail
            {
                ID = Guid.NewGuid(),
                BusinessID = string.IsNullOrWhiteSpace(f.BusinessID) ? dto.BusinessID : f.BusinessID,
                Name = f.Name,
                CardID = f.CardID,
                BankName = f.BankName,
                BankNo = f.BankNo,
                TransAmount = f.TransAmount,
                TransTime = null,
                Status = PaymentOrderTransStatusEnumType.待支付,
                PaymentOrderID = orderId,
                BillsPayOrderNo = Guid.NewGuid().ToString(),
            }).ToList();

            var billsPayOrderNo = Guid.NewGuid().ToString();
            await EnsureBillsPayOrderNoUnique(billsPayOrderNo, details, null);

            var payType = dto.PayType;
            if (!Enum.IsDefined(typeof(CzyPayTypeEnumType), payType) || payType == 0)
            {
                switch (dto.DataType)
                {
                    case TransactionDataTypeEnumType.客户工资:
                        payType = CzyPayTypeEnumType.批量对私;
                        break;
                    case TransactionDataTypeEnumType.暂缓异常工资:
                    case TransactionDataTypeEnumType.预发工资:
                        payType = CzyPayTypeEnumType.普通付款;
                        break;
                    default:
                        throw new ErrorCodeException(-1, "未知的数据类型");
                }
            }

            var payChannel = dto.PayChannel;
            if (!Enum.IsDefined(typeof(CzyPayChannelEnumType), payChannel) || payChannel == 0)
            {
                payChannel = CzyPayChannelEnumType.线下支付;
            }

            var model = new Live_PaymentOrderExtension
            {
                ID = orderId,
                CompanyID = dto.CompanyID,
                CompanyName = account.CompanyName,
                SignUnitName = account.UnitName,
                SignUnitNo = account.UnitNo,
                CustomerName = dto.CustomerName,
                CustomerNo = dto.CustomerNo,
                BusinessID = dto.BusinessID,
                SalaryType = dto.SalaryType,
                TotalCount = details.Count,
                TotalAmount = details.Sum(f => f.TransAmount),
                Status = PaymentOrderTransStatusEnumType.待支付,
                PushTime = null,
                FailMsg = string.Empty,
                PushCount = 0,
                TransTime = null,
                PayUse = string.IsNullOrWhiteSpace(dto.PayUse) ? "工资" : dto.PayUse,
                Purpose = string.IsNullOrWhiteSpace(dto.Purpose) ? "工资" : dto.Purpose,
                Remark = dto.Remark ?? string.Empty,
                DataType = dto.DataType,
                BillsPayOrderNo = billsPayOrderNo,
                PayType = payType,
                PayChannel = payChannel,
                WhetherUrgent = dto.WhetherUrgent,
                PaymentAccountID = account.ID,
                PayAccountNo = account.AccountNo,
                PayOrgName = account.UseOrgName.Trim(),
                PayOrgCode = account.UseOrgCode ?? string.Empty,
                BusinessTypeName = account.BusinessTypeName,
                PaymentApplicationID = account.PaymentApplicationID,
                PaymentSignUnitID = account.PaymentSignUnitID,
                AppName = account.AppName,
                AppID = account.AppID,
                PaymentDate = dto.PaymentDate ?? DateTime.Today,
                CreateTime = now,
                UpdateTime = now,
                Records = details,
            };

            var insertSuc = await AddNav(model);
            if (!insertSuc)
            {
                throw new ErrorCodeException(-1, "支付订单创建失败");
            }
            return model;
        }

        public async Task<bool> AddNav(Live_PaymentOrderExtension model)
        {
            return await base.Context.InsertNav(model)
                .Include(f => f.Records)
                .ExecuteCommandAsync();
        }

        public async Task<bool> AddNav(List<Live_PaymentOrderExtension> models)
        {
            return await base.Context.InsertNav(models)
                .Include(f => f.Records)
                .ExecuteCommandAsync();
        }


        private async Task<bool> UpdateRecord(List<Live_PaymentOrderDetail> models)
        {
            return await base.Context.Updateable(models).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Update(Live_PaymentOrderExtension model)
        {
            try
            {
                return await base.Context.Updateable(model)
          .IgnoreColumns(ignoreAllNullColumns: true)
          .IsEnableUpdateVersionValidation()
          .ExecuteCommandAsync() > 0;
            }
            catch (VersionExceptions ex)
            {
                throw new ErrorCodeException(-1,"此记录非最新数据,请刷新后重试!");
            }
        }


        public async Task<bool> UpdateByVersion(Live_PaymentOrderExtension model)
        {
            try
            {
                itenant.BeginTran();
                await Update(model);
                if (model.Records != null && model.Records.Any())
                {
                    await UpdateRecord(model.Records);
                }
                itenant.CommitTran();
                return true;
            }
            catch (VersionExceptions ex)
            {
                itenant.RollbackTran();
                if (ex is VersionExceptions)
                {
                    throw new ErrorCodeException(-1, "此记录非最新数据,请刷新后重试!");
                }
                else
                {

                    throw new ErrorCodeException(-1, $"更新支付订单失败,{ex.Message}");
                }
            }
        }

        public async Task<bool> UpdateNav(Live_PaymentOrderExtension model)
        {
            return await UpdateByVersion(model);
        }

        public async Task<List<Live_PaymentOrderDetail>> PageDetailList(Expression<Func<Live_PaymentOrderDetail, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var list = await base.Context.Queryable<Live_PaymentOrderDetail>().Where(exp).OrderByDescending(it => it.CardID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<bool> DeleteNav(Guid id)
        {
            return await base.Context.DeleteNav<Live_PaymentOrderExtension>(f => f.ID == id)
                .Include(f => f.Records)
                .ExecuteCommandAsync();
        }

        public async Task<bool> Cancel(Live_PaymentOrderExtension model)
        {
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            if (model.Status == PaymentOrderTransStatusEnumType.取消支付)
            {
                throw new ErrorCodeException(-1, "订单已作废");
            }
            if (model.Status != PaymentOrderTransStatusEnumType.待支付)
            {
                throw new ErrorCodeException(-1, "非【待支付】状态，无法作废！");
            }
            //if (model.PushCount > 0)
            //{
            //    throw new ErrorCodeException(-1, "检测到已推送记录，无法作废！");
            //}

            model.SetCancel();
            var suc = await UpdateNav(model);
            if (!suc)
            {
                throw new ErrorCodeException(-1, "订单作废失败");
            }

            await RollbackSalaryCustomerPush(model);
            return true;
        }

        public async Task<bool> SubmitPay(Live_PaymentOrderExtension model)
        {
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            if (model.Status != PaymentOrderTransStatusEnumType.待支付)
            {
                throw new ErrorCodeException(-1, "非【待支付】状态，无法执行此操作！");
            }
            if (!model.PaymentAccountID.HasValue || model.PaymentAccountID.Value == Guid.Empty)
            {
                throw new ErrorCodeException(-1, "付款账户不能为空");
            }
            if (model.PaymentApplicationID == Guid.Empty)
            {
                throw new ErrorCodeException(-1, "支付订单未关联支付应用");
            }
            if (model.Records == null || model.Records.Count == 0)
            {
                throw new ErrorCodeException(-1, "支付明细不能为空");
            }
            if (model.Records.Any(f => string.IsNullOrWhiteSpace(f.BankNo)))
            {
                throw new ErrorCodeException(-1, "明细账号不能为空");
            }
            if (model.Records.Any(f => f.TransAmount <= 0))
            {
                throw new ErrorCodeException(-1, "明细金额必须大于0");
            }

            await EnsurePaymentAccountReadyToPay(model);

            model.PushCount += 1;
            model.PushTime = DateTime.Now;
            model.UpdateTime = DateTime.Now;
            model.FailMsg = string.Empty;
            model.Status = PaymentOrderTransStatusEnumType.待系统推送;

            var claimed = await Update(model);
            if (!claimed)
            {
                throw new ErrorCodeException(-1, "支付订单更新失败");
            }

            bool success;
            string message;
            try
            {
                (success, message) = await PushToBank(model);
            }
            catch (Exception ex)
            {
                success = false;
                message = string.IsNullOrWhiteSpace(ex.Message) ? "支付交易推送银行失败" : ex.Message;
            }
            if (success)
            {
                return true;
            }

            await MarkSubmitPayFailed(model.ID, message);
            throw new ErrorCodeException(-1, string.IsNullOrWhiteSpace(message) ? "支付交易推送银行失败" : message);
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
            if (string.IsNullOrWhiteSpace(account.UnitName) || string.IsNullOrWhiteSpace(model.SignUnitName)
                || !string.Equals(account.UnitName.Trim(), model.SignUnitName.Trim(), StringComparison.Ordinal))
            {
                throw new ErrorCodeException(-1, "付款账户签约单位与订单不一致");
            }

            var application = await _paymentApplicationService.Get(f => f.ID == model.PaymentApplicationID && !f.IsDelete);
            if (application == null)
            {
                throw new ErrorCodeException(-1, "支付应用不存在");
            }
            if (!application.IsEnable)
            {
                throw new ErrorCodeException(-1, "支付应用未启用");
            }
        }

        private async Task MarkSubmitPayFailed(Guid paymentOrderId, string message)
        {
            var failMsg = message ?? string.Empty;
            var now = DateTime.Now;
            await base.Context.Updateable<Live_PaymentOrder>()
                .SetColumns(it => it.Status == PaymentOrderTransStatusEnumType.支付失败)
                .SetColumns(it => it.FailMsg == failMsg)
                .SetColumns(it => it.UpdateTime == now)
                .Where(it => it.ID == paymentOrderId)
                .ExecuteCommandAsync();
            await base.Context.Updateable<Live_PaymentOrderDetail>()
                .SetColumns(it => it.Status == PaymentOrderTransStatusEnumType.支付失败)
                .Where(it => it.PaymentOrderID == paymentOrderId)
                .ExecuteCommandAsync();
        }

        /// <summary>
        /// 推送到银行公共创建订单接口
        /// </summary>
        public async Task<(bool Success, string Message)> PushToBank(Live_PaymentOrderExtension model)
        {
            var dto = BuildCzyPayOrderCreateDto(model);
            var json = JsonConvert.SerializeObject(dto);
            if (string.IsNullOrWhiteSpace(json) || _httpService == null)
            {
                return (false, "银行推送数据组装失败");
            }

            var res = await _httpService.PostAsync(ServiceNames.BankService, "/api/CzyPayOrder/Add", json);
            if (res == null)
            {
                return (false, "未能获取到正确的响应信息，请稍后再试！");
            }
            if (res.Code != 200)
            {
                return (false, string.IsNullOrWhiteSpace(res.Message) ? "支付订单推送财资云失败" : res.Message);
            }
            return (true, string.Empty);
        }

        public async Task<bool> HandlePushNotice(PaymentOrderPushNoticeDto dto)
        {
            if (dto == null)
            {
                throw new ErrorCodeException(-1, "通知参数不能为空");
            }

            Live_PaymentOrderExtension model = null;
            if (Guid.TryParse(dto.RelBillsCode, out var paymentOrderId) && paymentOrderId != Guid.Empty)
            {
                model = await GetNav(f => f.ID == paymentOrderId);
            }
            if (model == null && !string.IsNullOrWhiteSpace(dto.BillsPayOrderNo))
            {
                model = await GetNav(f => f.BillsPayOrderNo == dto.BillsPayOrderNo);
            }
            if (model == null)
            {
                throw new ErrorCodeException(-1, "支付订单不存在");
            }
            if (model.Status == PaymentOrderTransStatusEnumType.支付成功
                || model.Status == PaymentOrderTransStatusEnumType.取消支付)
            {
                return true;
            }

            model.UpdateTime = DateTime.Now;
            model.PushTime = dto.PushTime ?? DateTime.Now;
            if (dto.IsSuccess)
            {
                model.Status = PaymentOrderTransStatusEnumType.待核对支付结果;
                model.FailMsg = string.Empty;
                if (model.Records != null)
                {
                    foreach (var item in model.Records)
                    {
                        item.Status = PaymentOrderTransStatusEnumType.待核对支付结果;
                    }
                    await UpdateNav(model);
                }
                else
                {
                    await Update(model);
                }
                return true;
            }

            model.Status = PaymentOrderTransStatusEnumType.支付失败;
            model.FailMsg = dto.ReturnMsg;
            if (model.Records != null)
            {
                foreach (var item in model.Records)
                {
                    item.Status = PaymentOrderTransStatusEnumType.支付失败;
                }
            }
            // 回滚客户工资推送状态
            await RollbackSalaryCustomerPush(model);

            await UpdateNav(model);
            return true;
        }

        private static CzyPayOrderCreateDto BuildCzyPayOrderCreateDto(Live_PaymentOrderExtension model)
        {
            var billsPayOrderNo = !string.IsNullOrWhiteSpace(model.BillsPayOrderNo)
                ? model.BillsPayOrderNo
                : model.ID.ToString();

            return new CzyPayOrderCreateDto
            {
                BillsPayOrderNo = billsPayOrderNo,
                RelBillsCode = model.ID.ToString(),
                PayType = model.PayType,
                PaymentDate = model.PaymentDate ?? DateTime.Today,
                BusinessTypeCode = string.Empty,
                BusinessTypeName = model.BusinessTypeName,
                PayOrgCode = model.PayOrgCode,
                PayOrgName = model.PayOrgName,
                PayAccountNo = model.PayAccountNo,
                PayChannel = model.PayChannel,
                PayAmount = model.TotalAmount,
                PayCurrencyType = "CNY",
                PayCurrencyName = "",
                WhetherUrgent = model.WhetherUrgent,
                Remark = model.Remark,
                PayUse = model.PayUse,
                Purpose = model.Purpose,
                Records = model.Records.Select(f => new CzyPayOrderDetailCreateDto
                {
                    BillsPayOrderNo = f.BillsPayOrderNo,
                    PrivateAmount = f.TransAmount,
                    ReceiveAccountName = f.Name,
                    ReceiveAccountNo = f.BankNo,
                    ReceiveNature = "2",
                    PayUse = model.PayUse,
                    Purpose = model.Purpose,
                }).ToList()
            };
        }

        private async Task RollbackSalaryCustomerPush(Live_PaymentOrderExtension order)
        {
            if (order == null)
            {
                return;
            }
            if (order.DataType == TransactionDataTypeEnumType.暂缓异常工资)
            {
                if (!Guid.TryParse(order.BusinessID, out var salaryRecordId) || salaryRecordId == Guid.Empty)
                {
                    return;
                }
                await base.Context.Updateable<Live_SalaryRecord>()
                    .SetColumns(it => it.IsPushToOrder == false)
                    .Where(it => it.ID == salaryRecordId)
                    .ExecuteCommandAsync();
                return;
            }
            if (order.DataType != TransactionDataTypeEnumType.客户工资)
            {
                return;
            }
            if (!Guid.TryParse(order.BusinessID, out var salaryCustomerId) || salaryCustomerId == Guid.Empty)
            {
                return;
            }

            var recordIds = order.Records?
                .Select(f => Guid.TryParse(f.BusinessID, out var id) ? id : Guid.Empty)
                .Where(f => f != Guid.Empty)
                .ToList() ?? new List<Guid>();

            await base.Context.Updateable<Live_SalaryCustomer>()
                .SetColumns(it => it.IsPushToOrder == false)
                .SetColumns(it => it.IsPushTrans == false)
                .SetColumns(it => it.PushTransTime == null)
                .Where(it => it.ID == salaryCustomerId)
                .ExecuteCommandAsync();

            if (recordIds.Count > 0)
            {
                await base.Context.Updateable<Live_SalaryRecord>()
                    .SetColumns(it => it.IsPushToOrder == false)
                    .Where(it => recordIds.Contains(it.ID))
                    .ExecuteCommandAsync();
            }
        }

        private async Task<Live_V_PaymentAccount> ResolveAddAccount(PaymentOrderAddDto dto)
        {
            var signUnitName = dto.SigningUnitName.Trim();
            List<Live_V_PaymentAccount> accounts;
            if (dto.PaymentAccountID.HasValue && dto.PaymentAccountID.Value != Guid.Empty)
            {
                accounts = await _paymentAccountService.ListView(f =>
                    f.ID == dto.PaymentAccountID.Value
                    && !f.IsDelete
                    && f.IsEnable
                    && f.CompanyID == dto.CompanyID);
            }
            else
            {
                accounts = await _paymentAccountService.ListView(f =>
                    !f.IsDelete
                    && f.IsEnable
                    && f.UnitName == signUnitName
                    && f.CompanyID == dto.CompanyID);
            }
            Live_V_PaymentAccount account;
            if (dto.PaymentAccountID.HasValue && dto.PaymentAccountID.Value != Guid.Empty)
            {
                account = accounts?.FirstOrDefault(f => f.ID == dto.PaymentAccountID.Value);
            }
            else
            {
                account = accounts?.FirstOrDefault(f => f.IsDefault) ?? accounts?.FirstOrDefault();
            }
            if (account == null)
            {
                throw new ErrorCodeException(-1, $"签约单位【{signUnitName}】没有可用的付款账户");
            }
            if (dto.PaymentAccountID.HasValue && dto.PaymentAccountID.Value != Guid.Empty && account.ID != dto.PaymentAccountID.Value)
            {
                throw new ErrorCodeException(-1, "付款账户不存在");
            }
            if (!string.Equals((account.UnitName ?? string.Empty).Trim(), signUnitName, StringComparison.Ordinal))
            {
                throw new ErrorCodeException(-1, "付款账户签约单位与订单不一致");
            }
            if (!account.CompanyID.HasValue || account.CompanyID.Value != dto.CompanyID)
            {
                throw new ErrorCodeException(-1, "付款账户不属于当前公司");
            }
            if (string.IsNullOrWhiteSpace(account.CompanyName))
            {
                throw new ErrorCodeException(-1, "公司名称不能为空");
            }
            if (string.IsNullOrWhiteSpace(account.UnitName))
            {
                throw new ErrorCodeException(-1, "签约单位名称不能为空");
            }
            if (string.IsNullOrWhiteSpace(account.UnitNo))
            {
                throw new ErrorCodeException(-1, "签约单位编号不能为空");
            }
            if (account.PaymentSignUnitID == Guid.Empty)
            {
                throw new ErrorCodeException(-1, "付款账户未关联签约单位");
            }
            if (account.PaymentApplicationID == Guid.Empty)
            {
                throw new ErrorCodeException(-1, "付款账户未关联支付应用");
            }
            if (string.IsNullOrWhiteSpace(account.UseOrgName))
            {
                throw new ErrorCodeException(-1, "付款账户未配置付款机构名称");
            }
            if (string.IsNullOrWhiteSpace(account.AppName))
            {
                throw new ErrorCodeException(-1, "付款账户未配置支付应用信息");
            }
            if (string.IsNullOrWhiteSpace(account.AppID))
            {
                throw new ErrorCodeException(-1, "付款账户未配置支付应用编号");
            }
            var application = await _paymentApplicationService.Get(f => f.ID == account.PaymentApplicationID && !f.IsDelete);
            if (application == null)
            {
                throw new ErrorCodeException(-1, "支付应用不存在");
            }
            if (!application.IsEnable)
            {
                throw new ErrorCodeException(-1, "支付应用未启用");
            }
            return account;
        }

        private static void ValidatePreDto(PrePaymentOrderDto dto)
        {
            if (dto == null)
            {
                throw new ErrorCodeException(-1, "支付订单不能为空");
            }
            if (dto.CompanyID <= 0)
            {
                throw new ErrorCodeException(-1, "公司不能为空");
            }
            if (string.IsNullOrWhiteSpace(dto.SignUnitName))
            {
                throw new ErrorCodeException(-1, "签约单位不能为空");
            }
            if (string.IsNullOrWhiteSpace(dto.CustomerName))
            {
                throw new ErrorCodeException(-1, "客户名称不能为空");
            }
            if (string.IsNullOrWhiteSpace(dto.CustomerNo))
            {
                throw new ErrorCodeException(-1, "客户编号不能为空");
            }
            if (string.IsNullOrWhiteSpace(dto.BusinessID))
            {
                throw new ErrorCodeException(-1, "业务单号不能为空");
            }
            if (!dto.PaymentAccountID.HasValue || dto.PaymentAccountID.Value == Guid.Empty)
            {
                throw new ErrorCodeException(-1, "付款账户不能为空");
            }
            if (dto.DataType == TransactionDataTypeEnumType.未知)
            {
                throw new ErrorCodeException(-1, "未知的数据类型");
            }
            if (dto.Records == null || dto.Records.Count == 0)
            {
                throw new ErrorCodeException(-1, "支付明细不能为空");
            }
            if (dto.Records.Any(f => string.IsNullOrWhiteSpace(f.Name)))
            {
                throw new ErrorCodeException(-1, "明细姓名不能为空");
            }
            if (dto.Records.Any(f => string.IsNullOrWhiteSpace(f.CardID)))
            {
                throw new ErrorCodeException(-1, "明细证件号不能为空");
            }
            if (dto.Records.Any(f => string.IsNullOrWhiteSpace(f.BankName)))
            {
                throw new ErrorCodeException(-1, "明细开户行不能为空");
            }
            if (dto.Records.Any(f => string.IsNullOrWhiteSpace(f.BankNo)))
            {
                throw new ErrorCodeException(-1, "明细账号不能为空");
            }
            if (dto.Records.Any(f => f.TransAmount <= 0))
            {
                throw new ErrorCodeException(-1, "明细金额必须大于0");
            }
        }

        private static void ValidateAddDto(PaymentOrderAddDto dto)
        {
            if (dto == null)
            {
                throw new ErrorCodeException(-1, "支付订单不能为空");
            }
            if (dto.CompanyID <= 0)
            {
                throw new ErrorCodeException(-1, "公司不能为空");
            }
            if (string.IsNullOrWhiteSpace(dto.SigningUnitName))
            {
                throw new ErrorCodeException(-1, "签约单位不能为空");
            }
            if (string.IsNullOrWhiteSpace(dto.CustomerName))
            {
                throw new ErrorCodeException(-1, "客户名称不能为空");
            }
            if (string.IsNullOrWhiteSpace(dto.CustomerNo))
            {
                throw new ErrorCodeException(-1, "客户编号不能为空");
            }
            if (string.IsNullOrWhiteSpace(dto.BusinessID))
            {
                throw new ErrorCodeException(-1, "业务单号不能为空");
            }
            if (dto.Records == null || dto.Records.Count == 0)
            {
                throw new ErrorCodeException(-1, "支付明细不能为空");
            }
            if (dto.Records.Count > 1)
            {
                if (string.IsNullOrWhiteSpace(dto.BillsPayOrderNo))
                {
                    throw new ErrorCodeException(-1, "支付单号不能为空");
                }
                if (dto.Records.Any(f => string.IsNullOrWhiteSpace(f.BillsPayOrderNo)))
                {
                    throw new ErrorCodeException(-1, "明细支付单号不能为空");
                }
            }
            var billsNos = dto.Records
                .Where(f => !string.IsNullOrWhiteSpace(f.BillsPayOrderNo))
                .Select(f => f.BillsPayOrderNo.Trim())
                .ToList();
            if (!string.IsNullOrWhiteSpace(dto.BillsPayOrderNo))
            {
                billsNos.Insert(0, dto.BillsPayOrderNo.Trim());
            }
            if (billsNos.Count != billsNos.Distinct().Count())
            {
                throw new ErrorCodeException(-1, "支付单号不能重复");
            }
            if (dto.Records.Any(f => string.IsNullOrWhiteSpace(f.Name)))
            {
                throw new ErrorCodeException(-1, "明细姓名不能为空");
            }
            if (dto.Records.Any(f => string.IsNullOrWhiteSpace(f.CardID)))
            {
                throw new ErrorCodeException(-1, "明细证件号不能为空");
            }
            if (dto.Records.Any(f => string.IsNullOrWhiteSpace(f.BankName)))
            {
                throw new ErrorCodeException(-1, "明细开户行不能为空");
            }
            if (dto.Records.Any(f => string.IsNullOrWhiteSpace(f.BankNo)))
            {
                throw new ErrorCodeException(-1, "明细账号不能为空");
            }
            if (dto.Records.Any(f => f.TransAmount <= 0))
            {
                throw new ErrorCodeException(-1, "明细金额必须大于0");
            }
        }

        private async Task EnsureBillsPayOrderNoUnique(string billsPayOrderNo, List<Live_PaymentOrderDetail> details, Guid? excludeOrderId)
        {
            var nos = new List<string> { billsPayOrderNo };
            nos.AddRange(details.Select(f => f.BillsPayOrderNo));
            if (nos.Any(string.IsNullOrWhiteSpace))
            {
                throw new ErrorCodeException(-1, "支付单号不能为空");
            }
            if (nos.Count != nos.Distinct().Count())
            {
                throw new ErrorCodeException(-1, "支付单号不能重复");
            }

            foreach (var no in nos)
            {
                var orderExist = excludeOrderId.HasValue
                    ? await Exist(f => f.BillsPayOrderNo == no && f.ID != excludeOrderId.Value)
                    : await Exist(f => f.BillsPayOrderNo == no);
                var detailQuery = base.Context.Queryable<Live_PaymentOrderDetail>().Where(f => f.BillsPayOrderNo == no);
                if (excludeOrderId.HasValue)
                {
                    detailQuery = detailQuery.Where(f => f.PaymentOrderID != excludeOrderId.Value);
                }
                var detailExist = await detailQuery.AnyAsync();
                if (orderExist || detailExist)
                {
                    throw new ErrorCodeException(-1, $"支付单号已存在：{no}");
                }
            }
        }
    }
}
