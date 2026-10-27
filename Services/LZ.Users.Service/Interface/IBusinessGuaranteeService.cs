using LZ.Model;
using LZ.Model.PagedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Users.Service.Interface
{
    public interface IBusinessGuaranteeService
    {
        Task<bool> Add(List<Live_BusinessGuarantee> models);
        Task Delete(Expression<Func<Live_BusinessGuarantee, bool>> exp);
        Task<Live_BusinessGuarantee> Get(Expression<Func<Live_BusinessGuarantee, bool>> exp);
        Task<List<Live_BusinessGuarantee>> List(Expression<Func<Live_BusinessGuarantee, bool>> exp);
        Task<List<Live_BusinessGuarantee>> PageList(Expression<Func<Live_BusinessGuarantee, bool>> exp, PagedModelBase query);
        Task<bool> Update(List<Live_BusinessGuarantee> models);
    }
}
