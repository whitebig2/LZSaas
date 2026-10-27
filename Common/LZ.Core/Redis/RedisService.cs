using LZ.Core.Services;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSRedis;
using LZ.Core.Interface;
using LZ.Model.PagedModel;
using Senparc.Weixin.WxOpen.AdvancedAPIs.DataCube;
using NPOI.SS.Formula.Functions;

namespace LZ.Core.Redis
{
    public class RedisService: IRedisService,IScopedDependency
    {
        public async Task<long> DeleteAsync(string key)
        {
            return await RedisHelper.DelAsync(key);
        }

        public async Task<T> GetAsync<T>(string key)
        {
            return await RedisHelper.GetAsync<T>(key);
        }

        public async Task<bool> PingAsync()
        {
            return await RedisHelper.PingAsync();
        }
        public async Task IncrByAsync(string key, int value=1)
        {
            await RedisHelper.IncrByAsync(key, value);
        }

        public async Task<dynamic> ScanAsync(PagedModelBase model)
        {
            List<string> list = new List<string>();

            //根沐model.Keyword进行模糊匹配
            var scanResult = await RedisHelper.ScanAsync(model.Page, $"*{model.Keyword}*", model.PageSize);
            list.AddRange(scanResult.Items);

            var values = await RedisHelper.MGetAsync(list.ToArray());

            var resultDictionary = list.Zip(values, (key, value) => new { key, value })
                                            .ToDictionary(item => item.key, item => item.value);
            dynamic result = new ExpandoObject();
            result.Items = resultDictionary;
            result.Cursor = scanResult.Cursor;  // 下一次要通过这个Cursor获取下一页的keys
            return result;
        }

        public async Task<bool> SetAsync(string key, object value)
        {
            return await RedisHelper.SetAsync(key, value);
        }

        public async Task<bool> SetAsync(string key, object value, TimeSpan expire, RedisExistence? exists = null)
        {
            return await RedisHelper.SetAsync(key, value, expire, exists);
        }

        public async Task<bool> SetAsync(string key, object value, int expireSeconds = -1, RedisExistence? exists = null)
        {
            return await RedisHelper.SetAsync(key, value, expireSeconds, exists);
        }
    }
}
