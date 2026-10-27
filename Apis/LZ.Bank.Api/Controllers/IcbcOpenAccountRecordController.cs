using Consul;
using LZ.ApiService.Interface;
using LZ.Bank.Service.Interface;
using LZ.Common;
using LZ.Core;
using LZ.Model;
using LZ.Model.Dtos;
using LZ.Model.Exceptions;
using LZ.Model.ICBC.Plkh;
using LZ.Model.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
using System.IO;
using System.IO.Compression;
using Renci.SshNet;
using LZ.ApiService.Services;
using NPOI.Util;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using LZ.Model.PagedModel;
using LZ.Core.Services;
using SqlSugar;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using LZ.Model.Enums;
using OfficeOpenXml.Style.XmlAccess;

namespace LZ.Bank.Api.Controllers
{
    [AllowAnonymous]
    public class IcbcOpenAccountRecordController : BankControllerBase
    {

        private readonly ILogger<IcbcOpenAccountRecordController> _logger;
        private readonly IIcbcOpenAccountRecordService _service;
        private readonly IcbcOpenCardOption _options;
        private readonly IAliyunOSSService _oSSService;
        private readonly IBankIcbcService _bankIcbcService;
        private readonly IWebHostEnvironment _env;
        private readonly BankServiceOption _bsOptions;

