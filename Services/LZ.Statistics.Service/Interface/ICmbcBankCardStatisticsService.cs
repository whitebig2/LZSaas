using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Statistics.Service.Interface
{
    public interface ICmbcBankCardStatisticsService
    {
        Task<decimal> AvailableSumAmount();
        Task<int> RegisterCount();

        Task<int> SaleCustomerCount();
    }
}
