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
    public class SKUItemsService : ServicesBase<Model.Shop_SKUItems>, ISKUItemsService, IScopedDependency
    {
        public async Task<List<Shop_V_SKUItems>> List(Expression<Func<Shop_V_SKUItems, bool>> exp)
        {
            return await base.Context.Queryable<Shop_V_SKUItems>().Where(exp).OrderByDescending(it => it.SkuID).ToListAsync();

        }
        public async Task<int> Add(Shop_SKUItems model)
        {
            return await base.Context.Insertable(model).ExecuteReturnIdentityAsync();
        }
        public async Task<int> Adds(List<Shop_SKUItems> models)
        {
            return await base.Context.Insertable(models).ExecuteCommandAsync();
        }
        public async Task<bool> Exist(Expression<Func<Shop_SKUItems, bool>> exp)
        {
            return await base.Context.Queryable<Shop_SKUItems>().AnyAsync(exp);
        }
        public override async Task<bool> UpdateAsync(Shop_SKUItems model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> ProductDelete(int productId)
        {
            return await base.Context.Deleteable<Shop_SKUItems>().Where(it => SqlFunc.Subqueryable<Model.Shop_SKUs>().Where(s => s.ProductID == productId && s.SkuID == it.SkuID).Any()).ExecuteCommandAsync() > 0;
        }
    }
}
