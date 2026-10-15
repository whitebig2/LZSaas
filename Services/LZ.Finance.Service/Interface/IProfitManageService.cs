using LZ.Model;
using LZ.Model.PagedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Finance.Service.Interface
{
    public interface IProfitManageService
    {
        Task<int> Add(Live_ProfitManage model);
        Task<int> Add(List<Live_ProfitManage> models);
        Task<Live_ProfitManage> CalculateProfit(Guid id);
        Task<List<Live_ProfitManage>> CreateByRecords(DateTime grossMonth);
        Task<bool> Delete(Guid id);
        Task<bool> Exist(Guid id);
        Task<bool> GenerateProfitRecord();
        Task<Live_ProfitManage> Get(Expression<Func<Live_ProfitManage, bool>> exp);
        Task<Live_BusinessInsuranceConfig> GetBusInsuranceConfig(Expression<Func<Live_BusinessInsuranceConfig, bool>> exp = null);
        Task<List<Live_ProfitManage>> List(Expression<Func<Live_ProfitManage, bool>> exp);
        Task<List<Live_V_ProfitManage>> ListView(Expression<Func<Live_V_ProfitManage, bool>> exp);
        Task<List<Live_V_ProfitManage>> PageList(Expression<Func<Live_V_ProfitManage, bool>> exp, PagedModelBase query);
        Task<bool> Update(Live_ProfitManage model);
    }
}
