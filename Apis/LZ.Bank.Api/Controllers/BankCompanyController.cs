//using LZ.Bank.Service.Interface;
//using LZ.Core;
//using LZ.Model.Dtos.BankDo;
//using LZ.Model.Exceptions;
//using LZ.Model.PagedModel;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using SqlSugar;
//using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

//namespace LZ.Bank.Api.Controllers
//{
//    [Route("api/[controller]/[action]")]
//    [ApiController]
//    [AllowAnonymous]
//    public class BankCompanyController : ApiControllerBase
//    {
//        private readonly IBankCompanyService _service;

//        public BankCompanyController(IBankCompanyService service)
//        {
//            _service = service;
//        }

//        [HttpGet]
//        public async Task<IActionResult> PageList(string name,  [FromQuery] PagedModelBase query)
//        {
//            // 查询条件 dev 分支测试
//            var exp = Expressionable.Create<Model.Live_V_BankCompany>();
//            exp.And(f=>!f.IsDelete);
//            exp.AndIF(!string.IsNullOrWhiteSpace(name), f => f.Name.Contains(name));
//            var models = await _service.PageList(exp.ToExpression(), query);
//            return Ok(new { items = models, count = query.Count });
//        }

//        [HttpGet]
//        public async Task<IActionResult> List(string name,string queryType)
//        {
//            var exp = Expressionable.Create<Model.Live_V_BankCompany>();
//            exp.And(f => !f.IsDelete);
//            exp.AndIF(!string.IsNullOrWhiteSpace(name), f => f.Name.Contains(name));

//            if (string.IsNullOrWhiteSpace(queryType))
//            {
//                exp.And(f => f.CompanyID == CurrentUser.CompanyID);
//            }
//            else
//            {
//                if (!string.Equals(queryType, "all", StringComparison.OrdinalIgnoreCase))
//                {
//                    exp.And(f => f.CompanyID == CurrentUser.CompanyID);
//                }
//            }
//            var models = await _service.ListView(exp.ToExpression());
//            return Ok(models);
//        }

//        [HttpPost]
//        public async Task<IActionResult> Add(BankCompanyDto dto)
//        {
//            var model = new Model.Live_BankCompany()
//            {
//                CreateTime = DateTime.Now,
//                Name = dto.Name.Trim(),
//                CompanyNo = dto.CompanyNo.Trim(),
//                DeleteTime = null,
//                ID = Guid.NewGuid(),
//                IsDelete = false,
//                IsEnable = dto.IsEnable,
//                CompanyID = CurrentUser.CompanyID
//            };
//            var isExist = await _service.Exist(f => (f.Name == model.Name||f.CompanyNo==model.CompanyNo) && !f.IsDelete);
//            if (isExist)
//            {
//                throw new ErrorCodeException(-1, $"【{dto.Name}或{dto.CompanyNo}】已存在，无法重复创建！");
//            }

//            await _service.Add(model);
//            return Ok();
//        }
//        [HttpGet]
//        public async Task<IActionResult> Get(Guid id)
//        {
//            var model = await _service.Get(f=>f.ID==id);
//            if (model == null)
//            {
//                throw new ErrorCodeException(-1, "你要读取的数据不存在");
//            }
//            return Ok(model);
//        }
//        [HttpPut]
//        public async Task<IActionResult> Update(BankCompanyDto dto)
//        {
//            var model = await _service.Get(f => f.ID == dto.ID);
//            if (model == null)
//            {
//                throw new ErrorCodeException(-1, "你要读取的数据不存在");
//            }
//            model.Name = dto.Name.Trim();
//            model.CompanyNo = dto.CompanyNo.Trim();
//            model.IsEnable = dto.IsEnable;

//            var isExist = await _service.Exist(f => (f.Name == model.Name || f.CompanyNo == model.CompanyNo) &&f.ID!=dto.ID&& !f.IsDelete);
//            if (isExist)
//            {
//                throw new ErrorCodeException(-1, $"【{dto.Name}或{dto.CompanyNo}】已存在，无法重复创建！");
//            }

//            await _service.Update(model);
//            return Ok();
//        }
//        [HttpDelete]
//        public async Task<IActionResult> Delete([FromForm] Guid id)
//        {
//            var model = await _service.Get(f => f.ID == id);
//            if (model == null)
//            {
//                throw new ErrorCodeException(-1, "数据不存在");
//            }
//            model.IsEnable = false;
//            model.IsDelete = true;
//            model.DeleteTime= DateTime.Now;
//            await _service.Update(model);
//            return Ok();
//        }




//    }
//}
