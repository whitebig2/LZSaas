using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NPOI.SS.Util;
using SqlSugar;
using NPOI.SS.Formula.Functions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using LZ.Model.IExcelTemplate.Banks;
using LZ.Model.IExcelTemplate;
using NPOI.HSSF.UserModel;
using System.Collections;

namespace LZ.Common
{
    public class NpoiExcelHelper
    {

        public static string YouZhengTempUri= "https://oss.cn-labor.cn//files/20251020/b01953efbe824d038fdff778cbe7edd3.xlsx";
        //public static string MinShengTempUri= "https://oss.cn-labor.cn//files/20251020/c2dd428d73ff4cb68d2a8385cc2747e6.xls";
        public static string MinShengTempUri= "https://oss.cn-labor.cn//files/20251022/66299a80095f47f39feac8b53cca1bb2.xls";
        public static string JianSheTempUri= "https://oss.cn-labor.cn//files/20251020/9b233d435c4342aca5dbd3490232a51b.xls";
        public static string ZhaoShangTempUri= "https://oss.cn-labor.cn//files/20251020/5d8fdd914c81463f94571b75f8d0bc62.xlsx";

        private record ExcelColumeInfo(string Name, string PropName, string Format);
        private static List<ExcelColumeInfo> GetDataTypes<T>()
        {
            List<ExcelColumeInfo> dataTypes = new List<ExcelColumeInfo>();
            var props = typeof(T).GetProperties();
            string name = string.Empty;
            string propName = string.Empty;
            string format = string.Empty;
            foreach (var propItem in props)
            {
                name = string.Empty;
                format = string.Empty;
                //属性名称
                propName = propItem.Name;
                //标题名称
                if (propItem.CustomAttributes.Any(f => f.AttributeType.Name == nameof(DisplayNameAttribute)))
                {
                    DisplayNameAttribute displayNameAttribute = propItem.GetCustomAttributes(typeof(DisplayNameAttribute), false).FirstOrDefault() as DisplayNameAttribute;
                    name = displayNameAttribute.DisplayName;
                }
                // 数据格式
                if (propItem.CustomAttributes.Any(f => f.AttributeType.Name == nameof(DisplayFormatAttribute)))
                {
                    DisplayFormatAttribute displayNameAttribute = propItem.GetCustomAttributes(typeof(DisplayFormatAttribute), false).FirstOrDefault() as DisplayFormatAttribute;
                    format = displayNameAttribute.DataFormatString;
                }
                Console.WriteLine($"属性名称：{propName}   标题名称：{name}     数据格式：{format}");
                dataTypes.Add(new ExcelColumeInfo(name, propName, format));
            }
            return dataTypes;
        }
        private static object GetCellValue(ICell cell)
        {


            CellType cellType = cell.CellType;
            if (cell.CellType == CellType.Formula)
            {
                cellType = cell.CachedFormulaResultType;
            }


            switch (cellType)
            {
                case CellType.Blank:
                    return null; // 或 string.Empty
                case CellType.Boolean:
                    return cell.BooleanCellValue;
                case CellType.Error:
                    return cell.ErrorCellValue.ToString(); // 或使用 FormulaError 枚举
                case CellType.Formula:

                    throw new Exception("导入文件存在公式，导入失败");
                    // 评估公式（可能需要先设置公式评估器）
                    // IFormulaEvaluator evaluator = workbook.GetCreationHelper().CreateFormulaEvaluator();
                    // evaluator.EvaluateFormulaCell(cell);
                    // 然后可以返回 cell.NumericCellValue、cell.StringCellValue 等，取决于公式的结果类型
                    return "Formula"; // 这里仅返回 "Formula" 作为示例
                case CellType.Numeric:
                    if (DateUtil.IsCellDateFormatted(cell))
                    {
                        return cell.DateCellValue;
                    }
                    else
                    {
                        return cell.NumericCellValue;
                    }
                case CellType.String:
                    return cell.StringCellValue;
                case CellType.Unknown:
                    return "Unknown";
                // 如果存在 FormulaString 类型（在某些版本的 NPOI 中可能不存在）:
                // case CellType.FormulaString:
                //     return cell.StringCellValue; // 与 String 类型类似，但强调它是公式的文本表示
                default:
                    throw new InvalidOperationException("Unexpected cell type.");
            }
        }

        private static object GetCellValue(ICell cell,IFormulaEvaluator evaluator)
        {


            CellType cellType = cell.CellType;
            if (cell.CellType == CellType.Formula)
            {
                cellType = cell.CachedFormulaResultType;
            }


            switch (cellType)
            {
                case CellType.Blank:
                    return null; // 或 string.Empty
                case CellType.Boolean:
                    return cell.BooleanCellValue;
                case CellType.Error:
                    return cell.ErrorCellValue.ToString(); // 或使用 FormulaError 枚举
                case CellType.Formula:


                    try
                    {
                        var evaluatedValue = evaluator.Evaluate(cell);

                        // 根据结果类型处理
                        switch (evaluatedValue.CellType)
                        {
                            case CellType.Numeric:
                                return evaluatedValue.NumberValue;
                            case CellType.String:
                                return evaluatedValue.StringValue;
                            case CellType.Boolean:
                                return evaluatedValue.BooleanValue;
                            default:
                                return evaluatedValue.ToString();
                        }
                    }
                    catch (Exception)
                    {
                        throw new Exception($"根据公式获取值失败，导入失败");
                    }
                    // 评估公式（可能需要先设置公式评估器）
                    // IFormulaEvaluator evaluator = workbook.GetCreationHelper().CreateFormulaEvaluator();
                    // evaluator.EvaluateFormulaCell(cell);
                    // 然后可以返回 cell.NumericCellValue、cell.StringCellValue 等，取决于公式的结果类型
                    return ""; // 这里仅返回 "Formula" 作为示例
                case CellType.Numeric:
                    if (DateUtil.IsCellDateFormatted(cell))
                    {
                        return cell.DateCellValue;
                    }
                    else
                    {
                        return cell.NumericCellValue;
                    }
                case CellType.String:
                    return cell.StringCellValue;
                case CellType.Unknown:
                    return "Unknown";
                // 如果存在 FormulaString 类型（在某些版本的 NPOI 中可能不存在）:
                // case CellType.FormulaString:
                //     return cell.StringCellValue; // 与 String 类型类似，但强调它是公式的文本表示
                default:
                    throw new InvalidOperationException("Unexpected cell type.");
            }
        }
        private static object GetCellValue(ICell cell, IFormulaEvaluator evaluator, ExcelColumeInfo info)
        {


