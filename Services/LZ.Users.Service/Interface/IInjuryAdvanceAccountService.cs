using LZ.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Users.Service.Interface
{
    public interface IInjuryAdvanceAccountService
    {
        Task<List<Model.Live_V_InjuryAdvanceAccount>> List(Expression<Func<Model.Live_V_InjuryAdvanceAccount, bool>> exp, Model.PagedModel.UserPagedModel query, string order = "");
        Task<int> Add(Model.Live_InjuryAdvanceAccount model);
        Task<bool> Update(Model.Live_InjuryAdvanceAccount model);
        Task<bool> Exist(Expression<Func<Model.Live_InjuryAdvanceAccount, bool>> exp);
        Task<bool> ExistID(int id);
        Task<bool> Delete(int id);
        Task<bool> Delete(Expression<Func<Model.Live_InjuryAdvanceAccount, bool>> exp);
        Task<Model.Live_InjuryAdvanceAccount> Get(int id);
        Task<Live_InjuryAdvanceAccount> Get(Expression<Func<Live_InjuryAdvanceAccount, bool>> exp);
        Task<Live_V_InjuryAdvanceAccount> GetView(Expression<Func<Live_V_InjuryAdvanceAccount, bool>> exp);
        Task<bool> ChangeMoney(int id, decimal money);
        Task<int> DictionarySet(Dictionary<string, object> dt);
    }
}
