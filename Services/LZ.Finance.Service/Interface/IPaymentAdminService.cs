using LZ.Model;
using LZ.Model.PagedModel;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LZ.Finance.Service.Interface
{
    public interface IPaymentAdminService
    {
        Task<int> Add(Live_PaymentAdmin model);
        Task<bool> Delete(Guid id);
        Task<bool> Exist(Expression<Func<Live_PaymentAdmin, bool>> exp);
        Task<Live_PaymentAdmin> Get(Guid id);
        Task<Live_PaymentAdmin> Get(Expression<Func<Live_PaymentAdmin, bool>> exp);
        Task<Live_PaymentAdmin> GetWithSecretkey(Expression<Func<Live_PaymentAdmin, bool>> exp);
        Task<List<Live_PaymentAdmin>> List(Expression<Func<Live_PaymentAdmin, bool>> exp);
        Task<List<Live_V_PaymentAdmin>> PageList(Expression<Func<Live_V_PaymentAdmin, bool>> exp, PagedModelBase query);
        Task<bool> Update(Live_PaymentAdmin model);
    }
}
