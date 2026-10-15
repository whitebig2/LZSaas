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
    public interface ISalaryConfigService 
    {

        Task<List<Model.Live_V_SalaryConfig>> List(Expression<Func<Model.Live_V_SalaryConfig, bool>> exp, PagedModelBase query);

        Task<List<Model.Live_SalaryConfig>> List(Expression<Func<Model.Live_SalaryConfig, bool>> exp, PagedModelBase query);

        Task<int> Add(Model.Live_SalaryConfig model);

        Task<bool> Update(Model.Live_SalaryConfig model);
        Task<bool> Delete(Guid id);
        Task<bool> Exist(Guid id);

        Task<Model.Live_SalaryConfig> Get(Guid id);
        Task<Live_SalaryConfig> Get(Expression<Func<Live_SalaryConfig, bool>> exp);
    }
}
