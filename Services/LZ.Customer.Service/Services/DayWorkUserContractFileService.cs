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
    public class DayWorkUserContractFileService : ServicesBase<Live_DayWorkUserContractFile>, IDayWorkUserContractFileService, IScopedDependency
    {
        public async Task<int> Add(Live_DayWorkUserContractFile model)
        {
            return await base.Context.Insertable(model).ExecuteReturnIdentityAsync();
        }
        public async Task<bool> Exist(int id)
        {
            return await base.Context.Queryable<Live_DayWorkUserContractFile>().AnyAsync(it => it.ID == id);
        }
        public async Task<bool> Update(Live_DayWorkUserContractFile model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public async Task<Live_DayWorkUserContractFile> Get(int id)
        {
            return await base.Context.Queryable<Live_DayWorkUserContractFile>().FirstAsync(it => it.ID == id);
        }
        public async Task<Model.Live_DayWorkUserContractFile> Get(Expression<Func<Model.Live_DayWorkUserContractFile, bool>> exp)
        {
            return await base.Context.Queryable<Live_DayWorkUserContractFile>().Where(exp).FirstAsync();
        }
    }
}
