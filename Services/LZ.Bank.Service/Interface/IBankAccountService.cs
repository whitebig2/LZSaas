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
    public interface IBankAccountService
    {
        Task<int> Add(Live_BankAccount model);
        Task<bool> Delete(Guid id);
        Task<bool> Exist(Expression<Func<Live_BankAccount, bool>> exp);
        Task<Live_BankAccount> Get(Expression<Func<Live_BankAccount, bool>> exp);
        Task<List<Live_BankAccount>> List(Expression<Func<Live_BankAccount, bool>> exp);
        Task<List<Live_BankAccount>> PageList(Expression<Func<Live_BankAccount, bool>> exp, PagedModelBase query);
        Task<bool> Update(Live_BankAccount model);
    }
}
