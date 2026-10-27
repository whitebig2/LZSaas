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
    public interface IAttendanceDayWorkApplyService
    {
        Task<int> Add(Live_AttendanceDayWorkApply model);
        Task<int> Add(List<Live_AttendanceDayWorkApply> list);
        Task<bool> Delete(int id);

        Task<bool> Delete(Expression<Func<Live_AttendanceDayWorkApply, bool>> exp);
        Task<bool> ExistID(int id);
        Task<bool> Exist(Expression<Func<Model.Live_AttendanceDayWorkApply, bool>> exp);
        Task<Live_AttendanceDayWorkApply> Get(int id);
        Task<Live_AttendanceDayWorkApply> GetV(int id);
        Task<List<Live_AttendanceDayWorkApply>> List(Expression<Func<Live_AttendanceDayWorkApply, bool>> exp, UserPagedModel query);

        Task<bool> Update(Live_AttendanceDayWorkApply model);
        Task<int> DictionarySet(Dictionary<string, object> dt);
        Task<bool> UpdateStatus(int id, int status, string msg = "");
    }
}
