using LZ.Model.Enums;
using LZ.Model;
using LZ.Core.Interface;
using LZ.Core.Redis;
using Microsoft.AspNetCore.Http;
using LZ.Common;
using SqlSugar.IOC;
using LZ.Core.Attributes;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text;

namespace LZ.Core.Services
{
    public class SettingsService(IRedisService redisService) : ServicesBase<SYS_SiteSettings>,ISettingsService, IScopedDependency
    {

        public async Task Save(Model.SiteSettings settings)
        {
            await SaveMasterSettings(settings);
            await redisService.DeleteAsync("MasterSettings");
        }
        public async Task<Model.SiteSettings> GetMasterSettings()
        {
            var siteSettings = await redisService.GetAsync<SiteSettings>("MasterSettings");
            if (siteSettings == null)
            {
                siteSettings = ConvertDictionaryToEntity<Model.SiteSettings>(await GetDataSiteSettings());
                await redisService.SetAsync("MasterSettings", siteSettings, TimeSpan.FromHours(10));
            }
            return siteSettings;
        }
        private async Task SaveMasterSettings(SiteSettings settings)
        {
            var items = ConvertEntityToObjDictionary((object)settings);
            var oldItems = await GetDataSiteSettings();
            StringBuilder stringBuilder = new StringBuilder();
            bool isExist = true;
            foreach (string key in items.Keys)
            {
                if (this.IsNeedUpdate(key, items[key].ToString(), oldItems, out isExist))
                {
                    if (isExist)
                        stringBuilder.AppendLine("UPDATE SYS_SiteSettings SET [Value] = '" + items[key].ToNullString() + "' WHERE [Key] = '" + key.ToNullString() + "';");
                    else
                        stringBuilder.AppendLine("INSERT INTO SYS_SiteSettings ([Key],[Value]) Values('" + key.ToNullString() + "','" + items[key].ToNullString() + "');");
                }
            }
            if (stringBuilder == null || string.IsNullOrEmpty(stringBuilder.ToNullString()))
                return;
            await base.Context.Ado.ExecuteCommandAsync(stringBuilder.ToString());
        }
        private T ConvertDictionaryToEntity<T>(IDictionary<string, object> dict)
        {
            T instance = Activator.CreateInstance<T>();
            PropertyInfo[] properties = instance.GetType().GetProperties();
            if (properties.Length != 0 && dict.Count > 0)
            {
                foreach (PropertyInfo propertyInfo in properties)
                {
                    propertyInfo.GetGetMethod();
                    if (dict.ContainsKey(propertyInfo.Name))
                    {
                        string name = propertyInfo.Name;
                        string nullString = dict[propertyInfo.Name].ToNullString();
                        try
                        {
                            if (propertyInfo.CanWrite)
                                propertyInfo.SetValue((object)instance, string.IsNullOrEmpty(nullString) ? (propertyInfo.PropertyType == typeof(string) ? (object)"" : DefaultForType(propertyInfo.PropertyType)) : Convert.ChangeType((object)nullString, propertyInfo.PropertyType), (object[])null);
                        }
                        catch (Exception ex)
                        {
                            Common.StaticLog.Error("", ex);
                        }
                    }
                }
            }
            return instance;
        }
        private object DefaultForType(Type targetType)
        {
            if (!targetType.IsValueType)
                return (object)null;
            return Activator.CreateInstance(targetType);
        }
        private async Task<IDictionary<string, object>> GetDataSiteSettings()
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            using (IDataReader dataReader = await base.Context.Ado.GetDataReaderAsync("SELECT [Key],Value FROM SYS_SiteSettings"))
            {
                while (dataReader.Read())
                    dictionary.Add(dataReader.GetString(0), dataReader[1].ToNullString());
            }
            return dictionary;
        }
        private IDictionary<string, object> ConvertEntityToObjDictionary(object obj)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            foreach (PropertyInfo property in obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                MethodInfo getMethod = property.GetGetMethod();
                if (getMethod != (MethodInfo)null && getMethod.IsPublic && property.CanRead)
                    dictionary.Add(property.Name, getMethod.Invoke(obj, new object[0]));
            }
            return (IDictionary<string, object>)dictionary;
        }

        private bool IsNeedUpdate(string key, string value, IDictionary<string, object> oldItems, out bool isExist)
        {
            isExist = true;
            if (oldItems.ContainsKey(key))
            {
                string nullString = oldItems[key].ToNullString();
                return !(value == nullString);
            }
            isExist = false;
            return true;
        }
    }
}
