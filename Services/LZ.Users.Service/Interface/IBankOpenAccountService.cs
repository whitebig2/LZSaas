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
    public interface IBankOpenAccountService
    {
        Task<int> Add(Live_BankOpenAccount model);
        Task<bool> Add(List<Live_BankOpenAccount> models);
        Task<bool> Delete(Guid id);
        Task<bool> Exist(Expression<Func<Live_BankOpenAccount, bool>> exp);
        Task<Live_BankOpenAccount> Get(Expression<Func<Live_BankOpenAccount, bool>> exp);
        Task<List<Live_BankOpenAccount>> List(Expression<Func<Live_BankOpenAccount, bool>> exp);
        Task<List<Live_BankOpenAccount>> PageList(Expression<Func<Live_BankOpenAccount, bool>> exp, PagedModelBase query);
        Task<bool> Update(Live_BankOpenAccount model);
        Task<bool> Update(List<Live_BankOpenAccount> models);
    }
}
