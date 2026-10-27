using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace LZ.Users.Service.Interface
{
    public interface IEntryRecordService
    {
        Task<List<Model.Live_V_UserEntryRecord>> List(Expression<Func<Model.Live_V_UserEntryRecord, bool>> exp, UserPagedModel query);
        Task<int> Add(Model.Live_UserEntryRecord model);
        Task<bool> Delete(int id);
    }
}
