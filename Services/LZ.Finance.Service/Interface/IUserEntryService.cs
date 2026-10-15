using LZ.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Finance.Service.Interface
{
    public interface IUserEntryService
    {
        Task<Live_UserEntry> Get(Expression<Func<Live_UserEntry, bool>> exp);
        Task<Live_Customer> GetCustomer(Expression<Func<Live_Customer, bool>> exp);
        Task<Live_V_UserEntryUnit> GetUnit(Expression<Func<Live_V_UserEntryUnit, bool>> exp);
        Task<Live_V_UserEntry> GetView(Expression<Func<Live_V_UserEntry, bool>> exp);
        Task<List<Live_UserEntry>> List(Expression<Func<Live_UserEntry, bool>> exp);
        Task<List<Live_V_UserEntry>> ListView(Expression<Func<Live_V_UserEntry, bool>> exp);
        Task<bool> UpdateBank(Live_UserEntry model);
        Task<List<Live_V_UserEntryUnit>> UserEntryUnitList(Expression<Func<Live_V_UserEntryUnit, bool>> exp);
    }
}
