using LZ.ApiService.Interface;
using LZ.ApiService.Services;
using LZ.Core.Attributes;
using LZ.Core.Interface;
using LZ.Model.Exceptions;
using LZ.Users.Service.Interface;
using LZ.Users.Service.Services;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace LZ.Users.API.Controllers
{
    public class FileUploadController(IFileUploadService fileUploadService,IAliyunOSSService aliyunOSSService) : UsersControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List(string gid ,[FromQuery] Model.PagedModel.UserPagedModel query)
        {
            var cid = CurrentUser.CompanyID;
            var expable = Expressionable.Create<Model.Live_FileUpload>();
            
            if(query.CID.HasValue)
            {
                expable.And(t => t.FID == query.CID);
            }
            if (!string.IsNullOrEmpty(gid))
            {
                expable.And(t => t.FGID == Guid.Parse(gid));
            }
            if (query.SID.HasValue)
            {
                expable.And(t => t.SID == query.SID);
            }
            if (!string.IsNullOrEmpty(query.Type))
            {
                string[] types = new string[] { };
                types = query.Type.Split(',');
                expable.And(t => types.Contains(t.TypeKey.ToString()));
            }
            var exp = expable.ToExpression();
            var models = await fileUploadService.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Model.Live_FileUpload model)
        {
            int cid = 0;
            var expable = Expressionable.Create<Model.Live_FileUpload>();
            if (CurrentUser.PlatformType != Model.Enums.PlatformType.Admin)
            {
                cid = CurrentUser.CompanyID;
                model.CompanyGID = CurrentUser.CompanyGID;
            }
            model.CompanyID = cid;
            model.CreateTime = DateTime.Now;
            model.UserID = CurrentUser.UserID;
            await fileUploadService.Add(model);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> AddList([FromBody] List<Model.Live_FileUpload> list)
        {
            int cid = 0;
            Guid? gid = Guid.Empty;
            var expable = Expressionable.Create<Model.Live_FileUpload>();
            if (CurrentUser.PlatformType != Model.Enums.PlatformType.Admin)
            {
                cid = CurrentUser.CompanyID;
                gid = CurrentUser.CompanyGID;
            }
            foreach(var x in list)
            {
                x.CompanyID = cid;
                x.CompanyGID = gid;
                x.CreateTime = DateTime.Now;
                x.UserID = CurrentUser.UserID;
            }
            
            await fileUploadService.Add(list);
            return Ok();
        }

        [HttpGet]
        public async Task<Model.Live_FileUpload> Get(int id)
        {
            return await fileUploadService.Get(id);
            //return await userService.Get(CurrentUser.UserID);
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_FileUpload model)
        {
            if (!await fileUploadService.ExistID(model.ID))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            await fileUploadService.Update(model);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            if (!await fileUploadService.ExistID(id))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            await fileUploadService.Delete(id);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> SaveFile([FromQuery] int id)
        {
            var model = await fileUploadService.Get(id);

            byte[] buffer = new byte[1024 * 5];
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(model.Path);
                response.EnsureSuccessStatusCode(); // 确保请求成功

                // 将响应内容作为字节流读取
                byte[] fileBytes = await response.Content.ReadAsByteArrayAsync();

                // 创建一个文件流，用于进一步处理或保存文件
                using (MemoryStream fileStream = new MemoryStream(fileBytes))
                {
                    buffer = fileStream.ToArray();
                }
            }
            string base64String = Convert.ToBase64String(buffer);


            return Ok(new { base64String, fileName = model.FileName, mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }



        [HttpPost]
        public async Task<IActionResult> Upload([FromForm] int type, [FromForm] string value, [FromForm] string length, [FromForm] int fid, [FromForm] Guid? gid, [FromForm] string filename)
        {
            var files = Request.Form.Files;
           
            if (files.Count > 0)
            {
                IFormFile file = files[0];
                var res = await aliyunOSSService.UploadFile(file);
                if (res.Code == 0)
                {
                    string path = "http://oss.cn-labor.cn" + res.Data;
                    Model.Live_FileUpload model = new Model.Live_FileUpload();
                    model.FileName = filename;
                    model.TypeKey = type;
                    model.TypeValue = value;
                    model.FileLength = file.Length.ToString();
                    model.Path = path;
                    if (fid > 0)
                    {
                        model.FID = fid;
                    }
                    if(gid.HasValue)
                    {
                        model.FGID = gid;
                    }
                    model.UserID = CurrentUser.UserID;
                    model.CreateTime = DateTime.Now;
                    //model.CompanyGID = CurrentUser.CompanyGID;
                    model.CompanyID = CurrentUser.CompanyID;
                    await fileUploadService.Add(model);
                    return Ok("上传成功");
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
