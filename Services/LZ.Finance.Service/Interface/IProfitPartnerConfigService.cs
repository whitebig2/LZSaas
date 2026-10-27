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
    public interface IProfitPartnerConfigService
    {
        Task<int> Add(Live_ProfitPartnerConfig model);
        Task<int> Add(List<Live_ProfitPartnerConfig> model);
        Task<bool> Delete(Guid id);
        Task<bool> Exist(Expression<Func<Live_ProfitPartnerConfig, bool>> exp);
        Task<Live_ProfitPartnerConfig> Get(Expression<Func<Live_ProfitPartnerConfig, bool>> exp);
        Task<Live_V_ProfitPartnerConfig> GetView(Expression<Func<Live_V_ProfitPartnerConfig, bool>> exp);
        Task<List<Live_V_ProfitPartnerConfig>> ListView(Expression<Func<Live_V_ProfitPartnerConfig, bool>> exp = null);
        Task<List<Live_V_ProfitPartnerConfig>> PageList(Expression<Func<Live_V_ProfitPartnerConfig, bool>> exp, PagedModelBase query);
        Task<bool> Update(Live_ProfitPartnerConfig model);
    }
}
