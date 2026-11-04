using LZ.Model;
using LZ.Model.PagedModel;
using System.Linq.Expressions;

namespace LZ.Invoice.Service.Interface
{
    public interface IPartnerAuthCodeService
    {
        Task<List<FIN_PartnerAuthCode>> List(Expression<Func<FIN_PartnerAuthCode, bool>> exp, PagedModelBase query);

        Task<List<FIN_PartnerAuthCode>> List(Expression<Func<FIN_PartnerAuthCode, bool>> exp);

        Task<int> Add(FIN_PartnerAuthCode model);

        Task<int> Add(List<FIN_PartnerAuthCode> models);

        Task<bool> Update(FIN_PartnerAuthCode model);

        Task<bool> Update(List<FIN_PartnerAuthCode> models);

        Task<bool> Delete(int id);

        Task<bool> Delete(Expression<Func<FIN_PartnerAuthCode, bool>> exp);

        Task<bool> Exist(int id);

        Task<bool> Exist(Expression<Func<FIN_PartnerAuthCode, bool>> exp);

        Task<FIN_PartnerAuthCode> Get(int id);

        Task<FIN_PartnerAuthCode> Get(Expression<Func<FIN_PartnerAuthCode, bool>> exp);

        Task<int> DictionarySet(Dictionary<string, object> dt);
    }
}
