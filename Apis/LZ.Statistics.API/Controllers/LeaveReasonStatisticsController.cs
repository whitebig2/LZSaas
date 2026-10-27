using LZ.Statistics.Service.Interface;
using LZ.Core;
using LZ.Core.Interface;
using Microsoft.AspNetCore.Mvc;
using LZ.Model.Exceptions;
using SqlSugar;
using LZ.Model.PagedModel;
using LZ.Statistics.API;
using LZ.Model.Enums;
using NPOI.SS.Formula.Functions;
using LZ.Model;
using System.Linq.Expressions;
using LZ.Statistics.Service.Services;
using System.Globalization;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
using NetTaste;

namespace LZ.Statistics.API.Controllers
{
    public class LeaveReasonStatisticsController(IEntryStatisticsService entryStatisticsService) : StatisticsControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> CustomerList(int id)
        {
            var user = CurrentUser;
            var models = await entryStatisticsService.LeaveReason(it=> SqlFunc.Subqueryable<Model.Live_UserEntry>().Where(s => s.ID == it.EntryID && s.CustomerID == id).Any());
            return Ok(models);
        }
        [HttpGet]
        public async Task<IActionResult> CustomerSubList(int id, int leaveId)
        {
            var models = await entryStatisticsService.LeaveReasonList(it =>it.LeaveID== leaveId && SqlFunc.Subqueryable<Model.Live_UserEntry>().Where(s => s.ID == it.EntryID && s.CustomerID == id).Any());
            var objs = new Dictionary<string, int>();
            foreach (var m in models)
            {
                var vals = m.LeaveValue.Split(',');
                foreach (var val in vals)
                {
                    if (objs.ContainsKey(val))
                    {
                        objs[val]++;
                    }
                    else
                    {
                        objs.Add(val, 1);
                    }
                }
            }
            var mcounts = new List<Model.Statistics.EntryModel>();
            foreach (var m in objs)
            {
                var mcount = new Model.Statistics.EntryModel();
                mcount.Name = m.Key;
                mcount.Count = m.Value;
                mcounts.Add(mcount);
            }
            return Ok(mcounts);
        }
    }
}