            CellType cellType = cell.CellType;
            if (cell.CellType == CellType.Formula)
            {
                cellType = cell.CachedFormulaResultType;
            }


            switch (cellType)
            {
                case CellType.Blank:
                    return null; // 或 string.Empty
                case CellType.Boolean:
                    return cell.BooleanCellValue;
                case CellType.Error:
                    return cell.ErrorCellValue.ToString(); // 或使用 FormulaError 枚举
                case CellType.Formula:


                    try
                    {
                        var evaluatedValue = evaluator.Evaluate(cell);

                        // 根据结果类型处理
                        switch (evaluatedValue.CellType)
                        {
                            case CellType.Numeric:
                                return evaluatedValue.NumberValue;
                            case CellType.String:
                                return evaluatedValue.StringValue;
                            case CellType.Boolean:
                                return evaluatedValue.BooleanValue;
                            default:
                                return evaluatedValue.ToString();
                        }
                    }
                    catch (Exception)
                    {
                        throw new Exception($"根据公式获取值失败，导入失败");
                    }
                    // 评估公式（可能需要先设置公式评估器）
                    // IFormulaEvaluator evaluator = workbook.GetCreationHelper().CreateFormulaEvaluator();
                    // evaluator.EvaluateFormulaCell(cell);
                    // 然后可以返回 cell.NumericCellValue、cell.StringCellValue 等，取决于公式的结果类型
                    return ""; // 这里仅返回 "Formula" 作为示例
                case CellType.Numeric:
                    if (DateUtil.IsCellDateFormatted(cell))
                    {
                        return cell.DateCellValue;
                    }
                    else
                    {
                        if (info.Name.EndsWith("所属月") || info.Name.EndsWith("日期") || info.Name.EndsWith("时间")&& cell.DateOnlyCellValue!=null)
                        {
                            return cell.DateOnlyCellValue.Value;
                        }
                        else { 

                            return cell.NumericCellValue;
                        
                        }
                       
                    }
                case CellType.String:
                    return cell.StringCellValue;
                case CellType.Unknown:
                    return "Unknown";
                // 如果存在 FormulaString 类型（在某些版本的 NPOI 中可能不存在）:
                // case CellType.FormulaString:
                //     return cell.StringCellValue; // 与 String 类型类似，但强调它是公式的文本表示
                default:
                    throw new InvalidOperationException("Unexpected cell type.");
            }
        }
        private static void SetPropValue(object entity, PropertyInfo prop, object value)
        {

            Console.WriteLine(prop.Name);
            Type propType = prop.PropertyType;
            if (propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // 如果是可空类型，获取其基础类型（实际类型）
                propType = Nullable.GetUnderlyingType(propType);
            }

            if (propType == typeof(bool))
            {
                if (value == null)
                {
                    prop.SetValue(entity, false);
                }
                else
                {
                    if (string.Equals("是", value.ToString()))
                    {
                        prop.SetValue(entity, true);
                    }
                    else
                    {
                        prop.SetValue(entity, false);
                    }
                }
            }
            else
            {
                if (value!=null)
                {

                    if (propType == typeof(int))
                    {
                        prop.SetValue(entity, string.IsNullOrWhiteSpace(value.ToString())? 0:Convert.ToInt32(value));

                    }else
                    if (propType == typeof(string))
                    {
                        prop.SetValue(entity, value.ToString().Trim());

                    }
                    else
                    {
                        prop.SetValue(entity, Convert.ChangeType(value, propType));

                    }
                }
            }

        }
        /// <summary>
        ///  获取文件表头信息
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        private static Dictionary<int, string> GetExcelFileHeader(IRow[] rows)
        {
            var rowLength = rows.Length;
            Dictionary<int, string> headerTitleDic = new Dictionary<int, string>();
            IRow row = null;
            for (int i = 0; i < rowLength; i++)
            {
                row = rows[i];
                foreach (var cell in row.Cells)
                {
                    if (!headerTitleDic.ContainsKey(cell.ColumnIndex))
                    {
                        headerTitleDic[cell.ColumnIndex] = string.Empty;
                    }

                    if (i <= 0)
                    {
                        if (string.IsNullOrWhiteSpace(cell.StringCellValue))
                        {
                            headerTitleDic[cell.ColumnIndex] = headerTitleDic[cell.ColumnIndex - 1];
                        }
                        else
                        {
                            headerTitleDic[cell.ColumnIndex] = cell.StringCellValue;
                        }
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(cell.StringCellValue))
                        {
                            if (headerTitleDic.ContainsKey(cell.ColumnIndex - 1))
                            {
                                if (headerTitleDic[cell.ColumnIndex - 1].StartsWith(headerTitleDic[cell.ColumnIndex]))
                                {
                                    headerTitleDic[cell.ColumnIndex] += "|" + headerTitleDic[cell.ColumnIndex - 1].Split('|').LastOrDefault();
                                }
                                else
                                {
                                    headerTitleDic[cell.ColumnIndex] += "|" + headerTitleDic[cell.ColumnIndex].Split('|').LastOrDefault();
                                }
                            }
                            else
                            {
                                headerTitleDic[cell.ColumnIndex] += "|" + headerTitleDic[cell.ColumnIndex].Split('|').LastOrDefault();
                            }
                        }
                        else
                        {
                            headerTitleDic[cell.ColumnIndex] += "|" + cell.StringCellValue;
                        }
                    }
                }
            }
            foreach (var cell in headerTitleDic.Keys)
            {
                Console.WriteLine($"{cell}--{headerTitleDic[cell]}");
            }
            headerTitleDic.ToDictionary(f => f.Key, f => string.Join('|', f.Value.Split('|').Distinct()));
            return headerTitleDic;
        }

