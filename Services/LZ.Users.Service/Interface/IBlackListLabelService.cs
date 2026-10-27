using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LZ.Model.PagedModel;

namespace LZ.Users.Service.Interface
{
    public interface IBlackListLabelService
    {
        Task<List<Model.Live_BlackListLabel>> List(Expression<Func<Model.Live_BlackListLabel, bool>> exp, UserPagedModel query,string order="");
        Task<int> Add(Model.Live_BlackListLabel model);
        Task<bool> Update(Model.Live_BlackListLabel model);
        Task<bool> Exist(Expression<Func<Model.Live_BlackListLabel, bool>> exp);
        Task<bool> ExistID(int id);
        Task<bool> Delete(int id);
        Task<Model.Live_BlackListLabel> Get(int id);
    }
}
