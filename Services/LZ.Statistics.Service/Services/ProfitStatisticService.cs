using LZ.Core;
using LZ.Core.Interface;
using LZ.Model;
using LZ.Statistics.Service.Interface;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Statistics.Service.Services
{
    public class ProfitStatisticService : ServicesBase<Model.Live_V_UserEntry>, IProfitStatisticService, IScopedDependency
    {


        /// <summary>
        ///  获取入离职记录
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public Task<List<Live_V_UserEntry>> ListEntryView(Expression<Func<Model.Live_V_UserEntry, bool>> exp)
        {
            return base.Context.Queryable<Live_V_UserEntry>().Where(exp).ToListAsync();
        }
        /// <summary>
        ///  获取毛利配置
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public Task<List<Live_V_ProfitConfig>> ListProfitConfigView(Expression<Func<Model.Live_V_ProfitConfig, bool>> exp = null)
        {
            return base.Context.Queryable<Live_V_ProfitConfig>().Where(exp).ToListAsync();
        }
        /// <summary>
        ///  获取客户工资记录
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public Task<List<Live_V_SalaryRecord>> ListSalaryRecordView(Expression<Func<Model.Live_V_SalaryRecord, bool>> exp = null)
        {
            return base.Context.Queryable<Live_V_SalaryRecord>().Where(exp).ToListAsync();
        }


 
        public async Task<bool> AddProfitManageRecord(List<Live_ProfitManageRecord> records)
        {
            return await base.Context.Insertable<Live_ProfitManageRecord>(records).ExecuteCommandAsync()>0;
        }

        /// <summary>
        ///  获取商保配置
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public Task<List<Live_BusinessInsuranceConfig>> ListBusInsuranceConfig(Expression<Func<Model.Live_BusinessInsuranceConfig, bool>> exp = null)
        {
            return base.Context.Queryable<Live_BusinessInsuranceConfig>().Where(exp).ToListAsync();
        }

        public Task<Live_BusinessInsuranceConfig> GetBusInsuranceConfig(Expression<Func<Model.Live_BusinessInsuranceConfig, bool>> exp = null)
        {
            return base.Context.Queryable<Live_BusinessInsuranceConfig>().Where(exp).FirstAsync();
        }



    }
}
