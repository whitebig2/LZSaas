using LZ.Bank.Service.Interface;
using LZ.Core.Interface;
using LZ.Core;
using LZ.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LZ.Model.Options;
using Microsoft.Extensions.Options;
using com.icbc.api.request;
using com.icbc.api;
using static com.icbc.api.request.JftUiUserOpenacctH5SubmitRequestV1;
using java.util;
using static com.icbc.api.request.JftUiUserOpenacctH5MywalletRequestV1;
using LZ.Model.Bank.Wallet;
using com.icbc.api.response;
using static com.icbc.api.request.JftApiUserOpenacctH5QueryRequestV1;
using static com.icbc.api.request.JftWalletApiAcctCloseRequestV1;
using static com.icbc.api.request.JftWalletApiAcctCloseMsgCheckRequestV1;

namespace LZ.Bank.Service.Services
{
    public class BankWalletService : ServicesBase<Live_BankWallet>, IBankWalletService, IScopedDependency
    {
        public string Acct(IOptions<BankWalletOption> option, Live_BankWallet wallet)
        {
            UiIcbcClient client = new UiIcbcClient(option.Value.AppId, IcbcConstants.SIGN_TYPE_RSA2, option.Value.MyPrivateKey, IcbcConstants.CHARSET_UTF8);
            JftUiUserOpenacctH5SubmitRequestV1 request = new JftUiUserOpenacctH5SubmitRequestV1();
            request.setServiceUrl(option.Value.Url + "/ui/jft/ui/user/openacctH5/submit/V1");//测试地址
            JftUiUserOpenacctH5SubmitRequestV1Biz bizContent = new JftUiUserOpenacctH5SubmitRequestV1Biz();
            bizContent.setAppId(option.Value.AppId);
            bizContent.setCorpNo(option.Value.CorpNo);
            bizContent.setCorpSerno(wallet.GID.ToString());

            Map preFillItems = new HashMap();
            preFillItems.put("custName", wallet.Name);
            preFillItems.put("certType", "0");
            preFillItems.put("certNo", wallet.CertNo);
            if (!string.IsNullOrEmpty(wallet.SignDate))
            {
                preFillItems.put("signDate", wallet.SignDate);
            }
            if (!string.IsNullOrEmpty(wallet.ValidityPeriod))
            {
                preFillItems.put("validityPeriod", wallet.ValidityPeriod);
            }

            preFillItems.put("address", wallet.Address);
            preFillItems.put("bindMedium", wallet.BindMedium);
            preFillItems.put("mobile", wallet.Mobile);
            preFillItems.put("occupation", "3");
            bizContent.setPreFillItems(preFillItems);
            bizContent.setOutUserId(wallet.OutUserID);
            bizContent.setCorpSerno(wallet.GID.ToString());

            bizContent.setTrxChannel("01");
            //bizContent.setCallbackUrl("http://localhost:9981/api/icbc/openAcctResult");
            //bizContent.setJumpUrl("http://localhost:9981/api/icbc/openAcctResult");
            //bizContent.setFailJumpUrl("http://localhost:9981/api/icbc/openAcctResult");
            //bizContent.setAccountOrgNo(null);
            bizContent.setCamsPublicKey(option.Value.CamsPublicKey);//工行的SM2 公钥
            bizContent.setOpenProtocol("0");
            bizContent.setCopyMediumId("1");

            request.setBizContent(bizContent);

            var str = client.buildPostForm(request);



            return str;
        }

        public string MyWallet(IOptions<BankWalletOption> option, Live_BankWallet wallet)
        {
            UiIcbcClient client = new UiIcbcClient(option.Value.AppId, IcbcConstants.SIGN_TYPE_RSA2, option.Value.MyPrivateKey, IcbcConstants.CHARSET_UTF8);
            JftUiUserOpenacctH5SubmitRequestV1 request = new JftUiUserOpenacctH5SubmitRequestV1();
            request.setServiceUrl(option.Value.Url + "/ui/jft/ui/user/openacctH5/mywallet/V1");
            JftUiUserOpenacctH5MywalletRequestV1Biz bizContent = new JftUiUserOpenacctH5MywalletRequestV1Biz();
            bizContent.setAppId(option.Value.AppId);
            bizContent.setCorpNo(option.Value.CorpNo);
            bizContent.setOutUserId(wallet.OutUserID);
            bizContent.setCamsPublicKey(option.Value.CamsPublicKey);
            bizContent.setMediumId(wallet.MediumId);
            bizContent.setBindMedium(wallet.BindMedium);
            bizContent.setShowProtocolModel("1");
            bizContent.setRechargeWithdrawHidden("1");
            request.setBizContent(bizContent);
            var str = client.buildPostForm(request);
            return str;
        }


        public BankWalletAcctQuery Query(IOptions<BankWalletOption> option, Model.Live_BankWallet wallet)
        {
            DefaultIcbcClient client = new DefaultIcbcClient(
                option.Value.AppId,
                IcbcConstants.SIGN_TYPE_RSA2,
                option.Value.MyPrivateKey,
                IcbcConstants.CHARSET_UTF8,
                IcbcConstants.FORMAT_JSON,
                option.Value.ApiGWPublicKey,
                IcbcConstants.ENCRYPT_TYPE_AES,
                option.Value.AESkey,
                "", "");
            JftApiUserOpenacctH5QueryRequestV1 request = new JftApiUserOpenacctH5QueryRequestV1();
            request.setServiceUrl(option.Value.Url + "/api/jft/api/user/openacctH5/query/V1");
            JftApiUserOpenacctH5QueryRequestV1Biz bizContent = new JftApiUserOpenacctH5QueryRequestV1Biz();
            bizContent.setAppId(option.Value.AppId);
            bizContent.setOutUserId(wallet.OutUserID);
            request.setBizContent(bizContent);
            JftApiUserOpenacctH5QueryResponseV1 response;
            response = (JftApiUserOpenacctH5QueryResponseV1)client.execute(request, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString());
            if (response.getReturnCode() != 0)
            {
                BankWalletAcctQuery data = new BankWalletAcctQuery()
                {
                    Code = response.getReturnCode(),
                    Msg = response.getReturnMsg()
                };
                return data;
            }
            else
            {
                BankWalletAcctQuery data = new BankWalletAcctQuery()
                {
                    Code = response.getReturnCode(),
                    Msg = response.getReturnMsg(),
                    AppId = response.getAppId(),
                    OutUserId = response.getOutUserId(),
                    OpenacctStatus = response.getOpenacctStatus(),
                    MediumId = response.getMediumId(),
                };
                return data;
            }
        }


