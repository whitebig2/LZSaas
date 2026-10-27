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
    public class SalaryFailRecordService : ServicesBase<Live_SalaryFailRecord>,  ISalaryFailRecordService, IScopedDependency
    {

        public async Task<List<Live_V_SalaryFailRecord>> PageList(Expression<Func<Live_V_SalaryFailRecord, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var list = await   base.Context.Queryable<Live_V_SalaryFailRecord>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

       

        public async Task<List<Live_SalaryFailRecord>> List(Expression<Func<Live_SalaryFailRecord, bool>> exp, string strOrderByFileds = null)
        {
            return await   base.Context.Queryable<Live_SalaryFailRecord>()
                .OrderByIF(!string.IsNullOrWhiteSpace(strOrderByFileds), strOrderByFileds)
                .Where(exp).ToListAsync();
        }
        public async Task<Live_SalaryFailRecord> Get(Expression<Func<Live_SalaryFailRecord, bool>> exp, string strOrderByFileds = null)
        {
            return await   base.Context.Queryable<Live_SalaryFailRecord>()
                .OrderByIF(!string.IsNullOrWhiteSpace(strOrderByFileds), strOrderByFileds)
                .Where(exp).FirstAsync();
        }

        public async Task<Live_V_SalaryFailRecord> GetView(Expression<Func<Live_V_SalaryFailRecord, bool>> exp, string strOrderByFileds = null)
        {
            return await   base.Context.Queryable<Live_V_SalaryFailRecord>()
                .OrderByIF(!string.IsNullOrWhiteSpace(strOrderByFileds), strOrderByFileds)
                .Where(exp).FirstAsync();
        }
        public async Task<List<Live_V_SalaryFailRecord>> ListView(Expression<Func<Live_V_SalaryFailRecord, bool>> exp, string strOrderByFileds = null)
        {
            return await   base.Context.Queryable<Live_V_SalaryFailRecord>()
                .OrderByIF(!string.IsNullOrWhiteSpace(strOrderByFileds), strOrderByFileds)
                .Where(exp).ToListAsync();
        }

        public async Task<bool> Update(Live_SalaryFailRecord model)
        {
            return await   base.Context.Updateable<Live_SalaryFailRecord>(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Update(List<Live_SalaryFailRecord> models)
        {
            return await   base.Context.Updateable<Live_SalaryFailRecord>(models).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }


        public async Task<bool> Add(Live_SalaryFailRecord model)
        {
            var record = await   base.Context.Queryable<Live_SalaryRecord>().Where(f => f.ID == model.SalaryRecordID).FirstAsync();
            if (record != null)
            {
                record.IsFailSalary = true;

                await   base.Context.Updateable<Live_SalaryRecord>(record).ExecuteCommandAsync();
            }
            return await   base.Context.Insertable<Live_SalaryFailRecord>(model).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Delete(Expression<Func<Live_SalaryFailRecord, bool>> exp)
        {
            return await   base.Context.Deleteable<Live_SalaryFailRecord>(exp).ExecuteCommandAsync() > 0;
        }
    }
}
