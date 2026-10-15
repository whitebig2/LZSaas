using LZ.Model.PagedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Basic.Service.Interface
{
    public interface INotificationService
    {
        Task<List<Model.Live_Notification>> List(Expression<Func<Model.Live_Notification, bool>> exp, BasicPagedModel query, string order = "");
        Task<int> Add(Model.Live_Notification model);

        Task<int> Adds(List<Model.Live_Notification> models);
        Task<bool> Update(Model.Live_Notification model);
        Task<bool> ChangeStatus(int id,int status);
        Task<bool> Exist(Expression<Func<Model.Live_Notification, bool>> exp);
        Task<bool> ExistID(int id);
        Task<bool> Delete(int id);
        Task<Model.Live_Notification> Get(int id);
    }
}
