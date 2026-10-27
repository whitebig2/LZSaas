using LZ.Core;
using LZ.Core.Interface;
using LZ.Model;
using SqlSugar;
using SqlSugar.IOC;
using System.Linq.Expressions;
using System.Text;
using LZ.Model.PagedModel;
using LZ.Core.Services;
using Consul;
using LZ.Common;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using LZ.Finance.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using LZ.Model.ModelExtensions;
using Microsoft.Extensions.Options;
using LZ.Model.Options;
using NPOI.OpenXmlFormats.Dml.ChartDrawing;
using System.Net.Http.Headers;
using LZ.Model.Response;
using LZ.Model.Response.Xfz;
using System.Web;

namespace LZ.Finance.Service.Services
{
    public class FinanceOtherService : ServicesBase<Live_Partner>, IFinanceOtherService, IScopedDependency
    {


       

        private readonly AesOption  _aesOption;
        private readonly IHttpClientFactory  _httpClientFactory;
        private readonly IHttpService  _httpService;

        public FinanceOtherService(IHttpClientFactory httpClientFactory, IOptions<AesOption> aseOption, IHttpService httpService)
        {
            _httpClientFactory = httpClientFactory;
            _aesOption = aseOption.Value;
            _httpService = httpService;
        }


        public record ListPartnersRecord(List<Live_Partner> items, int count);
        public async Task<ApiRequestResultModel<List<Live_Partner>>> ListPartners(Guid cityPartnerID)
        {
            Dictionary<string, string> kvs = new Dictionary<string, string>();
            kvs.Add("cityPartnerID", cityPartnerID.ToString());
            var queryParams = string.Join("&", kvs.Select(kvp =>
            $"{HttpUtility.UrlEncode(kvp.Key)}={HttpUtility.UrlEncode(kvp.Value)}"));


            var httpClient = _httpClientFactory.CreateClient(ServiceNames.BasicService);



            var resp = await httpClient.GetAsync($"/api/Partner/ListByCityPartnerID?{queryParams}");
            if (resp.IsSuccessStatusCode)
            {
                var json = await resp.Content.ReadAsStringAsync();
                Console.WriteLine(json);
                var respContent = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiRequestResultModel<ListPartnersRecord>>(json);
                if (respContent.Data == null || respContent.Data.items == null || respContent.Data.items.Count <= 0)
                {
                    return new ApiRequestResultModel<List<Live_Partner>>()
                    {
                        Code = 500,
                        Data = null,
                        Message = $"未获取到符合条件的服务商信息",
                    };
                }
                return new ApiRequestResultModel<List<Live_Partner>>()
                {
                    Code = 0,
                    Data = respContent.Data.items,
                    Message = $"成功",
                };
            }
            else
            {
                return new ApiRequestResultModel<List<Live_Partner>>()
                {
                    Code = 500,
                    Data = null,
                    Message = $"获取服务商信息失败",
                };
            }
        }



        public async Task<ApiRequestResultModel<List<Live_Partner>>> ListPartners(Guid cityPartnerID,int partnerType)
        {
            Dictionary<string, string> kvs = new Dictionary<string, string>();
            kvs.Add("cityPartnerID", cityPartnerID.ToString());
            kvs.Add("partnerType", partnerType.ToString());
            var queryParams = string.Join("&", kvs.Select(kvp =>
            $"{HttpUtility.UrlEncode(kvp.Key)}={HttpUtility.UrlEncode(kvp.Value)}"));


            var httpClient = _httpClientFactory.CreateClient(ServiceNames.BasicService);



            var resp = await httpClient.GetAsync($"/api/Partner/ListByCityPartnerID?{queryParams}");
            if (resp.IsSuccessStatusCode)
            {
                var json = await resp.Content.ReadAsStringAsync();
                Console.WriteLine(json);
                var respContent = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiRequestResultModel<ListPartnersRecord>>(json);
                if (respContent.Data == null || respContent.Data.items == null || respContent.Data.items.Count <= 0)
                {
                    return new ApiRequestResultModel<List<Live_Partner>>()
                    {
                        Code = 500,
                        Data = null,
                        Message = $"未获取到符合条件的服务商信息",
                    };
                }
                return new ApiRequestResultModel<List<Live_Partner>>()
                {
                    Code = 0,
                    Data = respContent.Data.items,
                    Message = $"成功",
                };
            }
            else
            {
                return new ApiRequestResultModel<List<Live_Partner>>()
                {
                    Code = 500,
                    Data = null,
                    Message = $"获取服务商信息失败",
                };
            }
        }


