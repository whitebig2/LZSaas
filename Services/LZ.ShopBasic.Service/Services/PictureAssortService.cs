using LZ.Core;
using LZ.Core.Interface;
using LZ.Core.Redis;
using LZ.Model;
using LZ.Model.Exceptions;
using LZ.ShopBasic.Service.Interface;
using System.Linq.Expressions;


namespace LZ.ShopBasic.Service.Services
{
    public class PictureAssortService : ServicesBase<Model.Shop_PictureAssort>, IPictureAssortService, IScopedDependency
    {
        public async Task<List<Shop_PictureAssort>> List(Expression<Func<Shop_PictureAssort, bool>> exp)
        {
            return await base.Context.Queryable<Shop_PictureAssort>().Where(exp).OrderByDescending(it => it.AssortID).ToListAsync();

        }
        public async Task<int> Add(Shop_PictureAssort model)
        {
            return await base.Context.Insertable(model).ExecuteReturnIdentityAsync();
        }
        public async Task<bool> Exist(Expression<Func<Shop_PictureAssort, bool>> exp)
        {
            return await base.Context.Queryable<Shop_PictureAssort>().AnyAsync(exp);
        }
        public async Task<bool> Update(Shop_PictureAssort model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<Shop_PictureAssort> Get(int id)
        {
            return await base.Context.Queryable<Shop_PictureAssort>().FirstAsync(it => it.AssortID == id);
        }
        public async Task<bool> Delete(int id)
        {
            var has = await base.Context.Deleteable<Shop_PictureAssort>().Where(it => it.AssortID == id).ExecuteCommandAsync() > 0;
            return has;
        }
    }
}
