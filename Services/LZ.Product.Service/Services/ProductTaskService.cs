using LZ.Core;
using LZ.Core.EventData;
using LZ.Core.Interface;
using LZ.Core.Redis;
using LZ.Model;
using LZ.Model.Enums;
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
using System.Security.Policy;


namespace LZ.Product.Service.Services
{
    public class ProductTaskService(IProductUserTaskService productUserTaskService): ServicesBase<Model.Shop_ProductTask>, IProductTaskService, IScopedDependency
    {
        public async Task<List<Shop_V_ProductTask>> List(Expression<Func<Shop_V_ProductTask, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Shop_V_ProductTask>().Where(exp).OrderByDescending(it=>it.TaskID).ToListAsync();
            }
            var list = await base.Context.Queryable<Shop_V_ProductTask>().Where(exp).OrderByDescending(it => it.TaskID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
            
        }
        public async Task<List<Shop_ProductTaskItems>> Items(Expression<Func<Shop_ProductTaskItems, bool>> exp)
        {
            return await base.Context.Queryable<Shop_ProductTaskItems>().Where(exp).OrderBy(it=>it.Sort).ToListAsync();
        }
        public async Task<List<Shop_ProductTaskItems>> WeightItems(Expression<Func<Shop_ProductTaskItems, bool>> exp)
        {
            return await base.Context.Queryable<Shop_ProductTaskItems>().Where(exp).OrderBy(it => it.Weight).ToListAsync();
        }
        public async Task<int> Add(Shop_ProductTask model)
        {
            return await base.Context.Insertable(model).ExecuteReturnIdentityAsync();
        }
        public async Task<int> AddItems(List<Shop_ProductTaskItems> models)
        {
            return await base.Context.Insertable(models).ExecuteCommandAsync();
        }
        public async Task<bool> Exist(Expression<Func<Shop_ProductTask, bool>> exp)
        {
            return await base.Context.Queryable<Shop_ProductTask>().AnyAsync(exp);
        }
        public async Task<Shop_ProductTask> Get(Expression<Func<Shop_ProductTask, bool>> exp)
        {
            return await base.Context.Queryable<Shop_ProductTask>().FirstAsync(exp);
        }
        public async Task<bool> UpdateAsync(Shop_ProductTask model)
        {
            return await base.Context.Updateable(model).IgnoreNullColumns(true) .ExecuteCommandAsync() > 0;
        }
        public async Task<bool> AddUseCount(int taskId,int count=1)
        {
            return await base.Context.Updateable<Shop_ProductTask>().SetColumns(it=>it.UseCount==it.UseCount+count).Where(it=>it.TaskID==taskId).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> AddItemUseCount(int id, int count = 1)
        {
            return await base.Context.Updateable<Shop_ProductTaskItems>().SetColumns(it => it.UseCount == it.UseCount + count).Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> UpdateItems(List<Shop_ProductTaskItems> items)
        {
            return await base.Context.Updateable(items).ExecuteCommandAsync() > 0;
        }
        public async Task<Shop_ProductTask> Get(int id)
        {
            return await base.Context.Queryable<Shop_ProductTask>().FirstAsync(it => it.TaskID == id);
        }
        public async Task<bool> SetStatus(int id, int status)
        {
            return await base.Context.Updateable<Shop_ProductTask>().SetColumns(it=>it.Status==status).Where(it => it.TaskID == id).ExecuteCommandAsync()>0;
        }
        public async Task<Shop_V_ProductTask> GetView(int id)
        {
            return await base.Context.Queryable<Shop_V_ProductTask>().FirstAsync(it => it.TaskID == id);
        }
        public async Task<bool> DeleteItem(int[] ids,int taskId)
        {
            var has = await base.Context.Deleteable<Shop_ProductTaskItems>().Where(it => !ids.Contains(it.ID) && it.TaskID==taskId).ExecuteCommandAsync() > 0;
            return has;
        }
        public async Task<bool> Delete(int id)
        {
            var status =new int[]{ (int)UserTaskStatus.Oning,(int)UserTaskStatus.Exchange};
            ;
            if(await productUserTaskService.Exist(it => it.TaskID == id && status.Contains(it.Status.Value)))
            {
                throw new ErrorCodeException(-1, "有用户正在进行中或者等待兑换中");
            }
            var has = await base.Context.Deleteable<Shop_ProductTask>().Where(it => it.TaskID == id).ExecuteCommandAsync() > 0;
            return has;
        }
    }
}
