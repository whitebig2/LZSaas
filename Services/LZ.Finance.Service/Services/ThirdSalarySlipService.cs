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
    public class ThirdSalarySlipService : ServicesBase<Live_ThirdSalarySlip>, IThirdSalarySlipService, IScopedDependency
    {

        public async Task<List<Live_ThirdSalarySlip>> PageList(Expression<Func<Live_ThirdSalarySlip, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var list = await   base.Context.Queryable<Live_ThirdSalarySlip>()
                .Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<List<Live_ThirdSalarySlipRecord>> PageRecordList(Expression<Func<Live_ThirdSalarySlipRecord, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var list = await   base.Context.Queryable<Live_ThirdSalarySlipRecord>()
                .Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<Live_ThirdSalarySlipExtension> GetNav(Expression<Func<Live_ThirdSalarySlipExtension, bool>> exp, string strOrderByFileds = null)
        {
            return await   base.Context.Queryable<Live_ThirdSalarySlipExtension>()
                .Includes(f => f.Records)
                .OrderByIF(!string.IsNullOrWhiteSpace(strOrderByFileds), strOrderByFileds)
                .Where(exp).FirstAsync();
        }
        public async Task<bool> AddNav(Live_ThirdSalarySlipExtension model)
        {
            return await   base.Context.InsertNav<Live_ThirdSalarySlipExtension>(model)
                .Include(f => f.Records).ExecuteCommandAsync();
        }

        public async Task<bool> UpdateNav(Live_ThirdSalarySlipExtension model)
        {
            return await   base.Context
                  .UpdateNav<Live_ThirdSalarySlipExtension>(model)
                  .Include(f => f.Records, new SqlSugar.UpdateNavOptions()
                  {
                      OneToManyInsertOrUpdate = true,//配置启用 插入、更新或删除模式
                  })
                  .ExecuteCommandAsync();
        }

        public async Task<bool> DeleteNav(Live_ThirdSalarySlipExtension model)
        {
            return await   base.Context
                  .DeleteNav<Live_ThirdSalarySlipExtension>(model)
                  .Include(f => f.Records)
                  .ExecuteCommandAsync();
        }



        public async Task<List<Live_ThirdSalarySlipRecord>> ListRecords(Expression<Func<Live_ThirdSalarySlipRecord, bool>> exp)
        {
            var list = await   base.Context.Queryable<Live_ThirdSalarySlipRecord>()
                .Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            return list;
        }


        public async Task<Live_ThirdSalarySlipRecord> GetRecord(Expression<Func<Live_ThirdSalarySlipRecord, bool>> exp)
        {
            var model = await   base.Context.Queryable<Live_ThirdSalarySlipRecord>()
                .Where(exp).OrderByDescending(it => it.CreateTime).FirstAsync();
            return model;
        }









    }
}
