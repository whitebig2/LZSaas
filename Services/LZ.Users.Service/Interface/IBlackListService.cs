using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LZ.Model;
using LZ.Model.PagedModel;

namespace LZ.Users.Service.Interface
{
    public interface IBlackListService
    {
        Task<List<Model.Live_BlackList>> List(Expression<Func<Model.Live_BlackList, bool>> exp, UserPagedModel query);

        Task<List<Model.Live_V_BlackList>> ListView(Expression<Func<Model.Live_V_BlackList, bool>> exp, UserPagedModel query);
        Task<int> Add(Model.Live_BlackList model);
        Task<bool> Update(Model.Live_BlackList model);
        Task<bool> Exist(Expression<Func<Model.Live_BlackList, bool>> exp);
        Task<bool> ExistID(int id);
        Task<bool> Delete(int id);
        Task<Model.Live_BlackList> Get(int id);
        byte[] ExportExcel(List<Model.Live_V_BlackList> list);
    }
}
