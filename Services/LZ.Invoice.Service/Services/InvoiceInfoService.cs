using LZ.Core;
using LZ.Core.Interface;
using LZ.Invoice.Service.Interface;
using LZ.Model;
using LZ.Model.PagedModel;
using SqlSugar;
using System.Linq.Expressions;

namespace LZ.Invoice.Service.Services
{
    public class InvoiceInfoService : ServicesBase<FIN_InvoiceInfo>, IInvoiceInfoService, IScopedDependency
    {
        public async Task<List<FIN_InvoiceInfo>> List(Expression<Func<FIN_InvoiceInfo, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<FIN_InvoiceInfo>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            }
            var list = await base.Context.Queryable<FIN_InvoiceInfo>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<List<FIN_InvoiceInfo>> List(Expression<Func<FIN_InvoiceInfo, bool>> exp)
        {
            var list = await base.Context.Queryable<FIN_InvoiceInfo>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            return list;
        }

        public async Task<int> Add(FIN_InvoiceInfo model)
        {
            return await base.Context.Insertable(model).ExecuteCommandAsync();
        }

        public async Task<bool> Exist(Guid id)
        {
            return await base.Context.Queryable<FIN_InvoiceInfo>().AnyAsync(it => it.ID == id);
        }

        public async Task<bool> Exist(Expression<Func<FIN_InvoiceInfo, bool>> exp)
        {
            return await base.Context.Queryable<FIN_InvoiceInfo>().Where(exp).AnyAsync();
        }

        public async Task<bool> Update(FIN_InvoiceInfo model)
        {
            return await base.Context.Updateable<FIN_InvoiceInfo>(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<FIN_InvoiceInfo> Get(Guid id)
        {
            return await base.Context.Queryable<FIN_InvoiceInfo>().FirstAsync(it => it.ID == id);
        }

        public async Task<FIN_InvoiceInfo> Get(Expression<Func<FIN_InvoiceInfo, bool>> exp)
        {
            return await base.Context.Queryable<FIN_InvoiceInfo>().FirstAsync(exp);
        }

        public async Task<bool> Delete(Guid id)
        {
            return await base.Context.Deleteable<FIN_InvoiceInfo>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
    }
}
