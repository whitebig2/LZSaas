using LZ.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Users.Service.Interface
{
    public interface IBusinessApiOrderDetailService
    {
        Task<bool> Add(List<Live_BusinessApiOrderDetail> models);
        Task<List<Live_BusinessApiOrderDetail>> List(Expression<Func<Live_BusinessApiOrderDetail, bool>> exp);
        Task<bool> Update(List<Live_BusinessApiOrderDetail> models);
    }
}
