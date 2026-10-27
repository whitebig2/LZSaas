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
using System.Threading.Tasks;

namespace LZ.Statistics.Service.Services
{
    public class CmbcBankCardStatisticsService : ServicesBase<Model.Live_BusinessInsuranceRecord>, ICmbcBankCardStatisticsService, IScopedDependency
    {
        public async Task<int> RegisterCount()
        {
          return await base.Context.Ado.GetIntAsync("select count(id) from [entry.cn-labor.cn].[dbo].[Cmbc_BankCard] where CmbcAccountNo  is not null");
        }

        public async Task<decimal> AvailableSumAmount()
        {
            return await base.Context.Ado.GetIntAsync("select  sum(AvailableAmount) from [entry.cn-labor.cn].[dbo].[Cmbc_BankCard]");
        }

        public async Task<int> SaleCustomerCount()
        {
            return await base.Context.Ado.GetIntAsync("select  count(id) from [sale.cn-labor.cn].[dbo].[Live_Customer] where Status!=11");
        }
    }
}
