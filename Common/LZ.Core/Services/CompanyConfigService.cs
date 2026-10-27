using LZ.Model.Enums;
using LZ.Model;
using LZ.Core.Interface;
using LZ.Core.Redis;
using Microsoft.AspNetCore.Http;
using LZ.Common;
using SqlSugar.IOC;
using LZ.Core.Attributes;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using System.Text.Json.Serialization;
using LZ.Model.Exceptions;

namespace LZ.Core.Services
{
    public class CompanyConfigService(IRedisService redisService) : ServicesBase<SYS_Company>,ICompanyConfigService, IScopedDependency
    {

        /// <summary>
        /// 获取公司信息及配置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Model.CompanyModel> Get(Guid? id)
        {
            if (id==null)
            {
                throw new ErrorCodeException(-1, "公司ID错误！");
            }
            var companyObj = await redisService.GetAsync<Model.CompanyModel>(string.Format(CoreRedisConstants.CompanyConfig, id));
            if (companyObj == null)
            {
                var  model = await base.Context.Queryable<SYS_Company>().FirstAsync(it => it.CompanyGID == id);
                if (model == null)
                {
                    throw new ErrorCodeException(-1, "公司信息不存在！");
                }
                companyObj = new Model.CompanyModel();
                companyObj.Company = model;
                if (!string.IsNullOrEmpty(model.Config))
                {
                    companyObj.Config= JsonSerializer.Deserialize<Model.CompanyConfig>(model.Config, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                }
                else
                {
                    companyObj.Config = new Model.CompanyConfig();
                }
                if (companyObj.Config.EnableDockJDY && !string.IsNullOrEmpty(model.JDYForms))
                {
                    try
                    {
                        companyObj.JDYForms = JsonSerializer.Deserialize<List<Model.JDYFormModel>>(model.JDYForms, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                    }
                   catch(Exception ex)
                    {
                        Common.StaticLog.Error(ex);
                    }
                }
                await redisService.SetAsync(string.Format(CoreRedisConstants.CompanyConfig, id), companyObj, TimeSpan.FromHours(10));
            }
            return companyObj;
        }
    }
}
