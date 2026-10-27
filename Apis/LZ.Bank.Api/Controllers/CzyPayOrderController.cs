using LZ.Bank.Service.Interface;
using LZ.Core;
using LZ.Model;
using LZ.Model.Dtos.BankDo;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;

namespace LZ.Bank.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CzyPayOrderController : ApiControllerBase
    {
        private readonly ICzyPayOrderService _service;

        public CzyPayOrderController(ICzyPayOrderService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> PageList(string billsPayOrderNo, string payOrgName, string payAccountNo, string payStatus, int? transStatus, CzyPushStatusEnumType? pushStatus, [FromQuery] PagedModelBase query)
        {
            var exp = Expressionable.Create<Czy_PayOrder>();
            exp.AndIF(!string.IsNullOrWhiteSpace(billsPayOrderNo), f => f.BillsPayOrderNo.Contains(billsPayOrderNo));
            exp.AndIF(!string.IsNullOrWhiteSpace(payOrgName), f => f.PayOrgName.Contains(payOrgName));
            exp.AndIF(!string.IsNullOrWhiteSpace(payAccountNo), f => f.PayAccountNo.Contains(payAccountNo));
            exp.AndIF(!string.IsNullOrWhiteSpace(payStatus), f => f.PayStatus == payStatus);
            exp.AndIF(transStatus.HasValue, f => f.TransStatus == transStatus.Value);
            exp.AndIF(pushStatus.HasValue, f => f.PushStatus == pushStatus.Value);
            var models = await _service.PageList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }

        [HttpGet]
        public async Task<IActionResult> PageDetailList(Guid? payOrderID, string billsPayOrderNo, string receiveAccountName, string receiveAccountNo, string payStatus, [FromQuery] PagedModelBase query)
        {
            var exp = Expressionable.Create<Czy_PayOrderDetail>();
            exp.AndIF(payOrderID.HasValue && payOrderID.Value != Guid.Empty, f => f.PayOrderID == payOrderID.Value);
            exp.AndIF(!string.IsNullOrWhiteSpace(billsPayOrderNo), f => f.BillsPayOrderNo.Contains(billsPayOrderNo));
            exp.AndIF(!string.IsNullOrWhiteSpace(receiveAccountName), f => f.ReceiveAccountName.Contains(receiveAccountName));
            exp.AndIF(!string.IsNullOrWhiteSpace(receiveAccountNo), f => f.ReceiveAccountNo.Contains(receiveAccountNo));
            exp.AndIF(!string.IsNullOrWhiteSpace(payStatus), f => f.PayStatus == payStatus);
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
            return Ok(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetByBillsPayOrderNo(string billsPayOrderNo)
        {
            if (string.IsNullOrWhiteSpace(billsPayOrderNo))
            {
                throw new ErrorCodeException(-1, "应付单号不能为空");
            }
            var model = await _service.GetNav(f => f.BillsPayOrderNo == billsPayOrderNo);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return Ok(model);
        }

        /// <summary>
        /// 公共创建订单接口
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Add(CzyPayOrderCreateDto dto)
        {
            await CreateOrder(dto);
            return Ok("创建成功");
        }

        //[HttpPut]
        //public async Task<IActionResult> Update(Czy_PayOrderExtension model)
        //{
        //    var exist = await _service.Get(f => f.ID == model.ID);
        //    if (exist == null)
        //    {
        //        throw new ErrorCodeException(-1, "你要读取的数据不存在");
        //    }
        //    ValidateEntityDetails(model);
        //    PrepareEntity(model, exist);
        //    await EnsureBillsPayOrderNoUnique(model, exist.ID);

        //    try
        //    {
        //        DbScoped.SugarScope.BeginTran();
        //        await _service.UpdateNav(model);
        //        DbScoped.SugarScope.CommitTran();
        //    }
        //    catch (Exception ex)
        //    {
        //        DbScoped.SugarScope.RollbackTran();
        //        throw new ErrorCodeException(-1, ex.Message, ex);
        //    }
        //    return Ok();
        //}

        //[HttpDelete]
        //public async Task<IActionResult> Delete([FromForm] Guid id)
        //{
        //    var exist = await _service.Get(f => f.ID == id);
        //    if (exist == null)
        //    {
        //        throw new ErrorCodeException(-1, "你要读取的数据不存在");
        //    }
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

        private async Task CreateOrder(CzyPayOrderCreateDto dto)
        {
            ValidateCreateDto(dto);
            var model = MapFromCreateDto(dto);
            PrepareEntity(model);
            await EnsureBillsPayOrderNoUnique(model);
            try
            {
                DbScoped.SugarScope.BeginTran();
                await _service.AddNav(model);
                DbScoped.SugarScope.CommitTran();
            }
            catch (Exception ex)
            {
                DbScoped.SugarScope.RollbackTran();
                throw new ErrorCodeException(-1, ex.Message, ex);
            }
        }

        private static void ValidateCreateDto(CzyPayOrderCreateDto dto)
        {
            if (dto == null)
            {
                throw new ErrorCodeException(-1, "支付订单不能为空");
            }
            if (dto.Records == null || dto.Records.Count == 0)
            {
                throw new ErrorCodeException(-1, "支付明细不能为空");
            }
            if (dto.Records.Any(f => string.IsNullOrWhiteSpace(f.ReceiveAccountName)))
            {
                throw new ErrorCodeException(-1, "收款户名不能为空");
            }
            if (dto.Records.Any(f => string.IsNullOrWhiteSpace(f.ReceiveAccountNo)))
            {
                throw new ErrorCodeException(-1, "收款账号不能为空");
            }
            if (dto.Records.Any(f => f.PrivateAmount <= 0))
            {
                throw new ErrorCodeException(-1, "明细金额必须大于0");
            }
            if (dto.Records.Count > 1)
            {
                if (string.IsNullOrWhiteSpace(dto.BillsPayOrderNo))
                {
                    throw new ErrorCodeException(-1, "应付单号不能为空");
                }
                if (dto.Records.Any(f => string.IsNullOrWhiteSpace(f.BillsPayOrderNo)))
                {
                    throw new ErrorCodeException(-1, "明细应付单号不能为空");
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
                throw new ErrorCodeException(-1, "应付单号不能重复");
            }
        }

        private static Czy_PayOrderExtension MapFromCreateDto(CzyPayOrderCreateDto dto)
        {
            return new Czy_PayOrderExtension
            {
                ID = Guid.NewGuid(),
                BillsPayOrderNo = dto.BillsPayOrderNo,
                RelBillsCode = dto.RelBillsCode,
                PayType = dto.PayType,
                PaymentDate = dto.PaymentDate,
                BusinessTypeCode = dto.BusinessTypeCode,
                BusinessTypeName = dto.BusinessTypeName,
                PayOrgCode = dto.PayOrgCode,
                PayOrgName = dto.PayOrgName,
                PayAccountNo = dto.PayAccountNo,
                PayChannel = dto.PayChannel,
                PayAmount = dto.PayAmount,
                PayCurrencyType = dto.PayCurrencyType,
                PayCurrencyName = dto.PayCurrencyName,
                WhetherUrgent = dto.WhetherUrgent,
                Remark = dto.Remark,
                PayUse = dto.PayUse,
                Purpose = dto.Purpose,
                CreateByCode = dto.CreateByCode,
                CreateByName = dto.CreateByName,
                SerialNo = dto.SerialNo,
                TransStatus = 0,
                PushStatus = CzyPushStatusEnumType.待推送,
                CreateTime = DateTime.Now,
                PayableAmount = dto.PayAmount,
                Records = dto.Records.Select(f => new Czy_PayOrderDetail
                {
                    ID = Guid.NewGuid(),
                    BillsPayOrderNo = f.BillsPayOrderNo,
                    PrivateAmount = f.PrivateAmount,
                    ReceiveAccountNo = f.ReceiveAccountNo,
                    ReceiveAccountName = f.ReceiveAccountName,
                    ReceiveNature = f.ReceiveNature,
                    PayUse = f.PayUse,
                    Purpose = f.Purpose,
                }).ToList()
            };
        }

        private static void ValidateEntityDetails(Czy_PayOrderExtension model)
        {
            if (model == null)
            {
                throw new ErrorCodeException(-1, "支付订单不能为空");
            }
            if (model.Records == null || model.Records.Count == 0)
            {
                throw new ErrorCodeException(-1, "支付明细不能为空");
            }
            if (model.Records.Any(f => string.IsNullOrWhiteSpace(f.ReceiveAccountName)))
            {
                throw new ErrorCodeException(-1, "收款户名不能为空");
            }
            if (model.Records.Any(f => string.IsNullOrWhiteSpace(f.ReceiveAccountNo)))
            {
                throw new ErrorCodeException(-1, "收款账号不能为空");
            }
            if (model.Records.Any(f => f.PrivateAmount <= 0))
            {
                throw new ErrorCodeException(-1, "明细金额必须大于0");
            }
        }

        private static void PrepareEntity(Czy_PayOrderExtension model, Czy_PayOrder exist = null)
        {
            if (model.ID == Guid.Empty)
            {
                model.ID = Guid.NewGuid();
            }
            if (string.IsNullOrWhiteSpace(model.BillsPayOrderNo))
            {
                model.BillsPayOrderNo = Guid.NewGuid().ToString();
            }
            else
            {
                model.BillsPayOrderNo = model.BillsPayOrderNo.Trim();
            }
            //if (model.PayType <= 0)
            //{
            //    model.PayType = model.PayType

            //}
            if (string.IsNullOrWhiteSpace(model.BusinessTypeName))
            {
                model.BusinessTypeName = "批量对私";
            }
            if (string.IsNullOrWhiteSpace(model.PayUse))
            {
                model.PayUse = "工资";
            }
            if (string.IsNullOrWhiteSpace(model.PayCurrencyType))
            {
                model.PayCurrencyType = "CNY";
            }
            if (string.IsNullOrWhiteSpace(model.PayCurrencyName))
            {
                model.PayCurrencyName = "";
            }
            if (model.PayChannel <= 0)
            {
                model.PayChannel = CzyPayChannelEnumType.直联支付;
            }
            if (string.IsNullOrWhiteSpace(model.SerialNo))
            {
                model.SerialNo = Guid.NewGuid().ToString();
            }
            if (!model.PaymentDate.HasValue)
            {
                model.PaymentDate = DateTime.Today;
            }
            if (!model.CreateTime.HasValue)
            {
                model.CreateTime = exist?.CreateTime ?? DateTime.Now;
            }
            if (exist != null)
            {
                model.PushStatus = exist.PushStatus;
                model.PushTime = exist.PushTime;
            }
            else
            {
                model.PushStatus = CzyPushStatusEnumType.待推送;
            }

            model.PayAmount = model.Records.Sum(f => f.PrivateAmount);
            if (!model.PayableAmount.HasValue)
            {
                model.PayableAmount = model.PayAmount;
            }

            foreach (var detail in model.Records)
            {
                if (detail.ID == Guid.Empty)
                {
                    detail.ID = Guid.NewGuid();
                }
                detail.PayOrderID = model.ID;
                if (string.IsNullOrWhiteSpace(detail.BillsPayOrderNo))
                {
                    do
                    {
                        detail.BillsPayOrderNo = Guid.NewGuid().ToString();
                    } while (detail.BillsPayOrderNo == model.BillsPayOrderNo);
                }
                else
                {
                    detail.BillsPayOrderNo = detail.BillsPayOrderNo.Trim();
                    if (detail.BillsPayOrderNo == model.BillsPayOrderNo)
                    {
                        throw new ErrorCodeException(-1, "应付单号不能与明细应付单号相同");
                    }
                }
                if (string.IsNullOrWhiteSpace(detail.ReceiveCurrencyCode))
                {
                    detail.ReceiveCurrencyCode = "CNY";
                }
                if (string.IsNullOrWhiteSpace(detail.ReceiveCurrencyName))
                {
                    detail.ReceiveCurrencyName = "人民币";
                }
                if (string.IsNullOrWhiteSpace(detail.ReceiveNature))
                {
                    detail.ReceiveNature = "2";
                }
                if (string.IsNullOrWhiteSpace(detail.PayUse))
                {
                    detail.PayUse = model.PayUse;
                }
                if (string.IsNullOrWhiteSpace(detail.Purpose))
                {
                    detail.Purpose = model.Purpose;
                }
            }
        }

        private async Task EnsureBillsPayOrderNoUnique(Czy_PayOrderExtension model, Guid? excludeOrderId = null)
        {
            var nos = new List<string> { model.BillsPayOrderNo };
            nos.AddRange(model.Records.Select(f => f.BillsPayOrderNo));
            if (nos.Any(string.IsNullOrWhiteSpace))
            {
                throw new ErrorCodeException(-1, "应付单号不能为空");
            }
            if (nos.Count != nos.Distinct().Count())
            {
                throw new ErrorCodeException(-1, "应付单号不能重复");
            }

            foreach (var no in nos)
            {
                var orderExist = excludeOrderId.HasValue
                    ? await _service.Exist(f => f.BillsPayOrderNo == no && f.ID != excludeOrderId.Value)
                    : await _service.Exist(f => f.BillsPayOrderNo == no);
                var detailExist = excludeOrderId.HasValue
                    ? await _service.ExistDetail(f => f.BillsPayOrderNo == no && f.PayOrderID != excludeOrderId.Value)
                    : await _service.ExistDetail(f => f.BillsPayOrderNo == no);
                if (orderExist || detailExist)
                {
                    throw new ErrorCodeException(-1, $"应付单号已存在：{no}");
                }
            }
        }
    }
}
