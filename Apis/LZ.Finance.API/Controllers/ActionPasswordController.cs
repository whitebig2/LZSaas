using AutoMapper;
using LZ.ApiService.Interface;
using LZ.Common;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Finance.Service.Interface;
using LZ.Finance.Service.Services;
using LZ.Model;
using LZ.Model.Dtos.SettlementDtos;
using LZ.Model.Exceptions;
using LZ.Model.Options;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Senparc.CO2NET.Extensions;
using SqlSugar;
using SqlSugar.SplitTableExtensions;

namespace LZ.Finance.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    public class ActionPasswordController : ApiControllerBase
    {

        //private readonly ICurrentUser    _fxqService;

        private readonly AesOption _aesOption;
        private readonly IFxqService    _fxqService;
        private readonly IActionPasswordService   _actionPasswordService;
        private readonly IMapper    _mapper;

        public ActionPasswordController(IActionPasswordService actionPasswordService, IMapper mapper, IFxqService fxqService,IOptions<AesOption> options)
        {
            _actionPasswordService = actionPasswordService;
            _mapper = mapper;
            _fxqService = fxqService;
            _aesOption= options.Value;
        }

        [HttpGet]
        public async Task<IActionResult> PageList(string name,string phone,string idCardNo,[FromQuery] PagedModelBase query)
        {
            Console.WriteLine(  Guid.Empty);
            var exp = Expressionable.Create<Model.Live_ActionPassword>();
            exp.AndIF(!string.IsNullOrWhiteSpace(name), f => f.Name.Contains(name));
            exp.AndIF(!string.IsNullOrWhiteSpace(idCardNo), f => f.IdCardNo.Contains(idCardNo));
            exp.AndIF(!string.IsNullOrWhiteSpace(phone), f => f.Phone.Contains(phone));
            var models = await _actionPasswordService.PageList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }
        [HttpPost]
        public async Task<IActionResult> Add(Live_ActionPassword model)
        {
            var realNameResp = await _fxqService.IdentityAuthentication3(model.Name, model.IdCardNo,model.Phone);
            if (realNameResp!=null&& realNameResp.Data != null && realNameResp.Data.State == 1)
            {
                model.ID = Guid.NewGuid();
                model.Status = Model.Enums.ActionPasswordStatusEnumType.待审核;
                model.CreateTime = DateTime.Now;
                model.SmsCode = Tool.GetRandomNumber(6);
                model.IsEnable = true;
                model.UserID = CurrentUser.UserID;


                var encryptext = AesGcmHelper.Encrypt(model.Pwd, Convert.FromBase64String(_aesOption.Key));
                model.Pwd = encryptext;
                await _actionPasswordService.Add(model);
            }
            else
            {
                throw new ErrorCodeException(-1, "实名认证失败");
            }
            return Ok();
        }


        [HttpPut]
        public async Task<IActionResult> AuditSuc([FromForm]Guid id)
        {

            if (CurrentUser.UserID>1)
            {
                throw new ErrorCodeException(-1, "无审核权限，请联系管理员处理");
            }
            var model = await _actionPasswordService.Get(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            model.IsEnable = true;
            model.Status = Model.Enums.ActionPasswordStatusEnumType.审核通过;
           
            await _actionPasswordService.Update(model);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> AuditFail([FromForm]Guid id)
        {

            if (CurrentUser.UserID > 1)
            {
                throw new ErrorCodeException(-1, "无审核权限，请联系管理员处理");
            }
            var model = await _actionPasswordService.Get(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            model.IsEnable = true;
            model.Status = Model.Enums.ActionPasswordStatusEnumType.审核退回;
            await _actionPasswordService.Update(model);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Get(Guid id)
        {
            var model = await _actionPasswordService.Get(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return Ok(model);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] Guid id)
        {
            await _actionPasswordService.Delete(id);
            return Ok();
        }

    }
}
