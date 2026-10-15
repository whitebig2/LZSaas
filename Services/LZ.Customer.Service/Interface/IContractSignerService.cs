using LZ.Model;
using LZ.Model.PagedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Customer.Service.Interface
{
    public interface IContractSignerService
    {
        Task<int> Add(Live_ContractSigner model);
        Task<bool> Delete(Guid id);
        Task<bool> Exist(Guid id);
        Task<Live_ContractSigner> Get(Guid id);
        Task<Live_ContractSigner> Get(Expression<Func<Live_ContractSigner, bool>> exp);
        Task<Live_ContractSigner> GetSigner(string name, string idno, bool isCompany);
        Task<List<Live_ContractSigner>> List(Expression<Func<Live_ContractSigner, bool>> exp);
        Task<List<Live_ContractSigner>> PageList(Expression<Func<Live_ContractSigner, bool>> exp, PagedModelBase query);
        Task<bool> Update(Live_ContractSigner model);
        Task<bool> Update(List<Live_ContractSigner> models);
    }
}
