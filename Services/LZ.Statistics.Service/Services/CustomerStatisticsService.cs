using LZ.Core;
using LZ.Core.EventData;
using LZ.Core.Interface;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Core.Redis;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Statistics.Service.Interface;
using SqlSugar;
using SqlSugar.IOC;
using System.Linq.Expressions;

namespace LZ.Statistics.Service.Services
{
    public class CustomerStatisticsService : ServicesBase<Model.Live_Customer>, ICustomerStatisticsService, IScopedDependency
    {
        public async Task<int> Count(Expression<Func<Model.Live_Customer, bool>> exp=null)
        {
            var qb = base.Context.Queryable<Live_Customer>();
            return await qb.CountAsync(exp);
        }
    }
}
