using LZ.Basic.Service.Services;
using LZ.Core.EventData;
using LZ.Core.Interface;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Core.Redis;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Basic.Service.EventHandler
{
    public class SchedulerJobEventHandler(IServiceScopeFactory scopeFactory, IHttpService httpService, IRedisService redisService) : IEventHandler<SchedulerTestEventData>
    {
        public async Task HandleAsync(SchedulerTestEventData @event)
        {
            using var scope = scopeFactory.CreateScope();
            var sch = scope.ServiceProvider.GetService<IScheduler>();
            try
            {
                SchedulerService schedulerService = new SchedulerService();
                Model.PagedModel.BasicPagedModel query = new Model.PagedModel.BasicPagedModel();
                query.Page = 0;
                query.Enabled = true;
                var expable = Expressionable.Create<Model.Live_Scheduler>();
                expable.And(t => t.IsRun == query.Enabled);
                var models = await schedulerService.List(expable.ToExpression(), query);
                if (models.Count > 0)
                {
                    foreach (var x in models)
                    {
                        await schedulerService.RunJob(x, httpService, redisService);
                    }
                }
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
            }
        }
    }
}
