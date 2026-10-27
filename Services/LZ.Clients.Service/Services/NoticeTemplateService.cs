using LZ.Clients.Service.Interface;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Core.Redis;
using LZ.Model;
using LZ.Model.PagedModel;
using SqlSugar;
using SqlSugar.IOC;
using System.Linq.Expressions;


namespace LZ.Clients.Service.Services
{
    public class NoticeTemplateService : ServicesBase<Model.Live_NoticeTemplate>, INoticeTemplateService, IScopedDependency
    {
        public async Task<List<Live_NoticeTemplate>> List(Expression<Func<Live_NoticeTemplate, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_NoticeTemplate>().Where(exp).OrderByDescending(it=>it.ID).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_NoticeTemplate>().Where(exp).OrderByDescending(it => it.ID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<int> Add(Live_NoticeTemplate model)
        {
          return  await base.Context.Insertable(model).ExecuteCommandAsync();
        }
        public async Task<bool> Exist(int id)
        {
            var expable = Expressionable.Create<Model.Live_NoticeTemplate>();
            expable.And(it => it.ID == id);
            var exp = expable.ToExpression();
            return await Exist(exp);
        }
        public async Task<bool> Exist(Expression<Func<Live_NoticeTemplate, bool>> exp)
        {
            return await base.Context.Queryable<Live_NoticeTemplate>().AnyAsync(exp);
        }
        public async Task<bool> Update(Live_NoticeTemplate model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public async Task<Live_NoticeTemplate> Get(int id)
        {
            return await base.Context.Queryable<Live_NoticeTemplate>().FirstAsync(it=>it.ID==id);
        }
        public async Task<bool> Delete(int id)
        {
            return await base.Context.Deleteable<Live_NoticeTemplate>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
    }
}
