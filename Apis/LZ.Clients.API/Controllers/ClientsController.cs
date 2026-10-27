using LZ.ApiService.Interface;
using LZ.ApiService.Services;
using LZ.Clients.Service.Interface;
using LZ.Core;
using LZ.Core.Attributes;
using LZ.Core.EventData;
using LZ.Core.Interface;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Core.Services;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.NodeServices;
using Newtonsoft.Json;
using Senparc.CO2NET.Utilities;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.Containers;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp;
using SqlSugar;

namespace LZ.Clients.API
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    public class ClientsController(IClientsService clientsService, ICompanyConfigService companyConfigService, INodeServices nodeServices, IEventPublisher publisher,IClientPageService clientPageService,IClientMyTemplateService clientMyTemplateService,IAliyunOSSService aliyunOSSService) : ClientsControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] PagedModelBase query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Live_V_Clients>();
          
            if (!string.IsNullOrEmpty(query.Name))
            {
                expable.And(it => it.Name.Contains(query.Name));
            }
            if (user.PlatformType == Model.Enums.PlatformType.Admin)
            {
                if (query.CID.HasValue)
                {
                    expable.And(it => it.CompanyID == query.CID);
                }
            }
            else
            {
                expable.And(it => it.CompanyID == user.CompanyID);
            }
            var exp = expable.ToExpression();
            var models = await clientsService.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }
        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadCert()
        {
            var files = Request.Form.Files;
            if (files.Count > 0)
            {
                var path =await Common.UploadFile.UpFile(files[0], "/files/keys",false);
                //var res = await aliyunOSSService.UploadFile(files[0], "files/keys");
                if (path.IndexOf("error")==-1)
                {
                    return Ok(path);
                }
                else
                {
                    throw new ErrorCodeException(-1, path);
                }
            }
            else
            {
                throw new ErrorCodeException(-1, "请选择文件");
            }
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Model.Live_Clients model)
        {
            if(await clientsService.Exist(model.AppID))
            {
                throw new ErrorCodeException(-1, "该appid已存在，请勿重复新增!");
            }
            model.GID = Guid.NewGuid();
            model.ReleaseStatus = 1;
            model.AppConfig = "{\"header\":{\"backgroundColor\":\"#07B4DF\",\"color\":\"#ffffff\"}}";
            var id = await clientsService.Add(model);
            if (id > 0)
            {
                var types = new string[] { "mp", "mini" };
                if (types.Contains(model.AppType))//注册微信
                {
                    if (model.AppType == "mini")
                    {
                        if (!await Senparc.Weixin.WxOpen.Containers.AccessTokenContainer.CheckRegisteredAsync(model.AppID))
                        {
                            await Senparc.Weixin.WxOpen.Containers.AccessTokenContainer.RegisterAsync(model.AppID, model.AppSecret, model.Name);
                        }
                    }
                    else
                    {
                        if (!await Senparc.Weixin.MP.Containers.AccessTokenContainer.CheckRegisteredAsync(model.AppID))
                        {
                            await Senparc.Weixin.MP.Containers.AccessTokenContainer.RegisterAsync(model.AppID, model.AppSecret, model.Name);
                        }
                    }
                    
                    if (model.Type == "xfz")
                    {
                        await clientMyTemplateService.AddToClient(id);//将默认模板复制到自己模板库中
                        //新增订阅消息
                        var record = new NoticeTempEventData();
                        record.ClientID = id;
                        record.Type = model.Type;
                        publisher.Publish(record);
                    }
                }
            }
            return Ok();
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> test()
        {
            try
            {
                var clientsModel = await clientsService.Get(1, "xfz","mini");
                var qrcode = "/qrcode/" + Guid.NewGuid().ToString("N") + ".png";
                var path = Directory.GetCurrentDirectory() + "/uploadfile/" + qrcode;
                var result = await WxAppApi.GetWxaCodeAsync(clientsModel.AppID, path, "pages/userinfo/index?userid=1&customerid=1&companyid=1", 700);
                Common.StaticLog.Debug(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex) { 
                Common.StaticLog.Error(ex);
            }
            return Ok();
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<Model.CompanyModel> GetCompanyConfig()
        {
            var guid = HttpContext.Request.Headers["company-guid"].ToString();
            if (string.IsNullOrEmpty(guid))
            {
                throw new ErrorCodeException(-1, "来源错误");
            }
            var config = await companyConfigService.Get(new Guid(guid));
            return config;
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<Model.Live_ClientPage> GetClientPage(string type)
        {
            var id = HttpContext.Request.Headers["client-id"].ToString();
            var model = await clientPageService.GetPage(Convert.ToInt32(id), type);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "该页面数据不存在");
            }
            return model;
        }
        [HttpGet]
        public async Task<Model.Live_Clients> GetClient(string appid)
        {
            var model = await clientsService.Get(appid);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }
        [PlatformLimit(Model.Enums.PlatformType.All)]
        [HttpGet]
        public async Task<Model.Live_Clients> GetType(int companyId,string type,string appType)
        {
            var user = CurrentUser;
            if (user.PlatformType != Model.Enums.PlatformType.Admin && companyId > 0)
            {
                companyId = user.CompanyID;
            }
            var model = await clientsService.Get(companyId, type, appType);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "该页面数据不存在");
            }
            return model;
        }
        [HttpGet]
        public async Task<Model.Live_Clients> Get(int id)
        {
           
            var model = await clientsService.Get(id);
            if (model==null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            //if (model.AppType == "mp")
            //{
            //    var uploadResult = await MediaApi.UploadForeverMediaAsync(model.AppID, ServerUtility.ContentRootMapPath("~/wwwroot/images/mpscan.png"), UploadForeverMediaType.image);
            //    Common.StaticLog.Debug(JsonConvert.SerializeObject(uploadResult));
            //}
            
            return model;
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_Clients model)
        {
            if (await clientsService.Exist(model.AppID,model.ID))
            {
                throw new ErrorCodeException(-1, "该appid已存在，请勿重复!");
            }
            if (!await clientsService.Exist(model.ID))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
           var has = await clientsService.Update(model);
            if (has)
            {
                var types = new string[] { "mp", "mini" };
                if (types.Contains(model.AppType))//注册微信
                {
                    if (model.AppType == "mini")
                    {
                        if (!await Senparc.Weixin.WxOpen.Containers.AccessTokenContainer.CheckRegisteredAsync(model.AppID))
                        {
                            await Senparc.Weixin.WxOpen.Containers.AccessTokenContainer.RegisterAsync(model.AppID, model.AppSecret, model.Name);
                        }
                    }
                    else
                    {
                        if (!await Senparc.Weixin.MP.Containers.AccessTokenContainer.CheckRegisteredAsync(model.AppID))
                        {
                            await Senparc.Weixin.MP.Containers.AccessTokenContainer.RegisterAsync(model.AppID, model.AppSecret, model.Name);
                        }
                    }
                }
            }
            return Ok();
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [HttpPut]
        public async Task<IActionResult> Enable([FromForm] int id)
        {
            if (!await clientsService.Exist(id))
            {
                throw new ErrorCodeException(-1, "你要启用的数据不存在");
            }
            await clientsService.Set(id, true);
            return Ok();
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [HttpPut]
        public async Task<IActionResult> Disable([FromForm] int id)
        {
            if (!await clientsService.Exist(id))
            {
                throw new ErrorCodeException(-1, "你要禁用的数据不存在");
            }
            await clientsService.Set(id, false);
            return Ok();
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            if (!await clientsService.Exist(id))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            await clientsService.Delete(id);
            return Ok();
        }
        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<string> CreateQrcode([FromForm] int companyId, [FromForm] string type, [FromForm] string appType, [FromForm] string page)
        {
            var model = await clientsService.Get(companyId, type, appType);
            
            if (model == null)
            {
                throw new ErrorCodeException(-1, "该应用不存在");
            }
            var timeName = DateTime.Now.ToString("yyyy/MM/dd");
            var qrcode = "qrcode/" + timeName + "/" + Guid.NewGuid().ToString("N") + ".png";
            var path = Directory.GetCurrentDirectory() + "/uploadfile/" + qrcode;
            var funPath = Directory.GetCurrentDirectory() + "/uploadfile/qrcode/" + timeName;
            if (!Directory.Exists(funPath))
            {
                Directory.CreateDirectory(funPath);
            }
            var result = await WxAppApi.GetWxaCodeAsync(model.AppID, path, page, 700);
            var qrcodepath = await aliyunOSSService.UploadFile(path, qrcode);
            if (qrcodepath != "error")
            {

                return "/"+qrcode;
            }
            else
            {
                throw new ErrorCodeException(-1, "上传二维码错误");
            }
        }
        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<string> CreateInQrcode([FromForm] int clientId, [FromForm] string paras)
        {
            var model = await clientsService.Get(clientId);

            if (model == null)
            {
                throw new ErrorCodeException(-1, "该应用不存在");
            }
            var timeName = DateTime.Now.ToString("yyyy/MM/dd");
            var qrcode = "qrcode/" + timeName + "/" + Guid.NewGuid().ToString("N") + ".png";
            var path = Directory.GetCurrentDirectory() + "/uploadfile/" + qrcode;
            var funPath = Directory.GetCurrentDirectory() + "/uploadfile/qrcode/" + timeName;
            if (!Directory.Exists(funPath))
            {
                Directory.CreateDirectory(funPath);
            }
            if (model.AppType == "mini")
            {
                var result = await WxAppApi.GetWxaCodeAsync(model.AppID, path, paras, 700);
            }
            else
            {
                var result = await QrCodeApi.CreateAsync(model.AppID, 100, 0, QrCode_ActionName.QR_LIMIT_STR_SCENE, paras);
                if (result.errcode == 0)
                {
                    using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
                    {
                        QrCodeApi.ShowQrCode(result.ticket, fs);//下载二维码
                    }
                }
                else
                {
                    Common.StaticLog.Error("公众号二维码生成错误！");
                    throw new ErrorCodeException(-1, "公众号二维码生成错误");
                }
            }
            var qrcodepath = await aliyunOSSService.UploadFile(path, qrcode);
            if (qrcodepath != "error")
            {

                return "/" + qrcode;
            }
            else
            {
                throw new ErrorCodeException(-1, "上传二维码错误");
            }
        }
        /// <summary>
        /// 密钥和文件需要绝对路径
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        [HttpPost]
        public async Task<IActionResult> Release([FromForm] int id)
        {
            var model=await clientsService.Get(id);
            if (model==null)
            {
                throw new ErrorCodeException(-1, "你要发布的应用不存在");
            }
            if (model.ReleaseStatus==3)
            {
                throw new ErrorCodeException(-1, "您的应用在发布中");
            }
            publisher.Publish(new ClientReleaseEventData()
            {
                ClientID = id
            });
            await clientsService.ChangeStatus(id, 3);
            return Ok();
        }
        public async Task<IActionResult> Release1([FromForm] int id)
        {
            var model = await clientsService.Get(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要发布的应用不存在");
            }
            try
            {
                // string res = await nodeServices.InvokeAsync<string>("D:/ME/Business/2024/SaaS-miniprogram/index.js", model.AppID, model.ProjectPath, model.PrivateKeyPath, model.Version).ConfigureAwait(false);
                string res = await nodeServices.InvokeAsync<string>("http://127.0.0.1:8891/index.js", model.AppID, model.ProjectPath, model.PrivateKeyPath, model.Version).ConfigureAwait(false);
                Common.StaticLog.Debug(res);
                return Ok(res);
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
                throw new ErrorCodeException(-1, "发布应用失败！");
            }
        }

        public async Task<IActionResult> IsExit ()
        {
            

            //var path = "/app/minis/common";
            //List<string> files = new List<string>();
            //if (Directory.Exists(path))
            //{
            //    foreach (string file in Directory.GetFiles(path))
            //    {
            //        files.Add(file);
            //    }
            //}
            try
            {
                var clientsModel = await clientsService.Get(1, "xfz", "mini");
                var qrcode =Guid.NewGuid().ToString("N") + ".png";
                var path = Directory.GetCurrentDirectory() + "/wwwroot/" + qrcode;
                var result = await WxAppApi.GetWxaCodeAsync(clientsModel.AppID, path, "pages/userinfo/index?userid=1&customerid=1&companyid=1", 700);
                Common.StaticLog.Debug(JsonConvert.SerializeObject(result));
                return Ok(new { clientsModel, qrcode, path, result });
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
                throw new ErrorCodeException(-1, ex.ToString());
            }
            
        }
    }
}
