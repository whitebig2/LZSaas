using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using NPOI.SS.UserModel;
using System.Diagnostics.Eventing.Reader;
using Microsoft.VisualBasic;
using NPOI.SS.Util;
using Consul;
using System.Drawing;
using NPOI.XSSF.UserModel;
using SixLabors.ImageSharp.Drawing;
using NPOI.SS.Formula.Functions;

namespace LZ.Common
{
    public class ExcelHeler
    {
        /// <summary>
        /// 读取Excel多Sheet数据
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="sheetName">Sheet名</param>
        /// <returns></returns>
        public static DataSet ReadExcelToDataSet(string filePath, string sheetName =null, int startrow = 0)
        {
            filePath = Directory.GetCurrentDirectory() + filePath;
            if (!File.Exists(filePath))
            {
                return null;
            }
            //获取文件信息
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            IWorkbook workbook = WorkbookFactory.Create(fs);
            //获取sheet信息
            ISheet sheet = null;
            DataSet ds = new DataSet();
            if (!string.IsNullOrEmpty(sheetName))
            {
                sheet = workbook.GetSheet(sheetName);
                if (sheet == null)
                {
                    //logger.LogError($"{filePath}未找到sheet:{sheetName}");
                    fs.Dispose();
                    File.Delete(filePath);
                    return null;
                }
                DataTable dt = ReadExcelFunc(workbook, sheet, startrow);
                ds.Tables.Add(dt);
            }
            else
            {
                //遍历获取所有数据
                int sheetCount = workbook.NumberOfSheets;
                for (int i = 0; i < sheetCount; i++)
                {
                    sheet = workbook.GetSheetAt(i);
                    if (sheet != null)
                    {
                        DataTable dt = ReadExcelFunc(workbook, sheet, startrow);
                        if (dt != null) ds.Tables.Add(dt);
                    }
                }
            }
            fs.Dispose();
            File.Delete(filePath);
            return ds;
        }

        //设置单表头数据
        public static void SetHeaderSingle(IWorkbook workbook, ISheet sheet, string[] heads,IFont font, double width = 20 * 256, short height = 17 * 20, int row = 0)
        {
            sheet.DefaultRowHeight = height;
            for (int x = 0; x < heads.Length; x++)
            {
                sheet.SetColumnWidth(x, width);
            }
            sheet.CreateRow(row);
            ICellStyle style = workbook.CreateCellStyle();
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderTop = BorderStyle.Thin;
            style.SetFont(font);
            for (int j = 0; j < heads.Length; j++)
            {
                var cell = sheet.GetRow(row).GetCell(j) ?? sheet.GetRow(row).CreateCell(j);
                cell.CellStyle = style;
                cell.SetCellValue($"{heads[j]}");
            }
        }

        public static void SetHeaderSingleNoSetWidth(IWorkbook workbook, ISheet sheet, string[] heads, IFont font, double width = 20 * 256, short height = 17 * 20, int row = 0)
        {
            sheet.CreateRow(row);
            ICellStyle style = workbook.CreateCellStyle();
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderTop = BorderStyle.Thin;
            style.SetFont(font);
            for (int j = 0; j < heads.Length; j++)
            {
                var cell = sheet.GetRow(row).GetCell(j) ?? sheet.GetRow(row).CreateCell(j);
                cell.CellStyle = style;
                cell.SetCellValue($"{heads[j]}");
            }
        }

        public static void SetCellValue(IWorkbook workbook,ISheet sheet, IFont font, int row, int index,object value,short? color = null)
        {
            var cell = sheet.GetRow(row).GetCell(index) ?? sheet.GetRow(row).CreateCell(index);
            
            ICellStyle style = workbook.CreateCellStyle();
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderTop = BorderStyle.Thin;
            style.WrapText = true;
            style.IsHidden = true;
            if (color.HasValue)
            {
                style.FillForegroundColor = color.Value;
                style.FillPattern = FillPattern.SolidForeground;
            }
            style.SetFont(font);
            cell.CellStyle = style;
            SetValue(value, workbook, cell, style);
            //short percentFormat = .GetFormat("0.00%");
            //cell.CellStyle.DataFormat = percentFormat;
        }
        public static void CommonSetCell(IWorkbook workbook, ISheet sheet, ICellStyle style, int row, int index, object value)
        {
            var cell = sheet.GetRow(row).GetCell(index) ?? sheet.GetRow(row).CreateCell(index);
            SetValue(value, workbook, cell, style);
        }

