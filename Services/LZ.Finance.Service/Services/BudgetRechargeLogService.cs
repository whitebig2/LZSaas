using LZ.Core.Interface;
using LZ.Finance.Service.Interface;
using LZ.Model.PagedModel;
using LZ.Model;
using SqlSugar.IOC;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LZ.Core;

namespace LZ.Finance.Service.Services
{
    public class BudgetRechargeLogService : ServicesBase<Live_BudgetRechargeLog>, IBudgetRechargeLogService, IScopedDependency
    {
        public async Task<List<Live_BudgetRechargeLog>> List(Expression<Func<Live_BudgetRechargeLog, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                if (!string.IsNullOrEmpty(query.Order))
                {
                    if (query.Order == "partner")
                    {
                        return await base.Context.Queryable<Live_BudgetRechargeLog>().Where(exp).OrderByDescending(it => it.PartnerName).ToListAsync();
                    }
                }
                return await base.Context.Queryable<Live_BudgetRechargeLog>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_BudgetRechargeLog>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<int> Add(Live_BudgetRechargeLog model)
        {
            return await base.Context.Insertable(model).ExecuteReturnIdentityAsync();
        }

        public async Task<int> Add(List<Live_BudgetRechargeLog> list)
        {
            return await base.Context.Insertable(list).ExecuteCommandAsync();
        }
        public async Task<bool> Exist(int id)
        {
            return await base.Context.Queryable<Live_BudgetRechargeLog>().AnyAsync(it => it.ID == id);
        }

        public async Task<bool> Exist(Expression<Func<Live_BudgetRechargeLog, bool>> exp)
        {
            return await base.Context.Queryable<Live_BudgetRechargeLog>().AnyAsync(exp);
        }

        public async Task<bool> Update(Live_BudgetRechargeLog model)
        {
            return await base.Context.Updateable<Live_BudgetRechargeLog>(model).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Update(List<Model.Live_BudgetRechargeLog> list)
        {
            return await base.Context.Updateable<Live_BudgetRechargeLog>(list).ExecuteCommandAsync() > 0;
        }
        public async Task<Live_BudgetRechargeLog> Get(int id)
        {
            return await base.Context.Queryable<Live_BudgetRechargeLog>().FirstAsync(it => it.ID == id);
        }
        public async Task<Live_BudgetRechargeLog> Get(Expression<Func<Live_BudgetRechargeLog, bool>> exp)
        {
            return await base.Context.Queryable<Live_BudgetRechargeLog>().Where(exp).OrderByDescending(it => it.CreateTime).FirstAsync();
        }
        public async Task<bool> Delete(int id)
        {
            return await base.Context.Deleteable<Live_BudgetRechargeLog>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
    }
}
