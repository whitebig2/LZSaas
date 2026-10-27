using LZ.ApiService.Interface;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Core.Services;
using LZ.Finance.Service.Interface;
using LZ.Finance.Service.Services;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.ModelExtensions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NetTaste;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.SS.Formula.Functions;
using Senparc.CO2NET.Extensions;
using SqlSugar;
using System.Collections.Generic;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace LZ.Finance.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    public class CityPartnerReportController : ApiControllerBase
    {

        private readonly ICityPartnerReportService  _cityPartnerReportService;
        private readonly IFinanceOtherService  _financeOtherService;
        private readonly IAuthService  _authService;
        private readonly IHttpService _httpService;
        private readonly ISaleService _saleService;

        public CityPartnerReportController(ICityPartnerReportService cityPartnerReportService, IFinanceOtherService financeOtherService, IAuthService authService, IHttpService httpService,ISaleService saleService)
        {
            _cityPartnerReportService = cityPartnerReportService;
            _financeOtherService = financeOtherService;
            _authService = authService;
            _httpService = httpService;
            _saleService = saleService;
        }

        [HttpGet]
        public async Task<IActionResult> PageList(string title, string agentName, string agentPhone, [FromQuery] PagedModelBase query)
        {
            var user=CurrentUser;
            var exp = Expressionable.Create<Model.Live_V_CityPartnerReport>();
            //exp.AndIF(CurrentUser.PlatformType != PlatformType.Admin && CurrentUser.PlatformType != PlatformType.Api, f => f.CompanyID == CurrentUser.CompanyID);
            //exp.AndIF(!string.IsNullOrWhiteSpace(customerName), f => f.CustomerName.Contains(customerName));
            if (user.PlatformType == Model.Enums.PlatformType.MiniAdmin && user.CompanyType != CompanyType.Party.ToString().ToLower())
            {
                var info = await _authService.UserInfo<Model.SYS_CompanyUser>();
                if (info != null && info.UserType == 2&& info.PartnerGID.HasValue)
                {
                    exp.And(it => it.ProfitPartnerGIDArr.Contains(info.PartnerGID.Value.ToString()));
                }
            }
            exp.AndIF(!string.IsNullOrWhiteSpace(title), f => f.Title.Contains(title));
            exp.AndIF(!string.IsNullOrWhiteSpace(agentName), f => f.AgentName.Contains(agentName));
            exp.AndIF(!string.IsNullOrWhiteSpace(agentPhone), f => f.AgentPhone.Contains(agentPhone));
            var models = await _cityPartnerReportService.PageList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }

        [HttpGet]
        public async Task<IActionResult> GetReportByToken()
        {
            var user = CurrentUser;
            var exp = Expressionable.Create<Model.Live_V_CityPartnerReport>();
            //exp.AndIF(CurrentUser.PlatformType != PlatformType.Admin && CurrentUser.PlatformType != PlatformType.Api, f => f.CompanyID == CurrentUser.CompanyID);
            //exp.AndIF(!string.IsNullOrWhiteSpace(customerName), f => f.CustomerName.Contains(customerName));
            var info = await _authService.UserInfo<Model.Live_Personnel>();
            //服务商
            if (info != null && !string.IsNullOrEmpty(info.Phone))
            {
                exp.And(it => it.AgentPhone.Contains(info.Phone));
                var model = await _cityPartnerReportService.GetView(exp.ToExpression());
                if (model != null)
                {
                    Dictionary<string, string> data = new Dictionary<string, string>();
                    data.Add("gid", model.ID.ToString());
                    var count = await _saleService.GetCount(data,"/api/web/api/GetCount");
                    var list = await _saleService.GetCustomerCountList(data, "/api/web/api/GetCityPartnerCount");


                    return Ok(new { data = model, count,Customer = list,phone = info.Phone, });

                }
                return Ok(new { data = model});
            }
            else
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetSalePartnerList(Guid gid,int page)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("pid", gid.ToString());
            data.Add("page", page.ToString());
            var datajson = JsonConvert.SerializeObject(data);
            var list = await _saleService.GetManageUserList(datajson,"/api/web/api/list");
            return Ok(new { items = list });
        }

        [HttpPost]
        public async Task<SaleMessageResult> AddSalePartner([FromBody] Model.SaleManageUser model)
        {
            var citypartner = await _cityPartnerReportService.GetView(t=>t.ID==model.CityPartnerGID);
            model.CityPartnerName = citypartner.Title;
            model.Enabled = true;
            model.RoleIDS = "3939f3fc-c3ff-42ff-9e75-7451394b69d1";
            model.RoleNames = "营销人员";
            var datajson = JsonConvert.SerializeObject(model);
            var result = await _saleService.AuditManageUser(datajson,"/api/web/api/add");
            return result;
        }

        [HttpGet]
        public async Task<IActionResult> GetView(Guid id)
        {
            var model = await _cityPartnerReportService.GetView(f=>f.ID==id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return Ok(model);
        }


        [HttpGet]
        public async Task<IActionResult> ListCustomer(string cityPartnerTitle,string name,int page,int pageSize)
        {
            //var model = await _cityPartnerReportService.GetView(f => f.ID == id);
            //List<SYS_Company> customers = new List<SYS_Company>();
            //var cusRes = await _financeOtherService.ListCompany(id);
            //if (cusRes.Code == 0 && cusRes.Data != null)
            //{
            //    customers = cusRes.Data;
            //}
            //if (model == null)
            //{
            //    throw new ErrorCodeException(-1, "你要读取的数据不存在");
            //}


            if (string.IsNullOrWhiteSpace(cityPartnerTitle))
            {
                return Ok(new { items = new List<Live_Customer>(), count = 0 });
            }

            var res = await _financeOtherService.ListCustomerPage(name, cityPartnerTitle, true, page, pageSize);

            if (res.Data != null)
            {
                return Ok(new { items = res.Data.items, count = res.Data.count });
            }
            else
            {

                throw new ErrorCodeException(-1, "未获取到匹配数据");
            }
        }


        [HttpGet]
        public async Task<IActionResult> ListCompany(Guid id)
        {
            var model = await _cityPartnerReportService.GetView(f => f.ID == id);

            List<SYS_Company> customers = new List<SYS_Company>();

            var cusRes = await _financeOtherService.ListCompany(id);
            if (cusRes.Code == 0 && cusRes.Data != null)
            {
                customers = cusRes.Data;
            }
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return Ok(new { items = customers, count = customers.Count });
        }

        [HttpGet]
        public async Task<IActionResult> ListPartner(Guid id,int partnerType)
        {
            var model = await _cityPartnerReportService.GetView(f => f.ID == id);


            bool isBusinessPartner = partnerType == 9;
            List<Live_Partner> customers = new List<Live_Partner>();
            var partnerRes = await _financeOtherService.ListPartner(id, partnerType==9?null: partnerType, partnerType == 9?true:null);
            if (partnerRes == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return Ok(new { items = partnerRes.items, count = partnerRes.items.Count });
        }


        [HttpGet]
        public async Task<IActionResult> ListHQReports()
        {
            List<string> cityPartnerTitleList = new List<string>() { "幸福职营业部", "宁波合伙人", "台州合伙人" };
            //List<string> cityPartnerTitleList = new List<string>() { "测试", "ceshi", "ceshi1" };
            var user = CurrentUser;
            var exp = Expressionable.Create<Model.Live_V_CityPartnerReport>();
            exp.And(f=>cityPartnerTitleList.Contains(f.Title));
            var models = await _cityPartnerReportService.ListView(exp.ToExpression());
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("names", "幸福职营业部,宁波合伙人,台州合伙人");
            //data.Add("names", "测试,ceshi,ceshi1");
            var list = await _saleService.GetCustomerCountList(data, "/api/web/api/GetCityPartnerCount");

            return Ok(new { items = models, count = models.Count,list });
        }


        [HttpGet]
        public async Task<IActionResult> GetByCityPartnerID(Guid cityPartnerID)
        {
            var user = CurrentUser;
            var exp = Expressionable.Create<Model.Live_V_CityPartnerReport>();
            exp.And(f => f.ID == cityPartnerID);
            var model = await _cityPartnerReportService.GetView(exp.ToExpression());
            return Ok(model);
        }

    }
}