        public static void SetCellValueNoStyle(IWorkbook workbook, ISheet sheet, ICellStyle style, IFont font, int row, int index, object value, short? color = null)
        {
            var cell = sheet.GetRow(row).GetCell(index) ?? sheet.GetRow(row).CreateCell(index);
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderTop = BorderStyle.Thin;
            style.IsHidden = true;
            if (color.HasValue)
            {
                style.FillForegroundColor = color.Value;
                style.FillPattern = FillPattern.SolidForeground;
            }
            style.SetFont(font);
            cell.CellStyle = style;
            SetValue(value, workbook, cell, style);
            //short percentFormat = .GetFormat("0.00%");
            //cell.CellStyle.DataFormat = percentFormat;
        }

        public static void CommonSetCellRangeValue(IWorkbook workbook, ISheet sheet, ICellStyle style, int firstrow, int lastrow, int firstcol, int lastcol, object value)
        {
            var region1 = new CellRangeAddress(firstrow, lastrow, firstcol, lastcol);
            sheet.AddMergedRegion(region1);
            var cell = sheet.GetRow(firstrow).GetCell(firstcol) ?? sheet.GetRow(firstrow).CreateCell(firstcol);
            SetValue(value, workbook, cell, style);
            RegionUtil.SetBorderBottom(BorderStyle.Thin, region1, sheet);
            RegionUtil.SetBorderRight(BorderStyle.Thin, region1, sheet);
            RegionUtil.SetBorderLeft(BorderStyle.Thin, region1, sheet);
            RegionUtil.SetBorderBottom(BorderStyle.Thin, region1, sheet);

        }

        public static void SetCellRangeValue(IWorkbook workbook, ISheet sheet, IFont font, int firstrow, int lastrow,int firstcol,int lastcol, object value)
        {
            var region1 = new CellRangeAddress(firstrow, lastrow, firstcol, lastcol);
            sheet.AddMergedRegion(region1);
            var cell = sheet.GetRow(firstrow).GetCell(firstcol) ?? sheet.GetRow(firstrow).CreateCell(firstcol);
            ICellStyle style = workbook.CreateCellStyle();
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderTop = BorderStyle.Thin;
            style.WrapText = true;
            //style.IsHidden = true;
            style.SetFont(font);
            cell.CellStyle = style;
            //cell.SetCellValue($"{value}");
            SetValue(value, workbook, cell,style);
            RegionUtil.SetBorderBottom(BorderStyle.Thin, region1, sheet);
            RegionUtil.SetBorderRight(BorderStyle.Thin, region1, sheet);
            RegionUtil.SetBorderLeft(BorderStyle.Thin, region1, sheet);
            RegionUtil.SetBorderBottom(BorderStyle.Thin, region1, sheet);
        }

        public static void SetCellRangeValueWithStyle(IWorkbook workbook, ISheet sheet, IFont font, int firstrow, int lastrow, int firstcol, int lastcol, object value,int? alignmentStyle = null)
        {
            var region1 = new CellRangeAddress(firstrow, lastrow, firstcol, lastcol);
            sheet.AddMergedRegion(region1);
            var cell = sheet.GetRow(firstrow).GetCell(firstcol) ?? sheet.GetRow(firstrow).CreateCell(firstcol);
            ICellStyle style = workbook.CreateCellStyle();
            if(alignmentStyle.HasValue)
            {
                style.Alignment = (HorizontalAlignment)alignmentStyle;
            }
            else
            {
                style.Alignment = HorizontalAlignment.Center;
            }
                
            style.VerticalAlignment = VerticalAlignment.Center;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderTop = BorderStyle.Thin;
            style.WrapText = true;
            style.IsHidden = true;
            style.SetFont(font);
            cell.CellStyle = style;
            //cell.SetCellValue($"{value}");
            SetValue(value, workbook, cell, style);
            RegionUtil.SetBorderBottom(BorderStyle.Thin, region1, sheet);
            RegionUtil.SetBorderRight(BorderStyle.Thin, region1, sheet);
            RegionUtil.SetBorderLeft(BorderStyle.Thin, region1, sheet);
            RegionUtil.SetBorderBottom(BorderStyle.Thin, region1, sheet);
        }

