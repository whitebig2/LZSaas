using LZ.Model.Bank.Apip;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LZ.Bank.Service.Interface
{
    public interface ICzyApipService
    {
        /// <summary>T0001 交易流水信息查询</summary>
        Task<AccountQhisdResponseV1> QueryTransactionHistory(AccountQhisdRequestV1.AccountQhisdRequestParamSet paramSet);
        /// <summary>T0004 应付单业务状态查询</summary>
        Task<QueryPaymentStatusResponseV1> QueryPaymentStatus(QueryPaymentStatusRequestV1.QueryPaymentStatusRequestParamSet paramSet);
        /// <summary>T0004 应付单业务状态查询（多个单据编号）</summary>
        Task<QueryPaymentStatusResponseV1> QueryPaymentStatusBatch(List<QueryPaymentStatusRequestV1.QueryPaymentStatusRequestParamSet> paramSet);
        /// <summary>T0005 账户信息查询</summary>
        Task<AccountQacctinfResponseV1> QueryAccountInfo(AccountQacctinfRequestV1.AccountQacctinfRequestParamSet paramSet);
        /// <summary>T0006 账户余额查询</summary>
        Task<AccountQaccbalResponseV1> QueryAccountBalance(AccountQaccbalRequestV1.AccountQaccbalRequestParamSet paramSet);
        /// <summary>T0014 金融网点查询</summary>
        Task<BankQuerybranchResponseV1> QueryBankBranch(BankQuerybranchRequestV1.BankQuerybranchRequestParamSet paramSet);
        /// <summary>T0023 认领结果查询</summary>
        Task<ClaimQryclmrstResponseV1> QueryClaimResult(ClaimQryclmrstRequestV1.ClaimQryclmrstRequestParamSet paramSet);
        /// <summary>T0039 单笔电子回单查询</summary>
        Task<QuerySingleElectronicReceiptResponseV1> QuerySingleElectronicReceipt(QuerySingleElectronicReceiptRequestV1.QuerySingleElectronicReceiptRequestParamSet paramSet);
        /// <summary>T1002 单笔付款</summary>
        Task<PaymentOrdinarypayResponseV1> PaymentOrdinaryPay(PaymentOrdinarypayRequestV1.PaymentOrdinarypayRequestParamSet paramSet);
        /// <summary>T1003 工资代发批量对私</summary>
        Task<BillBatchPrivateResponseV1> BillBatchPrivate(BillBatchPrivateRequestV1.BillBatchPrivateRequestParamSet paramSet);
        /// <summary>T1018 批量付款</summary>
        Task<PaymentBatchpayResponseV1> PaymentBatchPay(PaymentBatchpayRequestV1.PaymentBatchpayRequestParamSet paramSet);
        /// <summary>T1019 银行对账查询</summary>
        Task<QueryBankreconciliationResponseV1> QueryBankReconciliation(QueryBankreconciliationRequestV1.QueryBankreconciliationRequestParamSet paramSet);
        /// <summary>T1022 通用付款</summary>
        Task<PaymentCommonpayResponseV1> PaymentCommonPay(PaymentCommonpayRequestV1.PaymentCommonpayRequestParamSet paramSet);
        /// <summary>T1028 电子回单查询</summary>
        Task<BillQueryelectronicreceiptResponseV1> QueryElectronicReceipt(BillQueryelectronicreceiptRequestV1.BillQueryelectronicreceiptRequestParamSet paramSet);
    }
}
