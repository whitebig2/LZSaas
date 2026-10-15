using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Customer.Service.Interface
{
    public interface IDayWorkUserContractFileService
    {
        Task<Model.Live_DayWorkUserContractFile> Get(int id);
        Task<Model.Live_DayWorkUserContractFile> Get(Expression<Func<Model.Live_DayWorkUserContractFile, bool>> exp);
        Task<bool> Exist(int id);
        Task<int> Add(Model.Live_DayWorkUserContractFile model);
        Task<bool> Update(Model.Live_DayWorkUserContractFile model);
    }
}