        /// <summary>
        /// 列索引匹配信息
        /// </summary>
        /// <param name="indexTitleDic"></param>
        /// <param name="dataInfos"></param>
        private static Dictionary<int, ExcelColumeInfo> HeaderTitleMathProp(Dictionary<int, string> indexTitleDic, List<ExcelColumeInfo> dataInfos)
        {
            Dictionary<int, ExcelColumeInfo> dss = new Dictionary<int, ExcelColumeInfo>();
            foreach (var kvItem in indexTitleDic)
            {
                if (dataInfos.Any(f => f.Name == string.Join('|', kvItem.Value.Split('|').Distinct())))
                {
                    dss.Add(kvItem.Key, dataInfos.First(f => f.Name ==string.Join('|', kvItem.Value.Split('|').Distinct())));
                }
            }

            return dss;
        }

        public static List<T> ReadExcelFile<T>(string filePath, string sheetName)
        {
            var dataInfoList = GetDataTypes<T>();
            // 获取表头 深度
            var headerRowDeep = dataInfoList.Max(f => f.Name.Split('|').Length);


            using FileStream fs = new FileStream(filePath, FileMode.Open);

            IWorkbook workbook = new XSSFWorkbook(fs);
            var sheet = workbook.GetSheet(sheetName);
            List<IRow> headerRows = new List<IRow>();
            for (int i = 0; i < headerRowDeep; i++)
            {
                headerRows.Add(sheet.GetRow(i));
            }
            // 获取表头信息
            var columeIndexHeaderTitleDic = GetExcelFileHeader(headerRows.ToArray());
            // 列索引对应属性信息
            var kv_info = HeaderTitleMathProp(columeIndexHeaderTitleDic, dataInfoList);

            List<T> values = new List<T>();

            var props = typeof(T).GetProperties();


            // 获取excel 表头
            var firstRow = sheet.GetRow(0);
            var colLength = firstRow.Cells.Count;
            Dictionary<int, string> dss = new Dictionary<int, string>();

            T obj;

            ExcelColumeInfo info = null;
            PropertyInfo prop = null;
            IRow row;
            ICell cell;
            for (int i = 1; i <= sheet.LastRowNum; i++)
            {
                row = sheet.GetRow(i);
                obj = Activator.CreateInstance<T>();
                foreach (var kvItem in kv_info)
                {
                    cell = row.Cells.FirstOrDefault(f => f.ColumnIndex == kvItem.Key);
                    if (cell == null)
                    {
                        continue;
                    }
                    info = kvItem.Value;
                    prop = props.FirstOrDefault(f => f.Name == info.PropName);
                    var cellValue = GetCellValue(cell);
                    SetPropValue(obj, prop, cellValue);
                }
                values.Add(obj);
            }
            return values;
        }

        public static List<T> ReadExcelFile<T>(Stream stream, string sheetName)
        {
            stream.Position = 0;
            var dataInfoList = GetDataTypes<T>();
            // 获取表头 深度
            var headerRowDeep = dataInfoList.Max(f => f.Name.Split('|').Length);
            IFormFile file;
            IWorkbook workbook = new XSSFWorkbook(stream,true);


            IFormulaEvaluator evaluator = workbook.GetCreationHelper().CreateFormulaEvaluator();

            var sheet = string.IsNullOrWhiteSpace(sheetName)? workbook.GetSheetAt(0): workbook.GetSheet(sheetName);

            sheet.ForceFormulaRecalculation = true;
            List<IRow> headerRows = new List<IRow>();
            for (int i = 0; i < headerRowDeep; i++)
            {
                headerRows.Add(sheet.GetRow(i));
            }
            // 获取表头信息
            var columeIndexHeaderTitleDic = GetExcelFileHeader(headerRows.ToArray());
            // 列索引对应属性信息
            var kv_info = HeaderTitleMathProp(columeIndexHeaderTitleDic, dataInfoList);

            List<T> values = new List<T>();

            var props = typeof(T).GetProperties();


            // 获取excel 表头
            var firstRow = sheet.GetRow(0);
            var colLength = firstRow.Cells.Count;
            Dictionary<int, string> dss = new Dictionary<int, string>();

            T obj;

            ExcelColumeInfo info = null;
            PropertyInfo prop = null;
            IRow row;
            ICell cell;
            for (int i = headerRowDeep; i <= sheet.LastRowNum; i++)
            {
                row = sheet.GetRow(i);
                if (row == null) { continue; }
                var cells = row.Cells;
                if(cells==null || cells.Count == 0)
                {
                    continue;
                }
                var firstCell = cells.FirstOrDefault();
                if (firstCell == null || firstCell.CellType == CellType.Blank)
                {
                    continue;
                }
                obj = Activator.CreateInstance<T>();
                foreach (var kvItem in kv_info)
                {
                    cell = cells.FirstOrDefault(f => f.ColumnIndex == kvItem.Key);
                    if (cell == null)
                    {
                        continue;
                    }
                    info = kvItem.Value;
                    prop = props.FirstOrDefault(f => f.Name == info.PropName);
                    var cellValue = GetCellValue(cell,evaluator,info);
                 
                        Console.WriteLine($"{info.PropName}--{cellValue}");
                   
                    SetPropValue(obj, prop, cellValue);
                }
                values.Add(obj);
            }
            return values;
        }
        public static void WriteExcelTemplateFile<T>(Stream stream, string sheetName)
        {
            var columeDataInfo = GetDataTypes<T>();
            // 获取表头 深度
            var headerRowDeep = columeDataInfo.Max(f => f.Name.Split('|').Length);
            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet(sheetName);
            sheet.DefaultColumnWidth = 12;
            List<IRow> rows = new List<IRow>();
            for (int i = 0; i < headerRowDeep; i++)
            {
                rows.Add(sheet.CreateRow(i));
            }
            IRow row;
            ICell cell;
            foreach (var item in columeDataInfo)
            {
                var titleArr = item.Name.Split('|');
                for (int i = 0; i < headerRowDeep; i++)
                {
                    row = rows[i];
                    cell = row.CreateCell(columeDataInfo.IndexOf(item));
                    if (titleArr.Length > i)
                    {
                        cell.SetCellValue(titleArr[i]);

                    }
                    else
                    {
                        cell.SetCellValue(titleArr.Last());
                    }
                }
            }
            workbook.Write(stream);
        }

