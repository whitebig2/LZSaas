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
    public interface IInsuranceCustomerConfigService
    {
        Task<int> Add(Live_InsuranceCustomerConfig model);
        Task<bool> Delete(Guid id);
        Task<bool> Exist(Guid id);
        Task<Live_InsuranceCustomerConfig> Get(Guid id);
        Task<Live_InsuranceCustomerConfig> Get(Expression<Func<Live_InsuranceCustomerConfig, bool>> exp);
        Task<Live_V_InsuranceCustomerConfig> GetView(Expression<Func<Live_V_InsuranceCustomerConfig, bool>> exp);
        Task<List<Live_InsuranceCustomerConfig>> List(Expression<Func<Live_InsuranceCustomerConfig, bool>> exp, PagedModelBase query);
        Task<List<Live_V_InsuranceCustomerConfig>> ListView(Expression<Func<Live_V_InsuranceCustomerConfig, bool>> exp, PagedModelBase query);
        Task<List<Live_V_InsuranceCustomerConfig>> ListView(Expression<Func<Live_V_InsuranceCustomerConfig, bool>> exp);
        Task<bool> Update(Live_InsuranceCustomerConfig model);
    }
}
