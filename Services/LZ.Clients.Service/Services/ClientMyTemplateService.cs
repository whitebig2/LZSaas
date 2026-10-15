using LZ.Clients.Service.Interface;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Core.Redis;
using LZ.Model;
using LZ.Model.PagedModel;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SqlSugar;
using SqlSugar.IOC;
using System.ComponentModel.Design;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace LZ.Clients.Service.Services
{
    public class ClientMyTemplateService(IClientTemplateService clientTemplateService,IClientTemplatePageService clientTemplatePageService,IClientPageService clientPageService) : ServicesBase<Model.Live_ClientMyTemplate>, IClientMyTemplateService, IScopedDependency
    {
        public async Task<List<Live_ClientMyTemplate>> List(Expression<Func<Live_ClientMyTemplate, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_ClientMyTemplate>().Where(exp).OrderByDescending(it=>it.ID).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_ClientMyTemplate>().Where(exp).OrderByDescending(it => it.ID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<int> Add(Live_ClientMyTemplate model)
        {
            return await base.Context.Insertable(model).ExecuteCommandAsync();
        }
        public async Task<int> AddToClient(int id,int tempID=0,string appType="xfz")
        {
            var model = new Model.Live_ClientMyTemplate();
            var expable = Expressionable.Create<Model.Live_ClientTemplate>();
            if (tempID>0)
            {
                expable.And(it => it.ID== tempID);
            }
            else
            {
                expable.And(it => it.IsDefault);
            }
            var exp = expable.ToExpression();
            var defaultModel = await clientTemplateService.Get(exp);
            if (defaultModel != null)
            {
                var defaultPages = await clientTemplatePageService.List(it=>it.TempID==defaultModel.ID,new PagedModelBase() { Page=0});
                
                model.AppConfig = defaultModel.PageConfig;
                model.ClientID = id;
                model.CreateTime=DateTime.Now;
                model.TempID = defaultModel.ID;
                model.Name=defaultModel.Name;
                model.IsDefault = true;
                await base.Context.Insertable(model).ExecuteCommandIdentityIntoEntityAsync();
                var page = new List<Model.Live_ClientPage>();
                foreach(var m in defaultPages)
                {
                    var p =new Model.Live_ClientPage();
                    p.ClientID = id;
                    p.TempID = model.ID;
                    p.Title=m.Title;
                    p.Moulds = m.Moulds;
                    p.Style=m.Style;
                    p.PageType = m.PageType;
                    page.Add(p);
                }
                await clientPageService.Add(page);
            }
            return model.ID;
        }
        public async Task<bool> Exist(int id)
        {
            return await base.Context.Queryable<Live_ClientMyTemplate>().AnyAsync(it=>it.ID==id);
        }
        public async Task<bool> Update(Live_ClientMyTemplate model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<Live_ClientMyTemplate> Get(int id)
        {
            return await base.Context.Queryable<Live_ClientMyTemplate>().FirstAsync(it => it.ID == id);
        }
        public async Task<Live_ClientMyTemplate> GetDefault(int clientId)
        {
            return await base.Context.Queryable<Live_ClientMyTemplate>().FirstAsync(it => it.ClientID == clientId && it.IsDefault);
        }
        public async Task<bool> Delete(int id)
        {
            return await base.Context.Deleteable<Live_ClientMyTemplate>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
    }
}
