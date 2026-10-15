using LZ.Core.EventData;
using LZ.Core.EventData.FxqEventData;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Finance.Service.Interface;
using LZ.Finance.Service.Services;
using LZ.Model.Exceptions;
using LZ.Model;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Senparc.CO2NET.Extensions;
using Microsoft.Extensions.Logging;
using LZ.Core.Interface;
using LZ.Core.Services;
using LZ.Model.Enums;
using LZ.Core.EventData.Salary;
using SqlSugar.IOC;
using NPOI.SS.Formula.Functions;

namespace LZ.Finance.Service.EventHandler
{
    public class SalaryBudgetApplyCompleteEventHandler(IServiceScopeFactory scopeFactory) : IEventHandler<SalaryBudgetApplyCompleteEventData>
    {
        public async Task HandleAsync(SalaryBudgetApplyCompleteEventData @event)
        {


            // 员工工资单笔发放处理
            using (var scope = scopeFactory.CreateScope())
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<SalaryBudgetApplyCompleteEventHandler>>();
                var otherService = scope.ServiceProvider.GetRequiredService<IFinanceOtherService>();
                await otherService.BudgetApplyRecordAuditing(@event.BudgetApplyRecordID);
            }
        }
    }
}
