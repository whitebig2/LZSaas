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
using LZ.Model.ModelExtensions;

namespace LZ.Finance.Service.Services
{
    public class SalarySlipService : ServicesBase<Live_SalarySlip>, ISalarySlipService, IScopedDependency
    {

        public async Task<List<Live_V_SalarySlip>> PageList(Expression<Func<Live_V_SalarySlip, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var list = await   base.Context.Queryable<Live_V_SalarySlip>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<Live_SalarySlip> Get(Expression<Func<Live_SalarySlip, bool>> exp)
        {
            return await   base.Context.Queryable<Live_SalarySlip>()
                .FirstAsync(exp);
        }


        public async Task<List<Live_SalarySlip>> List(Expression<Func<Live_SalarySlip, bool>> exp)
        {
            return await   base.Context.Queryable<Live_SalarySlip>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }

        public async Task<List<Live_V_SalarySlip>> ListView(Expression<Func<Live_V_SalarySlip, bool>> exp)
        {
            return await   base.Context.Queryable<Live_V_SalarySlip>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }

        public async Task<bool> AddNav(Live_SalarySlipExtension model)
        {
            return await   base.Context.InsertNav(model)
                .Include(f => f.Records).ExecuteCommandAsync();
        }
        public async Task<bool> Exist(Expression<Func<Live_SalarySlip, bool>> exp)
        {
            return await   base.Context.Queryable<Live_SalarySlip>().AnyAsync(exp);
        }
        public async Task<Live_SalarySlipExtension> GetNav(Guid id)
        {
            return await   base.Context.Queryable<Live_SalarySlipExtension>()
                .Includes(f => f.Records)
                .FirstAsync(it => it.ID == id);
        }
        public async Task<bool> DeleteNav(Live_SalarySlipExtension model)
        {
            return await   base.Context.DeleteNav<Live_SalarySlipExtension>(model)
                .Include(f => f.Records).ExecuteCommandAsync();
        }

        public async Task<Live_SalarySlipRecord> GetSalarySlipRecord(Expression<Func<Live_SalarySlipRecord, bool>> exp)
        {
            return await   base.Context.Queryable<Live_SalarySlipRecord>().Where(exp).OrderByDescending(f=>f.SalaryMonth).FirstAsync();
        }



    }
}
