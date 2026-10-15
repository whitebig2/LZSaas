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
using Senparc.Weixin.WxOpen.AdvancedAPIs.DataCube;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Customer.Service.Interface;
using NPOI.SS.Formula.Functions;

namespace LZ.Customer.Service.Services
{
    public class InsuranceOrderService : ServicesBase<Live_InsuranceOrderExtension>, IInsuranceOrderService, IScopedDependency
    {


        public async Task<List<Live_V_InsuranceOrderExtension>> PageList(Expression<Func<Live_V_InsuranceOrderExtension, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_V_InsuranceOrderExtension>()
                    .Includes(f=>f.OrderPlans)
                    .Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_V_InsuranceOrderExtension>().Includes(f => f.OrderPlans).Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<List<Live_InsuranceOrder>> List(Expression<Func<Live_InsuranceOrder, bool>> exp)
        {
            return await base.Context.Queryable<Live_InsuranceOrder>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }
        public async Task<int> Add(Live_InsuranceOrder model)
        {
            return await base.Context.Insertable(model).ExecuteCommandAsync();
        }

        public async Task<bool> AddNav(Live_InsuranceOrderExtension model)
        {
            return await base.Context.InsertNav(model)
                .Include(f=>f.OrderPlans)
                .ExecuteCommandAsync();
        }
        public async Task<bool> Exist(Guid id)
        {
            return await base.Context.Queryable<Live_InsuranceOrder>().AnyAsync(it => it.ID == id);
        }
       
        public async Task<bool> Update(Live_InsuranceOrder model)
        {
            return await base.Context.Updateable(model).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Update(List<Live_InsuranceOrder> models)
        {
            return await base.Context.Updateable(models).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<Live_InsuranceOrder> Get(Expression<Func<Live_InsuranceOrder, bool>> exp)
        {
            return await base.Context.Queryable<Live_InsuranceOrder>().OrderByDescending(f => f.CreateTime).FirstAsync(exp);
        }

        public async Task<Live_InsuranceOrderExtension> GetNav(Expression<Func<Live_InsuranceOrderExtension, bool>> exp)
        {
            return await base.Context.Queryable<Live_InsuranceOrderExtension>()
                .Includes(f => f.OrderPlans)
                .Includes(f => f.NameRecords)
                .OrderByDescending(f => f.CreateTime).FirstAsync(exp);
        }

        public async Task<bool> UpdateNav(Live_InsuranceOrderExtension model)
        {
            return await base.Context.UpdateNav<Live_InsuranceOrderExtension>(model)
                .Include(f => f.OrderPlans)
                .Include(f => f.NameRecords).ExecuteCommandAsync();
        }
        public async Task<bool> DeleteNav(Expression<Func<Live_InsuranceOrderExtension, bool>> exp)
        {
            return await base.Context.DeleteNav<Live_InsuranceOrderExtension>(exp)
                .Include(f => f.OrderPlans)
                .Include(f => f.NameRecords)
                .ExecuteCommandAsync();
        }


        public async Task<Live_InsuranceOrder> Get(Guid id)
        {
            return await base.Context.Queryable<Live_InsuranceOrder>().FirstAsync(it => it.ID == id);
        }

        public async Task<Live_V_InsuranceOrder> GetView(Expression<Func<Live_V_InsuranceOrder, bool>> exp)
        {
            return await base.Context.Queryable<Live_V_InsuranceOrder>().Where(exp).FirstAsync();
        }
        public async Task<bool> Delete(Guid id)
        {
            return await base.Context.Deleteable<Live_InsuranceOrder>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<List<Live_V_InsuranceOrderPlan>> GetOrderPlanViewList(Expression<Func<Live_V_InsuranceOrderPlan, bool>> exp)
        {
            return await base.Context.Queryable<Live_V_InsuranceOrderPlan>().Where(exp).ToListAsync();
        }
        public async Task<Live_V_InsuranceOrderPlan> GetOrderPlanView(Expression<Func<Live_V_InsuranceOrderPlan, bool>> exp)
        {
            return await base.Context.Queryable<Live_V_InsuranceOrderPlan>().Where(exp).FirstAsync();
        }
    }
}
