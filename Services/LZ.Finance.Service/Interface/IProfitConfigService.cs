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
    public interface IProfitConfigService
    {
        Task<int> Add(Live_ProfitConfig model);
        Task<int> Add(List<Live_ProfitConfig> model);
        Task<bool> Delete(Guid id);
        Task<bool> Exist(Expression<Func<Live_ProfitConfig, bool>> exp);
        Task<Live_ProfitConfig> Get(Expression<Func<Live_ProfitConfig, bool>> exp);
        Task<List<Live_V_ProfitConfig>> ListView(Expression<Func<Live_V_ProfitConfig, bool>> exp = null);
        Task<List<Live_V_ProfitConfig>> PageList(Expression<Func<Live_V_ProfitConfig, bool>> exp, PagedModelBase query);
        Task<bool> Update(Live_ProfitConfig model);
    }
}
