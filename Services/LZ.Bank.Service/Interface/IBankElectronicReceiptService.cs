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
    public interface IBankElectronicReceiptService
    {
        Task<int> Add(List<Live_BankElectronicReceipt> models);
        Task<bool> Delete(Guid id);
        Task<bool> Exist(Expression<Func<Live_BankElectronicReceipt, bool>> exp);
        Task<Live_BankElectronicReceipt> Get(Expression<Func<Live_BankElectronicReceipt, bool>> exp);
        Task<List<Live_BankElectronicReceipt>> List(Expression<Func<Live_BankElectronicReceipt, bool>> exp);
        Task<List<Live_BankElectronicReceipt>> PageList(Expression<Func<Live_BankElectronicReceipt, bool>> exp, PagedModelBase query);
    }
}
