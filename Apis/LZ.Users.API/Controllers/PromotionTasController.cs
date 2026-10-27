using LZ.Model.PagedModel;
using LZ.Users.Service.Interface;
using LZ.Users.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LZ.Users.API.Controllers
{
    /// <summary>
    /// 推管员任务
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionTasController(IPromotionTaskService promotionTaskService) : UsersControllerBase
    {
        /// <summary>
        /// 推销员任务
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> UpdateHaveMoneyOfPromotionTask()
        {
            await promotionTaskService.UpdateHaveMoneyOfPromotionTask();
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] PromotionPageTaskModel query)
        {
            var models = await promotionTaskService.List(query);
            return Ok(new { items = models, count = query.Count });
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Model.Shop_PromotionTask model)
        {
            var id = await promotionTaskService.Add(model);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Shop_PromotionTask model)
        {
            var id = await promotionTaskService.Update(model);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await promotionTaskService.Delete(id);
            return Ok(result);
        }
    }

}
