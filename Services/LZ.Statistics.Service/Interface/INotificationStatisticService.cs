using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Statistics.Service.Interface
{
    public interface INotificationStatisticService
    {
        Task<int> InfoCount(Expression<Func<Model.Live_Notification, bool>> exp);
    }
}
