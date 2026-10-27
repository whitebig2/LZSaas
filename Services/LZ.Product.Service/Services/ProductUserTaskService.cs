using LZ.Core;
using LZ.Core.EventData;
using LZ.Core.Interface;
using LZ.Core.Redis;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.PagedModel;
using LZ.Product.Service.Interface;
using SqlSugar;
using System.Linq.Expressions;


namespace LZ.Product.Service.Services
{
    public class ProductUserTaskService : ServicesBase<Model.Shop_ProductUserTask>, IProductUserTaskService, IScopedDependency
    {
        public async Task<List<Shop_V_ProductUserTask>> List(Expression<Func<Shop_V_ProductUserTask, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Shop_V_ProductUserTask>().Where(exp).OrderByDescending(it=>it.ID).ToListAsync();
            }
            var list = await base.Context.Queryable<Shop_V_ProductUserTask>().Where(exp).OrderByDescending(it => it.ID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;

        }
        public async Task<int> Add(Shop_ProductUserTask model)
        {
            await base.Context.Insertable(model).ExecuteReturnEntityAsync();
            return model.ID;
        }
        public async Task<int> AddItem(Shop_ProductUserTaskItems model)
        {
            await base.Context.Insertable(model).ExecuteReturnEntityAsync();
            if (model.ID > 0)
            {
                await base.Context.Updateable<Shop_ProductUserTask>().SetColumns(it=>it.HaveCount==it.HaveCount+1).Where(it=>it.ID==model.UserTaskID).ExecuteCommandAsync();
            }
            return model.ID;
        }
        public async Task<bool> Exist(Expression<Func<Shop_V_ProductUserTask, bool>> exp)
        {
            return await base.Context.Queryable<Shop_V_ProductUserTask>().AnyAsync(exp);
        }
        public async Task<bool> UpdateAsync(Shop_ProductUserTask model)
        {
            return await base.Context.Updateable(model).ExecuteCommandAsync() > 0;
        }
        public async Task<int> CountSync(Expression<Func<Shop_V_ProductUserTask, bool>> exp)
        {
            return await base.Context.Queryable<Shop_V_ProductUserTask>().CountAsync(exp);
        }
        public async Task<Shop_ProductUserTask> Get(int id)
        {
            return await base.Context.Queryable<Shop_ProductUserTask>().FirstAsync(it => it.ID == id);
        }
        public async Task<Shop_V_ProductUserTask> GetView(int id)
        {
            return await base.Context.Queryable<Shop_V_ProductUserTask>().FirstAsync(it => it.ID == id);
        }
        public async Task<Shop_V_ProductUserTask> GetView(Expression<Func<Shop_V_ProductUserTask, bool>> exp)
        {
            return await base.Context.Queryable<Shop_V_ProductUserTask>().FirstAsync(exp);
        }
        public async Task<bool> SetStatus(int id, UserTaskStatus status)
        {
            var has = await base.Context.Updateable<Shop_ProductUserTask>().SetColumns(it => it.Status == (int)status).Where(it=>it.ID==id).ExecuteCommandAsync() > 0;
            return has;
        }
        public async Task<bool> Cancel(int id, int userId=0)
        {
            var expable = Expressionable.Create<Model.Shop_ProductUserTask>();
            expable.And(it=>it.ID==id);
            expable.AndIF(userId>0,it=>it.UserID==userId);
            var exp = expable.ToExpression();
            var has = await base.Context.Updateable<Shop_ProductUserTask>().SetColumns(it=>it.Status==(int)Model.Enums.UserTaskStatus.Cancel).Where(exp).ExecuteCommandAsync() > 0;
            return has;
        }
        public async Task<bool> Delete(int id,int userId=0)
        {
            var expable = Expressionable.Create<Model.Shop_ProductUserTask>();
            expable.And(it => it.ID == id);
            expable.AndIF(userId > 0, it => it.UserID == userId);
            var exp = expable.ToExpression();
            var has = await base.Context.Deleteable<Shop_ProductUserTask>().Where(exp).ExecuteCommandAsync() > 0;
            return has;
        }
    }
}