        public IcbcOpenAccountRecordController(ILogger<IcbcOpenAccountRecordController> logger, IIcbcOpenAccountRecordService service, IOptions<IcbcOpenCardOption> options, IAliyunOSSService oSSService, IBankIcbcService bankIcbcService, IWebHostEnvironment env, IOptions<BankServiceOption> bsOptions)
        {
            _logger = logger;
            _service = service;
            _options = options.Value;
            _bsOptions = bsOptions.Value;
            _oSSService = oSSService;
            _bankIcbcService = bankIcbcService;
            _env = env;

        }

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] UserPagedModel query, string BatchNo)
        {
            var expable = Expressionable.Create<Model.Live_IcbcOpenAccountRecord>();
            if (query.Status.HasValue)
            {
                expable.And(t => t.Status == (OpenAccountProcessStatusEnumType)query.Status);
            }

            expable.AndIF(!string.IsNullOrEmpty(query.Name), t => t.Name.Contains(query.Name));
            expable.AndIF(!string.IsNullOrEmpty(query.CardID), t => t.CardID.Contains(query.CardID));
            expable.AndIF(!string.IsNullOrEmpty(BatchNo), t => t.BatchNo.ToString().Contains(BatchNo));
            var exp = expable.ToExpression();
            var models = await _service.PageList(exp, query);
            return Ok(new { items = models, count = query.Count });
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("suc");
        }

        [HttpGet("download-zip")]
        public async Task<IActionResult> DownloadZip()
        {

            var openAccountRecords = await _service.List(f => !string.IsNullOrWhiteSpace(f.Name));
            var zipMs = new MemoryStream();
            // 创建 ZipArchive，模式为 Create，leaveOpen: true 确保流在 using 结束后不被关闭
            using (var archive = new ZipArchive(zipMs, ZipArchiveMode.Create, leaveOpen: true))
            {
                foreach (var item in openAccountRecords)
                {
                    string frontFileName = Path.GetFileName(item.CardFrontPath);
                    string backFileName = Path.GetFileName(item.CardBackPath);
                    var frontEntry = archive.CreateEntry($"{item.CardID}/{frontFileName}");
                    using var forntStream = new MemoryStream();
                    var frontOssRes = await _oSSService.GetObject(item.CardFrontPath.TrimStart('/'), forntStream);
                    if (frontOssRes)
                    {
                        using (var entryStream = frontEntry.Open())
                        {
                            forntStream.Position = 0;
                            await forntStream.CopyToAsync(entryStream);
                        }
                    }
                    else
                    {
                        throw new ErrorCodeException(-1, "获取水印图片失败！");
                    }

                    var backEntry = archive.CreateEntry($"{item.CardID}/{backFileName}");
                    using var backStream = new MemoryStream();
                    var backOssRes = await _oSSService.GetObject(item.CardBackPath.TrimStart('/'), backStream);
                    if (backOssRes)
                    {
                        using (var entryStream = backEntry.Open())
                        {
                            backStream.Position = 0;
                            await backStream.CopyToAsync(entryStream);
                        }
                    }
                    else
                    {
                        throw new ErrorCodeException(-1, "获取水印图片失败！");
                    }
                }
            }

            // 重置流位置到开头
            zipMs.Seek(0, SeekOrigin.Begin);

            // 返回文件结果
            return File(zipMs, "application/zip", "archive.zip");
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Apply(List<OpenAccountDto>? records)
        {
            try
            {
                if (records == null)
                {
                    throw new ErrorCodeException(-1, "参数错误");
                }
                if (records.Count == 0)
                {
                    throw new ErrorCodeException(-1, "申请数量为0");
                }

                var dt = DateTime.Now;
                Live_IcbcOpenAccountRecord openAccountRecord = null;
                List<Live_IcbcOpenAccountRecord> openAccountRecords = new List<Live_IcbcOpenAccountRecord>();
                foreach (var item in records)
                {
                    var isExist = await _service.Exist(f => f.CardID == item.CardID);
                    if (!isExist)
                    {
                        openAccountRecord = new Live_IcbcOpenAccountRecord()
                        {
                            ID = Guid.NewGuid(),
                            Name = item.Name,
                            Phone = item.Phone,
                            Sex = item.Sex,
                            BirthDate = item.BirthDate,
                            CardID = item.CardID,
                            Address = item.Address,
                            BatchNo = item.BatchNo,
                            BusinessID = item.BusinessID,
                            CardEndDate = item.CardEndDate,
                            CardStartDate = item.CardStartDate,
                            CardType = "身份证",
                            CreateTime = DateTime.Now,
                            CustomerName = item.CustomerName,
                            FilePath = "/plkh/",
                            ExcelFileName = string.Empty,
                            OpenBrno = _options.openBrno,
                            OpenZono = _options.OpenZono,
                            ZipFileName = string.Empty,
                            ResCode = string.Empty,
                            ResMsg = string.Empty,
                            Status = 0,
                            SendDate = dt.Date,
                            CardBackPath = item.CardBackPath,
                            CardFrontPath = item.CardFrontPath
                        };
                        openAccountRecords.Add(openAccountRecord);
                    }
                    else
                    {

                        throw new ErrorCodeException(-1, $"开户失败，{item.CardID}开户信息已存在！");
                    }
                }
                var excelData = openAccountRecords.Select(f => new PlkhExcelRecord()
                {
                    ADDRESS = f.Address,
                    BIRTHDATE = f.BirthDate.Value.ToString("yyyy-MM-dd"),
                    CHNSNAME = f.Name,
                    LGEPIRD = f.CardEndDate?.ToString("yyyy-MM-dd"),
                    LGLDOCNO = f.CardID,
                    LGLDOCTP = "0000",
                    NOTES4 = f.CardStartDate?.ToString("yyyy-MM-dd"),
                    OPENBRNO = f.OpenBrno,
                    OPENZONO = f.OpenZono,
                    PHONE1 = f.Phone,
                    SENDATE = f.SendDate.Value.ToString("yyyyMMdd"),
                    SEX = f.Sex == "男" ? "0001" : "0002",
                }).ToList();
                byte[] excelBytes;
                try
                {
                    excelBytes = NpoiExcelHelper.WriteExcel<PlkhExcelRecord>(excelData, "批量开户");
                    //using MemoryStream excelMs = new MemoryStream(excelBytes);
                    string excelFileName = $"xfz_kc_{dt.ToString("yyyyMMdd_HHmmss")}.xlsx";
                    //string excelFullPath = Path.Combine(_env.WebRootPath, "BankFile", "ICBC", "Plkh", excelFileName);
                    //using (var excelFileStream = new FileStream(excelFullPath, FileMode.CreateNew))
                    //{
                    //    excelMs.Position = 0;
                    //    excelMs.CopyTo(excelFileStream);
                    //    await excelFileStream.FlushAsync();
                    //}
                    string excelPath = "";
                    using (var client = new SftpClient(_bsOptions.Url, _bsOptions.Port, _bsOptions.UserName, _bsOptions.MiMa))
                    {
                        client.Connect();
                        using (MemoryStream excelMs = new MemoryStream(excelBytes))
                        {
                            //var fileInfo = new FileInfo(excelFullPath);
                            //var stream = fileInfo.OpenRead();
                            //var formFile = new FormFile(stream, 0, stream.Length, fileInfo.Name, fileInfo.Name);
                            //formFile.CopyTo(ms);
                            excelPath = $"/plkh/{excelFileName}";
                            client.UploadFile(excelMs, excelPath);
                        }
                    }
                    foreach (var item in openAccountRecords)
                    {
                        item.ExcelFileName = excelFileName;
                    }
                }
                catch (Exception e)
                {
                    throw new ErrorCodeException(-1, "报表错误" + _bsOptions.Url + "|" + _bsOptions.Port + "|" + _bsOptions.UserName + "|" + _bsOptions.MiMa);
                }


                //var excelRes = await _oSSService.UploadOssFile(excelMs, $"BankFile/ICBC/Plkh/{excelFileName}");
                //if (excelRes.Code == 0)
                //{
                //    foreach (var item in openAccountRecords)
                //    {
                //        item.ExcelFileName = excelFileName;
                //        item.FilePath = "/BankFile/ICBC/Plkh/";
                //    }
                //}



                var zipMs = new MemoryStream();
                // 创建 ZipArchive，模式为 Create，leaveOpen: true 确保流在 using 结束后不被关闭
                using (var archive = new ZipArchive(zipMs, ZipArchiveMode.Create, leaveOpen: true))
                {
                    foreach (var item in openAccountRecords)
                    {
                        string frontFileName = Path.GetFileName(item.CardFrontPath);
                        string backFileName = Path.GetFileName(item.CardBackPath);
                        var frontEntry = archive.CreateEntry($"{item.CardID}/{frontFileName}");
                        using var forntStream = new MemoryStream();
                        var frontOssRes = await _oSSService.GetObject(item.CardFrontPath.TrimStart('/'), forntStream);
                        if (frontOssRes)
                        {
                            using (var entryStream = frontEntry.Open())
                            {
                                forntStream.Position = 0;
                                await forntStream.CopyToAsync(entryStream);
                            }
                        }
                        else
                        {
                            throw new ErrorCodeException(-1, "获取水印图片失败！");
                        }
                        var backEntry = archive.CreateEntry($"{item.CardID}/{backFileName}");
                        using var backStream = new MemoryStream();
                        var backOssRes = await _oSSService.GetObject(item.CardBackPath.TrimStart('/'), backStream);
                        if (backOssRes)
                        {
                            using (var entryStream = backEntry.Open())
                            {
                                backStream.Position = 0;
                                await backStream.CopyToAsync(entryStream);
                            }
                        }
                        else
                        {
                            throw new ErrorCodeException(-1, "获取水印图片失败！");
                        }
                    }
                }
                // 重置流位置到开头
                zipMs.Seek(0, SeekOrigin.Begin);
                string zipFileName = $"xfz_kc_{dt.ToString("yyyyMMdd_HHmmss")}.zip";
                //string zipFullPath = Path.Combine(_env.WebRootPath, "BankFile", "ICBC", "Plkh", zipFileName);

                using (var client = new SftpClient(_bsOptions.Url, _bsOptions.Port, _bsOptions.UserName, _bsOptions.MiMa))
                {
                    client.Connect();
                    zipMs.Position = 0;
                    var excelPath = $"/plkh/{zipFileName}";
                    client.UploadFile(zipMs, excelPath);
                }
                foreach (var item in openAccountRecords)
                {
                    item.ZipFileName = zipFileName;
                }



                //using (var zipFileStream = new FileStream(zipFullPath, FileMode.Create))
                //{
                //    throw new ErrorCodeException(-1, "压缩包失败");
                //}






                //var imgresult = await _bankIcbcService.OpenAccountUploadImageApplyAsync(zipFullPath, zipFileName, DateTime.Now);


                //var aipRes = await _oSSService.UploadOssFile(zipMs, $"BankFile/ICBC/Plkh/{zipFileName}");
                //if (aipRes.Code == 0)
                //{
                //    foreach (var item in openAccountRecords)
                //    {
                //        item.ZipFileName = zipFileName;
                //        item.FilePath = "/BankFile/ICBC/Plkh/";
                //    }
                //}


                await _service.Add(openAccountRecords);
                return Ok("suc");
            }
            catch (Exception ex)
            {
                throw new ErrorCodeException(-1, ex.Message);

            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Test()
        {
            
            var model = await _service.Get(t => t.ID == Guid.Parse("7374944b-400a-4a71-b470-db16d7baef30"));
            if (model != null)
            {

                var result = await _bankIcbcService.OpenAccountApplyAsync(model.FilePath, model.ExcelFileName);
                Console.WriteLine("报表开卡申请返回结果：" + JsonConvert.SerializeObject(result));
                if (result.Retcode == "0000")
                {
                    var imgresult = await _bankIcbcService.OpenAccountUploadImageApplyAsync(model.FilePath, model.ZipFileName, DateTime.Now);
                    Console.WriteLine("身份证开卡申请返回结果：" + JsonConvert.SerializeObject(imgresult));
                }

                return Ok(new { model });
            }

            return Ok();
            //var imgresult = await _bankIcbcService.OpenAccountUploadImageApplyAsync(zipFullPath, zipFileName, DateTime.Now);

        }

    }
}
