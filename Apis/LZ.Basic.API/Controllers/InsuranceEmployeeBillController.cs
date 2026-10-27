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
using LZ.Model.Enums.Insurance;
using LZ.Core.Redis;
using System.Text;
using System.IO;
using Consul;
using LZ.Core.Services;
using NPOI.SS.Formula.Functions;
using System;
using LZ.Basic.Service.Services;
using NPOI.XSSF.UserModel;
using SixLabors.ImageSharp.Drawing;
using LZ.Model.PagedModel;
using LZ.Model;
namespace LZ.Basic.API.Controllers
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.LZMini)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    public class InsuranceEmployeeBillController(IInsuranceEmployeeBillService insuranceEmployeeBillService,IInsuranceEmployeeService insuranceEmployeeService,IAuthService authService) : BasicControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] Model.PagedModel.BasicPagedModel query)
        {
            var user = CurrentUser;
            var cid = user.CompanyID;
            var expable = Expressionable.Create<Model.Live_V_InsuranceEmployeeBill>();
            if (user.PlatformType == Model.Enums.PlatformType.Admin || user.PlatformType == Model.Enums.PlatformType.Api)
            {
                expable.AndIF(query.CompanyID.HasValue && query.CompanyID > 0, it => it.CompanyID == query.CompanyID);
            }
            else
            {
               // expable.And(it => it.CompanyID == CurrentUser.CompanyID);
                if (user.PlatformType == Model.Enums.PlatformType.MiniAdmin)
                {
                    var info = await authService.UserInfo<Model.SYS_CompanyUser>();
                    if (info.UserType == 2)
                    {
                        expable.And(t => t.PartnerID == info.PartnerID);
                    }
                }
            }
            if (!string.IsNullOrEmpty(query.UserName))
            {
                expable.And(t => t.Name.Contains(query.UserName));
            }
            if (!string.IsNullOrEmpty(query.Name))
            {
                expable.And(t => t.EmployeeCompanyName.Contains(query.Name));
            }
            if (!string.IsNullOrEmpty(query.CardID))
            {
                expable.And(t => t.IdcardNo.Contains(query.CardID));
            }
            expable.AndIF(query.Year.HasValue, t => t.BillMonth.HasValue && t.BillMonth.Value.Year == query.Year);
            expable.AndIF(query.Month.HasValue, t => t.BillMonth.HasValue && t.BillMonth.Value.Month == query.Month);
            var exp = expable.ToExpression();
            
            var models = await insuranceEmployeeBillService.List(exp, query, "CompanyID desc,ID desc");
            return Ok(new { items = models, count = query.Count });
        }
        [HttpGet]
        public async Task<Model.Live_V_InsuranceEmployeeBill> Get(int id)
        {
            return await insuranceEmployeeBillService.Get(id);
            //return await userService.Get(CurrentUser.UserID);
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            if (!await insuranceEmployeeBillService.ExistID(id))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            await insuranceEmployeeBillService.Delete(id);
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> BatchEmployeeCreate([FromForm] DateTime date)
        {
            Model.PagedModel.BasicPagedModel query = new Model.PagedModel.BasicPagedModel();
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
            expable.And(it => it.ProcessStatus != (int)InsuranceProcessStatusEnumType.作废 && it.ProcessStatus != (int)InsuranceProcessStatusEnumType.增员撤销 && it.ProcessStatus != (int)InsuranceProcessStatusEnumType.待处理);
            expable.And(it => it.Enable == true && (((it.YlRule == 2 && it.StartDateGongJiJin <= date) || (it.YlRule == 3 && it.StartDateGongJiJin <= date.AddMonths(1))) && (it.EndDateGongJiJin == null || it.EndDateGongJiJin > date) && it.StartDateGongJiJin.HasValue) || (((it.YlRule == 2 && it.StartDateSheBao <= date) || (it.YlRule == 3 && it.StartDateSheBao <= date.AddMonths(1))) && (it.EndDateSheBao == null || it.EndDateSheBao > date) && it.StartDateSheBao.HasValue));
          
            var employees = await insuranceEmployeeService.ListView(expable.ToExpression(), query);
            string msg = "没有数据"; 
            if(employees.Count>0)
            {
                msg = await insuranceEmployeeBillService.BatchEmployeeCreate(employees, date);
            }
            var ss = employees.Where(t => t.Name == "陈婵娜");
            return Ok(new {msg,items= employees,model=ss });
        }

        [HttpGet]
        public async Task<IActionResult> Export([FromQuery] BasicPagedModel query,string cardID)
        {
            var expable = Expressionable.Create<Model.Live_V_InsuranceEmployeeBill>();
            if (CurrentUser.PlatformType == Model.Enums.PlatformType.Admin || CurrentUser.PlatformType == Model.Enums.PlatformType.Api)
            {
                expable.AndIF(query.CompanyID.HasValue && query.CompanyID > 0, it => it.CompanyID == query.CompanyID);
            }
            else
            {
                expable.And(it => it.CompanyID == CurrentUser.CompanyID);
            }
            expable.AndIF(query.CID.HasValue, t => t.YlSchemeId == query.CID);
            expable.AndIF(query.Status.HasValue, t => t.ProcessStatus == query.Status);
            expable.AndIF(!string.IsNullOrEmpty(query.Name), t => t.EmployeeCompanyName == query.Name);
            expable.AndIF(!string.IsNullOrEmpty(query.UserName), t => t.Name == query.UserName);
            expable.AndIF(!string.IsNullOrEmpty(query.UserName), t => t.Name == query.UserName);
            expable.AndIF(!string.IsNullOrEmpty(cardID), t => t.IdcardNo == cardID);
            var exp = expable.ToExpression();
            var models = await insuranceEmployeeBillService.List(exp, query, "CreateTime desc");

            var result = insuranceEmployeeBillService.Export(models);
            string base64String = Convert.ToBase64String(result);
            return Ok(new { base64String, fileName = "社保员工记录", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }

        [HttpGet]
        public async Task<IActionResult> ExportToExcel([FromQuery] BasicPagedModel query)
        {

            var expable = Expressionable.Create<Model.Live_V_InsuranceEmployeeBill>();

            if (CurrentUser.PlatformType == Model.Enums.PlatformType.Admin || CurrentUser.PlatformType == Model.Enums.PlatformType.Api)
            {
                expable.AndIF(query.CompanyID.HasValue && query.CompanyID > 0, it => it.CompanyID == query.CompanyID);
            }
            else
            {
                expable.And(it => it.CompanyID == CurrentUser.CompanyID);
            }
            if (!string.IsNullOrEmpty(query.Name))
            {
                expable.And(t => t.Name.Contains(query.Name));
            }
            //if(!string.IsNullOrEmpty(query.BillMonth))
            //{
            //    expable.And(t => t.BillMonth == Convert.ToDateTime(query.BillMonth));
            //}
            var exp = expable.ToExpression();

            string templatePath = "/uploadfiles/模板/";//模板文件地址
            List<Model.Live_V_InsuranceEmployeeBill> bills = new List<Model.Live_V_InsuranceEmployeeBill>();
            bills = await insuranceEmployeeBillService.List(exp,query);
            string fileName = "社保员工账单模板.xlsx";//模板文件名称
            string saveName = insuranceEmployeeBillService.ExportToExcel(bills, fileName, templatePath, 3, 50,600);

            return Ok(new {msg=saveName });
        }
    }
}
    