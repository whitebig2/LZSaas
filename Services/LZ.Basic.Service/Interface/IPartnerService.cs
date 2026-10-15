using System.Linq.Expressions;
using LZ.Model;
using LZ.Model.PagedModel;

namespace LZ.Basic.Service.Interface
{
    public interface IPartnerService
    {
        Task<List<Model.Live_Partner>> List(Expression<Func<Model.Live_Partner, bool>> exp, UserPagedModel query);
        Task<List<Live_V_PartnerCustomer>> CustomerList(Expression<Func<Live_V_PartnerCustomer, bool>> exp, UserPagedModel query);
        Task<int> Add(Model.Live_Partner model);
        Task<bool> Update(Model.Live_Partner model);
        Task<bool> ChangeIntegral(Guid partnerGID, int integral);
        Task<bool> Delete(int id, int companyId = 0);
        Task<bool> Exist(int id, int companyId = 0);
        Task<bool> Exist(Expression<Func<Model.Live_Partner, bool>> exp);
        Task<bool> Set(int id, bool enabled, int companyId = 0);
        Task<Model.Live_Partner> Get(int id, int companyId = 0);
        Task<Live_Partner> Get(Expression<Func<Live_Partner, bool>> exp);
    }
}
