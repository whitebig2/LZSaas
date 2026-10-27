//using LZ.Bank.Service.Interface;
//using LZ.Core;
//using LZ.Model;
//using LZ.Model.Bank.CZY.ParamSetModels;
//using LZ.Model.Dtos.BankDo;
//using LZ.Model.Enums;
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
//    public class BankElectronicReceiptController : ApiControllerBase
//    {
//        private readonly IBankElectronicReceiptService _service;
//        private readonly ICzyService _czyService;

//        public BankElectronicReceiptController(IBankElectronicReceiptService service, ICzyService czyService)
//        {
//            _service = service;
//            _czyService = czyService;
//        }

//        [HttpGet]
//        public async Task<IActionResult> PageList(string name,  [FromQuery] PagedModelBase query)
//        {
//            var exp = Expressionable.Create<Model.Live_BankElectronicReceipt>();
//            var models = await _service.PageList(exp.ToExpression(), query);
//            return Ok(new { items = models, count = query.Count });
//        }


//        [AllowAnonymous]
//        [HttpGet]
//        public async Task<IActionResult> PageListElectronicReceiptRecords(string transType, string queryType, string acctNo, DateTime? beginDate, DateTime? endDate, int page = 0)
//        {
//            page = page--;
//            T0028ParamSetItem req = new T0028ParamSetItem()
//            {
//                AcctNo = acctNo,
//                BeginDate = (beginDate ?? DateTime.Today).ToString("yyyyMMdd"),
//                EndDate = (endDate ?? DateTime.Today).ToString("yyyyMMdd"),
//                CurrencyCode = "CNY",
//                TransType = transType,
//                QueryType = queryType,
//                NextTag = page.ToString()
//            };
//            var res = await _czyService.QueryElectronicReceiptList(req);
//            return Ok(res.Cmscloud.Body.Data);
//        }




//    }
//}
