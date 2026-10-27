using LZ.Model;
using LZ.Model.PagedModel;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LZ.Finance.Service.Interface
{
    public interface IPaymentAccountService
    {
        Task<int> Add(Live_PaymentAccount model);
        Task<bool> Delete(Guid id);
        Task<bool> Exist(Expression<Func<Live_PaymentAccount, bool>> exp);
        Task<Live_PaymentAccount> Get(Guid id);
        Task<Live_PaymentAccount> Get(Expression<Func<Live_PaymentAccount, bool>> exp);
        Task<List<Live_PaymentAccount>> List(Expression<Func<Live_PaymentAccount, bool>> exp);
        Task<List<Live_V_PaymentAccount>> ListView(Expression<Func<Live_V_PaymentAccount, bool>> exp);
        Task<List<Live_V_PaymentAccount>> PageList(Expression<Func<Live_V_PaymentAccount, bool>> exp, PagedModelBase query);
        Task<bool> Update(Live_PaymentAccount model);
    }
}
