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
    public interface IActionPasswordService
    {
        Task<int> Add(Live_ActionPassword model);
        Task<bool> Delete(Guid id);
        Task<bool> Exist(Guid id);
        Task<Live_ActionPassword> Get(Guid id);
        Task<Live_ActionPassword> Get(Expression<Func<Live_ActionPassword, bool>> exp);
        Task<List<Live_ActionPassword>> PageList(Expression<Func<Live_ActionPassword, bool>> exp, PagedModelBase query);
        Task<bool> Update(Live_ActionPassword model);
    }
}
