using LZ.Model;
using LZ.Model.ModelExtensions;
using LZ.Model.PagedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Customer.Service.Interface
{
    public interface IContractFileConfigService
    {
        Task<int> Add(Live_ContractFileConfig model);
        Task<bool> AddNav(Live_ContractFileConfigExtension model);
        Task<bool> Delete(Guid id);
        Task<bool> DeleteNav(Live_ContractFileConfigExtension model);
        Task<bool> Exist(Guid id);
        Task<Live_ContractFileConfig> Get(Guid id);
        Task<Live_ContractFileConfig> Get(Expression<Func<Live_ContractFileConfig, bool>> exp);
        Task<Live_ContractFileConfigExtension> GetNav(Expression<Func<Live_ContractFileConfigExtension, bool>> exp);
        Task<Live_V_UserEntry> GetUserEntry(Expression<Func<Live_V_UserEntry, bool>> exp);
        Task<Live_UserEntry> GetUserEntry1(Expression<Func<Live_UserEntry, bool>> exp);
        Task<Live_V_CustomerContractConfig> GetView(Expression<Func<Live_V_CustomerContractConfig, bool>> exp);
        Task<List<Live_ContractFileConfig>> List(Expression<Func<Live_ContractFileConfig, bool>> exp, PagedModelBase query);
        Task<List<Live_V_UserEntry>> ListUserEntryView(Expression<Func<Live_V_UserEntry, bool>> exp);
        Task<bool> Update(Live_ContractFileConfig model);
        Task<int> UpdateContract(Live_UserEntry model);
        Task<bool> UpdateNav(Live_ContractFileConfigExtension model);
    }
}
