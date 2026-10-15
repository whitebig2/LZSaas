using LZ.Core.EventData;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Customer.Service.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace LZ.Customer.Service.EventHandler
{
    /// <summary>
    /// 岗位候选人清单重算事件消费：
    /// 岗位新增/修改（CompanyConfig.EnablePrecomputeJob=true 的公司）发布本事件，
    /// 此处调用 IJobRecommendService.PrecomputeJob(jobID) 原地替换该岗位清单行。
    ///
    /// 语义保证：
    /// - 幂等：PrecomputeJob 为删旧插新，重复消费只是多算一次，结果一致；
    /// - 失败安全：异常仅记日志不上抛（不 requeue 防死循环），清单缺失时在线接口有实时计算兜底；
    /// - 岗位不存在/已过期：PrecomputeJob 内部清空该岗位清单并返回 0（下架即失效）。
    /// </summary>
    public class JobPrecomputeEventHandler(IServiceScopeFactory scopeFactory) : IEventHandler<JobPrecomputeEventData>
    {
        public async Task HandleAsync(JobPrecomputeEventData @event)
        {
            using var scope = scopeFactory.CreateScope();
            var logger = scope.ServiceProvider.GetService<ILogger<JobPrecomputeEventHandler>>();
            var recommendService = scope.ServiceProvider.GetService<IJobRecommendService>();
            if (recommendService == null)
            {
                logger?.LogError($"JobPrecomputeEventHandler: IJobRecommendService 未注册，JobID={@event.JobID} 未处理");
                return;
            }

            try
            {
                var rows = await recommendService.PrecomputeJob(@event.JobID);
                if (rows < 0)
                {
                    // 夜批正在写该岗位（夜批优先策略）：跳过不重试，夜批结果即最新
                    logger?.LogInformation($"岗位{@event.JobID}({@event.Action})重算让位夜批，本次跳过");
                    return;
                }
                logger?.LogInformation($"岗位{@event.JobID}({@event.Action})候选人清单重算完成，写入 {rows} 行");
            }
            catch (Exception ex)
            {
                // 不上抛：MQ 消费失败 ack 掉（清单会有夜间全量批兜底），错误记日志便于排查
                logger?.LogError(ex, $"岗位{@event.JobID}({@event.Action})候选人清单重算失败：{ex.Message}");
            }
        }
    }
}
