using LZ.Common;
using LZ.Core.Interface;
using LZ.Finance.Service.Interface;
using LZ.Model.PagedModel;
using LZ.Model;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using SqlSugar.IOC;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using NPOI.SS.Formula.Functions;
using LZ.Core;

namespace LZ.Finance.Service.Services
{
    internal class BudgetReportService : ServicesBase<Live_BudgetReport>, IBudgetReportService, IScopedDependency
    {
        public async Task<List<Live_BudgetReport>> List(Expression<Func<Live_BudgetReport, bool>> exp, PagedModelBase query, string order = "")
        {
            var count = new RefAsync<int>();
            var sco = base.Context.Queryable<Live_BudgetReport>().Where(exp);
            //sco.OrderBy(t => t.InjuryTime, OrderByType.Desc);
            //sco.OrderBy(t => t.ShowTime, OrderByType.Desc);
            if (!string.IsNullOrEmpty(order))
            {
                sco.OrderBy(order);
            }
            else
            {
                sco.OrderBy(t => t.Year);
            }
            if (query.Page == 0)
            {
                return await sco.ToListAsync();
            }
            var list = await sco.ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<int> Add(Live_BudgetReport model)
        {
            return await base.Context.Insertable(model).ExecuteReturnIdentityAsync();
        }
        public async Task<int> Add(List<Live_BudgetReport> list)
        {
            return await base.Context.Insertable(list).ExecuteCommandAsync();
        }

        public async Task<bool> Exist(int id)
        {
            return await base.Context.Queryable<Live_BudgetReport>().AnyAsync(it => it.ID == id);
        }

        public async Task<bool> Exist(Expression<Func<Live_BudgetReport, bool>> exp)
        {
            return await base.Context.Queryable<Live_BudgetReport>().AnyAsync(exp);
        }

        public async Task<bool> Update(Live_BudgetReport model)
        {
            return await base.Context.Updateable<Live_BudgetReport>(model).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Update(List<Model.Live_BudgetReport> list)
        {
            return await base.Context.Updateable<Live_BudgetReport>(list).ExecuteCommandAsync() > 0;
        }

        public async Task<Live_BudgetReport> Get(int id)
        {
            return await base.Context.Queryable<Live_BudgetReport>().FirstAsync(it => it.ID == id);
        }
        public async Task<Model.Live_BudgetReport> Get(Expression<Func<Live_BudgetReport, bool>> exp)
        {
            return await base.Context.Queryable<Live_BudgetReport>().Where(exp).FirstAsync();
        }


        public async Task<bool> Delete(int id)
        {
            return await base.Context.Deleteable<Live_BudgetReport>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
        public async Task<int> DictionarySet(Dictionary<string, object> dt)
        {
            return await base.Context.Updateable<Model.Live_BudgetReport>(dt).With(SqlWith.UpdLock).ExecuteCommandAsync();
        }
        public async Task<byte[]> ReportExcel(List<Model.Live_BudgetReportConfig> configs, List<Model.Live_BudgetReportDetail> details,List<Model.Live_ProfitManage> profit)
        {

            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("Sheet1");
            var font = workbook.CreateFont();
            font.FontHeightInPoints = 9;
            font.FontName = "微软雅黑";
            font.IsBold = true;
            double width = 11 * 256;
            short height = 20 * 20;

            sheet.DefaultRowHeight = height;
            #region excel 表头
            string[] heads1 = {
                "项目",
                "一季度","一季度","一季度","一季度",
                "二季度","二季度","二季度","二季度",
                "三季度","三季度","三季度","三季度",
                "四季度","四季度","四季度","四季度",
                "合计"

            };

            string[] heads2 = {
                "项目",
                "4月","5月","6月","小计",
                "7月","8月","9月","小计",
                "10月","11月","12月","小计",
                "1月","2月","3月","小计",
                "合计"

            };
            sheet.DefaultRowHeight = height;
            sheet.SetColumnWidth(0, 35 * 256);
            for (int x = 1; x < heads1.Length; x++)
            {

                sheet.SetColumnWidth(x, width);
            }
            ExcelHeler.SetHeaderNoSetWidth(workbook, sheet, font, heads1, heads2, width, height);

            #endregion

            for (int y = 2; y < 41; y++)
            {
               
                sheet.CreateRow(y);
            }
            #region 表格主体
            var font_blue = workbook.CreateFont();
            font_blue.FontHeightInPoints = 9;
            font_blue.FontName = "微软雅黑";

            //ExcelHeler.SetCellValue(workbook, sheet, font_blue, 2, 0, 1231, (short)29);
            //ExcelHeler.SetCellValue(workbook, sheet, font_blue, 3, 0, 1231, (short)44);
            int[] Quarters = { 0, 1, 2, 3 };
            int[] months = { 0, 1, 2, 3 };
            Dictionary<string, int> dic = new Dictionary<string, int>();
            dic.Add("营业收入", 1);
            dic.Add("减：直接成本", 2);
            dic.Add("毛利", 0);
            dic.Add("减：管理成本", 0);
            dic.Add("行政费用", 3);
            dic.Add("财务费用", 4);
            dic.Add("人工成本", 5);
            dic.Add("经营利润", 0);
            //项目名称
            int index = 2;//行
           
            foreach (var d in dic)
            {
                if (d.Value != 0)
                {
                    var list = configs.Where(t => t.CID == d.Value).ToList();
                    ExcelHeler.SetCellValue(workbook, sheet, font_blue, index, 0, d.Key, (short)44);
                    index++;
                    foreach (var l in list)
                    {
                        ExcelHeler.SetCellValue(workbook, sheet, font_blue, index, 0, l.CostName);
                        index++;
                    }
                    
                   
                }
                if(d.Value==0 )
                {
                    ExcelHeler.SetCellValue(workbook, sheet, font_blue, index, 0, d.Key, (short)29);
                    index++;
                }

            }
            int row = 1;//列
            int guanlicol = 0;
            List<decimal> heji = new List<decimal>();
            
            // 月份数据
            foreach (var q in Quarters)
            {
                foreach (var m in months)
                {
                    index = 2;
                    List<decimal> decimals = new List<decimal>();
                    foreach (var d in dic)
                    {
                        decimal totalMoney = 0;

                        if (d.Value != 0)
                        {

                            var list = configs.Where(t => t.CID == d.Value).ToList();
                            int sign = index;
                            if (q == 0 && m==3)
                            {
                                heji.Add(0M);
                            }
                            index++;
                            foreach (var l in list)
                            {


                                decimal money = 0;
                                var costname = m < 3 ? l.CostName : "小计";
                                var detail = details.Find(t => t.Title == costname && t.BudgetReportConfigID == l.ID && t.QuarterType == q && t.MonthType == m);
                                if (detail != null)
                                {
                                    money += detail.Money;
                                }

                                if (l.CostName == "人数")
                                {
                                    ExcelHeler.SetCellValue(workbook, sheet, font_blue, index, row, (int)money);
                                }
                                else
                                {
                                    totalMoney += money;
                                    ExcelHeler.SetCellValue(workbook, sheet, font_blue, index, row, money);
                                }
                                if (m == 3)//小计
                                {
                                    if (heji.Count < index - 1)
                                    {
                                        heji.Add(money);
                                    } else
                                    {
                                        heji[index - 2] += money;
                                    }
                                }

                                index++;

                            }
                            if (m == 3)
                            {
                                heji[sign - 2] += totalMoney;
                            }

                            ExcelHeler.SetCellValue(workbook, sheet, font_blue, sign, row, totalMoney, (short)44);
                            decimals.Add(totalMoney);
                        }
                        if (d.Value == 0 && d.Key == "毛利")
                        {
                            //毛利
                            var maoli = decimals[0] - decimals[1];
                            decimals.Add(maoli);
                            if (m == 3)//小计
                            {
                                if (heji.Count < index - 1)
                                {
                                    heji.Add(maoli);
                                }
                                else
                                {
                                    heji[index - 2] += maoli;
                                }
                            }
                            ExcelHeler.SetCellValue(workbook, sheet, font_blue, index, row, maoli, (short)29);
                            index++;
                        }
                        if (d.Value == 0 && d.Key == "经营利润")
                        {
                            //经营利润
                            var lirun = decimals[2] - decimals[3] - decimals[4] - decimals[5];
                            decimals.Add(lirun);
                            var guanli = decimals[3] + decimals[4] + decimals[5];
                           
                            if (m == 3)//小计
                            {
                                heji[guanlicol - 2] += guanli;
                                if (heji.Count < index - 1)
                                {
                                    heji.Add(lirun);
                                }
                                else
                                {
                                    heji[index - 2] += lirun;
                                }
                            }
                            ExcelHeler.SetCellValue(workbook, sheet, font_blue, guanlicol, row, guanli, (short)29);
                            ExcelHeler.SetCellValue(workbook, sheet, font_blue, index, row, lirun, (short)29);
                            index++;
                        }
                        if (d.Value == 0 && d.Key == "减：管理成本")
                        {
                            //管理成本
                            guanlicol = index;//先记录行数，等获取数据后在做计算
                            if (q == 0 && m == 3)
                            {
                                heji.Add(0M);
                            }
                            index++;
                            
                            
                        }

                    }
                    row++;
                }

            }


            index = 2;
            //ExcelHeler.SetCellValue(workbook, sheet, font_blue, index, row, 11);

            
            foreach (var x in heji)
            {

                ExcelHeler.SetCellValue(workbook, sheet, font_blue, index, row,x);
                index++;
            }



            #endregion


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
