using LZ.Basic.Service.Interface;
using LZ.Core;
using LZ.Core.Attributes;
using LZ.Model.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using SqlSugar;
using LZ.Core.Interface;
using System.Data;

using LZ.Core.Redis;
using System.Text;
using System.IO;
using Consul;
using LZ.Core.Services;
using NPOI.SS.Formula.Functions;
using LZ.Model;
using System;
using LZ.Model.PagedModel;
using LZ.Model.Enums;
using LZ.Model.ModelExtensions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NPOI.POIFS.FileSystem;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using SixLabors.ImageSharp.Drawing;
namespace LZ.Basic.API.Controllers
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.LZMini)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    public class InsuranceEmployeeController(IInsuranceEmployeeService insuranceEmployeeService, IRegionAddressService regionAddressService, IDictionaryService dictionaryService, IInsuranceSupplierService insuranceSupplierService, IHttpService httpService,IInsuranceSchemeService insuranceSchemeService,IAuthService authService) : BasicControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] BasicPagedModel query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Live_V_InsuranceEmployee>();
            if (user.PlatformType == Model.Enums.PlatformType.Admin || user.PlatformType == Model.Enums.PlatformType.Api)
            {
                expable.AndIF(query.CompanyID.HasValue && query.CompanyID > 0, it => it.CompanyID == query.CompanyID);
            }
            else
            {
                //expable.And(it => it.CompanyID == CurrentUser.CompanyID);
                if (user.PlatformType == Model.Enums.PlatformType.MiniAdmin)
                {
                    var info = await authService.UserInfo<Model.SYS_CompanyUser>();
                    if (info.UserType == 2)
                    {
                        expable.And(t => t.PartnerID == info.PartnerID);
                    }
                }
            }
            if (!string.IsNullOrEmpty(query.Name))
            {
                expable.And(t => t.Name.Contains(query.Name));
            }
            if (!string.IsNullOrEmpty(query.UserName))
            {
                expable.And(t => t.CustomerName.Contains(query.UserName));
            }
            if (!string.IsNullOrEmpty(query.CardID))
            {
                expable.And(t => t.IdcardNo.Contains(query.CardID));
            }
            if (query.SID.HasValue)
            {
                expable.And(t => t.YlSupplierIdSheBao==query.SID);
            }
            if (query.GID.HasValue)
            {
                expable.And(t => t.YlSupplierIdGongJiJin == query.GID);
            }
            
            if (query.Main.HasValue)
            {
                expable.And(t => t.IsCustomize == query.Main);
            }
            expable.AndIF(query.CID.HasValue, t => t.YlSchemeId == query.CID);
            expable.AndIF(query.Status.HasValue, t => t.ProcessStatus == query.Status);
            var exp = expable.ToExpression();
            var models = await insuranceEmployeeService.ListView(exp, query, "CompanyID desc,ID desc");
            return Ok(new { items = models, count = query.Count });
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Model.Live_InsuranceEmployee model)
        {
            try
            {
                var user = CurrentUser;
                string msg = "";
                Guid? cgid = Guid.Parse("00000000-0000-0000-0000-000000000000");
                int cid = 0;
                var expable = Expressionable.Create<Model.Live_InsuranceEmployee>();
                if (user.PlatformType == Model.Enums.PlatformType.LZMini || user.PlatformType == Model.Enums.PlatformType.MiniAdmin)
                {
                    cgid = user.CompanyGID;
                    cid = user.CompanyID;
                    var info = await authService.UserInfo<Model.SYS_CompanyUser>();
                    if (info.UserType == 2)
                    {
                        model.PartnerID = info.PartnerID;
                    }
                }
                expable.And(t => t.CompanyID == cid);
                expable.And(t => t.IdcardNo == model.IdcardNo);
                expable.And(t => t.EmployeeCompanyId == model.EmployeeCompanyId);
                expable.And(t => t.ProcessStatus != 5 && t.ProcessStatus != 6 && t.ProcessStatus != 8);
                if (await insuranceEmployeeService.Exist(expable.ToExpression()))
                {
                    throw new ErrorCodeException(-1, "该员工已添加!");
                }
               
                model.CreateTime = DateTime.Now;
                model.EmployeeGID = Guid.NewGuid();
                model.ProcessStatus = 1;
                var index = await insuranceEmployeeService.Add(model);

                return Ok();
            }
            catch (Exception e)
            {
                Common.StaticLog.Error(e.ToString());
                throw new ErrorCodeException(-1, e.ToString());
            }
        }



        //[HttpGet]
        //public async Task<IActionResult> AAA([FromQuery] int id)
        //{
        //    try
        //    {
        //        string msg = "";
        //        var model = await insuranceEmployeeService.Get(id);
        //        List<Model.EntrySocial.UpdateEntrySocialModel> list = new List<Model.EntrySocial.UpdateEntrySocialModel>();
        //        Model.EntrySocial.UpdateEntrySocialModel item = new Model.EntrySocial.UpdateEntrySocialModel();
        //        item.SocialID = model.ID;
        //        item.CardID = model.IdcardNo;
        //        item.CustomerID = model.EmployeeCompanyId;
        //        item.Status = model.ProcessStatus;
        //        list.Add(item);
        //        var datajson = JsonConvert.SerializeObject(list);
        //        var obj = await httpService.PostModelAsync(ServiceNames.UsersService, "/api/entry/SetSocial", datajson);
        //        List<Model.Live_ContractFileConfig> contracts = new List<Live_ContractFileConfig>();
        //        if (!string.IsNullOrEmpty(obj.Data.ToString()))
        //        {
        //            var ii = JsonConvert.DeserializeObject(obj.Data.ToString()) as JObject;
        //            msg = ii["message"].ToString();
        //        }
        //        return Ok(new { message = msg });
        //    }
        //    catch (Exception e)
        //    {
        //        Common.StaticLog.Error(e.ToString());
        //        throw new ErrorCodeException(-1, e.ToString());
        //    }
        //}










        [HttpGet]
        public async Task<Model.Live_InsuranceEmployee> Get(int id)
        {
            return await insuranceEmployeeService.Get(id);
            //return await userService.Get(CurrentUser.UserID);
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_InsuranceEmployee model)
        {
            string msg = "";
            try
            {
                if (!await insuranceEmployeeService.ExistID(model.ID))
                {
                    throw new ErrorCodeException(-1, "你要修改的数据不存在");
                }
                if (model.ProcessStatus == 1)
                {
                    model.StartDateGongJiJin = null;
                    model.StartDateSheBao = null;
                    model.EndDateGongJiJin = null;
                    model.EndDateSheBao = null;
                }
                await insuranceEmployeeService.Update(model);

                Model.EntrySocial.UpdateEntrySocialModel item = new Model.EntrySocial.UpdateEntrySocialModel();
                item.SocialID = model.ID;
                item.CardID = model.IdcardNo;
                item.CustomerID = model.EmployeeCompanyId;
                item.Status = model.ProcessStatus;
                var datajson = JsonConvert.SerializeObject(item);
                if (model.ProcessStatus == 6)
                {
                    var obj = await httpService.PostModelAsync(ServiceNames.UsersService, "/api/entry/CancelSocial", datajson);
                    if (!string.IsNullOrEmpty(obj.Data.ToString()))
                    {
                        var ii = JsonConvert.DeserializeObject(obj.Data.ToString()) as JObject;
                        msg = ii["message"].ToString();
                    }
                }
                else
                {
                    var obj = await httpService.PostModelAsync(ServiceNames.UsersService, "/api/entry/UpdateSocial", datajson);
                    if (!string.IsNullOrEmpty(obj.Data.ToString()))
                    {
                        var ii = JsonConvert.DeserializeObject(obj.Data.ToString()) as JObject;
                        msg = ii["message"].ToString();
                    }
                }


            }
            catch(Exception ex)
            {
                throw new ErrorCodeException(-1, ex.ToString());
            }


            return Ok(new {message=msg});
        }
        [HttpPut]
        public async Task<IActionResult> ChangeProcessStatus([FromBody] Model.Live_InsuranceEmployee model)
        {
            if (!await insuranceEmployeeService.ExistID(model.ID))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            if(model.ProcessStatus==1)
            {
                model.StartDateGongJiJin = null;
                model.StartDateSheBao = null;
                model.EndDateGongJiJin = null;
                model.EndDateSheBao = null;
            }
            await insuranceEmployeeService.Update(model);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> AddEmployeeSocial([FromForm] Model.Live_InsuranceEmployee model, [FromForm] int entryID)
        {
            try
            {
                string msg = "";
                model.CreateTime = DateTime.Now;
                model.EmployeeGID = Guid.NewGuid();
                var info = await authService.UserInfo<Model.Live_Personnel>();
                model.PartnerID = info.PartnerID;
                var index = await insuranceEmployeeService.Add(model);

                Model.EntrySocial.UpdateEntrySocialModel item = new Model.EntrySocial.UpdateEntrySocialModel();
                item.SocialID = index;
                item.CardID = model.IdcardNo;
                item.CustomerID = model.EmployeeCompanyId;
                item.Status = model.ProcessStatus;
                item.SocialGID = model.EmployeeGID;
                item.entryID = entryID;
                var datajson = JsonConvert.SerializeObject(item);
                var obj = await httpService.PostModelAsync(ServiceNames.UsersService, "/api/entry/UpdateSocial", datajson);
                if (!string.IsNullOrEmpty(obj.Data.ToString()))
                {
                    var ii = JsonConvert.DeserializeObject(obj.Data.ToString()) as JObject;
                    msg = ii["message"].ToString();
                }
                return Ok(new { message = msg });
            }
            catch (Exception e)
            {
                Common.StaticLog.Error(e.ToString());
                throw new ErrorCodeException(-1, e.ToString());
            }
        }


        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            string msg = "";
            try
            {
                if (!await insuranceEmployeeService.ExistID(id))
                {
                    throw new ErrorCodeException(-1, "你要删除的数据不存在");
                }
                Model.EntrySocial.UpdateEntrySocialModel item = new Model.EntrySocial.UpdateEntrySocialModel();
                item.SocialID = id;
                var datajson = JsonConvert.SerializeObject(item);
                var obj = await httpService.PostModelAsync(ServiceNames.UsersService, "/api/entry/CancelSocial", datajson);
                if (!string.IsNullOrEmpty(obj.Data.ToString()))
                {
                    var ii = JsonConvert.DeserializeObject(obj.Data.ToString()) as JObject;
                    msg = ii["message"].ToString();
                }
            }
            catch (Exception e)
            {
                Common.StaticLog.Error(e.ToString());
                throw new ErrorCodeException(-1, e.ToString());
            }
            await insuranceEmployeeService.Delete(id);
            return Ok(new { message = msg });
        }

        [HttpGet]
        public IActionResult CreateDefaultExcel()
        {
            var result = insuranceEmployeeService.CreateDefaultExcel();
            string base64String = Convert.ToBase64String(result);
            return Ok(new { base64String, fileName = "社保员工记录模板", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }

        [HttpGet]
        public async Task<IActionResult> Export([FromQuery] BasicPagedModel query)
        {
            query.Page = 0;
            var expable = Expressionable.Create<Model.Live_V_InsuranceEmployee>();
            if (CurrentUser.PlatformType == Model.Enums.PlatformType.Admin || CurrentUser.PlatformType == Model.Enums.PlatformType.Api)
            {
                expable.AndIF(query.CompanyID.HasValue && query.CompanyID > 0, it => it.CompanyID == query.CompanyID);
            }
            else
            {
                expable.And(it => it.CompanyID == CurrentUser.CompanyID);
            }



            //expable.And(it => it.StartDateGongJiJin == null && it.StartDateSheBao == null);
            expable.AndIF(query.Year.HasValue, t => t.CreateTime.HasValue && t.CreateTime.Value.Year == query.Year);
            expable.AndIF(query.Month.HasValue, t => t.CreateTime.HasValue && t.CreateTime.Value.Month == query.Month);
            expable.AndIF(query.CID.HasValue, t => t.YlSchemeId == query.CID);
            expable.AndIF(query.Status.HasValue, t => t.ProcessStatus == query.Status);
            var exp = expable.ToExpression();
            var models = await insuranceEmployeeService.ListView(exp, query, "CreateTime desc");

            var result = insuranceEmployeeService.Export(models);
            string base64String = Convert.ToBase64String(result);
            return Ok(new { base64String, fileName = "社保员工记录", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }

        [HttpPost]
        public async Task<IActionResult> Import()
        {
            List<string> data = new List<string>();
            Guid? cgid = Guid.Parse("00000000-0000-0000-0000-000000000000");
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
                    var paras = new Dictionary<string, string>();
                    paras.Add("page", "0");
                    //paras.Add("AppID", "");
                    var obj = await httpService.GetAsync(ServiceNames.CustomerService, "/api/customer/list", paras);
                    List<Model.Live_Customer> customers = new List<Live_Customer>();
                    if (!string.IsNullOrEmpty(obj.Data.ToString()))
                    {
                        var ii = JsonConvert.DeserializeObject(obj.Data.ToString()) as JObject;
                        customers = JsonConvert.DeserializeObject<List<Model.Live_Customer>>(ii["items"].ToString());
                    }
                    
                    List<Model.Live_InsuranceEmployee> list = new List<Model.Live_InsuranceEmployee>();
                    DataSet? ds = new DataSet();
                    ds = Common.ExcelHeler.ReadExcelToDataSet(fn, "", 2);
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        var rexpable = Expressionable.Create<Model.Live_RegionAddress>();
                        var regions = await regionAddressService.List(rexpable.ToExpression());

                        var dexpable = Expressionable.Create<Model.SYS_Dictionary>();
                        var gid = Guid.Parse("ecf62977-c49f-4ea3-84bb-2c541aef4805");//增减规则
                        var gid1 = Guid.Parse("c110254c-03a9-4511-9c7e-586464008a6b");//方案类型
                        dexpable.And(t => t.PID == gid || t.PID == gid1);
                        PagedModelBase query = new Model.PagedModel.PagedModelBase();
                        query.Page = 0;
                        var dics = await dictionaryService.List(dexpable.ToExpression(), query);
                        BasicPagedModel squery = new Model.PagedModel.BasicPagedModel();
                        squery.Page = 0;
                        var supplieer_expable = Expressionable.Create<Model.Live_V_InsuranceSupplier>();
                        supplieer_expable.And(t => t.Enable == true);
                        supplieer_expable.AndIF(cid > 0, t => t.CompanyID == cid);
                        var suppliers = await insuranceSupplierService.List(supplieer_expable.ToExpression(), squery);

                        var schmem_expable = Expressionable.Create<Model.Live_InsuranceScheme>();
                        schmem_expable.And(t => t.Enable == true);

                        schmem_expable.AndIF(cid > 0, t => t.CompanyID == cid);
                        var schmems = await insuranceSchemeService.List(schmem_expable.ToExpression(), squery);

                        List<Live_InsuranceEmployee> updatelist = new List<Live_InsuranceEmployee>();

                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            Model.Live_InsuranceEmployee model;
                            var expable = Expressionable.Create<Model.Live_InsuranceEmployee>();
                            bool isExist = false;
                            expable.AndIF(cid > 0, t => t.CompanyID == cid);
                            var nn = row[0].ToString();
                            expable.And(t => t.Name == row[0].ToString());
                            model = await insuranceEmployeeService.Get(expable.ToExpression());
                            if (model!=null)
                            {
                                continue;
                               // isExist = true;
                               // int index = 4; 
                               // var shebao = row[2] != null ? row[2].ToString() : "";
                               // var gongjijin = row[3] != null ? row[2].ToString() : "";
                               // char characterToAdd = '-';
                               // if(shebao!="/")
                               // {
                               //     StringBuilder sb = new StringBuilder(shebao);
                               //     sb.Insert(index, characterToAdd);
                               //     model.StartDateSheBao = Convert.ToDateTime(sb.ToString() + "-01");
                               // }
                               //if(gongjijin!="/")
                               // {
                               //     StringBuilder ggj = new StringBuilder(shebao);
                               //     ggj.Insert(index, characterToAdd);
                               //     model.StartDateSheBao = Convert.ToDateTime(ggj.ToString() + "-01");
                               // }

                            }

                            else
                            {
                                try
                                {
                                    model = new Model.Live_InsuranceEmployee();
                                    if(CurrentUser.PlatformType == Model.Enums.PlatformType.MiniAdmin)
                                    {
                                        var info = await authService.UserInfo<Model.SYS_CompanyUser>();
                                        if(info.PartnerID.HasValue)
                                        {
                                            model.PartnerID = info.PartnerID;
                                        }
                                    }
                                    model.CompanyID = cid;
                                    model.CompanyGID = cgid;
                                    model.CreateTime = DateTime.Now;
                                    model.Enable = true;
                                    model.ProcessStatus = 1;
                                    model.Name = row[0] != null ? row[0].ToString() : "";
                                    
                                }


                                catch (Exception ex)
                                {
                                    throw new ErrorCodeException(-1, ex.ToString());
                                }
                            }
                            model.IdcardNo = row[1] != null ? row[1].ToString() : "";
                            if (row[2] != null)
                            {
                                var customer = customers.Where(t => t.CustomerName == row[2].ToString()).FirstOrDefault();
                                if (customer != null)
                                {
                                    model.EmployeeCompanyId = customer.CustomerID;
                                    model.EmployeeCompanyGID = customer.CustomerGID;
                                }
                            }
                            model.Sex = row[3] != null ? row[3].ToString() : "";
                            model.Nation = row[4] != null ? row[4].ToString() : "";
                            model.Phone = row[5] != null ? row[5].ToString() : "";
                            model.IdcardAddress = row[6] != null ? row[6].ToString() : "";
                            model.Education = row[7] != null ? row[7].ToString() : "";
                            if (row[8] != null)
                            {
                                var type = dics.Where(t => t.Name == row[8].ToString().Trim() && t.PID == gid1).FirstOrDefault();
                                if (type != null)
                                {
                                    model.ShemeType = Convert.ToInt32(type.NO);
                                }
                            }
                            if (row[9] != null)
                            {
                                var a = row[9].ToString().Trim();
                                var schmem = schmems.Where(t => t.Name == row[9].ToString().Trim()).FirstOrDefault();
                                if (schmem != null)
                                {
                                    model.YlSchemeId = schmem.ID;
                                }
                            }
                            if (row[10] != null)
                            {
                                var region = regions.Where(t => t.Name == row[10].ToString()).FirstOrDefault();
                                if (region != null)
                                {
                                    model.Province = region.Name;
                                    model.ProvinceCode = region.Code;
                                }
                            }
                            if (row[11] != null)
                            {
                                var region = regions.Where(t => t.Name == row[11].ToString()).FirstOrDefault();
                                if (region != null)
                                {
                                    model.City = region.Name;
                                    model.CityCode = region.Code;
                                }
                            }
                            if (row[12] != null)
                            {
                                var dic = dics.Where(t => t.Name == row[12].ToString().Trim() && t.PID == gid).FirstOrDefault();
                                if (dic != null)
                                {
                                    model.YlRule = Convert.ToInt32(dic.NO);
                                }
                            }
                            model.TimeNode = row[13] != null ? Convert.ToInt32(row[13].ToString()) : 0;
                            if (row[14] != null)
                            {
                                var supplier = suppliers.Where(t => t.Name == row[14].ToString().Trim()).FirstOrDefault();
                                if (supplier != null)
                                {
                                    model.YlSupplierIdSheBao = supplier.ID;

                                }
                            }
                            if (row[15] != null)
                            {
                                var supplier = suppliers.Where(t => t.Name == row[15].ToString().Trim()).FirstOrDefault();
                                if (supplier != null)
                                {
                                    model.YlSupplierIdGongJiJin = Convert.ToInt32(supplier.ID);

                                }
                            }
                            if (row[16] != null && !string.IsNullOrEmpty(row[16].ToString()))
                            {
                                model.StartDateSheBao = Convert.ToDateTime(row[16].ToString());
                                if (model.StartDateSheBao < model.CreateTime)
                                {
                                    model.CreateTime = model.StartDateSheBao;
                                }
                                model.ProcessStatus = 3;
                            }
                            if (row[18] != null && !string.IsNullOrEmpty(row[18].ToString()))
                            {
                                model.StartDateGongJiJin = Convert.ToDateTime(row[18].ToString());
                                if (model.StartDateGongJiJin < model.CreateTime)
                                {
                                    model.CreateTime = model.StartDateGongJiJin;
                                }
                                model.ProcessStatus = 3;
                            }


                            model.BaseAmountYangLao = row[20] != null ? Convert.ToDecimal(row[20].ToString()) : 0;
                            model.BaseAmountYiLiao = row[21] != null ? Convert.ToDecimal(row[21].ToString()) : 0;
                            model.BaseAmountShiYe = row[22] != null ? Convert.ToDecimal(row[22].ToString()) : 0;
                            model.BaseAmountShengYu = row[23] != null ? Convert.ToDecimal(row[23].ToString()) : 0;
                            model.BaseAmountGongShang = row[24] != null ? Convert.ToDecimal(row[24].ToString()) : 0;
                            model.BaseAmountCanBao = row[25] != null ? Convert.ToDecimal(row[25].ToString()) : 0;
                            model.BaseAmountGongJiJin = row[26] != null ? Convert.ToDecimal(row[26].ToString()) : 0;
                            model.EmployeePercentYangLao = row[27] != null ? Convert.ToDecimal(row[27].ToString()) : 0;
                            model.EmployeePercentYiLiao = row[28] != null ? Convert.ToDecimal(row[28].ToString()) : 0;
                            model.EmployeePercentShiYe = row[29] != null ? Convert.ToDecimal(row[29].ToString()) : 0;
                            model.EmployeePercentShengYu = row[30] != null ? Convert.ToDecimal(row[30].ToString()) : 0;
                            model.EmployeePercentGongShang = row[31] != null ? Convert.ToDecimal(row[31].ToString()) : 0;
                            model.EmployeeAmountCanBao = row[32] != null ? Convert.ToDecimal(row[32].ToString()) : 0;
                            model.EmployeeAmountDaBingYiLiao = row[33] != null ? Convert.ToDecimal(row[33].ToString()) : 0;
                            model.EmployeeAmountYiLiaoBuChong = row[34] != null ? Convert.ToDecimal(row[34].ToString()) : 0;
                            model.UnitPercentYangLao = row[35] != null ? Convert.ToDecimal(row[35].ToString()) : 0;
                            model.UnitPercentYiLiao = row[36] != null ? Convert.ToDecimal(row[36].ToString()) : 0;
                            model.UnitPercentShiYe = row[37] != null ? Convert.ToDecimal(row[37].ToString()) : 0;
                            model.UnitPercentShengYu = row[38] != null ? Convert.ToDecimal(row[38].ToString()) : 0;
                            model.UnitPercentGongShang = row[39] != null ? Convert.ToDecimal(row[39].ToString()) : 0;

                            model.UnitPercentCanBao = row[40] != null ? Convert.ToDecimal(row[40].ToString()) : 0;
                            model.UnitAmountDaBingYiLiao = row[41] != null ? Convert.ToDecimal(row[41].ToString()) : 0;
                            model.UnitAmountYiLiaoBuChong = row[42] != null ? Convert.ToDecimal(row[42].ToString()) : 0;
                            model.EmployeePercentGongJiJin = row[43] != null ? Convert.ToDecimal(row[43].ToString()) : 0;
                            model.UnitPercentGongJiJin = row[44] != null ? Convert.ToDecimal(row[44].ToString()) : 0;
                            model.BaseDifferenceAmount = row[45] != null ? Convert.ToDecimal(row[45].ToString()) : 0;
                            model.BaseRemark = row[46] != null ? row[46].ToString() : "";

                            list.Add(model);

                        }
                        int i = await insuranceEmployeeService.Adds(list);
                        await insuranceEmployeeService.Update(updatelist);
                        return Ok(new {msg="导入成功"});

                    }

                }


            }
            catch (Exception ex)
            {
                return Ok(new { msg = ex.ToString() });
            }
            return Ok();
        }
    }
}
