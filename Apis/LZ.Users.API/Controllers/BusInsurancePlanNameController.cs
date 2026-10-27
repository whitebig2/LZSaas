using LZ.Model;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using LZ.Users.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace LZ.Users.API.Controllers
{
    public class BusInsurancePlanNameController : UsersControllerBase
    {
        private readonly IBusInsurancePlanNameService _service;

        public BusInsurancePlanNameController(IBusInsurancePlanNameService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> PageList(string planName, string insurancePlanName, bool? isEnable, [FromQuery] PagedModelBase query)
        {
            var exp = Expressionable.Create<Live_BusInsurancePlanName>();
            exp.AndIF(!string.IsNullOrWhiteSpace(planName), f => f.PlanName.Contains(planName));
            exp.AndIF(!string.IsNullOrWhiteSpace(insurancePlanName), f => f.InsurancePlanName.Contains(insurancePlanName));
            if (isEnable.HasValue)
            {
                var enable = isEnable.Value;
                exp.And(f => f.IsEnable == enable);
            }
            var models = await _service.PageList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }

        [HttpGet]
        public async Task<IActionResult> List(string planName, bool? isEnable)
        {
            var exp = Expressionable.Create<Live_BusInsurancePlanName>();
            exp.AndIF(!string.IsNullOrWhiteSpace(planName), f => f.PlanName.Contains(planName));
            if (isEnable.HasValue)
            {
                var enable = isEnable.Value;
                exp.And(f => f.IsEnable == enable);
            }
            var models = await _service.List(exp.ToExpression());
            return Ok(models);
        }

        [HttpGet]
        public async Task<IActionResult> EnableList()
        {
            var models = await _service.List(f => f.IsEnable);
            var names = models.Select(f => f.PlanName).Distinct().ToList();
            return Ok(names);
        }

        [HttpGet]
        public async Task<IActionResult> Get(Guid id)
        {
            var model = await _service.Get(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return Ok(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(Live_BusInsurancePlanName model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.PlanName))
            {
                throw new ErrorCodeException(-1, "方案名称不能为空");
            }

            model.PlanName = model.PlanName.Trim();
            model.InsurancePlanName = model.InsurancePlanName?.Trim();
            model.Remark = model.Remark?.Trim();

            var exist = await _service.Get(f => f.PlanName == model.PlanName);
            if (exist != null)
            {
                throw new ErrorCodeException(-1, "此方案名称已配置，无需重复添加");
            }

            if (model.ID == Guid.Empty)
            {
                model.ID = Guid.NewGuid();
            }
            model.CreateTime = DateTime.Now;
            await _service.Add(model);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update(Live_BusInsurancePlanName model)
        {
            if (model == null || model.ID == Guid.Empty)
            {
                throw new ErrorCodeException(-1, "方案不能为空");
            }
            if (string.IsNullOrWhiteSpace(model.PlanName))
            {
                throw new ErrorCodeException(-1, "方案名称不能为空");
            }

            var current = await _service.Get(f => f.ID == model.ID);
            if (current == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }

            model.PlanName = model.PlanName.Trim();
            model.InsurancePlanName = model.InsurancePlanName?.Trim();
            model.Remark = model.Remark?.Trim();

            var exist = await _service.Get(f => f.PlanName == model.PlanName && f.ID != model.ID);
            if (exist != null)
            {
                throw new ErrorCodeException(-1, "此方案名称已配置，无法修改为此名称");
            }

            model.CreateTime = current.CreateTime;
            await _service.Update(model);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> SetEnable([FromForm] Guid id)
        {
            var model = await _service.Get(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "数据不存在");
            }
            model.IsEnable = !model.IsEnable;
            await _service.Update(model);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ErrorCodeException(-1, "方案不能为空");
            }
            await _service.Delete(f => f.ID == id);
            return Ok();
        }
    }
}
