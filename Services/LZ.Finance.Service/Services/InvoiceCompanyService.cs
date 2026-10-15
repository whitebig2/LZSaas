using LZ.Finance.Service.Interface;
using LZ.Model;
using LZ.Model.PagedModel;
using SqlSugar.IOC;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LZ.Core.Interface;
using LZ.Core;

namespace LZ.Finance.Service.Services
{
    public class InvoiceCompanyService : ServicesBase<Live_InvoiceCompany>,IInvoiceCompanyService, IScopedDependency
    {

        public async Task<List<Live_InvoiceCompany>> PageList(Expression<Func<Live_InvoiceCompany, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var list = await   base.Context.Queryable<Live_InvoiceCompany>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<List<Live_InvoiceCompany>> List(Expression<Func<Live_InvoiceCompany, bool>> exp)
        {
            var list = await   base.Context.Queryable<Live_InvoiceCompany>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            return list;
        }

     
        public async Task<int> Add(Live_InvoiceCompany model)
        {
            return await   base.Context.Insertable(model).ExecuteCommandAsync();
        }
        public async Task<bool> Exist(Expression<Func<Live_InvoiceCompany, bool>> exp)
        {
            return await   base.Context.Queryable<Live_InvoiceCompany>().Where(exp).AnyAsync();
        }
        public async Task<bool> Update(Live_InvoiceCompany model)
        {
            return await   base.Context.Updateable<Live_InvoiceCompany>(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
     

        public async Task<Live_InvoiceCompany> Get(Guid id)
        {
            return await   base.Context.Queryable<Live_InvoiceCompany>().FirstAsync(it => it.ID == id);
        }

        public async Task<Live_InvoiceCompany> Get(Expression<Func<Live_InvoiceCompany, bool>> exp)
        {
            return await   base.Context.Queryable<Live_InvoiceCompany>().FirstAsync(exp);
        }
        public async Task<bool> Delete(Guid id)
        {
            return await   base.Context.Deleteable<Live_InvoiceCompany>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
    }
}
