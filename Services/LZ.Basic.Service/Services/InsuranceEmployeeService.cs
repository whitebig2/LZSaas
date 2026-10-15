using Consul;
using LZ.Basic.Service.Interface;
using LZ.Common;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Model;
using LZ.Model.Enums.Insurance;
using LZ.Model.PagedModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using SqlSugar;
using SqlSugar.IOC;
using System.Linq.Expressions;

namespace LZ.Basic.Service.Services
{
    public class InsuranceEmployeeService : ServicesBase<Live_InsuranceEmployee>, IInsuranceEmployeeService, IScopedDependency
    {
        public async Task<List<Model.Live_InsuranceEmployee>> List(Expression<Func<Model.Live_InsuranceEmployee, bool>> exp, Model.PagedModel.BasicPagedModel query, string order = "")
        {
            var count = new RefAsync<int>();
            var sco = base.Context.Queryable<Live_InsuranceEmployee>().Where(exp);
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

        public async Task<List<Model.Live_V_InsuranceEmployee>> ListView(Expression<Func<Model.Live_V_InsuranceEmployee, bool>> exp, Model.PagedModel.BasicPagedModel query, string order = "")
        {
            var count = new RefAsync<int>();
            var sco = base.Context.Queryable<Live_V_InsuranceEmployee>().Where(exp);
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

        public async Task<int> Add(Model.Live_InsuranceEmployee model)
        {
            var db = base.Context;
            return await db.Insertable(model).ExecuteReturnIdentityAsync();
        }

        public async Task<int> Adds(List<Model.Live_InsuranceEmployee> models)
        {
            var db = base.Context;
            return await db.Insertable(models).ExecuteCommandAsync();
        }
        public async Task<bool> Update(Model.Live_InsuranceEmployee model)
        {
            var db = base.Context;
            return await db.Updateable(model).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Update(List<Model.Live_InsuranceEmployee> list)
        {
            var db = base.Context;
            return await db.Updateable(list).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Delete(int id)
        {
            var db = base.Context;
            return await db.Deleteable<Model.Live_InsuranceEmployee>().Where(t => t.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Exist(Expression<Func<Live_InsuranceEmployee, bool>> exp)
        {
            return await base.Context.Queryable<Live_InsuranceEmployee>().AnyAsync(exp);
        }
        public async Task<Model.Live_InsuranceEmployee> Get(int id)
        {
            var db = base.Context;
            return await db.Queryable<Model.Live_InsuranceEmployee>().Where(t => t.ID == id).FirstAsync();
        }
        public async Task<bool> ChangeProcessStatus(int id, int status)
        {
            return await base.Context.Updateable<Live_InsuranceEmployee>().SetColumns(it => it.ProcessStatus == status).Where(it => it.ID == id).ExecuteCommandAsync() > 0;

        }
        public async Task<bool> ExistID(int id)
        {
            return await base.Context.Queryable<Live_InsuranceEmployee>().AnyAsync(it => it.ID == id);
        }

        public async Task<Model.Live_InsuranceEmployee> Get(Expression<Func<Model.Live_InsuranceEmployee, bool>> exp)
        {
            var db = base.Context;
            return await db.Queryable<Model.Live_InsuranceEmployee>().Where(exp).FirstAsync();
        }

        public byte[] Export(List<Model.Live_V_InsuranceEmployee> list)
        {

            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("Sheet1");
            var font = workbook.CreateFont();
            font.FontHeightInPoints = 9;
            font.FontName = "微软雅黑";
            font.IsBold = true;
            double width = 20 * 256;
            short height = 20 * 20;

            sheet.DefaultRowHeight = height;
            //for (int x = 0; x < 6; x++)
            //{
            //    sheet.SetColumnWidth(x, width);
            //}
            //sheet.SetColumnWidth(6, 25 * 256);
            //for (int y = 0; y < 27; y++)
            //{
            //    sheet.CreateRow(y);
            //}
            #region excel 表头
            string[] heads1 = {
                "姓名",
                "身份证号码",
                "社保起始月",
                "公积金起始月",
                "客户单位",
                "性别",
                "民族",
                "联系方式",
                "身份证地址",
                "教育程度",
                "方案类型",
                "预设方案",
                "区域选择（省）",
                "区域选择（市）",
                "增减规则",
                "时间节点",
                "社保供应商",
                "公积金供应商",
                "状态",
                "社保费用产生月",
                "社保费用不产生月",
                "公积金费用产生月",
                "公积金费用不产生月",
                "社保基数","社保基数","社保基数","社保基数","社保基数","社保基数",
                "公积金基数",
                "社保个人比例","社保个人比例","社保个人比例","社保个人比例","社保个人比例","社保个人比例","社保个人比例","社保个人比例",
                "社保企业缴费","社保企业缴费","社保企业缴费","社保企业缴费","社保企业缴费","社保企业缴费","社保企业缴费","社保企业缴费",
                "公积金","公积金",
                "补退金额",
                "备注"
            };

            string[] heads2 = {
                "姓名",
                "身份证号码",
                "社保起始月",
                "公积金起始月",
                "客户单位",
                "性别",
                "民族",
                "联系方式",
                "身份证地址",
                "教育程度",
                "方案类型",
                "预设方案",
                "区域选择（省）",
                "区域选择（市）",
                "增减规则",
                "时间节点",
                "社保供应商",
                "公积金供应商",
                "状态",
                "社保费用产生月",
                "社保费用不产生月",
                "公积金费用产生月",
                "公积金费用不产生月",
                "养老基数","医疗基数","失业基数","生育基数","工伤基数","残保金基数",
                "公积金基数",
                "养老比例%","医疗比例%","失业比例%","生育比例%","工伤比例%","残保金","大病医疗","医疗补充",
                "养老比例","医疗比例%","失业比例%","生育比例%","工伤比例v","残保金%","大病医疗","医疗补充",
                "公积金个人比例%","公积金单位比例%",
                "补退金额",
                "备注"

            };
            ExcelHeler.SetHeader(workbook, sheet, font, heads1, heads2, width, height);
            #endregion
            #region 表格主体
            ICellStyle style = workbook.CreateCellStyle();
            var font1 = workbook.CreateFont();
            font1.FontHeightInPoints = 9;
            font1.FontName = "微软雅黑";
            int index = 1;
            foreach (var x in list)
            {
                int row = 0;
                sheet.CreateRow(index + 1);//index=1,从第三行开始,前2行是表头

                List<object> arr = new List<object>();
                arr.Add(x.Name);
                arr.Add(x.IdcardNo);
                arr.Add(x.StartDateSheBao.HasValue ? x.StartDateSheBao.Value.ToString("yyyyMM") : "/");
                arr.Add(x.StartDateGongJiJin.HasValue ? x.StartDateGongJiJin.Value.ToString("yyyyMM") : "/");
                arr.Add(x.CustomerName);
                arr.Add(x.Sex);
                arr.Add(x.Nation);
                arr.Add(x.Phone);
                arr.Add(x.IdcardAddress);
                arr.Add(x.Education);
                arr.Add((InsuranceSocialTypeEnum)x.ShemeType);
                arr.Add(x.SchemeName);
                arr.Add(x.Province);
                arr.Add(x.City);
                arr.Add((InsuranceSocialRuleEnum)x.YlRule);
                arr.Add(x.TimeNode.HasValue ? x.TimeNode : 1);
                arr.Add(x.SheBaoSuplierName);
                arr.Add(x.GongJiJinSuplierName);
                arr.Add((InsuranceProcessStatusEnumType)x.ProcessStatus);
                arr.Add(x.StartDateSheBao.HasValue ? x.StartDateSheBao.Value.ToString("yyyy-MM-dd") : "");
                arr.Add(x.EndDateSheBao.HasValue ? x.EndDateSheBao.Value.ToString("yyyy-MM-dd") : "");
                arr.Add(x.StartDateGongJiJin.HasValue ? x.StartDateGongJiJin.Value.ToString("yyyy-MM-dd") : "");
                arr.Add(x.EndDateGongJiJin.HasValue ? x.EndDateGongJiJin.Value.ToString("yyyy-MM-dd") : "");
                arr.Add(x.BaseAmountYangLao);
                arr.Add(x.BaseAmountYiLiao);
                arr.Add(x.BaseAmountShiYe);
                arr.Add(x.BaseAmountShengYu);
                arr.Add(x.BaseAmountGongShang);
                arr.Add(x.BaseAmountCanBao);
                arr.Add(x.BaseAmountGongJiJin);
                arr.Add(x.EmployeePercentYangLao);
                arr.Add(x.EmployeePercentYiLiao);
                arr.Add(x.EmployeePercentShiYe);
                arr.Add(x.EmployeePercentShengYu);
                arr.Add(x.EmployeePercentGongShang);
                arr.Add(x.EmployeeAmountCanBao);
                arr.Add(x.EmployeeAmountDaBingYiLiao);
                arr.Add(x.EmployeeAmountYiLiaoBuChong);
                arr.Add(x.UnitPercentYangLao);
                arr.Add(x.UnitPercentYiLiao);
                arr.Add(x.UnitPercentShiYe);
                arr.Add(x.UnitPercentShengYu);
                arr.Add(x.UnitPercentGongShang);
                arr.Add(x.UnitPercentCanBao);
                arr.Add(x.UnitAmountDaBingYiLiao);
                arr.Add(x.UnitAmountYiLiaoBuChong);
                arr.Add(x.EmployeePercentGongJiJin);
                arr.Add(x.UnitPercentGongJiJin);
                arr.Add(x.BaseDifferenceAmount);
                arr.Add(x.BaseRemark);

                foreach (var value in arr)
                {
                    ExcelHeler.SetCellValueNoStyle(workbook, sheet,style, font1, index + 1, row, value);
                    row++;
                }

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

        public byte[] CreateDefaultExcel()
        {
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("Sheet1");
            var font = workbook.CreateFont();
            font.FontHeightInPoints = 9;
            font.FontName = "微软雅黑";
            font.IsBold = true;
            double width = 20 * 256;
            short height = 20 * 20;

            sheet.DefaultRowHeight = height;
            //for (int x = 0; x < 6; x++)
            //{
            //    sheet.SetColumnWidth(x, width);
            //}
            //sheet.SetColumnWidth(6, 25 * 256);
            //for (int y = 0; y < 27; y++)
            //{
            //    sheet.CreateRow(y);
            //}
            #region excel 表头
            string[] heads1 = {
                "姓名",
                "身份证号码",
                "客户单位",
                "性别",
                "民族",
                "联系方式",
                "身份证地址",
                "教育程度",
                "方案类型",
                "预设方案",
                "区域选择（省）",
                "区域选择（市）",
                "增减规则",
                "时间节点",
                "社保供应商",
                "公积金供应商",
                "社保费用产生月",
                "社保费用不产生月",
                "公积金费用产生月",
                "公积金费用不产生月",
                "社保基数","社保基数","社保基数","社保基数","社保基数","社保基数",
                "公积金基数",
                "社保个人比例","社保个人比例","社保个人比例","社保个人比例","社保个人比例","社保个人比例","社保个人比例","社保个人比例",
                "社保企业缴费","社保企业缴费","社保企业缴费","社保企业缴费","社保企业缴费","社保企业缴费","社保企业缴费","社保企业缴费",
                "公积金","公积金",
                "补退金额",
                "备注"
            };

            string[] heads2 = {
               "姓名",
                "身份证号码",
                "客户单位",
                "性别",
                "民族",
                "联系方式",
                "身份证地址",
                "教育程度",
                "方案类型",
                "预设方案",
                "区域选择（省）",
                "区域选择（市）",
                "增减规则",
                "时间节点",
                "社保供应商",
                "公积金供应商",
                "社保费用产生月",
                "社保费用不产生月",
                "公积金费用产生月",
                "公积金费用不产生月",
                "养老基数","医疗基数","失业基数","生育基数","工伤基数","残保金基数",
                "公积金基数",
                "养老比例%","医疗比例%","失业比例%","生育比例%","工伤比例%","残保金","大病医疗","医疗补充",
                "养老比例","医疗比例%","失业比例%","生育比例%","工伤比例v","残保金%","大病医疗","医疗补充",
                "公积金个人比例%","公积金单位比例%",
                "补退金额",
                "备注"

            };
            ExcelHeler.SetHeader(workbook, sheet, font, heads1, heads2, width, height);
            #endregion
            #region 表格主体

            var font1 = workbook.CreateFont();
            font1.FontHeightInPoints = 9;
            font1.FontName = "微软雅黑";
            int row = 0;
            sheet.CreateRow(2);
            ExcelHeler.SetCellValue(workbook, sheet, font1, 2, row, "测试人员");
            row++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, 2, row, "XXXXXXXXXXXXXXX");
            row++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, 2, row, "XXXXX有限公司");
            row++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, 2, row, "男");
            row++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, 2, row, "汉族");
            row++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, 2, row, "18768541109");
            row++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, 2, row, "浙江省宁波市XXXXX");
            row++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, 2, row, "本科");
            row++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, 2, row, "社保方案");
            row++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, 2, row, "江西南昌默认方案");
            row++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, 2, row, "江西省");
            row++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, 2, row, "南昌市");
            row++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, 2, row, "当月增减当月");
            row++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, 2, row, "01");
            row++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, 2, row, "服务商A");
            row++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, 2, row, "服务商B");
            row++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, 2, row, "2025/1/1");
            row++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, 2, row, "");
            row++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, 2, row, "2025/1/1");
            row++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, 2, row, "");
            row++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, 2, row, "1000");
            row++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, 2, row, 1000);
            row++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, 2, row, 1000);
            row++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, 2, row, 1000);
            row++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, 2, row, 1000);
            row++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, 2, row, 1000);
            row++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, 2, row, 1000);
            row++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, 2, row, 10);
            row++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, 2, row, 10);
            row++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, 2, row, 10);
            row++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, 2, row, 10);
            row++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, 2, row, 10);
            row++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, 2, row, 100);
            row++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, 2, row, 100);
            row++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, 2, row, 100);
            row++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, 2, row, 10);
            row++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, 2, row, 10);
            row++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, 2, row, 10);
            row++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, 2, row, 10);
            row++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, 2, row, 10);
            row++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, 2, row, 10);
            row++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, 2, row, 100);
            row++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, 2, row, 100);
            row++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, 2, row, 10);
            row++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, 2, row, 22.12);
            row++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, 2, row, 300);
            row++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, 2, row, "备注");
            row++;

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