        public record ss<T> (List<T> dataList, string sheetName);

        public static byte[] WriteExcel<T>( List<T>dataList, string sheetName)
        {
            using (var ms = new MemoryStream())
            {
                var columeDataInfo = GetDataTypes<T>();
                // 获取表头 深度
                var headerRowDeep = columeDataInfo.Max(f => f.Name.Split('|').Length);
                var workbook = new XSSFWorkbook();
                var sheet = workbook.CreateSheet(sheetName);
                ICellStyle style = workbook.CreateCellStyle();
                sheet.DefaultColumnWidth = 12;
                List<IRow> rows = new List<IRow>();
                for (int i = 0; i < headerRowDeep; i++)
                {
                    rows.Add(sheet.CreateRow(i));
                }
                IRow row;
                ICell cell;
                foreach (var item in columeDataInfo)
                {
                    var titleArr = item.Name.Split('|');
                    for (int i = 0; i < headerRowDeep; i++)
                    {
                        row = rows[i];
                        cell = row.CreateCell(columeDataInfo.IndexOf(item));
                        if (titleArr.Length > i)
                        {
                            cell.SetCellValue(titleArr[i]);
                        }
                        else
                        {
                            cell.SetCellValue(titleArr.Last());
                        }
                    }
                }
                int rowNum = headerRowDeep;
                foreach (var dataItem in dataList)
                {
                    row = sheet.CreateRow(rowNum);
                    foreach (var item in columeDataInfo)
                    {
                        var value = GetProperty(dataItem, item);
                        cell = row.CreateCell(columeDataInfo.IndexOf(item));
                        SetValue(value, workbook, cell, style);
                        //cell.SetCellValue(value);
                    }
                    rowNum++;
                }
                workbook.Write(ms);
                return ms.ToArray();
            }
        }

        public static void SetValue(object value, IWorkbook workbook, ICell cell, ICellStyle style)
        {
            if (value == null)
            {
                value = "";
            }
            else
            {

            }
            Type type = value.GetType();
            var dataformat = ((XSSFWorkbook)workbook).CreateDataFormat();
            switch (type.FullName)
            {
                case "System.Decimal":
                    Double DB = (double)Math.Round(Convert.ToDecimal(value), 2);
                    style.DataFormat = dataformat.GetFormat("0.00");
                    cell.CellStyle = style;
                    cell.SetCellValue(DB);
                    //cell1.SetCellValue(str);
                    break;
                case "System.Double":
                    Double DBD = (double)Math.Round(Convert.ToDecimal(value), 2);
                    style.DataFormat = dataformat.GetFormat("0.00");
                    cell.CellStyle = style;
                    cell.SetCellValue(DBD);
                    break;
                case "System.DateTime":
                    var str = Convert.ToDateTime(value).ToString("yyyy-MM-dd");// HH:mm:ss");
                    cell.CellStyle = style;
                    cell.SetCellValue(str);
                    break;
                case "System.Int32":
                    int DBInt = (int)Math.Round(Convert.ToDecimal(value), 2);
                    style.DataFormat = dataformat.GetFormat("0");
                    cell.CellStyle = style;
                    cell.SetCellValue(DBInt);
                    break;
                default:
                    cell.CellStyle = style;
                    cell.SetCellValue($"{value}");
                    break;
            }
        }

        public static byte[] WriteExcel<T1, T2>(List<T1> data1, string sheetName1, List<T2> data2, string sheetName2)
        {
            using (var ms = new MemoryStream())
            {
                var workbook = new XSSFWorkbook();
                WriteSheet<T1>(workbook, data1, sheetName1);
                WriteSheet<T2>(workbook, data2, sheetName2);
                workbook.Write(ms);
                return ms.ToArray();
            }
        }
        public static byte[] WriteExcel<T1,T2,T3>(List<T1> data1, string sheetName1, List<T2> data2, string sheetName2, List<T3> data3, string sheetName3)
        {
            using (var ms = new MemoryStream())
            {
                var workbook = new XSSFWorkbook();
                WriteSheet<T1>(workbook, data1, sheetName1);
                WriteSheet<T2>(workbook, data2, sheetName2);
                WriteSheet<T3>(workbook, data3, sheetName3);
                workbook.Write(ms);
                return ms.ToArray();
            }
        }

        public static byte[] WriteExcel<T1, T2, T3,T4>(List<T1> data1, string sheetName1, List<T2> data2, string sheetName2, List<T3> data3, string sheetName3, List<T4> data4, string sheetName4)
        {
            using (var ms = new MemoryStream())
            {
                var workbook = new XSSFWorkbook();
                WriteSheet<T1>(workbook, data1, sheetName1);
                WriteSheet<T2>(workbook, data2, sheetName2);
                WriteSheet<T3>(workbook, data3, sheetName3);
                WriteSheet<T4>(workbook, data4, sheetName4);
                workbook.Write(ms);
                return ms.ToArray();
            }
        }

        public static byte[] WriteExcel<T1, T2, T3, T4, T5, T6>(List<T1> data1, string sheetName1, List<T2> data2, string sheetName2, List<T3> data3, string sheetName3, List<T4> data4, string sheetName4, List<T5> data5, string sheetName5, List<T6> data6, string sheetName6)
        {
            using (var ms = new MemoryStream())
            {
                var workbook = new XSSFWorkbook();
                WriteSheet<T1>(workbook, data1, sheetName1);
                WriteSheet<T2>(workbook, data2, sheetName2);
                WriteSheet<T3>(workbook, data3, sheetName3);
                WriteSheet<T4>(workbook, data4, sheetName4);
                WriteSheet<T5>(workbook, data5, sheetName5);
                WriteSheet<T6>(workbook, data6, sheetName6);
                workbook.Write(ms);
                return ms.ToArray();
            }
        }
        public static byte[] WriteExcel<T1, T2, T3, T4, T5, T6,T7>(List<T1> data1, string sheetName1, List<T2> data2, string sheetName2, List<T3> data3, string sheetName3, List<T4> data4, string sheetName4, List<T5> data5, string sheetName5, List<T6> data6, string sheetName6, List<T7> data7, string sheetName7)
        {
            using (var ms = new MemoryStream())
            {
                var workbook = new XSSFWorkbook();
                WriteSheet<T1>(workbook, data1, sheetName1);
                WriteSheet<T2>(workbook, data2, sheetName2);
                WriteSheet<T3>(workbook, data3, sheetName3);
                WriteSheet<T4>(workbook, data4, sheetName4);
                WriteSheet<T5>(workbook, data5, sheetName5);
                WriteSheet<T6>(workbook, data6, sheetName6);
                WriteSheet<T7>(workbook, data7, sheetName7);
                workbook.Write(ms);
                return ms.ToArray();
            }
        }



