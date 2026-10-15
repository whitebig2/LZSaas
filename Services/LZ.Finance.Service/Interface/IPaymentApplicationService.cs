using LZ.Model;
using LZ.Model.PagedModel;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LZ.Finance.Service.Interface
{
    public interface IPaymentApplicationService
    {
        Task<int> Add(Live_PaymentApplication model);
        Task<bool> Delete(Guid id);
        Task<bool> Exist(Expression<Func<Live_PaymentApplication, bool>> exp);
        Task<Live_PaymentApplication> Get(Guid id);
        Task<Live_PaymentApplication> Get(Expression<Func<Live_PaymentApplication, bool>> exp);
        Task<List<Live_PaymentApplication>> List(Expression<Func<Live_PaymentApplication, bool>> exp);
        Task<List<Live_PaymentApplication>> PageList(Expression<Func<Live_PaymentApplication, bool>> exp, PagedModelBase query);
        Task<bool> Update(Live_PaymentApplication model);
        Task ClearOtherDefault(int companyId, Guid excludeId);
    }
}
