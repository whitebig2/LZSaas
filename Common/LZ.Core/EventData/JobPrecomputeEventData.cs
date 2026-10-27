using LZ.Core.RabbitMQ.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Core.EventData
{
    /// <summary>
    /// 岗位候选人清单重算事件（岗位新增/修改影响匹配字段后经 MQ 异步触发，
    /// 消费端调用 IJobRecommendService.PrecomputeJob(jobID) 原地替换该岗位清单）。
    /// 公司配置 CompanyConfig.EnablePrecomputeJob=true 时才发布。
    /// </summary>
    public class JobPrecomputeEventData : EventBase
    {
        /// <summary>
        /// 岗位 ID
        /// </summary>
        public int JobID { get; set; }

        /// <summary>
        /// 触发类型：add=新增 / update=修改
        /// </summary>
        public string Action { get; set; } = "update";
    }
}
