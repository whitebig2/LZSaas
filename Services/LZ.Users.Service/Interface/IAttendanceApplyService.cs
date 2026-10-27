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
    public interface IAttendanceApplyService
    {
        Task<int> Add(Live_AttendanceApply model);
        Task<int> Add(List<Live_AttendanceApply> list);
        Task<bool> Delete(int id);

        Task<bool> Delete(Expression<Func<Live_AttendanceApply, bool>> exp);
        Task<bool> ExistID(int id);
        Task<bool> Exist(Expression<Func<Model.Live_AttendanceApply, bool>> exp);
        Task<Live_AttendanceApply> Get(int id);
        Task<Live_V_AttendanceApply> GetV(int id);
        Task<List<Live_V_AttendanceApply>> List(Expression<Func<Live_V_AttendanceApply, bool>> exp, UserPagedModel query);

        Task<bool> Update(Live_AttendanceApply model);
        Task<int> DictionarySet(Dictionary<string, object> dt);

        Task<int> Count(Expression<Func<Model.Live_AttendanceApply, bool>> exp);
    }
}
