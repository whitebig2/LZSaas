using LZ.Core;
using LZ.Core.Interface;
using LZ.Invoice.Service.Interface;
using LZ.Model;
using LZ.Model.PagedModel;
using SqlSugar;
using System.Linq.Expressions;

namespace LZ.Invoice.Service.Services
{
    public class InvoicePartnerRelationService : ServicesBase<FIN_InvoicePartnerRelation>, IInvoicePartnerRelationService, IScopedDependency
    {
        public async Task<List<FIN_InvoicePartnerRelation>> List(Expression<Func<FIN_InvoicePartnerRelation, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<FIN_InvoicePartnerRelation>().Where(exp).OrderByDescending(it => it.ID).ToListAsync();
            }
            var list = await base.Context.Queryable<FIN_InvoicePartnerRelation>().Where(exp).OrderByDescending(it => it.ID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<List<FIN_InvoicePartnerRelation>> List(Expression<Func<FIN_InvoicePartnerRelation, bool>> exp)
        {
            var list = await base.Context.Queryable<FIN_InvoicePartnerRelation>().Where(exp).OrderByDescending(it => it.ID).ToListAsync();
            return list;
        }

        public async Task<int> Add(FIN_InvoicePartnerRelation model)
        {
            return await base.Context.Insertable(model).ExecuteCommandAsync();
        }

        public async Task<int> Add(List<FIN_InvoicePartnerRelation> models)
        {
            return await base.Context.Insertable(models).ExecuteCommandAsync();
        }

        public async Task<bool> Exist(int id)
        {
            return await base.Context.Queryable<FIN_InvoicePartnerRelation>().AnyAsync(it => it.ID == id);
        }

        public async Task<bool> Exist(Expression<Func<FIN_InvoicePartnerRelation, bool>> exp)
        {
            return await base.Context.Queryable<FIN_InvoicePartnerRelation>().Where(exp).AnyAsync();
        }

        public async Task<bool> Update(FIN_InvoicePartnerRelation model)
        {
            return await base.Context.Updateable<FIN_InvoicePartnerRelation>(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<FIN_InvoicePartnerRelation> Get(int id)
        {
            return await base.Context.Queryable<FIN_InvoicePartnerRelation>().FirstAsync(it => it.ID == id);
        }

        public async Task<FIN_InvoicePartnerRelation> Get(Expression<Func<FIN_InvoicePartnerRelation, bool>> exp)
        {
            return await base.Context.Queryable<FIN_InvoicePartnerRelation>().FirstAsync(exp);
        }

        public async Task<bool> Delete(int id)
        {
            return await base.Context.Deleteable<FIN_InvoicePartnerRelation>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Delete(Expression<Func<FIN_InvoicePartnerRelation, bool>> exp)
        {
            return await base.Context.Deleteable<FIN_InvoicePartnerRelation>().Where(exp).ExecuteCommandAsync() > 0;
        }
    }
}
