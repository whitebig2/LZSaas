using LZ.Model;
using LZ.Model.PagedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Users.Service.Interface
{
    public interface IBusinessInsuranceOrderEmpoyeeService
    {
        Task<bool> Exist(Expression<Func<Live_BusinessInsuranceOrderEmpoyee, bool>> exp);
        Task<bool> ExistView(Expression<Func<Live_V_BusinessInsuranceOrderEmpoyee, bool>> exp);
        Task<Live_BusinessInsuranceOrderEmpoyee> Get(Expression<Func<Live_BusinessInsuranceOrderEmpoyee, bool>> exp);
        Task<Live_V_BusinessInsuranceOrderEmpoyee> GetView(Expression<Func<Live_V_BusinessInsuranceOrderEmpoyee, bool>> exp);
        Task<List<Live_BusinessInsuranceOrderEmpoyee>> List(Expression<Func<Live_BusinessInsuranceOrderEmpoyee, bool>> exp);
        Task<List<Live_BusinessInsuranceOrderEmpoyee>> PageList(Expression<Func<Live_BusinessInsuranceOrderEmpoyee, bool>> exp, PagedModelBase query);
        Task<bool> Update(Live_BusinessInsuranceOrderEmpoyee model);
    }
}
