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
    public interface IUpdateRecordService
    {
        Task<List<Model.Live_UpdateRecord>> List(Expression<Func<Model.Live_UpdateRecord, bool>> exp, BasicPagedModel query, string order = "");
        Task<int> Add(Model.Live_UpdateRecord model);

        Task<int> Adds(List<Model.Live_UpdateRecord> models);
        Task<bool> Update(Model.Live_UpdateRecord model);
        Task<bool> Exist(Expression<Func<Model.Live_UpdateRecord, bool>> exp);
        Task<bool> ExistID(int id);
        Task<bool> Delete(int id);
        Task<Model.Live_UpdateRecord> Get(int id);
        Task<Model.Live_UpdateRecord> Get(Expression<Func<Model.Live_UpdateRecord, bool>> exp);
    }
}
