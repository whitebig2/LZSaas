using LZ.Core;
using LZ.Core.Interface;
using LZ.Invoice.Service.Interface;
using LZ.Model;
using LZ.Model.PagedModel;
using SqlSugar;
using System.Linq.Expressions;

namespace LZ.Invoice.Service.Services
{
    public class InvoiceConfigService : ServicesBase<FIN_InvoiceConfig>, IInvoiceConfigService, IScopedDependency
    {
        public async Task<List<FIN_InvoiceConfig>> List(Expression<Func<FIN_InvoiceConfig, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<FIN_InvoiceConfig>().Where(exp).OrderByDescending(it => it.ID).ToListAsync();
            }
            var list = await base.Context.Queryable<FIN_InvoiceConfig>().Where(exp).OrderByDescending(it => it.ID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<List<FIN_InvoiceConfig>> List(Expression<Func<FIN_InvoiceConfig, bool>> exp)
        {
            var list = await base.Context.Queryable<FIN_InvoiceConfig>().Where(exp).OrderByDescending(it => it.ID).ToListAsync();
            return list;
        }

        public async Task<List<FIN_InvoiceConfig>> ListView(Expression<Func<FIN_InvoiceConfig, bool>> exp)
        {
            var list = await base.Context.Queryable<FIN_InvoiceConfig>().Where(exp).OrderByDescending(it => it.ID).ToListAsync();
            return list;
        }

        public async Task<int> Add(FIN_InvoiceConfig model)
        {
            return await base.Context.Insertable(model).ExecuteCommandAsync();
        }

        public async Task<bool> Exist(int id)
        {
            return await base.Context.Queryable<FIN_InvoiceConfig>().AnyAsync(it => it.ID == id);
        }
        public async Task<bool> Exist(Expression<Func<FIN_InvoiceConfig, bool>> exp)
        {
            return await base.Context.Queryable<FIN_InvoiceConfig>().Where(exp).AnyAsync();
        }
        public async Task<bool> Update(FIN_InvoiceConfig model)
        {
            return await base.Context.Updateable<FIN_InvoiceConfig>(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<FIN_InvoiceConfig> Get(int id)
        {
            return await base.Context.Queryable<FIN_InvoiceConfig>().FirstAsync(it => it.ID == id);
        }

        public async Task<FIN_InvoiceConfig> Get(Expression<Func<FIN_InvoiceConfig, bool>> exp)
        {
            return await base.Context.Queryable<FIN_InvoiceConfig>().FirstAsync(exp);
        }

        public async Task<FIN_InvoiceConfig> GetView(Expression<Func<FIN_InvoiceConfig, bool>> exp)
        {
            return await base.Context.Queryable<FIN_InvoiceConfig>().FirstAsync(exp);
        }

        public async Task<bool> Delete(int id)
        {
            return await base.Context.Deleteable<FIN_InvoiceConfig>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
    }
}
