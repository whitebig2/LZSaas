using LZ.Clients.Service.Interface;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Core.Redis;
using LZ.Model;
using LZ.Model.PagedModel;
using Polly;
using SqlSugar;
using SqlSugar.IOC;
using System.ComponentModel.Design;
using System.Linq.Expressions;


namespace LZ.Clients.Service.Services
{
    public class ClientShortLinkService(IClientsService clientsService) : ServicesBase<Model.Live_ClientShortLink>, IClientShortLinkService, IScopedDependency
    {
        public async Task<List<Live_V_ClientShortLink>> List(Expression<Func<Live_V_ClientShortLink, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_V_ClientShortLink>().Where(exp).OrderByDescending(it=>it.ID).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_V_ClientShortLink>().Where(exp).OrderByDescending(it => it.ID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
            
        }
        public async Task<int> Add(Live_ClientShortLink model)
        {
            return await base.Context.Insertable(model).ExecuteReturnIdentityAsync();
        }

        public async Task<Live_ClientShortLink> Get(int id)
        {
            return await base.Context.Queryable<Live_ClientShortLink>().FirstAsync(it => it.ID == id);
        }
        public async Task<string> GetLink(int companyId, string clientType,string pageUrl,string pageTitle)
        {
            try
            {
                var model =await base.Context.Queryable<Live_V_ClientShortLink>().FirstAsync(it => it.CompanyID == companyId && it.Type == clientType && it.PageUrl == pageUrl);
                if (model == null)
                {
                    var addModel = new Model.Live_ClientShortLink();
                    var client = await clientsService.Get(companyId, clientType, "mini");
                    if (client != null)
                    {
                        var res =await Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.ShortLinkApi.GenerateAsync(client.AppID, pageUrl, pageTitle, true);
                        if (res.errcode == 0)
                        {
                            addModel.ClientID = client.ID;
                            addModel.PageUrl = pageUrl;
                            addModel.PageTitle = pageTitle;
                            addModel.IsPermanent = true;
                            addModel.ShortLinkUrl = res.link;
                            addModel.CreateTime = DateTime.Now;
                            await base.Context.Insertable(addModel).ExecuteCommandAsync();
                            return res.link;
                        }
                        else
                        {
                            Common.StaticLog.Error(res.errmsg);
                            return "error";
                        }
                    }
                    else
                    {
                        return "error";
                    }
                }
                else
                {
                    return model.ShortLinkUrl;
                }
            }
            catch(Exception ex)
            {
                Common.StaticLog.Error(ex);
                return "error";
            }
        }
        public async Task<bool> Delete(int id)
        {
            return await base.Context.Deleteable<Live_ClientShortLink>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
    }
}
