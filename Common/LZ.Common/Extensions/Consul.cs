using Consul.AspNetCore;
using Consul;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace LZ.Common.Extensions
{
    public static class ConsulServiceExtensions
    {
        /// <summary>
        /// 向容器中添加Consul必要的依赖注入
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddMCodeConsul(this IServiceCollection services)
        {
            try
            {
                var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
                // 配置consul服务注册信息
                var consulOptions = configuration.GetSection("Consul").Get<Model.ConsulModel>();
                // 通过consul提供的注入方式注册consulClient
                services.AddConsul(options => options.Address = new Uri($"http://{consulOptions.ConsulIP}:{consulOptions.ConsulPort}"));

                // 通过consul提供的注入方式进行服务注册
                var httpCheck = new AgentServiceCheck()
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),//服务启动多久后注册
                    Interval = TimeSpan.FromSeconds(10),//健康检查时间间隔，或者称为心跳间隔
                    HTTP = $"http://{consulOptions.IP}:{consulOptions.Port}/health",//健康检查地址
                    Timeout = TimeSpan.FromSeconds(10)
                };

                // Register service with consul
                services.AddConsulServiceRegistration(options =>
                {
                    options.Checks = new[] { httpCheck };
                    options.ID = Guid.NewGuid().ToString();
                    options.Name = consulOptions.ServiceName;
                    options.Address = consulOptions.IP;
                    options.Port = consulOptions.Port;
                    options.Meta = new Dictionary<string, string>() { { "Weight", consulOptions.Weight.HasValue ? consulOptions.Weight.Value.ToString() : "1" } };
                    options.Tags = new[] { $"urlprefix-/{consulOptions.ServiceName}" }; //添加
                });
            }
            catch (Exception ex) {
                Common.StaticLog.Error(ex);
            }

            return services;
        }

        /// <summary>
        /// 配置Consul检查服务
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseConsulCheckService(this IApplicationBuilder app)
        {
            try
            {
                app.Map("/health", app =>
            {
                app.Run(async context =>
                {
                    await Task.Run(() => context.Response.StatusCode = 200);
                });
            });
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
            }
            return app;
        }
        /// <summary>
        /// 设置心跳响应
        /// </summary>
        /// <param name="app"></param>
        /// <param name="checkPath">默认是/Health</param>
        /// <returns></returns>
        public static void UseHealthCheckMiddleware(this IApplicationBuilder app, string checkPath = "/health")
        {
            try
            {
                app.Map(checkPath, applicationBuilder => applicationBuilder.Run(async context =>
            {
                Console.WriteLine($"This is Health Check");
                context.Response.StatusCode = (int)HttpStatusCode.OK;
                await context.Response.WriteAsync("OK");
            }));
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
            }
        }
    }
}
