using LZ.Finance.Service.Interface;
using LZ.Model;
using LZ.Model.PagedModel;
using SqlSugar.IOC;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LZ.Core.Interface;
using LZ.Core;

namespace LZ.Finance.Service.Services
{
    public class UserEntryService : ServicesBase<Live_UserEntry>, IUserEntryService, IScopedDependency
    {

        public async Task<List<Live_UserEntry>> List(Expression<Func<Live_UserEntry, bool>> exp)
        {
            return await   base.Context.Queryable<Live_UserEntry>().Where(exp).OrderByDescending(it => it.EntryTime.Value).ToListAsync();
        }

        public async Task<List<Live_V_UserEntry>> ListView(Expression<Func<Live_V_UserEntry, bool>> exp)
        {
            return await   base.Context.Queryable<Live_V_UserEntry>().Where(exp).OrderByDescending(it => it.EntryTime.Value).ToListAsync();
        }

        public async Task<Live_UserEntry> Get(Expression<Func<Live_UserEntry, bool>> exp)
        {
            return await   base.Context.Queryable<Live_UserEntry>().Where(exp).OrderByDescending(it => it.EntryTime.Value).FirstAsync();
        }
        public async Task<Live_V_UserEntry> GetView(Expression<Func<Live_V_UserEntry, bool>> exp)
        {
            return await   base.Context.Queryable<Live_V_UserEntry>().Where(exp).OrderByDescending(it => it.EntryTime.Value).FirstAsync();
        }

        public async Task<bool> UpdateBank(Live_UserEntry model)
        {

        Live_UserEntryRecord record=new Live_UserEntryRecord() { 
         
        };
            return await   base.Context.Updateable<Live_UserEntry>(model).UpdateColumns(f => new { f.BankName, f.BankNumber }).ExecuteCommandAsync() > 0;
        }




        public async Task<List<Live_V_UserEntryUnit>> UserEntryUnitList(Expression<Func<Live_V_UserEntryUnit, bool>> exp)
        {
            return await   base.Context.Queryable<Live_V_UserEntryUnit>().Where(exp).OrderByDescending(it => it.EntryTime.Value).ToListAsync();
        }
        public async Task<Live_V_UserEntryUnit> GetUnit(Expression<Func<Live_V_UserEntryUnit, bool>> exp)
        {
            return await   base.Context.Queryable<Live_V_UserEntryUnit>().Where(exp).OrderByDescending(it => it.EntryTime.Value).FirstAsync();
        }

        public async Task<Live_Customer> GetCustomer(Expression<Func<Live_Customer, bool>> exp)
        {
            return await base.Context.Queryable<Live_Customer>().Where(exp).FirstAsync();
        }
    }
}
