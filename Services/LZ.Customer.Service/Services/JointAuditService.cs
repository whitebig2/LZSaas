using LZ.Customer.Service.Interface;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Model;
using SqlSugar;
using SqlSugar.IOC;
using System.Linq.Expressions;
using System.Text;
using LZ.Model.PagedModel;


namespace LZ.Customer.Service.Services
{
    public class JointAuditService : ServicesBase<Live_JointAudit>, IJointAuditService, IScopedDependency
    {
        public async Task<List<Live_V_JointAudit>> List(Expression<Func<Live_V_JointAudit, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var qb = base.Context.Queryable<Live_V_JointAudit>();
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
        public async Task<int> Add(Live_JointAudit model)
        {
          return  await base.Context.Insertable(model).ExecuteReturnIdentityAsync();
        }
        public async Task<bool> Exist(int id,int companyId=0)
        {
            var expable = Expressionable.Create<Model.Live_V_JointAudit>();
            expable.And(it => it.ID == id);
            if (companyId > 0)
            {
                expable.And(it => it.CompanyID == companyId);
            }
            var exp = expable.ToExpression();
            return await base.Context.Queryable<Live_V_JointAudit>().AnyAsync(exp);
        }
        public async Task<bool> Update(Live_JointAudit model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> ChangePartner(int id, int partnerId,Guid partnerGID)
        {
            return await base.Context.Updateable<Live_JointAudit>().SetColumns(it => new Model.Live_JointAudit() { PartnerID= partnerId,PartnerGID= partnerGID }).Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> ChangeStatus(int id,int status,string remarks)
        {
            return await base.Context.Updateable<Live_JointAudit>().SetColumns(it => new Model.Live_JointAudit() { Status = status, AuditRemarks = remarks }).Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<Live_JointAudit> Get(int id)
        {
            return await base.Context.Queryable<Live_JointAudit>().FirstAsync(it=>it.ID == id);
        }
        public async Task<bool> Delete(int id)
        {
            return await base.Context.Deleteable<Live_JointAudit>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
    }
}
