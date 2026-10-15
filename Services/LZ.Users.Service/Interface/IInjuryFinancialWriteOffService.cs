using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Users.Service.Interface
{
    public interface IInjuryFinancialWriteOffService
    {
        Task<List<Model.Live_V_InjuryFinancialWriteOff>> List(Expression<Func<Model.Live_V_InjuryFinancialWriteOff, bool>> exp, Model.PagedModel.UserPagedModel query, string order = "");
        Task<int> Add(Model.Live_InjuryFinancialWriteOff model);
        Task<bool> Update(Model.Live_InjuryFinancialWriteOff model);
        Task<bool> Exist(Expression<Func<Model.Live_InjuryFinancialWriteOff, bool>> exp);
        Task<bool> UpdateStatus(int id, int status, string msg = "");
        Task<bool> ExistID(int id);
        Task<bool> Delete(int id);
        Task<bool> Delete(Expression<Func<Model.Live_InjuryFinancialWriteOff, bool>> exp);
        Task<Model.Live_InjuryFinancialWriteOff> Get(int id);
        Task<Model.Live_V_InjuryFinancialWriteOff> View(int id);
        Task<Model.Live_InjuryFinancialWriteOff> GetByInjuryID(int id);
    }
}
