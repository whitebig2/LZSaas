using LZ.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Statistics.Service.Interface
{
    public interface IContractStatisticsService
    {
        Task<int> ContractSignerCount(Expression<Func<Live_ContractSigner, bool>> exp = null);
        Task<int> EntryContractCount(Expression<Func<Live_V_ContractRecord, bool>> exp = null);
        Task<int> GetCaAuthCount(Expression<Func<Live_CaAuthIdentity, bool>> exp = null);
        Task<Live_CaAuthIdentity> GetCaAuthIdentity(Expression<Func<Live_CaAuthIdentity, bool>> exp = null);
        Task<Live_ContractSigner> GetContractSigner(Expression<Func<Live_ContractSigner, bool>> exp = null);
        Task<Live_ContractRecord> GetEntryContractRecord(Expression<Func<Live_ContractRecord, bool>> exp = null);
        Task<List<Live_V_ContractRecord>> ListEntryContract(Expression<Func<Live_V_ContractRecord, bool>> exp = null);
    }
}
