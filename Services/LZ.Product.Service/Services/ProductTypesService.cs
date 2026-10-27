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


namespace LZ.Product.Service.Services
{
    public class ProductTypesService(IProductService productService,IAttributeService attributeService,IAttributeValuesService attributeValuesService) : ServicesBase<Model.Shop_ProductTypes>, IProductTypesService, IScopedDependency
    {
        public async Task<List<Shop_ProductTypes>> List(Expression<Func<Shop_ProductTypes, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Shop_ProductTypes>().Where(exp).OrderByDescending(it=>it.TypeID).ToListAsync();
            }
            var list = await base.Context.Queryable<Shop_ProductTypes>().Where(exp).OrderByDescending(it => it.TypeID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
            
        }
        public async Task<int> Add(Shop_ProductTypes model)
        {
            return await base.Context.Insertable(model).ExecuteReturnIdentityAsync();
        }
        public async Task<bool> Exist(Expression<Func<Shop_ProductTypes, bool>> exp)
        {
            return await base.Context.Queryable<Shop_ProductTypes>().AnyAsync(exp);
        }
        public override async Task<bool> UpdateAsync(Shop_ProductTypes model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<Shop_ProductTypes> Get(int id)
        {
            return await base.Context.Queryable<Shop_ProductTypes>().FirstAsync(it => it.TypeID == id);
        }
        public async Task<bool> Delete(int id)
        {
            if(await productService.Exist(it => it.ProductType == id))
            {
                throw new ErrorCodeException(-1, "该分类类型已使用!");
            }
            var has = await base.Context.Deleteable<Shop_ProductTypes>().Where(it => it.TypeID == id).ExecuteCommandAsync() > 0;
            if (has)
            {
                await attributeService.DeleteType(id);
            }
            return has;
        }
    }
}
