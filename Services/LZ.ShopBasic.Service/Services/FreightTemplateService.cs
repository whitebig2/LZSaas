using LZ.Core;
using LZ.Core.Interface;
using LZ.Core.Redis;
using LZ.Model;
using LZ.Model.Exceptions;
using LZ.ShopBasic.Service.Interface;
using System.Linq.Expressions;


namespace LZ.ShopBasic.Service.Services
{
    public class FreightTemplateService : ServicesBase<Model.Shop_Attribute>, IFreightTemplateService, IScopedDependency
    {
        public async Task<List<Shop_FreightTemplate>> List(Expression<Func<Shop_FreightTemplate, bool>> exp)
        {
            return await base.Context.Queryable<Shop_FreightTemplate>().Where(exp).OrderByDescending(it => it.ID).ToListAsync();

        }
        public async Task<int> Add(Shop_FreightTemplate model)
        {
            return await base.Context.Insertable(model).ExecuteReturnIdentityAsync();
        }
        public async Task<bool> Exist(Expression<Func<Shop_FreightTemplate, bool>> exp)
        {
            return await base.Context.Queryable<Shop_FreightTemplate>().AnyAsync(exp);
        }
        public async Task<bool> Update(Shop_FreightTemplate model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<Shop_FreightTemplate> Get(int id)
        {
            return await base.Context.Queryable<Shop_FreightTemplate>().FirstAsync(it => it.ID == id);
        }
        public async Task<bool> Delete(int id)
        {
            var has = await base.Context.Deleteable<Shop_FreightTemplate>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
            return has;
        }
    }
}
