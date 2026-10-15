using LZ.Model;
using LZ.Model.PagedModel;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LZ.Finance.Service.Interface
{
    public interface IPaymentAdminSignUnitService
    {
        Task<int> Add(Live_PaymentAdminSignUnit model);
        Task<int> Add(List<Live_PaymentAdminSignUnit> models);
        Task<bool> Delete(Guid id);
        Task<bool> Delete(Expression<Func<Live_PaymentAdminSignUnit, bool>> exp);
        Task<bool> Exist(Expression<Func<Live_PaymentAdminSignUnit, bool>> exp);
        Task<Live_PaymentAdminSignUnit> Get(Guid id);
        Task<Live_PaymentAdminSignUnit> Get(Expression<Func<Live_PaymentAdminSignUnit, bool>> exp);
        Task<List<Live_PaymentAdminSignUnit>> List(Expression<Func<Live_PaymentAdminSignUnit, bool>> exp);
        Task<List<Live_PaymentAdminSignUnit>> PageList(Expression<Func<Live_PaymentAdminSignUnit, bool>> exp, PagedModelBase query);
        Task<bool> Update(Live_PaymentAdminSignUnit model);
    }
}
