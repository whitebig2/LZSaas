//using LZ.Bank.Service.Interface;
//using LZ.Core;
//using LZ.Model;
//using LZ.Model.Exceptions;
//using LZ.Model.PagedModel;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using SqlSugar;

//namespace LZ.Bank.Api.Controllers
//{
//    [Route("api/[controller]/[action]")]
//    [ApiController]
//    public class BankUserActionsController : ApiControllerBase
//    {
//        private readonly IBankUserCompanyService _service;

//        public BankUserActionsController(IBankUserCompanyService service)
//        {
//            _service = service;
//        }

//        [HttpGet]
//        public async Task<IActionResult> PageList(string customerName, string signUnitName, [FromQuery] PagedModelBase query)
//        {
//            var exp = Expressionable.Create<Model.Live_BankUserCompany>();
//            //exp.And(f=>!f.IsDelete);
//            //exp.AndIF(!string.IsNullOrWhiteSpace(customerName), f => f.CustomerName.Contains(customerName));
//            //exp.AndIF(!string.IsNullOrWhiteSpace(signUnitName), f => f.SignUnitName.Contains(signUnitName));
//            var models = await _service.PageList(exp.ToExpression(), query);
//            return Ok(new { items = models, count = query.Count });
//        }
//        [HttpPost]
//        public async Task<IActionResult> Add(Model.Live_BankUserCompany dto)
//        {
//            var model = new Model.Live_BankUserCompany()
//            {
//                CreateTime = DateTime.Now,
//                BankCompanyID = dto.BankCompanyID,
//                ID = Guid.NewGuid(),
//                IsDetail = dto.IsDetail,
//                IsEnable = dto.IsEnable,
//                IsExecute = dto.IsExecute,
//                IsPay = dto.IsPay,
//                UserID = dto.UserID,
               
//            };

//            var isExist=await _service.Exist(f =>f.UserID==model.UserID&&f.BankCompanyID == dto.BankCompanyID);
//            if (isExist)
//            {
//                throw new ErrorCodeException(-1,$"此操作记录已存在，无法重复创建！");
//            }
           
//            await _service.Add(model);
//            return Ok();
//        }
//        [HttpGet]
//        public async Task<Model.Live_BankUserCompany> Get(Guid id)
//        {
//            var model = await _service.Get(f=>f.ID==id);
//            if (model == null)
//            {
//                throw new ErrorCodeException(-1, "你要读取的数据不存在");
//            }
//            return model;
//        }
//        [HttpPut]
//        public async Task<IActionResult> Update([FromBody] Model.Live_BankUserCompany model)
//        {
//            await _service.Update(model);
//            return Ok();
//        }
//        [HttpDelete]
//        public async Task<IActionResult> Delete([FromForm] Guid id)
//        {
//            await _service.Delete(id);
//            return Ok();
//        }

//    }
//}
