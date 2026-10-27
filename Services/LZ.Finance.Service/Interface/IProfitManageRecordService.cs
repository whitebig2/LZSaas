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
    public interface IProfitManageRecordService
    {
        Task<int> Add(Live_ProfitManageRecord model);
        Task<int> Add(List<Live_ProfitManageRecord> models);
        Task<bool> Delete(Guid id);
        Task<bool> Delete(Expression<Func<Live_ProfitManageRecord, bool>> exp);
        Task<bool> Exist(Guid id);
        Task<Live_ProfitManageRecord> Get(Expression<Func<Live_ProfitManageRecord, bool>> exp);
        Task<List<Live_ProfitManageRecord>> List(Expression<Func<Live_ProfitManageRecord, bool>> exp);
        Task<List<Live_ProfitManageRecord>> PageList(Expression<Func<Live_ProfitManageRecord, bool>> exp, PagedModelBase query);
        Task<decimal> SumBusinessAmount(Expression<Func<Live_ProfitManageRecord, bool>> exp);
        Task<bool> Update(Live_ProfitManageRecord model);
        Task<bool> Update(List<Live_ProfitManageRecord> models);
    }
}
