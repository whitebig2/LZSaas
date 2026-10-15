using LZ.Product.Service.Interface;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Core.Redis;
using LZ.Model;
using LZ.Model.PagedModel;
using Polly;
using SqlSugar;
using SqlSugar.IOC;
using System.ComponentModel.Design;
using System.Linq.Expressions;


namespace LZ.Product.Service.Services
{
    public class SKUsService : ServicesBase<Model.Shop_SKUs>, ISKUsService, IScopedDependency
    {
        public async Task<List<Shop_SKUs>> List(Expression<Func<Shop_SKUs, bool>> exp)
        {
            return await base.Context.Queryable<Shop_SKUs>().Where(exp).OrderBy(it => it.Sort).ToListAsync();
        }
        public async Task<int> Add(Shop_SKUs model)
        {
            return await base.Context.Insertable(model).ExecuteReturnIdentityAsync();
        }
        public async Task<int> Adds(List<Shop_SKUs> models)
        {
            return await base.Context.Insertable(models).ExecuteReturnIdentityAsync();
        }
        public async Task<bool> Exist(Expression<Func<Shop_SKUs, bool>> exp)
        {
            return await base.Context.Queryable<Shop_SKUs>().AnyAsync(exp);
        }
        public override async Task<bool> UpdateAsync(Shop_SKUs model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> ChangeInventory(int productId, string skuId, int count)
        {
            return await base.Context.Updateable<Model.Shop_SKUs>().SetColumns(it=>it.Stock==count).Where(it=>it.ProductID==productId && it.SkuID==skuId).ExecuteCommandAsync() > 0;
        }
        public async Task<Shop_SKUs> Get(string id)
        {
            return await base.Context.Queryable<Shop_SKUs>().FirstAsync(it => it.SkuID == id);
        }
        public async Task<bool> Delete(string id)
        {
            return await base.Context.Deleteable<Shop_SKUs>().Where(it => it.SkuID == id).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> ProductDelete(int productId)
        {
            return await base.Context.Deleteable<Shop_SKUs>().Where(it => it.ProductID == productId).ExecuteCommandAsync() > 0;
        }
    }
}
