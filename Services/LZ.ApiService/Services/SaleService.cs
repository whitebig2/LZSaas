using LZ.ApiService.Interface;
using LZ.Core.Interface;
using LZ.Core.Services;
using LZ.Model.Exceptions;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LZ.Model;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;

namespace LZ.ApiService.Services
{
    public class SaleService(IHttpService httpService) : ISaleService, IScopedDependency
    {
        private string token = "8f05270e-0294-4382-9354-45553ea8ab6b";
        //private string path = "http://localhost:8857";
        private string path = "https://sale.cn-labor.cn";

        public async Task<int> GetCount(Dictionary<string, string> data, string url)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", token);
            var result = await httpService.GetAsync(path + url, data, headers);
            try
            {
                int count = Convert.ToInt32(result);
                return count;
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        public async Task<List<Model.SaleManageUser>?> GetManageUserList(string json, string url)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", token);
            var result = await httpService.PostJsonAsync(path + url, json, headers);
            var obj = JsonConvert.DeserializeObject(result) as JObject;
            try
            {
                var list = JsonConvert.DeserializeObject<List<Model.SaleManageUser>>(obj["data"].ToString());
                return list;
            }
            catch (Exception e)
            {
                throw new ErrorCodeException(-1, "获取业务合伙人列表失败");
            }
        }

        public async Task<List<Model.CustomerCountByCityPartnerModel>?> GetCustomerCountList(Dictionary<string, string> data, string url)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", token);
            var result = await httpService.GetAsync(path + url, data, headers);
            //var obj = JsonConvert.DeserializeObject(result) as JObject;
            try
            {
                var list = JsonConvert.DeserializeObject<List<Model.CustomerCountByCityPartnerModel>?>(result);
                return list;
            }
            catch (Exception e)
            {
                throw new ErrorCodeException(-1, "获取客户单位数量失败");
            }
        }

        public async Task<SaleMessageResult> AuditManageUser(string json, string url)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", token);
            var result = await httpService.PostJsonAsync(path + url, json, headers);
            //var obj = JsonConvert.DeserializeObject(result) as JObject;
            try
            {
                var message = JsonConvert.DeserializeObject<SaleMessageResult>(result);
                return message;
            }
            catch (Exception e)
            {
                throw new ErrorCodeException(-1, "接口调用失败");
            }
        }

        #region 城市合伙人
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="url"></param>
        /// <param name="type">1为合伙人账单，2个账单客户列表详情</param>
        /// <returns></returns>
        public async Task<SaleCityPartnerResult> CityPartnerBillList(Dictionary<string, string> dic, string url,int type)
        {
            string ur = path + url;
            var result = await httpService.GetAsync(path + url, dic);
            var obj = JsonConvert.DeserializeObject(result) as JObject;
            try
            {
                SaleCityPartnerResult data = new SaleCityPartnerResult
                {
                    Code = Convert.ToInt32(obj["code"].ToString()),
                    Msg = obj["msg"].ToString()
                };
                if(obj["code"].ToString() == "0")
                {
                    if(type == 1)
                    {
                        data.Bills = JsonConvert.DeserializeObject<List<Model.CityPartnerBill>>(obj["data"].ToString());
                    }
                    else if(type == 2)
                    {
                        data.Records = JsonConvert.DeserializeObject<List<Model.CityPartnerBillRecord>>(obj["data"].ToString());
                    }
                }
                else
                {
                    
                }
                return data;
            }
            catch(Exception e)
            {
                SaleCityPartnerResult data = new SaleCityPartnerResult
                {
                    Code = 10000,
                    Msg = result
                };

                return data;
            }

        }

        #endregion

    }
}
