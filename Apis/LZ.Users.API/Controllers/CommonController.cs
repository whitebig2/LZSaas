using LZ.Users.Service.Interface;
using LZ.Core;
using LZ.Core.Interface;
using Microsoft.AspNetCore.Mvc;
using LZ.Model.Exceptions;
using SqlSugar;
using LZ.ApiService.Interface;
using LZ.ApiService.Services;
using Microsoft.AspNetCore.Authorization;

namespace LZ.Users.API.Controllers
{
    public class CommonController(IAliyunOSSService aliyunOSSService) : UsersControllerBase
    {
        [HttpPost]
        [DisableRequestSizeLimit]
        [AllowAnonymous]
        public async Task<IActionResult> UploadAvatar()
        {
            var files = Request.Form.Files;
            if (files.Count > 0)
            {
                var res = await aliyunOSSService.UploadImage(files[0], "avatar", 400, 400);
                if (res.Code == 0)
                {
                    return Ok(res.Data);
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

        [HttpPost]
        public async Task<IActionResult> UploadFile()
        {
            var files = Request.Form.Files;
            if (files.Count > 0)
            {
                var res = await aliyunOSSService.UploadFile(files[0]);
                if (res.Code == 0)
                {
                    return Ok(res.Data);
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
