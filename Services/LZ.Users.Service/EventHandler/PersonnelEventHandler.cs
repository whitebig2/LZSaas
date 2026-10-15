using LZ.Core.EventData;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Users.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Security.Policy;

namespace LZ.Users.Service.EventHandler
{
    public class PersonnelEventHandler(IServiceScopeFactory scopeFactory): IEventHandler<PersonnelEventData>
    {
        public async Task HandleAsync(PersonnelEventData @event)
        {
            Common.StaticLog.Debug("开始处理注册人事！id:"+ @event.PartnerID);
            using var scope = scopeFactory.CreateScope();
            var personnelService = scope.ServiceProvider.GetService<IPersonnelService>();
            var personnelItemsService = scope.ServiceProvider.GetService<IPersonnelItemsService>();
            var publisher = scope.ServiceProvider.GetService<IEventPublisher>();
            try
            {
                if (personnelService != null)
                {
                    var model = new Model.Live_Personnel();
                    model.CreateTime = DateTime.Now;
                    model.PartnerID = @event.PartnerID;
                    model.PartnerGID = @event.PartnerGID;
                    model.CompanyID = @event.CompanyID;
                    model.CompanyGID = @event.CompanyGID;
                    model.Phone = @event.Phone;
                    model.FullName = @event.FullName;
                    model.Enabled = true;
                    model.UserType= @event.UserType;
                    model.PartnerName= @event.PartnerName;
                    model.UserGID=Guid.NewGuid();
                    model.UserID = await personnelService.Add(model);
                    publisher.Publish(new UserQrcodeEventData()
                    {
                        UserID = model.UserID,
                        CompanyID = model.CompanyID.Value,
                        CompanyGID = model.CompanyGID.Value,
                        FromType = "lz"
                    });
                    if (@event.CustomerID > 0)
                    {
                        var item=new Model.Live_PersonnelItems() { 
                            UserID=model.UserID,
                            CID=@event.CustomerID,
                            GID=Guid.NewGuid(),
                            Enabled=true,
                            IsManage=true
                        };
                        await personnelItemsService.Add(item);
                    }
                    if (@event.IsRegisterCompanyUser)
                    {
                        var record = new CompanyUserEventData();
                        record.PartnerID = model.PartnerID.Value;
                        record.PartnerGID = model.PartnerGID.Value;
                        record.CompanyID = model.CompanyID.Value;
                        record.CompanyGID = model.CompanyGID.Value;
                        record.PersonnelID = model.UserID;
                        record.PersonnelGID= model.UserGID.Value;
                        record.FullName = model.FullName;
                        record.UserName = model.UserName;
                        record.UserPassword = Common.Tool.Md5Psw32("111111");
                        record.UserType = (int)Model.Enums.CompanyUserType.Partner;
                        record.Phone = model.Phone;
                        //自营公司的默认权限
                        record.RoleNames = "服务商";
                        record.RoleIDS = "5cc7afeb-6e6d-4e20-bae9-5e0ba23ab1d2";
                        publisher.Publish(record);
                    }
                }
                
            }
            catch (Exception ex) { 
                Common.StaticLog.Error(ex);
            }
            
        }
    }
}
