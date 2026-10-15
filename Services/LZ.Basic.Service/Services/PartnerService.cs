using LZ.Basic.Service.Interface;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Model;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using SqlSugar;
using SqlSugar.IOC;
using System.Linq.Expressions;


namespace LZ.Basic.Service.Services
{
    public class PartnerService : ServicesBase<Live_Partner>, IPartnerService, IScopedDependency
    {
        public async Task<List<Live_Partner>> List(Expression<Func<Live_Partner, bool>> exp, UserPagedModel query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_Partner>().Where(exp).OrderByDescending(it => it.PartnerID).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_Partner>().Where(exp).OrderByDescending(it => it.PartnerID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<List<Live_V_PartnerCustomer>> CustomerList(Expression<Func<Live_V_PartnerCustomer, bool>> exp, UserPagedModel query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_V_PartnerCustomer>().Where(exp).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_V_PartnerCustomer>().Where(exp).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<int> Add(Live_Partner model)
        {
            model.PartnerID = await base.Context.Insertable(model).ExecuteReturnIdentityAsync();
            if (!string.IsNullOrEmpty(model.CIDS))
            {
                var models = new List<Live_PartnerCustomer>();
                var cids=model.CIDS.Split(',');
                foreach(var cid in cids)
                {
                    var m =new Live_PartnerCustomer();
                    m.PID = model.PartnerID;
                    m.CID=Convert.ToInt32(cid);
                    models.Add(m);
                }
                if (models.Count > 0)
                {
                    await base.Context.Insertable(models.ToArray()).ExecuteCommandAsync();
                }
            }
            return model.PartnerID;
        }
        public async Task<bool> ChangeIntegral(Guid partnerGID, int integral)
        {
            var has = await base.Context.Updateable<Model.Live_Partner>().SetColumns(it => it.Integral == SqlFunc.IsNull(it.Integral, 0) + integral).Where(it => it.PartnerGID == partnerGID).ExecuteCommandAsync() > 0;
            return has;
        }
        public async Task<bool> Exist(int id, int companyId = 0)
        {
            var expable = Expressionable.Create<Model.Live_Partner>();
            expable.And(it => it.PartnerID == id);
            if (companyId > 0)
            {
                expable.And(it => it.CompanyID == companyId);
            }
            var exp = expable.ToExpression();
            return await base.Context.Queryable<Live_Partner>().AnyAsync(exp);
        }
        public async Task<bool> Exist(Expression<Func<Live_Partner, bool>> exp)
        {
            return await base.Context.Queryable<Live_Partner>().AnyAsync(exp);
        }
        public async Task<bool> Update(Live_Partner model)
        {
            var has = await base.Context.Updateable(model).Where(it => it.PartnerID == model.PartnerID).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
            if (has)
            {
                await base.Context.Deleteable<Live_PartnerCustomer>().Where(it=>it.PID==model.PartnerID).ExecuteCommandAsync();
                if (!string.IsNullOrEmpty(model.CIDS))
                {
                    var models = new List<Live_PartnerCustomer>();
                    var cids = model.CIDS.Split(',');
                    foreach (var cid in cids)
                    {
                        var m = new Live_PartnerCustomer();
                        m.PID = model.PartnerID;
                        m.CID = Convert.ToInt32(cid);
                        models.Add(m);
                    }
                    if (models.Count > 0)
                    {
                        await base.Context.Insertable(models.ToArray()).ExecuteCommandAsync();
                    }
                }
            }
            
            return has;
        }
        public async Task<bool> Set(int id, bool enabled, int companyId = 0)
        {
            var expable = Expressionable.Create<Model.Live_Partner>();
            expable.And(it => it.PartnerID == id);
            if (companyId > 0)
            {
                expable.And(it => it.CompanyID == companyId);
            }
            var exp = expable.ToExpression();
            return await base.Context.Updateable<Live_Partner>().SetColumns(it => it.Enabled == enabled).Where(exp).ExecuteCommandAsync() > 0;
        }

        public async Task<Live_Partner> Get(int id, int companyId = 0)
        {
            var expable = Expressionable.Create<Model.Live_Partner>();
            expable.And(it => it.PartnerID == id);
            if (companyId > 0)
            {
                expable.And(it => it.CompanyID == companyId);
            }
            var exp = expable.ToExpression();
            return await base.Context.Queryable<Live_Partner>().FirstAsync(exp);
        }
        public async Task<bool> Delete(int id, int companyId = 0)
        {
            if(await base.Context.Queryable<Live_UserEntry>().AnyAsync(it => it.PartnerID == id))
            {
                throw new ErrorCodeException(-1, "该服务商名下有人员关联不可删除");
            }
            if (await base.Context.Queryable<Live_Personnel>().AnyAsync(it => it.PartnerID == id))
            {
                throw new ErrorCodeException(-1, "有驻厂关联该服务商不可删除");
            }
            if (await base.Context.Queryable<Live_Customer>().AnyAsync(it => it.PartnerID == id))
            {
                throw new ErrorCodeException(-1, "有客户关联该服务商不可删除");
            }
            var expable = Expressionable.Create<Model.Live_Partner>();
            expable.And(it => it.PartnerID == id);
            if (companyId > 0)
            {
                expable.And(it => it.CompanyID == companyId);
            }
            var exp = expable.ToExpression();
            return await base.Context.Deleteable<Live_Partner>().Where(exp).ExecuteCommandAsync() > 0;
        }


        public async Task<Live_Partner> Get(Expression<Func<Live_Partner, bool>> exp)
        {
            return await base.Context.Queryable<Live_Partner>().FirstAsync(exp);
        }
    }
}
