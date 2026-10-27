using LZ.Core;
using LZ.Core.Interface;
using LZ.Core.Services;
using LZ.Model;
using LZ.Model.PagedModel;
using LZ.Users.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Users.Service.Services
{
    public class PromotionTaskService(IUserService userService) : ServicesBase<Shop_PromotionTask>, IPromotionTaskService, IScopedDependency
    {
        public Task<int> Add(Shop_PromotionTask model)
        {
            return base.Context.Insertable(model).ExecuteReturnIdentityAsync();
        }

        public Task<bool> Delete(int id)
        {
            return base.Context.Deleteable<Shop_PromotionTask>().Where(it => it.ID == id).ExecuteCommandHasChangeAsync();
        }

        public async Task<List<Shop_PromotionTask>> List(PromotionPageTaskModel query)
        {
            var totalNumer = new RefAsync<int>();
            var data = await base.Context.Queryable<Shop_PromotionTask>().Where(it => SqlFunc.Between(it.CreateTime, query.startTime, query.endTime)).OrderByDescending(it => it.ID).ToOffsetPageAsync(query.Page, query.PageSize, totalNumer);
            query.Count = totalNumer.Value;
            return data;
        }

        public async Task UpdateHaveMoneyOfPromotionTask()
        {
            try
            {
                var now = DateTime.Now;
                var promotionTasks = await base.Context.Queryable<Shop_PromotionTask>().Where(it => it.CreateTime <= SqlFunc.DateAdd(now, -15, DateType.Minute)).ToListAsync();
                if (promotionTasks?.Count > 0)
                {
                    var userIds = new HashSet<int>(promotionTasks.Where(it => it.UserID.HasValue && it != null).Select(it => it.UserID!.Value)).ToList();
                    var ids = (await userService.List(it => it.FUID != null && userIds.Contains(it.FUID.Value), new Model.PagedModel.UserPagedModel { Page = 0 })).Select(t => t.UserID).ToList();
                    if (ids?.Count > 0)
                    {
                        var orderTotalMap = base.Context.Queryable<Shop_Order>().Where(it => it.UserID != null && ids.Contains(it.UserID.Value))
                             .GroupBy(g => g.UserID!.Value)
                             .ToDictionary(it => it.UserID!.Value, it => SqlFunc.AggregateSum(it.OrderTotal));
                        await base.Context.Ado.BeginTranAsync();
                        foreach (var (k, v) in orderTotalMap)
                        {
                            var id = int.Parse(k);
                            var orderTotal = Convert.ToInt32(v?.ToString() ?? "0");
                            var promotionTask = promotionTasks.Find(x => x.UserID == id);
                            if (promotionTask != null)
                            {
                                await base.Context.Updateable<Shop_PromotionTask>().SetColumns(it => it.HaveMoney == orderTotal).Where(it => it.UserID == id).ExecuteCommandAsync();
                            }
                        }
                        await base.Context.Ado.CommitTranAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
                await base.Context.Ado.RollbackTranAsync();
            }
            await Task.CompletedTask;
        }

        public new Task<int> Update(Shop_PromotionTask model)
        {
            return base.Context.Updateable(model).Where(it => it.ID == model.ID).ExecuteCommandAsync();
        }
    }
}
