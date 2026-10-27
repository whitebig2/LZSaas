using LZ.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Users.Service.Interface
{
    public interface IInjuryPaymentService
    {
        Task<List<Model.Live_V_InjuryPayment>> List(Expression<Func<Model.Live_V_InjuryPayment, bool>> exp, Model.PagedModel.UserPagedModel query, string order = "");
        Task<int> Add(Model.Live_InjuryPayment model);
        Task<bool> Update(Model.Live_InjuryPayment model);
        Task<bool> Exist(Expression<Func<Model.Live_InjuryPayment, bool>> exp);
        Task<bool> ExistID(int id);
        Task<bool> Delete(int id);
        Task<bool> Delete(Expression<Func<Model.Live_InjuryPayment, bool>> exp);
        Task<bool> UpdateStatus(int id, int status, string msg = "");
        Task<Model.Live_InjuryPayment> Get(int id);
        Task<Model.Live_InjuryPayment> GetByInjuryID(int id);
        byte[] ExportExcel(List<Live_V_InjuryPayment> list);
    }
}
