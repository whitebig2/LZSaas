using com.icbc.api;
using com.icbc.api.@internal.util.@internal.util.fastjson;
using com.icbc.api.request;
using com.icbc.api.response;
using java.util;
using LZ.Bank.Service.Interface;
using LZ.Core.Interface;
using LZ.Core.Services;
using LZ.Model;
using LZ.Model.Bank.Wallet;
using LZ.Model.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using static com.icbc.api.request.JftApiUserOpenacctH5QueryRequestV1;
using static com.icbc.api.request.JftUiUserOpenacctH5MywalletRequestV1;
using static com.icbc.api.request.JftUiUserOpenacctH5SubmitRequestV1;
using static com.icbc.api.request.JftWalletApiAcctCloseMsgCheckRequestV1;
using static com.icbc.api.request.JftWalletApiAcctCloseRequestV1;
namespace LZ.Bank.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    public class BankWalletController(IOptions<BankWalletOption> options, IHttpService httpService,IBankWalletService bankWalletService) : Controller
    {
        private string ApiGWPublicKey = options.Value.ApiGWPublicKey;
        private string APP_ID = options.Value.AppId;
        private string MY_PRIVATE_KEY = options.Value.MyPrivateKey;//应用私钥
        private string AESkey = options.Value.AESkey;
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Acct(Guid? gid)
        {
            
            StringBuilder str = new StringBuilder();
            //UiIcbcClient client = new UiIcbcClient(APP_ID, IcbcConstants.SIGN_TYPE_RSA2, MY_PRIVATE_KEY, IcbcConstants.CHARSET_UTF8);
            //JftUiUserOpenacctH5SubmitRequestV1 request = new JftUiUserOpenacctH5SubmitRequestV1();
            //request.setServiceUrl(options.Value.Url + "/ui/jft/ui/user/openacctH5/submit/V1");//测试地址
            //JftUiUserOpenacctH5SubmitRequestV1Biz bizContent = new JftUiUserOpenacctH5SubmitRequestV1Biz();
            //bizContent.setAppId(APP_ID);
            //bizContent.setCorpNo(options.Value.CorpNo);
            //bizContent.setOutUserId(options.Value.OutUserId);
            Live_BankWallet wallet = new Live_BankWallet();
            if (gid.HasValue)
            {
                
                try
                {
                    
                    //bizContent.setCorpSerno(gid.Value.ToString());
                    //Model.Live_BankWallet wallet = new Model.Live_BankWallet();
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    dic.Add("gid", gid.Value.ToString());
                    var result = await httpService.GetAsync(ServiceNames.UsersService, "/api/BankWallet/get", dic);
                    var ii = JsonConvert.DeserializeObject(result.Data.ToString()) as JObject;
                    //var ii = JsonConvert.DeserializeObject(obj.Data.ToString()) as JObject;
                    wallet = JsonConvert.DeserializeObject<Model.Live_BankWallet>(ii.ToString());
                    

                }
                catch (Exception e)
                {
                    str.AppendFormat("<div class=\"msg\">{0}</div>", e.ToString());
                    ViewBag.Html = str.ToString();
                    return View();
                }
            }
            else
            {
                wallet.OutUserID = "LB0100011";
                wallet.GID = Guid.NewGuid();
                wallet.Name = "邱迂";
                wallet.SignDate = "2007-10-21";
                wallet.ValidityPeriod = "2027-10-21";
                wallet.CertNo = "110649195708117394";
                wallet.Address = "北京市海淀区西三旗街道西三旗建材城东路6号中国工商银行数据中心";
                wallet.BindMedium = "6212260200070532094";
                wallet.Mobile = "13801216127";
            }

            var html = bankWalletService.Acct(options, wallet);

            //str.Append(html);
            //Console.WriteLine("报文：" + str);
            ViewBag.Html = html;
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> MyWallet(Guid? gid)
        {
            StringBuilder str = new StringBuilder();
            if (gid.HasValue)
            {
                string APP_ID = options.Value.AppId;
                string MY_PRIVATE_KEY = options.Value.MyPrivateKey;//应用私钥
                try
                {
                    Model.Live_BankWallet wallet = new Model.Live_BankWallet();
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    dic.Add("gid", gid.Value.ToString());
                    var result = await httpService.GetAsync(ServiceNames.UsersService, "/api/BankWallet/get", dic);
                    var ii = JsonConvert.DeserializeObject(result.Data.ToString()) as JObject;
                    wallet = JsonConvert.DeserializeObject<Model.Live_BankWallet>(ii.ToString());

                    //UiIcbcClient client = new UiIcbcClient(APP_ID, IcbcConstants.SIGN_TYPE_RSA2, MY_PRIVATE_KEY, IcbcConstants.CHARSET_UTF8);
                    //JftUiUserOpenacctH5SubmitRequestV1 request = new JftUiUserOpenacctH5SubmitRequestV1();
                    //request.setServiceUrl(options.Value.Url + "/ui/jft/ui/user/openacctH5/mywallet/V1");
                    //JftUiUserOpenacctH5MywalletRequestV1Biz bizContent = new JftUiUserOpenacctH5MywalletRequestV1Biz();
                    //bizContent.setAppId(APP_ID);
                    //bizContent.setCorpNo(options.Value.CorpNo);
                    //bizContent.setOutUserId(wallet.OutUserID);
                    //bizContent.setCamsPublicKey(options.Value.CamsPublicKey);
                    //bizContent.setMediumId(wallet.MediumId);
                    //bizContent.setBindMedium(wallet.BindMedium);
                    //bizContent.setShowProtocolModel("1");
                    //bizContent.setRechargeWithdrawHidden("1");
                    //request.setBizContent(bizContent);
                    //str.Append(client.buildPostForm(request));
                    var s = bankWalletService.MyWallet(options, wallet);
                    ViewBag.Html = s.ToString();
                    return View();

                }
                catch (Exception e)
                {
                    str.AppendFormat("<div class=\"msg\">{0}</div>", e.ToString());
                    ViewBag.Html = str.ToString();
                    return View();
                }
            }
            else
            {
                str.AppendFormat("<div class=\"msg\">{0}</div>", "未传参数");
                ViewBag.Html = str.ToString();
                return View();
            }



        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> OpenAcctResult()
        {
            //var request = HttpContext.Request;
            //Stream stream = request.Body;
            //byte[] buffer = new byte[request.ContentLength.Value];
            //await stream.ReadAsync(buffer, 0, buffer.Length);
            //var json = Encoding.UTF8.GetString(buffer);
            //var data = JsonSerializer.Deserialize(json);
            //ViewBag.Html = aa.ToString();


            //using var reader = new StreamReader(HttpContext.Request.Body, Encoding.UTF8);
            //var json = await reader.ReadToEndAsync();
            //var data = JsonConvert.DeserializeObject<OpenAcctModel>(json); // System
            //Map param = new HashMap();
            //param.put("from", data);
            //param.put("api", api);
            //param.put("app_id", app_id);
            //param.put("charset", charset);
            //param.put("format", format);
            //param.put("encrypt_type", encrypt_type);
            //param.put("timestamp", timestamp);
            //param.put("biz_content", biz_content);
            //param.put("sign_type", sign_type);//目前上行网关签名暂时仅支持RSA
            ///**********验证工行上行网关RSA签名**********/
            //String signStr = WebUtils.buildOrderedSignStr(path, param);
            //bool flag = IcbcSignature.verify(signStr, sign_type, ApiGWPublicKey,
            //charset, sign);

            return View();

        }
       
        [AllowAnonymous]
        [HttpPost]
        public async Task<BankWalletAcctQuery> Query([FromBody] Model.Live_BankWallet wallet)
        {
            //DefaultIcbcClient client = new DefaultIcbcClient(
            //    APP_ID,
            //    IcbcConstants.SIGN_TYPE_RSA2,
            //    MY_PRIVATE_KEY,
            //    IcbcConstants.CHARSET_UTF8,
            //    IcbcConstants.FORMAT_JSON,
            //    ApiGWPublicKey,
            //    IcbcConstants.ENCRYPT_TYPE_AES,
            //    AESkey,
            //    "", "");
            //JftApiUserOpenacctH5QueryRequestV1 request = new JftApiUserOpenacctH5QueryRequestV1();
            //request.setServiceUrl(options.Value.Url + "/api/jft/api/user/openacctH5/query/V1");
            //JftApiUserOpenacctH5QueryRequestV1Biz bizContent = new JftApiUserOpenacctH5QueryRequestV1Biz();
            //bizContent.setAppId(APP_ID);
            //bizContent.setOutUserId(wallet.OutUserID);
            //request.setBizContent(bizContent);
            //JftApiUserOpenacctH5QueryResponseV1 response;
            //response = (JftApiUserOpenacctH5QueryResponseV1)client.execute(request, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString());
            //if (response.getReturnCode() != 0)
            //{
            //    BankWalletAcctQuery data = new BankWalletAcctQuery()
            //    {
            //        Code = response.getReturnCode(),
            //        Msg = response.getReturnMsg()
            //    };
            //    return data;
            //}
            //else
            //{
            //    BankWalletAcctQuery data = new BankWalletAcctQuery()
            //    {
            //        Code = response.getReturnCode(),
            //        Msg = response.getReturnMsg(),
            //        AppId = response.getAppId(),
            //        OutUserId = response.getOutUserId(),
            //        OpenacctStatus = response.getOpenacctStatus(),
            //        MediumId = response.getMediumId(),
            //    };
            //    return data;
            //}

            return bankWalletService.Query(options, wallet);

        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<BankWalletAcctQuery> QueryTest()
        {
            DefaultIcbcClient client = new DefaultIcbcClient(
                APP_ID,
                IcbcConstants.SIGN_TYPE_RSA2,
                MY_PRIVATE_KEY,
                IcbcConstants.CHARSET_UTF8,
                IcbcConstants.FORMAT_JSON,
                ApiGWPublicKey,
                IcbcConstants.ENCRYPT_TYPE_AES,
                AESkey,
                "", "");
            JftApiUserOpenacctH5QueryRequestV1 request = new JftApiUserOpenacctH5QueryRequestV1();
            request.setServiceUrl(options.Value.Url + "/api/jft/api/user/openacctH5/query/V1");
            JftApiUserOpenacctH5QueryRequestV1Biz bizContent = new JftApiUserOpenacctH5QueryRequestV1Biz();
            bizContent.setAppId(APP_ID);
            bizContent.setOutUserId("LB20260903143622");
            request.setBizContent(bizContent);
            JftApiUserOpenacctH5QueryResponseV1 response;
            response = (JftApiUserOpenacctH5QueryResponseV1)client.execute(request, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString());
            Console.WriteLine("报文：" + JSONObject.toJSONString(request));
            Console.WriteLine("返回：" + JSONObject.toJSONString(response));
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


        #region e钱包解绑
        //发送验证码
        [AllowAnonymous]
        [HttpPost]
        public async Task<BankWalletAcctClose> SendSmsBankWalletLogout([FromBody] Live_BankWallet wallet)
        {
            //var date = DateTime.Now;
            //DefaultIcbcClient client = new DefaultIcbcClient(APP_ID, "RSA2", MY_PRIVATE_KEY, "UTF-8", "json",
            //    ApiGWPublicKey, "AES", AESkey, "", "");
            //JftWalletApiAcctCloseRequestV1 request = new JftWalletApiAcctCloseRequestV1();
            //request.setServiceUrl(options.Value.Url + "/api/jft/wallet/api/acctclose/V1");
            //CloseAcctSendMsgInBiz bizContent = new CloseAcctSendMsgInBiz();
            //bizContent.setAppId(APP_ID);
            //bizContent.setCorpDate(date.ToString("yyyy-MM-dd"));
            //bizContent.setCorpSerno(APP_ID + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString());
            //bizContent.setMediumId(wallet.MediumId);
            //bizContent.setOutServiceCode("closeaccount");
            //bizContent.setNotifyUrl("https://localhost:7152/api/bankwallet/Acct");
            //bizContent.setTrxAccDate(date.ToString("yyyy-MM-dd"));
            //bizContent.setTrxAccTime(date.ToString("HH:mm:ss"));
            //bizContent.setPhoneNo(wallet.Mobile);
            //bizContent.setOutUserId(wallet.OutUserID);
            //request.setBizContent(bizContent);
            //JftWalletApiAcctCloseResponseV1 response;
            //response = (JftWalletApiAcctCloseResponseV1)client.execute(request, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString());
            ////Console.WriteLine("报文：" + JSONObject.toJSONString(request));
            ////Console.WriteLine("返回：" + JSONObject.toJSONString(response));
            //if (response.getReturnCode() != 0)
            //{
            //    BankWalletAcctClose data = new BankWalletAcctClose()
            //    {
            //        Code = response.getReturnCode(),
            //        Msg = response.getReturnMsg()
            //    };
            //    return data;
            //}
            //else
            //{
            //    BankWalletAcctClose data = new BankWalletAcctClose()
            //    {
            //        Code = response.getReturnCode(),
            //        Msg = response.getReturnMsg(),
            //        MsgID = response.getMsgId(),
            //        CorpSerno = response.getCorpSerno(),
            //        EventNo = response.getEventNo(),
            //        SmsSendNo = response.getSmsSendNo()
            //    };
            //    return data;
            //}
            return bankWalletService.SendSmsBankWalletLogout(options, wallet);

        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<BankWalletAcctClose> SendSmsBankWalletLogoutTest()
        {
            var date = DateTime.Now;
            DefaultIcbcClient client = new DefaultIcbcClient(APP_ID, "RSA2", MY_PRIVATE_KEY, "UTF-8", "json",
                ApiGWPublicKey, "AES", AESkey, "", "");
            JftWalletApiAcctCloseRequestV1 request = new JftWalletApiAcctCloseRequestV1();
            request.setServiceUrl(options.Value.Url + "/api/jft/wallet/api/acctclose/V1");
            CloseAcctSendMsgInBiz bizContent = new CloseAcctSendMsgInBiz();
            bizContent.setAppId(APP_ID);
            bizContent.setCorpDate(date.ToString("yyyy-MM-dd"));
            bizContent.setCorpSerno(APP_ID + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString());
            bizContent.setMediumId("6212260200070532094");
            bizContent.setOutServiceCode("closeaccount");
            bizContent.setNotifyUrl("https://localhost:7152/api/bankwallet/Acct");
            bizContent.setTrxAccDate(date.ToString("yyyy-MM-dd"));
            bizContent.setTrxAccTime(date.ToString("HH:mm:ss"));
            bizContent.setPhoneNo("13801216127");
            bizContent.setOutUserId("LB20260903143622");
            request.setBizContent(bizContent);
            JftWalletApiAcctCloseResponseV1 response;
            response = (JftWalletApiAcctCloseResponseV1)client.execute(request, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString());
            Console.WriteLine("报文：" + JSONObject.toJSONString(request));
            Console.WriteLine("返回：" + JSONObject.toJSONString(response));
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


        //注销e钱包校对验证码

        [AllowAnonymous]
        [HttpPost]
        public async Task<BankWalletAcctCloseMsgCheck> SendSmsBankWalletLogoutCheck([FromBody] Model.Live_BankWallet wallet)
        {
            var date = DateTime.Now;
            if (wallet == null)
            {
                BankWalletAcctCloseMsgCheck data = new BankWalletAcctCloseMsgCheck()
                {
                    Code = -1,
                    Msg = "参数错误"
                };
                return data;
            }
            if(string.IsNullOrEmpty(wallet.CloseCorpSerno) || string.IsNullOrEmpty(wallet.SmsSendNo) || string.IsNullOrEmpty(wallet.PhoneMsgNo))
            {
                BankWalletAcctCloseMsgCheck data = new BankWalletAcctCloseMsgCheck()
                {
                    Code = -1,
                    Msg = "注销参数信息不全"
                };
                return data;
            }

            return bankWalletService.SendSmsBankWalletLogoutCheck(options, wallet);
            //DefaultIcbcClient client = new DefaultIcbcClient(APP_ID, "RSA2", MY_PRIVATE_KEY, "UTF-8", "json", ApiGWPublicKey, "AES", AESkey, "", "");
            //JftWalletApiAcctCloseMsgCheckRequestV1 request = new JftWalletApiAcctCloseMsgCheckRequestV1();
            //request.setServiceUrl(options.Value.Url + "/api/jft/wallet/api/acctclosemsgcheck/V1");
            //CloseAcctCheckMsgInBiz bizContent = new CloseAcctCheckMsgInBiz();
            //bizContent.setAppId(APP_ID);
            //bizContent.setCorpDate(date.ToString("yyyy-MM-dd"));
            //bizContent.setCorpSerno(APP_ID + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString());
            //bizContent.setMediumId("6214760200652797132");
            //bizContent.setOutServiceCode("scodeverify");
            //bizContent.setTrxAccDate(date.ToString("yyyy-MM-dd"));
            //bizContent.setTrxAccTime(date.ToString("HH:mm:ss"));
            //bizContent.setOrigCorpSerno(wallet.CloseCorpSerno);
            //bizContent.setPhoneMsgNo(wallet.PhoneMsgNo);
            //bizContent.setSmsSendNo(wallet.SmsSendNo);
            //request.setBizContent(bizContent);
            //JftWalletApiAcctCloseResponseV1 response;
            //response = (JftWalletApiAcctCloseResponseV1)client.execute(request, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString());
            ////Console.WriteLine("报文：" + JSONObject.toJSONString(request));
            ////Console.WriteLine("返回：" + JSONObject.toJSONString(response));
            //if (response.getReturnCode() != 0)
            //{
            //    BankWalletAcctCloseMsgCheck data = new BankWalletAcctCloseMsgCheck()
            //    {
            //        Code = response.getReturnCode(),
            //        Msg = response.getReturnMsg()
            //    };
            //    return data;
            //}
            //else
            //{
            //    BankWalletAcctCloseMsgCheck data = new BankWalletAcctCloseMsgCheck()
            //    {
            //        Code = response.getReturnCode(),
            //        Msg = response.getReturnMsg(),
            //        MsgID = response.getMsgId(),
            //        CorpSerno = response.getCorpSerno(),
            //        EventNo = response.getEventNo()
            //    };
            //    return data;
            //}


        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<BankWalletFinStatusQuery> QueryFinStatus([FromBody] Model.Live_BankWallet wallet)
        {
            var date = DateTime.Now;
            if (wallet == null)
            {
                BankWalletFinStatusQuery data = new BankWalletFinStatusQuery()
                {
                    Code = -1,
                    Msg = "参数错误"
                };
                return data;
            }
            if (string.IsNullOrEmpty(wallet.CloseCorpSerno))
            {
                BankWalletFinStatusQuery data = new BankWalletFinStatusQuery()
                {
                    Code = -1,
                    Msg = "销户发送验证码的合作方交易单号不存在"
                };
                return data;
            }


            return bankWalletService.QueryFinStatus(options, wallet);


        }


        #endregion

    }
}
