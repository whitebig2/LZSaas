using LZ.Core;
using LZ.Core.Interface;
using LZ.Core.Redis;
using LZ.Model;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using LZ.Product.Service.Interface;
using NPOI.SS.Formula.Functions;
using Polly;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
using SqlSugar;
using SqlSugar.IOC;
using System.ComponentModel.Design;
using System.Linq.Expressions;


namespace LZ.Product.Service.Services
{
    public class PromotionService : ServicesBase<Model.Shop_Promotion>, IPromotionService, IScopedDependency
    {
        public async Task<List<Shop_Promotion>> List(Expression<Func<Shop_Promotion, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            { 
                return await base.Context.Queryable<Shop_Promotion>().Where(exp).OrderByDescending(it => it.PromotionID).ToListAsync();
            }
            var list = await base.Context.Queryable<Shop_Promotion>().Where(exp).OrderByDescending(it => it.PromotionID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;

        }
        public async Task<List<Shop_V_PromotionProduct>> Items(Expression<Func<Shop_V_PromotionProduct, bool>> exp, PagedModelBase query=null)
        {
            if (query == null)
            {
                return await base.Context.Queryable<Shop_V_PromotionProduct>().Where(exp).OrderByDescending(it => it.ID).ToListAsync();
            }
            else
            {
                var count = new RefAsync<int>();
                if (query.Page == 0)
                {
                    return await base.Context.Queryable<Shop_V_PromotionProduct>().Where(exp).OrderByDescending(it => it.PromotionID).ToListAsync();
                }
                var list = await base.Context.Queryable<Shop_V_PromotionProduct>().Where(exp).OrderByDescending(it => it.PromotionID).ToOffsetPageAsync(query.Page, query.PageSize, count);
                query.Count = count.Value;
                return list;
            }
               
            
        }
        public async Task<int> Add(Shop_Promotion model)
        {
            return await base.Context.Insertable(model).ExecuteReturnIdentityAsync();
        }
        public async Task<int> AddItems(List<Shop_PromotionProduct> models)
        {
            return await base.Context.Insertable(models).ExecuteCommandAsync();
        }
        public async Task<bool> UpdateItems(List<Shop_PromotionProduct> models, int promotionId, Guid promotionGid)
        {
            var updateItems = models.Where(it => it.ID > 0).ToList();
            var addItems = models.Where(it => it.ID == 0).ToList();
            foreach (var item in addItems)
            {
                item.PromotionGID = promotionGid;
                item.PromotionID = promotionId;
            }
            await base.Context.Updateable(updateItems).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            var ids = updateItems.Select(it => it.ID).ToList();
            await base.Context.Deleteable<Shop_PromotionProduct>().Where(it => !ids.Contains(it.ID) && it.PromotionID == promotionId).ExecuteCommandAsync();
            await base.Context.Insertable(addItems).ExecuteCommandAsync();
            return true;
        }
        public async Task<bool> Exist(Expression<Func<Shop_Promotion, bool>> exp)
        {
            return await base.Context.Queryable<Shop_Promotion>().AnyAsync(exp);
        }
        public async Task<Shop_Promotion> Get(Expression<Func<Shop_Promotion, bool>> exp)
        {
            return await base.Context.Queryable<Shop_Promotion>().FirstAsync(exp);
        }
        public override async Task<bool> UpdateAsync(Shop_Promotion model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<Shop_Promotion> Get(int id)
        {
            return await base.Context.Queryable<Shop_Promotion>().FirstAsync(it => it.PromotionID == id);
        }
        public async Task<bool> Delete(int id)
        {
            var has = await base.Context.Deleteable<Shop_Promotion>().Where(it => it.PromotionID == id).ExecuteCommandAsync() > 0;
            if (has)
            {
                await base.Context.Deleteable<Shop_PromotionProduct>().Where(it => it.PromotionID == id).ExecuteCommandAsync();
            }
            return has;
        }
    }
}
