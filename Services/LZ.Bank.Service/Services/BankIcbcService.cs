using LZ.Bank.Service.Interface;
using LZ.Core.Interface;
using LZ.Model.ICBC.Plkh;
using LZ.Model.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using NPOI.HPSF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Bank.Service.Services
{
    public class BankIcbcService : IBankIcbcService,IScopedDependency
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IcbcOpenCardOption _openCardOption;

        public BankIcbcService(IHttpClientFactory httpClientFactory,IOptions<IcbcOpenCardOption> options)
        {
            _httpClientFactory = httpClientFactory;
            _openCardOption= options.Value;
        }

        public async Task<PlkhResult> OpenAccountApplyAsync(string filePath, string fileName)
        {
            try
            {
                var gid = Guid.NewGuid().ToString();
                var client = _httpClientFactory.CreateClient("ICBC");
                PlkhApply apply = new PlkhApply()
                {
                    AppCode = _openCardOption.AppCode,
                    Filename = fileName,
                    Id = gid,
                    Path = filePath,
                    Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(),
                };
                var serializerSettings = new JsonSerializerSettings
                {
                    // 设置为驼峰命名
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(apply, Formatting.None, serializerSettings);
                
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                var resp = await client.PostAsync("/icbc/dcdf/plkh", content);

                if (resp.IsSuccessStatusCode)
                {
                   return await resp.Content.ReadFromJsonAsync<PlkhResult>();
                }
                else {
                    return new PlkhResult()
                    {
                        Id = gid,
                        Retcode = "9999",
                        Retmsg = $"请求码获取异常，请求码：{resp.StatusCode}"
                    };
                }
            }
            catch (Exception ex)
            {
                return new PlkhResult()
                {
                    Id = string.Empty,
                    Retcode = "9999",
                    Retmsg = $"请求异常:{ex.Message}"
                };
            }
        }

        public async Task<PlkhResult> OpenAccountUploadImageApplyAsync(string filePath, string fileName, DateTime kkrq)
        {
         
            try
            {
                var gid = Guid.NewGuid().ToString();
                var client = _httpClientFactory.CreateClient("ICBC");
                PlkhUpladImagesApply apply = new PlkhUpladImagesApply()
                {
                    AppCode = _openCardOption.AppCode,
                    Filename = fileName,
                    Id = gid,
                    Path = filePath,
                    Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(),
                    Kkrq = kkrq.ToString("yyyyMMdd"),
                };
                var serializerSettings = new JsonSerializerSettings
                {
                    // 设置为驼峰命名
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(apply, Formatting.None, serializerSettings);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                var resp = await client.PostAsync("/icbc/dcdf/plkhimage", content);

                if (resp.IsSuccessStatusCode)
                {
                    return await resp.Content.ReadFromJsonAsync<PlkhResult>();
                }
                else
                {
                    return new PlkhResult()
                    {
                        Id = gid,
                        Retcode = "9999",
                        Retmsg = $"请求码获取异常，请求码：{resp.StatusCode}"
                    };
                }
            }
            catch (Exception ex)
            {
                return new PlkhResult()
                {
                    Id = string.Empty,
                    Retcode = "9999",
                    Retmsg = $"请求异常:{ex.Message}"
                };
            }
        }

        public async Task<PlkhResult> OpenAccountQueryDetailAsync(string name, string cardID, DateTime kkrq)
        {
            
            try
            {
                var gid = Guid.NewGuid().ToString();
                var client = _httpClientFactory.CreateClient("ICBC");
                PlkhQueryDetailApply apply = new PlkhQueryDetailApply()
                {
                    AppCode = _openCardOption.AppCode,
                    Id = gid,
                    Lgldocno = cardID,
                    Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(),
                    Name = name,
                    Senddate = kkrq.ToString("yyyymMMdd")
                };

                var serializerSettings = new JsonSerializerSettings
                {
                    // 设置为驼峰命名
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(apply, Formatting.None, serializerSettings);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                var resp = await client.PostAsync("/icbc/dcdf/search", content);

                if (resp.IsSuccessStatusCode)
                {
                    return await resp.Content.ReadFromJsonAsync<PlkhResult>();
                }
                else
                {
                    return new PlkhResult()
                    {
                        Id = gid,
                        Retcode = "9999",
                        Retmsg = $"请求码获取异常，请求码：{resp.StatusCode}"
                    };
                }
            }
            catch (Exception ex)
            {
                return new PlkhResult()
                {
                    Id = string.Empty,
                    Retcode = "9999",
                    Retmsg = $"请求异常:{ex.Message}"
                };
            }
        }
    }
}
