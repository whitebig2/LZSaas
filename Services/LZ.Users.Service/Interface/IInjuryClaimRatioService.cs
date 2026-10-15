using LZ.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Users.Service.Interface
{
    public interface IInjuryClaimRatioService
    {
        Task<List<Model.Live_InjuryClaimRatio>> List(Expression<Func<Model.Live_InjuryClaimRatio, bool>> exp, Model.PagedModel.UserPagedModel query, string order = "");
        Task<List<Model.Live_V_InjuryClaimRatio>> ListView(Expression<Func<Model.Live_V_InjuryClaimRatio, bool>> exp, Model.PagedModel.UserPagedModel query, string order = "");
        Task<int> Add(Model.Live_InjuryClaimRatio model);

        Task<int> Add(List<Model.Live_InjuryClaimRatio> list);
        Task<bool> Update(Model.Live_InjuryClaimRatio model);
        Task<bool> Exist(Expression<Func<Model.Live_InjuryClaimRatio, bool>> exp);
        Task<bool> ExistID(int id);
        Task<bool> Delete(int id);
        Task<bool> Delete(Expression<Func<Model.Live_InjuryClaimRatio, bool>> exp);
        Task<Model.Live_InjuryClaimRatio> Get(int id);
        Task<Live_InjuryClaimRatio> Get(Expression<Func<Live_InjuryClaimRatio, bool>> exp);
        Task<int> DictionarySet(Dictionary<string, object> dt);
    }
}
