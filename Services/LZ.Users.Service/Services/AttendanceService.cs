using LZ.Common;
using LZ.Core.Interface;
using LZ.Model;
using LZ.Model.Enums.Attendance;
using LZ.Model.Enums.Budget;
using LZ.Model.Model;
using LZ.Users.Service.Interface;
using NetTaste;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LZ.Model.Model.AttendanceModel;
using LZ.Model.PagedModel;
using Consul;
using NPOI.HSSF.Record;
using NPOI.POIFS.FileSystem;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
using SqlSugar;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace LZ.Users.Service.Services
{
    public class AttendanceService(IAttendanceCalendarService attendanceCalendarService,IAttendanceStructureService attendanceStructureService,IAttendanceDetailService attendanceDetailService) : IAttendanceService, IScopedDependency
    {
        public async Task<byte[]> StatisticsExportExcel(List<Live_V_AttendanceRecord> list, List<Model.Live_V_AttendanceApply> applys, List<Model.Live_V_UserEntry> entrys, int structureID, DateTime start, DateTime end, UserPagedModel userPagedModel)
        {
            var expable = Expressionable.Create<Model.Live_V_AttendanceStructure>();
            if(!string.IsNullOrEmpty(userPagedModel.CustomerIDS))
            {
                var arr = userPagedModel.CustomerIDS.Split(",");
                expable.And(t => arr.Contains(t.CustomerID.ToString()));
            }
            expable.AndIF(userPagedModel.structureID > 0, t => t.ID == userPagedModel.structureID);


            var structures = await attendanceStructureService.List(expable.ToExpression(),new UserPagedModel() { Page = 0 });
            

            List<Live_AttendanceCalendar> calendars = new List<Live_AttendanceCalendar>();
            calendars = await attendanceCalendarService.List(t => t.Year == start.Year && t.Month == start.Month, new UserPagedModel { Page = 0 });
            //List<Live_AttendanceCalendar> calendar = new List<Live_AttendanceCalendar>();
            //string[] items = structure.Days.Split(",");
            //if (structure.Holiday)
            //{
            //    calendar = calendars.Where(t => (t.Status == 1 || t.Status == 3) && items.Contains(t.DayEnum.ToString())).ToList();
            //}
            IWorkbook workbook = new XSSFWorkbook();
            //表格大字体样式（针对公司名称等第一行特定内容）
            var font = workbook.CreateFont();
            font.FontHeightInPoints = 16;
            font.FontName = "微软雅黑";
            ICellStyle style = workbook.CreateCellStyle();
            style.Alignment = HorizontalAlignment.Left;
            style.VerticalAlignment = VerticalAlignment.Center;
            style.SetFont(font);
            double width = 25 * 256;
            short height = 30 * 20;

            //表格正常样式
            var font1 = workbook.CreateFont();
            font1.FontHeightInPoints = 10;
            font1.FontName = "微软雅黑";
            ICellStyle style1 = workbook.CreateCellStyle();
            style1.Alignment = HorizontalAlignment.Center;
            style1.VerticalAlignment = VerticalAlignment.Center;
            style1.SetFont(font1);
            ISheet sheet1 = workbook.CreateSheet("考勤统计");
            string[] heads = {
                "姓名",
                "身份证",
                "客户单位",
                "车间",
                "月份",
                "应出勤天数",
                "出勤天数",
                "外勤次数",
                "迟到",
                "早退",
                "请假",
                "加班"
            };
            sheet1.DefaultRowHeight = height;
            for (int x = 0; x < heads.Length; x++)
            {
                sheet1.SetColumnWidth(x, width);
            }

            sheet1.SetColumnWidth(6, 25 * 256);
            for (int y = 0; y < 2; y++)
            {
                sheet1.CreateRow(y);
            }
            ExcelHeler.SetHeaderSingleNoSetWidth(workbook, sheet1, heads, font1, width, height, 0);
            int index = 0;

            foreach (var x in entrys)
            {
                if(!x.StructureID.HasValue)
                {
                    continue;
                }
                var structure = structures.Find(t => t.ID == x.StructureID);
                if(structure==null)
                {
                    continue;
                }
                var apply = applys.Where(t => t.UserID == x.UserID).ToList();
                var records = list.Where(t => t.UserID == x.UserID).ToList();
                List<object> arr = new();
                int col = 0;
                arr.Add(x.Name);
                arr.Add(x.CardID);
                arr.Add(structure.CustomerName);
                arr.Add(structure.Name);
                arr.Add(start.ToString("yyyy-MM"));
                List<Live_AttendanceCalendar> calendar = new List<Live_AttendanceCalendar>();
                string[] items = structure.Days.Split(",");
                if (structure.Holiday)
                {
                    calendar = calendars.Where(t => (t.Status == 1 || t.Status == 3) && items.Contains(t.DayEnum.ToString())).ToList();
                }
                //item.records = records.Where(t => t.UserID == entry.UserID.Value).ToList();
                //item.model.TotalDays = item.calendars.Count;
                //item.model.Days = item.records.GroupBy(t => t.SignDate).Select(g => new { SignDate = g.Key, Count = g.Count() }).ToList().Count;
                //item.model.OutsideWorkCount = item.records.Where(t => t.Status == (int)AttendanceStatusEnum.外勤).ToList().Count;
                //item.model.LateCount = item.records.Where(t => t.Status == (int)AttendanceStatusEnum.迟到 || t.Status == (int)AttendanceStatusEnum.外勤迟到).ToList().Count;
                //item.model.LeaveEarlyCount = item.records.Where(t => t.Status == (int)AttendanceStatusEnum.早退 || t.Status == (int)AttendanceStatusEnum.外勤早退).ToList().Count;
                //item.model.AskForLeaveCount = item.records.Where(t => t.Status == (int)AttendanceStatusEnum.请假).ToList().Count;
                //if (item.structure.RuleType == 2)
                //{
                //    item.model.TotalDays = item.model.Days;

                //}
                //else
                //{
                //    item.model.NoCardCount = item.model.TotalDays * item.details.Count - records.Where(t => t.Type != 3 && t.Type != 4).ToList().Count;
                //}
                var chuqian = records.Where(t => t.Status != (int)AttendanceStatusEnum.加班 && t.Status != (int)AttendanceStatusEnum.外勤加班).GroupBy(t =>  t.SignDate).Select(g => new { SignDate = g.Key, Count = g.Count() }).ToList().Count;
                if (structure.RuleType == 2)
                {
                    arr.Add(chuqian);
                }
                else
                {
                    if (structure.Holiday)
                    {
                        arr.Add(calendar.Count);
                    }
                    else
                    {
                        arr.Add(calendars.Where(t=>items.Contains(t.DayEnum.ToString())).ToList().Count);
                    }
                    
                }
                arr.Add(chuqian);
                arr.Add(records.Where(t => t.Status == (int)AttendanceStatusEnum.外勤 || t.Status == (int)AttendanceStatusEnum.外勤迟到 || t.Status == (int)AttendanceStatusEnum.外勤早退).ToList().Count);
                arr.Add(records.Where(t => t.Status == (int)AttendanceStatusEnum.迟到 || t.Status == (int)AttendanceStatusEnum.外勤迟到).ToList().Count);
                arr.Add(records.Where(t => t.Status == (int)AttendanceStatusEnum.早退 || t.Status == (int)AttendanceStatusEnum.外勤早退).ToList().Count);
                arr.Add(apply.Where(t => t.ApplyType == (int)AttendanceApplyTypeEnum.请假 || t.ApplyType == (int)AttendanceApplyTypeEnum.补卡).ToList().Count);//请假次数
                arr.Add(apply.Where(t => t.ApplyType == (int)AttendanceApplyTypeEnum.加班).ToList().Count);//加班次数
                sheet1.CreateRow(index + 1);//index=1,从第三行开始,前2行是表头
                foreach (var value in arr)
                {
                    ExcelHeler.SetCellValue(workbook, sheet1, font1, index + 1, col, value);
                    col++;
                }
                index++;
            }


            ISheet sheet2 = workbook.CreateSheet("打卡记录");
            
            //font.IsBold = true;
            
            string[] heads1 = {
                "姓名",
                "服务商",
                "考勤日期",
                "考勤时间",
                "打卡时间",
                "打卡结果",
                "申请补卡记录",
                "异常打卡原因",
                "补卡结果",
                "补卡证明（附件）",
                "加班小时数",
                "加班结果",
                "外勤图片",
                "外勤原因",
                "打卡设备",
                "系统备注",
            };
            sheet2.DefaultRowHeight = height;
            for (int x = 0; x < heads1.Length; x++)
            {
                sheet2.SetColumnWidth(x, width);
            }

            sheet2.SetColumnWidth(6, 25 * 256);
            for (int y = 0; y < 2; y++)
            {
                sheet2.CreateRow(y);
            }
            ExcelHeler.SetCellRangeValueWithStyle(workbook, sheet2, font, 0, 0, 0, 15, "记录统计日期:" + start.ToString("yyyy-MM-dd") + "至" + end.ToString("yyyy-MM-dd"), 1);
            ExcelHeler.SetCellRangeValueWithStyle(workbook, sheet2, font, 1, 1, 0, 15, "报表生成时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm"), 1);
           
            ExcelHeler.SetHeaderSingleNoSetWidth(workbook, sheet2, heads1, font1, width, height, 2);

            index = 2;

            foreach(var entry in entrys)
            {

                if (!entry.StructureID.HasValue)
                {
                    continue;
                }
                var structure = structures.Find(t => t.ID == entry.StructureID);
                if (structure == null)
                {
                    continue;
                }
                string[] items = structure.Days.Split(",");
                if (structure.RuleType == 2)
                {
                    var entry_list = list.Where(t => t.EntryID == entry.UserID).ToList();
                    foreach (var x in entry_list)
                    {
                        List<object> arr = new();
                        int col = 0;
                        arr.Add(x.UserName);//姓名
                        arr.Add(x.PartnerName);//服务商
                        arr.Add(x.SignDate.HasValue ? x.SignDate.Value.ToString("yyyy-MM-dd") + " " + ((AttendanceDayOfWeekEnum)x.SignDate.Value.DayOfWeek).ToString() : "");//考勤日期
                        arr.Add(x.SignDate.HasValue ? x.SignDate.Value.ToString("yyyy-MM-dd") + " " + x.TimeShow : "");//考勤时间


                        if (x.Status == (int)AttendanceStatusEnum.补卡)
                        {
                            arr.Add("");//打卡时间
                            arr.Add(x.StatusName);//打卡结果
                            arr.Add(x.ApplyApplyDate.HasValue ? x.ApplyApplyDate.Value.ToString("yyyy-MM-dd HH:mm") : "");//申请补卡记录
                            arr.Add(x.ApplyContent);//异常打卡原因
                            arr.Add(x.ApplyStatus.HasValue ? (AttendanceApplyStatusEnum)x.ApplyStatus : "");//补卡结果
                            if (!string.IsNullOrEmpty(x.ApplyFileInfo))
                            {
                                var slist = JsonConvert.DeserializeObject(x.ApplyFileInfo) as JArray;
                                string urls = "";
                                foreach (var item in slist)
                                {
                                    urls += item["url"] + "\n";

                                }
                                arr.Add(urls);//补卡证明（附件）
                            }
                            else
                            {
                                arr.Add("");//补卡证明（附件）
                            }
                            arr.Add("");//加班小时数
                            arr.Add("");//加班结果

                        }

                        else if (x.Status == (int)AttendanceStatusEnum.加班 || x.Status == (int)AttendanceStatusEnum.外勤加班)
                        {
                            arr.Add(x.AttendanceDate.HasValue ? x.AttendanceDate.Value.ToString("yyyy-MM-dd HH:mm") : "");//打卡时间
                            arr.Add(x.StatusName);//打卡结果
                            arr.Add("");//申请补卡记录
                            arr.Add("");//异常打卡原因
                            arr.Add("");//补卡结果
                            arr.Add("");//补卡证明（附件）
                            arr.Add(x.ApplyNumber.HasValue ? (x.ApplyNumber.Value.ToString("#0.######") + "小时") : "");//加班小时数
                            arr.Add(x.ApplyStatus.HasValue ? (AttendanceApplyStatusEnum)x.ApplyStatus : "");//加班结果
                        }
                        else
                        {
                            arr.Add(x.AttendanceDate.HasValue ? x.AttendanceDate.Value.ToString("yyyy-MM-dd HH:mm") : "");//打卡时间
                            arr.Add(x.StatusName);//打卡结果
                            arr.Add("");//申请补卡记录
                            arr.Add(!string.IsNullOrEmpty(x.ApplyContent) ? x.ApplyContent : "");//异常打卡原因
                            arr.Add(x.ApplyStatus.HasValue ? (AttendanceApplyStatusEnum)x.ApplyStatus : "");//补卡结果
                            if (!string.IsNullOrEmpty(x.ApplyFileInfo))
                            {
                                var slist = JsonConvert.DeserializeObject(x.ApplyFileInfo) as JArray;
                                string urls = "";
                                foreach (var item in slist)
                                {
                                    urls += item["url"] + "\n";

                                }
                                arr.Add(urls);//补卡证明（附件）
                            }
                            else
                            {
                                arr.Add("");//补卡证明（附件）
                            }
                            arr.Add("");//加班小时数
                            arr.Add("");//加班结果
                        }
                        if (!string.IsNullOrEmpty(x.FileInfo))
                        {
                            var slist = JsonConvert.DeserializeObject(x.FileInfo) as JArray;
                            string urls = "";
                            foreach (var item in slist)
                            {
                                urls += item["url"] + "\n";

                            }
                            arr.Add(urls);//外勤图片
                        }
                        else
                        {
                            arr.Add("");//外勤图片
                        }
                        arr.Add(x.Content);//外勤原因
                        arr.Add(x.MobileDevice);//打卡设备
                        arr.Add(x.Remark);//系统备注
                        sheet2.CreateRow(index + 1);
                        foreach (var value in arr)
                        {
                            try
                            {
                                Type type = value.GetType();
                                if (type.FullName == "System.Decimal" || type.FullName == "System.Double" || type.FullName == "System.Int32")
                                {
                                    ExcelHeler.SetCellValue(workbook, sheet2, font1, index + 1, col, value);
                                }
                                else
                                {
                                    XSSFRichTextString richString = new XSSFRichTextString(value.ToString());
                                    ExcelHeler.SetCellValue(workbook, sheet2, font1, index + 1, col, richString);
                                }
                            }
                            catch (Exception e)
                            {
                                ExcelHeler.SetCellValue(workbook, sheet2, font1, index + 1, col, value);
                            }
                            col++;
                        }
                        index++;
                    }
                }
                else if (structure.RuleType == 1)
                {
                    var details = await attendanceDetailService.List(t => t.ConfigID == structure.AttendanceID, new UserPagedModel() { Page = 0 });
                    foreach (var c in calendars)
                    {
                        if (!c.Calendar.HasValue)
                        {
                            continue;
                        }
                        bool isWork = items.Contains(c.DayEnum.ToString()) && ((structure.Holiday && c.Status != 2) || !structure.Holiday); //是否是工作日
                        //非工作日打卡情况
                        if (!isWork)
                        {
                            var app = list.Where(t => t.SignDate.HasValue && t.SignDate.Value.ToString("yyyy-MM-dd") == c.Calendar.Value.ToString("yyyy-MM-dd") && t.EntryID == entry.ID).ToList();
                            if (app.Count > 0)
                            {
                                foreach (var x in app)
                                {
                                    List<object> arr = new();
                                    int col = 0;
                                    arr.Add(x.UserName);//姓名
                                    arr.Add(x.PartnerName);//服务商
                                    arr.Add(x.SignDate.HasValue ? x.SignDate.Value.ToString("yyyy-MM-dd") + " " + ((AttendanceDayOfWeekEnum)x.SignDate.Value.DayOfWeek).ToString() : "");//考勤日期
                                    arr.Add(x.SignDate.HasValue ? x.SignDate.Value.ToString("yyyy-MM-dd") + " " + x.TimeShow : "");//考勤时间
                                    arr.Add(x.AttendanceDate.HasValue ? x.AttendanceDate.Value.ToString("yyyy-MM-dd HH:mm") : "");//打卡时间
                                    arr.Add(x.StatusName);//打卡结果
                                    arr.Add("");//申请补卡记录
                                    arr.Add("");//异常打卡原因
                                    arr.Add("");//补卡结果
                                    arr.Add("");//补卡证明（附件）
                                    arr.Add(x.ApplyNumber.HasValue ? (x.ApplyNumber.Value.ToString("#0.######") + "小时") : "");//加班小时数
                                    arr.Add(x.ApplyStatus.HasValue ? (AttendanceApplyStatusEnum)x.ApplyStatus : "");//加班结果
                                    if (!string.IsNullOrEmpty(x.FileInfo))
                                    {
                                        var slist = JsonConvert.DeserializeObject(x.FileInfo) as JArray;
                                        string urls = "";
                                        foreach (var item in slist)
                                        {
                                            urls += item["url"] + "\n";

                                        }
                                        arr.Add(urls);//外勤图片
                                    }
                                    else
                                    {
                                        arr.Add("");//外勤图片
                                    }
                                    arr.Add(x.Content ?? ""); //外勤原因
                                    arr.Add(x.MobileDevice ?? "");//打卡设备
                                    arr.Add(x.Remark ?? "");//系统备注
                                    sheet2.CreateRow(index + 1);
                                    foreach (var value in arr)
                                    {
                                        try
                                        {
                                            Type type = value.GetType();
                                            if (type.FullName == "System.Decimal" || type.FullName == "System.Double" || type.FullName == "System.Int32")
                                            {
                                                ExcelHeler.SetCellValue(workbook, sheet2, font1, index + 1, col, value);
                                            }
                                            else
                                            {
                                                XSSFRichTextString richString = new XSSFRichTextString(value.ToString());
                                                ExcelHeler.SetCellValue(workbook, sheet2, font1, index + 1, col, richString);
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            ExcelHeler.SetCellValue(workbook, sheet2, font1, index + 1, col, value);
                                        }
                                        col++;
                                    }
                                    index++;
                                }
                            }

                        }
                        //工作日打卡情况
                        else
                        {
                            foreach (var d in details)
                            {
                                var record = list.Find(t => t.SignDate.HasValue && t.SignDate.Value.ToString("yyyy-MM-dd") == c.Calendar.Value.ToString("yyy-MM-dd") && t.Sort == d.Sort && t.EntryID == entry.ID);
                                if (record == null)
                                {
                                    //没打卡
                                    List<object> arr = new();
                                    int col = 0;
                                    arr.Add(entry.Name);//姓名
                                    arr.Add(entry.PartnerName);//服务商
                                    arr.Add(c.Calendar.HasValue ? c.Calendar.Value.ToString("yyyy-MM-dd") + " " + ((AttendanceDayOfWeekEnum)c.Calendar.Value.DayOfWeek).ToString() : "");//考勤日期
                                    arr.Add(c.Calendar.HasValue ? c.Calendar.Value.ToString("yyyy-MM-dd") + " " + d.TimeShow : "");//考勤时间
                                    arr.Add("");//打卡时间
                                    arr.Add("缺卡");//打卡结果
                                    arr.Add("");//申请补卡记录
                                    arr.Add("");//异常打卡原因
                                    arr.Add("");//补卡结果
                                    arr.Add("");//补卡证明（附件）
                                    arr.Add("");//加班小时数
                                    arr.Add("");//加班结果
                                    arr.Add("");//外勤图片
                                    arr.Add("");
                                    arr.Add("");
                                    arr.Add("");
                                    sheet2.CreateRow(index + 1);
                                    foreach (var value in arr)
                                    {
                                        try
                                        {
                                            Type type = value.GetType();
                                            if (type.FullName == "System.Decimal" || type.FullName == "System.Double" || type.FullName == "System.Int32")
                                            {
                                                ExcelHeler.SetCellValue(workbook, sheet2, font1, index + 1, col, value);
                                            }
                                            else
                                            {
                                                XSSFRichTextString richString = new XSSFRichTextString(value.ToString());
                                                ExcelHeler.SetCellValue(workbook, sheet2, font1, index + 1, col, richString);
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            ExcelHeler.SetCellValue(workbook, sheet2, font1, index + 1, col, value);
                                        }
                                        col++;
                                    }
                                    index++;

                                }
                                else
                                {
                                    //有打卡记录
                                    List<object> arr = new();
                                    int col = 0;
                                    arr.Add(entry.Name);//姓名
                                    arr.Add(entry.PartnerName);//服务商
                                    arr.Add(c.Calendar.HasValue ? c.Calendar.Value.ToString("yyyy-MM-dd") + " " + ((AttendanceDayOfWeekEnum)c.Calendar.Value.DayOfWeek).ToString() : "");//考勤日期
                                    arr.Add(c.Calendar.HasValue ? c.Calendar.Value.ToString("yyyy-MM-dd") + " " + d.TimeShow : "");//考勤时间
                                    //arr.Add(record.ApplyApplyDate.HasValue ? record.ApplyApplyDate.Value.ToString("yyyy-MM-dd HH:mm") : "");//打卡时间
                                    //arr.Add("缺卡");//打卡结果
                                    //arr.Add("");//申请补卡记录
                                    //arr.Add("");//异常打卡原因
                                    //arr.Add("");//补卡结果
                                    //arr.Add("");//补卡证明（附件）
                                    //arr.Add("");//加班小时数
                                    //arr.Add("");//加班结果
                                    if (record.Status == (int)AttendanceStatusEnum.补卡)
                                    {
                                        arr.Add("");//打卡时间
                                        arr.Add(record.StatusName);//打卡结果
                                        arr.Add(record.ApplyApplyDate.HasValue ? record.ApplyApplyDate.Value.ToString("yyyy-MM-dd HH:mm") : "");//申请补卡记录
                                        arr.Add(record.ApplyContent);//异常打卡原因
                                        arr.Add(record.ApplyStatus.HasValue ? (AttendanceApplyStatusEnum)record.ApplyStatus : "");//补卡结果
                                        if (!string.IsNullOrEmpty(record.ApplyFileInfo))
                                        {
                                            var slist = JsonConvert.DeserializeObject(record.ApplyFileInfo) as JArray;
                                            string urls = "";
                                            foreach (var item in slist)
                                            {
                                                urls += item["url"] + "\n";

                                            }
                                            arr.Add(urls);//补卡证明（附件）
                                        }
                                        else
                                        {
                                            arr.Add("");//补卡证明（附件）
                                        }
                                        arr.Add("");//加班小时数
                                        arr.Add("");//加班结果
                                    }
                                    else
                                    {
                                        arr.Add(record.AttendanceDate.HasValue ? record.AttendanceDate.Value.ToString("yyyy-MM-dd HH:mm") : "");//打卡时间
                                        arr.Add(record.StatusName);//打卡结果
                                        arr.Add("");//申请补卡记录
                                        arr.Add(!string.IsNullOrEmpty(record.ApplyContent) ? record.ApplyContent : "");//异常打卡原因
                                        arr.Add(record.ApplyStatus.HasValue ? (AttendanceApplyStatusEnum)record.ApplyStatus : "");//补卡结果
                                        if (!string.IsNullOrEmpty(record.ApplyFileInfo))
                                        {
                                            var slist = JsonConvert.DeserializeObject(record.ApplyFileInfo) as JArray;
                                            string urls = "";
                                            foreach (var item in slist)
                                            {
                                                urls += item["url"] + "\n";

                                            }
                                            arr.Add(urls);//补卡证明（附件）
                                        }
                                        else
                                        {
                                            arr.Add("");//补卡证明（附件）
                                        }
                                        arr.Add("");//加班小时数
                                        arr.Add("");//加班结果
                                    }
                                    if (!string.IsNullOrEmpty(record.FileInfo))
                                    {
                                        var slist = JsonConvert.DeserializeObject(record.FileInfo) as JArray;
                                        string urls = "";
                                        foreach (var item in slist)
                                        {
                                            urls += item["url"] + "\n";

                                        }
                                        arr.Add(urls);//外勤图片
                                    }
                                    else
                                    {
                                        arr.Add("");//外勤图片
                                    }
                                    arr.Add(record.Content);//外勤原因
                                    arr.Add(record.MobileDevice);//打卡设备
                                    arr.Add(record.Remark);//系统备注
                                    sheet2.CreateRow(index + 1);
                                    foreach (var value in arr)
                                    {
                                        try
                                        {
                                            Type type = value.GetType();
                                            if (type.FullName == "System.Decimal" || type.FullName == "System.Double" || type.FullName == "System.Int32")
                                            {
                                                ExcelHeler.SetCellValue(workbook, sheet2, font1, index + 1, col, value);
                                            }
                                            else
                                            {
                                                XSSFRichTextString richString = new XSSFRichTextString(value.ToString());
                                                ExcelHeler.SetCellValue(workbook, sheet2, font1, index + 1, col, richString);
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            ExcelHeler.SetCellValue(workbook, sheet2, font1, index + 1, col, value);
                                        }
                                        col++;
                                    }
                                    index++;
                                }
                            }
                        }

                    }
                }
            }









            ISheet sheet3 = workbook.CreateSheet("打卡月份表");
            var font_head = workbook.CreateFont();
            font_head.FontHeightInPoints = 9;
            font_head.FontName = "微软雅黑";
            font_head.IsBold = true;
            double width_head = 13 * 256;//默认宽度
            short height_head = 30 * 20;//默认高度
            List<string> headsFirst = new List<string>();
            List<string> headsSecond = new List<string>();
            headsFirst.Add("姓名");
            headsFirst.Add("客户单位");
            headsFirst.Add("考勤时间");
            headsSecond.Add("姓名");
            headsSecond.Add("客户单位");
            headsSecond.Add("考勤时间");
            foreach (var c in calendars)
            {
                headsFirst.Add("打卡");
                headsSecond.Add(c.Day.HasValue ? c.Day.Value.ToString() + "(" + c.WeekDay + ")" : c.WeekDay);
            }
            ExcelHeler.SetListHeader(workbook, sheet3, font_head, headsFirst, headsSecond, width_head, height_head);
            var font_content = workbook.CreateFont();
            font1.FontHeightInPoints = 9;
            font1.FontName = "微软雅黑";
            index = 1;
            foreach (var entry in entrys)
            {
                List<object> arr = new();
                int col = 0;
                var records = list.Where(t => t.EntryID == entry.ID).ToList();
                //var records = list.Where(t => t.UserName == entry.Name).ToList();
                arr.Add(entry.Name);
                if (records.Count > 0)
                {
                    arr.Add(records[0].CustomerName);
                }
                else
                {
                    arr.Add("");
                }
                arr.Add(start.ToString("yyyy-MM"));
                foreach (var c in calendars)
                {
                    if (c.Calendar.HasValue)
                    {
                        var datas = records.Where(t => t.SignDate.HasValue && t.SignDate.Value.ToString("yyyy-MM-dd") == c.Calendar.Value.ToString("yyyy-MM-dd")).ToList();
                        string content = "";
                        foreach (var d in datas)
                        {
                            content += d.AttendanceDate.HasValue ? d.AttendanceDate.Value.ToString("HH:mm") : "";
                            content += d.StatusName + "\n";
                        }
                        arr.Add(content);
                    }
                    else
                    {
                        arr.Add("");
                    }
                }
                sheet3.CreateRow(index + 1).Height = 50 * 20;//index=1,从第三行开始,前2行是表头
                foreach (var value in arr)
                {
                    try
                    {
                        XSSFRichTextString richString = new XSSFRichTextString(value.ToString());
                        ExcelHeler.SetCellValue(workbook, sheet3, font1, index + 1, col, richString);
                        //ExcelHeler.SetCellRangeValue(workbook, sheet, font1, index + 1, index + 1, col, col, richString);
                    }
                    catch (Exception e)
                    {
                        ExcelHeler.SetCellValue(workbook, sheet3, font1, index + 1, col, value);
                    }
                    col++;
                }
                index++;
            }

            //int start = 7;
            //int last = start;
            //for (int i = 0; i < 5; i++)
            //{
            //    sheet.CreateRow(last);
            //    last++;
            //}
            //string info = string.Format("汇款信息:\n开户名称 :{0}\n开户银行:{1}\n人民币帐号:{2}", model.CooperationName, model.CooperationBank, model.CooperationBankNo);
            //XSSFRichTextString richString = new XSSFRichTextString(info);
            //ExcelHeler.SetCellRangeValue(workbook, sheet, font1, start, last, 0, 0, richString);



            ISheet sheet4 = workbook.CreateSheet("考勤申请记录");

            string[] heads2 = {
                "申请人",
                "客户单位",
                "申请类型",
                "申请时间",
                "申请时长",
                "打卡时间",
                "开始时间",
                "结束时间",
                "添加日期",
                "上传文件",
                "备注"
            };
            ExcelHeler.SetHeaderSingle(workbook, sheet4, heads2, font1, width, height);

            int row = 0;
            foreach (var d in applys)
            {
                List<object> arr = new();
                int col = 0;
                arr.Add(d.UserName);
                arr.Add(d.CustomerName);
                arr.Add(d.ApplyType.HasValue ? (AttendanceApplyTypeEnum)d.ApplyType : "");
                arr.Add(d.SignDate.HasValue ? d.SignDate.Value.ToString("yyyy-MM-dd") : "");
                arr.Add(d.Number.HasValue ? (d.Number.Value.ToString("#0.######") + "小时") : "");
                arr.Add(d.ApplyDate.HasValue ? d.ApplyDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : "");
                arr.Add(d.UpTime.HasValue ? d.UpTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : "");
                arr.Add(d.OffTime.HasValue ? d.OffTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : "");
                arr.Add(d.CreateTime.HasValue ? d.CreateTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : "");
                string file = "";
                if (!string.IsNullOrEmpty(d.FileInfo))
                {
                    var slist = JsonConvert.DeserializeObject(d.FileInfo) as JArray;
                    foreach (var item in slist)
                    {
                        file += item["url"] + "\n";
                    }
                }
                arr.Add(file);
                arr.Add(d.Content);
                sheet4.CreateRow(row + 1);
                foreach (var value in arr)
                {
                    try
                    {
                        Type type = value.GetType();
                        if (type.FullName == "System.Decimal" || type.FullName == "System.Double" || type.FullName == "System.Int32")
                        {
                            ExcelHeler.SetCellValueNoStyle(workbook, sheet4, style, font1, row + 1, col, value);
                        }
                        else
                        {
                            XSSFRichTextString richString = new XSSFRichTextString(value.ToString());
                            ExcelHeler.SetCellValueNoStyle(workbook, sheet4, style, font1, row + 1, col, richString);
                        }
                    }
                    catch (Exception e)
                    {
                        ExcelHeler.SetCellValueNoStyle(workbook, sheet4, style, font1, row + 1, col, value);
                    }
                    //ExcelHeler.SetCellValueNoStyle(workbook, sheet4, style, font1, row + 1, col, value);
                    col++;
                }
                row++;


            }

            byte[] buffer = new byte[1024 * 5];
            using (MemoryStream stream = new MemoryStream())
            {
                workbook.Write(stream);
                buffer = stream.ToArray();
            }
            return buffer;
        }






    }
}
