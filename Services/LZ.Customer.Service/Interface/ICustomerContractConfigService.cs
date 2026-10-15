using System.Linq.Expressions;
using LZ.Model;
using LZ.Model.ModelExtensions;
using LZ.Model.PagedModel;

namespace LZ.Customer.Service.Interface
{
    public interface ICustomerContractConfigService
    {
        Task<List<Model.Live_CustomerContractConfig>> List(Expression<Func<Model.Live_CustomerContractConfig, bool>> exp, PagedModelBase query);

        Task<int> Add(Model.Live_CustomerContractConfig model);

        Task<bool> Update(Model.Live_CustomerContractConfig model);
        Task<bool> Delete(Guid id);
        Task<bool> Exist(Guid id);

        Task<Model.Live_CustomerContractConfig> Get(Guid id);
        Task<Live_CustomerContractConfig> Get(Expression<Func<Live_CustomerContractConfig, bool>> exp);
        Task<Live_CustomerContractConfig> GetByCustomerGID(Guid CustomerGID);
    }
}
