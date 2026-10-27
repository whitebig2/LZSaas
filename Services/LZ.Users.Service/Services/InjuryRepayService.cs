using LZ.Model;
using SqlSugar.IOC;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LZ.Core.Interface;
using LZ.Users.Service.Interface;
using LZ.Common;
using LZ.Model.Enums;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using LZ.Core;

namespace LZ.Users.Service.Services
{
    public class InjuryRepayService : ServicesBase<Live_InjuryRepay>, IInjuryRepayService, IScopedDependency
    {
        public async Task<List<Model.Live_V_InjuryRepay>> List(Expression<Func<Model.Live_V_InjuryRepay, bool>> exp, Model.PagedModel.UserPagedModel query, string order = "")
        {
            var count = new RefAsync<int>();
            var sco = base.Context.Queryable<Live_V_InjuryRepay>().Where(exp);
            if (!string.IsNullOrEmpty(order))
            {
                sco.OrderBy(order);
            }

            if (query.Page == 0)
            {
                return await sco.ToListAsync();
            }
            var list = await sco.ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<long> Add(Model.Live_InjuryRepay model)
        {
            var db = base.Context;
            return await db.Insertable(model).ExecuteReturnBigIdentityAsync();
        }
        public async Task<bool> Update(Model.Live_InjuryRepay model)
        {
            var db = base.Context;
            return await db.Updateable(model).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Delete(int id)
        {
            var db = base.Context;
            return await db.Deleteable<Model.Live_InjuryRepay>().Where(t => t.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Exist(Expression<Func<Live_InjuryRepay, bool>> exp)
        {
            return await base.Context.Queryable<Live_InjuryRepay>().AnyAsync(exp);
        }
        public async Task<Model.Live_InjuryRepay> Get(int id)
        {
            var db = base.Context;
            return await db.Queryable<Model.Live_InjuryRepay>().Where(t => t.ID == id).FirstAsync();
        }
        
        public async Task<bool> UpdateStatus(int id, int status, string msg = "")
        {

            var db = base.Context;
            var ub = db.Updateable<Live_InjuryRepay>();
            if (status > 0)
            {
                ub.SetColumns(it => it.Status == status);
            }
            if (!string.IsNullOrEmpty(msg))
            {
                ub.SetColumns(it => it.Content == msg);
            }
            ub.Where(t => t.ID == id);
            return await ub.ExecuteCommandAsync() > 0;
        }
        public async Task<bool> ExistID(int id)
        {
            return await base.Context.Queryable<Live_InjuryRepay>().AnyAsync(it => it.ID == id);
        }
        public async Task<Live_V_InjuryRepay> View(int id, string statusArray="", int companyId = 0)
        {
            var expable = Expressionable.Create<Model.Live_V_InjuryRepay>();
            if(id>0)
            {
                expable.And(it => it.ID == id);
            }
            
            if (companyId > 0)
            {
                expable.And(it => it.CompanyID == companyId);
            }
            if(!string.IsNullOrEmpty(statusArray))
            {
                var ids = statusArray.Split(',');
                expable.And(t => ids.Contains(t.Status.ToString()));
            }
           
            var exp = expable.ToExpression();
            return await View(exp);
        }
        public async Task<Live_V_InjuryRepay> View(Expression<Func<Live_V_InjuryRepay, bool>> exp)
        {
            return await base.Context.Queryable<Live_V_InjuryRepay>().FirstAsync(exp);
        }


        



    }
}
