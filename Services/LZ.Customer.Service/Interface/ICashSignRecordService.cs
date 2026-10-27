using LZ.Model;
using LZ.Model.PagedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Customer.Service.Interface
{
    public interface ICashSignRecordService
    {
        Task<bool> Add(Live_CashSignRecord model);
        Task<bool> Delete(Expression<Func<Live_CashSignRecord, bool>> exp);
        Task<bool> Exist(Expression<Func<Live_CashSignRecord, bool>> exp);
        Task<Live_CashSignRecord> Get(Expression<Func<Live_CashSignRecord, bool>> exp);
        Task<List<Live_CashSignRecord>> List(Expression<Func<Live_CashSignRecord, bool>> exp);
        Task<List<Live_CashSignRecord>> PageList(Expression<Func<Live_CashSignRecord, bool>> exp, PagedModelBase query);
        Task<int> SetDisabled(Expression<Func<Live_CashSignRecord, bool>> exp);
        Task<bool> Update(Live_CashSignRecord model);
    }
}
