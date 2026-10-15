using LZ.Model;
using LZ.Model.ModelExtensions;
using LZ.Model.PagedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Finance.Service.Interface
{
    public interface ISalaryTransactionService
    {
        Task<bool> AddNav(Live_SalaryTransactionExtension model);
        Task<bool> DeleteNav(Guid id);
        Task<bool> Exist(Expression<Func<Live_SalaryTransaction, bool>> exp);
        Task<Live_SalaryTransaction> Get(Expression<Func<Live_SalaryTransaction, bool>> exp);
        Task<Live_SalaryTransactionExtension> GetNav(Expression<Func<Live_SalaryTransactionExtension, bool>> exp);
        Task<List<Live_SalaryTransaction>> PageList(Expression<Func<Live_SalaryTransaction, bool>> exp, PagedModelBase query);
        Task<List<Live_SalaryTransactionRecord>> PageRecordList(Expression<Func<Live_SalaryTransactionRecord, bool>> exp, PagedModelBase query);
        Task<bool> Update(Live_SalaryTransaction model);
        Task<bool> UpdateNav(Live_SalaryTransactionExtension model);
    }
}