        public static void SetCellRangeValueNoStyle(IWorkbook workbook, ISheet sheet, ICellStyle style, int firstrow, int lastrow, int firstcol, int lastcol, object value)
        {
            var region1 = new CellRangeAddress(firstrow, lastrow, firstcol, lastcol);
            sheet.AddMergedRegion(region1);
            var cell = sheet.GetRow(firstrow).GetCell(firstcol) ?? sheet.GetRow(firstrow).CreateCell(firstcol);
            cell.CellStyle = style;
            //cell.SetCellValue($"{value}");
            SetValue(value, workbook, cell, style);
            RegionUtil.SetBorderBottom(BorderStyle.Thin, region1, sheet);
            RegionUtil.SetBorderRight(BorderStyle.Thin, region1, sheet);
            RegionUtil.SetBorderLeft(BorderStyle.Thin, region1, sheet);
            RegionUtil.SetBorderBottom(BorderStyle.Thin, region1, sheet);
        }

        //设置单元格数据
        public static void SetValue(object value, IWorkbook workbook,ICell cell,ICellStyle style)
        {
            if (value==null)
            {
                value = "";
            }
            Type type = value.GetType();
            //var dataformat = ((XSSFWorkbook)workbook).CreateDataFormat();
            IDataFormat dataformat = workbook.CreateDataFormat();
            switch (type.FullName)
            {
                case "System.Decimal":
                    Double DB = (double)Math.Round(Convert.ToDecimal(value), 2);
                    style.DataFormat = dataformat.GetFormat("0.00");
                    cell.CellStyle = style;
                    cell.SetCellValue(DB.ToString("#0.######"));
                    //cell1.SetCellValue(str);
                    break;
                case "System.Double":
                    Double DBD = (double)Math.Round(Convert.ToDecimal(value), 2);
                    style.DataFormat = dataformat.GetFormat("0.00");
                    cell.CellStyle = style;
                    cell.SetCellValue(DBD.ToString("#0.######"));
                    break;
                case "System.DateTime":
                    var str = Convert.ToDateTime(value).ToString("yyyy-MM-dd");// HH:mm:ss");
                    cell.CellStyle = style;
                    cell.SetCellValue(str);
                    break;
                case "System.Int32":
                    cell.CellStyle = style;
                    cell.SetCellValue(value.ToString());
                    break;
                default:
                    cell.CellStyle = style;
                    cell.SetCellValue($"{value}");
                    break;
            }
        }

        //双表头统一宽度
        public static void SetHeader(IWorkbook workbook,ISheet sheet, IFont font, string[] heads1, string[] heads2,double width= 20 * 256, short height= 17 * 20)
        {
            sheet.DefaultRowHeight = height;
            for (int x = 0; x < heads1.Length; x++)
            {
                
                sheet.SetColumnWidth(x, width);
            }
            IRow headerRow1 = sheet.CreateRow(0);
            IRow headerRow2 = sheet.CreateRow(1);
            int index = 0;
            while (index < heads1.Length)
            {
                if (index == heads1.Length - 1)
                {
                    int hh = 0;
                }
                bool iscombin = false;
                int firstRow = 0;

                int lastRow = 0;
                int firstCol = index;
                int combin = 0;
                int lastCol = index;
                string head = heads1[index];
                combin = firstCol + 1;
                if (heads1[index] == heads2[index])
                {
                    iscombin = true;
                    lastRow = 1;
                }
                if (firstCol != heads1.Length - 1)
                {
                    while (heads1[combin] == head)
                    {
                        iscombin = true;
                        lastCol = combin;
                        if (combin == heads1.Length - 1) break;
                        combin++;
                    }
                }
                ICellStyle style = workbook.CreateCellStyle();
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Thin;
                //var font = workbook.CreateFont();
                //font.FontHeightInPoints = 12;
                //font.FontName = "微软雅黑";
                //font.IsBold = true;
                style.SetFont(font);
                if (iscombin)
                {
                    
                    SetCellRangeValue(workbook, sheet, font, firstRow, lastRow, firstCol, lastCol, heads1[firstCol]);
                    
                } else
                {
                    SetCellValue(workbook, sheet, font, firstRow, firstCol, heads1[firstCol]);
                }
                if (lastRow == 0)
                {
                    for (int j = firstCol; j <= lastCol; j++)
                    {
                        SetCellValue(workbook, sheet, font, 1, j, heads2[j]);
                    }
                }
                index = lastCol + 1;
            }
        }

