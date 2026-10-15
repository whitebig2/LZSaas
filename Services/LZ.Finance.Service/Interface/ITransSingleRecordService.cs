using LZ.Model;
using LZ.Model.PagedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Finance.Service.Interface
{
    public interface ITransSingleRecordService
    {
        Task<bool> Add(Live_TransSingleRecord model);
        Task<bool> Add(List<Live_TransSingleRecord> models);
        Task<bool> Delete(Guid id);
        Task<bool> Exist(Guid id);
        Task<Live_TransSingleRecord> Get(Expression<Func<Live_TransSingleRecord, bool>> exp);
        Task<List<Live_TransSingleRecord>> List(Expression<Func<Live_TransSingleRecord, bool>> exp);
        Task<List<Live_TransSingleRecord>> PageList(Expression<Func<Live_TransSingleRecord, bool>> exp, PagedModelBase query);
        Task<bool> Update(Live_TransSingleRecord model);
    }
}
