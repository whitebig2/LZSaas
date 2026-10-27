using LZ.Company.Service.Interface;
using LZ.Core;
using LZ.Core.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using LZ.Company.Service.Services;
using LZ.Model.Exceptions;
using SqlSugar;
using LZ.Core.Attributes;
using LZ.ApiService.Services;
using LZ.ApiService.Interface;
using LZ.Model.PagedModel;
using LZ.Company.API;

namespace LZ.Basic.API.Controllers
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    public class CompanyUserController(ICompanyUserService companyUserService, IPermissionService permissionService,IAuthService authService, IAliyunOSSService aliyunOSSService,ICompanyConfigService companyConfigService) : CompanyControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] UserPagedModel query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.SYS_CompanyUser>();
            if (!string.IsNullOrEmpty(query.UserName))
            {
                expable.And(it => it.UserName.Contains(query.UserName));
            }
            if (!string.IsNullOrEmpty(query.Name))
            {
                expable.And(it => it.FullName.Contains(query.Name));
            }
            if (query.Enabled.HasValue)
            {
                expable.And(it => it.Enabled == query.Enabled);
            }
            if(user.PlatformType== Model.Enums.PlatformType.Admin)
            {
                expable.AndIF(query.CompanyID.HasValue, it => it.CompanyID == user.CompanyID);
            }
            else
            {
                expable.And(it => it.CompanyID == user.CompanyID);
            }
            var exp = expable.ToExpression();
            var models = await companyUserService.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }
        [HttpGet]
        public async Task<IActionResult> Info()
        {
            var cuser = CurrentUser;
            var roles = await permissionService.CompanyRoles(cuser.UserID);
            var user = await companyUserService.Info(cuser.UserID);
            var company = await companyConfigService.Get(cuser.CompanyGID);
            return Ok(new { roles, info = new { name = user.FullName, picture = user.Picture, id = user.UserID,companyID= cuser.CompanyID,companyGID=cuser.CompanyGID,userType=user.UserType, partnerID=user.PartnerID, partnerGID=user.PartnerGID }, company= company.Company, companyConfig= company.Config });
        }
        [HttpPost]
        [OperationLogAction("新增账号: {model.FullName}")]
        public async Task<IActionResult> Add([FromBody] Model.SYS_CompanyUser model)
        {
            var user = CurrentUser;
            if (await companyUserService.ExistName(model.UserName))
            {
                throw new ErrorCodeException(-1, "该用户名已存在!");
            }
            model.UserPassword = Common.Tool.Md5Psw32("111111");
            model.UserGID = Guid.NewGuid();
            model.CreateTime = DateTime.Now;
            model.CompanyID= user.CompanyID;
            model.CompanyGID = user.CompanyGID;
            await companyUserService.Add(model);
            return Ok();
        }
        [HttpGet]
        public async Task<Model.SYS_CompanyUser> Get(int id)
        {
            var model = await companyUserService.Get(id);
            if (model==null)
            {
                throw new ErrorCodeException(-1, "数据不存在");
            }
            return model;
        }
        [HttpPost]
        public async Task<Model.SYS_CompanyUser> GetExpress([FromForm] string name)
        {
            var expable = Expressionable.Create<Model.SYS_CompanyUser>();
            if(!string.IsNullOrEmpty(name))
            {
                expable.And(t => t.FullName == name);
            }
            var model = await companyUserService.Get(expable.ToExpression());
            return model;
        }
        [HttpPut]
        [OperationLogAction("更新账号: {model.FullName}")]
        public async Task<IActionResult> Update([FromBody] Model.SYS_CompanyUser model)
        {
            if (await companyUserService.ExistUserName(model.UserName, model.UserID))
            {
                throw new ErrorCodeException(-1, "该用户名已存在");
            }
            await companyUserService.Update(model);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> Enable([FromForm] int id)
        {
            if (!await companyUserService.Exist(id))
            {
                throw new ErrorCodeException(-1, "你要启用的数据不存在");
            }
            await companyUserService.Set(id, true);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> Disable([FromForm] int id)
        {
            if (!await companyUserService.Exist(id))
            {
                throw new ErrorCodeException(-1, "你要禁用的数据不存在");
            }
            await companyUserService.Set(id, false);
            return Ok();
        }
        [HttpDelete]
        [OperationLogAction("删除账号id: {id}")]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            var user = CurrentUser;
            if (!await companyUserService.Delete(id,user.CompanyID))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            return Ok();
        }
        [HttpPut]
        [OperationLogAction("修改密码")]
        public async Task<IActionResult> ChangePassword([FromForm] string ypassword, [FromForm] string npassword)
        {
            var user = CurrentUser;
            var count = await companyUserService.ChangePassword(CurrentUser.UserID, Common.Tool.Md5Psw32(ypassword), Common.Tool.Md5Psw32(npassword), user.CompanyID);
            if (count == 0)
            {
                throw new ErrorCodeException(-1, "原密码错误");
            }
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> ChangePicture([FromForm] IFormFile file)
        {
            if (file!=null)
            {
                var user = CurrentUser;
                var res =await aliyunOSSService.UploadImage(file);
                if (res.Code == 0)
                {
                    //return Ok(res.Message);
                    var count = await companyUserService.ChangePicture(CurrentUser.UserID, res.Data.ToString(), user.CompanyID);
                    if (count == 0)
                    {
                        throw new ErrorCodeException(-1, "修改错误");
                    }
                    return Ok(res.Data.ToString());
                }
                else
                {
                    throw new ErrorCodeException(-1, res.Message);
                }
            }
            else
            {
                throw new ErrorCodeException(-1, "请选择文件");
            }
        }
    }
}
