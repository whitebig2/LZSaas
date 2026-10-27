using LZ.Core.EventData;
using LZ.Core.Interface;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Core.Services;
using LZ.Model.Enums;
using LZ.Users.Service.Interface;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;

namespace LZ.Users.Service.EventHandler
{
    public class IntegralLogEventHandler(IServiceScopeFactory scopeFactory): IEventHandler<IntegralLogEventData>
    {
        public async Task HandleAsync(IntegralLogEventData @event)
        {
            Common.StaticLog.Debug("开始处理积分日志事件！id:"+ @event.UserID+";update:"+ @event.IsUpdate);
            using var scope = scopeFactory.CreateScope();
            var integralService = scope.ServiceProvider.GetService<IIntegralService>();
            var userService = scope.ServiceProvider.GetService<IUserService>();
            var httpService = scope.ServiceProvider.GetService<IHttpService>();
            var configService = scope.ServiceProvider.GetService<ICompanyConfigService>();
            var personnelService = scope.ServiceProvider.GetService<IPersonnelService>();
            try
            {
                if (integralService != null)
                {
                    if(@event.IntegralType == Model.Enums.IntegralType.ShareJob || @event.IntegralType == Model.Enums.IntegralType.Recommend || @event.IntegralType == Model.Enums.IntegralType.Register)
                    {
                        var companyInfo = await configService.Get(@event.CompanyGID);
                        var companyConfig = companyInfo.Config;
                        switch (@event.IntegralType)
                        {
                            case Model.Enums.IntegralType.ShareJob:
                                @event.Integral = companyConfig.ShopShareJobIntegral;
                                break;
                            case Model.Enums.IntegralType.Recommend:
                                @event.Integral = companyConfig.ShopRecommendIntegral;
                                break;
                            case Model.Enums.IntegralType.Register:
                                @event.Integral = companyConfig.ShopRegisterIntegral;
                                break;
                        }
                    }
                    if (@event.Integral == 0)
                    {
                        return;
                    }
                    if (@event.ToLZUserID > 0)//用户通过职喜用户分享的岗位或者注册，获取积分，ToLZUserID为职喜用户ID，需要转换成幸福职用户ID才能修改积分
                    {
                        var lzUser = await personnelService.Get(@event.ToLZUserID);
                        if (lzUser == null)
                        {
                            Common.StaticLog.Debug($"{@event.ToLZUserID}驻厂信息获取失败");
                            return;
                        }
                        var xfUser = await userService.Get(it => it.Phone == lzUser.Phone && it.CompanyID == @event.CompanyID);
                        if (xfUser == null)
                        {
                            Common.StaticLog.Debug($"id:{lzUser.UserID};{lzUser.Phone}幸福职信息获取失败");
                            return;
                        }
                        @event.UserID = xfUser.UserID;
                    }
                    switch (@event.IntegralType)
                    {
                        case Model.Enums.IntegralType.ShareJob:
                            {
                                var job = await httpService.GetAsync<Model.Live_Jobs>(ServiceNames.CustomerService, $"/api/jobs/getmodel?id={@event.FromJobID}");
                                if (job == null)
                                {
                                    Common.StaticLog.Debug($"{@event.FromJobID}岗位获取失败");
                                    return;
                                }
                                else
                                {
                                    if (!job.Integrals.HasValue || job.Integrals <= 0)
                                    {
                                        return;
                                    }
                                    var jobIntegrals = await integralService.Sum(it => it.FromJobID == @event.FromJobID && it.IntegralType == (int)Model.Enums.IntegralType.ShareJob && it.CompanyID == @event.CompanyID);
                                    if (job.Integrals < jobIntegrals + @event.Integral)
                                    {
                                        Common.StaticLog.Debug($"{@event.FromJobID}岗位积分不足");
                                        return;
                                    }
                                }
                                ///同一岗位同一用户只能有一次分享积分
                                if (await integralService.Exist(it => it.UserID == @event.UserID && it.FromJobID == @event.FromJobID && it.IntegralType == (int)Model.Enums.IntegralType.ShareJob))
                                {
                                    return;
                                }
                            }
                            break;
                        case Model.Enums.IntegralType.Register:
                            {
                                ///同一用户只能有一次注册积分
                                if (await integralService.Exist(it => it.UserID == @event.UserID && it.IntegralType == (int)Model.Enums.IntegralType.Register))
                                {
                                    return;
                                }
                            }
                            break;
                        case Model.Enums.IntegralType.Sign:
                            {
                                ///一天只能签到一次
                                if (await integralService.Exist(it => it.UserID == @event.UserID && it.IntegralType == (int)Model.Enums.IntegralType.Sign && SqlFunc.DateIsSame(it.CreateTime, DateTime.Now)))
                                {
                                    return;
                                }
                            }
                            break;
                    }
                    var model = new Model.Shop_IntegralLog();
                    model.CreateTime = DateTime.Now;
                    model.Integral = @event.Integral;
                    model.UserID = @event.UserID;
                    model.IntegralType = (int)@event.IntegralType;
                    model.OrderID = @event.OrderID;
                    model.Remark = @event.Remark;
                    model.PartnerGID = @event.PartnerGID;
                    model.PartnerID = @event.PartnerID;
                    model.FromUserID = @event.FromUserID;
                    model.CompanyID=@event.CompanyID;
                    model.CompanyGID=@event.CompanyGID;
                    model.FromJobID = @event.FromJobID;
                    model.ToLZUserID = @event.ToLZUserID;
                    model.RefundID = @event.RefundID;
                    var has = await integralService.Add(model);
                    if (has>0)
                    {
                        if (@event.IsUpdate)
                        {
                            await userService.ChangeIntegral(@event.UserID, @event.Integral);
                        }
                        
                        if (@event.IntegralType == Model.Enums.IntegralType.PartnerRecharge || @event.IntegralType == Model.Enums.IntegralType.PartnerGift || @event.IntegralType == Model.Enums.IntegralType.JobRecharge)
                        {
                            var integral = @event.Integral;
                            if (@event.IntegralType == Model.Enums.IntegralType.PartnerGift || @event.IntegralType == Model.Enums.IntegralType.JobRecharge)
                            {
                                integral = @event.Integral*-1;
                            }
                            var paras=new Dictionary<string, string>();
                            paras.Add("id",@event.PartnerGID.ToString());
                            paras.Add("integral", integral.ToString());
                           var res = await httpService.PutAsync(ServiceNames.BasicService, "/api/partner/changeintegral", paras);
                            if(res.Code!=200 || !Convert.ToBoolean(res.Data))
                            {
                                Common.StaticLog.Error($"{@event.PartnerGID}更新积分出错{@event.IntegralType}积分：{integral}");
                            }
                        }
                        if (@event.IntegralType == Model.Enums.IntegralType.JobRecharge || @event.IntegralType == Model.Enums.IntegralType.ShareJob)
                        {
                            var integral = @event.Integral;
                            if (@event.IntegralType == Model.Enums.IntegralType.ShareJob)
                            {
                                integral = @event.Integral * -1;
                            }
                            var paras = new Dictionary<string, string>();
                            paras.Add("id", @event.FromJobID.ToString());
                            paras.Add("integral", integral.ToString());
                            var res = await httpService.PutAsync(ServiceNames.CustomerService, "/api/jobs/changeintegral", paras);
                            if (res.Code != 200 || !Convert.ToBoolean(res.Data))
                            {
                                Common.StaticLog.Error($"{@event.FromJobID}更新积分出错{@event.IntegralType}积分：{integral}");
                            }
                        }
                    }
                }
                
            }
            catch (Exception ex) {
                if(@event.IntegralType==IntegralType.CancelConsume || @event.IntegralType == IntegralType.Consume)
                {
                    Common.StaticLog.Debug("扣积分错误"+@event.OrderID);
                }
                Common.StaticLog.Error(ex);
            }
            
        }
    }
}
