using LZ.Basic.Service.Interface;
using LZ.Basic.Service.Services;
using LZ.Core;
using LZ.Core.Attributes;
using LZ.Core.Redis;
using LZ.Model;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace LZ.Basic.API.Controllers
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    public class VoteUserController(IVoteUserService voteUserService) : BasicControllerBase
    {
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [PlatformLimit(Model.Enums.PlatformType.LZMini)]
        [HttpGet]
        public async Task<IActionResult> List([FromQuery]BasicPagedModel query)
        {
            var expable = Expressionable.Create<Model.Live_VoteUser>();

            var exp= expable.ToExpression();
            var models = await voteUserService.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Submit([FromBody] Model.VoteUserModel model)
        {
            if (string.IsNullOrEmpty(model.Name) || string.IsNullOrEmpty(model.Phone) || string.IsNullOrEmpty(model.LikeProduct) || model.Items == null || model.Items.Count == 0)
            {
                throw new ErrorCodeException(-1, "提交的信息不全");
            }
            if(await voteUserService.Exist(it => it.Phone == model.Phone))
            {
                throw new ErrorCodeException(-1, "该手机号已投票！");
            }
            var voteUser = new Model.Live_VoteUser();
            voteUser.Name = model.Name;
            voteUser.Phone = model.Phone;
            voteUser.LikeProduct = model.LikeProduct;
            voteUser.CreateTime = DateTime.Now;
            voteUser.UserID= await voteUserService.Add(voteUser);
            var voteItems=new List<Model.Live_VoteItems>();
            foreach (var item in model.Items)
            {
                var voteItem = new Model.Live_VoteItems();
                voteItem.UserID = voteUser.UserID;
                voteItem.Name = item.Name;
                voteItem.ClassName = item.Class;
                voteItem.FromID = item.ID;
                voteItems.Add(voteItem);
            }
            await voteUserService.AddItems(voteItems);
            return Ok();
        }
       
    }
}
