using LZ.Core.Interface;
using LZ.Finance.Service.Interface;
using LZ.Model.PagedModel;
using LZ.Model;
using SqlSugar.IOC;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LZ.Common;
using LZ.Model.Enums.Budget;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using LZ.Core;

namespace LZ.Finance.Service.Services
{
    public class AttendancePrepaidApplyCustomerService : ServicesBase<Live_AttendancePrepaidApplyCustomer>, IAttendancePrepaidApplyCustomerService, IScopedDependency
    {
        public async Task<List<Model.Live_V_AttendancePrepaidApplyCustomer>> List(Expression<Func<Model.Live_V_AttendancePrepaidApplyCustomer, bool>> exp, FinancePageModel query)
        {
            var count = new RefAsync<int>();
            var sco = DbScoped.SugarScope.Queryable<Live_V_AttendancePrepaidApplyCustomer>().Where(exp);
            if (!string.IsNullOrEmpty(query.Order))
            {
                sco.OrderBy(query.Order);
            }
            else
            {
                sco.OrderByDescending(t => t.CreateTime);
            }
            if (query.Page == 0)
            {
                return await sco.ToListAsync();
            }
            
            var list = await sco.ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<int> Add(Model.Live_AttendancePrepaidApplyCustomer model)
        {
            var db = DbScoped.SugarScope;
            return await db.Insertable(model).ExecuteReturnIdentityAsync();
        }

        public async Task<int> Add(List<Live_AttendancePrepaidApplyCustomer> list)
        {
            var db = DbScoped.SugarScope;
            return await db.Insertable(list).ExecuteCommandAsync();
        }
        public async Task<bool> Update(Model.Live_AttendancePrepaidApplyCustomer model)
        {
            var db = DbScoped.SugarScope;
            return await db.Updateable(model).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Delete(int id)
        {
            var db = DbScoped.SugarScope;
            return await db.Deleteable<Model.Live_AttendancePrepaidApplyCustomer>().Where(t => t.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Delete(Expression<Func<Live_AttendancePrepaidApplyCustomer, bool>> exp)
        {
            var db = DbScoped.SugarScope;
            return await db.Deleteable<Model.Live_AttendancePrepaidApplyCustomer>().Where(exp).ExecuteCommandAsync() > 0;
        }
        public async Task<Model.Live_AttendancePrepaidApplyCustomer> Get(int id)
        {
            var db = DbScoped.SugarScope;
            return await db.Queryable<Model.Live_AttendancePrepaidApplyCustomer>().Where(t => t.ID == id).FirstAsync();
        }

        public async Task<Live_V_AttendancePrepaidApplyCustomer> GetView(int id)
        {
            var db = DbScoped.SugarScope;
            return await db.Queryable<Model.Live_V_AttendancePrepaidApplyCustomer>().Where(t => t.ID == id).FirstAsync();
        }

        public async Task<Live_AttendancePrepaidApplyCustomer> Get(Expression<Func<Live_AttendancePrepaidApplyCustomer, bool>> exp)
        {
            var db = DbScoped.SugarScope;
            return await db.Queryable<Model.Live_AttendancePrepaidApplyCustomer>().Where(exp).FirstAsync();
        }
        public async Task<bool> ExistID(int id)
        {
            return await DbScoped.SugarScope.Queryable<Live_AttendancePrepaidApplyCustomer>().AnyAsync(it => it.ID == id);
        }

        public async Task<bool> Exist(Expression<Func<Live_AttendancePrepaidApplyCustomer, bool>> exp)
        {
            return await DbScoped.SugarScope.Queryable<Live_AttendancePrepaidApplyCustomer>().AnyAsync(exp);
        }

        public async Task<int> DictionarySet(Dictionary<string, object> dt)
        {
            return await DbScoped.SugarScope.Updateable<Model.Live_AttendancePrepaidApplyCustomer>(dt).With(SqlWith.UpdLock).ExecuteCommandAsync();
        }

        public byte[] ExportExcel(List<Live_V_AttendancePrepaidApplyRecord> list)
        {
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("日结结算");
            var font = workbook.CreateFont();
            font.FontHeightInPoints = 9;
            font.FontName = "微软雅黑";
            font.IsBold = true;
            double width = 20 * 256;
            short height = 25 * 20;

            sheet.DefaultRowHeight = height;

            string[] heads = {
                "客户名称",
                "岗位编号",
                "员工姓名",
                "员工编号",
                "身份证号码",
                "手机号码",
                "工资所属月",
                "工作类型",
                "结算时长(小时)",
                "发包价(元/小时)",
                "发包结算金额(元)",
                "接单结算标准(元/小时)",
                "接单结算金额(元)",
                "服务费"
            };
            double width1 = 15 * 256;
            double width2 = 25 * 256;
            double width3 = 35 * 256;
            //for (int x = 0; x < heads.Length -1; x++)
            //{

            //    sheet.SetColumnWidth(x, width);
            //}
            sheet.SetColumnWidth(0, width3);//客户名称
            sheet.SetColumnWidth(1, width3);//岗位编号
            sheet.SetColumnWidth(2, width1);//员工姓名
            sheet.SetColumnWidth(3, width2);//员工编号
            sheet.SetColumnWidth(4, width2);//身份证号码
            sheet.SetColumnWidth(5, width2);//手机号码
            sheet.SetColumnWidth(6, width1);//工资所属月
            sheet.SetColumnWidth(7, width1);//工作类型
            sheet.SetColumnWidth(8, width1);//结算时长
            sheet.SetColumnWidth(9, width1);//发包价
            sheet.SetColumnWidth(10, width1);//发包结算金额
            sheet.SetColumnWidth(11, width1);//接单结算标准
            sheet.SetColumnWidth(12, width1);//接单结算金额
            sheet.SetColumnWidth(13, width1);//服务费

            ExcelHeler.SetHeaderSingleNoSetWidth(workbook, sheet, heads, font, width, height);

            var font1 = workbook.CreateFont();
            font1.FontHeightInPoints = 9;
            font1.FontName = "微软雅黑";
            int index = 0;
            foreach (var x in list)
            {
                List<object> arr = new();
                int col = 0;
                arr.Add(x.CustomerName ?? "");
                arr.Add(x.JobNo);
                arr.Add(x.UserName);
                arr.Add(x.EntryNo);
                arr.Add(x.CardID);
                arr.Add(x.Phone);
                arr.Add(x.BillMonth);
                arr.Add(x.DayWorkType);
                arr.Add(x.ShiftHours ?? 0M);
                arr.Add(x.ShiftPrice ?? 0M);
                arr.Add(x.ShiftMoney ?? 0M);
                arr.Add(x.JobPrice ?? 0M);
                arr.Add(x.JobMoney ?? 0M);
                arr.Add((x.ShiftMoney ?? 0M) - (x.JobMoney ?? 0M));
                sheet.CreateRow(index + 1);//index=1,从第三行开始,前2行是表头
                foreach (var value in arr)
                {
                    ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, col, value);
                    col++;
                }
                index++;
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
