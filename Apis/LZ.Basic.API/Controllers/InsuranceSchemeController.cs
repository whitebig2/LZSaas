using LZ.Basic.Service.Interface;
using LZ.Basic.Service.Services;
using LZ.Core;
using LZ.Core.Attributes;
using LZ.Model.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using SqlSugar;
using System.Data;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.VisualBasic;
using System.IO;
using Consul;
using LZ.Model.PagedModel;
using NPOI.Util;
using Senparc.Weixin.WxOpen.Entities;

namespace LZ.Basic.API.Controllers
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.LZMini)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    public class InsuranceSchemeController(IInsuranceSchemeService insuranceSchemeService,IRegionAddressService regionAddressService,IDictionaryService dictionaryService,IInsuranceSupplierService insuranceSupplierService,IInsuranceEmployeeService insuranceEmployeeService) : BasicControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] BasicPagedModel query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Live_InsuranceScheme>();
            if (user.PlatformType == Model.Enums.PlatformType.Admin || user.PlatformType == Model.Enums.PlatformType.Api)
            {
                expable.AndIF(query.CompanyID.HasValue && query.CompanyID > 0, it => it.CompanyID == query.CompanyID);
            }
            //else
            //{
            //    expable.And(it => it.CompanyID == CurrentUser.CompanyID);
            //}
            if (query.From=="wx")
            {
                expable.And(t => t.Name.Contains("默认方案"));
            }
            if(!string.IsNullOrEmpty(query.Name))
            {
                expable.And(t => t.Name.Contains(query.Name));
            }
            if (!string.IsNullOrEmpty(query.ProvinceCode))
            {
                expable.And(t => t.ProvinceCode == query.ProvinceCode);
            }
            if (!string.IsNullOrEmpty(query.CityCode))
            {
                expable.And(t => t.CityCode == query.CityCode);
            }
            var exp = expable.ToExpression();
            var models = await insuranceSchemeService.List(exp, query, "CompanyID desc,ID desc");
            return Ok(new { items = models, count = query.Count });
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Model.Live_InsuranceScheme model)
        {
            var user = CurrentUser;
            Guid? cgid = Guid.Parse("00000000-0000-0000-0000-000000000000");
            int cid = 0;
            var expable = Expressionable.Create<Model.Live_InsuranceScheme>();
            if (user.PlatformType == Model.Enums.PlatformType.LZMini || user.PlatformType == Model.Enums.PlatformType.MiniAdmin)
            {
                cgid = user.CompanyGID;
                cid = user.CompanyID;
            }
            expable.And(t => t.CompanyID == cid);
            expable.And(t => t.Name == model.Name);
            if (await insuranceSchemeService.Exist(expable.ToExpression()))
            {
                throw new ErrorCodeException(-1, "该标签已存在!");
            }
            model.CompanyID = cid;
            model.CompanyGID = cgid;
            model.SchemeGID = Guid.NewGuid();
            model.CreateTime = DateTime.Now;
            await insuranceSchemeService.Add(model);
            return Ok();
        }
        [HttpGet]
        public async Task<Model.Live_InsuranceScheme> Get(int id)
        {
            return await insuranceSchemeService.Get(id);
            //return await userService.Get(CurrentUser.UserID);
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromForm] Model.Live_InsuranceScheme model, [FromForm] string type)
        {
            if (!await insuranceSchemeService.ExistID(model.ID))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            await insuranceSchemeService.Update(model);
            var expable = Expressionable.Create<Model.Live_InsuranceEmployee>();
            expable.And(t => t.YlSchemeId == model.ID);
            BasicPagedModel basicPaged = new BasicPagedModel();
            basicPaged.Page = 0;

            try
            {
                var EmployeeDatas = await insuranceEmployeeService.List(expable.ToExpression(), basicPaged);
                foreach (var x in EmployeeDatas)
                {

                    

                    if(type != "gjj")
                    {
                        x.BaseAmountYangLao = Math.Max(model.BaseAmountYangLao, x.BaseAmountYangLao);
                        x.BaseAmountYiLiao = Math.Max(model.BaseAmountYiLiao, x.BaseAmountYiLiao);
                        x.BaseAmountShiYe = Math.Max(model.BaseAmountShiYe, x.BaseAmountShiYe);
                        x.BaseAmountShengYu = Math.Max(model.BaseAmountShengYu, x.BaseAmountShengYu);
                        x.BaseAmountGongShang = Math.Max(model.BaseAmountGongShang, x.BaseAmountGongShang);
                        x.BaseAmountCanBao = Math.Max(model.BaseAmountCanBao, x.BaseAmountCanBao);
                        x.UnitPercentYangLao = Math.Max(model.UnitPercentYangLao, x.UnitPercentYangLao);
                        x.UnitPercentYiLiao = Math.Max(model.UnitPercentYiLiao, x.UnitPercentYiLiao);
                        x.UnitPercentShiYe = Math.Max(model.UnitPercentShiYe, x.UnitPercentShiYe);
                        x.UnitPercentShengYu = Math.Max(model.UnitPercentShengYu, x.UnitPercentShengYu);
                        x.UnitPercentGongShang = Math.Max(model.UnitPercentGongShang, x.UnitPercentGongShang);
                        x.UnitPercentCanBao = Math.Max(model.UnitPercentCanBao, x.UnitPercentCanBao);
                        x.EmployeePercentYangLao = Math.Max(model.EmployeePercentYangLao, x.EmployeePercentYangLao);
                        x.EmployeePercentYiLiao = Math.Max(model.EmployeePercentYiLiao, x.EmployeePercentYiLiao);
                        x.EmployeePercentShiYe = Math.Max(model.EmployeePercentShiYe, x.EmployeePercentShiYe);
                        x.EmployeePercentShengYu = Math.Max(model.EmployeePercentShengYu, x.EmployeePercentShengYu);
                        x.EmployeePercentGongShang = Math.Max(model.EmployeePercentGongShang, x.EmployeePercentGongShang);
                        x.EmployeePercentCanBao = Math.Max(model.EmployeePercentCanBao, x.EmployeePercentCanBao);
                        x.EmployeeAmountCanBao = Math.Max(model.EmployeeAmountCanBao, x.EmployeeAmountCanBao);
                        x.EmployeeAmountDaBingYiLiao = Math.Max(model.EmployeeAmountDaBingYiLiao, x.EmployeeAmountDaBingYiLiao);
                        x.EmployeeAmountYiLiaoBuChong = Math.Max(model.EmployeeAmountYiLiaoBuChong, x.EmployeeAmountYiLiaoBuChong);
                        x.UnitAmountCanBao = Math.Max(model.UnitAmountCanBao, x.UnitAmountCanBao);
                        x.UnitAmountDaBingYiLiao = Math.Max(model.UnitAmountDaBingYiLiao, x.UnitAmountDaBingYiLiao);
                        x.UnitAmountYiLiaoBuChong = Math.Max(model.UnitAmountYiLiaoBuChong, x.UnitAmountYiLiaoBuChong);


                        //x.BaseAmountYangLao = model.BaseAmountYangLao;
                        //x.BaseAmountYiLiao = model.BaseAmountYiLiao;
                        //x.BaseAmountShiYe = model.BaseAmountShiYe;
                        //x.BaseAmountShengYu = model.BaseAmountShengYu;
                        //x.BaseAmountGongShang = model.BaseAmountGongShang;
                        //x.BaseAmountCanBao = model.BaseAmountCanBao;
                        //x.UnitPercentYangLao = model.UnitPercentYangLao;
                        //x.UnitPercentYiLiao = model.UnitPercentYiLiao;
                        //x.UnitPercentShiYe = model.UnitPercentShiYe;
                        //x.UnitPercentShengYu = model.UnitPercentShengYu;
                        //x.UnitPercentGongShang = model.UnitPercentGongShang;
                        //x.UnitPercentCanBao = model.UnitPercentCanBao;
                        //x.EmployeePercentYangLao = model.EmployeePercentYangLao;
                        //x.EmployeePercentYiLiao = model.EmployeePercentYiLiao;
                        //x.EmployeePercentShiYe = model.EmployeePercentShiYe;
                        //x.EmployeePercentShengYu = model.EmployeePercentShengYu;
                        //x.EmployeePercentGongShang = model.EmployeePercentGongShang;
                        //x.EmployeePercentCanBao = model.EmployeePercentCanBao;
                        //x.EmployeeAmountCanBao = model.EmployeeAmountCanBao;
                        //x.EmployeeAmountDaBingYiLiao = model.EmployeeAmountDaBingYiLiao;
                        //x.EmployeeAmountYiLiaoBuChong = model.EmployeeAmountYiLiaoBuChong;
                        //x.UnitAmountCanBao = model.UnitAmountCanBao;
                        //x.UnitAmountDaBingYiLiao = model.UnitAmountDaBingYiLiao;
                        //x.UnitAmountYiLiaoBuChong = model.UnitAmountYiLiaoBuChong;
                    }
                    if(type != "sb")
                    {
                        x.BaseAmountGongJiJin = Math.Max(model.BaseAmountGongJiJin, x.BaseAmountGongJiJin);
                        x.EmployeePercentGongJiJin = Math.Max(model.EmployeePercentGongJiJin, x.EmployeePercentGongJiJin);
                        x.UnitPercentGongJiJin = Math.Max(model.UnitPercentGongJiJin, x.UnitPercentGongJiJin);
                    }
                    

                    x.BaseDifferenceAmount = model.BaseDifferenceAmount;
                    x.BaseRemark = model.BaseRemark;

                }
                await insuranceEmployeeService.Update(EmployeeDatas);
            }
            catch (Exception e)
            {
                Common.StaticLog.Error(e.ToString());
                throw new ErrorCodeException(-1, "员工记录更新错误");
            }


            return Ok(new {type,model});
        }

        [HttpGet]
        public async Task<IActionResult> CalculationModel([FromQuery] int id)
        {
            var model = await insuranceSchemeService.CalculationModel(id);
            return Ok(new {items=model});
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            if (!await insuranceSchemeService.ExistID(id))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            await insuranceSchemeService.Delete(id);
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> RegionAddress()
        {
            var rexpable = Expressionable.Create<Model.Live_RegionAddress>();
            var regions = await regionAddressService.List(rexpable.ToExpression());
            return Ok(new { items = regions });
        }
        [HttpPost]
        public async Task<IActionResult> Import()
        {
            List<string> data = new List<string>();
            int cid = 0;
            Guid? cgid = Guid.Parse("00000000-0000-0000-0000-000000000000");
            
            if (CurrentUser.PlatformType == Model.Enums.PlatformType.LZMini || CurrentUser.PlatformType == Model.Enums.PlatformType.MiniAdmin)
            {
                cgid = CurrentUser.CompanyGID;
                cid = CurrentUser.CompanyID;
            }
            try
            {
                var files = Request.Form.Files;
                string fn = string.Empty;
                var filePath = "/";


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
                    List<Model.Live_InsuranceScheme> list = new List<Model.Live_InsuranceScheme>();
                    var ds = Common.ExcelHeler.ReadExcelToDataSet(fn, "",2);
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        var rexpable = Expressionable.Create<Model.Live_RegionAddress>();
                        var regions = await regionAddressService.List(rexpable.ToExpression());

                        var dexpable = Expressionable.Create<Model.SYS_Dictionary>();
                        var gid = Guid.Parse("ecf62977-c49f-4ea3-84bb-2c541aef4805");
                        dexpable.And(t => t.PID == gid);
                        PagedModelBase query = new PagedModelBase();
                        query.Page = 0;
                        var dics = await dictionaryService.List(dexpable.ToExpression(), query);
                        BasicPagedModel squery = new BasicPagedModel();
                        squery.Page = 0;
                        var sexpable = Expressionable.Create<Model.Live_V_InsuranceSupplier>();
                        sexpable.And(t => t.Enable == true);
                        sexpable.And(t => t.CompanyID == cid);
                        var suppliers = await insuranceSupplierService.List(sexpable.ToExpression(), squery);

                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            
                            var expable = Expressionable.Create<Model.Live_InsuranceScheme>();
                            
                            expable.And(t => t.CompanyID == cid);
                            expable.And(t => t.Name == row[0].ToString());
                            if (await insuranceSchemeService.Exist(expable.ToExpression()))
                            {
                                continue;
                            }

                            else
                            {

                                try
                                {
                                    Model.Live_InsuranceScheme model = new Model.Live_InsuranceScheme();
                                    model.CompanyID = cid;
                                    model.CompanyGID = cgid;
                                    model.CreateTime = DateTime.Now;
                                    model.Enable = true;
                                    //model.No = row[0] != null ? row[0].ToString() : "";
                                    model.Name = row[0] != null ? row[0].ToString() : "";
                                    if (row[1]!=null)
                                    {
                                        var region = regions.Where(t => t.Name == row[1].ToString()).FirstOrDefault();
                                        if(region!=null)
                                        {
                                            model.Province = region.Name;
                                            model.ProvinceCode = region.Code;
                                        }
                                    }
                                    if (row[2] != null)
                                    {
                                        var region = regions.Where(t => t.Name == row[2].ToString()).FirstOrDefault();
                                        if (region != null)
                                        {
                                            model.City = region.Name;
                                            model.CityCode = region.Code;
                                        }
                                    }
                                    if(row[3] != null)
                                    {
                                        var dic = dics.Where(t => t.Name == row[3].ToString()).FirstOrDefault();
                                        if (dic!=null)
                                        {
                                            model.YlRule = Convert.ToInt32(dic.NO);
                                        }
                                    }
                                    model.TimeNode = row[4] != null ? Convert.ToInt32(row[4].ToString()) : null;
                                    if(row[5] != null)
                                    {
                                        var supplier = suppliers.Where(t => t.Name == row[5].ToString().Trim()).FirstOrDefault();
                                        if (supplier != null)
                                        {
                                            model.YlSupplierIdSheBao = supplier.ID;
                                        }
                                    }
                                    if (row[6] != null)
                                    {
                                        var supplier = suppliers.Where(t => t.Name == row[6].ToString().Trim()).FirstOrDefault();
                                        if (supplier != null)
                                        {
                                            model.YlSupplierIdGongJiJin = supplier.ID;
                                        }
                                    }
                                    model.BaseAmountYangLao = row[7] != null ? Convert.ToDecimal(row[7].ToString()) : 0;
                                    model.BaseAmountYiLiao = row[8] != null ? Convert.ToDecimal(row[8].ToString()) : 0;
                                    model.BaseAmountShiYe = row[9] != null ? Convert.ToDecimal(row[9].ToString()) : 0;
                                    model.BaseAmountShengYu = row[10] != null ? Convert.ToDecimal(row[10].ToString()) : 0;
                                    model.BaseAmountGongShang = row[11] != null ? Convert.ToDecimal(row[11].ToString()) : 0;
                                    model.BaseAmountCanBao = row[12] != null ? Convert.ToDecimal(row[12].ToString()) : 0;
                                    model.BaseAmountGongJiJin = row[13] != null ? Convert.ToDecimal(row[13].ToString()) : 0;
                                    
                                    model.EmployeePercentYangLao = row[14] != null ? Convert.ToDecimal(row[14].ToString()) : 0;
                                    model.EmployeePercentYiLiao = row[15] != null ? Convert.ToDecimal(row[15].ToString()) : 0;
                                    model.EmployeePercentShiYe = row[16] != null ? Convert.ToDecimal(row[16].ToString()) : 0;
                                    model.EmployeePercentShengYu = row[17] != null ? Convert.ToDecimal(row[17].ToString()) : 0;
                                    model.EmployeePercentGongShang = row[18] != null ? Convert.ToDecimal(row[18].ToString()) : 0;
                                    model.EmployeeAmountCanBao = row[19] != null ? Convert.ToDecimal(row[19].ToString()) : 0;
                                    model.EmployeeAmountDaBingYiLiao = row[20] != null ? Convert.ToDecimal(row[20].ToString()) : 0;
                                    model.EmployeeAmountYiLiaoBuChong = row[21] != null ? Convert.ToDecimal(row[21].ToString()) : 0;
                                    model.UnitPercentYangLao = row[22] != null ? Convert.ToDecimal(row[22].ToString()) : 0;
                                    model.UnitPercentYiLiao = row[23] != null ? Convert.ToDecimal(row[23].ToString()) : 0;
                                    model.UnitPercentShiYe = row[24] != null ? Convert.ToDecimal(row[24].ToString()) : 0;
                                    model.UnitPercentShengYu = row[25] != null ? Convert.ToDecimal(row[25].ToString()) : 0;
                                    model.UnitPercentGongShang = row[26] != null ? Convert.ToDecimal(row[26].ToString()) : 0;
                                    
                                    model.UnitPercentCanBao = row[27] != null ? Convert.ToDecimal(row[27].ToString()) : 0;
                                    model.UnitAmountDaBingYiLiao = row[28] != null ? Convert.ToDecimal(row[28].ToString()) : 0;
                                    model.UnitAmountYiLiaoBuChong = row[29] != null ? Convert.ToDecimal(row[29].ToString()) : 0;
                                    model.EmployeePercentGongJiJin = row[30] != null ? Convert.ToDecimal(row[30].ToString()) : 0;
                                    model.UnitPercentGongJiJin = row[31] != null ? Convert.ToDecimal(row[31].ToString()) : 0;
                                    model.BaseDifferenceAmount = row[32] != null ? Convert.ToDecimal(row[32].ToString()) : 0;
                                    model.BaseRemark = row[33] != null ? row[33].ToString() : "";








                                    //model.HouseLocation = row[2] != null ? row[2].ToString() : "";
                                    //model.Tel = row[3] != null ? row[3].ToString() : "";
                                    //model.IdentityCard = row[4] != null ? row[4].ToString() : "";
                                    //model.Content = row[5] != null ? row[5].ToString() : "";
                                    //model.UserID = user.UserID;
                                    //model.CreateTime = DateTime.Now;
                                    //model.PID = pid;
                                    //model.Status = 1;
                                    list.Add(model);
                                }
                                catch (Exception ex)
                                {
                                    throw new ErrorCodeException(-1, ex.ToString());
                                }
                            }

                        }
                       int i= await insuranceSchemeService.Adds(list);
                       return Ok();

                    }

                }


            }
            catch (Exception ex)
            {
                return Ok( new {msg = ex.ToString()});
            }
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Export([FromQuery] BasicPagedModel query)
        {
            query.Page = 0;
            var expable = Expressionable.Create<Model.Live_V_InsuranceScheme>();
            if (CurrentUser.PlatformType == Model.Enums.PlatformType.Admin || CurrentUser.PlatformType == Model.Enums.PlatformType.Api)
            {
                expable.AndIF(query.CompanyID.HasValue && query.CompanyID > 0, it => it.CompanyID == query.CompanyID);
            }
            else
            {
                expable.And(it => it.CompanyID == CurrentUser.CompanyID);
            }
            var list = await insuranceSchemeService.ListV(expable.ToExpression(),query);
            var result = await insuranceSchemeService.ExportExcel(list);
            string base64String = Convert.ToBase64String(result);
            return Ok(new { base64String, fileName = "社保配置方案", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }
    }
}
