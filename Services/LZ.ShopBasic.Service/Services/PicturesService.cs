using LZ.Core;
using LZ.Core.Interface;
using LZ.Core.Redis;
using LZ.Model;
using LZ.Model.Exceptions;
using LZ.ShopBasic.Service.Interface;
using System.Linq.Expressions;


namespace LZ.ShopBasic.Service.Services
{
    public class PicturesService : ServicesBase<Model.Shop_Pictures>, IPicturesService, IScopedDependency
    {
        public async Task<List<Shop_V_Pictures>> List(Expression<Func<Shop_V_Pictures, bool>> exp)
        {
            return await base.Context.Queryable<Shop_V_Pictures>().Where(exp).OrderByDescending(it => it.ID).ToListAsync();

        }
        public async Task<int> Add(Shop_Pictures model)
        {
            return await base.Context.Insertable(model).ExecuteReturnIdentityAsync();
        }
        public async Task<int> Add(List<Model.Shop_Pictures> models)
        {
            return await base.Context.Insertable(models).ExecuteCommandAsync();
        }
        public async Task<bool> Exist(Expression<Func<Shop_Pictures, bool>> exp)
        {
            return await base.Context.Queryable<Shop_Pictures>().AnyAsync(exp);
        }
        public async Task<bool> Update(Shop_Pictures model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Update(string[] ids,int assortId)
        {
            return await base.Context.Updateable<Shop_Pictures>().SetColumns(it=>it.AssortID== assortId).Where(it=>ids.Contains(it.ID.ToString())).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Update(int pictureId, string name)
        {
            return await base.Context.Updateable<Shop_Pictures>().SetColumns(it => it.Name == name).Where(it =>it.ID== pictureId).ExecuteCommandAsync() > 0;
        }
        public async Task<Shop_Pictures> Get(int id)
        {
            return await base.Context.Queryable<Shop_Pictures>().FirstAsync(it => it.ID == id);
        }
        public async Task<bool> Delete(int id)
        {
            var has = await base.Context.Deleteable<Shop_Pictures>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
            return has;
        }
        public async Task<bool> Delete(string[] ids)
        {
            var has = await base.Context.Deleteable<Shop_Pictures>().Where(it => ids.Contains(it.ID.ToString())).ExecuteCommandAsync() > 0;
            return has;
        }
    }
}
