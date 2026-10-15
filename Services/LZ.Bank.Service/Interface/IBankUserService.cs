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
    public interface IBankUserService
    {
        Task<int> Add(Live_BankUser model);
        Task<bool> Delete(Guid id);
        Task<bool> Exist(Expression<Func<Live_BankUser, bool>> exp);
        Task<Live_BankUser> Get(Expression<Func<Live_BankUser, bool>> exp);
        Task<Live_V_BankUser> GetView(Expression<Func<Live_V_BankUser, bool>> exp);
        Task<Live_BankUser> GetWithSecretkey(Expression<Func<Live_BankUser, bool>> exp);
        Task<List<Live_V_BankUser>> ListView(Expression<Func<Live_V_BankUser, bool>> exp);
        Task<List<Live_V_BankUser>> PageList(Expression<Func<Live_V_BankUser, bool>> exp, PagedModelBase query);
        Task<bool> Update(Live_BankUser model);
    }
}