        public async Task<ListPartnersRecord> ListPartner(Guid cityPartnerID, int? partnerType, bool? isBusinessPartner)
        {
            Dictionary<string, string> kvs = new Dictionary<string, string>();
            kvs.Add("cityPartnerID", cityPartnerID.ToString());

            if (partnerType.HasValue)
            {
                kvs.Add("partnerType", partnerType.Value.ToString());
            }
            if (isBusinessPartner.HasValue)
            {
                kvs.Add("isBusinessPartner", isBusinessPartner.Value.ToString());
            }


            var queryParams = string.Join("&", kvs.Select(kvp =>
        $"{HttpUtility.UrlEncode(kvp.Key)}={HttpUtility.UrlEncode(kvp.Value)}"));
            var resp = await _httpService.GetAsync<ListPartnersRecord>(ServiceNames.BasicService, $"/api/Partner/ListByCityPartnerID?{queryParams}");
            return resp;
        }

        public record ListCityPartnerRecord(List<Live_CityPartner> items);
        public async Task<ApiRequestResultModel<List<Live_CityPartner>>> ListCityPartners(string title, bool? enabled)
        {
            Dictionary<string, string> kvs = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(title))
            {
                kvs.Add("title", title);
            }
            if (enabled.HasValue)
            {
                kvs.Add("enabled", enabled.Value.ToString());
            }


            var queryParams = string.Join("&", kvs.Select(kvp =>
            $"{HttpUtility.UrlEncode(kvp.Key)}={HttpUtility.UrlEncode(kvp.Value)}"));


            var httpClient = _httpClientFactory.CreateClient(ServiceNames.BasicService);



            var resp = await httpClient.GetAsync($"/api/CityPartner/List?{queryParams}");
            if (resp.IsSuccessStatusCode)
            {
                var json = await resp.Content.ReadAsStringAsync();
                var respContent = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiRequestResultModel<ListCityPartnerRecord>>(json);
                if (respContent.Data == null || respContent.Data.items == null || respContent.Data.items.Count <= 0)
                {
                    return new ApiRequestResultModel<List<Live_CityPartner>>()
                    {
                        Code = 500,
                        Data = null,
                        Message = $"未获取到符合条件的城市合伙人信息",
                    };
                }
                return new ApiRequestResultModel<List<Live_CityPartner>>()
                {
                    Code = 0,
                    Data = respContent.Data.items,
                    Message = $"成功",
                };
            }
            else
            {
                return new ApiRequestResultModel<List<Live_CityPartner>>()
                {
                    Code = 500,
                    Data = null,
                    Message = $"获取城市合伙人信息失败",
                };
            }
        }


        public record ListCustomerRecord(List<Live_V_Customer> items,int count);
        public async Task<ApiRequestResultModel<List<Live_V_Customer>>> ListCustomers(string name,string cityPartnerTitle,string salesman, bool? enabled)
        {
            Dictionary<string, string> kvs = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(name))
            {
                kvs.Add("Name", name);
            }
            if (!string.IsNullOrWhiteSpace(cityPartnerTitle))
            {
                kvs.Add("cityPartnerTitle", cityPartnerTitle);
            }
            if (!string.IsNullOrWhiteSpace(salesman))
            {
                kvs.Add("salesman", salesman);
            }
            if (enabled.HasValue)
            {
                kvs.Add("enabled", enabled.Value.ToString());
            }
            kvs.Add("page", "0");



            var queryParams = string.Join("&", kvs.Select(kvp =>
            $"{HttpUtility.UrlEncode(kvp.Key)}={HttpUtility.UrlEncode(kvp.Value)}"));


            var httpClient = _httpClientFactory.CreateClient(ServiceNames.CustomerService);



