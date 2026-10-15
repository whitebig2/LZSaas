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
    public interface ISettlementService
    {
        Task<bool> Add(Live_Settlement model);
        Task<bool> Add(List<Live_Settlement> model);
        Task<bool> Delete(Guid id);
        Task<bool> Delete(Expression<Func<Live_Settlement, bool>> exp);
        Task<bool> Exist(Guid id);
        Task<Live_Settlement> Get(Guid id);
        Task<List<Live_V_Settlement>> List(Expression<Func<Live_V_Settlement, bool>> exp);
        Task<List<Live_V_Settlement>> PageList(Expression<Func<Live_V_Settlement, bool>> exp, PagedModelBase query);
        Task<bool> Update(Live_Settlement model);
    }
}
