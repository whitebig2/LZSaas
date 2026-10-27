using LZ.Model.PagedModel;
using LZ.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Users.Service.Interface
{
    public interface IBankWalletService
    {
        Task<int> Add(Live_BankWallet model);
        Task<bool> AddList(List<Live_BankWallet> models);
        Task<bool> Delete(int id);
        Task<bool> Exist(Expression<Func<Live_BankWallet, bool>> exp);
        Task<Live_BankWallet> Get(Expression<Func<Live_BankWallet, bool>> exp);
        Task<List<Live_BankWallet>> List(Expression<Func<Live_BankWallet, bool>> exp);
        Task<List<Live_BankWallet>> PageList(Expression<Func<Live_BankWallet, bool>> exp, PagedModelBase query);
        Task<bool> Update(Live_BankWallet model);
        Task<bool> Update(List<Live_BankWallet> models);
    }
}
