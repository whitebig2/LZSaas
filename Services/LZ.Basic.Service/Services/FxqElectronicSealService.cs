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
using LZ.Basic.Service.Interface;

namespace LZ.Basic.Service.Services
{
    public class FxqElectronicSealService : ServicesBase<Live_FxqElectronicSeal>, IFxqElectronicSealService, IScopedDependency
    {
        public async Task<List<Live_FxqElectronicSeal>> List(Expression<Func<Live_FxqElectronicSeal, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if ( query.Page == 0)
            {
                return await base.Context.Queryable<Live_FxqElectronicSeal>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_FxqElectronicSeal>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<int> Add(Live_FxqElectronicSeal model)
        {

            return await base.Context.Insertable(model).ExecuteCommandAsync();
        }
        public async Task<bool> Exist(Guid id)
        {
            return await base.Context.Queryable<Live_FxqElectronicSeal>().AnyAsync(it => it.ID == id);
        }
        public async Task<bool> Update(Live_FxqElectronicSeal model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }


        public async Task<Live_FxqElectronicSeal> Get(Expression<Func<Live_FxqElectronicSeal, bool>> exp )
        {
            return await base.Context.Queryable<Live_FxqElectronicSeal>().FirstAsync(exp);
        }


        public async Task<Live_FxqElectronicSeal> Get(Guid id)
        {
            return await base.Context.Queryable<Live_FxqElectronicSeal>().FirstAsync(it => it.ID == id);
        }
        public async Task<bool> Delete(Guid id)
        {
            return await base.Context.Deleteable<Live_FxqElectronicSeal>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
    }
}
