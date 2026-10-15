using LZ.ApiService.Interface;
using LZ.Core;
using LZ.Core.Attributes;
using LZ.Core.Interface;
using LZ.Core.Services;
using LZ.Model;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using LZ.Users.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IISIntegration;
using NetTaste;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlSugar;
using SqlSugar.Extensions;

namespace LZ.Users.API.Controllers
{
    public class UserAddressController(IUserAddressService userAddressService) : UsersControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] UserPagedModel query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Shop_UserAddress>();
            if (!string.IsNullOrEmpty(query.UserName))
            {
                expable.And(it => it.Name.Contains(query.UserName));
            }
            expable.AndIF(query.IsDefault.HasValue,it=>it.IsDefault== query.IsDefault);
            expable.And(it => it.UserID == user.UserID);
            var exp = expable.ToExpression();
            var models = await userAddressService.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }
        [HttpPost]
        [OperationLogAction("新增地址：{model.Name}")]
        public async Task<IActionResult> Add([FromBody] Model.Shop_UserAddress model)
        {
            var user = CurrentUser;
            model.CreateTime = DateTime.Now;
            model.UserID = user.UserID;
            await userAddressService.Add(model);
            return Ok();
        }
        [HttpGet]
        public async Task<Model.Shop_UserAddress> Get(int id)
        {
            return await userAddressService.Get(id);
        }
        [HttpPut]
        [OperationLogAction("更新地址：{model.Name}")]
        public async Task<IActionResult> Update([FromBody] Model.Shop_UserAddress model)
        {
            await userAddressService.Update(model);
            return Ok();
        }
    }
}
