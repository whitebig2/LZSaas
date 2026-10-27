using LZ.Core;
using LZ.Core.Interface;
using LZ.Invoice.Service.Interface;
using LZ.Model;
using LZ.Model.PagedModel;
using SqlSugar;
using System.Linq.Expressions;

namespace LZ.Invoice.Service.Services
{
    public class InvoiceCustomerService : ServicesBase<FIN_InvoiceCustomer>, IInvoiceCustomerService, IScopedDependency
    {
        public async Task<List<FIN_InvoiceCustomer>> PageList(Expression<Func<FIN_InvoiceCustomer, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var list = await base.Context.Queryable<FIN_InvoiceCustomer>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<List<FIN_InvoiceCustomer>> List(Expression<Func<FIN_InvoiceCustomer, bool>> exp)
        {
            var list = await base.Context.Queryable<FIN_InvoiceCustomer>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            return list;
        }

        public async Task<int> Add(FIN_InvoiceCustomer model)
        {
            return await base.Context.Insertable(model).ExecuteCommandAsync();
        }

        public async Task<bool> Exist(Expression<Func<FIN_InvoiceCustomer, bool>> exp)
        {
            return await base.Context.Queryable<FIN_InvoiceCustomer>().Where(exp).AnyAsync();
        }

        public async Task<bool> Update(FIN_InvoiceCustomer model)
        {
            return await base.Context.Updateable<FIN_InvoiceCustomer>(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<FIN_InvoiceCustomer> Get(Guid id)
        {
            return await base.Context.Queryable<FIN_InvoiceCustomer>().FirstAsync(it => it.ID == id);
        }

        public async Task<FIN_InvoiceCustomer> Get(Expression<Func<FIN_InvoiceCustomer, bool>> exp)
        {
            return await base.Context.Queryable<FIN_InvoiceCustomer>().FirstAsync(exp);
        }

        public async Task<bool> Delete(Guid id)
        {
            return await base.Context.Deleteable<FIN_InvoiceCustomer>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
    }
}
