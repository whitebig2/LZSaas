using LZ.Core.Interface;
using LZ.Finance.Service.Interface;
using LZ.Model.PagedModel;
using LZ.Model;
using SqlSugar.IOC;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LZ.Model.Budget;
using LZ.Core;

namespace LZ.Finance.Service.Services
{
    public class BudgetReportDetailService : ServicesBase<Live_BudgetReportDetail>, IBudgetReportDetailService, IScopedDependency
    {
        public async Task<List<Live_BudgetReportDetail>> List(Expression<Func<Live_BudgetReportDetail, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_BudgetReportDetail>().Where(exp).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_BudgetReportDetail>().Where(exp).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<int> Add(Live_BudgetReportDetail model)
        {
            return await base.Context.Insertable(model).ExecuteReturnIdentityAsync();
        }
        public async Task<int> Add(List<Live_BudgetReportDetail> list)
        {
            return await base.Context.Insertable(list).ExecuteCommandAsync();
        }

        public async Task<bool> Exist(int id)
        {
            return await base.Context.Queryable<Live_BudgetReportDetail>().AnyAsync(it => it.ID == id);
        }

        public async Task<bool> Exist(Expression<Func<Live_BudgetReportDetail, bool>> exp)
        {
            return await base.Context.Queryable<Live_BudgetReportDetail>().AnyAsync(exp);
        }

        public async Task<bool> Update(Live_BudgetReportDetail model)
        {
            return await base.Context.Updateable<Live_BudgetReportDetail>(model).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Update(List<Model.Live_BudgetReportDetail> list)
        {
            return await base.Context.Updateable<Live_BudgetReportDetail>(list).ExecuteCommandAsync() > 0;
        }

        public async Task<Live_BudgetReportDetail> Get(int id)
        {
            return await base.Context.Queryable<Live_BudgetReportDetail>().FirstAsync(it => it.ID == id);
        }
        public async Task<Model.Live_BudgetReportDetail> Get(Expression<Func<Live_BudgetReportDetail, bool>> exp)
        {
            return await base.Context.Queryable<Live_BudgetReportDetail>().Where(exp).FirstAsync();
        }


        public async Task<bool> Delete(int id)
        {
            return await base.Context.Deleteable<Live_BudgetReportDetail>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Delete(Expression<Func<Live_BudgetReportDetail, bool>> exp)
        {
            return await base.Context.Deleteable<Live_BudgetReportDetail>().Where(exp).ExecuteCommandAsync() > 0;
        }


        public async Task<int> DictionarySet(Dictionary<string, object> dt)
        {
            return await base.Context.Updateable<Model.Live_BudgetReportDetail>(dt).With(SqlWith.UpdLock).ExecuteCommandAsync();
        }

        public List<Model.Budget.RecordMonthModel> ReportMonthList(List<Live_BudgetReportDetail> list, RecordMonthModel info)
        {
            var now = DateTime.Now;
            now = Convert.ToDateTime(now.Year + "-" + now.Month + "-01");
            List<Model.Budget.RecordMonthModel> models = new List<Model.Budget.RecordMonthModel>();
            var start = info.Year + "-04-01";
            var date = Convert.ToDateTime(start);
            int month = date.Month;
            int syear = date.AddMonths(-3).Year;
            while(info.Year == syear)
            {
                Model.Budget.RecordMonthModel model = new Model.Budget.RecordMonthModel();
                model.Year = info.Year;
                model.Month = month;
                model.PartnerName = info.PartnerName;
                model.PartnerID = info.PartnerID;
                model.PartnerGID = info.PartnerGID;
                var items = list.Where(t => t.Month == month).ToList();
                model.Exist = items.Count > 0;
                model.IsTimeCheck = now >= date;
                models.Add(model);
                date = date.AddMonths(1);
                month = date.Month;
                syear = date.AddMonths(-3).Year;
            }
            return models;
        }
    }
}
