using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using LZ.Core.EventData;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Company.Service.Interface;

namespace LZ.Company.Service.EventHandler
{
    public class CompanyUserEventHandler(IServiceScopeFactory scopeFactory): IEventHandler<CompanyUserEventData>
    {
        public async Task HandleAsync(CompanyUserEventData @event)
        {
            Common.StaticLog.Debug("开始处理注册分后台账号！id:"+ @event.FullName);
            using var scope = scopeFactory.CreateScope();
            var companyUserService = scope.ServiceProvider.GetService<ICompanyUserService>();
            try
            {
                if (companyUserService != null)
                {
                    var model = new Model.SYS_CompanyUser();
                    if (await companyUserService.ExistName(@event.UserName))
                    {
                        model.UserName = @event.UserName+$"_{Common.Tool.GetRandomNumber(4)}";
                    }
                    else
                    {
                        model.UserName = @event.UserName;
                    }
                    model.UserGID = Guid.NewGuid();
                    model.CreateTime = DateTime.Now;
                    model.PartnerID = @event.PartnerID;
                    model.PartnerGID = @event.PartnerGID;
                    model.CompanyID = @event.CompanyID;
                    model.CompanyGID = @event.CompanyGID;
                    model.Phone = @event.Phone;
                    model.FullName = @event.FullName;
                    model.Enabled = true;
                    model.UserType= @event.UserType;
                    model.UserPassword= @event.UserPassword;
                    model.RoleNames = @event.RoleNames;
                    model.RoleIDS = @event.RoleIDS;
                    model.PersonnelID = @event.PersonnelID;
                    model.PersonnelGID = @event.PersonnelGID;
                    await companyUserService.Add(model);
                }
                
            }
            catch (Exception ex) { 
                Common.StaticLog.Error(ex);
            }
            
        }
    }
}
