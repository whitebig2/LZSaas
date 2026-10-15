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

namespace LZ.Customer.Service.Services
{
    public class BusinessInsuranceRecordService : ServicesBase<Live_BusinessInsuranceRecord>, IBusinessInsuranceRecordService, IScopedDependency
    {


        public async Task<List<Live_BusinessInsuranceRecord>> PageList(Expression<Func<Live_BusinessInsuranceRecord, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var list = await base.Context.Queryable<Live_BusinessInsuranceRecord>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<List<Live_BusinessInsuranceRecord>> List(Expression<Func<Live_BusinessInsuranceRecord, bool>> exp)
        {
            return await base.Context.Queryable<Live_BusinessInsuranceRecord>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }
        public async Task<List<Live_V_BusinessInsuranceRecord>> ListView(Expression<Func<Live_V_BusinessInsuranceRecord, bool>> exp)
        {
            return await base.Context.Queryable<Live_V_BusinessInsuranceRecord>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }

        public async Task<Live_BusinessInsuranceRecord> Get(Expression<Func<Live_BusinessInsuranceRecord, bool>> exp)
        {
            return await base.Context.Queryable<Live_BusinessInsuranceRecord>().Where(exp).OrderByDescending(it => it.CreateTime).FirstAsync();
        }
        public async Task<int> Add(Live_BusinessInsuranceRecord model)
        {
            return await base.Context.Insertable(model).ExecuteCommandAsync();
        }

        public async Task<int> Add(List<Live_BusinessInsuranceRecord> models)
        {
            return await base.Context.Insertable(models).ExecuteCommandAsync();
        }
       
       
        public async Task<bool> Update(Live_BusinessInsuranceRecord model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Update(List<Live_BusinessInsuranceRecord> models)
        {
            return await base.Context.Updateable(models).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }


       


     
        public async Task<bool> Delete(Guid id)
        {
            return await base.Context.Deleteable<Live_BusinessInsuranceRecord>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Delete(Expression<Func<Live_BusinessInsuranceRecord, bool>> exp)
        {
            return await base.Context.Deleteable<Live_BusinessInsuranceRecord>().Where(exp).ExecuteCommandAsync() > 0;
        }
    }
}
