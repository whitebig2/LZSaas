using LZ.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Statistics.Service.Interface
{
    public interface IProfitStatisticService
    {
        Task<List<Live_V_ProfitConfig>> ListProfitConfigView(Expression<Func<Live_V_ProfitConfig, bool>> exp = null);
        Task<List<Live_V_UserEntry>> ListEntryView(Expression<Func<Live_V_UserEntry, bool>> exp);
        Task<List<Live_V_SalaryRecord>> ListSalaryRecordView(Expression<Func<Live_V_SalaryRecord, bool>> exp = null);
        Task<bool> AddProfitManageRecord(List<Live_ProfitManageRecord> records);
        Task<List<Live_BusinessInsuranceConfig>> ListBusInsuranceConfig(Expression<Func<Live_BusinessInsuranceConfig, bool>> exp = null);
        Task<Live_BusinessInsuranceConfig> GetBusInsuranceConfig(Expression<Func<Live_BusinessInsuranceConfig, bool>> exp = null);
    }
}
