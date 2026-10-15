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
    public interface IBankCompanyService
    {
        Task<int> Add(Live_BankCompany model);
        Task<bool> Delete(Guid id);
        Task<bool> Exist(Expression<Func<Live_BankCompany, bool>> exp);
        Task<Live_BankCompany> Get(Expression<Func<Live_BankCompany, bool>> exp);
        Task<List<Live_BankCompany>> List(Expression<Func<Live_BankCompany, bool>> exp);
        Task<List<Live_V_BankCompany>> ListView(Expression<Func<Live_V_BankCompany, bool>> exp);
        Task<List<Live_V_BankCompany>> PageList(Expression<Func<Live_V_BankCompany, bool>> exp, PagedModelBase query);
        Task<bool> Update(Live_BankCompany model);
    }
}
