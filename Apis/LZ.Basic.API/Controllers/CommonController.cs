using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Model.Exceptions;
using Microsoft.AspNetCore.Authorization;
using LZ.Core.Attributes;
using LZ.Basic.Service.Interface;
using LZ.ApiService.Interface;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LZ.Basic.API.Controllers
{
    
    public class CommonController(IAliyunOSSService aliyunOSSService) : BasicControllerBase
    {
        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadImage()
        {
            var files = Request.Form.Files;
            if (files.Count > 0)
            {
                var res =await aliyunOSSService.UploadImage(files[0]);
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
        [DisableRequestSizeLimit]
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
        public async Task<IActionResult> UploadImageSpecial(int quality=50)
        {
            var files = Request.Form.Files;
            if (files.Count > 0)
            {
                var res = await aliyunOSSService.UploadImage(files[0], "pictures",1000,300, quality);
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
        public async Task<IActionResult> WangeditorUploadImage(int quality = 50)
        {
            var files = Request.Form.Files;
            if (files.Count > 0)
            {
                var res = await aliyunOSSService.UploadImage(files[0], "pictures", 1000, 300, quality);
                if (res.Code == 0)
                {
                    
                    var data = new { url = "http://oss.cn-labor.cn" + res.Data };
                    var result = new { errno = 0, data };
                    JsonResult json = new JsonResult(result);
                    return json;
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
