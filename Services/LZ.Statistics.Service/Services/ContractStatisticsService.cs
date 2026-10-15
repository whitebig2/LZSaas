using LZ.Core;
using LZ.Core.EventData;
using LZ.Core.Interface;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Core.Redis;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Statistics.Service.Interface;
using SqlSugar;
using SqlSugar.IOC;
using System.Linq.Expressions;

namespace LZ.Statistics.Service.Services
{
    public class ContractStatisticsService : ServicesBase<Model.Live_ContractRecord>, IContractStatisticsService, IScopedDependency
    {
        /// <summary>
        ///  现金签收单
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public async Task<int> EntryContractCount(Expression<Func<Live_V_ContractRecord, bool>> exp=null)
        {
            var qb = base.Context.Queryable<Live_V_ContractRecord>();
            return await qb.CountAsync(exp);
        }

        public async Task<Live_ContractRecord> GetEntryContractRecord(Expression<Func<Live_ContractRecord, bool>> exp = null)
        {
            return await base.Context.Queryable<Live_ContractRecord>().Where(exp).OrderByDescending(f=>f.CreateTime).FirstAsync();
        }
        public async Task<List<Live_V_ContractRecord>> ListEntryContract(Expression<Func<Live_V_ContractRecord, bool>> exp = null)
        {
            return await base.Context.Queryable<Live_V_ContractRecord>().Where(exp).ToListAsync();
        }


        public async Task<int> ContractSignerCount(Expression<Func<Live_ContractSigner, bool>> exp = null)
        {
            var qb = base.Context.Queryable<Live_ContractSigner>();
            return await qb.CountAsync(exp);
        }

        public async Task<Live_ContractSigner> GetContractSigner(Expression<Func<Live_ContractSigner, bool>> exp = null)
        {
            return await base.Context.Queryable<Live_ContractSigner>().Where(exp).OrderByDescending(f => f.CreateTime).FirstAsync() ;
        }


        public async Task<int> GetCaAuthCount(Expression<Func<Live_CaAuthIdentity, bool>> exp = null)
        {
            var qb = base.Context.Queryable<Live_CaAuthIdentity>();
            return await qb.CountAsync(exp);
        }
        public async Task<Live_CaAuthIdentity> GetCaAuthIdentity(Expression<Func<Live_CaAuthIdentity, bool>> exp = null)
        {
            var qb = base.Context.Queryable<Live_CaAuthIdentity>().OrderByDescending(f=>f.CreateTime);
            return await qb.FirstAsync(exp);
        }

    }
}
