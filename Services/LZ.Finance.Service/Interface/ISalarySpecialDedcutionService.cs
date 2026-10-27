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
    public interface ISalarySpecialDedcutionService
    {
        Task<int> Add(List<Live_SalarySpecialDedcution> models);
        Task<bool> Delete(int id);
        Task<bool> Delete(Expression<Func<Live_SalarySpecialDedcution, bool>> exp);
        Task<Live_SalarySpecialDedcution> Get(Expression<Func<Live_SalarySpecialDedcution, bool>> exp);
        Task<List<Live_SalarySpecialDedcution>> PageList(Expression<Func<Live_SalarySpecialDedcution, bool>> exp, PagedModelBase query);
    }
}
