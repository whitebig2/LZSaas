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
    public interface IBusinessInsurancePushService
    {
        Task<bool> Add(List<Live_BusinessInsurancePush> models);
        Task<bool> AddNav(List<Live_BusinessInsurancePushExtension> models);
        Task<bool> DeleteNav(Live_BusinessInsurancePushExtension model);
        Task<Live_BusinessInsurancePush> Get(Expression<Func<Live_BusinessInsurancePush, bool>> exp);
        Task<Live_BusinessInsurancePushExtension> GetNav(Expression<Func<Live_BusinessInsurancePushExtension, bool>> exp);
        Task<List<Live_BusinessInsurancePush>> List(Expression<Func<Live_BusinessInsurancePush, bool>> exp);
        Task<List<Live_BusinessInsurancePushExtension>> ListNav(Expression<Func<Live_BusinessInsurancePushExtension, bool>> exp);
        Task<List<Live_V_BusinessInsurancePush>> PageList(Expression<Func<Live_V_BusinessInsurancePush, bool>> exp, PagedModelBase query);
        Task<bool> Update(Live_BusinessInsurancePush model);
        Task<bool> Update(List<Live_BusinessInsurancePush> models);
        Task<bool> UpdateNav(Live_BusinessInsurancePushExtension model);
    }
}
