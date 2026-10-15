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
    public interface IBankUserCompanyService
    {
        Task<int> Add(Live_BankUserCompany model);
        Task<int> Add(List<Live_BankUserCompany> models);
        Task<bool> Delete(Guid id);
        Task<bool> Delete(Expression<Func<Live_BankUserCompany, bool>> exp);
        Task<bool> Exist(Expression<Func<Live_BankUserCompany, bool>> exp);
        Task<Live_BankUserCompany> Get(Expression<Func<Live_BankUserCompany, bool>> exp);
        Task<List<Live_BankUserCompany>> List(Expression<Func<Live_BankUserCompany, bool>> exp);
        Task<List<Live_BankUserCompany>> PageList(Expression<Func<Live_BankUserCompany, bool>> exp, PagedModelBase query);
        Task<bool> Update(Live_BankUserCompany model);
    }
}
