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
    public class BasicDataService : ServicesBase<Model.Live_PersonnelFunctionItems>, IBasicDataService, IScopedDependency
    {
        public async Task<List<Live_PersonnelFunctionItems>> PersonnelFunctionItems(Expression<Func<Live_PersonnelFunctionItems, bool>> exp)
        {
            return await base.Context.Queryable<Live_PersonnelFunctionItems>().Where(exp).OrderByDescending(it => it.ID).ToListAsync();
        }
    }
}
