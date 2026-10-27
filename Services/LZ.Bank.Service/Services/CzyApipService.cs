using LZ.Bank.Service.Interface;
using LZ.Core.Interface;
using LZ.Model.Bank.Apip;
using LZ.Model.Options;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LZ.Bank.Service.Services
{
    public class CzyApipService : ICzyApipService, IScopedDependency
    {
        private const string DefaultGatewayUrl = "https://apipcssaasDMZ4.dccnet.com.cn";

        private readonly ApipClient _apipClient;
        private readonly string _gatewayUrl;

        public CzyApipService(ApipClient apipClient, IOptions<CzyOption> options)
        {
            _apipClient = apipClient;
            var gateway = options?.Value?.GatewayUrl;
            _gatewayUrl = string.IsNullOrWhiteSpace(gateway)
                ? DefaultGatewayUrl
                : gateway.TrimEnd('/');
        }

        public Task<AccountQhisdResponseV1> QueryTransactionHistory(AccountQhisdRequestV1.AccountQhisdRequestParamSet paramSet)
        {
            var biz = new AccountQhisdRequestV1.AccountQhisdRequestBiz
            {
                MesgNo = AccountQhisdRequestV1.MesgNoValue,
                ParamSet = new List<AccountQhisdRequestV1.AccountQhisdRequestParamSet> { paramSet }
            };
            return ExecuteAsync<AccountQhisdRequestV1, AccountQhisdResponseV1>(AccountQhisdRequestV1.ServicePath, biz);
        }

        public Task<QueryPaymentStatusResponseV1> QueryPaymentStatus(QueryPaymentStatusRequestV1.QueryPaymentStatusRequestParamSet paramSet)
        {
            return QueryPaymentStatusBatch(new List<QueryPaymentStatusRequestV1.QueryPaymentStatusRequestParamSet> { paramSet });
        }

        public Task<QueryPaymentStatusResponseV1> QueryPaymentStatusBatch(List<QueryPaymentStatusRequestV1.QueryPaymentStatusRequestParamSet> paramSet)
        {
            var biz = new QueryPaymentStatusRequestV1.QueryPaymentStatusRequestBiz
            {
                MesgNo = QueryPaymentStatusRequestV1.MesgNoValue,
                ParamSet = paramSet
            };
            return ExecuteAsync<QueryPaymentStatusRequestV1, QueryPaymentStatusResponseV1>(QueryPaymentStatusRequestV1.ServicePath, biz);
        }

        public Task<AccountQacctinfResponseV1> QueryAccountInfo(AccountQacctinfRequestV1.AccountQacctinfRequestParamSet paramSet)
        {
            var biz = new AccountQacctinfRequestV1.AccountQacctinfRequestBiz
            {
                MesgNo = AccountQacctinfRequestV1.MesgNoValue,
                ParamSet = new List<AccountQacctinfRequestV1.AccountQacctinfRequestParamSet> { paramSet }
            };
            return ExecuteAsync<AccountQacctinfRequestV1, AccountQacctinfResponseV1>(AccountQacctinfRequestV1.ServicePath, biz);
        }

        public Task<AccountQaccbalResponseV1> QueryAccountBalance(AccountQaccbalRequestV1.AccountQaccbalRequestParamSet paramSet)
        {
            var biz = new AccountQaccbalRequestV1.AccountQaccbalRequestBiz
            {
                MesgNo = AccountQaccbalRequestV1.MesgNoValue,
                ParamSet = new List<AccountQaccbalRequestV1.AccountQaccbalRequestParamSet> { paramSet }
            };
            return ExecuteAsync<AccountQaccbalRequestV1, AccountQaccbalResponseV1>(AccountQaccbalRequestV1.ServicePath, biz);
        }

        public Task<BankQuerybranchResponseV1> QueryBankBranch(BankQuerybranchRequestV1.BankQuerybranchRequestParamSet paramSet)
        {
            var biz = new BankQuerybranchRequestV1.BankQuerybranchRequestBiz
            {
                MesgNo = BankQuerybranchRequestV1.MesgNoValue,
                ParamSet = new List<BankQuerybranchRequestV1.BankQuerybranchRequestParamSet> { paramSet }
            };
            return ExecuteAsync<BankQuerybranchRequestV1, BankQuerybranchResponseV1>(BankQuerybranchRequestV1.ServicePath, biz);
        }

        public Task<ClaimQryclmrstResponseV1> QueryClaimResult(ClaimQryclmrstRequestV1.ClaimQryclmrstRequestParamSet paramSet)
        {
            var biz = new ClaimQryclmrstRequestV1.ClaimQryclmrstRequestBiz
            {
                MesgNo = ClaimQryclmrstRequestV1.MesgNoValue,
                ParamSet = new List<ClaimQryclmrstRequestV1.ClaimQryclmrstRequestParamSet> { paramSet }
            };
            return ExecuteAsync<ClaimQryclmrstRequestV1, ClaimQryclmrstResponseV1>(ClaimQryclmrstRequestV1.ServicePath, biz);
        }

        public Task<QuerySingleElectronicReceiptResponseV1> QuerySingleElectronicReceipt(QuerySingleElectronicReceiptRequestV1.QuerySingleElectronicReceiptRequestParamSet paramSet)
        {
            var biz = new QuerySingleElectronicReceiptRequestV1.QuerySingleElectronicReceiptRequestBiz
            {
                MesgNo = QuerySingleElectronicReceiptRequestV1.MesgNoValue,
                ParamSet = new List<QuerySingleElectronicReceiptRequestV1.QuerySingleElectronicReceiptRequestParamSet> { paramSet }
            };
            return ExecuteAsync<QuerySingleElectronicReceiptRequestV1, QuerySingleElectronicReceiptResponseV1>(QuerySingleElectronicReceiptRequestV1.ServicePath, biz);
        }

        public Task<PaymentOrdinarypayResponseV1> PaymentOrdinaryPay(PaymentOrdinarypayRequestV1.PaymentOrdinarypayRequestParamSet paramSet)
        {
            var biz = new PaymentOrdinarypayRequestV1.PaymentOrdinarypayRequestBiz
            {
                MesgNo = PaymentOrdinarypayRequestV1.MesgNoValue,
                ParamSet = new List<PaymentOrdinarypayRequestV1.PaymentOrdinarypayRequestParamSet> { paramSet }
            };
            return ExecuteAsync<PaymentOrdinarypayRequestV1, PaymentOrdinarypayResponseV1>(PaymentOrdinarypayRequestV1.ServicePath, biz);
        }

        public Task<BillBatchPrivateResponseV1> BillBatchPrivate(BillBatchPrivateRequestV1.BillBatchPrivateRequestParamSet paramSet)
        {
            var biz = new BillBatchPrivateRequestV1.BillBatchPrivateRequestBiz
            {
                MesgNo = BillBatchPrivateRequestV1.MesgNoValue,
                ParamSet = new List<BillBatchPrivateRequestV1.BillBatchPrivateRequestParamSet> { paramSet }
            };
            return ExecuteAsync<BillBatchPrivateRequestV1, BillBatchPrivateResponseV1>(BillBatchPrivateRequestV1.ServicePath, biz);
        }

        public Task<PaymentBatchpayResponseV1> PaymentBatchPay(PaymentBatchpayRequestV1.PaymentBatchpayRequestParamSet paramSet)
        {
            var biz = new PaymentBatchpayRequestV1.PaymentBatchpayRequestBiz
            {
                MesgNo = PaymentBatchpayRequestV1.MesgNoValue,
                ParamSet = new List<PaymentBatchpayRequestV1.PaymentBatchpayRequestParamSet> { paramSet }
            };
            return ExecuteAsync<PaymentBatchpayRequestV1, PaymentBatchpayResponseV1>(PaymentBatchpayRequestV1.ServicePath, biz);
        }

        public Task<QueryBankreconciliationResponseV1> QueryBankReconciliation(QueryBankreconciliationRequestV1.QueryBankreconciliationRequestParamSet paramSet)
        {
            var biz = new QueryBankreconciliationRequestV1.QueryBankreconciliationRequestBiz
            {
                MesgNo = QueryBankreconciliationRequestV1.MesgNoValue,
                ParamSet = new List<QueryBankreconciliationRequestV1.QueryBankreconciliationRequestParamSet> { paramSet }
            };
            return ExecuteAsync<QueryBankreconciliationRequestV1, QueryBankreconciliationResponseV1>(QueryBankreconciliationRequestV1.ServicePath, biz);
        }

        public Task<PaymentCommonpayResponseV1> PaymentCommonPay(PaymentCommonpayRequestV1.PaymentCommonpayRequestParamSet paramSet)
        {
            var biz = new PaymentCommonpayRequestV1.PaymentCommonpayRequestBiz
            {
                MesgNo = PaymentCommonpayRequestV1.MesgNoValue,
                ParamSet = new List<PaymentCommonpayRequestV1.PaymentCommonpayRequestParamSet> { paramSet }
            };
            return ExecuteAsync<PaymentCommonpayRequestV1, PaymentCommonpayResponseV1>(PaymentCommonpayRequestV1.ServicePath, biz);
        }

        public Task<BillQueryelectronicreceiptResponseV1> QueryElectronicReceipt(BillQueryelectronicreceiptRequestV1.BillQueryelectronicreceiptRequestParamSet paramSet)
        {
            var biz = new BillQueryelectronicreceiptRequestV1.BillQueryelectronicreceiptRequestBiz
            {
                MesgNo = BillQueryelectronicreceiptRequestV1.MesgNoValue,
                ParamSet = new List<BillQueryelectronicreceiptRequestV1.BillQueryelectronicreceiptRequestParamSet> { paramSet }
            };
            return ExecuteAsync<BillQueryelectronicreceiptRequestV1, BillQueryelectronicreceiptResponseV1>(BillQueryelectronicreceiptRequestV1.ServicePath, biz);
        }

        private async Task<TResponse> ExecuteAsync<TRequest, TResponse>(string servicePath, object bizContent)
            where TRequest : ApipRequest<TResponse>, new()
            where TResponse : ApipResponse
        {
            var request = new TRequest
            {
                ServiceUrl = _gatewayUrl + servicePath,
                BizContent = bizContent
            };
            //Common.StaticLog.Info("财资云接口请求："+ Newtonsoft.Json.JsonConvert.SerializeObject(request));
            var res = await _apipClient.ExecuteAsync(request);
            return res;
        }
    }
}
