using Microsoft.AspNetCore.Mvc;
using LZ.Core;
using LZ.Model.Exceptions;
using SqlSugar;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Senparc.Weixin.MP.Containers;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp;
using LZ.Core.Interface;
using LZ.Model.PagedModel;
using LZ.Clients.Service.Interface;
using LZ.Core.Attributes;

namespace LZ.Clients.API
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    public class ClientShortLinkController(IClientShortLinkService clientShortLinkService) : ClientsControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<string> GetLink(int companyId, string clientType, string pageUrl, string pageTitle)
        {
            var url = await clientShortLinkService.GetLink(companyId, clientType, pageUrl, pageTitle);
            if (url == "error")
            {
                throw new ErrorCodeException(-1, "生成链接错误");
            }
            return url;
        }
    }
}
