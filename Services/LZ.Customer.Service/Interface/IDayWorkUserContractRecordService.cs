using LZ.Model.PagedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Customer.Service.Interface
{
    public interface IDayWorkUserContractRecordService
    {
        Task<List<Model.Live_V_DayWorkUserContractRecord>> List(Expression<Func<Model.Live_V_DayWorkUserContractRecord, bool>> exp, PagedModelBase query);
        Task<Model.Live_DayWorkUserContractRecord> Get(int id);
        Task<Model.Live_DayWorkUserContractRecord> Get(Expression<Func<Model.Live_DayWorkUserContractRecord, bool>> exp);

        Task<Model.Live_V_DayWorkUserContractRecord> GetView(Expression<Func<Model.Live_V_DayWorkUserContractRecord, bool>> exp);
        Task<bool> Exist(int id);
        Task<int> Add(Model.Live_DayWorkUserContractRecord model);
        Task<bool> Update(Model.Live_DayWorkUserContractRecord model);

        Task<bool> Delete(int id);

        Task<bool> ChangeStatus(int id, int status);


    }
}
