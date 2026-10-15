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
    public class PersonnelStatisticsService : ServicesBase<Model.Live_Personnel>, IPersonnelStatisticsService, IScopedDependency
    {
        public async Task<int> Count(Expression<Func<Model.Live_Personnel, bool>> exp=null)
        {
            var qb = base.Context.Queryable<Live_Personnel>();
            return await qb.CountAsync(exp);
        }
    }
}
