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
//using LZ.Customer.Service.Interface;
//using LZ.Core;
//using LZ.Core.Interface;
//using LZ.Model.ModelExtensions;

//namespace LZ.Customer.Service.Services
//{
//    public class CustomerContractFileService:ServicesBase, ICustomerContractFileService, IScopedDependency
//    {
//        public async Task<List<Live_V_CustomerContractFile>> ListView(Expression<Func<Live_V_CustomerContractFile, bool>> exp, PagedModelBase query)
//        {
//            var count = new RefAsync<int>();
//            if (query.Page == 0)
//            {
//                return await DbScoped.SugarScope.Queryable<Live_V_CustomerContractFile>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
//            }
//            var list = await DbScoped.SugarScope.Queryable<Live_V_CustomerContractFile>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
//            query.Count = count.Value;
//            return list;
//        }

//        public async Task<List<Live_CustomerContractFile>> List(Expression<Func<Live_CustomerContractFile, bool>> exp, PagedModelBase query)
//        {
//            var count = new RefAsync<int>();
//            if (query.Page == 0)
//            {
//                return await DbScoped.SugarScope.Queryable<Live_CustomerContractFile>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
//            }
//            var list = await DbScoped.SugarScope.Queryable<Live_CustomerContractFile>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
//            query.Count = count.Value;
//            return list;
//        }
//        public async Task<int> Add(Live_CustomerContractFile model)
//        {
//            return await DbScoped.SugarScope.Insertable(model).ExecuteCommandAsync();
//        }
//        public async Task<int> Add(List<Live_CustomerContractConfig> models)
//        {
//            return await DbScoped.SugarScope.Insertable(models).ExecuteCommandAsync();
//        }
//        public async Task<bool> Exist(Guid id)
//        {
//            return await DbScoped.SugarScope.Queryable<Live_CustomerContractFile>().AnyAsync(it => it.ID == id);
//        }
//        public async Task<bool> Update(Live_CustomerContractFile model)
//        {
//            return await DbScoped.SugarScope.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
//        }
       

//        public async Task<Live_CustomerContractFile> Get(Guid id)
//        {
//            return await DbScoped.SugarScope.Queryable<Live_CustomerContractFile>().FirstAsync(it => it.ID == id);
//        }
//        public async Task<Live_V_CustomerContractFile> GetView(Guid id)
//        {
//            return await DbScoped.SugarScope.Queryable<Live_V_CustomerContractFile>().FirstAsync(it => it.ID == id);
//        }
//        public async Task<bool> Delete(Guid id)
//        {
//            return await DbScoped.SugarScope.Deleteable<Live_CustomerContractFile>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
//        }

       



//    }
//}