            var resp = await httpClient.GetAsync($"/api/Customer/List?{queryParams}");
            if (resp.IsSuccessStatusCode)
            {
                var json = await resp.Content.ReadAsStringAsync();
                var respContent = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiRequestResultModel<ListCustomerRecord>>(json);
                if (respContent.Data == null || respContent.Data.items == null || respContent.Data.items.Count <= 0)
                {
                    return new ApiRequestResultModel<List<Live_V_Customer>>()
                    {
                        Code = 500,
                        Data = null,
                        Message = $"未获取到符合条件的客户单位信息",
                    };
                }
                return new ApiRequestResultModel<List<Live_V_Customer>>()
                {
                    Code = 0,
                    Data = respContent.Data.items,
                    Message = $"成功",
                };
            }
            else
            {
                return new ApiRequestResultModel<List<Live_V_Customer>>()
                {
                    Code = 500,
                    Data = null,
                    Message = $"获取客户单位信息失败",
                };
            }
        }

        public async Task<ApiRequestResultModel<List<Live_V_Customer>>> ListCustomersByCompanyID(int companyID)
        {
            Dictionary<string, string> kvs = new Dictionary<string, string>();
        
            kvs.Add("companyID", companyID.ToString());



            var queryParams = string.Join("&", kvs.Select(kvp =>
            $"{HttpUtility.UrlEncode(kvp.Key)}={HttpUtility.UrlEncode(kvp.Value)}"));


            var httpClient = _httpClientFactory.CreateClient(ServiceNames.CustomerService);



            var resp = await httpClient.GetAsync($"/api/Customer/ListByCompanyID?{queryParams}");
            if (resp.IsSuccessStatusCode)
            {
                var json = await resp.Content.ReadAsStringAsync();
                var respContent = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiRequestResultModel<ListCustomerRecord>>(json);
                if (respContent.Data == null || respContent.Data.items == null || respContent.Data.items.Count <= 0)
                {
                    return new ApiRequestResultModel<List<Live_V_Customer>>()
                    {
                        Code = 500,
                        Data = null,
                        Message = $"未获取到符合条件的客户单位信息",
                    };
                }
                return new ApiRequestResultModel<List<Live_V_Customer>>()
                {
                    Code = 0,
                    Data = respContent.Data.items,
                    Message = $"成功",
                };
            }
            else
            {
                return new ApiRequestResultModel<List<Live_V_Customer>>()
                {
                    Code = 500,
                    Data = null,
                    Message = $"获取客户单位信息失败",
                };
            }
        }
        public async Task<ApiRequestResultModel<ListCustomerRecord>> ListCustomerPage(string name, string cityPartnerTitle, bool? enabled,int? page,int? pageSize)
        {
            Dictionary<string, string> kvs = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(name))
            {
                kvs.Add("Name", name);
            }
            if (!string.IsNullOrWhiteSpace(cityPartnerTitle))
            {
                kvs.Add("cityPartnerTitle", cityPartnerTitle);
            }
          
            if (enabled.HasValue)
            {
                kvs.Add("enabled", enabled.Value.ToString());
            }

            if (page.HasValue)
            {
                kvs.Add("page", page.ToString());
            }
            if (pageSize.HasValue)
            {
                kvs.Add("pageSize", pageSize.ToString());
            }

     



            var resp = await _httpService.GetAsync<ListCustomerRecord>(ServiceNames.CustomerService,$"/api/Customer/List",kvs);


            if (resp != null)
            {
                return new ApiRequestResultModel<ListCustomerRecord>()
                {
                    Code = 0,
                    Data = resp,
                    Message = $"成功",
                };
            }
            else {
                return new ApiRequestResultModel<ListCustomerRecord>()
                {
                    Code = 500,
                    Data = null,
                    Message = $"未获取到符合条件的客户单位信息",
                };
            }
     
        }

        public record ListCompanyRecord(List<SYS_Company> items);
        public async Task<ApiRequestResultModel<List<SYS_Company>>> ListCompany(Guid cityParterID)
        {
            Dictionary<string, string> kvs = new Dictionary<string, string>();

            kvs.Add("cityPartnerID", cityParterID.ToString());
            kvs.Add("page", "0");



            var queryParams = string.Join("&", kvs.Select(kvp =>
            $"{HttpUtility.UrlEncode(kvp.Key)}={HttpUtility.UrlEncode(kvp.Value)}"));
            var httpClient = _httpClientFactory.CreateClient(ServiceNames.CompanyService);
            var resp = await httpClient.GetAsync($"/api/Company/List?{queryParams}");
            if (resp.IsSuccessStatusCode)
            {
                var json = await resp.Content.ReadAsStringAsync();
                var respContent = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiRequestResultModel<ListCompanyRecord>>(json);
                if (respContent.Data == null || respContent.Data.items == null || respContent.Data.items.Count <= 0)
                {
                    return new ApiRequestResultModel<List<SYS_Company>>()
                    {
                        Code = 500,
                        Data = null,
                        Message = $"未获取到符合条件的客户单位信息",
                    };
                }
                return new ApiRequestResultModel<List<SYS_Company>>()
                {
                    Code = 0,
                    Data = respContent.Data.items,
                    Message = $"成功",
                };
            }
            else
            {
                return new ApiRequestResultModel<List<SYS_Company>>()
                {
                    Code = 500,
                    Data = null,
                    Message = $"获取客户单位信息失败",
                };
            }
        }

        public async Task<ApiRequestResultModel<Live_V_Customer>> GetCustomerByNameAsync(string name)
        {
            var httpClient = _httpClientFactory.CreateClient(ServiceNames.CustomerService);
            var resp = await httpClient.GetAsync($"/api/Customer/GetByName?name={name}");
            if (resp.IsSuccessStatusCode)
            {
                var json = await resp.Content.ReadAsStringAsync();
                var respContent = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiRequestResultModel<Live_V_Customer>>(json);
                return respContent;
            }
            else
            {
                return new ApiRequestResultModel<Live_V_Customer>()
                {
                    Code = 500,
                    Data = null,
                    Message = "发生异常",
                };
            }
        }

        public async Task<ApiRequestResultModel<Live_V_Customer>> GetCustomerByGIDAsync(Guid customerGID)
        {
            var httpClient = _httpClientFactory.CreateClient(ServiceNames.CustomerService);
            var resp = await httpClient.GetAsync($"/api/Customer/GetByGID?customerGID={customerGID}");
            if (resp.IsSuccessStatusCode)
            {
                var json = await resp.Content.ReadAsStringAsync();
                var respContent = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiRequestResultModel<Live_V_Customer>>(json);
                return respContent;
            }
            else
            {
                return new ApiRequestResultModel<Live_V_Customer>()
                {
                    Code = 500,
                    Data = null,
                    Message = "发生异常",
                };
            }
        }
        

        public async Task<ApiRequestResultModel<Live_Customer>> GetCustomerByID(int id)
        {
            var httpClient = _httpClientFactory.CreateClient(ServiceNames.CustomerService);
            var resp = await httpClient.GetAsync($"/api/Customer/get?id={id}");
            if (resp.IsSuccessStatusCode)
            {
                var json = await resp.Content.ReadAsStringAsync();
                var respContent = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiRequestResultModel<Live_Customer>>(json);
                return respContent;
            }
            else
            {
                return new ApiRequestResultModel<Live_Customer>()
                {
                    Code = 500,
                    Data = null,
                    Message = "发生异常",
                };
            }
        }

        public async Task<ApiRequestResultModel<Live_Enterprise>> GetEnterpriseByNameAsync(string name)
        {
            var httpClient = _httpClientFactory.CreateClient(ServiceNames.CustomerService);
            var resp = await httpClient.GetAsync($"/api/Enterprise/GetByName?name={name}");
            if (resp.IsSuccessStatusCode)
            {
                var json = await resp.Content.ReadAsStringAsync();
                var respContent = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiRequestResultModel<Live_Enterprise>>(json);
                return respContent;
            }
            else
            {
                return new ApiRequestResultModel<Live_Enterprise>()
                {
                    Code = 500,
                    Data = null,
                    Message = "发生异常",
                };
            }
        }

        public async Task<ApiRequestResultModel<Live_Enterprise>> GetEnterpriseByIdAsync(Guid id)
        {
            var httpClient = _httpClientFactory.CreateClient(ServiceNames.CustomerService);
            var resp = await httpClient.GetAsync($"/api/Enterprise/Get?id={id.ToString()}");
            if (resp.IsSuccessStatusCode)
            {
                var json = await resp.Content.ReadAsStringAsync();
                var respContent = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiRequestResultModel<Live_Enterprise>>(json);
                return respContent;
            }
            else
            {
                return new ApiRequestResultModel<Live_Enterprise>()
                {

                    Code = 500,
                    Data = null,
                    Message = "发生异常",
                };
            }
        }

        public async Task<ApiRequestResultModel<List<SYS_Dictionary>>> GetDeductionHouseRentCityListAsync(string name)
        {
            var httpClient = _httpClientFactory.CreateClient(ServiceNames.BasicService);
            var resp = await httpClient.GetAsync($"/api/Dictionary/GetChildrenByName?name={name}");
            if (resp.IsSuccessStatusCode)
            {
                var json = await resp.Content.ReadAsStringAsync();
                var respContent = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiRequestResultModel<List<SYS_Dictionary>>>(json);
                return respContent;
            }
            else
            {
                return new ApiRequestResultModel<List<SYS_Dictionary>>()
                {

                    Code = 500,
                    Data = null,
                    Message = "发生异常",
                };
            }



        }


        public async Task<ApiRequestResultModel<List<Live_WhiteList>>> GeCashWhiteAllListAsync()
        {
            var httpClient = _httpClientFactory.CreateClient(ServiceNames.UsersService);
            var resp = await httpClient.GetAsync($"/api/WhiteList/GetAll");
            if (resp.IsSuccessStatusCode)
            {
                var json = await resp.Content.ReadAsStringAsync();
                var respContent = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiRequestResultModel<List<Live_WhiteList>>>(json);
                return respContent;
            }
            else
            {
                return new ApiRequestResultModel<List<Live_WhiteList>>()
                {
                    Code = 500,
                    Data = null,
                    Message = "发生异常",
                };
            }
        }

        public async Task<ApiRequestResultModel<List<Live_V_UserEntry>>> GetUserEntryListBySalaryMonth(Guid customerGID, DateTime salaryMonth)
        {
            var httpClient = _httpClientFactory.CreateClient(ServiceNames.UsersService);
            var resp = await httpClient.GetAsync($"/api/Entry/GetUserEntryListBySalaryMonth?customerGID={customerGID.ToString()}&salaryMonth={salaryMonth}");
            if (resp.IsSuccessStatusCode)
            {
                var json = await resp.Content.ReadAsStringAsync();
                var respContent = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiRequestResultModel<List<Live_V_UserEntry>>>(json);
                return respContent;
            }
            else
            {
                return new ApiRequestResultModel<List<Live_V_UserEntry>>()
                {

                    Code = 500,
                    Data = null,
                    Message = "发生异常",
                };
            }



        }

        public async Task<ApiRequestResultModel<List<Live_V_UserEntry>>> GetZeroSalaryUserEntryList(Guid customerGID)
        {
            var httpClient = _httpClientFactory.CreateClient(ServiceNames.UsersService);
            var resp = await httpClient.GetAsync($"/api/Entry/GetZeroSalaryUserEntryList?customerGID={customerGID.ToString()}");
            if (resp.IsSuccessStatusCode)
            {
                var json = await resp.Content.ReadAsStringAsync();
                var respContent = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiRequestResultModel<List<Live_V_UserEntry>>>(json);
                return respContent;
            }
            else
            {
                return new ApiRequestResultModel<List<Live_V_UserEntry>>()
                {

                    Code = 500,
                    Data = null,
                    Message = "发生异常",
                };
            }



        }

        public async Task<ApiRequestResultModel<Live_V_UserEntry>> GetUserEntryByID(int id)
        {
            var httpClient = _httpClientFactory.CreateClient(ServiceNames.UsersService);
            var resp = await httpClient.GetAsync($"/api/Entry/View?id={id}");
            if (resp.IsSuccessStatusCode)
            {
                var json = await resp.Content.ReadAsStringAsync();
                var respContent = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiRequestResultModel<Live_V_UserEntry>>(json);
                return respContent;
            }
            else
            {
                return new ApiRequestResultModel<Live_V_UserEntry>()
                {

                    Code = 500,
                    Data = null,
                    Message = "发生异常",
                };
            }



        }
        public async Task<ApiRequestResultModel<Live_Partner>> GetPartnerByID(int id)
        {
            var httpClient = _httpClientFactory.CreateClient(ServiceNames.BasicService);
            var resp = await httpClient.GetAsync($"/api/Partner/Get?id={id}");
            if (resp.IsSuccessStatusCode)
            {
                var json = await resp.Content.ReadAsStringAsync();
                var respContent = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiRequestResultModel<Live_Partner>>(json);
                return respContent;
            }
            else
            {
                return new ApiRequestResultModel<Live_Partner>()
                {
                    Code = 500,
                    Data = null,
                    Message = "发生异常",
                };
            }
        }

        public async Task<ApiRequestResultModel<Live_Partner>> GetPartnerByName(string name)
        {
            var httpClient = _httpClientFactory.CreateClient(ServiceNames.BasicService);
            var resp = await httpClient.GetAsync($"/api/Partner/GetByName?name={name}");
            if (resp.IsSuccessStatusCode)
            {
                var json = await resp.Content.ReadAsStringAsync();
                var respContent = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiRequestResultModel<Live_Partner>>(json);
                return respContent;
            }
            else
            {
                return new ApiRequestResultModel<Live_Partner>()
                {
                    Code = 500,
                    Data = null,
                    Message = "发生异常",
                };
            }
        }

        public Task<Live_Enterprise> GetEnterpriseAsync(Guid? id, string name)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiRequestResultModel<Live_CustomerContractConfig>> GetCustomerContractConfigByIdAsync(Guid id)
        {
            var httpClient = _httpClientFactory.CreateClient(ServiceNames.CustomerService);
            var resp = await httpClient.GetAsync($"/api/CustomerContractConfig/GetByCustomerGID?id={id}");
            if (resp.IsSuccessStatusCode)
            {
                var json = await resp.Content.ReadAsStringAsync();
                var respContent = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiRequestResultModel<Live_CustomerContractConfig>>(json);
                return respContent;
            }
            else
            {
                return new ApiRequestResultModel<Live_CustomerContractConfig>()
                {
                    Code = 500,
                    Data = null,
                    Message = "发生异常",
                };
            }
        }

        public async Task<ApiRequestResultModel<Live_ContractFileConfigExtension>> GetContractFileConfigByIdAsync(Guid id)
        {
            var httpClient = _httpClientFactory.CreateClient(ServiceNames.CustomerService);
            var resp = await httpClient.GetAsync($"/api/ContractFileConfig/Get?id={id}");
            if (resp.IsSuccessStatusCode)
            {
                var json = await resp.Content.ReadAsStringAsync();
                var respContent = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiRequestResultModel<Live_ContractFileConfigExtension>>(json);
                return respContent;
            }
            else
            {
                return new ApiRequestResultModel<Live_ContractFileConfigExtension>()
                {
                    Code = 500,
                    Data = null,
                    Message = "发生异常",
                };
            }
        }

        public async Task<ApiRequestResultModel<Live_ContractSigner>> GetContractSignerByIdno(string idno)
        {
            var httpClient = _httpClientFactory.CreateClient(ServiceNames.CustomerService);
            var resp = await httpClient.GetAsync($"/api/ContractSigner/GetByIdno?idno={idno}");
            if (resp.IsSuccessStatusCode)
            {
                var json = await resp.Content.ReadAsStringAsync();
                var respContent = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiRequestResultModel<Live_ContractSigner>>(json);
                return respContent;
            }
            else
            {
                return new ApiRequestResultModel<Live_ContractSigner>()
                {
                    Code = 500,
                    Data = null,
                    Message = "发生异常",
                };
            }
        }

        
        public async Task<ApiRequestResultModel<List<Live_V_UserEntry>>> GetUserEntryListByUserID(int userID)
        {
            var httpClient = _httpClientFactory.CreateClient(ServiceNames.UsersService);
            var resp = await httpClient.GetAsync($"/api/Entry/GetUserEntryListByUserID?userID={userID.ToString()}");
            if (resp.IsSuccessStatusCode)
            {
                var json = await resp.Content.ReadAsStringAsync();
                var respContent = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiRequestResultModel<List<Live_V_UserEntry>>>(json);
                return respContent;
            }
            else
            {
                return new ApiRequestResultModel<List<Live_V_UserEntry>>()
                {

                    Code = 500,
                    Data = null,
                    Message = "发生异常",
                };
            }
        }


        #region 幸福职

        public record AesRequest( string Body,string Nonce);
        /// <summary>
        /// 单笔交易状态查询
        /// </summary>
        /// <param name="businessID"></param>
        /// <returns></returns>
        public async Task<XfzBaseResponse<EIST_Q01000405_RES>> SingleTransactionDetail(Guid businessID)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("Xfz");
                Dictionary<string, string> dss = new Dictionary<string, string>();
                dss.Add("BusinessID", businessID.ToString());
                string businessJosn = Newtonsoft.Json.JsonConvert.SerializeObject(dss);
                var encryptext = AesGcmHelper.Encrypt(businessJosn, Convert.FromBase64String(_aesOption.Key));
                var request = new AesRequest(encryptext, Guid.NewGuid().ToString("N"));
                string requestJosn = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                StringContent stringContent = new StringContent(requestJosn, Encoding.UTF8, "application/json");
                var resp = await httpClient.PostAsync("/api/CmbcEcp/SingleTransactionDetail", stringContent);
                if (resp.IsSuccessStatusCode)
                {
                    var json = await resp.Content.ReadAsStringAsync();
                    var respContent = Newtonsoft.Json.JsonConvert.DeserializeObject<XfzBaseResponse<EIST_Q01000405_RES>>(json);

                    if (respContent.Result == null)
                    {
                        respContent.Code = 500;
                    }
                    return respContent;
                }
                else
                {
                    return new XfzBaseResponse<EIST_Q01000405_RES>()
                    {
                        Code = 500,
                        Result = null,
                        Message = "发生异常",
                    };
                }
            }
            catch (Exception ex)
            {
                return new XfzBaseResponse<EIST_Q01000405_RES>()
                {
                    Code = 500,
                    Result = null,
                    Message = $"发送Http请求异常,异常信息：{ex.Message}",
                };
            }
        }
        /// <summary>
        ///  单笔交易推送
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task<XfzBaseResponse<bool>> SingleTransaction(SingleTransactionRequest req)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("Xfz");
                string businessJosn = Newtonsoft.Json.JsonConvert.SerializeObject(req);
                var encryptext = AesGcmHelper.Encrypt(businessJosn, Convert.FromBase64String(_aesOption.Key));
                var request = new AesRequest(encryptext, Guid.NewGuid().ToString("N"));
                string requestJosn = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                StringContent stringContent = new StringContent(requestJosn, Encoding.UTF8, "application/json");
                var resp = await httpClient.PostAsync("/api/CmbcEcp/SingleTransaction", stringContent);
                if (resp.IsSuccessStatusCode)
                {
                    var json = await resp.Content.ReadAsStringAsync();
                    var respContent = Newtonsoft.Json.JsonConvert.DeserializeObject<XfzBaseResponse<bool>>(json);
                    if (respContent.Result == false)
                    {
                        respContent.Code = 500;
                    }
                    return respContent;
                }
                else
                {
                    return new XfzBaseResponse<Boolean>()
                    {
                        Code = 500,
                        Result = false,
                        Message = "发生异常",
                    };
                }
            }
            catch (Exception ex)
            {
                return new XfzBaseResponse<bool>()
                {
                    Code = 500,
                    Result = false,
                    Message = $"发送Http请求异常,异常信息：{ex.Message}",
                };
            }
        }
        /// <summary>
        ///  批量交易查询
        /// </summary>
        /// <param name="businessID"></param>
        /// <returns></returns>
        public async Task<XfzBaseResponse<EIST_Q01000407_RES>> BatchTransactionDetail(Guid businessID)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("Xfz");
                Dictionary<string, string> dss = new Dictionary<string, string>();
                dss.Add("BusinessID", businessID.ToString());
                string businessJosn = Newtonsoft.Json.JsonConvert.SerializeObject(dss);
                var encryptext = AesGcmHelper.Encrypt(businessJosn, Convert.FromBase64String(_aesOption.Key));
                var request = new AesRequest(encryptext, Guid.NewGuid().ToString("N"));
                string requestJosn = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                StringContent stringContent = new StringContent(requestJosn, Encoding.UTF8, "application/json");
                var resp = await httpClient.PostAsync("/api/CmbcEcp/BatchTransactionDetail", stringContent);
                if (resp.IsSuccessStatusCode)
                {
                    var json = await resp.Content.ReadAsStringAsync();
                    var respContent = Newtonsoft.Json.JsonConvert.DeserializeObject<XfzBaseResponse<EIST_Q01000407_RES>>(json);


                    if (respContent.Result==null)
                    {
                        respContent.Code = 500;
                    }
                    return respContent;
                }
                else
                {
                    return new XfzBaseResponse<EIST_Q01000407_RES>()
                    {
                        Code = 500,
                        Result = null,
                        Message = "发生异常",
                    };
                }
            }
            catch (Exception ex)
            {
                return new XfzBaseResponse<EIST_Q01000407_RES>()
                {
                    Code = 500,
                    Result = null,
                    Message = $"发送Http请求异常,异常信息：{ex.Message}",
                };
            }
        }

        /// <summary>
        ///  批量交易推送
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task<XfzBaseResponse<bool>> BatchTransaction(BatchTransactionRequest req)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("Xfz");
                string businessJosn = Newtonsoft.Json.JsonConvert.SerializeObject(req);
                var encryptext = AesGcmHelper.Encrypt(businessJosn, Convert.FromBase64String(_aesOption.Key));
                var request = new AesRequest(encryptext, Guid.NewGuid().ToString("N"));
                string requestJosn = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                StringContent stringContent = new StringContent(requestJosn, Encoding.UTF8, "application/json");
                var resp = await httpClient.PostAsync("/api/CmbcEcp/BatchTransaction", stringContent);
                if (resp.IsSuccessStatusCode)
                {
                    var json = await resp.Content.ReadAsStringAsync();
                    var respContent = Newtonsoft.Json.JsonConvert.DeserializeObject<XfzBaseResponse<bool>>(json);

                    if (!respContent.Result)
                    {
                        respContent.Code= 500;
                    }
                    return respContent;
                }
                else
                {
                    return new XfzBaseResponse<Boolean>()
                    {
                        Code = 500,
                        Result = false,
                        Message = "发生异常",
                    };
                }
            }
            catch (Exception ex)
            {
                return new XfzBaseResponse<bool>()
                {
                    Code = 500,
                    Result = false,
                    Message = $"发送Http请求异常,异常信息：{ex.Message}",
                };
            }
        }




       
        /// <summary>
        ///  批量交易推送
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task<XfzBaseResponse<XfzPageResponse<EIST_Q01000404_RES_ITEM>>> EcpSignalTransactionPageList(DateTime dateFrom, DateTime dateTo, string subAccountBookNo, int pageNo, int pageSize)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("Xfz");


                Dictionary<string, string> dss = new Dictionary<string, string>();

                dss.Add("dateFrom", dateFrom.ToString("yyyy-MM-dd"));
                dss.Add("dateTo", dateTo.ToString("yyyy-MM-dd"));
                dss.Add("subAccountBookNo", subAccountBookNo);
                dss.Add("pageNo", pageNo.ToString());
                dss.Add("pageSize", pageSize.ToString());

                var queryString = string.Join("&", dss.Select(x => $"{x.Key}={x.Value}"));

                var resp = await httpClient.GetAsync("/api/CmbcEcpApi/GetSignalTransactionPageList?"+ queryString);
                if (resp.IsSuccessStatusCode)
                {
                    var json = await resp.Content.ReadAsStringAsync();
                    var result = Newtonsoft.Json.JsonConvert.DeserializeObject<XfzBaseResponse<XfzPageResponse<EIST_Q01000404_RES_ITEM>>>(json);

                    if (result.Success)
                    {
                        return result;
                    }
                    else {
                        return new XfzBaseResponse<XfzPageResponse<EIST_Q01000404_RES_ITEM>>
                        {
                            Code = 500,
                            Message = "发生异常",
                            Result=null
                        };
                    }
                 
                }
                else
                {
                    return new XfzBaseResponse<XfzPageResponse<EIST_Q01000404_RES_ITEM>>
                    {
                        Code = 500,
                        Message = "发生异常",
                        Result = null
                    };
                }
            }
            catch (Exception ex)
            {
                return new XfzBaseResponse<XfzPageResponse<EIST_Q01000404_RES_ITEM>>()
                {
                    Code = 500,
                    Result = null,
                    Message = $"发送Http请求异常,异常信息：{ex.Message}",
                };
            }
        }
        #endregion



        public record CmbcBankInfo(string Name, string IdCardNo,string CmbcAccountNo);
       

        public async Task<XfzBaseResponse<List<CmbcBankInfo>>> GetCmbcAccounts(List<string> idCardNoList)
        {
            try
            {

                object req =new { IdCardNos = idCardNoList };
                var httpClient = _httpClientFactory.CreateClient("Xfz");
                string requestJosn = Newtonsoft.Json.JsonConvert.SerializeObject(req);
                StringContent stringContent = new StringContent(requestJosn, Encoding.UTF8, "application/json");
                var resp = await httpClient.PostAsync("/api/CmbcBankCard/GetAccounts", stringContent);
                if (resp.IsSuccessStatusCode)
                {
                    var json = await resp.Content.ReadAsStringAsync();
                    var respContent = Newtonsoft.Json.JsonConvert.DeserializeObject<XfzBaseResponse<List<CmbcBankInfo>>>(json);
                    return respContent;
                }
                else
                {
                    return new XfzBaseResponse<List<CmbcBankInfo>>()
                    {
                        Code = 500,
                        Result = null,
                        Message = "发生异常",
                    };
                }
            }
            catch (Exception ex)
            {
                return new XfzBaseResponse<List<CmbcBankInfo>>()
                {
                    Code = 500,
                    Result = null,
                    Message = $"发送Http请求异常,异常信息：{ex.Message}",
                };
            }
        }



        public async Task<ApiRequestResultModel<Live_V_CustomerContractConfig>> GetContractFilesByCustomerGIDAsync(Guid customerGID)
        {
            var httpClient = _httpClientFactory.CreateClient(ServiceNames.CustomerService);
            var resp = await httpClient.GetAsync($"/api/ContractFileConfig/GetByCustomerGID?customerGID={customerGID}");
            if (resp.IsSuccessStatusCode)
            {
                var json = await resp.Content.ReadAsStringAsync();
                var respContent = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiRequestResultModel<Live_V_CustomerContractConfig>>(json);
                return respContent;
            }
            else
            {
                return new ApiRequestResultModel<Live_V_CustomerContractConfig>()
                {
                    Code = 500,
                    Data = null,
                    Message = "发生异常",
                };
            }
        }


        public async Task<ApiRequestResultModel<List<Live_V_Personnel>>> GetPersonnelListByCustomerGIDAsync(Guid customerGID)
        {
            var httpClient = _httpClientFactory.CreateClient(ServiceNames.UsersService);
            var resp = await httpClient.GetAsync($"/api/users/personnel/list?types=3,4&customerGID={customerGID}");
            if (resp.IsSuccessStatusCode)
            {
                var json = await resp.Content.ReadAsStringAsync();
                var respContent = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiRequestResultModel<List<Live_V_Personnel>>>(json);
                return respContent;
            }
            else
            {
                return new ApiRequestResultModel<List<Live_V_Personnel>>()
                {
                    Code = 500,
                    Data = null,
                    Message = "发生异常",
                };
            }
        }

        #region 预算申请
        /// <summary>
        /// 企业工资申请预算
        /// </summary>
        /// <param name="customerGID"></param>
        /// <returns></returns>
        public async Task<ApiRequestResultModel<int>> SalaryCustomerAutoComplete(Guid gid,Guid userGID,string userName,int companyID,Guid companyGID)
        {
            var httpClient = _httpClientFactory.CreateClient(ServiceNames.FinanceService);

            string dataStr = $"gid={gid}&userGID={userGID}&userName={userName}&companyID={companyID}&companyGID={companyGID}";
            var content = new StringContent(dataStr);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            var resp = await httpClient.PostAsync($"/api/BudgetApplyRecord/SalaryCustomerAutoComplete", content);
            if (resp.IsSuccessStatusCode)
            {
                var json = await resp.Content.ReadAsStringAsync();
                var respContent = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiRequestResultModel<int>>(json);
                return respContent;
            }
            else
            {
                return new ApiRequestResultModel<int>()
                {
                    Code = 500,
                    Data = 0,
                    Message = "发生异常",
                };
            }
        }

        public async Task<ApiRequestResultModel<int>> BudgetApplyRecordUpdateStatus(int id,int status,string msg="客户工资重置")
        {
            var httpClient = _httpClientFactory.CreateClient(ServiceNames.FinanceService);
            string dataStr = $"id={id}&status={status}&mssage={msg}";
            var content = new StringContent(dataStr);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            var resp = await httpClient.PostAsync($"/api/BudgetApplyRecord/UpdateStatus", content);
            if (resp.IsSuccessStatusCode)
            {
                var json = await resp.Content.ReadAsStringAsync();
                var respContent = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiRequestResultModel<int>>(json);
                return respContent;
            }
            else
            {
                return new ApiRequestResultModel<int>()
                {
                    Code = 500,
                    Data = 0,
                    Message = "发生异常",
                };
            }
        }


        public async Task<ApiRequestResultModel<string>> BudgetApplyRecordAuditing([FromForm] int id)
        {
            var httpClient = _httpClientFactory.CreateClient(ServiceNames.FinanceService);
            string dataStr = $"id={id}&method=1&status=4";
            var content = new StringContent(dataStr);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            var resp = await httpClient.PostAsync($"/api/BudgetApplyRecord/Auditing", content);
            if (resp.IsSuccessStatusCode)
            {
                var json = await resp.Content.ReadAsStringAsync();
                var respContent = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiRequestResultModel<string>>(json);
                return respContent;
            }
            else
            {
                return new ApiRequestResultModel<string>()
                {
                    Code = 500,
                    Data =string.Empty,
                    Message = "发生异常",
                };
            }
        }



        #endregion




        public async Task<List<Live_BusinessInsuranceConfig>> BusinessInsuranceConfigListAll()
        {
            var resp = await _httpService.GetAsync<List<Live_BusinessInsuranceConfig>>(ServiceNames.UsersService, $"/api/BusinessInsuranceConfig/ListAll");
            return resp;
        }



    }



}
