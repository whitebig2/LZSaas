using LZ.ApiService.Interface;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Model;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using LZ.Product.Service.Interface;
using SqlSugar;
using System.Linq.Expressions;


namespace LZ.Product.Service.Services
{
    public class CategoriesService(IProductService productService, IHupunERPService hupunERPService) : ServicesBase<Model.Shop_Categories>, ICategoriesService, IScopedDependency
    {
        public async Task<List<Shop_Categories>> List(Expression<Func<Shop_Categories, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Shop_Categories>().Where(exp).OrderByDescending(it=>it.CateID).ToListAsync();
            }
            var list = await base.Context.Queryable<Shop_Categories>().Where(exp).OrderByDescending(it => it.CateID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
            
        }
        public async Task<int> Add(Shop_Categories model)
        {
            model.CateID = await base.Context.Insertable(model).ExecuteReturnIdentityAsync();
            if (model.CateID > 0)
            {
                
                var list = new List<Shop_Categories>();
                list.Add(model);
                await hupunERPService.Categories(list);
            }
            return model.CateID;
        }
        public async Task<bool> Exist(Expression<Func<Shop_Categories, bool>> exp)
        {
            return await base.Context.Queryable<Shop_Categories>().AnyAsync(exp);
        }
        public override async Task<bool> UpdateAsync(Shop_Categories model)
        {
            var has = await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
            if (has)
            {
                var list = new List<Shop_Categories>();
                list.Add(model);
                await hupunERPService.Categories(list);
            }
            return has;
        }

        public async Task<Shop_Categories> Get(int id)
        {
            return await base.Context.Queryable<Shop_Categories>().FirstAsync(it => it.CateID == id);
        }
        public async Task<bool> Delete(int id)
        {
            if(await productService.Exist(it => it.CateID == id))
            {
                throw new ErrorCodeException(-1, "该分类已被商品使用！");
            }
            var model=await Get(id);
            var has = await base.Context.Deleteable<Shop_Categories>().Where(it => it.CateID == id).ExecuteCommandAsync() > 0;
            if (has)
            {
                var list=new List<Shop_Categories>();
                list.Add(model);
               await  hupunERPService.Categories(list, 0);
            }
            return has;
        }
    }
}
