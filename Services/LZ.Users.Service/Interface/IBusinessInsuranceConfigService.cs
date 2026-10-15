using LZ.Model;
using LZ.Model.ModelExtensions;
using LZ.Model.PagedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Users.Service.Interface
{
    public interface IBusinessInsuranceConfigService
    {
        Task<int> Add(Live_BusinessInsuranceConfig model);
        Task<bool> AddNav(Live_BusinessInsuranceConfigExtension model);
        Task<bool> Delete(Guid id);
        Task<bool> DeleteNav(Guid id);
        Task<bool> Exist(Guid id);
        Task<bool> Exist(Expression<Func<Live_BusinessInsuranceConfig, bool>> exp);
        Task<Live_BusinessInsuranceConfig> Get(Guid id);
        Task<Live_BusinessInsuranceConfig> Get(Expression<Func<Live_BusinessInsuranceConfig, bool>> exp);
        Task<Live_BusinessInsuranceConfigExtension> GetNav(Expression<Func<Live_BusinessInsuranceConfigExtension, bool>> exp);
        Task<Live_V_BusinessInsuranceConfig> GetView(Expression<Func<Live_V_BusinessInsuranceConfig, bool>> exp);
        Task<List<Live_BusinessInsuranceConfig>> List(Expression<Func<Live_BusinessInsuranceConfig, bool>> exp, PagedModelBase query);
        Task<List<Live_BusinessInsuranceConfig>> List(Expression<Func<Live_BusinessInsuranceConfig, bool>> exp);
        Task<List<Live_V_BusinessInsuranceConfig>> ListView(Expression<Func<Live_V_BusinessInsuranceConfig, bool>> exp);
        Task<List<Live_V_BusinessInsuranceConfig>> PageList(Expression<Func<Live_V_BusinessInsuranceConfig, bool>> exp, PagedModelBase query);
        Task<bool> Update(Live_BusinessInsuranceConfig model);
        Task<bool> UpdateNav(Live_BusinessInsuranceConfigExtension model);
    }
}
