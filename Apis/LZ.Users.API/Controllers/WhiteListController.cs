using LZ.ApiService.Interface;
using LZ.Common;
using LZ.Core;
using LZ.Core.Attributes;
using LZ.Core.Interface;
using LZ.Core.Services;
using LZ.Model;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using LZ.Users.Service.Interface;
using LZ.Users.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using NPOI.HSSF.Record;
using NPOI.OpenXmlFormats.Dml;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using LZ.Model.Enums.white;
using LZ.Core.EventData;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Model.Enums;
using SqlSugar.IOC;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NPOI.SS.Formula.Functions;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
using LZ.Model.Dtos;
using Senparc.Weixin.MP.AdvancedAPIs.Card;
using System.ComponentModel.Design;

namespace LZ.Users.API.Controllers
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.LZMini)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    public class WhiteListController(IEntryService entryService,IWhiteListService whiteListService, IFileUploadService fileUploadService, IAuthService authService,IHttpService httpService) : UsersControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] UserPagedModel query)
        {
            var cid = CurrentUser.CompanyID;
            
            var expable = Expressionable.Create<Model.Live_V_WhiteList>();
            if (CurrentUser.PlatformType == Model.Enums.PlatformType.Admin || CurrentUser.PlatformType == Model.Enums.PlatformType.Api)
            {
                expable.AndIF(query.CompanyID.HasValue && query.CompanyID > 0, it => it.CompanyID == query.CompanyID);

            }
            else
            {
                expable.And(it => it.CompanyID == CurrentUser.CompanyID);
                if(CurrentUser.PlatformType == Model.Enums.PlatformType.MiniAdmin)
                {
                    var info = await authService.UserInfo<Model.SYS_CompanyUser>();
                    if (info.UserType == 2)
                    {
                        expable.And(t => t.PartnerID == info.PartnerID);
                    }
                } 
                else if (CurrentUser.PlatformType == Model.Enums.PlatformType.LZMini)
                {
                    var info = await authService.UserInfo<Model.Live_Personnel>();
                    if (!info.PartnerID.HasValue)
                    {
                        throw new ErrorCodeException(-1, "该账号没有关联服务商");
                    }
                    expable.And(t => t.PartnerID == info.PartnerID);
                }
                
            }
            if(!string.IsNullOrEmpty(query.Name))
            {
                expable.And(t => t.Name.Contains(query.Name));
            }
            if (!string.IsNullOrEmpty(query.CardID))
            {
                expable.And(t => t.IdentityCard == query.CardID);
            }
            var exp = expable.ToExpression();
            var models = await whiteListService.ListView(exp, query,"createTime desc");
            return Ok(new { items = models, count = query.Count,list = await authService.UserInfo<Model.Live_Personnel>() });
        }
        [HttpPost]
        public async Task<bool> IsExist([FromBody] Model.Live_User model)
        {
            var expable = Expressionable.Create<Model.Live_WhiteList>();
            expable.And(t => t.CompanyID == model.CompanyID && t.IdentityCard == model.CardID);
            return await whiteListService.Exist(expable.ToExpression());
        }
        [PlatformLimit(Model.Enums.PlatformType.LZMini)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpPost]
        public async Task<IActionResult> Add([FromForm] Model.Live_WhiteList model, [FromForm] List<Model.Live_FileUpload> files)
        {
            var entry = await entryService.View(f => f.CardID == model.IdentityCard);
            if (entry==null)
            {
                throw new ErrorCodeException(-1, "员工不存在");
            }
            //if (entry.UserID == null || entry.UserID.Value <= 0)
            //{
            //    throw new ErrorCodeException(-1, "此员工非线上实名员工，无法签署现金签收单，不符合现金发放条件！");
            //}
            bool isService = false;
            var exp = Expressionable.Create<Model.Live_WhiteList>();
            var exp_entry = Expressionable.Create<Model.Live_UserEntry>();
            if (CurrentUser.PlatformType == Model.Enums.PlatformType.LZMini)
            {
                var info = await authService.UserInfo<Model.Live_Personnel>();
                isService = true;
                model.PartnerID = info.PartnerID;
                exp.And(t => t.PartnerID == info.PartnerID);
                exp_entry.And(t => t.PartnerID == info.PartnerID);
            }
            else if (CurrentUser.PlatformType == Model.Enums.PlatformType.MiniAdmin)
            {
                var info = await authService.UserInfo<Model.SYS_CompanyUser>();
                isService = info.UserType == 2;
                if (isService)
                {
                    exp.And(t => t.PartnerID == info.PartnerID);
                    model.PartnerID = info.PartnerID;
                    exp_entry.And(t => t.PartnerID == info.PartnerID);
                }

            }

            if (model.LabelID == (int)WhiteLabelEnum.无银行卡过渡2个月 && isService)//服务商账号添加标签为其他的标签数量不能超过5个
            {
                int max = 0;
                exp_entry.And(t => t.Status == 2);
                var entryCount = await entryService.UsersCount(exp_entry.ToExpression());
                if(entryCount<=100)
                {
                    max = 5;
                }
                else
                {
                    max = (int)(entryCount * 0.05);
                }
                exp.And(t => t.CompanyID == CurrentUser.CompanyID);
                exp.And(t => t.LabelID == (int)WhiteLabelEnum.无银行卡过渡2个月);

                var count = await whiteListService.Count(exp.ToExpression());
                if (count >= max)
                {
                    throw new ErrorCodeException(-1, "添加人数不能超过"+ max + "个");
                }
            }
            int cid = CurrentUser.CompanyID;
            var expable = Expressionable.Create<Model.Live_WhiteList>();
            expable.And(t => t.CompanyID == cid);
            expable.And(t => t.IdentityCard == model.IdentityCard);
            expable.And(t => t.Name == model.Name);
            //expable.And(t => t.UserID == model.UserID);
            if (await whiteListService.Exist(expable.ToExpression()))
            {
                throw new ErrorCodeException(-1, "该员工已添加!");
            }
            model.EndTime = Convert.ToDateTime("9999-12-31");
            var x = IdCardHelper.GetAgeAndGender(model.IdentityCard);
            if (model.LabelID == (int)WhiteLabelEnum.年龄未满18)
            {
                if (x.Age >= 18)
                {
                    throw new ArgumentException("身份证号识别,已超过18岁----" + x.Age);
                }
                var info = IdCardHelper.GetBirthDate(model.IdentityCard);
                try
                {
                    var date = Convert.ToDateTime(info);
                    model.EndTime = date.AddYears(18).AddMonths(2);
                }
                catch (Exception e)
                {
                    throw new ErrorCodeException(-1, info);
                }
            }
            else if (model.LabelID == (int)WhiteLabelEnum.无银行卡过渡2个月)
            {
                var start = Convert.ToDateTime(DateTime.Today.ToString("yyyy-MM-01"));
                model.EndTime = start.AddMonths(3).AddDays(-1);
            }
            if (model.EndTime.HasValue && model.EndTime.Value.Day == 1)
            {
                model.EndTime = model.EndTime.Value.AddDays(-1);
            }

            model.GID = Guid.NewGuid();
            model.CompanyID = CurrentUser.CompanyID;
            model.CompanyGID = CurrentUser.CompanyGID;
            model.CreateTime = DateTime.Now;
            var index = await whiteListService.Add(model);
            if (model.LabelID == (int)WhiteLabelEnum.失信人员 || model.LabelID == (int)WhiteLabelEnum.领老家失业补助)
            {
                foreach (var file in files)
                {
                    file.CompanyID = model.CompanyID;
                    file.CompanyGID = model.CompanyGID;
                    file.UserID = CurrentUser.UserID;
                    file.FID = index;
                    file.CreateTime = DateTime.Now;
                }
                await fileUploadService.Add(files);
            }
           
            if (entry.UserID>0)
            {
                CashSignRecordDto record = new CashSignRecordDto()
                {
                    Name = entry.Name,
                    CardID = entry.CardID,
                    Sex = entry.Sex,
                    Age = entry.Age ?? 0,
                    Phone = entry.Phone,
                    CompanyID = entry.CompanyID ?? 0,
                    CompanyName = entry.CompanyName,
                    CustomerID = entry.CustomerID.Value,
                    CustomerName = entry.CustomerName,
                    PartnerID = entry.PartnerID ?? 0,
                    PartnerName = entry.PartnerName,
                    SignUnitName = entry.SignName,
                    UserEntryID = entry.ID,
                    UserID = entry.UserID ?? 0,
                    FailMsg = string.Empty,
                    CashType =model.LabelName,
                    Status = Model.Enums.CashSignStatusEnumType.待生成,
                    ExpirationDate = model.EndTime??DateTime.MinValue,
                    SignUnitCompanyNo = entry.SignCompanyNo,
                };
                var res = await httpService.PostAsync<Live_CashSignRecord>(ServiceNames.CustomerService, "/api/CashSignRecord/Add", JsonConvert.SerializeObject(record));
                if (res == null || res.Code != 200)
                {

                    throw new ErrorCodeException(-1, res?.Message);
                }
                
            }


            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var model = await whiteListService.Get(id);
            UserPagedModel query = new UserPagedModel();
            query.Page = 0;
            int key = 0;
            switch (model.LabelID)
            {
                case (int)WhiteLabelEnum.失信人员: key = 20; break;
                case (int)WhiteLabelEnum.领老家失业补助: key = 21; break;
            }
            if (key > 0)
            {
                var exp = Expressionable.Create<Model.Live_FileUpload>();
                if (CurrentUser.PlatformType == Model.Enums.PlatformType.Admin || CurrentUser.PlatformType == Model.Enums.PlatformType.Api)
                {
                    exp.AndIF(query.CompanyID.HasValue && query.CompanyID > 0, it => it.CompanyID == query.CompanyID);
                }
                else
                {
                    exp.And(it => it.CompanyID == CurrentUser.CompanyID);
                }
                exp.And(t => t.FID == id);

                exp.And(t => t.TypeKey == key);
                var files = await fileUploadService.List(exp.ToExpression(), query);
                return Ok(new { model, files });
            }
            else
            {
                return Ok(new { model });
            }
            
            //return await userService.Get(CurrentUser.UserID);
        }


        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var list = await whiteListService.List(t => t.LabelID>0, new UserPagedModel() { Page = 0 });
            var entrys = await entryService.ListView(t => t.Status == 2);
            string msg = "";
            foreach(var x in list)
            {
                string ll ="'330227198103078227','612324196610082534','522422201001232060','52222720071008364X','522631199802020057','532626200905202933','610424198505162615','41092219890430311X','513701199412071322','50022520030825334X','510821200001062118','522601198503156023','610424198505162615','350424199709020919','522701200104085315','522426198709128032','362326200504242733','433127200011130332','513436200508274510','513432198104194711','433127200011130332','513436200508274510','513432198104194711','513433198302103645','513432198201274529'";
                if (ll.Contains(x.IdentityCard))
                {
                    continue;
                }
                var item = entrys.Find(t => t.CardID == x.IdentityCard);

                if(item!=null && x.PartnerID==null)
                {
                    x.PartnerID = item.PartnerID;
                }
                //if(x.LabelID == (int)WhiteLabelEnum.年龄未满18)
                //{
                //    try
                //    {
                //        var info = IdCardHelper.GetBirthDate(x.IdentityCard);
                //        var date = Convert.ToDateTime(info);
                //        x.EndTime = date.AddYears(18).AddMonths(2);
                //    }
                //    catch (Exception e)
                //    {
                //        throw new ErrorCodeException(-1, "身份证转化错误:" + x.IdentityCard);
                //    }
                //}
                //else if (x.LabelID == (int)WhiteLabelEnum.无银行卡过渡2个月)
                //{
                //    DateTime d  = DateTime.Now;
                //    if (x.CreateTime.HasValue)
                //    {
                //        d = x.CreateTime.Value;
                //    }
                //    var start = Convert.ToDateTime(d.ToString("yyyy-MM-01"));
                //    x.EndTime = start.AddMonths(3).AddDays(-1);
                //}
                //else
                //{
                //    x.EndTime = Convert.ToDateTime("9999-12-31");
                //}
                try
                {
                    if (item != null && item.UserID > 0)
                    {
                        CashSignRecordDto record = new CashSignRecordDto()
                        {
                            Name = item.Name,
                            CardID = item.CardID,
                            Sex = item.Sex,
                            Age = item.Age ?? 0,
                            Phone = item.Phone,
                            CompanyID = item.CompanyID ?? 0,
                            CompanyName = item.CompanyName,
                            CustomerID = item.CustomerID.Value,
                            CustomerName = item.CustomerName,
                            PartnerID = item.PartnerID ?? 0,
                            PartnerName = item.PartnerName,
                            SignUnitName = item.SignName,
                            UserEntryID = item.ID,
                            UserID = item.UserID ?? 0,
                            FailMsg = string.Empty,
                            CashType = x.LabelName,
                            Status = Model.Enums.CashSignStatusEnumType.待生成,
                            ExpirationDate = x.EndTime ?? DateTime.MinValue,
                            SignUnitCompanyNo = item.SignCompanyNo,
                        };
                        var res = await httpService.PostAsync<Live_CashSignRecord>(ServiceNames.CustomerService, "/api/CashSignRecord/Add", JsonConvert.SerializeObject(record));
                        if (res == null || res.Code != 200)
                        {

                            throw new ErrorCodeException(-1, res?.Message);
                        }
                    }
                }
                catch(Exception e)
                {
                    msg += x.IdentityCard + ',';
                    continue;
                }


            }
            await whiteListService.Update(list);


            return Ok(new { msg});
        }

        [HttpGet]
        public async Task<IActionResult> CardGet(string cardid,int companyId)
        {
            var model = await whiteListService.Get(it=>it.IdentityCard==cardid && it.CompanyID==companyId);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "不存在");
            }
            return Ok(model);
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromForm] Model.Live_WhiteList model, [FromForm] List<Model.Live_FileUpload> files)
        {
            if (!await whiteListService.ExistID(model.ID))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            await whiteListService.Update(model);
            if(model.LabelID==3)
            {
                await fileUploadService.DeleteFID(model.ID, 20);
                foreach (var file in files)
                {
                    file.CompanyID = model.CompanyID;
                    file.CompanyGID = model.CompanyGID;
                    file.UserID = CurrentUser.UserID;
                    file.FID = model.ID;
                    file.CreateTime = DateTime.Now;
                }
                await fileUploadService.Add(files);

            }

            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            if (!await whiteListService.ExistID(id))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            await whiteListService.Delete(id);
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var models= await whiteListService.List(f => f.ID > 0, new UserPagedModel() { Page = 0 });

            return Ok(models);
        }

        [HttpGet]
        public async Task<IActionResult> ExportExcel([FromQuery] Model.PagedModel.UserPagedModel query)
        {

            var cid = CurrentUser.CompanyID;
            var expable = Expressionable.Create<Model.Live_WhiteList>();

            if (!string.IsNullOrEmpty(query.CustomerName))
            {
                expable.And(t => t.CompanyID == cid);
            }
            var list = await whiteListService.List(expable.ToExpression(), query);
            var result = whiteListService.ExportExcel(list);
            string base64String = Convert.ToBase64String(result);
            return Ok(new { base64String, fileName = "白名单.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }

        [HttpPost]
        public async Task<IActionResult> Import()
        {
            List<string> data = new List<string>();
            Guid? cgid = Guid.Parse("00000000-0000-0000-0000-000000000000");
            List<Live_WhiteList> list = new List<Live_WhiteList>();
            List<string> arr = new List<string>();
            int cid = CurrentUser.CompanyID;
            if (CurrentUser.PlatformType == Model.Enums.PlatformType.LZMini || CurrentUser.PlatformType == Model.Enums.PlatformType.MiniAdmin)
            {
                cgid = CurrentUser.CompanyGID;
            }
            try
            {
                var files = Request.Form.Files;
                string fn = string.Empty;

                var filePath = "/uploadfiles/file/";


                foreach (var file in files)
                {
                    fn = await Common.UploadFile.UpFile(file, filePath, true);
                    if (fn.Contains("error"))
                    {
                        throw new ErrorCodeException(-1, "文件上传错误");
                    }
                }
                if (!string.IsNullOrEmpty(fn))
                {
                    
                    DataSet? ds = new DataSet();
                    ds = Common.ExcelHeler.ReadExcelToDataSet(fn, "", 2);
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        

                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            arr.Add(row[1].ToString());
                            
                        }

                    }
                   
                }
                list = await whiteListService.List(t => arr.Contains(t.IdentityCard), new UserPagedModel() { Page=0});
                if (list.Count > 0)
                {
                    await whiteListService.DeleteList(list);
                }

            }
            catch (Exception ex)
            {
                return Ok(new { msg = ex.ToString() });
            }
            return Ok(new {list,arr});
        }

    }
}
