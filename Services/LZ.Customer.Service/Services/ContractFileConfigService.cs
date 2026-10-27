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
using LZ.Customer.Service.Interface;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Model.FxqModel;
using LZ.Model.ModelExtensions;

namespace LZ.Customer.Service.Services
{
    public class ContractFileConfigService:ServicesBase<Live_ContractFileConfig>, IContractFileConfigService, IScopedDependency
    {

        public async Task<List<Live_ContractFileConfig>> List(Expression<Func<Live_ContractFileConfig, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_ContractFileConfig>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_ContractFileConfig>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<bool> AddNav(Live_ContractFileConfigExtension model)
        {
           return await base.Context.InsertNav(model).Include(f=>f.Signers).ExecuteCommandAsync();
        }

        public async Task<bool> UpdateNav(Live_ContractFileConfigExtension model)
        {
            return await base.Context.UpdateNav(model).Include(f => f.Signers).ExecuteCommandAsync();
        }
        public async Task<bool> DeleteNav(Live_ContractFileConfigExtension model)
        {
            return await base.Context.DeleteNav(model).Include(f=>f.Signers).ExecuteCommandAsync();
        }
        public async Task<Live_ContractFileConfigExtension> GetNav(Expression<Func<Live_ContractFileConfigExtension, bool>> exp )
        {
            return await base.Context.Queryable<Live_ContractFileConfigExtension>().Where(exp).Includes(f => f.Signers).FirstAsync();
        }

        public async Task<Live_V_CustomerContractConfig> GetView(Expression<Func<Live_V_CustomerContractConfig, bool>> exp)
        {
            return await base.Context.Queryable<Live_V_CustomerContractConfig>().Where(exp).FirstAsync();
        }

        public async Task<int> Add(Live_ContractFileConfig model)
        {
            return await base.Context.Insertable(model).ExecuteCommandAsync();
        }
        public async Task<bool> Exist(Guid id)
        {
            return await base.Context.Queryable<Live_ContractFileConfig>().AnyAsync(it => it.ID == id);
        }
        public async Task<bool> Update(Live_ContractFileConfig model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public async Task<Live_ContractFileConfig> Get(Guid id)
        {
            return await base.Context.Queryable<Live_ContractFileConfig>().FirstAsync(it => it.ID == id);
        }

        public async Task<Live_ContractFileConfig> Get(Expression<Func<Live_ContractFileConfig, bool>> exp)
        {
            return await base.Context.Queryable<Live_ContractFileConfig>().Where(exp).FirstAsync();
        }
        public async Task<bool> Delete(Guid id)
        {
            return await base.Context.Deleteable<Live_ContractFileConfig>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }

        /// <summary>
        /// 临时使用
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public async Task<Live_V_UserEntry> GetUserEntry(Expression<Func<Live_V_UserEntry, bool>> exp)
        {
            return await base.Context.Queryable<Live_V_UserEntry>().Where(exp).FirstAsync();
        }



        /// <summary>
        /// 临时使用
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public async Task<List<Live_V_UserEntry>> ListUserEntryView(Expression<Func<Live_V_UserEntry, bool>> exp)
        {
            return await base.Context.Queryable<Live_V_UserEntry>().Where(exp).ToListAsync();
        }

        public async Task<Live_UserEntry> GetUserEntry1(Expression<Func<Live_UserEntry, bool>> exp)
        {
            return await base.Context.Queryable<Live_UserEntry>().Where(exp).FirstAsync();
        }
        public async Task<int> UpdateContract(Live_UserEntry model)
        {
            return await base.Context.Updateable(model).UpdateColumns(t => new { t.RetireDate, t.ContractDate, t.ContractStartDate, t.ContractEndDate,t.ContractStatus }).ExecuteCommandAsync();
        }

    }
}
