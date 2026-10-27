using LZ.Model;
using LZ.Model.PagedModel;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LZ.Finance.Service.Interface
{
    public interface IPaymentSignUnitService
    {
        Task<int> Add(Live_PaymentSignUnit model);
        Task<bool> Delete(Guid id);
        Task<bool> Exist(Expression<Func<Live_PaymentSignUnit, bool>> exp);
        Task<Live_PaymentSignUnit> Get(Guid id);
        Task<Live_PaymentSignUnit> Get(Expression<Func<Live_PaymentSignUnit, bool>> exp);
        Task<List<Live_PaymentSignUnit>> List(Expression<Func<Live_PaymentSignUnit, bool>> exp);
        Task<List<Live_V_PaymentSignUnit>> PageList(Expression<Func<Live_V_PaymentSignUnit, bool>> exp, PagedModelBase query);
        Task<bool> Update(Live_PaymentSignUnit model);
    }
}