        //动态表头双表头统一宽度
        public static void SetListHeader(IWorkbook workbook, ISheet sheet, IFont font, List<string> heads1, List<string> heads2, double width = 20 * 256, short height = 17 * 20)
        {
            for (int x = 0; x < heads1.Count; x++)
            {

                sheet.SetColumnWidth(x, width);
            }
            IRow headerRow1 = sheet.CreateRow(0);
            IRow headerRow2 = sheet.CreateRow(1);
            headerRow1.Height = height;
            headerRow2.Height = height;
            int index = 0;
            while (index < heads1.Count)
            {
                if (index == heads1.Count - 1)
                {
                    int hh = 0;
                }
                bool iscombin = false;
                int firstRow = 0;

                int lastRow = 0;
                int firstCol = index;
                int combin = 0;
                int lastCol = index;
                string head = heads1[index];
                combin = firstCol + 1;
                if (heads1[index] == heads2[index])
                {
                    iscombin = true;
                    lastRow = 1;
                }
                if (firstCol != heads1.Count - 1)
                {
                    while (heads1[combin] == head)
                    {
                        iscombin = true;
                        lastCol = combin;
                        if (combin == heads1.Count - 1) break;
                        combin++;
                    }
                }
                ICellStyle style = workbook.CreateCellStyle();
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Thin;
                //var font = workbook.CreateFont();
                //font.FontHeightInPoints = 12;
                //font.FontName = "微软雅黑";
                //font.IsBold = true;
                style.SetFont(font);
                if (iscombin)
                {

                    SetCellRangeValue(workbook, sheet, font, firstRow, lastRow, firstCol, lastCol, heads1[firstCol]);

                }
                else
                {
                    SetCellValue(workbook, sheet, font, firstRow, firstCol, heads1[firstCol]);
                }
                if (lastRow == 0)
                {
                    for (int j = firstCol; j <= lastCol; j++)
                    {
                        SetCellValue(workbook, sheet, font, 1, j, heads2[j]);
                    }
                }
                index = lastCol + 1;
            }
        }

        //双表头不设置默认宽度，可以自定义宽度
        public static void SetHeaderNoSetWidth(IWorkbook workbook, ISheet sheet, IFont font, string[] heads1, string[] heads2, double width = 20 * 256, short height = 17 * 20)
        {
           
            IRow headerRow1 = sheet.CreateRow(0);
            IRow headerRow2 = sheet.CreateRow(1);
            int index = 0;
            while (index < heads1.Length)
            {
                if (index == heads1.Length - 1)
                {
                    int hh = 0;
                }
                bool iscombin = false;
                int firstRow = 0;

                int lastRow = 0;
                int firstCol = index;
                int combin = 0;
                int lastCol = index;
                string head = heads1[index];
                combin = firstCol + 1;
                if (heads1[index] == heads2[index])
                {
                    iscombin = true;
                    lastRow = 1;
                }
                if (firstCol != heads1.Length - 1)
                {
                    while (heads1[combin] == head)
                    {
                        iscombin = true;
                        lastCol = combin;
                        if (combin == heads1.Length - 1) break;
                        combin++;
                    }
                }
                ICellStyle style = workbook.CreateCellStyle();
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Thin;
                //var font = workbook.CreateFont();
                //font.FontHeightInPoints = 12;
                //font.FontName = "微软雅黑";
                //font.IsBold = true;
                style.SetFont(font);
                if (iscombin)
                {

                    SetCellRangeValue(workbook, sheet, font, firstRow, lastRow, firstCol, lastCol, heads1[firstCol]);

                }
                else
                {
                    SetCellValue(workbook, sheet, font, firstRow, firstCol, heads1[firstCol]);
                }
                if (lastRow == 0)
                {
                    for (int j = firstCol; j <= lastCol; j++)
                    {
                        SetCellValue(workbook, sheet, font, 1, j, heads2[j]);
                    }
                }
                index = lastCol + 1;
            }
        }


