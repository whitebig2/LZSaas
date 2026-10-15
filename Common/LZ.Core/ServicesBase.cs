using LZ.Core.CommonHandle;
using SqlSugar;
using SqlSugar.IOC;
using System.Reflection;

namespace LZ.Core
{
    public class ServicesBase<T> : SimpleClient<T> where T : class, new()
    {
        protected ITenant itenant = null;//事务
        public ServicesBase(ISqlSugarClient context = null) : base(context)
        {
            var configId = typeof(T).GetCustomAttribute<TenantAttribute>().configId;
            base.Context = DbScoped.SugarScope.GetConnection(configId);
            itenant = DbScoped.SugarScope;
        }

    }
}