        private static  void WriteSheet<T>( XSSFWorkbook workbook, List<T> data, string sheetName)
        {
       
                var columeDataInfo = GetDataTypes<T>();
                // 获取表头 深度
                var headerRowDeep = columeDataInfo.Max(f => f.Name.Split('|').Length);
                var sheet = workbook.CreateSheet(sheetName);
                sheet.DefaultColumnWidth = 12;
                ICellStyle style = workbook.CreateCellStyle();
                List<IRow> rows = new List<IRow>();
                for (int i = 0; i < headerRowDeep; i++)
                {
                    rows.Add(sheet.CreateRow(i));
                }
                IRow row;
                ICell cell;
                foreach (var item in columeDataInfo)
                {
                    var titleArr = item.Name.Split('|');
                    for (int i = 0; i < headerRowDeep; i++)
                    {
                        row = rows[i];
                        cell = row.CreateCell(columeDataInfo.IndexOf(item));
                        if (titleArr.Length > i)
                        {
                            cell.SetCellValue(titleArr[i]);
                        }
                        else
                        {
                            cell.SetCellValue(titleArr.Last());
                        }
                    }
                }
                int rowNum = headerRowDeep;
                foreach (var dataItem in data)
                {
                    row = sheet.CreateRow(rowNum);
                    foreach (var item in columeDataInfo)
                    {
                        var value = GetProperty(dataItem, item);
                        cell = row.CreateCell(columeDataInfo.IndexOf(item));
                        SetValue(value, workbook, cell, style);
                }
                    rowNum++;
                }
        }

        private static object GetProperty<T>(T item, ExcelColumeInfo info)
        {

            var prop = typeof(T).GetProperty(info.PropName);
            var propObje = prop.GetValue(item, null);

            object propValue;
            if (propObje == null)
            {
                propValue = string.Empty; ;
            }
            else
            {

                var type = GetPropertyType(prop);
                propValue = propObje;

               
                if (type == typeof(string))
                {
                    propValue = propObje.ToString();
                }
                else if (type == typeof(DateTime))
                {

                    if (!info.Format.IsNullOrEmpty())
                    {

                        propValue = Convert.ToDateTime(propObje).ToString(info.Format);
                    }
                    else
                    {

                        propValue = Convert.ToDateTime(propObje).ToString("yyyy-MM-dd");
                    }
                }
                else if (type == typeof(decimal))
                {

                    propValue = Convert.ToDouble(propObje);
                }
                else if (type == typeof(bool))
                {
                    var isb = Convert.ToBoolean(propObje);
                    if (isb)
                    {
                        propValue = "是";
                    }
                    else
                    {
                        propValue = "否";
                    }
                }
                else
                {
                    propValue = propObje.ToString();
                }

            }
            return propValue;
        }



        private static string GetPropertyValue<T>(T item, ExcelColumeInfo info)
        {

            var prop = typeof(T).GetProperty(info.PropName);
            var propObje = prop.GetValue(item, null);

            string propValue = string.Empty;
            if (propObje == null)
            {
                propValue = string.Empty; ;
            }
            else {

                var type = GetPropertyType(prop);


                if (type == typeof(int))
                {
                     propValue = propObje.ToString();
                }
                else if (type == typeof(string))
                {
                    propValue = propObje.ToString();
                }
                else if (type == typeof(DateTime))
                {

                    if (!info.Format.IsNullOrEmpty())
                    {

                        propValue = Convert.ToDateTime(propObje).ToString(info.Format);
                    }
                    else {

                        propValue = Convert.ToDateTime(propObje).ToString("yyyy-MM-dd");
                    }
                }
                else if (type == typeof(decimal))
                {

                    propValue = Convert.ToDouble(propObje).ToString();
                }
                else if (type == typeof(bool))
                {
                    var isb = Convert.ToBoolean(propObje);
                    if (isb)
                    {
                        propValue = "是";
                    }
                    else
                    {
                        propValue = "否";
                    }
                }
                else
                {
                    propValue = propObje.ToString();
                }

            }
            return propValue;
        }

        public static Type GetPropertyType(PropertyInfo propertyInfo)
        {
            if (propertyInfo.PropertyType == typeof(Nullable<>))
            {
                Console.WriteLine(propertyInfo.PropertyType.GenericTypeArguments[0].Name);
                return propertyInfo.PropertyType.GenericTypeArguments[0];
            }
            else
            {
                //Console.WriteLine(propertyInfo.PropertyType.Name);
                return propertyInfo.PropertyType;
            }
        }
        public static List<Dictionary<int, string>> ReadExcelKV(Stream stream)
        {
            stream.Position = 0;
            // 获取表头 深度
            IWorkbook workbook = new XSSFWorkbook(stream, true);

            IFormulaEvaluator evaluator = workbook.GetCreationHelper().CreateFormulaEvaluator();

            var sheet = workbook.GetSheetAt(0);
            //sheet.ForceFormulaRecalculation = true;
            List<Dictionary<int, string>> data = new List<Dictionary<int, string>>();
            IRow row = null;
            ICell cell = null;
            for (int i = 0; i <= sheet.LastRowNum; i++)
            {
                row = sheet.GetRow(i);
                if (row==null)
                {
                    continue;
                }
                Dictionary<int, string> dic = new Dictionary<int, string>();

               var firstCell= row.Cells.FirstOrDefault();
                if (firstCell==null|| firstCell.CellType==  CellType.Blank)
                {
                    continue;
                }
                for (int j = 0; j < row.LastCellNum; j++)
                {

                    cell = row.GetCell(j);
                    if (cell == null)
                    {
                        dic.Add(j, string.Empty);
                    }
                    else {
                        string cellValueString = string.Empty;
                        var cellValueObj = GetCellValueObj(cell);
                        if (cellValueObj != null)
                        {
                            cellValueString = cellValueObj.ToString();
                        }
                        // 数值类型
                        if (!string.IsNullOrWhiteSpace(cellValueString)&& cellValueString.Contains('.'))
                        {
                            if (decimal.TryParse(cellValueString, out decimal cellVal))
                            {
                                decimal truncatedNumber = Math.Round(cellVal,2, MidpointRounding.AwayFromZero);
                                cellValueString = truncatedNumber.ToString();
                            }
                        }
                        dic.Add(j, cellValueString);
                    }
                }
                data.Add(dic);


            }
            return data;
        }

