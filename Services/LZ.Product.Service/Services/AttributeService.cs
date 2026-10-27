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
    public class AttributeService(ISKUItemsService sKUItemsService,IAttributeValuesService attributeValuesService) : ServicesBase<Model.Shop_Attribute>, IAttributeService, IScopedDependency
    {
        public async Task<List<Shop_Attribute>> List(Expression<Func<Shop_Attribute, bool>> exp)
        {
            return await base.Context.Queryable<Shop_Attribute>().Where(exp).OrderBy(it => it.Sort).ToListAsync();

        }
        public async Task<int> Add(Shop_Attribute model)
        {
            return await base.Context.Insertable(model).ExecuteReturnIdentityAsync();
        }
        public async Task<int> Add(Shop_Attribute model, List<Shop_AttributeValues> items)
        {
            model.AttributeID = await base.Context.Insertable(model).ExecuteReturnIdentityAsync();
            foreach(var item in items)
            {
                item.AttributeID = model.AttributeID;
            }
           await  attributeValuesService.Add(items);
            return model.AttributeID;
        }
        public async Task<bool> Exist(Expression<Func<Shop_Attribute, bool>> exp)
        {
            return await base.Context.Queryable<Shop_Attribute>().AnyAsync(exp);
        }
        public override async Task<bool> UpdateAsync(Shop_Attribute model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<Shop_Attribute> Get(int id)
        {
            return await base.Context.Queryable<Shop_Attribute>().FirstAsync(it => it.AttributeID == id);
        }
        public async Task<bool> Delete(int id)
        {
            if(await sKUItemsService.Exist(it => it.AttributeID == id))
            {
                throw new ErrorCodeException(-1, "删除失败，有商品使用该规格！");
            }
            var has = await base.Context.Deleteable<Shop_Attribute>().Where(it => it.AttributeID == id).ExecuteCommandAsync() > 0;
            if (has)
            {
                await attributeValuesService.AttributeDelete(id);
            }
            return has;
        }
        public async Task<bool> DeleteType(int id)
        {
            var has = await base.Context.Deleteable<Shop_Attribute>().Where(it => it.TypeID == id).ExecuteCommandAsync() > 0;
            if (has)
            {
                await attributeValuesService.AttributeTypeDelete(id);
            }
            return has;
        }
    }
}
