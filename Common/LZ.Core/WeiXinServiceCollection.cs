using LZ.Core.RabbitMQ;
using LZ.Core.RabbitMQ.EventBus;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Senparc.CO2NET;
using Senparc.CO2NET.AspNet;
using Senparc.Weixin.AspNet;
using Senparc.Weixin.Cache.CsRedis;
using Senparc.Weixin.Entities;
using Senparc.Weixin.Helpers;
using Senparc.Weixin.MP;
using Senparc.Weixin.WxOpen;
using SqlSugar.IOC;
using Senparc.Weixin.RegisterServices;

namespace LZ.Core
{
    public static partial class WeiXinServiceCollection
    {
        /// <summary>
        /// 加载微信组件Senparc配置
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder AddSenparcConfig(this WebApplication app)
        {
            var senparcWeixinSetting = app.Services.GetService<IOptions<SenparcWeixinSetting>>()!.Value;
            var senpacSetting = app.Services.GetService<IOptions<SenparcSetting>>()!.Value;
            var registerService = app.UseSenparcGlobal(app.Environment, senpacSetting, globalRegister =>
            {
                #region CO2NET 全局配置

                #region 全局缓存配置（按需）

                //当同一个分布式缓存同时服务于多个网站（应用程序池）时，可以使用命名空间将其隔离（非必须）
                //globalRegister.ChangeDefaultCacheNamespace("DefaultCO2NETCache");

                #region 配置和使用 Redis

                //配置全局使用Redis缓存（按需，独立）
                if (UseRedis(senpacSetting, out string redisConfigurationStr))//这里为了方便不同环境的开发者进行配置，做成了判断的方式，实际开发环境一般是确定的，这里的if条件可以忽略
                {
                    /* 说明：
                     * 1、Redis 的连接字符串信息会从 Config.SenparcSetting.Cache_Redis_Configuration 自动获取并注册，如不需要修改，下方方法可以忽略
                    /* 2、如需手动修改，可以通过下方 SetConfigurationOption 方法手动设置 Redis 链接信息（仅修改配置，不立即启用）
                     */
                    Senparc.CO2NET.Cache.CsRedis.Register.SetConfigurationOption(redisConfigurationStr);

                    //以下会立即将全局缓存设置为 Redis
                    Senparc.CO2NET.Cache.CsRedis.Register.UseKeyValueRedisNow();//键值对缓存策略（推荐）

                    //Senparc.CO2NET.Cache.CsRedis.Register.UseHashRedisNow();//HashSet储存格式的缓存策略

                    //也可以通过以下方式自定义当前需要启用的缓存策略
                    //CacheStrategyFactory.RegisterObjectCacheStrategy(() => RedisObjectCacheStrategy.Instance);//键值对
                    //CacheStrategyFactory.RegisterObjectCacheStrategy(() => RedisHashSetObjectCacheStrategy.Instance);//HashSet

                    #region 注册 StackExchange.Redis

                    /* 如果需要使用 StackExchange.Redis，则可以使用 Senparc.CO2NET.Cache.Redis 库
                     * 注意：这一步注册和上述 CsRedis 库两选一即可，本 Sample 需要同时演示两个库，因此才都进行注册
                     */

                    //Senparc.CO2NET.Cache.Redis.Register.SetConfigurationOption(redisConfigurationStr);
                    //Senparc.CO2NET.Cache.Redis.Register.UseKeyValueRedisNow();//键值对缓存策略（推荐）

                    #endregion
                }
                //如果这里不进行Redis缓存启用，则目前还是默认使用内存缓存 

                #endregion


                #endregion



                #endregion
            }, true);
            return app;
        }
        /// <summary>
        /// 注册微信
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder RegisterWeiXin(this WebApplication app)
        {
            app.UseSenparcWeixin(app.Environment,
  null /* 不为 null 则覆盖 appsettings  中的 SenpacSetting 配置*/,
  null /* 不为 null 则覆盖 appsettings  中的 SenpacWeixinSetting 配置*/,
  register => { /* CO2NET 全局配置 */ },
            (register, weixinSetting) =>
            {
                register.UseSenparcWeixinCacheCsRedis();
                var types = new string[] { "mp", "mini" };
                var models = DbScoped.SugarScope.QueryableWithAttr<Model.Live_Clients>().Where(it => it.Enabled && types.Contains(it.AppType) && !string.IsNullOrEmpty(it.AppID)).ToList();
                foreach (var m in models)
                {
                    if (string.IsNullOrEmpty(m.Token)) m.Token = "1";
                    if (m.AppType == "mp")
                    {
                        register.RegisterMpAccount(new SenparcWeixinSetting
                        {
                            WeixinAppId = m.AppID,
                            WeixinAppSecret = m.AppSecret,
                            Token = m.Token,
                            EncodingAESKey = "1"
                        }, m.Name);
                    }
                    else
                    {
                        register.RegisterWxOpenAccount(new SenparcWeixinSetting
                        {
                            WxOpenAppId = m.AppID,
                            WxOpenAppSecret = m.AppSecret,
                            WxOpenToken = m.Token,
                            WxOpenEncodingAESKey = "1"
                        }, m.Name);
                    }
                }

            });
            return app;
        }
        /// <summary>
        /// 判断当前配置是否满足使用 Redis（根据是否已经修改了默认配置字符串判断）
        /// </summary>
        /// <param name="senparcSetting"></param>
        /// <returns></returns>
        private static bool UseRedis(SenparcSetting senparcSetting, out string redisConfigurationStr)
        {
            redisConfigurationStr = senparcSetting.Cache_Redis_Configuration;
            var useRedis = !string.IsNullOrEmpty(redisConfigurationStr) && redisConfigurationStr != "#{Cache_Redis_Configuration}#"/*默认值，不启用*/;
            return useRedis;
        }
        /// <summary>
        /// 注册微信支付
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IServiceCollection AddWeiXinPay(this IServiceCollection services, IConfiguration configuration)
        {
            var models = DbScoped.SugarScope.QueryableWithAttr<Model.SYS_Company>().Where(it => it.Products.Contains("Shop")).ToList();
            foreach (var m in models)
            {
                var companyConfig = JsonConvert.DeserializeObject<Model.CompanyConfig>(m.Config);
                if (!string.IsNullOrEmpty(companyConfig.WeiXinMerchantID) && !string.IsNullOrEmpty(companyConfig.WeiXinPaySecret) && !string.IsNullOrEmpty(companyConfig.WeiXinKeyFile))
                {
                    var path = companyConfig.WeiXinKeyFile;
                    string registerKey = TenPayHelper.GetRegisterKey(companyConfig.WeiXinMerchantID, "");
                    services.AddCertHttpClient(registerKey, companyConfig.WeiXinMerchantID, companyConfig.WeiXinKeyFile);
                }
            }
            return services;
        }
    }
}
