using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Customer.Service.Interface
{
    public interface IDayWorkCustomerContractRecordService
    {
        Task<Model.Live_DayWorkCustomerContractRecord> Get(int id);
        Task<Model.Live_DayWorkCustomerContractRecord> Get(Expression<Func<Model.Live_DayWorkCustomerContractRecord, bool>> exp);
        Task<bool> Exist(int id);
        Task<int> Add(Model.Live_DayWorkCustomerContractRecord model);
        Task<bool> Update(Model.Live_DayWorkCustomerContractRecord model);
        Task<bool> Delete(int id);
        Task<bool> ChangeStatus(int id, int status);
    }
}
