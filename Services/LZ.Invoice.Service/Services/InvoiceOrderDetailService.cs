using LZ.Core;
using LZ.Core.Interface;
using LZ.Invoice.Service.Interface;
using LZ.Model;
using LZ.Model.PagedModel;
using SqlSugar;
using System.Linq.Expressions;

namespace LZ.Invoice.Service.Services
{
    public class InvoiceOrderDetailService : ServicesBase<FIN_InvoiceOrderDetail>, IInvoiceOrderDetailService, IScopedDependency
    {
        public async Task<List<FIN_InvoiceOrderDetail>> List(Expression<Func<FIN_InvoiceOrderDetail, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<FIN_InvoiceOrderDetail>().Where(exp).OrderByDescending(it => it.ID).ToListAsync();
            }
            var list = await base.Context.Queryable<FIN_InvoiceOrderDetail>().Where(exp).OrderByDescending(it => it.ID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<List<FIN_InvoiceOrderDetail>> List(Expression<Func<FIN_InvoiceOrderDetail, bool>> exp)
        {
            var list = await base.Context.Queryable<FIN_InvoiceOrderDetail>().Where(exp).OrderByDescending(it => it.ID).ToListAsync();
            return list;
        }

        public async Task<int> Add(FIN_InvoiceOrderDetail model)
        {
            return await base.Context.Insertable(model).ExecuteCommandAsync();
        }

        public async Task<int> Add(List<FIN_InvoiceOrderDetail> models)
        {
            return await base.Context.Insertable(models).ExecuteCommandAsync();
        }

        public async Task<bool> Exist(int id)
        {
            return await base.Context.Queryable<FIN_InvoiceOrderDetail>().AnyAsync(it => it.ID == id);
        }

        public async Task<bool> Exist(Expression<Func<FIN_InvoiceOrderDetail, bool>> exp)
        {
            return await base.Context.Queryable<FIN_InvoiceOrderDetail>().Where(exp).AnyAsync();
        }

        public async Task<bool> Update(FIN_InvoiceOrderDetail model)
        {
            return await base.Context.Updateable<FIN_InvoiceOrderDetail>(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<FIN_InvoiceOrderDetail> Get(int id)
        {
            return await base.Context.Queryable<FIN_InvoiceOrderDetail>().FirstAsync(it => it.ID == id);
        }

        public async Task<FIN_InvoiceOrderDetail> Get(Expression<Func<FIN_InvoiceOrderDetail, bool>> exp)
        {
            return await base.Context.Queryable<FIN_InvoiceOrderDetail>().FirstAsync(exp);
        }

        public async Task<bool> Delete(int id)
        {
            return await base.Context.Deleteable<FIN_InvoiceOrderDetail>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Delete(Expression<Func<FIN_InvoiceOrderDetail, bool>> exp)
        {
            return await base.Context.Deleteable<FIN_InvoiceOrderDetail>().Where(exp).ExecuteCommandAsync() > 0;
        }
    }
}
