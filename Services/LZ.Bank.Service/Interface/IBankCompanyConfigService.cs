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
    public interface IBankCompanyConfigService
    {
        Task<int> Add(Live_BankCompanyConfig model);
        Task<bool> Delete(Guid id);
        Task<bool> Exist(Expression<Func<Live_BankCompanyConfig, bool>> exp);
        Task<Live_BankCompanyConfig> Get(Expression<Func<Live_BankCompanyConfig, bool>> exp);
        Task<Live_V_BankCompanyConfig> GetView(Expression<Func<Live_V_BankCompanyConfig, bool>> exp);
        Task<List<Live_BankCompanyConfig>> List(Expression<Func<Live_BankCompanyConfig, bool>> exp);
        Task<List<Live_V_BankCompanyConfig>> ListView(Expression<Func<Live_V_BankCompanyConfig, bool>> exp);
        Task<List<Live_V_BankCompanyConfig>> PageList(Expression<Func<Live_V_BankCompanyConfig, bool>> exp, PagedModelBase query);
        Task<bool> Update(Live_BankCompanyConfig model);
    }
}
