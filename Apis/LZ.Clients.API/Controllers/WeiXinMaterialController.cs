using LZ.ApiService.Interface;
using LZ.ApiService.Services;
using LZ.Clients.Service.Interface;
using LZ.Core;
using LZ.Core.Attributes;
using LZ.Core.Interface;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.NodeServices;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
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
    public class WeiXinMaterialController(IWeiXinMaterialService weiXinMaterialService,IClientsService clientsService,IAliyunOSSService aliyunOSSService) : ClientsControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] PagedModelBase query)
        {
            var user= CurrentUser;
            var expable = Expressionable.Create<Model.Live_WeiXinMaterial>();
            if (query.CID.HasValue)
            {
                expable.And(it => it.ClientID == query.CID);
            }
            var exp = expable.ToExpression();
            var models = await weiXinMaterialService.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }
        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadFile()
        {
            var files = Request.Form.Files;
            if (files.Count > 0)
            {
                var res = await aliyunOSSService.UploadFile(files[0]);
                if (res.Code == 0)
                {
                    return Ok(res.Data);
                }
                else
                {
                    throw new ErrorCodeException(-1, res.Message);
                }
            }
            else
            {
                throw new ErrorCodeException(-1, "请选择文件");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Model.Live_WeiXinMaterial model)
        {
            var user = CurrentUser;
            if (string.IsNullOrEmpty(model.Path))
            {
                throw new ErrorCodeException(-1, "文件地址为空");
            }
            var client = await clientsService.Get(model.ClientID.Value);
            if (client == null)
            {
                throw new ErrorCodeException(-1, "该客户端不存在");
            }
            try
            {
                var uploadResult = await MediaApi.UploadForeverMediaAsync(client.AppID, ServerUtility.ContentRootMapPath($"~/uploadfiles{model.Path}"), UploadForeverMediaType.image);
                Common.StaticLog.Debug(JsonConvert.SerializeObject(uploadResult));
                if (uploadResult.errcode == 0)
                {
                    model.MediaID = uploadResult.media_id;
                    model.CreateTime = DateTime.Now;
                    var count = await weiXinMaterialService.Add(model);
                    return Ok();
                }
                else
                {
                    throw new ErrorCodeException(-1, uploadResult.errmsg);
                }
            }
            catch(Exception ex)
            {
                Common.StaticLog.Error(ex);
                throw new ErrorCodeException(-1, ex.Message);
            }
        }
        [HttpGet]
        public async Task<Model.Live_WeiXinMaterial> Get(int id)
        {
            var model = await weiXinMaterialService.Get(id);
            if (model==null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_WeiXinMaterial model)
        {
            var m = await weiXinMaterialService.Get(model.ID);
            if (m == null)
            {
                throw new ErrorCodeException(-1, "该页面数据不存在");
            }
            var has = await weiXinMaterialService.Update(m);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            var m = await weiXinMaterialService.Get(id);
            if (m == null)
            {
                throw new ErrorCodeException(-1, "该素材不存在");
            }
            var client = await clientsService.Get(m.ClientID.Value);
            if (client == null)
            {
                throw new ErrorCodeException(-1, "该客户端不存在");
            }
            try
            {
                if (!string.IsNullOrEmpty(m.MediaID))
                {
                    await MediaApi.DeleteForeverMediaAsync(client.AppID, m.MediaID);
                }
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
            }
            await weiXinMaterialService.Delete(id);
            return Ok();
        }
    }
}
