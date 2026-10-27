using LZ.Customer.Service.Interface;
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
using System.Net.Http;


namespace LZ.Customer.Service.Services
{
    public class CustomerOtherService : ICustomerOtherService, IScopedDependency
    {


        private readonly IHttpService  _httpService;
        private readonly IHttpClientFactory _httpClientFactory;

        public CustomerOtherService(IHttpClientFactory httpClientFactory, IHttpService httpService)
        {
            _httpClientFactory = httpClientFactory;
            _httpService = httpService;
        }

        public async Task<RequestResultModel> GetClientShortLink(int companyId, string clientType, string pageUrl, string pageTitle)
        {


           
            var headers = new Dictionary<string, string>();
            //headers.Add("Authorization", "d00a4832-1da6-432f-883a-1eaa93359aab");
            Dictionary<string, string> dss = new Dictionary<string, string>();
            dss.Add("companyId", companyId.ToString());
            dss.Add("clientType", clientType);
            dss.Add("pageUrl", pageUrl);
            dss.Add("pageTitle", pageTitle);
            var userRecord = await _httpService.GetAsync(ServiceNames.ClientsService, $"/api/clientshortlink/getlink",dss);
            return userRecord;
        }


        public async Task<ApiRequestResultModel<Live_V_UserEntry>> GetUserEntryRecord(int id)
        {
            var httpClient = _httpClientFactory.CreateClient(ServiceNames.UsersService);
            //Dictionary<string, string> dss = new Dictionary<string, string>();
            //    dss.Add("id", id.ToString());
            //var resss=await  _httpService.GetAsync(ServiceNames.UsersService, "/api/Entry/Get", dss);
            var resp = await httpClient.GetAsync($"/api/Entry/View?id={id.ToString()}");
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

        public async Task<ApiRequestResultModel<List<Live_UserLeaveReason>>> ListLeaveReason(int id)
        {
            var httpClient = _httpClientFactory.CreateClient(ServiceNames.UsersService);
            var resp = await httpClient.GetAsync($"/api/Entry/ListLeaveReason?id={id.ToString()}");
            if (resp.IsSuccessStatusCode)
            {
                var json = await resp.Content.ReadAsStringAsync();
                var respContent = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiRequestResultModel<List<Live_UserLeaveReason>>>(json);
                return respContent;
            }
            else
            {
                return new ApiRequestResultModel<List<Live_UserLeaveReason>>()
                {

                    Code = 500,
                    Data = null,
                    Message = "发生异常",
                };
            }
        }

        

        public async Task<ApiRequestResultModel<Live_V_UserEntryContract>> GetUserEntryContract(int id)
        {
            var httpClient = _httpClientFactory.CreateClient(ServiceNames.UsersService);
            var resp = await httpClient.GetAsync($"/api/Entry/GetUserEntryContract?userEntryId={id.ToString()}");
            if (resp.IsSuccessStatusCode)
            {
                var json = await resp.Content.ReadAsStringAsync();
                var respContent = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiRequestResultModel<Live_V_UserEntryContract>>(json);
                return respContent;
            }
            else
            {
                return new ApiRequestResultModel<Live_V_UserEntryContract>()
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


        public async Task<ApiRequestResultModel<List<Live_V_UserEntry>>> GetUserEntryListByInsuranceStartDate(Guid customerGID, DateTime startDate)
        {
            var httpClient = _httpClientFactory.CreateClient(ServiceNames.UsersService);
            var resp = await httpClient.GetAsync($"/api/Entry/GetUserEntryListByInsuranceStartDate?customerGID={customerGID.ToString()}&startDate={startDate.ToString("yyyy-MM-dd")}");
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


        

        /// <summary>
        /// 获取签章单位
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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



        public async Task<ApiRequestResultModel<Live_V_SalaryCashSendRecord>> GetSalaryCashSendRecordByIdAsync(Guid id)
        {
            var httpClient = _httpClientFactory.CreateClient(ServiceNames.FinanceService);
            var resp = await httpClient.GetAsync($"/api/SalaryCashSendRecord/GetView?id={id.ToString()}");
            if (resp.IsSuccessStatusCode)
            {
                var json = await resp.Content.ReadAsStringAsync();
                var respContent = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiRequestResultModel<Live_V_SalaryCashSendRecord>>(json);
                return respContent;
            }
            else
            {
                return new ApiRequestResultModel<Live_V_SalaryCashSendRecord>()
                {

                    Code = 500,
                    Data = null,
                    Message = "发生异常",
                };
            }
        }



    }
}
