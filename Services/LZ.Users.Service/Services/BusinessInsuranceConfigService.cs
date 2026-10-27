using LZ.Core;
using LZ.Core.Interface;
using LZ.Model;
using SqlSugar;
using SqlSugar.IOC;
using System.Linq.Expressions;
using LZ.Model.PagedModel;
using LZ.Model.ModelExtensions;
using LZ.Users.Service.Interface;


namespace LZ.Users.Service.Services
{
    public class BusinessInsuranceConfigService : ServicesBase<Live_BusinessInsuranceConfig>, IBusinessInsuranceConfigService, IScopedDependency
    {
        public async Task<List<Live_V_BusinessInsuranceConfig>> PageList(Expression<Func<Live_V_BusinessInsuranceConfig, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var list = await base.Context.Queryable<Live_V_BusinessInsuranceConfig>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<List<Live_V_BusinessInsuranceConfig>> ListView(Expression<Func<Live_V_BusinessInsuranceConfig, bool>> exp)
        {
            return await base.Context.Queryable<Live_V_BusinessInsuranceConfig>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }
        public async Task<List<Live_BusinessInsuranceConfig>> List(Expression<Func<Live_BusinessInsuranceConfig, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_BusinessInsuranceConfig>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_BusinessInsuranceConfig>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<List<Live_BusinessInsuranceConfig>> List(Expression<Func<Live_BusinessInsuranceConfig, bool>> exp)
        {
            return await base.Context.Queryable<Live_BusinessInsuranceConfig>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }
        public async Task<int> Add(Live_BusinessInsuranceConfig model)
        {
            return await base.Context.Insertable(model).ExecuteCommandAsync();
        }



        public async Task<bool> Exist(Guid id)
        {
            return await base.Context.Queryable<Live_BusinessInsuranceConfig>().AnyAsync(it => it.ID == id);
        }

        public async Task<bool> Exist(Expression<Func<Live_BusinessInsuranceConfig, bool>> exp)
        {
            return await base.Context.Queryable<Live_BusinessInsuranceConfig>().AnyAsync(exp);
        }
        public async Task<bool> Update(Live_BusinessInsuranceConfig model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }


        public async Task<Live_BusinessInsuranceConfig> Get(Guid id)
        {
            return await base.Context.Queryable<Live_BusinessInsuranceConfig>().FirstAsync(it => it.ID == id);
        }

        public async Task<Live_BusinessInsuranceConfig> Get(Expression<Func<Live_BusinessInsuranceConfig, bool>> exp)
        {
            return await base.Context.Queryable<Live_BusinessInsuranceConfig>().FirstAsync(exp);
        }

        public async Task<Live_V_BusinessInsuranceConfig> GetView(Expression<Func<Live_V_BusinessInsuranceConfig, bool>> exp)
        {
            return await base.Context.Queryable<Live_V_BusinessInsuranceConfig>().FirstAsync(exp);
        }

        public async Task<bool> Delete(Guid id)
        {
            return await base.Context.Deleteable<Live_BusinessInsuranceConfig>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }


        public async Task<Live_BusinessInsuranceConfigExtension> GetNav(Expression<Func<Live_BusinessInsuranceConfigExtension, bool>> exp)
        {
            return await base.Context.Queryable<Live_BusinessInsuranceConfigExtension>()
                .Includes(f => f.WorkNamePlans)
                .FirstAsync(exp);
        }

        public async Task<bool> AddNav(Live_BusinessInsuranceConfigExtension model)
        {
            return await base.Context.InsertNav(model)
                .Include(f => f.WorkNamePlans)
                .ExecuteCommandAsync();
        }

        public async Task<bool> UpdateNav(Live_BusinessInsuranceConfigExtension model)
        {
            return await base.Context.UpdateNav(model)
                .Include(f => f.WorkNamePlans, new SqlSugar.UpdateNavOptions()
                {
                    OneToManyInsertOrUpdate = true,//配置启用 插入、更新或删除模式
                })
                .ExecuteCommandAsync();
        }

        public async Task<bool> DeleteNav(Guid id)
        {
            return await base.Context.DeleteNav<Live_BusinessInsuranceConfigExtension>(f=>f.ID==id)
                .Include(f => f.WorkNamePlans)
                .ExecuteCommandAsync();
        }
    }
}
