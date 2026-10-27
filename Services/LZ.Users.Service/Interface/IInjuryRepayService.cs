using LZ.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Users.Service.Interface
{
    public interface IInjuryRepayService
    {
        Task<List<Model.Live_V_InjuryRepay>> List(Expression<Func<Model.Live_V_InjuryRepay, bool>> exp, Model.PagedModel.UserPagedModel query, string order = "");
        Task<long> Add(Model.Live_InjuryRepay model);
        Task<bool> Update(Model.Live_InjuryRepay model);
        Task<bool> Exist(Expression<Func<Model.Live_InjuryRepay, bool>> exp);
        Task<bool> ExistID(int id);
        Task<bool> UpdateStatus(int id, int status, string msg = "");
        Task<bool> Delete(int id);
        Task<Model.Live_InjuryRepay> Get(int id);
        Task<Live_V_InjuryRepay> View(Expression<Func<Live_V_InjuryRepay, bool>> exp);
        Task<Model.Live_V_InjuryRepay> View(int id, string statusArray ,int companyId = 0);
        
    }
}