        public static byte[] WriteExcelKV(List<Dictionary<int, string>> kvList)
        {
            using (var ms=new MemoryStream())
            {
                var workbook = new XSSFWorkbook();
                var sheet = workbook.CreateSheet();

                IRow row;
                ICell cell;
                int index = 0;
                foreach (var itemKV in kvList)
                {
                    row = sheet.CreateRow(index);

                    foreach (var kv in itemKV)
                    {
                        cell = row.CreateCell(kv.Key);
                        cell.SetCellValue(kv.Value);
                    }
                    index++;
                }
                workbook.Write(ms);
                return ms.ToArray();
            }
        }


        public static byte[] WriteExcelKV(List<Dictionary<string, string>> kvList)
        {
            using (var ms = new MemoryStream())
            {
                var workbook = new XSSFWorkbook();
                var sheet = workbook.CreateSheet();

                IRow row;
                ICell cell;
                // 写入表头
                int index = 0;
                int colIndex = 0;
                var headerKv=  kvList.FirstOrDefault();
                if (headerKv != null)
                {
                    colIndex = 0;
                     row = sheet.CreateRow(index);
                    foreach (var kv in headerKv)
                    {
                        cell = row.CreateCell(colIndex);
                        cell.SetCellValue(kv.Key);
                        colIndex++;
                    }
                }

          
                foreach (var itemKV in kvList)
                {
                    index++;
                    colIndex = 0;
                    row = sheet.CreateRow(index);
                    foreach (var kv in itemKV)
                    {
                        cell = row.CreateCell(colIndex);
                        cell.SetCellValue(kv.Value);
                        colIndex++;
                    }
                }
                workbook.Write(ms);
                return ms.ToArray();
            }
        }



        private static object GetCellValueObj(ICell cell)
        {

            CellType cellType = cell.CellType;
            if (cell.CellType == CellType.Formula)
            {
                cellType = cell.CachedFormulaResultType;
            }

            cell.SetCellType(cellType);


            switch (cellType)
            {
                case CellType.Blank:
                    return null; // 或 string.Empty
                case CellType.Boolean:
                    return cell.BooleanCellValue ? "是" : "否";
                case CellType.Error:
                    return cell.ErrorCellValue.ToString(); // 或使用 FormulaError 枚举
                case CellType.Formula:
                    throw new Exception($"根据公式获取值失败，导入失败");
                case CellType.Numeric:
                    if (DateUtil.IsCellDateFormatted(cell))
                    {
                        return cell.DateCellValue;
                    }
                    else
                    {
                        return cell.NumericCellValue;
                    }
                case CellType.String:
                    return cell.StringCellValue;
                case CellType.Unknown:
                    return "Unknown";
                // 如果存在 FormulaString 类型（在某些版本的 NPOI 中可能不存在）:
                // case CellType.FormulaString:
                //     return cell.StringCellValue; // 与 String 类型类似，但强调它是公式的文本表示
                default:
                    throw new InvalidOperationException("Unexpected cell type.");
            }
        }