        /// <summary>
        /// 读取Excel多Sheet数据
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="sheetName">Sheet名</param>
        /// <returns></returns>
        public static DataSet ReadExcelToDataSet(FileStream fs, string sheetName = null)
        {
            IWorkbook workbook = WorkbookFactory.Create(fs);
            //获取sheet信息
            ISheet sheet = null;
            DataSet ds = new DataSet();
            if (!string.IsNullOrEmpty(sheetName))
            {
                sheet = workbook.GetSheet(sheetName);
                if (sheet == null)
                {
                    //logger.LogError($"{filePath}未找到sheet:{sheetName}");
                    return null;
                }
                DataTable dt = ReadExcelFunc(workbook, sheet);
                ds.Tables.Add(dt);
            }
            else
            {
                //遍历获取所有数据
                int sheetCount = workbook.NumberOfSheets;
                for (int i = 0; i < sheetCount; i++)
                {
                    sheet = workbook.GetSheetAt(i);
                    if (sheet != null)
                    {
                        DataTable dt = ReadExcelFunc(workbook, sheet);
                        if (dt != null)
                            ds.Tables.Add(dt);
                    }
                }
            }
            return ds;
        }

        /// <summary>
        /// 读取Excel信息
        /// </summary>
        /// <param name="workbook">工作区</param>
        /// <param name="sheet">sheet</param>
        /// /// <param name="startrow">从第几行开始获取数据,如果为0，则根据emptyCount的数量判断搜索第一行</param>
        /// <returns></returns>
        private static DataTable ReadExcelFunc(IWorkbook workbook, ISheet sheet, int startrow = 0)
        {
            DataTable dt = new DataTable();
            //获取列信息
            IRow cells = sheet.GetRow(sheet.FirstRowNum);
            //空数据化返回
            if (cells == null) return null;
            int cellsCount = cells.PhysicalNumberOfCells;
            //空列返回
            if (cellsCount == 0) return null;
            int emptyCount = 0;
            int cellIndex = sheet.FirstRowNum;
            List<string> listColumns = new List<string>();
            bool isFindColumn = false ;
            while (!isFindColumn)
            {
                emptyCount = 0;
                listColumns.Clear();
                for (int i = 0; i < cellsCount; i++)
                {
                    if (string.IsNullOrEmpty(cells.GetCell(i).ToString()))
                    {
                        emptyCount++;
                    }
                    listColumns.Add(cells.GetCell(i).ToString());
                }
                //这里根据逻辑需要，空列超过多少判断
                if (startrow>0 || emptyCount == 0)
                {
                    isFindColumn = true;
                }
                cellIndex++;
                cells = sheet.GetRow(cellIndex);
                if(cells==null)
                {
                    isFindColumn = true;
                }
            }
            int index = 0;
            foreach (string columnName in listColumns)
            {
                
                string name = columnName;
                if (dt.Columns.Contains(columnName))
                {
                    index++;
                    if (startrow>0)
                    {
                        name += index;
                    }
                    else
                    {
                        continue;
                    }
                    
                }
                dt.Columns.Add(name, typeof(string));
            }
            //开始获取数据
            int rowsCount = sheet.PhysicalNumberOfRows;
            var rowIndex = startrow==0? cellIndex: startrow;
            DataRow dr = null;
            //空数据化返回
            if (rowsCount <= 1) { return null; }
            for (int i = rowIndex; i < rowsCount; i++)
            {
                cells = sheet.GetRow(i);
                dr = dt.NewRow();
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    var cell = cells.GetCell(j);
                    if (cell != null)
                    {
                        var datetype = cells.GetCell(j).CellType;
                        //这里可以判断数据类型
                        switch (cells.GetCell(j).CellType)
                        {
                            case CellType.String:
                                dr[j] = cells.GetCell(j).StringCellValue;
                                break;
                            case CellType.Numeric:
                                if (cells.GetCell(j).ToString().Contains('-'))
                                {
                                    dr[j] = cells.GetCell(j).DateCellValue.ToString();
                                }
                                else
                                {
                                    dr[j] = cells.GetCell(j).NumericCellValue.ToString();
                                }
                               
                                
                                //dr[j] = cells.GetCell(j).DateCellValue.ToString();
                                break;
                            case CellType.Unknown:
                                dr[j] = cells.GetCell(j).StringCellValue;
                                break;
                        }
                    }
                    else
                    {
                        dr[j] = "";
                    }

                }
                dt.Rows.Add(dr);
            }
            return dt;
        }

        
    }
}
