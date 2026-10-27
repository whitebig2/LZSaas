using LZ.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Finance.Service.Interface
{
    public interface IPrepaidInterestRecordService
    {
        Task<bool> Add(List<Live_PrepaidInterestRecord> models);
        Task<bool> Delete(Expression<Func<Live_PrepaidInterestRecord, bool>> exp);
        Task<List<Live_PrepaidInterestRecord>> List(Expression<Func<Live_PrepaidInterestRecord, bool>> exp);
    }
}
