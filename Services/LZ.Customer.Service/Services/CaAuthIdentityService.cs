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
    public class CaAuthIdentityService : ServicesBase<Live_CaAuthIdentity>, ICaAuthIdentityService, IScopedDependency
    {


        public async Task<List<Live_CaAuthIdentity>> PageList(Expression<Func<Live_CaAuthIdentity, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_CaAuthIdentity>() .Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_CaAuthIdentity>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<List<Live_CaAuthIdentity>> List(Expression<Func<Live_CaAuthIdentity, bool>> exp)
        {
            return await base.Context.Queryable<Live_CaAuthIdentity>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }
        public async Task<int> Add(Live_CaAuthIdentity model)
        {
            return await base.Context.Insertable(model).ExecuteCommandAsync();
        }
        public async Task<bool> Exist(Expression<Func<Live_CaAuthIdentity, bool>> exp)
        {
            return await base.Context.Queryable<Live_CaAuthIdentity>().AnyAsync(exp);
        }
       
        public async Task<bool> Update(Live_CaAuthIdentity model)
        {
            return await base.Context.Updateable(model).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Update(List<Live_CaAuthIdentity> models)
        {
            return await base.Context.Updateable(models).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<Live_CaAuthIdentity> Get(Expression<Func<Live_CaAuthIdentity, bool>> exp)
        {
            return await base.Context.Queryable<Live_CaAuthIdentity>().OrderByDescending(f => f.CreateTime).FirstAsync(exp);
        }

   
        public async Task<bool> Delete(Guid id)
        {
            return await base.Context.Deleteable<Live_CaAuthIdentity>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }

      
    }
}
