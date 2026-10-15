using LZ.Model;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using LZ.Users.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace LZ.Users.API.Controllers
{
    [AllowAnonymous]
    public class BusInsuranceCompanyController : UsersControllerBase
    {
        private readonly IBusInsuranceCompanyService _companyService;

        public BusInsuranceCompanyController(IBusInsuranceCompanyService companyService)
        {
            _companyService = companyService;
        }

        [HttpGet]
        public async Task<IActionResult> PageList(string insuranceCompany, bool? isEnable, bool? isLimitEntry, bool? isWorkDay, [FromQuery] PagedModelBase query)
        {
            var exp = Expressionable.Create<Live_BusInsuranceCompany>();
            exp.AndIF(!string.IsNullOrWhiteSpace(insuranceCompany), f => f.InsuranceCompany.Contains(insuranceCompany));
            if (isEnable.HasValue)
            {
                var enable = isEnable.Value;
                exp.And(f => f.IsEnable == enable);
            }
            if (isLimitEntry.HasValue)
            {
                var limitEntry = isLimitEntry.Value;
                exp.And(f => f.IsLimitEntry == limitEntry);
            }
            if (isWorkDay.HasValue)
            {
                var workDay = isWorkDay.Value;
                exp.And(f => f.IsWorkDay == workDay);
            }
            var models = await _companyService.PageList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }

        [HttpGet]
        public async Task<IActionResult> EnableList()
        {
            var models = await _companyService.List(f => f.IsEnable);
            var names = models.Select(f => f.InsuranceCompany).ToList();
            return Ok(names);
        }

        [HttpGet]
        public async Task<IActionResult> Get(Guid id)
        {
            var model = await _companyService.Get(f => f.ID == id);
            return Ok(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(Live_BusInsuranceCompany model)
        {
            var exist = await _companyService.Get(f => f.InsuranceCompany == model.InsuranceCompany);
            if (exist != null)
            {
                throw new ErrorCodeException(-1, "此保险公司已配置，无需重复添加");
            }

            if (model.ID == Guid.Empty)
            {
                model.ID = Guid.NewGuid();
            }
            model.CreateTime = DateTime.Now;
            await _companyService.Add(model);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update(Live_BusInsuranceCompany model)
        {
            var exist = await _companyService.Get(f => f.InsuranceCompany == model.InsuranceCompany && f.ID != model.ID);
            if (exist != null)
            {
                throw new ErrorCodeException(-1, "此保险公司已配置，无法修改为此公司");
            }
            await _companyService.Update(model);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] Guid id)
        {
            await _companyService.Delete(f => f.ID == id);
            return Ok();
        }
    }
}
