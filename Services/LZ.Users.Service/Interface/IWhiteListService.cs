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
    public interface IWhiteListService
    {
        Task<List<Model.Live_WhiteList>> List(Expression<Func<Model.Live_WhiteList, bool>> exp, UserPagedModel query, string order = "");
        Task<List<Model.Live_V_WhiteList>> ListView(Expression<Func<Model.Live_V_WhiteList, bool>> exp, UserPagedModel query, string order = "");
        Task<int> Add(Model.Live_WhiteList model);
        Task<bool> Update(Model.Live_WhiteList model);

        Task<bool> Update(List<Model.Live_WhiteList> list);
        Task<bool> Disabled(string cardID,bool disabled);
        Task<bool> Exist(Expression<Func<Model.Live_WhiteList, bool>> exp);
        Task<bool> ExistID(int id);
        Task<bool> Delete(int id);

        Task<bool> DeleteList(List<Live_WhiteList> list);
        Task<Model.Live_WhiteList> Get(int id);
        Task<int> Count(Expression<Func<Model.Live_WhiteList, bool>> exp);
        Task<Model.Live_WhiteList> Get(Expression<Func<Live_WhiteList, bool>> exp);

        byte[] ExportExcel(List<Model.Live_WhiteList> list);
    }
}
