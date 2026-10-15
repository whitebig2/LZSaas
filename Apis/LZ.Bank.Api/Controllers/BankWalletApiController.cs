using LZ.Bank.Service.Interface;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace LZ.Bank.Api.Controllers
{
    [AllowAnonymous]
    public class BankWalletApiController():BankControllerBase
    {
        //[HttpGet]
        //public async Task<IActionResult> List([FromQuery] UserPagedModel query)
        //{
        //    var expable = Expressionable.Create<Model.Live_BankWallet>();
        //    if (query.Status.HasValue)
        //    {
        //        expable.And(t => t.Status == query.Status);
        //    }

        //    expable.AndIF(!string.IsNullOrEmpty(query.Name), t => t.Name.Contains(query.Name));
        //    var exp = expable.ToExpression();
        //    var models = await bankWalletService.PageList(exp, query);
        //    return Ok(new { items = models, count = query.Count });
        //}

        //[HttpGet]
        //public async Task<IActionResult> Add([from] int entryID)
        //{
        //    if (entryID == 0)
        //    {
        //        throw new ErrorCodeException(-1, "参数错误");
        //    }
        //    var entry = 
        //    var expable = Expressionable.Create<Model.Live_BankWallet>();
            
        //    return Ok();
        //}
    }
}