        #region 银行
        public static byte[] WriteMinShengYinHang(List<MinShengYinHangExcelTemplate> dataList, string tempFileBase64)
        {
            using (var ms = new MemoryStream())
            {
                using var ms1 = new MemoryStream(Convert.FromBase64String(tempFileBase64));
                IWorkbook tempWorkBook = new HSSFWorkbook(ms1);
                var tempSheet = tempWorkBook.GetSheetAt(0);
                var dropDownSheet = tempWorkBook.GetSheetAt(1);

                //SetDropdownFromSheet(tempSheet, dropDownSheet, 3, 3 + dataList.Count, 4, 4);


                IRow row1= tempSheet.GetRow(0);
                row1.GetCell(1).SetCellValue(dataList.Sum(f=>f.TransactionAmount).ToString());

                IRow row2 = tempSheet.GetRow(1);
                row2.GetCell(1).SetCellValue(dataList.Count.ToString());

                int rowIndex = 3;
                IRow row;
                foreach (var dataItem in dataList)
                {
                    row = tempSheet.CreateRow(rowIndex);
                    row.CreateCell(0).SetCellValue(dataItem.BankNo);
                    row.CreateCell(1).SetCellValue(dataItem.BankUserName);
                    row.CreateCell(2).SetCellValue(dataItem.TransactionAmount.ToString());
                    row.CreateCell(3).SetCellValue(dataItem.Summary);
                    row.CreateCell(4).SetCellValue(dataItem.Remark);
                    row.CreateCell(5).SetCellValue(dataItem.BankNameNo);
                    rowIndex++;
                }


        

                tempWorkBook.Write(ms);
                return ms.ToArray();
            }
        }
        public static byte[] WriteJianSheYinHang(List<JianSheYinHangExcelTemplate> dataList, string tempFileBase64)
        {
            using (var ms = new MemoryStream())
            {
                using var ms1 = new MemoryStream(Convert.FromBase64String(tempFileBase64));
                IWorkbook tempWorkBook = new HSSFWorkbook(ms1);


                ICellStyle defaultStyle = tempWorkBook.CreateCellStyle();

                defaultStyle.BorderTop = BorderStyle.Thin;
                defaultStyle.BorderBottom = BorderStyle.Thin;
                defaultStyle.BorderLeft = BorderStyle.Thin;
                defaultStyle.BorderRight = BorderStyle.Thin;

                var tempSheet = tempWorkBook.GetSheet("Sheet0");
                int rowIndex = 1;
                IRow row;
                ICell cell;
                foreach (var dataItem in dataList)
                {
                row = tempSheet.CreateRow(rowIndex);
                    cell = row.CreateCell(0);
                    cell.SetCellValue(rowIndex);
                    cell.CellStyle = defaultStyle;

                    cell = row.CreateCell(1);
                    cell.SetCellValue(dataItem.BankNo);
                    cell.CellStyle = defaultStyle;


                    cell = row.CreateCell(2);
                    cell.SetCellValue(dataItem.BankUserName);
                    cell.CellStyle = defaultStyle;


                    cell = row.CreateCell(3);
                    cell.SetCellValue(dataItem.TransactionAmount.ToString());

                    cell.CellStyle = defaultStyle;

                    cell = row.CreateCell(4);
                    cell.SetCellValue(dataItem.OtherBankTag);

                    cell.CellStyle = defaultStyle;

                    cell = row.CreateCell(5);
                    cell.SetCellValue(dataItem.BankName);

                    cell.CellStyle = defaultStyle;

                    cell = row.CreateCell(6);
                    cell.SetCellValue(dataItem.BankNameNo);

                    cell.CellStyle = defaultStyle;

                    cell = row.CreateCell(7);
                    cell.SetCellValue(dataItem.Summary);

                    cell.CellStyle = defaultStyle;
                    cell = row.CreateCell(8);
                    cell.SetCellValue(dataItem.Remark);
                    cell.CellStyle = defaultStyle;

                    rowIndex++;
                }



                tempWorkBook.Write(ms);
                return ms.ToArray();
            }
        }
        public static byte[] WriteZhaoShangYinHang(List<ZhaoShangYinHangExcelTemplate> dataList,string tempFileBase64)
        {
            using (var ms = new MemoryStream())
            {
                using var ms1 = new MemoryStream(Convert.FromBase64String(tempFileBase64));
                IWorkbook tempWorkBook = new XSSFWorkbook(ms1);
                var tempSheet = tempWorkBook.GetSheetAt(0);
                int rowIndex = 6;
                IRow row;
                ICell cell;
                foreach (var dataItem in dataList)
                {
                    row = tempSheet.CreateRow(rowIndex);
                    row.CreateCell(0).SetCellValue(dataItem.BankNo);
                    row.CreateCell(1).SetCellValue(dataItem.BankUserName);
                    row.CreateCell(2).SetCellValue(dataItem.TransactionAmount.ToString());
                    row.CreateCell(3).SetCellValue(dataItem.BankName);
                    row.CreateCell(4).SetCellValue(dataItem.BankAddreess);
                    row.CreateCell(5).SetCellValue(dataItem.Remark);
                    rowIndex++;
                }

                tempWorkBook.Write(ms);
                return ms.ToArray();
            }
        }

        public static byte[] WriteYouZhengYinHang(List<YouZhengYinHangExcelTemplate> dataList, string tempFileBase64)
        {
            using (var ms = new MemoryStream())
            {
                using var ms1 = new MemoryStream(Convert.FromBase64String(tempFileBase64));
                IWorkbook tempWorkBook = new XSSFWorkbook(ms1);
                var tempSheet = tempWorkBook.GetSheetAt(0);
                int rowIndex = 1;
                IRow row;
                ICell cell;
                foreach (var dataItem in dataList)
                {
                    row = tempSheet.CreateRow(rowIndex);
                    row.CreateCell(0).SetCellValue(dataItem.SignNo);
                    row.CreateCell(1).SetCellValue(dataItem.IdType);
                    row.CreateCell(2).SetCellValue(dataItem.IdCardNo);
                    row.CreateCell(3).SetCellValue(dataItem.IsYouZhengBank);
                    row.CreateCell(4).SetCellValue(dataItem.BankNo);
                    row.CreateCell(5).SetCellValue(dataItem.BankUserName);
                    row.CreateCell(6).SetCellValue(dataItem.TransactionAmount.ToString());
                    row.CreateCell(7).SetCellValue(dataItem.Remark);
                    row.CreateCell(8).SetCellValue(dataItem.BankNameNo);
                    rowIndex++;
                }

                tempWorkBook.Write(ms);
                return ms.ToArray();
            }
        }
        #endregion

        /// <summary>
        /// 基于Sheet2的数据设置下拉列表
        /// </summary>
        /// <param name="targetSheet">需要设置下拉列表的工作表</param>
        /// <param name="sourceSheet">数据源工作表（Sheet2）</param>
        /// <param name="firstRow">下拉列表起始行</param>
        /// <param name="lastRow">下拉列表结束行</param>
        /// <param name="firstCol">下拉列表起始列</param>
        /// <param name="lastCol">下拉列表结束列</param>
        public static void SetDropdownFromSheet(ISheet targetSheet, ISheet sourceSheet,
            int firstRow, int lastRow, int firstCol, int lastCol)
        {
            // 从Sheet2读取数据作为下拉选项
            List<string> dropdownValues = ReadDataFromSourceSheet(sourceSheet);

            // 设置下拉列表的作用区域
            var cellRegions = new CellRangeAddressList(firstRow, lastRow, firstCol, lastCol);

            // 创建下拉列表约束
            DVConstraint constraint = DVConstraint.CreateExplicitListConstraint(dropdownValues.ToArray());

            // 创建数据验证对象
            HSSFDataValidation dataValidation = new HSSFDataValidation(cellRegions, constraint);

            // 设置错误提示信息
            dataValidation.CreateErrorBox("输入不合法", "请从下拉列表中选择有效值。");
            dataValidation.ShowPromptBox = true;

            // 将数据验证添加到目标工作表
            targetSheet.AddValidationData(dataValidation);
        }

        /// <summary>
        /// 从数据源工作表读取数据
        /// </summary>
        /// <param name="sourceSheet">数据源工作表</param>
        /// <returns>下拉选项列表</returns>
        private static List<string> ReadDataFromSourceSheet(ISheet sourceSheet)
        {
            List<string> values = new List<string>();

            // 假设数据在Sheet2的第一列，从第一行开始
            for (int i = 0; i <= sourceSheet.LastRowNum; i++)
            {
                IRow row = sourceSheet.GetRow(i);
                if (row != null && row.GetCell(0) != null)
                {
                    string cellValue = row.GetCell(0).ToString();
                    if (!string.IsNullOrEmpty(cellValue))
                    {
                        values.Add(cellValue);
                    }
                }
            }

            return values;
        }


