using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Users.Service.Interface
{
    public interface IInjuryLoanService
    {
        Task<List<Model.Live_V_InjuryLoan>> List(Expression<Func<Model.Live_V_InjuryLoan, bool>> exp, Model.PagedModel.UserPagedModel query, string order = "");
        Task<int> Add(Model.Live_InjuryLoan model);
        Task<Model.Live_V_InjuryLoan> AddAndGet(Model.Live_InjuryLoan model);
        Task<bool> Update(Model.Live_InjuryLoan model);
        Task<bool> Exist(Expression<Func<Model.Live_InjuryLoan, bool>> exp);
        Task<bool> UpdateStatus(int id, int status, string msg = "");
        Task<bool> ExistID(int id);
        Task<bool> Delete(int id);
        Task<Model.Live_InjuryLoan> Get(int id);
        Task<Model.Live_V_InjuryLoan> View(int id);
    }
}
