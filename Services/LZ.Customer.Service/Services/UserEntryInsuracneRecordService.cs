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
    public class UserEntryInsuracneRecordService : ServicesBase<Live_UserEntryInsuracneRecord>, IUserEntryInsuracneRecordService, IScopedDependency
    {


        public async Task<List<Live_UserEntryInsuracneRecord>> PageList(Expression<Func<Live_UserEntryInsuracneRecord, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_UserEntryInsuracneRecord>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_UserEntryInsuracneRecord>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<List<Live_UserEntryInsuracneRecord>> List(Expression<Func<Live_UserEntryInsuracneRecord, bool>> exp)
        {
            return await base.Context.Queryable<Live_UserEntryInsuracneRecord>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }

        public async Task<Live_UserEntryInsuracneRecord> Get(Expression<Func<Live_UserEntryInsuracneRecord, bool>> exp)
        {
            return await base.Context.Queryable<Live_UserEntryInsuracneRecord>().Where(exp).OrderByDescending(it => it.CreateTime).FirstAsync();
        }
        public async Task<int> Add(Live_UserEntryInsuracneRecord model)
        {
            return await base.Context.Insertable(model).ExecuteCommandAsync();
        }

        public async Task<int> Add(List<Live_UserEntryInsuracneRecord> models)
        {
            return await base.Context.Insertable(models).ExecuteCommandAsync();
        }
       
       
        public async Task<bool> Update(Live_UserEntryInsuracneRecord model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Update(List<Live_UserEntryInsuracneRecord> models)
        {
            return await base.Context.Updateable(models).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }


       


     
        public async Task<bool> Delete(Guid id)
        {
            return await base.Context.Deleteable<Live_UserEntryInsuracneRecord>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Delete(Expression<Func<Live_UserEntryInsuracneRecord, bool>> exp)
        {
            return await base.Context.Deleteable<Live_UserEntryInsuracneRecord>().Where(exp).ExecuteCommandAsync() > 0;
        }


        //public async Task<List<Live_UserEntry>> ListEntry(Expression<Func<Live_UserEntry, bool>> exp)
        //{
        //    return await base.Context.Queryable<Live_UserEntry>().Where(exp).ToListAsync();
        //}

        //public async Task<List<Live_V_UserEntry>> ListEntryView(Expression<Func<Live_V_UserEntry, bool>> exp)
        //{
        //    return await base.Context.Queryable<Live_V_UserEntry>().Where(exp).ToListAsync();
        //}

        //public async Task<Live_UserEntry> GetEntry(Expression<Func<Live_UserEntry, bool>> exp)
        //{
        //    return await base.Context.Queryable<Live_UserEntry>().Where(exp).FirstAsync();
        //}

        //public async Task<int> UpdateEntry(Live_UserEntry entryModel)
        //{
        //    return await base.Context.Updateable<Live_UserEntry>(entryModel).UpdateColumns(f => new {f.RetireDate, f.ContractDate, f.ContractStartDate, f.ContractEndDate }).ExecuteCommandAsync();
        //}
    }
}
