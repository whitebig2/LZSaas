using LZ.Core;
using LZ.Core.Interface;
using LZ.Core.Redis;
using LZ.Model;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using LZ.Product.Service.Interface;
using Polly;
using SqlSugar;
using SqlSugar.IOC;
using System.ComponentModel.Design;
using System.Linq.Expressions;
using X.PagedList;


namespace LZ.Product.Service.Services
{
    public class ShoppingCartService : ServicesBase<Model.Shop_V_ShoppingCart>, IShoppingCartService, IScopedDependency
    {
        public async Task<List<Shop_V_ShoppingCart>> List(Expression<Func<Shop_V_ShoppingCart, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Shop_V_ShoppingCart>().Where(exp).OrderByDescending(it=>it.CartID).ToListAsync();
            }
            var list = await base.Context.Queryable<Shop_V_ShoppingCart>().Where(exp).OrderByDescending(it => it.CartID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
            
        }
        public async Task<List<Shop_ShoppingCart>> List(Expression<Func<Shop_ShoppingCart, bool>> exp)
        {
            return await base.Context.Queryable<Shop_ShoppingCart>().Where(exp).ToListAsync();

        }
        public async Task<int> Add(Shop_ShoppingCart model)
        {
            return await base.Context.Insertable(model).ExecuteReturnIdentityAsync();
        }
        public async Task<bool> Exist(Expression<Func<Shop_ShoppingCart, bool>> exp)
        {
            return await base.Context.Queryable<Shop_ShoppingCart>().AnyAsync(exp);
        }
        public async Task<int> CartCount(Expression<Func<Shop_V_ShoppingCart, bool>> exp)
        {
            return await base.Context.Queryable<Model.Shop_V_ShoppingCart>().CountAsync(exp);
        }
        public async Task<bool> UpdateCount(int id, int count)
        {
            return await base.Context.Updateable<Model.Shop_ShoppingCart>().SetColumns(it=>it.ProductCount==count).Where(it=>it.CartID==id).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> UpdateCheck(int id, bool check)
        {
            return await base.Context.Updateable<Model.Shop_ShoppingCart>().SetColumns(it => it.IsCheck == check).Where(it => it.CartID == id).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> UpdateAllCheck(int userId, bool check)
        {
            return await base.Context.Updateable<Model.Shop_ShoppingCart>().SetColumns(it => it.IsCheck == check).Where(it => it.UserID == userId).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> UpdateIncrement(int productId,string skuId, int userId)
        {
            return await base.Context.Updateable<Model.Shop_ShoppingCart>().SetColumns(it => it.ProductCount == it.ProductCount + 1).Where(it => it.ProductID == productId && it.SkuID == skuId && it.UserID == userId).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Delete(int id,int userId)
        {
            return await base.Context.Deleteable<Shop_ShoppingCart>().Where(it => it.CartID == id && it.UserID == userId).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Delete(string ids, int userId)
        {
            var idList=ids.Split(',');
            return await base.Context.Deleteable<Shop_ShoppingCart>().Where(it => idList.Contains(it.CartID.ToString()) && it.UserID == userId).ExecuteCommandAsync() > 0;
        }
    }
}
