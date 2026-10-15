using LZ.Core.Interface;
using LZ.Core;
using LZ.Customer.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LZ.Model.PagedModel;
using LZ.Model;
using SqlSugar.IOC;
using SqlSugar;
using System.Linq.Expressions;

namespace LZ.Customer.Service.Services
{
    public class JobsDayWorkService : ServicesBase<Live_JobsDayWork>, IJobsDayWorkService, IScopedDependency
    {
        public async Task<List<Live_V_JobsDayWork>> List(Expression<Func<Live_V_JobsDayWork, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var qb = base.Context.Queryable<Live_V_JobsDayWork>();
            qb.Where(exp);
            if (!string.IsNullOrEmpty(query.OrderField))
            {
                if (string.IsNullOrEmpty(query.Order)) query.Order = "asc";
                qb.OrderBy(query.OrderField + " " + query.Order);
            }
            else
            {
                qb.OrderByDescending(it => it.ID);
            }
            if (query.Page == 0)
            {
                if (query.Top > 0)
                {
                    qb.Take(query.Top);
                }
                return await qb.ToListAsync();
            }
            var list = await qb.ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        private string FormatTag(Model.Live_JobsDayWork model)
        {
            var tags = new StringBuilder();
            
           
            if (!string.IsNullOrEmpty(model.HostelCondition))
            {
                if (model.HostelCondition.Contains("夫妻房"))
                {
                    tags.Append("夫妻房,");
                }
                if (model.HostelCondition.Contains("双人间"))
                {
                    tags.Append("双人间,");
                }
            }
            
            if (!string.IsNullOrEmpty(model.Workshop))
            {
                if (model.Workshop.Contains("空调车间"))
                {
                    tags.Append("空调车间,");
                }
            }
            if (!string.IsNullOrEmpty(model.Intensity))
            {
                if (model.Intensity.Contains("轻"))
                {
                    tags.Append("工作轻松,");
                }
            }
            if (!string.IsNullOrEmpty(model.WorkTime))
            {
                if (model.WorkTime.Contains("长白班"))
                {
                    tags.Append("长白班,");
                }
            }
            if (!string.IsNullOrEmpty(model.UserAsk))
            {
                if (model.UserAsk.Contains("经验不限"))
                {
                    tags.Append("经验不限,");
                }
                if (model.UserAsk.Contains("学历不限"))
                {
                    tags.Append("学历不限,");
                }
            }
            if (!string.IsNullOrEmpty(model.NightSubsidy))
            {
                tags.Append("夜班补贴,");
            }
            if (tags.Length > 0)
            {
                tags.Length--;
            }
            return tags.ToString();
        }

        public long GetSequence()
        {
            var sqeuareNo = base.Context.Ado.SqlQuery<long>("select  next value for DayWorkJobID").First();
            return sqeuareNo;
            //var nextValue = await base.Context.SqlQueryable<CountModel>("select next value for DayWorkJobID as Count").FirstAsync();
            //return nextValue.Count;
        }
        public async Task<int> Add(Live_JobsDayWork model)
        {
            model.ShowTags = FormatTag(model);
            return await base.Context.Insertable(model).ExecuteReturnIdentityAsync();
        }
        public async Task<bool> Exist(int id, int companyId = 0)
        {
            var expable = Expressionable.Create<Model.Live_V_JobsDayWork>();
            expable.And(it => it.ID == id);
            if (companyId > 0)
            {
                expable.And(it => it.CompanyID == companyId);
            }
            var exp = expable.ToExpression();
            return await base.Context.Queryable<Live_V_JobsDayWork>().AnyAsync(exp);
        }
        public async Task<bool> Update(Live_JobsDayWork model)
        {
            model.ShowTags = FormatTag(model);
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> ChangeStatus(int id, int status)
        {
            return await base.Context.Updateable<Live_JobsDayWork>().SetColumns(it => it.Status == status).Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> ChangeNumber(int id, int number)
        {
            return await base.Context.Updateable<Live_JobsDayWork>().SetColumns(it => it.StaffNumber == it.StaffNumber + number).Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<Live_JobsDayWork> Get(int id)
        {
            return await base.Context.Queryable<Live_JobsDayWork>().FirstAsync(it => it.ID == id);
        }
        public async Task<Live_V_JobsDayWork> GetView(int id)
        {
            return await base.Context.Queryable<Live_V_JobsDayWork>().FirstAsync(it => it.ID == id);
        }
        public async Task<bool> Set(int id, string filed, bool enabled)
        {
            var db = base.Context.Updateable<Live_JobsDayWork>();
            if (filed == "min")
            {
                db.SetColumns(it => it.MinShow == enabled);
            }
            else
            {
                db.SetColumns(it => it.XFZShow == enabled);
            }
            return await db.Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Delete(int id)
        {
            return await base.Context.Deleteable<Live_JobsDayWork>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
    }
}
