using LZ.Clients.Service.Interface;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Core.Services;
using LZ.Model.Enums;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using Senparc.NeuChar.Entities;
using Senparc.NeuChar.Entities.Request;
using Senparc.NeuChar.Helpers;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.MessageContexts;
using Senparc.Weixin.MP.MessageHandlers;
using System.Xml.Linq;


namespace LZ.Clients.API.Handlers
{
    public class CustomMessageHandlers : MessageHandler<DefaultMpMessageContext>
    {
        /// <summary>
        /// 为中间件提供生成当前类的委托
        /// </summary>
        public static Func<Stream, PostModel, int, CustomMessageHandlers> GenerateMessageHandler = (stream, postModel, maxRecordCount) => new CustomMessageHandlers(stream, postModel, maxRecordCount);

        public CustomMessageHandlers(XDocument requestDoc, PostModel postModel = null, int maxRecordCount = 0)
            : base(requestDoc, postModel, maxRecordCount)
        {
        }

        public CustomMessageHandlers(RequestMessageBase requestMessage, PostModel postModel = null, int maxRecordCount = 0)
            : base(requestMessage, postModel, maxRecordCount)
        {
        }
        private Model.Live_Clients client;
        private IWeiXinReplyService replyService;
        private IClientsService clientsService;
        private IWeiXinMaterialService materialService;
        private IHttpService httpService;
        private ICompanyConfigService companyConfigService;

        public CustomMessageHandlers(Stream stream, PostModel postModel = null, int maxRecordCount = 0, Model.Live_Clients cli = null)
         : base(stream, postModel, maxRecordCount)
        {
            client = cli;
            replyService = LZ.Core.ServiceLocator.Instance.GetService<IWeiXinReplyService>();
            materialService = LZ.Core.ServiceLocator.Instance.GetService<IWeiXinMaterialService>();
            httpService = LZ.Core.ServiceLocator.Instance.GetService<IHttpService>();
            companyConfigService = LZ.Core.ServiceLocator.Instance.GetService<ICompanyConfigService>();
            clientsService = LZ.Core.ServiceLocator.Instance.GetService<IClientsService>();
        }

        //public override async Task<IResponseMessageBase> OnTextOrEventRequestAsync(RequestMessageText requestMessage)
        //{
        //    Common.Log.Debug("默认");
        //    var contens = await LiveCai.Service.Shop.WXReply.QueryKeywords(requestMessage.Content, customer.ID);
        //    var responeMessage = this.CreateResponseMessage<ResponseMessageText>();
        //    responeMessage.Content = contens;
        //    return responeMessage;
        //}

