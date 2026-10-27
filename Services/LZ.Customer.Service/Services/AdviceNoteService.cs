using LZ.Core;
using LZ.Core.Interface;
using LZ.Customer.Service.Interface;
using LZ.Model;
using LZ.Model.PagedModel;
using SqlSugar;
using SqlSugar.IOC;
using System.Linq.Expressions;


namespace LZ.Customer.Service.Services
{
    public class AdviceNoteService : ServicesBase<Live_AdviceNote>, IAdviceNoteService, IScopedDependency
    {

        public async Task<List<Live_AdviceNote>> List(Expression<Func<Live_AdviceNote, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_AdviceNote>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_AdviceNote>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<List<Live_AdviceNote>> List(Expression<Func<Live_AdviceNote, bool>> exp)
        {
            return await base.Context.Queryable<Live_AdviceNote>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }
        public async Task<int> Add(Live_AdviceNote model)
        {
            return await base.Context.Insertable(model).ExecuteCommandAsync();
        }

        public async Task<int> Add(List<Live_AdviceNote> models)
        {
            return await base.Context.Insertable(models).ExecuteCommandAsync();
        }
        public async Task<bool> Exist(int id)
        {
            return await base.Context.Queryable<Live_AdviceNote>().AnyAsync(it => it.ID == id);
        }
        public async Task<bool> Update(Live_AdviceNote model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public async Task<Live_AdviceNote> Get(int id)
        {
            return await base.Context.Queryable<Live_AdviceNote>().FirstAsync(it => it.ID == id);
        }
        public async Task<Live_AdviceNote> Get(Expression<Func<Live_AdviceNote, bool>> exp)
        {
            return await base.Context.Queryable<Live_AdviceNote>().FirstAsync(exp);
        }
        public async Task<bool> Delete(int id)
        {
            return await base.Context.Deleteable<Live_AdviceNote>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
    }
}
