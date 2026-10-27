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
using LZ.Finance.Service.Interface;
using LZ.Core.Interface;
using LZ.Core;

namespace LZ.Finance.Service.Services
{
    public class BudgetApplyRecordDetailService : ServicesBase<Live_BudgetApplyRecordDetail>, IBudgetApplyRecordDetailService, IScopedDependency
    {
        public async Task<List<Live_BudgetApplyRecordDetail>> List(Expression<Func<Live_BudgetApplyRecordDetail, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_BudgetApplyRecordDetail>().Where(exp).OrderByDescending(it => it.ID).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_BudgetApplyRecordDetail>().Where(exp).OrderByDescending(it => it.ID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<List<Live_V_BudgetApplyRecordDetail>> ListV(Expression<Func<Live_V_BudgetApplyRecordDetail, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_V_BudgetApplyRecordDetail>().Where(exp).OrderByDescending(it => it.ID).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_V_BudgetApplyRecordDetail>().Where(exp).OrderByDescending(it => it.ID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<int> Add(Live_BudgetApplyRecordDetail model)
        {
            return await base.Context.Insertable(model).ExecuteCommandAsync();
        }
        public async Task<int> Add(List<Live_BudgetApplyRecordDetail> list)
        {
            return await base.Context.Insertable(list).ExecuteCommandAsync();
        }
        public async Task<bool> Exist(int id)
        {
            return await base.Context.Queryable<Live_BudgetApplyRecordDetail>().AnyAsync(it => it.ID == id);
        }

        public async Task<bool> Exist(Expression<Func<Live_BudgetApplyRecordDetail, bool>> exp)
        {
            return await base.Context.Queryable<Live_BudgetApplyRecordDetail>().AnyAsync(exp);
        }

        public async Task<bool> Update(Live_BudgetApplyRecordDetail model)
        {
            return await base.Context.Updateable<Live_BudgetApplyRecordDetail>(model).ExecuteCommandAsync() > 0;
        }

        public async Task<Live_BudgetApplyRecordDetail> Get(int id)
        {
            return await base.Context.Queryable<Live_BudgetApplyRecordDetail>().FirstAsync(it => it.ID == id);
        }

        public async Task<List<Live_BudgetApplyRecordDetail>> GetByApply(int id)
        {
            return await base.Context.Queryable<Live_BudgetApplyRecordDetail>().Where(t=>t.BudgetApplyRecordID==id).ToListAsync();
        }


        public async Task<bool> Delete(int id)
        {
            return await base.Context.Deleteable<Live_BudgetApplyRecordDetail>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
    }
}
