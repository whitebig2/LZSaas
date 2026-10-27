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
    public interface IBankTransactionService
    {
        Task<bool> AddNav(Live_BankTransactionExtension model);
        Task<bool> AddNav(List<Live_BankTransactionExtension> models);
        Task<bool> CancelNav(Live_BankTransactionExtension model);
        Task<Live_BankTransaction> Get(Expression<Func<Live_BankTransaction, bool>> exp);
        Task<Live_BankTransactionExtension> GetNav(Guid id);
        Task<Live_BankTransactionExtension> GetNav(Expression<Func<Live_BankTransactionExtension, bool>> exp);
        Task<List<Live_BankTransaction>> List(Expression<Func<Live_BankTransaction, bool>> exp);
        Task<bool> NoticeResult(Live_BankTransactionExtension model);
        Task<List<Live_BankTransaction>> PageList(Expression<Func<Live_BankTransaction, bool>> exp, PagedModelBase query);
        Task<List<Live_BankTransactionRecord>> PageRecordList(Expression<Func<Live_BankTransactionRecord, bool>> exp, PagedModelBase query);
        Task<bool> ResetNav(Live_BankTransactionExtension model);
        Task<bool> Update(Live_BankTransaction model);
        Task<bool> UpdateNav(Live_BankTransactionExtension model);
    }
}
