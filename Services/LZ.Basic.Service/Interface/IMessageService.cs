using LZ.Model.PagedModel;
using LZ.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Basic.Service.Interface
{
    public interface IMessageService
    {
        Task<List<Model.Live_Message>> List(Expression<Func<Model.Live_Message, bool>> exp, BasicPagedModel query, string order = "");
        Task<int> Add(Model.Live_Message model);

        Task<int> Adds(List<Model.Live_Message> models);
        Task<bool> Update(Model.Live_Message model);
        Task<bool> Exist(Expression<Func<Model.Live_Message, bool>> exp);
        Task<bool> ExistID(int id);
        Task<bool> Delete(int id);
        Task<Model.Live_Message> Get(int id);
    }
}
