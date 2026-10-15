using LZ.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Users.Service.Interface
{
    public interface IInjuryAdvanceAccountLogService
    {
        Task<List<Model.Live_InjuryAdvanceAccountLog>> List(Expression<Func<Model.Live_InjuryAdvanceAccountLog, bool>> exp, Model.PagedModel.UserPagedModel query, string order = "");
        Task<int> Add(Model.Live_InjuryAdvanceAccountLog model);
        Task<bool> Update(Model.Live_InjuryAdvanceAccountLog model);
        Task<bool> Exist(Expression<Func<Model.Live_InjuryAdvanceAccountLog, bool>> exp);
        Task<bool> ExistID(int id);
        Task<bool> Delete(int id);
        Task<bool> Delete(Expression<Func<Model.Live_InjuryAdvanceAccountLog, bool>> exp);
        Task<Model.Live_InjuryAdvanceAccountLog> Get(int id);
        Task<Live_InjuryAdvanceAccountLog> Get(Expression<Func<Live_InjuryAdvanceAccountLog, bool>> exp);
    }
}
