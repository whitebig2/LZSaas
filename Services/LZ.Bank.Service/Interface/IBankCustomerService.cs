using LZ.Model;
using LZ.Model.PagedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Bank.Service.Interface
{
    public interface IBankCustomerService
    {
        Task<int> Add(Live_BankCustomer model);
        Task<bool> Delete(Guid id);
        Task<bool> Exist(Expression<Func<Live_BankCustomer, bool>> exp);
        Task<Live_BankCustomer> Get(Expression<Func<Live_BankCustomer, bool>> exp);
        Task<List<Live_BankCustomer>> List(Expression<Func<Live_BankCustomer, bool>> exp);
        Task<List<Live_V_BankCustomer>> ListView(Expression<Func<Live_V_BankCustomer, bool>> exp);
        Task<List<Live_V_BankCustomer>> PageList(Expression<Func<Live_V_BankCustomer, bool>> exp, PagedModelBase query);
        Task<bool> Update(Live_BankCustomer model);
    }
}
