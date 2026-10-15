using LZ.ApiService.Interface;
using LZ.Customer.Service.Interface;
using LZ.Customer.Service.Services;
using LZ.Model;
using LZ.Model.Dtos;
using LZ.Model.Exceptions;
using LZ.Model.FxqModel;
using LZ.Model.ModelExtensions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Senparc.CO2NET.Extensions;
using SqlSugar;
using System.Net.Http;

namespace LZ.Customer.API.Controllers
{
    public class CustomerContractConfigController(ICustomerContractConfigService service ) : CustomerControllerBase
    {

        [HttpGet]
        public async Task<IActionResult> GetByCustomerGID(Guid id)
        {
            var model = await service.GetByCustomerGID(id);
            return Ok(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Live_CustomerContractConfig model)
        {
            model.ID = Guid.NewGuid();
            await service.Add(model);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Live_CustomerContractConfig model)
        {
            await service.Update(model);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] Guid id)
        {
            if (!await service.Exist(id))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            await service.Delete(id);
            return Ok();
        }
    }
}
