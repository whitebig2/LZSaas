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
using NPOI.SS.Formula.Functions;
using LZ.Users.Service.Interface;

namespace LZ.Customer.Service.Services
{
    public class BusinessInsurancePushService : ServicesBase<Live_BusinessInsurancePush>, IBusinessInsurancePushService, IScopedDependency
    {
        public async Task<List<Live_V_BusinessInsurancePush>> PageList(Expression<Func<Live_V_BusinessInsurancePush, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_V_BusinessInsurancePush>()
                    .Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_V_BusinessInsurancePush>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<List<Live_BusinessInsurancePush>> List(Expression<Func<Live_BusinessInsurancePush, bool>> exp)
        {
            return await base.Context.Queryable<Live_BusinessInsurancePush>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }

        public async Task<List<Live_BusinessInsurancePushExtension>> ListNav(Expression<Func<Live_BusinessInsurancePushExtension, bool>> exp)
        {
            return await base.Context.Queryable<Live_BusinessInsurancePushExtension>()
                .Includes(f=>f.Records)
                .Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }

        public async Task<bool> AddNav(Live_BusinessInsurancePushExtension model)
        {
            return await base.Context.InsertNav(model)
                .Include(f=>f.Records)
                .ExecuteCommandAsync();
        }

        public async Task<bool> AddNav(List<Live_BusinessInsurancePushExtension> models)
        {
            return await base.Context.InsertNav(models).Include(f => f.Records).ExecuteCommandAsync();
        }

        public async Task<bool> Add(List<Live_BusinessInsurancePush> models)
        {
            return await base.Context.Insertable(models).ExecuteCommandAsync()>0;
        }


        public async Task<bool> Update(Live_BusinessInsurancePush model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Update(List<Live_BusinessInsurancePush> models)
        {
            return await base.Context.Updateable(models).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<Live_BusinessInsurancePush> Get(Expression<Func<Live_BusinessInsurancePush, bool>> exp)
        {
            return await base.Context.Queryable<Live_BusinessInsurancePush>().OrderByDescending(f => f.CreateTime).FirstAsync(exp);
        }


      




        public async Task<Live_BusinessInsurancePushExtension> GetNav(Expression<Func<Live_BusinessInsurancePushExtension, bool>> exp)
        {
            return await base.Context.Queryable<Live_BusinessInsurancePushExtension>()
                .Includes(f => f.Records)
                .OrderByDescending(f => f.CreateTime).FirstAsync(exp);
        }

        public async Task<bool> UpdateNav(Live_BusinessInsurancePushExtension model)
        {
            return await base.Context.UpdateNav(model).Include(f=>f.Records,new UpdateNavOptions() { 
            OneToManyInsertOrUpdate = true,
            }).ExecuteCommandAsync();
        }

      
        public async Task<bool> DeleteNav(Live_BusinessInsurancePushExtension model)
        {
            return await base.Context.DeleteNav<Live_BusinessInsurancePushExtension>(model).Include(f=>f.Records).ExecuteCommandAsync() ;
        }

        
    }
}
