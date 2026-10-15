using LZ.Model;
using LZ.Model.PagedModel;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LZ.Bank.Service.Interface
{
    public interface IBankApplicationService
    {
        Task<int> Add(Czy_BankApplication model);
        Task<bool> Delete(Guid id);
        Task<bool> Exist(Expression<Func<Czy_BankApplication, bool>> exp);
        Task<Czy_BankApplication> Get(Expression<Func<Czy_BankApplication, bool>> exp);
        Task<List<Czy_BankApplication>> List(Expression<Func<Czy_BankApplication, bool>> exp);
        Task<List<Czy_BankApplication>> PageList(Expression<Func<Czy_BankApplication, bool>> exp, PagedModelBase query);
        Task<bool> Update(Czy_BankApplication model);
    }
}
