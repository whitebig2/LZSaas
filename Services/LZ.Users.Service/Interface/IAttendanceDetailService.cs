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
    public interface IAttendanceDetailService
    {
        Task<int> Add(Live_AttendanceDetail model);
        Task<int> Add(List<Live_AttendanceDetail> list);
        Task<bool> Delete(int id);

        Task<bool> Delete(Expression<Func<Live_AttendanceDetail, bool>> exp);
        Task<bool> ExistID(int id);
        Task<bool> Exist(Expression<Func<Model.Live_AttendanceDetail, bool>> exp);
        Task<Live_AttendanceDetail> Get(int id);
        Task<Live_AttendanceDetail> Get(Expression<Func<Model.Live_AttendanceDetail, bool>> exp);
        Task<List<Live_AttendanceDetail>> List(Expression<Func<Live_AttendanceDetail, bool>> exp, UserPagedModel query);

        Task<bool> Update(Live_AttendanceDetail model);
        Task<int> DictionarySet(Dictionary<string, object> dt);
    }
}
