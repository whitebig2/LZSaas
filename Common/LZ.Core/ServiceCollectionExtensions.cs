using Consul;
using LZ.Common.Extensions;
using LZ.Core.Authentication;
using LZ.Core.CustomizeJsonConverter;
using LZ.Core.Filters;
using LZ.Core.Interface;
using LZ.Core.Middleware;
using LZ.Core.RabbitMQ;
using LZ.Core.Redis;
using LZ.Core.Services;
using LZ.Model;
using LZ.Model.Bank.Apip;
using LZ.Model.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OfficeOpenXml;
using Senparc.Weixin.RegisterServices;
using SqlSugar;
using SqlSugar.IOC;
using System.Runtime.Loader;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;

namespace LZ.Core
{
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// 项目初始化函数
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="moduleKey"></param>
        /// <param name="moduleTitle"></param>
        /// <returns></returns>
        /// 
        public static async Task InitAppliation(this WebApplicationBuilder builder, Model.AppliationModel appModel)
        {
            ExcelPackage.LicenseContext = LicenseContext.Commercial;

            builder.AddBuilderServices(appModel);
            var app = builder.Build();

           

            app.UseApp(appModel);
            try
            {
                await app.RunAsync();
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
            }
        }

        /// <summary>
        /// 应用程序启动时初始化
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="moduleKey"></param>
        /// <param name="moduleTitle"></param>
        /// <returns></returns>
        public static WebApplicationBuilder AddBuilderServices(this WebApplicationBuilder builder, Model.AppliationModel appModel)
        {
            // automapper
            builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

            var assemblyFiles = Directory.GetFiles(AppContext.BaseDirectory, "LZ.*.dll");
            foreach (var assemblyFile in assemblyFiles)
            {
                AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyFile);
            }

            builder.Configuration.AddJsonFile("config/commsettings.json", true, true);
            //测试用
            //builder.Configuration.AddJsonFile($"config/commsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);


