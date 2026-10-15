using Microsoft.AspNetCore.Mvc;
using LZ.Core;
using LZ.Model.Exceptions;
using SqlSugar;
using LZ.Customer.Service.Interface;
using LZ.Customer.API;
using LZ.Core.Attributes;
using LZ.Customer.Service.Services;
using Microsoft.AspNetCore.Authorization;
using LZ.Core.Interface;
using Pipelines.Sockets.Unofficial.Buffers;
using System.Collections.Generic;
using LZ.Model.PagedModel;
using LZ.Model.Enums;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Pinyin4net;
using LZ.Core.Services;
using LZ.Model;
using Newtonsoft.Json;
namespace LZ.Customer.API.Controllers
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    [PlatformLimit(Model.Enums.PlatformType.LZMini)]
    public class JobVideoController(IJobVideoService jobVideoService, ICustomerJobsService customerJobsService,IAuthService authService,IHttpService httpService) : CustomerControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] JobPagedModel query,string approvalName)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Live_V_JobVideo>();
            //expable.AndIF(user.PlatformType != PlatformType.Admin && user.PlatformType != PlatformType.Api, f => f.CompanyID == user.CompanyID);
            expable.AndIF(query.CustomerID.HasValue, it => it.CustomerID == query.CustomerID);
            //if (query.From == "lzmini")
            //{
            //    expable.And(it => SqlFunc.Subqueryable<Model.Live_PersonnelItems>().Where(s => s.CID == it.CustomerID && s.UserID == user.UserID).Any());
            //}
            if (user.PlatformType == PlatformType.LZMini)
            {
                var info = await authService.UserInfo<Model.Live_Partner>();
                expable.And(t => t.PartnerID == info.PartnerID);
            }
            if (query.Status.HasValue)
            {
                expable.And(it => it.Status == query.Status);
            }
            expable.AndIF(query.PartnerID.HasValue, t => t.PartnerID == query.PartnerID);
            expable.AndIF(!string.IsNullOrEmpty(approvalName), it => it.ApprovalName.Contains(approvalName));
            expable.AndIF(!string.IsNullOrEmpty(query.CustomerName), t => t.CustomerName.Contains(query.CustomerName));
            expable.AndIF(!string.IsNullOrEmpty(query.PartnerName), t => t.PartnerName.Contains(query.PartnerName));
            var exp = expable.ToExpression();
            var models = await jobVideoService.ListView(exp, query);
            return Ok(new { items = models, count = query.Count });
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Model.Live_JobVideo model)
        {
            var user = CurrentUser;
            model.GID = Guid.NewGuid();
            model.CompanyID = user.CompanyID;
            model.CompanyGID = user.CompanyGID;
            if (user.PlatformType == PlatformType.LZMini)
            {
                var info = await authService.UserInfo<Model.Live_Partner>();
                model.PartnerID = info.PartnerID;
            }
            
            model.CreateTime = DateTime.Now;
            model.Status = 1;
            await jobVideoService.Add(model);
            return Ok(model.ID);
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<Model.Live_JobVideo> GetView(int id)
        {
            var model = await jobVideoService.Get(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<Model.Live_JobVideo> Get(int id)
        {
            var model = await jobVideoService.Get(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }
        [HttpGet]
        public async Task<Model.Live_JobVideo> GetModel(int id)
        {
            var model = await jobVideoService.Get(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_JobVideo model)
        {
            if (!await jobVideoService.Exist(model.ID))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            model.AuditTime = DateTime.Now;
            await jobVideoService.Update(model);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Audit([FromBody] Model.Live_JobVideo model)
        {
            if (!await jobVideoService.Exist(model.ID))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            if (model.Status == 2)
            {

                if (model.JobID.HasValue && model.CustomerID.HasValue)
                {
                    var isexist = await customerJobsService.Exist(t => t.CustomerID == model.CustomerID && t.EntryJobID == model.JobID);
                    if (!isexist)
                    {
                        Live_CustomerJobs job = new Live_CustomerJobs();
                        job.CustomerID = model.CustomerID.Value;
                        job.EntryJobID = model.JobID.Value;
                        await customerJobsService.Add(job);
                    }
                    var view = await jobVideoService.GetView(model.ID);

                    Live_BusInsuranceWorkPlan personnel = new Live_BusInsuranceWorkPlan();
                    personnel.WorkName = model.ApprovalName;
                    personnel.PlanName = model.PlanName;
                    personnel.InsuranceCompany = model.InsuranceCompany;
                    personnel.CustomerName = view.CustomerName;
                    personnel.IsEnable = true;
                    personnel.CreateTime = DateTime.Now;
                    personnel.IsUseBusInsurance = true;
                    personnel.InsuranceType = (InsuranceTypeEnumType)model.InsuranceType.Value;
                    if (!string.IsNullOrEmpty(model.InsuranceName))
                    {
                        personnel.InsuranceWorkName = model.InsuranceName;
                    }
                    var modeljson = JsonConvert.SerializeObject(personnel);
                    var personnelObj = await httpService.PostModelAsync(ServiceNames.UsersService, "/api/BusInsuranceWorkPlan/Add", modeljson);
                    var msg = personnelObj.Message;
                    if (msg != "Success")
                    {
                        throw new ErrorCodeException(-1, msg);
                    }
                }
            }
            model.AuditTime = DateTime.Now;
            await jobVideoService.Update(model);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> ChangeStatus([FromForm] int id, [FromForm] int status)
        {
            if (!await jobVideoService.Exist(id, CurrentUser.CompanyID))
            {
                throw new ErrorCodeException(-1, "数据不存在");
            }
            await jobVideoService.ChangeStatus(id, status);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            if (!await jobVideoService.Exist(id, CurrentUser.CompanyID))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            await jobVideoService.Delete(id);
            return Ok();
        }
    }
}
