using LZ.Model.PagedModel;
using LZ.Model;
using SqlSugar.IOC;
using SqlSugar;
using System;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LZ.Core.Interface;
using LZ.Finance.Service.Interface;
using LZ.Common;
using Consul;
using NPOI.SS.Formula.Functions;
using LZ.Model.Enums.Budget;
using LZ.Core;

namespace LZ.Finance.Service.Services
{
    public class BudgetApplyRecordService : ServicesBase<Live_BudgetApplyRecord>, IBudgetApplyRecordService, IScopedDependency
    {
        public async Task<List<Live_BudgetApplyRecord>> List(Expression<Func<Live_BudgetApplyRecord, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_BudgetApplyRecord>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_BudgetApplyRecord>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<int> Add(Live_BudgetApplyRecord model)
        {
            return await base.Context.Insertable(model).ExecuteReturnIdentityAsync();
        }
        public async Task<int> Add(List<Live_BudgetApplyRecord> list)
        {
            return await base.Context.Insertable(list).ExecuteCommandAsync();
        }

        public async Task<bool> Exist(int id)
        {
            return await base.Context.Queryable<Live_BudgetApplyRecord>().AnyAsync(it => it.ID == id);
        }

        public async Task<bool> Exist(Expression<Func<Live_BudgetApplyRecord, bool>> exp)
        {
            return await base.Context.Queryable<Live_BudgetApplyRecord>().AnyAsync(exp);
        }

        public async Task<bool> Update(Live_BudgetApplyRecord model)
        {
            return await base.Context.Updateable<Live_BudgetApplyRecord>(model).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Update(List<Model.Live_BudgetApplyRecord> list)
        {
            return await base.Context.Updateable<Live_BudgetApplyRecord>(list).ExecuteCommandAsync() > 0;
        }

        public async Task<Live_BudgetApplyRecord> Get(int id)
        {
            return await base.Context.Queryable<Live_BudgetApplyRecord>().FirstAsync(it => it.ID == id);
        }
        public async Task<Model.Live_BudgetApplyRecord> Get(Expression<Func<Live_BudgetApplyRecord, bool>> exp)
        {
            return await base.Context.Queryable<Live_BudgetApplyRecord>().Where(exp).FirstAsync();
        }


        public async Task<bool> Delete(int id)
        {
            return await base.Context.Deleteable<Live_BudgetApplyRecord>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
        public async Task<int> DictionarySet(Dictionary<string, object> dt)
        {
            return await base.Context.Updateable<Model.Live_BudgetApplyRecord>(dt).With(SqlWith.UpdLock).ExecuteCommandAsync();
        }

        public byte[] CreateDefaultExcel()
        {
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("Sheet1");
            var font = workbook.CreateFont();
            font.FontHeightInPoints = 9;
            font.FontName = "微软雅黑";
            font.IsBold = true;
            double width = 20 * 256;
            short height = 25 * 20;

            sheet.DefaultRowHeight = height;
            
            string[] heads = {"服务商","金额","备注"};
            ExcelHeler.SetHeaderSingle(workbook, sheet, heads, font, width, height);

            byte[] buffer = new byte[1024 * 5];
            using (MemoryStream stream = new MemoryStream())
            {
                workbook.Write(stream);
                buffer = stream.ToArray();
            }
            return buffer;
        }

        public byte[] FinanceExportExcel(List<Live_BudgetApplyRecord> list, List<Live_V_BudgetApplyRecordDetail> details)
        {
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("预算申请");
            var font = workbook.CreateFont();
            font.FontHeightInPoints = 9;
            font.FontName = "微软雅黑";
            font.IsBold = true;
            double width = 20 * 256;
            short height = 25 * 20;

            sheet.DefaultRowHeight = height;

            string[] heads = { 
                "年度",
                "申请时间", 
                "事业合伙人",
                "成本归属单位", 
                "成本类型", 
                "费用名称",
                "法人主体",
                "费用用途",
                "申请金额",
                "支付方式",
                "收款单位",
                "收款单位所属行",
                "收款单位卡号",
                "申请人",
                "打款时间",
                "流程状态",
                "费用备注"
            };
            double width1 = 15 * 256;
            double width2 = 25 * 256;
            double width3 = 35 * 256;
            //for (int x = 0; x < heads.Length -1; x++)
            //{

            //    sheet.SetColumnWidth(x, width);
            //}
            sheet.SetColumnWidth(0, width1);//年度
            sheet.SetColumnWidth(1, width1);//申请时间
            sheet.SetColumnWidth(2, width1);//事业合伙人
            sheet.SetColumnWidth(3, width1);//成本归属单位
            sheet.SetColumnWidth(4, width1);//成本类型
            sheet.SetColumnWidth(5, width2);//费用名称
            sheet.SetColumnWidth(6, width3);//法人主体
            sheet.SetColumnWidth(7, width3);//费用用途
            sheet.SetColumnWidth(8, width1);//申请金额
            sheet.SetColumnWidth(9, width1);//支付方式
            sheet.SetColumnWidth(10, width3);//收款单位
            sheet.SetColumnWidth(11, width3);//收款单位所属行
            sheet.SetColumnWidth(12, width2);//收款单位卡号
            sheet.SetColumnWidth(13, width1);//申请人
            sheet.SetColumnWidth(14, width1);//打款时间
            sheet.SetColumnWidth(15, width1);//流程状态
            sheet.SetColumnWidth(16, width3);//费用备注

            sheet.SetColumnWidth(heads.Length -1 , 40*256);
            ExcelHeler.SetHeaderSingleNoSetWidth(workbook, sheet, heads, font, width, height);
            ICellStyle style = workbook.CreateCellStyle();//在外部创建，可避免数量过大报错
            var font1 = workbook.CreateFont();
            font1.FontHeightInPoints = 9;
            font1.FontName = "微软雅黑";
            int index = 0;
            foreach (var x in list)
            {
                List<object> arr = new();
                int col = 0;
                arr.Add(x.Year.ToString() ?? "");
                arr.Add(x.CreateTime.Value.ToString("yyyy-MM-dd"));
                arr.Add(x.PartnerName);
                arr.Add("/");
                if(x.CostType.HasValue)
                {
                    arr.Add(((CostType)x.CostType).ToString());
                } 
                else
                {
                    arr.Add("/");
                }

                arr.Add(x.ExpensesTitle);
                arr.Add(x.Payer);
                arr.Add(x.Description);
                arr.Add(x.ApplyAmount);
                arr.Add(x.PayType);
                arr.Add(x.ReceivePaymentCompanyName);
                arr.Add(x.Bank);
                arr.Add(x.BankNo);
                arr.Add(x.UserName);
                arr.Add(x.PayTime.HasValue ? x.PayTime : "");
                if(x.ProcessStatus.HasValue)
                {
                    arr.Add(((BudgetApplyRecordProcessStatus)x.ProcessStatus).ToString());
                }
                else
                {
                    arr.Add("/");
                }
                arr.Add(x.Remark);

                sheet.CreateRow(index + 1);//index=1,从第三行开始,前2行是表头
                foreach (var value in arr)
                {
                    ExcelHeler.SetCellValueNoStyle(workbook, sheet, style, font1, index + 1, col, value);
                    //ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, col, value);
                    col++;
                }
                index++;
            }

            ISheet sheet1 = workbook.CreateSheet("预算申请详细");

            string[] heads1 = {
                "年度",
                "申请时间",
                "事业合伙人",
                "成本区域",
                "成本归属单位",
                "成本类型",
                "费用名称",
                "法人主体",
                "费用用途",
                "申请金额",
                "详细金额",
                "支付方式",
                "收款单位",
                "收款单位所属行",
                "收款单位卡号",
                "申请人",
                "打款时间",
                "流程状态",
                "费用备注",
                "详情成本归属"
            };
            ExcelHeler.SetHeaderSingle(workbook, sheet1, heads1, font, width, height);

            int row = 0;
            var details_select = details.Where(t => t.ProcessStatus.HasValue);
            foreach (var d in details_select)
            {
                List<object> arr = new();
                var x = list.Find(t => t.ID == d.BudgetApplyRecordID);
                int col = 0;
                if (x == null) continue;
                arr.Add(x.Year ?? 0);
                arr.Add(x.CreateTime.Value.ToString("yyyy-MM-dd"));
                arr.Add(x.PartnerName);
                arr.Add(d.PartnerName);
                arr.Add("/");
                arr.Add(x.CostType.HasValue ? ((CostType)x.CostType).ToString() : "/");
                arr.Add(x.ExpensesTitle);
                arr.Add(x.Payer);
                arr.Add(x.Description);
                arr.Add(x.ApplyAmount);
                arr.Add(d.Money);
                arr.Add(x.PayType);
                arr.Add(x.ReceivePaymentCompanyName);
                arr.Add(x.Bank);
                arr.Add(x.BankNo);
                arr.Add(x.UserName);
                arr.Add(x.PayTime.HasValue ? x.PayTime : "");
                if(x.ProcessStatus.HasValue)
                {
                    arr.Add(((BudgetApplyRecordProcessStatus)x.ProcessStatus).ToString());
                } else
                {
                    arr.Add("/");
                }
                arr.Add(x.Remark ?? "");
                arr.Add("/");
                sheet1.CreateRow(row + 1);
                foreach (var value in arr)
                {
                    ExcelHeler.SetCellValueNoStyle(workbook, sheet1, style, font1, row + 1, col, value);
                    //ExcelHeler.SetCellValue(workbook, sheet1, font1, row + 1, col, value);
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
