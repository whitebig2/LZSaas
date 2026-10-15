using AutoMapper;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using LZ.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using LZ.Users.Service.Interface;
using System.Security.Principal;
using Microsoft.AspNetCore.Authorization;
using LZ.Model.ModelExtensions;

namespace LZ.Users.API.Controllers
{

    public class BusinessInsuranceConfigController : UsersControllerBase
    {

        private readonly IBusinessInsuranceConfigService   _configService;

        public BusinessInsuranceConfigController(IBusinessInsuranceConfigService configService)
        {
            _configService = configService;
        }

        [HttpGet]
        public async Task<IActionResult> PageList(string title,string insuranceCompany,string customerName, [FromQuery] PagedModelBase query)
        {
            var exp = Expressionable.Create<Model.Live_V_BusinessInsuranceConfig>();
            exp.AndIF(!string.IsNullOrWhiteSpace(title), f => f.Title.Contains(title));
            exp.AndIF(!string.IsNullOrWhiteSpace(insuranceCompany), f => f.InsuranceCompany.Contains(insuranceCompany));
            exp.AndIF(!string.IsNullOrWhiteSpace(customerName), f => f.CustomerName.Contains(customerName));
            var models = await _configService.PageList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }

        [HttpGet]
        public async Task<Live_V_BusinessInsuranceConfig> Get(Guid id)
        {
            var model = await _configService.GetView(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }
        [HttpGet]
        public async Task<Live_BusinessInsuranceConfigExtension> GetNav(Guid id)
        {
            var model = await _configService.GetNav(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Live_BusinessInsuranceConfigExtension model)
        {
            model.CreateTime = DateTime.Now;
            await _configService.AddNav(model);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Live_BusinessInsuranceConfigExtension model)
        {
            if (!await _configService.Exist(model.ID))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            await _configService.UpdateNav(model);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] Guid id)
        {
            if (!await _configService.Exist(id))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            var model = await _configService.Get(f => f.ID == id);
            await _configService.DeleteNav(id);
            return Ok();
        }


        [HttpGet]
        public async Task<IActionResult> ListAll()
        {
            var models = await _configService.List(f => f.IsEnabled == true);
            return Ok(models);
        }
    }
}