        public BankWalletAcctClose SendSmsBankWalletLogout(IOptions<BankWalletOption> option, Model.Live_BankWallet wallet)
        {
            var date = DateTime.Now;
            DefaultIcbcClient client = new DefaultIcbcClient(option.Value.AppId, "RSA2", option.Value.MyPrivateKey, "UTF-8", "json",
                option.Value.ApiGWPublicKey, "AES", option.Value.AESkey, "", "");
            JftWalletApiAcctCloseRequestV1 request = new JftWalletApiAcctCloseRequestV1();
            request.setServiceUrl(option.Value.Url + "/api/jft/wallet/api/acctclose/V1");
            CloseAcctSendMsgInBiz bizContent = new CloseAcctSendMsgInBiz();
            bizContent.setAppId(option.Value.AppId);
            bizContent.setCorpDate(date.ToString("yyyy-MM-dd"));
            bizContent.setCorpSerno(option.Value.AppId + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString());
            bizContent.setMediumId(wallet.MediumId);
            bizContent.setOutServiceCode("closeaccount");
            bizContent.setNotifyUrl("https://localhost:7152/api/bankwallet/Acct");
            bizContent.setTrxAccDate(date.ToString("yyyy-MM-dd"));
            bizContent.setTrxAccTime(date.ToString("HH:mm:ss"));
            bizContent.setPhoneNo(wallet.Mobile);
            bizContent.setOutUserId(wallet.OutUserID);
            request.setBizContent(bizContent);
            JftWalletApiAcctCloseResponseV1 response;
            response = (JftWalletApiAcctCloseResponseV1)client.execute(request, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString());
            //Console.WriteLine("报文：" + JSONObject.toJSONString(request));
            //Console.WriteLine("返回：" + JSONObject.toJSONString(response));
            if (response.getReturnCode() != 0)
            {
                BankWalletAcctClose data = new BankWalletAcctClose()
                {
                    Code = response.getReturnCode(),
                    Msg = response.getReturnMsg()
                };
                return data;
            }
            else
            {
                BankWalletAcctClose data = new BankWalletAcctClose()
                {
                    Code = response.getReturnCode(),
                    Msg = response.getReturnMsg(),
                    MsgID = response.getMsgId(),
                    CorpSerno = response.getCorpSerno(),
                    EventNo = response.getEventNo(),
                    SmsSendNo = response.getSmsSendNo()
                };
                return data;
            }
        }

        public BankWalletAcctCloseMsgCheck SendSmsBankWalletLogoutCheck(IOptions<BankWalletOption> option, Model.Live_BankWallet wallet)
        {
            var date = DateTime.Now;
            DefaultIcbcClient client = new DefaultIcbcClient(option.Value.AppId, "RSA2", option.Value.MyPrivateKey, "UTF-8", "json", option.Value.ApiGWPublicKey, "AES", option.Value.AESkey, "", "");
            JftWalletApiAcctCloseMsgCheckRequestV1 request = new JftWalletApiAcctCloseMsgCheckRequestV1();
            request.setServiceUrl(option.Value.Url + "/api/jft/wallet/api/acctclosemsgcheck/V1");
            CloseAcctCheckMsgInBiz bizContent = new CloseAcctCheckMsgInBiz();
            bizContent.setAppId(option.Value.AppId);
            bizContent.setCorpDate(date.ToString("yyyy-MM-dd"));
            bizContent.setCorpSerno(option.Value.AppId + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString());
            bizContent.setMediumId("6214760200652797132");
            bizContent.setOutServiceCode("scodeverify");
            bizContent.setTrxAccDate(date.ToString("yyyy-MM-dd"));
            bizContent.setTrxAccTime(date.ToString("HH:mm:ss"));
            bizContent.setOrigCorpSerno(wallet.CloseCorpSerno);
            bizContent.setPhoneMsgNo(wallet.PhoneMsgNo);
            bizContent.setSmsSendNo(wallet.SmsSendNo);
            request.setBizContent(bizContent);
            JftWalletApiAcctCloseResponseV1 response;
            response = (JftWalletApiAcctCloseResponseV1)client.execute(request, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString());
            //Console.WriteLine("报文：" + JSONObject.toJSONString(request));
            //Console.WriteLine("返回：" + JSONObject.toJSONString(response));
            if (response.getReturnCode() != 0)
            {
                BankWalletAcctCloseMsgCheck data = new BankWalletAcctCloseMsgCheck()
                {
                    Code = response.getReturnCode(),
                    Msg = response.getReturnMsg()
                };
                return data;
            }
            else
            {
                BankWalletAcctCloseMsgCheck data = new BankWalletAcctCloseMsgCheck()
                {
                    Code = response.getReturnCode(),
                    Msg = response.getReturnMsg(),
                    MsgID = response.getMsgId(),
                    CorpSerno = response.getCorpSerno(),
                    EventNo = response.getEventNo()
                };
                return data;
            }
        }
    }
}
