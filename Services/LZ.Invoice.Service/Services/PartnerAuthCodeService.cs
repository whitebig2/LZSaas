using LZ.Core;
using LZ.Core.Interface;
using LZ.Invoice.Service.Interface;
using LZ.Model;
using LZ.Model.PagedModel;
using SqlSugar;
using System.Linq.Expressions;

namespace LZ.Invoice.Service.Services
{
    public class PartnerAuthCodeService : ServicesBase<FIN_PartnerAuthCode>, IPartnerAuthCodeService, IScopedDependency
    {
        public async Task<List<FIN_PartnerAuthCode>> List(Expression<Func<FIN_PartnerAuthCode, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<FIN_PartnerAuthCode>().Where(exp).OrderByDescending(it => it.ID).ToListAsync();
            }
            var list = await base.Context.Queryable<FIN_PartnerAuthCode>().Where(exp).OrderByDescending(it => it.ID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<List<FIN_PartnerAuthCode>> List(Expression<Func<FIN_PartnerAuthCode, bool>> exp)
        {
            return await base.Context.Queryable<FIN_PartnerAuthCode>().Where(exp).OrderByDescending(it => it.ID).ToListAsync();
        }

        public async Task<int> Add(FIN_PartnerAuthCode model)
        {
            return await base.Context.Insertable(model).ExecuteCommandAsync();
        }

        public async Task<int> Add(List<FIN_PartnerAuthCode> models)
        {
            return await base.Context.Insertable(models).ExecuteCommandAsync();
        }

        public async Task<bool> Exist(int id)
        {
            return await base.Context.Queryable<FIN_PartnerAuthCode>().AnyAsync(it => it.ID == id);
        }

        public async Task<bool> Exist(Expression<Func<FIN_PartnerAuthCode, bool>> exp)
        {
            return await base.Context.Queryable<FIN_PartnerAuthCode>().Where(exp).AnyAsync();
        }

        public async Task<bool> Update(FIN_PartnerAuthCode model)
        {
            return await base.Context.Updateable<FIN_PartnerAuthCode>(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Update(List<FIN_PartnerAuthCode> models)
        {
            return await base.Context.Updateable<FIN_PartnerAuthCode>(models).ExecuteCommandAsync() > 0;
        }

        public async Task<FIN_PartnerAuthCode> Get(int id)
        {
            return await base.Context.Queryable<FIN_PartnerAuthCode>().FirstAsync(it => it.ID == id);
        }

        public async Task<FIN_PartnerAuthCode> Get(Expression<Func<FIN_PartnerAuthCode, bool>> exp)
        {
            return await base.Context.Queryable<FIN_PartnerAuthCode>().FirstAsync(exp);
        }

        public async Task<bool> Delete(int id)
        {
            return await base.Context.Deleteable<FIN_PartnerAuthCode>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Delete(Expression<Func<FIN_PartnerAuthCode, bool>> exp)
        {
            return await base.Context.Deleteable<FIN_PartnerAuthCode>().Where(exp).ExecuteCommandAsync() > 0;
        }

        public async Task<int> DictionarySet(Dictionary<string, object> dt)
        {
            return await base.Context.Updateable<FIN_PartnerAuthCode>(dt).With(SqlWith.UpdLock).ExecuteCommandAsync();
        }
    }
}