            #region 绑定配置
            builder.Services.Configure<FxqOption>(builder.Configuration.GetSection("FxqOption"));
            builder.Services.Configure<FxqConfigOption>(builder.Configuration.GetSection("FxqConfigOption"));
            builder.Services.Configure<InsuranceOptions>(builder.Configuration.GetSection("InsuranceOptions"));
            builder.Services.Configure<AesOption>(builder.Configuration.GetSection("AesOption"));
            builder.Services.Configure<IcbcOpenCardOption>(builder.Configuration.GetSection("IcbcOpenCardOption"));
            builder.Services.Configure<BankServiceOption>(builder.Configuration.GetSection("BankServiceOption"));
            builder.Services.Configure<BankWalletOption>(builder.Configuration.GetSection("BankWalletOption"));
            builder.Services.Configure<BankAesOption>(builder.Configuration.GetSection("BankAesOption"));
            builder.Services.Configure<CzyOption>(builder.Configuration.GetSection("CzyOption"));
            builder.Services.Configure<BaiWangOption>(builder.Configuration.GetSection("BaiWangOption"));
            builder.Services.Configure<ThirtySixOptions>(builder.Configuration.GetSection("ThirtySixOptions"));
            #endregion
            //builder.Configuration.AddJsonFile(Path.Combine(AppContext.BaseDirectory, $"../../../../config/commsettings.json"), true, true);//本地测试使用
            // 注册IHttpContextAccessor
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularApp",
                    policy => policy.SetIsOriginAllowed(origin => true)
                                    .AllowAnyHeader()
                                    .AllowAnyMethod()
                                    .AllowCredentials());
            });

            builder.Services.AddSqlConfig(builder.Configuration);//加载数据库配置
            if (appModel.HasWeixin || appModel.HasWeiXinPay)
            {
                builder.Services.Configure<KestrelServerOptions>(options => options.AllowSynchronousIO = true);
                //builder.Services.AddMemoryCache();
                builder.Services.AddSenparcWeixin(builder.Configuration);//启动Senparc
                if (appModel.HasWeiXinPay)
                {
                    builder.Services.AddWeiXinPay(builder.Configuration);
                }
            }
            if (appModel.HasAddHub)
            {
                builder.Services.AddSignalR();
            }
            if (appModel.HasNode)
            {
                builder.Services.AddNodeServices();
            }
            if (appModel.HasAddMVC)
            {
                builder.Services.AddMvc();
            }
            builder.Services.AddControllers(options => {
                options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.JsonSerializerOptions.Converters.Add(new DateTimeJsonConverter());
                options.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            });

            builder.Services.AddServices<ITransientDependency>(ServiceLifetime.Transient);
            builder.Services.AddServices<IScopedDependency>(ServiceLifetime.Scoped);
            builder.Services.AddServices<ISingletonDependency>(ServiceLifetime.Singleton);

            builder.Services.AddAuthentication("Authorization-Token").AddScheme<RequestAuthenticationSchemeOptions, RequestAuthenticationHandler>("Authorization-Token", options => { });
            builder.Services.AddMCodeConsul();
            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<RequestAsyncResultFilter>();//统一接口返回的处理
                options.Filters.Add<ApiAsyncExceptionFilter>(); //接口异常统一处理
                options.Filters.Add<RequestAuthorizeFilter>();// 接口权限验证
                options.Filters.Add<OperationLogActionFilter>();// 操作日志统一处理
                //options.Filters.Add<RecordInterfaceFilter>();// 记录接口信息
            });

            // 注册 IHttpClientFactory 
            builder.Services.AddHttpClient();

            #region 添加服务调用请求客户端

            builder.Services.AddHttpClient(ServiceNames.BasicService, config =>
            {
                var consulUri = "http://" + builder.Configuration["Consul:ConsulIP"] + ":" + builder.Configuration["Consul:ConsulPort"];
                ConsulClient client = new ConsulClient(x =>
                {
                    x.Address = new Uri(consulUri);
                });
                var consulServiceList = client.Agent.Services().Result.Response;
                var serviceList = consulServiceList.Where(it => it.Value.Service == ServiceNames.BasicService).ToArray();
                var service = serviceList[new Random().Next(serviceList.Length)].Value;
                string url = "http://" + service.Address + ":" + service.Port;
                config.BaseAddress = new Uri(url);
                config.DefaultRequestHeaders.Add("Authorization", "C220F681-0052-4E09-9B30-6CDD735A69E1");
                config.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            });

            builder.Services.AddHttpClient(ServiceNames.CustomerService, config =>
            {

                var consulUri = "http://" + builder.Configuration["Consul:ConsulIP"] + ":" + builder.Configuration["Consul:ConsulPort"];
                ConsulClient client = new ConsulClient(x =>
                {
                    x.Address = new Uri(consulUri);
                });
                var consulServiceList = client.Agent.Services().Result.Response;
                var serviceList = consulServiceList.Where(it => it.Value.Service == ServiceNames.CustomerService).ToArray();
                var service = serviceList[new Random().Next(serviceList.Length)].Value;
                string url = "http://" + service.Address + ":" + service.Port;
                config.BaseAddress = new Uri(url);
                config.DefaultRequestHeaders.Add("Authorization", "C220F681-0052-4E09-9B30-6CDD735A69E1");
                config.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            });

            builder.Services.AddHttpClient(ServiceNames.UsersService, config =>
            {

                var consulUri = "http://" + builder.Configuration["Consul:ConsulIP"] + ":" + builder.Configuration["Consul:ConsulPort"];
                ConsulClient client = new ConsulClient(x =>
                {
                    x.Address = new Uri(consulUri);
                });
                var consulServiceList = client.Agent.Services().Result.Response;
                var serviceList = consulServiceList.Where(it => it.Value.Service == ServiceNames.UsersService).ToArray();
                var service = serviceList[new Random().Next(serviceList.Length)].Value;
                string url = "http://" + service.Address + ":" + service.Port;
                config.BaseAddress = new Uri(url);
                config.DefaultRequestHeaders.Add("Authorization", "C220F681-0052-4E09-9B30-6CDD735A69E1");
                config.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            });

            builder.Services.AddHttpClient(ServiceNames.FinanceService, config =>
            {

                var consulUri = "http://" + builder.Configuration["Consul:ConsulIP"] + ":" + builder.Configuration["Consul:ConsulPort"];
                ConsulClient client = new ConsulClient(x =>
                {
                    x.Address = new Uri(consulUri);
                });
                var consulServiceList = client.Agent.Services().Result.Response;
                var serviceList = consulServiceList.Where(it => it.Value.Service == ServiceNames.FinanceService).ToArray();
                var service = serviceList[new Random().Next(serviceList.Length)].Value;
                string url = "http://" + service.Address + ":" + service.Port;
                config.BaseAddress = new Uri(url);
                config.DefaultRequestHeaders.Add("Authorization", "C220F681-0052-4E09-9B30-6CDD735A69E1");
                config.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            });


            builder.Services.AddHttpClient(ServiceNames.CompanyService, config =>
            {

                var consulUri = "http://" + builder.Configuration["Consul:ConsulIP"] + ":" + builder.Configuration["Consul:ConsulPort"];
                ConsulClient client = new ConsulClient(x =>
                {
                    x.Address = new Uri(consulUri);
                });
                var consulServiceList = client.Agent.Services().Result.Response;
                var serviceList = consulServiceList.Where(it => it.Value.Service == ServiceNames.CompanyService).ToArray();
                var service = serviceList[new Random().Next(serviceList.Length)].Value;
                string url = "http://" + service.Address + ":" + service.Port;
                config.BaseAddress = new Uri(url);
                config.DefaultRequestHeaders.Add("Authorization", "C220F681-0052-4E09-9B30-6CDD735A69E1");
                config.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            });



            builder.Services.AddHttpClient("InsuranceApi", client =>
            {
                client.BaseAddress = new Uri("http://47.116.206.126:8894");
            });


            builder.Services.AddHttpClient("ThirtySix", client =>
            {
                client.BaseAddress = new Uri("https://www.gz36000.com");
                client.DefaultRequestHeaders.Add("Authorization", "Basic bGFtcF93ZWJfcGx1czpsYW1wX3dlYl9wbHVzX3NlY3JldA==");
                client.DefaultRequestHeaders.Add("Tenant", "MDAwMA==");
            });




            builder.Services.AddHttpClient("Xfz", client =>
            {
                client.BaseAddress = new Uri("https://entry.cn-labor.cn");
                client.DefaultRequestHeaders.Add("Authorization", "bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwic2lkIjoiMSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6ImFkbWluIiwiZmFtaWx5X25hbWUiOiIwMDAiLCJzdWIiOiIxIiwianRpIjoiZDI1ZTMyNDRhNDM0NDRjOWIzOGMyMWU3NGQzNzE0MmIiLCJpYXQiOiIxNjQyNjY2MjgzIiwibmJmIjoiMTY0MjY2NjI4MyIsImV4cCI6IjE5NTgxOTkwODMiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL2V4cGlyYXRpb24iOiIyMDMyLzEvMjAgMTY6MTE6MjMiLCJpc3MiOiJFbXBsb3llZUpvaW5MZWF2ZVN5c3RlbS5BcGkuSXNzdWVyIiwiYXVkIjoiRW1wbG95ZWVKb2luTGVhdmVTeXN0ZW0uQXBpLkF1ZGllbmNlIn0.I-hV2YL4sDKbCPnNPI9nWafrda_193dRraRB9CpJwZ8");
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            });

            builder.Services.AddHttpClient("ICBC", client =>
            {
                //client.BaseAddress = new Uri("http://109.32.98.130:8086");
                client.BaseAddress = new Uri("http://47.102.210.210:9981");
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            });

            #endregion
            builder.Services.AddRabbitMQ(builder.Configuration);
            // 本地开发不需要 MQ 时，注册空实现避免 DI 解析失败
            //builder.Services.AddSingleton<LZ.Core.RabbitMQ.EventBus.IEventPublisher, LZ.Core.RabbitMQ.NullEventPublisher>();

            builder.Services.AddSingleton<IContentTypeProvider, FileExtensionContentTypeProvider>();

            if (appModel.HasCzyApipClient)
            {



                builder.Services.AddSingleton<ApipClient>(sp =>
                {
                    var cfg = sp.GetRequiredService<IOptions<CzyOption>>().Value;
                    var client = new ApipClient(cfg.AppId, cfg.SignMethod, cfg.PrivateKey, cfg.PublicKey, cfg.EncryptMethod, cfg.EncryptKey);
                    client.Trace = (message) =>
                    {
                        var logger = sp.GetRequiredService<ILogger<ApipClient>>();
                        logger.LogInformation(message);
                    };
                    return client;
                });
            }
          
            return builder;
        }

        /// <summary>
        /// 应用程序启动时 注册中间件
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseApp(this WebApplication app, Model.AppliationModel appModel)
        {
            //app.UseHttpLogging();

            app.UseRedis(app.Configuration);
            if (appModel.HasWeixin || appModel.HasWeiXinPay)//注册微信
            {
                app.AddSenparcConfig();
                if (appModel.HasWeixin)
                {
                    app.RegisterWeiXin();
                }
            }

            app.UseHttpsRedirection();
            
            app.UseCors("AllowAngularApp");
            app.UseAuthorization();
            app.UseHealthCheckMiddleware();//注册Consul
            //app.UseConsulCheckService();
            app.MapControllers();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            // 使用静态文件中间件
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            if (appModel.HasAddHub)//注册Hub
            {
                app.AddHub();
            }
            app.UseStaticFiles();
            app.UseRouting();

            if (appModel.HasMQ)
            {
                app.AddRabbitMQEventBus();
            }
            ServiceLocator.Instance = app.Services;

            app.UseMiddleware<TransactionEncryptMiddleware>();
            return app;
        }
        /// <summary>
        /// 注册Hub
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder AddHub(this WebApplication app)
        {
            app.MapHub<Services.MessageService>("/message");
            return app;
        }
        /// <summary>
        /// 注入数据库服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddSqlConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSqlSugar(new IocConfig()
            {
                ConfigId =CommonHandle.DatabaseNames.Main,
                ConnectionString = configuration.GetSection($"Database:{CommonHandle.DatabaseNames.Main}").Value,
                DbType = IocDbType.SqlServer,
                IsAutoCloseConnection = true//自动释放
            });
            var shopConfig = configuration.GetSection($"Database:{CommonHandle.DatabaseNames.Shop}");
            if (shopConfig != null)
            {
                services.AddSqlSugar(new IocConfig()
                {
                    ConfigId = CommonHandle.DatabaseNames.Shop,
                    ConnectionString = shopConfig.Value,
                    DbType = IocDbType.SqlServer,
                    IsAutoCloseConnection = true//自动释放
                });
            }
            var agentConfig = configuration.GetSection($"Database:{CommonHandle.DatabaseNames.Agent}");
            if (agentConfig != null)
            {
                services.AddSqlSugar(new IocConfig()
                {
                    ConfigId = CommonHandle.DatabaseNames.Agent,
                    ConnectionString = agentConfig.Value,
                    DbType = IocDbType.SqlServer,
                    IsAutoCloseConnection = true//自动释放
                });
            }
            var financeConfig = configuration.GetSection($"Database:{CommonHandle.DatabaseNames.Finance}");
            if (financeConfig != null)
            {
                services.AddSqlSugar(new IocConfig()
                {
                    ConfigId = CommonHandle.DatabaseNames.Finance,
                    ConnectionString = financeConfig.Value,
                    DbType = IocDbType.SqlServer,
                    IsAutoCloseConnection = true//自动释放
                });
            }
            //设置参数
            services.ConfigurationSugar(db =>
            {
                //db.Aop.OnLogExecuting = (sql, p) =>
                //{
                //    Common.StaticLog.Debug(sql);
                //    Console.WriteLine(sql);
                //};
                //从库
                //var laveConnectionConfigs = new List<SlaveConnectionConfig>() {
                //     new SlaveConnectionConfig() { HitRate=10, ConnectionString=configuration.GetSection("Database:Main").Value }
                //};
            });
            return services;
        }
        /// <summary>
        /// 根据不同的接口，注册不同生命周期的接口服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <param name="lifetime"></param>
        /// <returns></returns>
        public static IServiceCollection AddServices<T>(this IServiceCollection services, ServiceLifetime lifetime)
        {
            var types = TypeFinders.SearchTypes(typeof(T), TypeFinders.TypeClassification.Interface);

            foreach (var type in types)
            {
                var interfaces = type.GetInterfaces().Where(t => !t.IsGenericType && t != typeof(T));

                foreach (var interfaceType in interfaces)
                {
                    services.AddService(interfaceType, type, lifetime);
                }
            }

            return services;
        }
        private static IServiceCollection AddService(this IServiceCollection services, Type interfaceType, Type implementationType, ServiceLifetime lifetime)
        {
            services.Add(new ServiceDescriptor(implementationType, implementationType, lifetime));
            services.Add(new ServiceDescriptor(interfaceType, implementationType, lifetime));

            return services;
        }
    }
}
