using LZ.Model.PagedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Customer.Service.Interface
{
    public interface IJobsDayWorkService
    {
        Task<List<Model.Live_V_JobsDayWork>> List(Expression<Func<Model.Live_V_JobsDayWork, bool>> exp, PagedModelBase query);
        Task<int> Add(Model.Live_JobsDayWork model);

        Task<bool> Update(Model.Live_JobsDayWork model);
        Task<bool> Delete(int id);
        Task<bool> Exist(int id, int companyId = 0);
        Task<bool> ChangeStatus(int id, int status);
        Task<bool> ChangeNumber(int id, int number);
        Task<Model.Live_JobsDayWork> Get(int id);
        Task<Model.Live_V_JobsDayWork> GetView(int id);
        Task<bool> Set(int id, string filed, bool enabled);

        long GetSequence();
    }
}
