using LZ.Model.PagedModel;
using LZ.Model;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LZ.Core.Interface;
using LZ.Core;
using LZ.Customer.Service.Interface;

namespace LZ.Customer.Service.Services
{
    public class JobVideoService : ServicesBase<Live_JobVideo>, IJobVideoService, IScopedDependency
    {
        public async Task<List<Live_JobVideo>> List(Expression<Func<Live_JobVideo, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var qb = base.Context.Queryable<Live_JobVideo>();
            qb.Where(exp);
            if (!string.IsNullOrEmpty(query.OrderField))
            {
                if (string.IsNullOrEmpty(query.Order)) query.Order = "asc";
                qb.OrderBy(query.OrderField + " " + query.Order);
            }
            else
            {
                qb.OrderByDescending(it => it.CreateTime);
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
        public async Task<List<Model.Live_V_JobVideo>> ListView(Expression<Func<Model.Live_V_JobVideo, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var qb = base.Context.Queryable<Live_V_JobVideo>();
            qb.Where(exp);
            if (!string.IsNullOrEmpty(query.OrderField))
            {
                if (string.IsNullOrEmpty(query.Order)) query.Order = "asc";
                qb.OrderBy(query.OrderField + " " + query.Order);
            }
            else
            {
                qb.OrderByDescending(it => it.CreateTime);
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
        public async Task<int> Add(Live_JobVideo model)
        {
            return await base.Context.Insertable(model).ExecuteReturnIdentityAsync();
        }
        public async Task<bool> Exist(int id, int companyId = 0)
        {
            var expable = Expressionable.Create<Model.Live_JobVideo>();
            expable.And(it => it.ID == id);
            if (companyId > 0)
            {
                expable.And(it => it.CompanyID == companyId);
            }
            var exp = expable.ToExpression();
            return await base.Context.Queryable<Live_JobVideo>().AnyAsync(exp);
        }
        public async Task<bool> Update(Live_JobVideo model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> ChangeStatus(int id, int status)
        {
            return await base.Context.Updateable<Live_JobVideo>().SetColumns(it => it.Status == status).Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }


        public async Task<Live_JobVideo> Get(int id)
        {
            return await base.Context.Queryable<Live_JobVideo>().FirstAsync(it => it.ID == id);
        }

        public async Task<Model.Live_V_JobVideo> GetView(int id)
        {
            return await base.Context.Queryable<Live_V_JobVideo>().FirstAsync(it => it.ID == id);
        }
        public async Task<bool> Delete(int id)
        {
            return await base.Context.Deleteable<Live_JobVideo>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
    }
}
