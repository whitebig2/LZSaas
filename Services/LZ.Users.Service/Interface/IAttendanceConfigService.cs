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
    public interface IAttendanceConfigService
    {
        Task<int> Add(Live_AttendanceConfig model);
        Task<bool> Delete(int id);
        Task<bool> ExistID(int id);
        Task<bool> Exist(Expression<Func<Model.Live_AttendanceConfig, bool>> exp);
        Task<Live_AttendanceConfig> Get(int id);
        Task<List<Live_AttendanceConfig>> List(Expression<Func<Live_AttendanceConfig, bool>> exp, UserPagedModel query);
        Task<List<Live_V_AttendanceConfig>> ListV(Expression<Func<Live_V_AttendanceConfig, bool>> exp, UserPagedModel query);
        Task<bool> Update(Live_AttendanceConfig model);
        Task<int> DictionarySet(Dictionary<string, object> dt);
        long GetSequence();
    }
}
