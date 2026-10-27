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
    public interface IPrepaidRepaymentRecordService
    {
        Task<bool> Delete(Expression<Func<Live_PrepaidRepaymentRecord, bool>> exp);
        Task<bool> Exist(Expression<Func<Live_PrepaidRepaymentRecord, bool>> exp);
        Task<List<Live_PrepaidRepaymentRecord>> List(Expression<Func<Live_PrepaidRepaymentRecord, bool>> exp);
        Task<List<Live_PrepaidRepaymentRecord>> PageList(Expression<Func<Live_PrepaidRepaymentRecord, bool>> exp, PagedModelBase query);
        Task<int> Update(List<Live_PrepaidRepaymentRecord> models);
    }
}
