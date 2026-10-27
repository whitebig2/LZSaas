using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace LZ.Users.Service.Interface
{
    public interface INoteService
    {
        Task<List<Model.Live_V_UserEntryNote>> List(Expression<Func<Model.Live_V_UserEntryNote, bool>> exp, Model.PagedModel.UserPagedModel query);
        Task<int> Add(Model.Live_UserEntryNote model);
        Task<bool> Delete(int id);
    }
}
