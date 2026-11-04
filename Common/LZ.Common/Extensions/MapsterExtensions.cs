
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Common.Extensions
{
    public static class MapsterExtensions
    {
        /// <summary>
        /// 注册并配置 Mapster 映射规则
        /// </summary>
        public static IServiceCollection AddMapsterConfiguration(this IServiceCollection services)
        {
            //var config = TypeAdapterConfig.GlobalSettings;

            //// 1. 全局默认规范配置
            //config.Default.PreserveReference(true); // 循环引用保护（防止复杂对象嵌套时无限递归）
            //config.Default.IgnoreNullValues(false); // 映射时是否忽略源对象的 null 值（根据业务需要调整，false表示覆盖）

            //// 2. 自动扫描当前程序集或所有以 LZ 开头的程序集中的 IRegister 映射规则
            //// 这样你以后写自定义映射（比如 A -> B 怎么对齐）时，只需要实现 IRegister 接口，Mapster 会自动加载
            //var assemblies = Directory.GetFiles(AppContext.BaseDirectory, "LZ.*.dll")
            //    .Select(Assembly.LoadFrom)
            //    .ToArray();

            //config.Scan(assemblies);

            //// 3. 将 Mapster 映射服务注入到 .NET 8 DI 容器中
            //services.AddSingleton(config);
            //services.AddScoped<MapsterMapper.IMapper, MapsterMapper.ServiceMapper>();

            return services;
        }
    }
}
