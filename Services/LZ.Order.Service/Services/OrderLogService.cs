using LZ.Core;
using LZ.Core.EventData;
using LZ.Core.Interface;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Core.Services;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using LZ.Order.Service.Interface;
using SqlSugar;
using System.Linq.Expressions;


namespace LZ.Order.Service.Services
{
    public class OrderLogService : ServicesBase<Model.Shop_OrderLog>, IOrderLogService, IScopedDependency
    {
        public async Task<List<Shop_OrderLog>> List(Expression<Func<Shop_OrderLog, bool>> exp)
        {
            return await base.Context.Queryable<Shop_OrderLog>().Where(exp).OrderByDescending(it => it.ID).ToListAsync();

        }
       
        public async Task<int> Add(Shop_OrderLog model)
        {
            return await base.Context.Insertable(model).ExecuteReturnIdentityAsync();
        }
        public async Task<int> Add(List<Shop_OrderLog> models)
        {
            return await base.Context.Insertable(models).ExecuteReturnIdentityAsync();
        }
    }
}
