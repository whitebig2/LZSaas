using LZ.Core.Interface;
using LZ.Model;
using LZ.Users.Service.Interface;
using SqlSugar.IOC;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LZ.Common;
using LZ.Model.Enums;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using LZ.Core;

namespace LZ.Users.Service.Services
{
    public class InjuryPaymentService : ServicesBase<Live_InjuryPayment>, IInjuryPaymentService, IScopedDependency
    {
        public async Task<List<Model.Live_V_InjuryPayment>> List(Expression<Func<Model.Live_V_InjuryPayment, bool>> exp, Model.PagedModel.UserPagedModel query, string order = "")
        {
            var count = new RefAsync<int>();
            var sco = base.Context.Queryable<Live_V_InjuryPayment>().Where(exp);
            if (!string.IsNullOrEmpty(order))
            {
                sco.OrderBy(order);
            }

            if (query.Page == 0)
            {
                return await sco.ToListAsync();
            }
            var list = await sco.ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<int> Add(Model.Live_InjuryPayment model)
        {
            var db = base.Context;
            return await db.Insertable(model).ExecuteCommandAsync();
        }
        public async Task<bool> Update(Model.Live_InjuryPayment model)
        {
            var db = base.Context;
            return await db.Updateable(model).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Delete(int id)
        {
            var db = base.Context;
            return await db.Deleteable<Model.Live_InjuryPayment>().Where(t => t.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Delete(Expression<Func<Model.Live_InjuryPayment, bool>> exp)
        {
            var db = base.Context;
            return await db.Deleteable<Model.Live_InjuryPayment>().Where(exp).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Exist(Expression<Func<Live_InjuryPayment, bool>> exp)
        {
            return await base.Context.Queryable<Live_InjuryPayment>().AnyAsync(exp);
        }
        public async Task<Model.Live_InjuryPayment> Get(int id)
        {
            var db = base.Context;
            return await db.Queryable<Model.Live_InjuryPayment>().Where(t => t.ID == id).FirstAsync();
        }

        public async Task<bool> UpdateStatus(int id, int status, string msg = "")
        {

            var db = base.Context;
            var ub = db.Updateable<Live_InjuryPayment>();
            if (status > 0)
            {
                ub.SetColumns(it => it.Status == status);
            }
            if (!string.IsNullOrEmpty(msg))
            {
                ub.SetColumns(it => it.Message == msg);
            }
            ub.Where(t => t.ID == id);
            return await ub.ExecuteCommandAsync() > 0;
        }
        public async Task<Model.Live_InjuryPayment> GetByInjuryID(int id)
        {
            var db = base.Context;
            return await db.Queryable<Model.Live_InjuryPayment>().Where(t => t.InjuryID == id).FirstAsync();
        }
        public async Task<bool> ExistID(int id)
        {
            return await base.Context.Queryable<Live_InjuryPayment>().AnyAsync(it => it.ID == id);
        }

        public byte[] ExportExcel(List<Live_V_InjuryPayment> list)
        {
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("工伤支付信息");
            var font = workbook.CreateFont();
            font.FontHeightInPoints = 9;
            font.FontName = "微软雅黑";
            font.IsBold = true;
            double width = 14 * 256;//默认宽度
            short height = 20 * 20;//默认高度

            sheet.DefaultRowHeight = height;
            #region excel 表头
            string[] heads = {
                "打款主体",
                "申请时间",
                "费用名称",
                "服务商",
                "员工姓名",
                "身份证号码",
                "费用用途",
                "收款人姓名",
                "银行名称",
                "银行卡号",
                "打款金额",
                "备注"
            };
            double width1 = 15 * 256;
            double width2 = 25 * 256;
            double width3 = 35 * 256;
            //for (int x = 0; x < heads.Length -1; x++)
            //{

            //    sheet.SetColumnWidth(x, width);
            //}
            sheet.SetColumnWidth(0, width2);
            sheet.SetColumnWidth(1, width1);
            sheet.SetColumnWidth(2, width1);
            sheet.SetColumnWidth(3, width1);
            sheet.SetColumnWidth(4, width2);
            sheet.SetColumnWidth(5, width2);
            sheet.SetColumnWidth(6, width2);
            sheet.SetColumnWidth(7, width1);
            sheet.SetColumnWidth(8, width2);
            sheet.SetColumnWidth(9, width2);
            sheet.SetColumnWidth(10, width1);
            sheet.SetColumnWidth(11, width3);

            ExcelHeler.SetHeaderSingleNoSetWidth(workbook, sheet, heads, font, width, height);

            #endregion

            var font1 = workbook.CreateFont();
            font1.FontHeightInPoints = 9;
            font1.FontName = "微软雅黑";
            int index = 0;

            foreach (var x in list)
            {
                List<object> arr = new();
                int col = 0;
                arr.Add(x.PaymentSubject);
                arr.Add(x.ApplicationTime.HasValue?x.ApplicationTime.Value.ToString("yyyy-MM-dd"):"/");
                arr.Add(x.CostName);
                arr.Add(x.PartnerName);
                arr.Add(x.UserName);
                arr.Add(x.UserCardNo);
                arr.Add(x.Purpose);
                arr.Add(x.PayeeName);
                arr.Add(x.PayeeBank);
                arr.Add(x.PayeeBankNo);
                arr.Add(x.PaymentAmount);
                arr.Add(x.Remark);

                sheet.CreateRow(index + 1);
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
