using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using LZ.Core.EventData;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Users.Service.Interface;
using SqlSugar;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using LZ.Core;

namespace LZ.Users.Service.EventHandler
{
    public class CustomerJointEventHandler(IServiceScopeFactory scopeFactory): IEventHandler<CustomerJointEventData>
    {
        public async Task HandleAsync(CustomerJointEventData @event)
        {
            Common.StaticLog.Debug("开始处理联营事件！id:"+ @event.CustomerID);
            using var scope = scopeFactory.CreateScope();
            var personnelService = scope.ServiceProvider.GetService<IPersonnelService>();
            var personnelItemsService = scope.ServiceProvider.GetService<IPersonnelItemsService>();
            try
            {
                if (personnelService != null && personnelItemsService != null)
                {
                    var expable = Expressionable.Create<Model.Live_V_Personnel>();
                    var ps = @event.PartnerIDS.Split(',');
                    var ids=new List<int>();
                    foreach(var p in ps)
                    {
                        ids.Add(Convert.ToInt32(p));
                    }
                    expable.And(it => ids.Contains(it.PartnerID.Value));
                    var exp = expable.ToExpression();
                    var personnels = await personnelService.List(exp, new Model.PagedModel.UserPagedModel() { Page=0});
                    var itemExpable = Expressionable.Create<Model.Live_V_PersonnelItems>();
                    itemExpable.And(it=>ids.Contains(it.PartnerID.Value) && it.CID==@event.CustomerID);
                    var itemExp = itemExpable.ToExpression();
                    var oldItems = await personnelItemsService.List(itemExp, new Model.PagedModel.UserPagedModel() { Page = 0 });
                    var items = new List<Model.Live_PersonnelItems>();
                    foreach(var per in personnels)
                    {
                        if (!oldItems.Exists(it => it.UserID == per.UserID))
                        {
                            var item=new Model.Live_PersonnelItems();
                            item.CID=@event.CustomerID;
                            item.UserID=per.UserID;
                            item.Enabled = true;
                            item.IsJoin = true;
                            item.GID = Guid.NewGuid();
                            items.Add(item);
                        }
                    }
                   await personnelItemsService.Add(items);
                }
                
            }
            catch (Exception ex) { 
                Common.StaticLog.Error(ex);
            }
            
        }
    }
}
