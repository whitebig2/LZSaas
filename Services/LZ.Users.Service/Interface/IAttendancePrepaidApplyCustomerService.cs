using LZ.Model.PagedModel;
using LZ.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Users.Service.Interface
{
    public interface IAttendancePrepaidApplyCustomerService
    {
        Task<int> Add(Live_AttendancePrepaidApplyCustomer model);
        Task<int> Add(List<Live_AttendancePrepaidApplyCustomer> list);
        Task<bool> Delete(int id);

        Task<bool> Delete(Expression<Func<Live_AttendancePrepaidApplyCustomer, bool>> exp);
        Task<bool> ExistID(int id);
        Task<bool> Exist(Expression<Func<Model.Live_AttendancePrepaidApplyCustomer, bool>> exp);
        Task<Live_AttendancePrepaidApplyCustomer> Get(int id);
        Task<Live_AttendancePrepaidApplyCustomer> Get(Expression<Func<Live_AttendancePrepaidApplyCustomer, bool>> exp);
        Task<List<Live_AttendancePrepaidApplyCustomer>> List(Expression<Func<Live_AttendancePrepaidApplyCustomer, bool>> exp, UserPagedModel query);

        Task<bool> Update(Live_AttendancePrepaidApplyCustomer model);
        Task<int> DictionarySet(Dictionary<string, object> dt);
    }
}
