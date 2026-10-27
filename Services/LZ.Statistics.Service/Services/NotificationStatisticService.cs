using LZ.Core.Interface;
using LZ.Core;
using LZ.Statistics.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LZ.Model;
using System.Linq.Expressions;

namespace LZ.Statistics.Service.Services
{
    public class NotificationStatisticService : ServicesBase<Model.Live_Notification>, INotificationStatisticService, IScopedDependency
    {
        public async Task<int> InfoCount(Expression<Func<Model.Live_Notification, bool>> exp = null)
        {
            var qb = base.Context.Queryable<Live_Notification>();
            return await qb.CountAsync(exp);
        }
    }
}
