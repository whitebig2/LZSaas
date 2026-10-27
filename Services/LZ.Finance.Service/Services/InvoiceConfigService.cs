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
    public class InvoiceConfigService : ServicesBase<Live_InvoiceConfig>, IInvoiceConfigService, IScopedDependency
    {

        public async Task<List<Live_V_InvoiceConfig>> List(Expression<Func<Live_V_InvoiceConfig, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await   base.Context.Queryable<Live_V_InvoiceConfig>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            }
            var list = await   base.Context.Queryable<Live_V_InvoiceConfig>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<List<Live_InvoiceConfig>> List(Expression<Func<Live_InvoiceConfig, bool>> exp)
        {
            var list = await   base.Context.Queryable<Live_InvoiceConfig>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            return list;
        }

        public async Task<List<Live_V_InvoiceConfig>> ListView(Expression<Func<Live_V_InvoiceConfig, bool>> exp)
        {
            var list = await   base.Context.Queryable<Live_V_InvoiceConfig>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            return list;
        }
        public async Task<int> Add(Live_InvoiceConfig model)
        {
            return await   base.Context.Insertable(model).ExecuteCommandAsync();
        }
        public async Task<bool> Exist(Guid id)
        {
            return await   base.Context.Queryable<Live_InvoiceConfig>().AnyAsync(it => it.ID == id);
        }
        public async Task<bool> Update(Live_InvoiceConfig model)
        {
            return await   base.Context.Updateable<Live_InvoiceConfig>(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        //public async Task<bool> ChangeStatus(Guid id, int status)
        //{
        //    return await   base.Context.Updateable<Live_InvoiceConfig>().SetColumns(it => it.Status == status).Where(it => it.Id == id).ExecuteCommandAsync() > 0;
        //}

        public async Task<Live_InvoiceConfig> Get(Guid id)
        {
            return await   base.Context.Queryable<Live_InvoiceConfig>().FirstAsync(it => it.ID == id);
        }

        public async Task<Live_InvoiceConfig> Get(Expression<Func<Live_InvoiceConfig, bool>> exp)
        {
            return await   base.Context.Queryable<Live_InvoiceConfig>().FirstAsync(exp);
        }

        public async Task<Live_V_InvoiceConfig> GetView(Expression<Func<Live_V_InvoiceConfig, bool>> exp)
        {
            return await   base.Context.Queryable<Live_V_InvoiceConfig>().FirstAsync(exp);
        }
        public async Task<bool> Delete(Guid id)
        {
            return await   base.Context.Deleteable<Live_InvoiceConfig>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
    }
}
