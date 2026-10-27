using LZ.Bank.Service.Interface;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Model;
using LZ.Model.PagedModel;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Bank.Service.Services
{
    public class BankElectronicReceiptService : ServicesBase<Live_BankElectronicReceipt>, IBankElectronicReceiptService, IScopedDependency
    {

        public async Task<List<Live_BankElectronicReceipt>> PageList(Expression<Func<Live_BankElectronicReceipt, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_BankElectronicReceipt>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_BankElectronicReceipt>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<List<Live_BankElectronicReceipt>> List(Expression<Func<Live_BankElectronicReceipt, bool>> exp)
        {
            return await base.Context.Queryable<Live_BankElectronicReceipt>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }
        public async Task<int> Add(List<Live_BankElectronicReceipt> models)
        {
            return await base.Context.Insertable(models).ExecuteCommandAsync();
        }

        public async Task<bool> Exist(Expression<Func<Live_BankElectronicReceipt, bool>> exp)
        {
            return await base.Context.Queryable<Live_BankElectronicReceipt>().AnyAsync(exp);
        }

      

        public async Task<Live_BankElectronicReceipt> Get(Expression<Func<Live_BankElectronicReceipt, bool>> exp)
        {
            return await base.Context.Queryable<Live_BankElectronicReceipt>().OrderByDescending(f => f.CreateTime).FirstAsync(exp);
        }
        public async Task<bool> Delete(Guid id)
        {
            return await base.Context.Deleteable<Live_BankElectronicReceipt>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
    }
}