        public override async Task<IResponseMessageBase> OnTextRequestAsync(RequestMessageText requestMessage)
        {
            var contens = await replyService.QueryKeywords(requestMessage.Content, client.ID);
            var model = await replyService.Get(2, client.ID);
            var defaultContent = "";
            if (model != null)
            {
                defaultContent = model.ReplyContent;
            }
            if(string.IsNullOrEmpty(contens) && (defaultContent == contens || !string.IsNullOrEmpty(defaultContent)))
            {
                var xfz = await clientsService.Get(client.CompanyID.Value, "xfz", "mini");
                try
                {
                    if (xfz != null)
                    {
                        var mediaModel = await materialService.Get(it => it.ClientID == client.ID && it.MediaType == (int)MediaType.Service);
                        if (mediaModel != null)
                        {
                            var res = await Senparc.Weixin.MP.AdvancedAPIs.CustomApi.SendMiniProgramPageAsync(client.AppID, OpenId, "点击咨询AI客服", xfz.AppID, "pageChat/index/zhixi", mediaModel.MediaID);
                            return new SuccessResponseMessage();
                        }
                        else
                        {
                            var res = await Senparc.Weixin.MP.AdvancedAPIs.CustomApi.SendMiniProgramPageAsync(client.AppID, OpenId, "点击咨询AI客服", xfz.AppID, "pageChat/index/zhixi", xfz.MediaID);
                            return new SuccessResponseMessage();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Common.StaticLog.Error(ex);
                }
            }
            var responeMessage = this.CreateResponseMessage<ResponseMessageText>();
            responeMessage.Content = contens;
            return responeMessage;
        }

        public override IResponseMessageBase OnTextRequest(RequestMessageText requestMessage)
        {

            var responseMessage =
               ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(RequestMessage);

            var requestHandler = requestMessage.StartHandler();
            requestHandler.Keyword("代理", () =>
            {
                responseMessage.Content = "收到关键字：代理";
                return responseMessage;
            })
                .SelectMenuKeyword("101", () =>
                {
                    responseMessage.Content = $"选择菜单：{requestMessage.bizmsgmenuid}，文字：{requestMessage.Content}";
                    return responseMessage;
                })
                .SelectMenuKeyword("102", () =>
                {
                    responseMessage.Content = $"选择菜单：{requestMessage.bizmsgmenuid}，文字：{requestMessage.Content}";
                    return responseMessage;
                })
                .Default(() =>
                {
                    responseMessage.Content = "文字信息";
                    return responseMessage;
                });
            return requestHandler.ResponseMessage;
        }

        public override IResponseMessageBase OnEvent_LocationSelectRequest(RequestMessageEvent_Location_Select requestMessage)
        {
            var responeMessage = this.CreateResponseMessage<ResponseMessageText>();
            responeMessage.Content = "OnEvent_LocationSelectRequest";
            return responeMessage;
        }

        public override async Task<IResponseMessageBase> OnEvent_ScanRequestAsync(RequestMessageEvent_Scan requestMessage)
        {
            string key = requestMessage.EventKey;
            var qrcodeKey = key.Replace("qrscene_", "");
            if (!string.IsNullOrEmpty(qrcodeKey))
            {
                return await ScanOrSubscribe(qrcodeKey);
            }
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            return responseMessage;
        }

        public override IResponseMessageBase OnFileRequest(RequestMessageFile requestMessage)
        {
            var responeMessage = this.CreateResponseMessage<ResponseMessageText>();
            responeMessage.Content = requestMessage.FileMd5;
            return responeMessage;
        }

        #region 微信认证事件推送

        public override IResponseMessageBase OnEvent_QualificationVerifySuccessRequest(RequestMessageEvent_QualificationVerifySuccess requestMessage)
        {
            return new SuccessResponseMessage();
        }

        public override IResponseMessageBase OnEvent_QualificationVerifyFailRequest(RequestMessageEvent_QualificationVerifyFail requestMessage)
        {
            return new SuccessResponseMessage();
        }

        public override IResponseMessageBase OnEvent_NamingVerifySuccessRequest(RequestMessageEvent_NamingVerifySuccess requestMessage)
        {
            return new SuccessResponseMessage();
        }

        public override IResponseMessageBase OnEvent_NamingVerifyFailRequest(RequestMessageEvent_NamingVerifyFail requestMessage)
        {
            return new SuccessResponseMessage();
        }

        public override IResponseMessageBase OnEvent_AnnualRenewRequest(RequestMessageEvent_AnnualRenew requestMessage)
        {
            return new SuccessResponseMessage();
        }

        public override IResponseMessageBase OnEvent_VerifyExpiredRequest(RequestMessageEvent_VerifyExpired requestMessage)
        {
            return new SuccessResponseMessage();
        }

        #endregion


        #region v1.5之后，所有的OnXX方法均从抽象方法变为虚方法，并都有默认返回消息操作，不需要处理的消息类型无需重写。

        //public override IResponseMessageBase OnLocationRequest(RequestMessageLocation requestMessage)
        //{
        //    throw new NotImplementedException();
        //}

        //public override IResponseMessageBase OnImageRequest(RequestMessageImage requestMessage)
        //{
        //    throw new NotImplementedException();
        //}

        //public override IResponseMessageBase OnVoiceRequest(RequestMessageVoice requestMessage)
        //{
        //    throw new NotImplementedException();
        //}

        //public override IResponseMessageBase OnEvent_EnterRequest(RequestMessageEvent_Enter requestMessage)
        //{
        //    throw new NotImplementedException();
        //}

        //public override IResponseMessageBase OnEvent_LocationRequest(RequestMessageEvent_Location requestMessage)
        //{
        //    throw new NotImplementedException();
        //}

        //public override IResponseMessageBase OnEvent_SubscribeRequest(RequestMessageEvent_Subscribe requestMessage)
        //{
        //    throw new NotImplementedException();
        //}

        //public override IResponseMessageBase OnEvent_UnsubscribeRequest(RequestMessageEvent_Unsubscribe requestMessage)
        //{
        //    throw new NotImplementedException();
        //}

        public override async Task<IResponseMessageBase> OnEvent_ClickRequestAsync(RequestMessageEvent_Click requestMessage)
        {
            var contens = await replyService.QueryKeywords(requestMessage.EventKey, client.ID);
            var model = await replyService.Get(2, client.ID);
            var defaultContent = "";
            if (model != null)
            {
                defaultContent = model.ReplyContent;
            }
            if (string.IsNullOrEmpty(contens) && (defaultContent == contens || !string.IsNullOrEmpty(defaultContent)))
            {
              
                var xfz = await clientsService.Get(client.CompanyID.Value, "xfz", "mini");
                try
                {
                    if (xfz!=null)
                    {
                        var mediaModel = await materialService.Get(it => it.ClientID == client.ID && it.MediaType == (int)MediaType.Service);
                        if (mediaModel != null)
                        {
                            var res = await Senparc.Weixin.MP.AdvancedAPIs.CustomApi.SendMiniProgramPageAsync(client.AppID, OpenId, "点击咨询AI客服", xfz.AppID, "pageChat/index/zhixi", mediaModel.MediaID);
                            return new SuccessResponseMessage();
                        }
                        else
                        {
                            var res = await Senparc.Weixin.MP.AdvancedAPIs.CustomApi.SendMiniProgramPageAsync(client.AppID, OpenId, "点击咨询AI客服", xfz.AppID, "pageChat/index/zhixi", xfz.MediaID);
                            return new SuccessResponseMessage();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Common.StaticLog.Error(ex);
                }
            }
            var responeMessage = this.CreateResponseMessage<ResponseMessageText>();
            responeMessage.Content = contens;
            return responeMessage;
        }

        //public override IResponseMessageBase OnLinkRequest(RequestMessageLink requestMessage)
        //{
        //    throw new NotImplementedException();
        //}

        #endregion

        /// <summary>
        /// 默认消息
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override async Task<IResponseMessageBase> DefaultResponseMessageAsync(IRequestMessageBase requestMessage)
        {
            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            var model = await replyService.Get(2, client.ID);
            if (model != null)
            {
                responseMessage.Content = model.ReplyContent;
            }
            else
            {
                responseMessage.Content = "感谢关注";
            }

            return responseMessage;
        }

        /// <summary>
        /// 默认消息
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage)
        {
            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "感谢关注";

            return responseMessage;
        }

        public override IResponseMessageBase OnUnknownTypeRequest(RequestMessageUnknownType requestMessage)
        {
            var msgType = MsgTypeHelper.GetRequestMsgTypeString(requestMessage.RequestDocument);

            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "未知消息类型：" + msgType;

            return responseMessage;
            //return base.OnUnknownTypeRequest(requestMessage);
        }
        private async Task<IResponseMessageBase> ScanOrSubscribe(string key)
        {
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            var xfz = await clientsService.Get(client.CompanyID.Value, "xfz", "mini");
            if (key.Length < 20)
            {
                var keyInfos = key.Split('|');
                var from = keyInfos[0];
                var mediaID = "";
                var page = "";
                var title = "点击注册得100积分";
                var type = keyInfos.Length > 2 ? keyInfos[2] : "";
                var pageUrl = "";
                var regFromType= keyInfos.Length > 3 ? keyInfos[3] : "";
                if (from == "reg" || from == "packreg" || from== "bannerreg")
                {
                    var res = await httpService.GetAsync(ServiceNames.UsersService, $"/api/user/getopenIdexist?openid={OpenId}&clientId={client.ID}");
                    //Common.StaticLog.Debug(JsonConvert.SerializeObject(res));
                    var isExist = res.Code == 200 && res.Data != null ? Convert.ToBoolean(res.Data) : false;
                    if (isExist)
                    {
                        title = "您已是会员！点击推荐好友注册得50积分";
                        var mediaModel= await materialService.Get(it=>it.ClientID==client.ID && it.MediaType==(int)MediaType.Recommend);
                        if (mediaModel != null)
                        {
                            mediaID = mediaModel.MediaID;
                        }
                        page = "sharereg";
                    }
                    else
                    {
                        var mediaModel = await materialService.Get(it => it.ClientID == client.ID && it.MediaType == (int)MediaType.Reg);
                        if (mediaModel != null)
                        {
                            mediaID = mediaModel.MediaID;
                        }
                        page = "reg";
                    }
                    if (from == "reg")
                    {
                        pageUrl = $"pagesUser/user/{page}?fuid={keyInfos[1]}&from={type}&mpid={OpenId}&cli_id={client.ID}&regFromType={regFromType}";
                    }
                    else if (from == "bannerreg")
                    {
                        pageUrl = $"pagesUser/user/{page}?puid={keyInfos[1]}&from=bannerreg&mpid={OpenId}&cli_id={client.ID}&regFromType={regFromType}";
                    }
                    else
                    {
                        var companyInfo= await companyConfigService.Get(client.CompanyGID.Value);
                        var puid = companyInfo!=null && companyInfo.Config.PackregUserID > 0 ? companyInfo.Config.PackregUserID.ToString() : "";
                        pageUrl = $"pagesUser/user/{page}?puid={puid}&from=packreg&mpid={OpenId}&cli_id={client.ID}&regFromType={regFromType}";
                    }
                }
                else
                {
                    pageUrl = $"pagesUser/user/reg?fuid={keyInfos[1]}&from={type}&mpid={OpenId}&cli_id={client.ID}";
                }
               responseMessage.Content = $"<a data-miniprogram-appid=\"{xfz.AppID}\" data-miniprogram-path=\"{pageUrl}\" data-miniprogram-type=\"\" href=\"http://www.xingfuzhi.cn/wx\">{title}</a>";
                try
                {
                    if (!string.IsNullOrEmpty(mediaID))
                    {
                        var res = await Senparc.Weixin.MP.AdvancedAPIs.CustomApi.SendMiniProgramPageAsync(client.AppID, OpenId, title, xfz.AppID, pageUrl, mediaID);
                        Common.StaticLog.Debug(key + ";openid=" + OpenId);
                        return new SuccessResponseMessage();
                    }

                }
                catch (Exception ex)
                {
                    Common.StaticLog.Error(ex);
                }
            }
            else
            {
                responseMessage.Content = $"<a data-miniprogram-appid=\"{xfz.AppID}\" data-miniprogram-path=\"pages/login/scanPage?from=mpscan&qr_id={key}&mpid={OpenId}&cli_id={client.ID}\" data-miniprogram-type=\"\" href=\"http://www.xingfuzhi.cn/wx\">点击填写简历</a>";
                try
                {
                    //var uploadResult = await MediaApi.UploadTemporaryMediaAsync(client.AppID, UploadMediaFileType.image, ServerUtility.ContentRootMapPath("~/wwwroot/images/mpscan.jpg"));
                    //var uploadResult = await MediaApi.UploadForeverMediaAsync(client.AppID, ServerUtility.ContentRootMapPath("~/wwwroot/images/mpscan.jpg"), UploadForeverMediaType.image);
                    // Common.StaticLog.Debug(JsonConvert.SerializeObject(uploadResult));
                    var mediaModel = await materialService.Get(it => it.ClientID == client.ID && it.MediaType == (int)MediaType.Input);
                    if (mediaModel != null && !string.IsNullOrEmpty(mediaModel.MediaID))
                    {
                        xfz.MediaID = mediaModel.MediaID;
                    }
                    if (!string.IsNullOrEmpty(xfz.MediaID))
                    {
                        var res = await Senparc.Weixin.MP.AdvancedAPIs.CustomApi.SendMiniProgramPageAsync(client.AppID, OpenId, "欢迎来到" + xfz.Name + "，请点击填写简历", xfz.AppID, $"pages/login/scanPage?from=mpscan&qr_id={key}&mpid={OpenId}&cli_id={client.ID}", xfz.MediaID);
                        return new SuccessResponseMessage();
                    }
                }
                catch (Exception ex)
                {
                    Common.StaticLog.Error(ex);
                }
            }
            return responseMessage;
        }
        public override async Task<IResponseMessageBase> OnEvent_SubscribeRequestAsync(RequestMessageEvent_Subscribe requestMessage)
        {
            string key = requestMessage.EventKey;
            var qrcodeKey = key.Replace("qrscene_", "");
           // var openid = requestMessage.FromUserName;
            var responseMessage = CreateResponseMessage<ResponseMessageText>();

            if (!string.IsNullOrEmpty(qrcodeKey))
            {
                return await ScanOrSubscribe(qrcodeKey);
            }
            else
            {
                var model = await replyService.Get(1, client.ID);
                if (model != null)
                {
                    responseMessage.Content = model.ReplyContent;
                }
                else
                {
                    responseMessage.Content = "感谢关注";
                }
            }

            return responseMessage;
        }

        //public override async Task<IResponseMessageBase> OnEvent_SubscribeRequestAsync(RequestMessageEvent_Subscribe requestMessage)
        //{
        //    string key = requestMessage.EventKey.Replace("qrscene_", "");
        //    var openid = requestMessage.FromUserName;
        //    if (!string.IsNullOrEmpty(key) && !await LiveCai.Service.Shop.ShopUser.Exist(openid))
        //    {
        //        var umodel = new Model.Shop_User();
        //        if (!await AccessTokenContainer.CheckRegisteredAsync(customer.Config.WXAppId))
        //        {
        //            await AccessTokenContainer.RegisterAsync(customer.Config.WXAppId, customer.Config.WXAppSecret, customer.Name);
        //        }
        //        var user_info =await UserApi.InfoAsync(customer.Config.WXAppId, openid);
        //        umodel.FUID = Convert.ToInt32(key);
        //        umodel.CreateTime = DateTime.Now;
        //        umodel.CustomerGID = customer.GUID;
        //        umodel.CustomerID = customer.ID;
        //        umodel.FullName = user_info.nickname;
        //        umodel.UserName = user_info.nickname;
        //        umodel.Integral = 0;
        //        umodel.Money = 0;
        //        var minModel = await ShopCustomerContaxt.GetMinLevel(umodel.CustomerID.Value);
        //        umodel.LevelID = minModel.LevelID;
        //        umodel.LeaderID = 0;
        //        umodel.IsSubscribe = true;
        //        umodel.NickName = user_info.nickname;
        //        umodel.Picture = user_info.headimgurl;
        //        umodel.Sex = user_info.sex == 1 ? "男" : (user_info.sex == 2 ? "女" : "未知");
        //        umodel.UserGID = Guid.NewGuid();
        //        umodel.City = user_info.city;
        //        umodel.Province = user_info.province;
        //        var fmodel =await Service.Shop.ShopUser.GetV(umodel.FUID.Value);
        //        if (fmodel != null)
        //        {
        //            var lmodel = await ShopCustomerContaxt.GetMaxLevel(umodel.CustomerID.Value);
        //            if (lmodel.Sort == fmodel.LevelSort)
        //            {
        //                umodel.LeaderID = fmodel.UserID;
        //            }
        //            else
        //            {
        //                umodel.LeaderID = fmodel.LeaderID;
        //            }
        //        }
        //        var formModel = new Model.Shop_UserFroms();
        //        formModel.FromID = openid;
        //        formModel.FromType = "wx";
        //       await Service.Shop.ShopUser.Add(umodel, formModel);
        //    }
        //    var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
        //    var model = await LiveCai.Service.Shop.WXReply.Get(customer.ID, 1);
        //    if (model != null)
        //    {
        //        responseMessage.Content = model.ReplyContent;
        //    }
        //    else
        //    {
        //        responseMessage.Content = "感谢关注";
        //    }
        //    return responseMessage;
        //}

        public override IResponseMessageBase OnEvent_SubscribeRequest(RequestMessageEvent_Subscribe requestMessage)
        {
            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "欢迎关注";
            return responseMessage;
        }


        #region 卡券回调测试

        public override IResponseMessageBase OnEvent_GiftCard_Pay_DoneRequest(RequestMessageEvent_GiftCard_Pay_Done requestMessage)
        {
            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "这里是 OnEvent_GiftCard_Pay_DoneRequest";
            return responseMessage;
        }

        public override IResponseMessageBase OnEvent_GiftCard_Send_To_FriendRequest(RequestMessageEvent_GiftCard_Send_To_Friend requestMessage)
        {
            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "这里是 OnEvent_GiftCard_Send_To_FriendRequest";
            return responseMessage;
        }

        public override IResponseMessageBase OnEvent_GiftCard_User_AcceptRequest(RequestMessageEvent_GiftCard_User_Accept requestMessage)
        {
            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "这里是 OnEvent_GiftCard_User_AcceptRequest";
            return responseMessage;
        }

        #endregion


        public override IResponseMessageBase OnEvent_View_Miniprogram(RequestMessageEvent_View_Miniprogram requestMessage)
        {
            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = $"小程序被访问：{requestMessage.MenuId} - {requestMessage.EventKey}";
            return responseMessage;
        }

    }
}
