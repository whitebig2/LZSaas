//using LZ.Core;
//using LZ.Core.Interface;
//using LZ.Model;
//using LZ.Model.PagedModel;
//using LZ.Users.Service.Interface;
//using Microsoft.EntityFrameworkCore.Metadata.Internal;
//using NPOI.SS.Formula.Functions;
//using SqlSugar;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Text;
//using System.Threading.Tasks;
//using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

//namespace LZ.Users.Service.Services
//{
//    public class BusinessInsuranceSystemRecordService :   ServicesBase<Live_BusinessInsuranceSystemRecord>,IBusinessInsuranceSystemRecordService, IScopedDependency
//    {

       
//        public Task<int> Add(Live_BusinessInsuranceSystemRecord model)
//        {
//            return base.Context.Insertable<Live_BusinessInsuranceSystemRecord>(model).ExecuteCommandAsync();
//        }
//        public Task<int> Add(List<Live_BusinessInsuranceSystemRecord> models)
//        {
//            return base.Context.Insertable<Live_BusinessInsuranceSystemRecord>(models).ExecuteCommandAsync();
//        }

//        public Task<int> Delete(Expression<Func<Live_BusinessInsuranceSystemRecord, bool>> exp)
//        {
//            return base.Context.Deleteable<Live_BusinessInsuranceSystemRecord>(exp).ExecuteCommandAsync();
//        }

//        public Task<Live_BusinessInsuranceSystemRecord> Get(Expression<Func<Live_BusinessInsuranceSystemRecord, bool>> exp)
//        {
//            return base.Context.Queryable<Live_BusinessInsuranceSystemRecord>().Where(exp).FirstAsync();
//        }

//        public Task<List<Live_BusinessInsuranceSystemRecord>> List(Expression<Func<Live_BusinessInsuranceSystemRecord, bool>> exp)
//        {
//            return base.Context.Queryable<Live_BusinessInsuranceSystemRecord>().Where(exp).ToListAsync();
//        }

//        public async Task<List<Live_BusinessInsuranceSystemRecord>> PageList(Expression<Func<Live_BusinessInsuranceSystemRecord, bool>> exp, PagedModelBase query)
//        {
//            var count = new RefAsync<int>();
//            var list = await base.Context.Queryable<Live_BusinessInsuranceSystemRecord>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
//            query.Count = count.Value;
//            return list;
//        }

//        public async Task<bool> Update(List<Live_BusinessInsuranceSystemRecord> models)
//        {
//            return await base.Context.Updateable<Live_BusinessInsuranceSystemRecord>(models).ExecuteCommandAsync()>0;
//        }

//        public async Task<bool> Update(Live_BusinessInsuranceSystemRecord model)
//        {
//            return await base.Context.Updateable<Live_BusinessInsuranceSystemRecord>(model).ExecuteCommandAsync() > 0;
//        }



      
//    }
//}
