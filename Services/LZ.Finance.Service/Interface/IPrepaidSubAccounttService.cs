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
    public interface IPrepaidSubAccounttService
    {
        Task<bool> Add(Live_PrepaidSubAccount model);
        Task<bool> Delete(Expression<Func<Live_PrepaidSubAccount, bool>> exp);
        Task<Live_PrepaidSubAccount> Get(Expression<Func<Live_PrepaidSubAccount, bool>> exp);
        Task<List<Live_PrepaidSubAccount>> List(Expression<Func<Live_PrepaidSubAccount, bool>> exp);
        Task<List<Live_PrepaidSubAccount>> PageList(Expression<Func<Live_PrepaidSubAccount, bool>> exp, PagedModelBase query);
        Task<bool> Update(Live_PrepaidSubAccount model);
    }
}
