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
    public class SalaryConfigService : ServicesBase<Live_SalaryConfig>, ISalaryConfigService, IScopedDependency
    {

        public async Task<List<Live_V_SalaryConfig>> List(Expression<Func<Live_V_SalaryConfig, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await   base.Context.Queryable<Live_V_SalaryConfig>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            }
            var list = await   base.Context.Queryable<Live_V_SalaryConfig>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<List<Live_SalaryConfig>> List(Expression<Func<Live_SalaryConfig, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await   base.Context.Queryable<Live_SalaryConfig>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            }
            var list = await   base.Context.Queryable<Live_SalaryConfig>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<int> Add(Live_SalaryConfig model)
        {
            return await   base.Context.Insertable(model).ExecuteCommandAsync();
        }
        public async Task<bool> Exist(Guid id)
        {
            return await   base.Context.Queryable<Live_SalaryConfig>().AnyAsync(it => it.Id == id);
        }
        public async Task<bool> Update(Live_SalaryConfig model)
        {
            return await   base.Context.Updateable<Live_SalaryConfig>(model).ExecuteCommandAsync() > 0;
        }
        //public async Task<bool> ChangeStatus(Guid id, int status)
        //{
        //    return await   base.Context.Updateable<Live_SalaryConfig>().SetColumns(it => it.Status == status).Where(it => it.Id == id).ExecuteCommandAsync() > 0;
        //}

        public async Task<Live_SalaryConfig> Get(Guid id)
        {
            return await   base.Context.Queryable<Live_SalaryConfig>().FirstAsync(it => it.Id == id);
        }
        public async Task<Live_SalaryConfig> Get(Expression<Func<Live_SalaryConfig, bool>> exp)
        {
            return await   base.Context.Queryable<Live_SalaryConfig>().Where(exp).OrderByDescending(it => it.CreateTime).FirstAsync();
        }
        public async Task<bool> Delete(Guid id)
        {
            return await   base.Context.Deleteable<Live_SalaryConfig>().Where(it => it.Id == id).ExecuteCommandAsync() > 0;
        }


       
    }
}
