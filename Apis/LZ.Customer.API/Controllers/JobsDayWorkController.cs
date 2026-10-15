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

namespace LZ.Customer.API.Controllers
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    [PlatformLimit(Model.Enums.PlatformType.LZMini)]
    public class JobsDayWorkController(IJobsDayWorkService jobsDayWorkService,ICustomerService customerService) : CustomerControllerBase
    {
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] JobPagedModel query)
        {
            var user = CurrentUser;
            var guid = HttpContext.Request.Headers["company-guid"].ToString();
            var expable = Expressionable.Create<Model.Live_V_JobsDayWork>();
            if (query.CID.HasValue)
            {
                expable.And(it => it.CompanyID == query.CID);
            }
            expable.AndIF(query.CustomerID.HasValue, it => it.CustomerID == query.CustomerID);
            if (query.From == "lzmini")
            {
                expable.And(it => SqlFunc.Subqueryable<Model.Live_PersonnelItems>().Where(s => s.CID == it.CustomerID && s.UserID == user.UserID).Any());
            }
            if (!string.IsNullOrEmpty(query.Name))
            {
                expable.And(it => it.Title.Contains(query.Name));
            }
            if (!string.IsNullOrEmpty(query.Area))
            {
                expable.And(it => it.District == query.Area);
            }
            if (!string.IsNullOrEmpty(query.City) && query.City != "不限")
            {
                expable.And(it => it.City.Contains(query.City));
            }
            if (!string.IsNullOrEmpty(query.CustomerName))
            {
                expable.And(it => it.CustomerName.Contains(query.CustomerName));
            }
            if (query.Status.HasValue)
            { 
                expable.And(it => it.Status==query.Status);
            }
            if (!string.IsNullOrEmpty(query.Money))
            {
                var ms = query.Money.Split('-');
                if (ms.Length > 1)
                {
                    if (ms[1] == "0")
                    {
                        var money = Convert.ToInt32(ms[0]);
                        expable.And(it => it.MaxMoney >= money);
                    }
                    else if (ms[0] == "0")
                    {
                        var money = Convert.ToInt32(ms[1]);
                        expable.And(it => it.MaxMoney <= money);
                    }
                    else
                    {
                        var minmoney = Convert.ToInt32(ms[0]);
                        var maxmoney = Convert.ToInt32(ms[1]);
                        expable.And(it => it.MinMoney <= maxmoney && it.MaxMoney >= minmoney);
                    }
                }
                else
                {
                    expable.And(it => it.MaxMoney >= Convert.ToInt32(query.Money.Replace("以上", "")));
                }
            }
            expable.AndIF(query.XFZShow.HasValue, it => it.XFZShow == query.XFZShow);
            expable.AndIF(query.AttendanceType.HasValue, it => it.AttendanceType == query.AttendanceType);
            if (user == null || user.UserID == 0 || user.PlatformType == Model.Enums.PlatformType.XFZMini)
            {
                expable.And(it => it.XFZShow == true);
            }
            expable.AndIF(query.LWShow.HasValue, it => it.LWShow == query.LWShow);
            if (!string.IsNullOrEmpty(query.Tabs))
            {
                var tabs = query.Tabs.Split(',');
                expable.And(it => tabs.Any(s => it.ShowTags.Contains(s)));
            }
            if (user != null && user.CompanyID > 0)
            {
                expable.And(it => it.CompanyID == user.CompanyID);
            }
            //if (user != null && user.PlatformType == Model.Enums.PlatformType.LZMini && !query.IsJoint.HasValue)
            //{
            //    expable.And(it => SqlFunc.Subqueryable<Model.Live_PersonnelItems>().Where(s => s.CID == it.CustomerID && s.UserID == user.UserID && s.IsManage).Any());
            //}
            var exp = expable.ToExpression();
            var models = await jobsDayWorkService.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }

        [HttpPost]
        [OperationLogAction("新增岗位：{model.Title}")]
        public async Task<IActionResult> Add([FromBody] Model.Live_JobsDayWork model)
        {
            var user = CurrentUser;
            model.CreateTime = DateTime.Now;
            model.Status = 1;
            var customer = await customerService.Get(model.CustomerID.Value);
            
            string result = "";
            
            foreach (var ch in customer.CustomerName)
            {
                try
                {
                    string[] pinyin = PinyinHelper.ToHanyuPinyinStringArray(ch); // 获取每个字符的拼音，以空格分隔
                    result += pinyin[0][0];
                }
                catch(Exception e)
                {
                    continue;
                }
            }
            model.JobNo = result + jobsDayWorkService.GetSequence().ToString();
            model.XFZShow = model.TotalNumber > model.StaffNumber;
            model.ID = await jobsDayWorkService.Add(model);
            return Ok(model.ID);
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<Model.Live_V_JobsDayWork> GetView(int id)
        {
            var model = await jobsDayWorkService.GetView(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<Model.Live_JobsDayWork> Get(int id)
        {
            var model = await jobsDayWorkService.Get(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }
        [HttpGet]
        public async Task<Model.Live_JobsDayWork> GetModel(int id)
        {
            var model = await jobsDayWorkService.Get(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }
        [HttpPost]
        [OperationLogAction("灵活用工员工入职离职人数变化：{model.JobNo}")]
        public async Task<IActionResult> UpdateEntry(int id)
        {
            if (!await jobsDayWorkService.Exist(id))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            var model = await jobsDayWorkService.Get(id);
            model.StaffNumber = model.StaffNumber + 1;
            model.XFZShow = model.TotalNumber > model.StaffNumber;
            await jobsDayWorkService.Update(model);
            return Ok(new { number = model.StaffNumber });
        }
        [HttpPut]
        [OperationLogAction("更新岗位：{model.Title}")]
        public async Task<IActionResult> Update([FromBody] Model.Live_JobsDayWork model)
        {
            if (!await jobsDayWorkService.Exist(model.ID, CurrentUser.CompanyID))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            model.XFZShow = model.TotalNumber > model.StaffNumber;
            await jobsDayWorkService.Update(model);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> ChangeStatus([FromForm] int id, [FromForm] int status)
        {
            if (!await jobsDayWorkService.Exist(id, CurrentUser.CompanyID))
            {
                throw new ErrorCodeException(-1, "数据不存在");
            }
            await jobsDayWorkService.ChangeStatus(id, status);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> Enable([FromForm] int id, [FromForm] string field)
        {
            if (!await jobsDayWorkService.Exist(id, CurrentUser.CompanyID))
            {
                throw new ErrorCodeException(-1, "你要启用的数据不存在");
            }
            await jobsDayWorkService.Set(id, field, true);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> Disable([FromForm] int id, [FromForm] string field)
        {
            if (!await jobsDayWorkService.Exist(id, CurrentUser.CompanyID))
            {
                throw new ErrorCodeException(-1, "你要禁用的数据不存在");
            }
            await jobsDayWorkService.Set(id, field, false);
            return Ok();
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpDelete]
        [OperationLogAction("删除岗位id：{id}")]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            if (!await jobsDayWorkService.Exist(id, CurrentUser.CompanyID))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            await jobsDayWorkService.Delete(id);
            return Ok();
        }



    }
}
