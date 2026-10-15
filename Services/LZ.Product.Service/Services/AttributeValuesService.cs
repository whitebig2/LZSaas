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
    public class AttributeValuesService(ISKUItemsService sKUItemsService) : ServicesBase<Model.Shop_AttributeValues>, IAttributeValuesService, IScopedDependency
    {
        public async Task<List<Shop_AttributeValues>> List(Expression<Func<Shop_AttributeValues, bool>> exp)
        {
            return await base.Context.Queryable<Shop_AttributeValues>().Where(exp).OrderByDescending(it => it.ValueID).ToListAsync();

        }
        public async Task<int> Add(Shop_AttributeValues model)
        {
            return await base.Context.Insertable(model).ExecuteReturnIdentityAsync();
        }
        public async Task<int> Add(List<Shop_AttributeValues> models)
        {
            return await base.Context.Insertable(models).ExecuteCommandAsync();
        }
        public async Task<bool> Exist(Expression<Func<Shop_AttributeValues, bool>> exp)
        {
            return await base.Context.Queryable<Shop_AttributeValues>().AnyAsync(exp);
        }
        public override async Task<bool> UpdateAsync(Shop_AttributeValues model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<Shop_AttributeValues> Get(int id)
        {
            return await base.Context.Queryable<Shop_AttributeValues>().FirstAsync(it => it.ValueID == id);
        }
        public async Task<bool> Delete(int id)
        {
            if(await sKUItemsService.Exist(it => it.ValueID == id))
            {
                throw new ErrorCodeException(-1, "删除失败，有商品使用该规格！");
            }
            return await base.Context.Deleteable<Shop_AttributeValues>().Where(it => it.ValueID == id).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> AttributeDelete(int attributeId)
        {
            return await base.Context.Deleteable<Shop_AttributeValues>().Where(it => it.AttributeID== attributeId).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> AttributeTypeDelete(int typeId)
        {
            return await base.Context.Deleteable<Shop_AttributeValues>().Where(it => SqlFunc.Subqueryable<Model.Shop_Attribute>().Where(s => s.TypeID == typeId && s.AttributeID == it.AttributeID).Any()).ExecuteCommandAsync() > 0;
        }
    }
}
