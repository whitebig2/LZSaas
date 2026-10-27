//using LZ.Customer.Service.Interface;
//using LZ.Model.PagedModel;
//using LZ.Model;
//using SqlSugar.IOC;
//using SqlSugar;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Text;
//using System.Threading.Tasks;
//using Senparc.Weixin.WxOpen.AdvancedAPIs.DataCube;
//using LZ.Core;
//using LZ.Core.Interface;

//namespace LZ.Customer.Service.Services
//{
//    public class FxqElectronicSealService: ServicesBase, IFxqElectronicSealService, IScopedDependency
//    {
//        public async Task<List<Live_FxqElectronicSeal>> List(Expression<Func<Live_FxqElectronicSeal, bool>> exp, PagedModelBase query)
//        {
//            var count = new RefAsync<int>();
//            if (query.Page == 0)
//            {
//                return await DbScoped.SugarScope.Queryable<Live_FxqElectronicSeal>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
//            }
//            var list = await DbScoped.SugarScope.Queryable<Live_FxqElectronicSeal>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
//            query.Count = count.Value;
//            return list;
//        }
//        public async Task<int> Add(Live_FxqElectronicSeal model)
//        {
           
//            return await DbScoped.SugarScope.Insertable(model).ExecuteCommandAsync();
//        }
//        public async Task<bool> Exist(Guid id)
//        {
//            return await DbScoped.SugarScope.Queryable<Live_FxqElectronicSeal>().AnyAsync(it => it.ID == id);
//        }
//        public async Task<bool> Update(Live_FxqElectronicSeal model)
//        {
//            return await DbScoped.SugarScope.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
//        }
      

//        public async Task<Live_FxqElectronicSeal> Get(Guid id)
//        {
//            return await DbScoped.SugarScope.Queryable<Live_FxqElectronicSeal>().FirstAsync(it => it.ID == id);
//        }
//        public async Task<bool> Delete(Guid id)
//        {
//            return await DbScoped.SugarScope.Deleteable<Live_FxqElectronicSeal>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
//        }
//    }
//}
