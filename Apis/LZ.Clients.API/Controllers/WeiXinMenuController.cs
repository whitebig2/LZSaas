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
using LZ.Clients.Service;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.Entities.Menu;
using System.Drawing.Drawing2D;

namespace LZ.Clients.API
{
    public class WeiXinMenuController(IWeiXinMenuService weiXinMenuService,IClientsService clientsService ) : ClientsControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] UserPagedModel query)
        {
            var expable = Expressionable.Create<Model.Live_WeiXinMenu>();
            if (query.CID.HasValue)
            {
                expable.And(it => it.ClientID == query.CID);
            }
            if (!string.IsNullOrEmpty(query.Type))
            {
                expable.And(it => it.MType == Convert.ToInt32(query.Type));
            }
            if (!string.IsNullOrEmpty(query.Name))
            {
                expable.And(it => it.Name.Contains(query.Name));
            }
            var exp = expable.ToExpression();
            var models = await weiXinMenuService.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Model.Live_WeiXinMenu model)
        {
            var count = await weiXinMenuService.Add(model);
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> Send([FromForm] int id)
        {
            var expable = Expressionable.Create<Model.Live_WeiXinMenu>();
            expable.And(it => it.ClientID == id);
            var exp = expable.ToExpression();
            var query = new PagedModelBase();
            query.Page = 0;
            var models = await weiXinMenuService.List(exp, query);
            if (models.Count > 0)
            {
                var mains = models.Where(it => it.PID == 0).Take(3).OrderBy(it=>it.Sort);
                ButtonGroup bg = new ButtonGroup();
                foreach (var m in mains)
                {
                    switch (m.MType)
                    {
                        case 1:
                            {
                                var smodels = models.Where(it => it.PID == m.ID).OrderBy(it => it.Sort).Take(5);
                                var subButton = new SubButton()
                                {
                                    name = m.Name
                                };
                                foreach (var sm in smodels)
                                {
                                    switch (sm.MType)
                                    {
                                        case 2:
                                            subButton.sub_button.Add(new SingleClickButton()
                                            {
                                                name = sm.Name,
                                                key = sm.Keyword
                                            });
                                            break;
                                        case 3:
                                            subButton.sub_button.Add(new SingleViewButton()
                                            {
                                                name = sm.Name,
                                                url = sm.Url
                                            });
                                            break;
                                        case 4:
                                            subButton.sub_button.Add(new SingleMiniProgramButton()
                                            {
                                                appid = sm.MiniAppID,
                                                name = sm.Name,
                                                url = sm.Url,
                                                pagepath = sm.PagePath
                                            });
                                            break;
                                    }
                                }
                                if (subButton.sub_button.Count > 0)
                                {
                                    bg.button.Add(subButton);
                                }
                            }
                            break;
                        case 2:
                            bg.button.Add(new SingleClickButton()
                            {
                                name = m.Name,
                                key = m.Keyword
                            });
                            break;
                        case 3:
                            bg.button.Add(new SingleViewButton()
                            {
                                name = m.Name,
                                url = m.Url
                            });
                            break;
                        case 4:
                            bg.button.Add(new SingleMiniProgramButton()
                            {
                                appid = m.MiniAppID,
                                name = m.Name,
                                url = m.Url,
                                pagepath = m.PagePath
                            });
                            break;
                    }
                }
                if (bg.button.Count > 0)
                {
                    var model = await clientsService.Get(id);
                    var result = CommonApi.CreateMenu(model.AppID, bg);
                    Common.StaticLog.Debug(result);
                }
            }
            return Ok();
        }
        [HttpGet]
        public async Task<Model.Live_WeiXinMenu> Get(int id)
        {
            var model = await weiXinMenuService.Get(id);
            if (model==null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_WeiXinMenu model)
        {
           var has = await weiXinMenuService.Update(model);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            if (!await weiXinMenuService.Exist(id))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            await weiXinMenuService.Delete(id);
            return Ok();
        }
    }
}
