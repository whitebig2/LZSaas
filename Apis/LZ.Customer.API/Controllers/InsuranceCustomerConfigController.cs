//using AutoMapper;
//using LZ.ApiService.Interface;
//using LZ.Customer.Service.Interface;
//using LZ.Customer.Service.Services;
//using LZ.Model;
//using LZ.Model.Dtos;
//using LZ.Model.Exceptions;
//using LZ.Model.ModelExtensions;
//using LZ.Model.PagedModel;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Senparc.CO2NET.Extensions;
//using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
//using SqlSugar;
//using static System.Net.WebRequestMethods;

//namespace LZ.Customer.API.Controllers
//{
//    [AllowAnonymous]
//    public class InsuranceCustomerConfigController(IInsuranceCustomerConfigService service,IMapper mapper) : CustomerControllerBase
//    {

//        public record SigningAndCooperationUnitQuery(bool? IsSigningUnit,bool? IsCooperationUnit);

//        [HttpGet]
//        public async Task<IActionResult> PageList([FromQuery] PagedModelBase query)
//        {
//            var expable = Expressionable.Create<Model.Live_V_InsuranceCustomerConfig>();
//            var exp = expable.ToExpression();
//            var models = await service.ListView(exp, query);
//            return Ok(new { items = models, count = query.Count });
//        }
//        [HttpGet]
//        public async Task<IActionResult> List([FromQuery] PagedModelBase query)
//        {
//            var expable = Expressionable.Create<Model.Live_InsuranceCustomerConfig>();
//            expable.And(f => f.IsEnabled);
//            expable.AndIF(query.Main.HasValue, f => f.IsMain == query.Main);
//            expable.AndIF(query.GID.HasValue, f => f.CustomerGID == query.GID);
//            var exp = expable.ToExpression();
//            var models = await service.List(exp, query);
//            return Ok(new { items = models, count = query.Count });
//        }
       
//        [HttpGet]
//        public async Task<Live_V_InsuranceCustomerConfig> Get(Guid id)
//        {
//            var model = await service.GetView(f => f.ID == id);
//            if (model == null)
//            {
//                throw new ErrorCodeException(-1, "你要读取的数据不存在");
//            }
//            return model;
//        }
//        [HttpPost]
//        public async Task<IActionResult> Add([FromBody] Live_InsuranceCustomerConfig model)
//        {
//            model.CreateTime = DateTime.Now;
//            await service.Add(model);
//            return Ok();
//        }

//        [HttpPut]
//        public async Task<IActionResult> Update([FromBody] Live_InsuranceCustomerConfig model)
//        {
//            if (!await service.Exist(model.ID))
//            {
//                throw new ErrorCodeException(-1, "你要修改的数据不存在");
//            }
//            var existModel = await service.Get(f => f.ID == model.ID);
//            var updateModel = mapper.Map(model, existModel);
//            await service.Update(model);
//            return Ok();
//        }
//        [HttpDelete]
//        public async Task<IActionResult> Delete([FromForm] Guid id)
//        {
//            if (!await service.Exist(id))
//            {
//                throw new ErrorCodeException(-1, "你要删除的数据不存在");
//            }
//            var model = await service.Get(f => f.ID == id);
//            await service.Delete(id);
//            return Ok();
//        }


//        [HttpGet]
//        public async Task<IActionResult> GetConfigByCustomerGID([FromForm] Guid customerGID)
//        {
//            var models = await service.ListView(f => f.CustomerGID == customerGID, new PagedModelBase() { Page = 0 });
//            if (models == null|| models.Count<=0)
//            {
//                throw new ErrorCodeException(-1, "你要读取的数据不存在");
//            }
//            return Ok(models);
//        }
//    }
//}
