using LZ.Bank.Service.Interface;
using LZ.Core;
using LZ.Model.Bank.Apip;
using LZ.Model.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LZ.Bank.Api.Controllers
{
    /// <summary>
    /// 财资云 APIP 业务接口
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]

    public class CzyApipController : ApiControllerBase
    {
        private readonly ICzyApipService _czyApipService;

        public CzyApipController(ICzyApipService czyApipService)
        {
            _czyApipService = czyApipService;
        }

        #region 查询

        /// <summary>T0001 交易流水信息查询</summary>
        [HttpPost]
        public async Task<IActionResult> QueryTransactionHistory([FromBody] AccountQhisdRequestV1.AccountQhisdRequestParamSet paramSet)
        {
            EnsureParamSet(paramSet);
            var res = await _czyApipService.QueryTransactionHistory(paramSet);
            return Ok(res);
        }

        /// <summary>T0004 应付单业务状态查询</summary>
        [HttpPost]
        public async Task<IActionResult> QueryPaymentStatus([FromBody] QueryPaymentStatusRequestV1.QueryPaymentStatusRequestParamSet paramSet)
        {
            EnsureParamSet(paramSet);
            var res = await _czyApipService.QueryPaymentStatus(paramSet);
            return Ok(res);
        }

        /// <summary>T0004 应付单业务状态查询（多个单据编号）</summary>
        [HttpPost]
        public async Task<IActionResult> QueryPaymentStatusBatch([FromBody] List<string> billsPayOrderNos)
        {
            if (billsPayOrderNos == null || billsPayOrderNos.Count == 0)
            {
                throw new ErrorCodeException(-1, "单据编号不能为空");
            }
            var nos = billsPayOrderNos.Where(f => !string.IsNullOrWhiteSpace(f)).Select(f => f.Trim()).Distinct().ToList();
            if (nos.Count == 0)
            {
                throw new ErrorCodeException(-1, "单据编号不能为空");
            }
            var paramSet = nos.Select(f => new QueryPaymentStatusRequestV1.QueryPaymentStatusRequestParamSet
            {
                BillsPayOrderNo = f
            }).ToList();
            var res = await _czyApipService.QueryPaymentStatusBatch(paramSet);
            return Ok(res);
        }

        /// <summary>T0005 账户信息查询</summary>
        [HttpPost]
        public async Task<IActionResult> QueryAccountInfo([FromBody] AccountQacctinfRequestV1.AccountQacctinfRequestParamSet paramSet)
        {
            EnsureParamSet(paramSet);
            var res = await _czyApipService.QueryAccountInfo(paramSet);
            return Ok(res);
        }

        /// <summary>T0006 账户余额查询</summary>
        [HttpPost]
        public async Task<IActionResult> QueryAccountBalance([FromBody] AccountQaccbalRequestV1.AccountQaccbalRequestParamSet paramSet)
        {
            EnsureParamSet(paramSet);
            var res = await _czyApipService.QueryAccountBalance(paramSet);
            return Ok(res);
        }

        /// <summary>T0014 金融网点查询</summary>
        [HttpPost]
        public async Task<IActionResult> QueryBankBranch([FromBody] BankQuerybranchRequestV1.BankQuerybranchRequestParamSet paramSet)
        {
            EnsureParamSet(paramSet);
            var res = await _czyApipService.QueryBankBranch(paramSet);
            return Ok(res);
        }

        /// <summary>T0023 认领结果查询</summary>
        [HttpPost]
        public async Task<IActionResult> QueryClaimResult([FromBody] ClaimQryclmrstRequestV1.ClaimQryclmrstRequestParamSet paramSet)
        {
            EnsureParamSet(paramSet);
            var res = await _czyApipService.QueryClaimResult(paramSet);
            return Ok(res);
        }

        /// <summary>T0039 单笔电子回单查询</summary>
        [HttpPost]
        public async Task<IActionResult> QuerySingleElectronicReceipt([FromBody] QuerySingleElectronicReceiptRequestV1.QuerySingleElectronicReceiptRequestParamSet paramSet)
        {
            EnsureParamSet(paramSet);
            var res = await _czyApipService.QuerySingleElectronicReceipt(paramSet);
            return Ok(res);
        }

        /// <summary>T1019 银行对账查询</summary>
        [HttpPost]
        public async Task<IActionResult> QueryBankReconciliation([FromBody] QueryBankreconciliationRequestV1.QueryBankreconciliationRequestParamSet paramSet)
        {
            EnsureParamSet(paramSet);
            var res = await _czyApipService.QueryBankReconciliation(paramSet);
            return Ok(res);
        }

        /// <summary>T1028 电子回单查询</summary>
        [HttpPost]
        public async Task<IActionResult> QueryElectronicReceipt([FromBody] BillQueryelectronicreceiptRequestV1.BillQueryelectronicreceiptRequestParamSet paramSet)
        {
            EnsureParamSet(paramSet);
            var res = await _czyApipService.QueryElectronicReceipt(paramSet);
            return Ok(res);
        }

        #endregion

        #region 交易

        /// <summary>T1002 单笔付款</summary>
        [HttpPost]
        public async Task<IActionResult> PaymentOrdinaryPay([FromBody] PaymentOrdinarypayRequestV1.PaymentOrdinarypayRequestParamSet paramSet)
        {
            EnsureParamSet(paramSet);
            var res = await _czyApipService.PaymentOrdinaryPay(paramSet);
            return Ok(res);
        }

        /// <summary>T1003 工资代发批量对私</summary>
        [HttpPost]
        public async Task<IActionResult> BillBatchPrivate([FromBody] BillBatchPrivateRequestV1.BillBatchPrivateRequestParamSet paramSet)
        {
            EnsureParamSet(paramSet);
            var res = await _czyApipService.BillBatchPrivate(paramSet);
            return Ok(res);
        }

        /// <summary>T1018 批量付款</summary>
        [HttpPost]
        public async Task<IActionResult> PaymentBatchPay([FromBody] PaymentBatchpayRequestV1.PaymentBatchpayRequestParamSet paramSet)
        {
            EnsureParamSet(paramSet);
            var res = await _czyApipService.PaymentBatchPay(paramSet);
            return Ok(res);
        }

        /// <summary>T1022 通用付款</summary>
        [HttpPost]
        public async Task<IActionResult> PaymentCommonPay([FromBody] PaymentCommonpayRequestV1.PaymentCommonpayRequestParamSet paramSet)
        {
            EnsureParamSet(paramSet);
            var res = await _czyApipService.PaymentCommonPay(paramSet);
            return Ok(res);
        }

        #endregion

        private static void EnsureParamSet(object paramSet)
        {
            if (paramSet == null)
            {
                throw new ErrorCodeException(-1, "请求参数不能为空");
            }
        }
    }
}