        public static byte[] WriteDeathInsurance<T>(List<T>dataList)
        {
            using (var ms = new MemoryStream())
            {
                var workbook = new XSSFWorkbook();
                var sheet = workbook.CreateSheet("员工列表");

                var sheet1 = workbook.CreateSheet("投保方案");
                sheet1.CreateRow(0).CreateCell(0).SetCellValue("保额40w-保费300元/人/年");
                var columeDataInfo = GetDataTypes<T>();
           
                sheet.DefaultColumnWidth = 12;
                ICellStyle style = workbook.CreateCellStyle();
                style.Alignment = HorizontalAlignment.Center;   // 水平居中
                style.VerticalAlignment = VerticalAlignment.Center; // 垂直居中
                //                                                    // 2. 设置填充（前景色）
                //style.FillForegroundColor = IndexedColors.Grey25Percent.Index;  // 颜色索引

                //// 3. 启用填充
                //style.FillPattern = FillPattern.SolidForeground;
                //style.BorderTop = BorderStyle.Thin;         // 上边框
                //style.BorderBottom = BorderStyle.Thin;     // 下边框
                //style.BorderLeft = BorderStyle.Thin;        // 左边框
                //style.BorderRight = BorderStyle.Thin;       // 右边框

                //// 设置边框颜色（默认黑色）
                //style.TopBorderColor = IndexedColors.Black.Index;
                //style.BottomBorderColor = IndexedColors.Black.Index;
                //style.LeftBorderColor = IndexedColors.Black.Index;
                //style.RightBorderColor = IndexedColors.Black.Index;


                IRow row;
                ICell cell;
                // 写入表头
                int index = 0;
                int colIndex = 0;

                sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 10));
                sheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 1));
                sheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 1));
                sheet.AddMergedRegion(new CellRangeAddress(3, 3, 0, 1));
                sheet.AddMergedRegion(new CellRangeAddress(4, 4, 0, 1));
                sheet.AddMergedRegion(new CellRangeAddress(5, 5, 0, 1));


                sheet.AddMergedRegion(new CellRangeAddress(1, 1, 2, 5));
                sheet.AddMergedRegion(new CellRangeAddress(2, 2, 2, 10));
                sheet.AddMergedRegion(new CellRangeAddress(3, 3, 2, 5));
                sheet.AddMergedRegion(new CellRangeAddress(4, 4, 2, 5));
                sheet.AddMergedRegion(new CellRangeAddress(5, 5, 2, 5));

                sheet.AddMergedRegion(new CellRangeAddress(1, 1, 7, 10));
                //sheet.AddMergedRegion(new CellRangeAddress(2, 2, 7, 10));
                sheet.AddMergedRegion(new CellRangeAddress(3, 3, 7, 10));
                sheet.AddMergedRegion(new CellRangeAddress(4, 4, 7, 10));
                sheet.AddMergedRegion(new CellRangeAddress(5, 5, 7, 10));

                 row = sheet.CreateRow(0);
                cell = row.CreateCell(0);
                cell.CellStyle=style;
                cell.SetCellValue("保全变更人员名单");
                row = sheet.CreateRow(1);
                cell= row.CreateCell(0);
                cell.CellStyle = style;
                cell.SetCellValue("被保险人：");
                row.CreateCell(2).SetCellValue("上海正联成网络科技有限公司（宁波莱博）");
                cell = row.CreateCell(6);
                cell.CellStyle = style;
                cell.SetCellValue("险种：");


                row.CreateCell(7).SetCellValue(" 医疗健康险");

                row = sheet.CreateRow(2);
                cell = row.CreateCell(0);
                cell.CellStyle = style;
                cell.SetCellValue("保单号：");
                row = sheet.CreateRow(3);
                cell = row.CreateCell(0);
                cell.CellStyle = style;
                cell.SetCellValue("起保日期：");
                row.CreateCell(2).SetCellValue("2026年05月01日零时");
                cell = row.CreateCell(6);
                cell.CellStyle = style;
                cell.SetCellValue("截止日期：");
                row.CreateCell(7).SetCellValue(" 2027年04月30日二十四时");
                row = sheet.CreateRow(4);
                cell = row.CreateCell(0);
                cell.CellStyle = style;
                cell.SetCellValue("申请日期：");
                row.CreateCell(2).SetCellValue(DateTime.Today.ToString("yyyy年MM月dd日"));
                row = sheet.CreateRow(5);

                cell = row.CreateCell(0);
                cell.CellStyle = style;
                cell.SetCellValue("增加人数：");
                cell = row.CreateCell(6);
                cell.CellStyle = style;
                cell.SetCellValue("减少人数：");


                //---------------------------------------
                // 获取表头 深度
                var headerRowDeep = columeDataInfo.Max(f => f.Name.Split('|').Length);
                row = sheet.CreateRow(6);
                foreach (var item in columeDataInfo)
                {
                    var titleArr = item.Name.Split('|');
                    for (int i = 0; i < headerRowDeep; i++)
                    {
                        cell = row.CreateCell(columeDataInfo.IndexOf(item));
                        if (titleArr.Length > i)
                        {
                            cell.CellStyle = style;
                            cell.SetCellValue(titleArr[i]);
                        }
                        else
                        {
                            cell.CellStyle = style;
                            cell.SetCellValue(titleArr.Last());
                        }
                    }
                }
                int rowNum = headerRowDeep+6;
                foreach (var dataItem in dataList)
                {
                    row = sheet.CreateRow(rowNum);
                    foreach (var item in columeDataInfo)
                    {
                        var value = GetProperty(dataItem, item);
                        cell = row.CreateCell(columeDataInfo.IndexOf(item));
                        SetValue(value, workbook, cell, style);
                    }
                    rowNum++;
                }
                workbook.Write(ms);
                return ms.ToArray();
            }
        }



    }
}
