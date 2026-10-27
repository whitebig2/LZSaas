using LZ.Model;
using LZ.Model.PagedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Customer.Service.Interface
{
    public interface ICaAuthIdentityService
    {
        Task<int> Add(Live_CaAuthIdentity model);
        Task<bool> Delete(Guid id);
        Task<bool> Exist(Expression<Func<Live_CaAuthIdentity, bool>> exp);
        Task<Live_CaAuthIdentity> Get(Expression<Func<Live_CaAuthIdentity, bool>> exp);
        Task<List<Live_CaAuthIdentity>> List(Expression<Func<Live_CaAuthIdentity, bool>> exp);
        Task<List<Live_CaAuthIdentity>> PageList(Expression<Func<Live_CaAuthIdentity, bool>> exp, PagedModelBase query);
        Task<bool> Update(Live_CaAuthIdentity model);
        Task<bool> Update(List<Live_CaAuthIdentity> models);
    }
}
