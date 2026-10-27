using LZ.ApiService.Interface;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Model;
using LZ.Model.ApiModel;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using LZ.Product.Service.Interface;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using SqlSugar;
using System.Linq.Expressions;
using System.Text;


namespace LZ.Product.Service.Services
{
    public class ProductService(ISKUItemsService sKUItemsService,ISKUsService sKUsService,IHupunERPService hupunERPService,IProductTaskService productTaskService): ServicesBase<Model.Shop_Product>, IProductService, IScopedDependency
    {
        public async Task<List<Shop_Product>> List(Expression<Func<Shop_Product, bool>> exp, PagedModelBase query, Expression<Func<Shop_Product, object>>? order=null)
        {
            var count = new RefAsync<int>();
            if (order == null)
            {
                order = it => it.ProductID;
            }
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Shop_Product>().Where(exp).OrderByDescending(order).ToListAsync();
            }
            var list = await base.Context.Queryable<Shop_Product>().Where(exp).OrderByDescending(order).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
            
        }
        public async Task<int> Addold(Shop_Product model, List<Model.Shop_SKUs> skus, List<Model.Shop_Attribute> attrs, List<Model.Shop_AttributeValues> values)
        {
            List<Model.Shop_SKUItems> items = new List<Shop_SKUItems>();
            model.ProductID = await base.Context.Insertable(model).ExecuteReturnIdentityAsync();
            if (model.HasSKU)
            {
                foreach (var attr in attrs)
                {
                    attr.ProductID = model.ProductID;
                    attr.AttributeID = await base.Context.Insertable(attr).ExecuteReturnIdentityAsync();
                }
                foreach (var value in values)
                {
                    var attr = attrs.Find(it => it.AttributeStrID == value.AttributeStrID);
                    value.AttributeID = attr.AttributeID;
                    value.AttributeStrID = "";
                    value.ValueID = await base.Context.Insertable(value).ExecuteReturnIdentityAsync();
                }
                var i = 0;
                foreach (var sku in skus)
                {
                    i++;
                    sku.Sort = i;
                    sku.ProductID = model.ProductID;
                    var valueids = sku.SkuID.Split('_');
                    StringBuilder newSkuID = new StringBuilder();
                    foreach (var valueid in valueids)
                    {
                        var value = values.Find(it => it.ValueStrID == valueid);
                        newSkuID.AppendFormat("{0}_", value.ValueID);
                        var item = new Shop_SKUItems();
                        item.AttributeID = value.AttributeID.Value;
                        item.ValueID = value.ValueID;
                        items.Add(item);
                    }
                    newSkuID.Length--;
                    sku.SkuID = newSkuID.ToString();
                    sku.SkuID = model.ProductID + "_" + sku.SkuID;
                    foreach (var item in items)
                    {
                        item.SkuID = sku.SkuID;
                    }
                }
                await base.Context.Insertable(items).ExecuteReturnIdentityAsync();
            }
            else
            {
                foreach (var sku in skus)
                {
                    sku.ProductID = model.ProductID;
                    sku.SkuID = model.ProductID + "_" + sku.SkuID;
                }
            }
            await base.Context.Insertable(skus).ExecuteReturnIdentityAsync();
            return model.ProductID;
        }
        public async Task<int> Add(Shop_Product model,List<Model.Shop_SKUs> skus, List<Model.Shop_Attribute> attrs, List<Model.Shop_AttributeValues> values)
        {
            var db = itenant;
            List<Model.Shop_SKUItems> items = new List<Shop_SKUItems>();
            var result = await db.UseTranAsync(async () =>
            {
                model.ProductID = await db.InsertableWithAttr(model).ExecuteReturnIdentityAsync();
                if (model.HasSKU)
                {
                    foreach (var attr in attrs)
                    {
                        attr.ProductID = model.ProductID;
                        attr.IsDelete= false;
                        attr.AttributeID = await db.InsertableWithAttr(attr).ExecuteReturnIdentityAsync();
                    }
                    foreach (var value in values)
                    {
                        var attr = attrs.Find(it => it.AttributeStrID == value.AttributeStrID);
                        value.AttributeID = attr.AttributeID;
                        value.AttributeStrID = "";
                        value.IsDelete = false;
                        value.ValueID = await db.InsertableWithAttr(value).ExecuteReturnIdentityAsync();
                    }
                    var i = 0;
                    foreach (var sku in skus)
                    {
                        i++;
                        sku.Sort = i;
                        sku.ProductID = model.ProductID;
                        var valueids = sku.SkuID.Split('_');
                        StringBuilder newSkuID=new StringBuilder();
                        var valueItems = new List<Model.Shop_SKUItems>();
                        foreach (var valueid in valueids)
                        {
                            var value = values.Find(it=>it.ValueStrID==valueid);
                            newSkuID.AppendFormat("{0}_", value.ValueID);
                            var item = new Shop_SKUItems();
                            item.AttributeID = value.AttributeID.Value;
                            item.ValueID = value.ValueID;
                            valueItems.Add(item);
                        }
                        newSkuID.Length--;
                        sku.SkuID = newSkuID.ToString();
                        sku.SkuID = model.ProductID + "_" + sku.SkuID;
                        foreach (var item in valueItems)
                        {
                            item.SkuID = sku.SkuID;
                        }
                        items.AddRange(valueItems);
                    }
                    await db.InsertableWithAttr(items).ExecuteReturnIdentityAsync();
                }
                else
                {
                    foreach (var sku in skus)
                    {
                        sku.ProductID = model.ProductID;
                        sku.SkuID = model.ProductID + "_" + sku.SkuID;
                    }
                }
                await db.InsertableWithAttr(skus).ExecuteReturnIdentityAsync();
                return model.ProductID;
            }, ex => { throw ex; });
            if (result.IsSuccess)
            {
                await SendProduct(model);
                return result.Data;
            }
            else
            {
                return -1;
            }
        }
        public async Task<bool> Exist(Expression<Func<Shop_Product, bool>> exp)
        {
            return await base.Context.Queryable<Shop_Product>().AnyAsync(exp);
        }
        public async Task<bool> UpdateAsync(Shop_Product model, List<Model.Shop_SKUs> skus, List<Model.Shop_Attribute> attrs, List<Model.Shop_AttributeValues> values)
        {
            List<Model.Shop_SKUItems> items = new List<Shop_SKUItems>();
            var db = itenant;
            var result = await db.UseTranAsync(async () =>
            {
                await db.UpdateableWithAttr(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
                await db.DeleteableWithAttr<Shop_SKUItems>().Where(it => SqlFunc.Subqueryable<Model.Shop_SKUs>().Where(s => s.ProductID == model.ProductID && s.SkuID == it.SkuID).Any()).ExecuteCommandAsync();
                if (model.HasSKU)
                {
                    var updateAttrs = attrs.Where(it => it.AttributeID >0).ToList();
                    foreach (var attr in attrs)
                    {
                        if (attr.AttributeID == 0)
                        {
                            attr.IsDelete = false;
                            attr.ProductID = model.ProductID;
                            attr.AttributeID = await db.InsertableWithAttr(attr).ExecuteReturnIdentityAsync();
                        }
                    }
                    await db.UpdateableWithAttr(updateAttrs).ExecuteCommandAsync();
                    var updateValues = values.Where(it => it.ValueID > 0).ToList();
                    foreach (var value in values)
                    {
                        if (value.ValueID == 0)
                        {
                            var attr = attrs.Find(it => it.AttributeStrID == value.AttributeStrID);
                            value.AttributeID = attr.AttributeID;
                            value.AttributeStrID = "";
                            value.IsDelete = false;
                            value.ValueID = await db.InsertableWithAttr(value).ExecuteReturnIdentityAsync();
                        }
                    }
                    await db.UpdateableWithAttr(updateValues).ExecuteCommandAsync();
                    var i = 0;
                    foreach (var sku in skus)
                    {
                        i++;
                        sku.Sort = i;
                        sku.ProductID = model.ProductID;
                        var valueids = sku.SkuID.Split('_');
                        StringBuilder newSkuID = new StringBuilder();
                        var valueItems = new List<Model.Shop_SKUItems>();
                        foreach (var valueid in valueids)
                        {
                            var value = valueid.IndexOf("v")>-1? values.Find(it => it.ValueStrID == valueid):values.Find(it=>it.ValueID==Convert.ToInt32(valueid));
                            newSkuID.AppendFormat("{0}_", value.ValueID);
                            var item = new Shop_SKUItems();
                            item.AttributeID = value.AttributeID.Value;
                            item.ValueID = value.ValueID;
                            valueItems.Add(item);
                        }
                        newSkuID.Length--;
                        sku.SkuID = newSkuID.ToString();
                        sku.SkuID = model.ProductID + "_" + sku.SkuID;
                        foreach (var item in valueItems)
                        {
                            item.SkuID = sku.SkuID;
                        }
                        items.AddRange(valueItems);
                    }
                    var valuesIds = values.Select(x => x.ValueID).ToList();
                    await db.UpdateableWithAttr<Model.Shop_AttributeValues>().SetColumns(it=>it.IsDelete==true).Where(it => SqlFunc.Subqueryable<Model.Shop_Attribute>().Where(s => s.ProductID == model.ProductID && s.AttributeID == it.AttributeID).Any() && !valuesIds.Contains(it.ValueID)).ExecuteCommandAsync();
                    var attrIds = attrs.Select(x => x.AttributeID).ToList();
                    await db.UpdateableWithAttr<Model.Shop_Attribute>().SetColumns(it => it.IsDelete == true).Where(it => it.ProductID == model.ProductID && !attrIds.Contains(it.AttributeID)).ExecuteCommandAsync();
                    await db.InsertableWithAttr(items).ExecuteReturnIdentityAsync();
                }
                else
                {
                    foreach (var sku in skus)
                    {
                        sku.ProductID = model.ProductID;
                        sku.SkuID = model.ProductID + "_" + sku.SkuID;
                    }
                }
                await db.DeleteableWithAttr<Model.Shop_SKUs>().Where(it => it.ProductID == model.ProductID).ExecuteCommandAsync();
                await db.InsertableWithAttr(skus).ExecuteReturnIdentityAsync();
                return true;
            });
            if (result.IsSuccess)
            {
                await SendProduct(null,model.ProductID);
                return result.Data;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 推送商品给erp
        /// </summary>
        /// <param name="model"></param>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<HupunResultModel?> SendProduct(Shop_Product model,int id=0,int status=1)
        {
            try
            {
                if (model == null)
                {
                    model = await base.Context.Queryable<Shop_Product>().FirstAsync(it => it.ProductID == id);
                }
                if (model.IsVirtual || model.ProductType!=(int)ProductType.Ordinary)
                {
                    return new HupunResultModel() { code = 4444, data = new HupunResultDataModel() { success = false, error_msg = "虚拟商品不可同步" } };
                }
                if (model.HasSKU)
                {
                    var skus = await sKUsService.List(it => it.ProductID == id);
                    var skuIds = skus.Select(it => it.SkuID).ToList();
                    var items = await sKUItemsService.List(it => skuIds.Contains(it.SkuID));
                    var res = await hupunERPService.Product(model, skus, items, status);
                    if (res == null) return null;
                    if (res.code != 0 || !res.data.success)
                    {
                        Common.StaticLog.Debug(JsonConvert.SerializeObject(res));
                    }
                    return res;
                }
                else
                {
                    return await hupunERPService.Product(model, status: status);
                }
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
                return new HupunResultModel() { code = 4444, data = new HupunResultDataModel() { success = false, error_msg = ex.Message } };
            }
        }
        /// <summary>
        /// 推送商品给erp(批量)
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<HupunResultModel?> SendProducts(Expression<Func<Shop_Product, bool>> exp, int status = 1)
        {
            try
            {
                var models = await base.Context.Queryable<Shop_Product>().Where(exp).ToListAsync();
                if (models.Count == 0)
                {
                    return new HupunResultModel() { code = 4444, data = new HupunResultDataModel() { success = false, error_msg = "无商品" } };
                }
                var productIds = models.Select(it=>it.ProductID).ToList();
                var skus = await sKUsService.List(it => productIds.Contains(it.ProductID.Value));
                var items = await sKUItemsService.List(it => SqlFunc.Subqueryable<Model.Shop_SKUs>().Where(s => s.SkuID == it.SkuID && productIds.Contains(s.ProductID.Value)).Any());
               var res =  await hupunERPService.Product(models, skus, items, status);
                if(res.code!=0 || !res.data.success)
                {
                    Common.StaticLog.Debug(JsonConvert.SerializeObject(res));
                }
                return res;
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
                return new HupunResultModel() { code = 4444, data = new HupunResultDataModel() { success = false, error_msg = ex.Message } };
            }

        }
        public async Task<Shop_Product> Get(int id)
        {
            return await base.Context.Queryable<Shop_Product>().FirstAsync(it => it.ProductID == id);
        }
        public async Task<Shop_V_Product> GetView(int id)
        {
            return await base.Context.Queryable<Shop_V_Product>().FirstAsync(it => it.ProductID == id);
        }
        public async Task<bool> SetStatus(int id, int status)
        {
            return await base.Context.Updateable<Shop_Product>().SetColumns(it => it.Status == status).Where(it => it.ProductID == id).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> ChangeStatus(List<int> productIds, ProductStatus status)
        {
            return await base.Context.Updateable<Shop_Product>().SetColumns(it => it.Status == (int)status).Where(it => productIds.Contains(it.ProductID)).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> ChangeInventory(int productId, int count)
        {
            return await base.Context.Updateable<Shop_Product>().SetColumns(it => it.ProductCount == count).Where(it => it.ProductID == productId).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> ChangeInventory(int productId)
        {
            var count = await base.Context.Queryable<Shop_SKUs>().Where(it=>it.ProductID==productId).SumAsync(it=>it.Stock);
            return await base.Context.Updateable<Shop_Product>().SetColumns(it => it.ProductCount == count).Where(it => it.ProductID == productId).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Delete(int id)
        {
            if(await productTaskService.Exist(it => it.ProductID == id))
            {
                throw new ErrorCodeException(-1, "删除失败，活动已在使用该商品！");
            }
            if(await base.Context.Queryable<Shop_Order>().AnyAsync(it => it.ProductID == id) || await base.Context.Queryable<Shop_OrderItems>().AnyAsync(it => it.ProductID == id))
            {
                throw new ErrorCodeException(-1, "删除失败，订单已在关联该商品！");
            }
            var model = await base.Context.Queryable<Shop_Product>().FirstAsync(it => it.ProductID == id);
            var has = await base.Context.Deleteable<Shop_Product>().Where(it => it.ProductID == id).ExecuteCommandAsync() > 0;
            if (has)
            {
                await sKUsService.ProductDelete(id);
                await sKUItemsService.ProductDelete(id);
                await SendProduct(model, 0, 0);
            }
            return has;
        }
    }
}
