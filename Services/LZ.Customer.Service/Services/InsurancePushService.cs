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
//using LZ.Customer.Service.Interface;
//using NPOI.SS.Formula.Functions;

//namespace LZ.Customer.Service.Services
//{
//    public class InsurancePushService : ServicesBase, IInsurancePushService, IScopedDependency
//    {
//        public async Task<List<Live_V_InsurancePush>> PageViewList(Expression<Func<Live_V_InsurancePush, bool>> exp, PagedModelBase query)
//        {
//            var count = new RefAsync<int>();
//            if (query.Page == 0)
//            {
//                return await DbScoped.SugarScope.Queryable<Live_V_InsurancePush>()
//                    .Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
//            }
//            var list = await DbScoped.SugarScope.Queryable<Live_V_InsurancePush>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
//            query.Count = count.Value;
//            return list;
//        }

//        public async Task<List<Live_InsurancePush>> PageList(Expression<Func<Live_InsurancePush, bool>> exp, PagedModelBase query)
//        {
//            var count = new RefAsync<int>();
//            if (query.Page == 0)
//            {
//                return await DbScoped.SugarScope.Queryable<Live_InsurancePush>()
//                    .Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
//            }
//            var list = await DbScoped.SugarScope.Queryable<Live_InsurancePush>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
//            query.Count = count.Value;
//            return list;
//        }
//        public async Task<List<Live_InsurancePush>> List(Expression<Func<Live_InsurancePush, bool>> exp)
//        {
//            return await DbScoped.SugarScope.Queryable<Live_InsurancePush>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
//        }

//        public async Task<List<Live_InsurancePushExtension>> ListNav(Expression<Func<Live_InsurancePushExtension, bool>> exp)
//        {
//            return await DbScoped.SugarScope.Queryable<Live_InsurancePushExtension>()
//                .Includes(f=>f.PushRecords)
//                .Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
//        }

//        public async Task<bool> AddNav(Live_InsurancePushExtension model)
//        {
//            return await DbScoped.SugarScope.InsertNav(model)
//                .Include(f=>f.PushRecords)
//                .ExecuteCommandAsync();
//        }
        
       
//        public async Task<bool> Update(Live_InsurancePush model)
//        {
//            return await DbScoped.SugarScope.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
//        }
//        public async Task<bool> Update(List<Live_InsurancePush> models)
//        {
//            return await DbScoped.SugarScope.Updateable(models).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
//        }

//        public async Task<Live_InsurancePush> Get(Expression<Func<Live_InsurancePush, bool>> exp)
//        {
//            return await DbScoped.SugarScope.Queryable<Live_InsurancePush>().OrderByDescending(f => f.CreateTime).FirstAsync(exp);
//        }


//        public async Task<Live_V_InsurancePushExtension> GetViewNav(Expression<Func<Live_V_InsurancePushExtension, bool>> exp)
//        {
//            return await DbScoped.SugarScope.Queryable<Live_V_InsurancePushExtension>()
//                .Includes(f => f.PushRecords)
//                .OrderByDescending(f => f.CreateTime).FirstAsync(exp);
//        }

//        public async Task<List<Live_V_InsurancePushExtension>> GetViewListNav(Expression<Func<Live_V_InsurancePushExtension, bool>> exp)
//        {
//            return await DbScoped.SugarScope.Queryable<Live_V_InsurancePushExtension>()
//                .Includes(f => f.PushRecords)
//                .Where(exp)
//                .OrderByDescending(f => f.CreateTime).ToListAsync();
//        }


//        public async Task<Live_InsurancePushExtension> GetNav(Expression<Func<Live_InsurancePushExtension, bool>> exp)
//        {
//            return await DbScoped.SugarScope.Queryable<Live_InsurancePushExtension>()
//                .Includes(f => f.PushRecords)
//                .OrderByDescending(f => f.CreateTime).FirstAsync(exp);
//        }

//        public async Task<bool> UpdateNav(Live_InsurancePushExtension model)
//        {
//            return await DbScoped.SugarScope.UpdateNav(model).Include(f=>f.PushRecords,new UpdateNavOptions() { 
            
//            OneToManyInsertOrUpdate = true,
//            }).ExecuteCommandAsync();
//        }

//        //public async Task<bool> UpdateNav(Live_InsuranceOrderExtension model)
//        //{
//        //    return await DbScoped.SugarScope.UpdateNav<Live_InsuranceOrderExtension>(model)
//        //        .Include(f => f.OrderPlans)
//        //        .Include(f => f.NameRecords).ExecuteCommandAsync();
//        //}
//        //public async Task<bool> DeleteNav(Expression<Func<Live_InsuranceOrderExtension, bool>> exp)
//        //{
//        //    return await DbScoped.SugarScope.DeleteNav<Live_InsuranceOrderExtension>(exp)
//        //        .Include(f => f.OrderPlans)
//        //        .Include(f => f.NameRecords)
//        //        .ExecuteCommandAsync();
//        //}
//        public async Task<Live_InsurancePush> Get(Guid id)
//        {
//            return await DbScoped.SugarScope.Queryable<Live_InsurancePush>().FirstAsync(it => it.ID == id);
//        }
//        public async Task<bool> DeleteNav(Live_InsurancePushExtension model)
//        {
//            return await DbScoped.SugarScope.DeleteNav<Live_InsurancePushExtension>(model).Include(f=>f.PushRecords).ExecuteCommandAsync() ;
//        }

        
//    }
//}
