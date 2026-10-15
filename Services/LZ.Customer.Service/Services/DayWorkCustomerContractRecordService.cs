using LZ.Core.Interface;
using LZ.Core;
using LZ.Customer.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LZ.Model;
using SqlSugar.IOC;
using System.Linq.Expressions;

namespace LZ.Customer.Service.Services
{
    public class DayWorkCustomerContractRecordService : ServicesBase<Live_DayWorkCustomerContractRecord>, IDayWorkCustomerContractRecordService, IScopedDependency
    {
        public async Task<int> Add(Live_DayWorkCustomerContractRecord model)
        {
            return await base.Context.Insertable(model).ExecuteReturnIdentityAsync();
        }
        public async Task<bool> Exist(int id)
        {
            return await base.Context.Queryable<Live_DayWorkCustomerContractRecord>().AnyAsync(it => it.ID == id);
        }
        public async Task<bool> Update(Live_DayWorkCustomerContractRecord model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public async Task<Live_DayWorkCustomerContractRecord> Get(int id)
        {
            return await base.Context.Queryable<Live_DayWorkCustomerContractRecord>().FirstAsync(it => it.ID == id);
        }
        public async Task<Model.Live_DayWorkCustomerContractRecord> Get(Expression<Func<Model.Live_DayWorkCustomerContractRecord, bool>> exp)
        {
            return await base.Context.Queryable<Live_DayWorkCustomerContractRecord>().Where(exp).FirstAsync();
        }
        public async Task<bool> Delete(int id)
        {
            return await base.Context.Deleteable<Live_DayWorkCustomerContractRecord>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> ChangeStatus(int id, int status)
        {
            return await base.Context.Updateable<Live_DayWorkCustomerContractRecord>().SetColumns(it => it.Status == status).Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
    }
}
