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
    public interface IAttendanceStructureService
    {
        Task<int> Add(Live_AttendanceStructure model);
        Task<int> Add(List<Live_AttendanceStructure> list);
        Task<bool> Delete(int id);
        Task<bool> ExistID(int id);

        Task<Live_AttendanceStructure> Get(int id);
        Task<bool> Exist(Expression<Func<Model.Live_AttendanceStructure, bool>> exp);
        Task<Live_AttendanceStructure> Get(Expression<Func<Model.Live_AttendanceStructure, bool>> exp);
        Task<Live_V_AttendanceStructure> GetView(Expression<Func<Model.Live_V_AttendanceStructure, bool>> exp);
        Task<List<Live_V_AttendanceStructure>> List(Expression<Func<Live_V_AttendanceStructure, bool>> exp, UserPagedModel query);

        Task<bool> Update(Live_AttendanceStructure model);
        Task<int> DictionarySet(Dictionary<string, object> dt);

    }
}
