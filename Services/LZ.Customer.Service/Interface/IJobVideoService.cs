using LZ.Model.PagedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Customer.Service.Interface
{
    public interface IJobVideoService
    {
        Task<List<Model.Live_JobVideo>> List(Expression<Func<Model.Live_JobVideo, bool>> exp, PagedModelBase query);

        Task<List<Model.Live_V_JobVideo>> ListView(Expression<Func<Model.Live_V_JobVideo, bool>> exp, PagedModelBase query);
        Task<int> Add(Model.Live_JobVideo model);

        Task<bool> Update(Model.Live_JobVideo model);
        Task<bool> Delete(int id);
        Task<bool> Exist(int id, int companyId = 0);
        Task<bool> ChangeStatus(int id, int status);
        Task<Model.Live_JobVideo> Get(int id);
        Task<Model.Live_V_JobVideo> GetView(int id);
    }
}
