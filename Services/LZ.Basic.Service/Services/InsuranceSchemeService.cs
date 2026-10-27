using LZ.Basic.Service.Interface;
using LZ.Common;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Model;
using LZ.Model.PagedModel;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using SqlSugar;
using SqlSugar.IOC;
using System.Linq.Expressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LZ.Basic.Service.Services
{
    public class InsuranceSchemeService(IRegionAddressService regionAddressService) : ServicesBase<Live_InsuranceScheme>, IInsuranceSchemeService, IScopedDependency
    {
        public async Task<List<Model.Live_InsuranceScheme>> List(Expression<Func<Model.Live_InsuranceScheme, bool>> exp, BasicPagedModel query, string order = "")
        {
            var count = new RefAsync<int>();
            var sco = base.Context.Queryable<Live_InsuranceScheme>().Where(exp);
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
        public async Task<List<Model.Live_V_InsuranceScheme>> ListV(Expression<Func<Model.Live_V_InsuranceScheme, bool>> exp, BasicPagedModel query, string order = "")
        {
            var count = new RefAsync<int>();
            var sco = base.Context.Queryable<Live_V_InsuranceScheme>().Where(exp);
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

        public async Task<int> Add(Model.Live_InsuranceScheme model)
        {
            var db = base.Context;
            return await db.Insertable(model).ExecuteCommandAsync();
        }

        public async Task<int> Adds(List<Model.Live_InsuranceScheme> models)
        {
            var db = base.Context;
            return await db.Insertable(models).ExecuteCommandAsync();
        }
        public async Task<bool> Update(Model.Live_InsuranceScheme model)
        {
            var db = base.Context;
            return await db.Updateable(model).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Delete(int id)
        {
            var db = base.Context;
            return await db.Deleteable<Model.Live_InsuranceScheme>().Where(t => t.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Exist(Expression<Func<Live_InsuranceScheme, bool>> exp)
        {
            return await base.Context.Queryable<Live_InsuranceScheme>().AnyAsync(exp);
        }
        public async Task<Model.Live_InsuranceScheme> Get(int id)
        {
            var db = base.Context;
            return await db.Queryable<Model.Live_InsuranceScheme>().Where(t => t.ID == id).FirstAsync();
        }
        public async Task<Model.Live_V_InsuranceScheme> GetView(int id)
        {
            var db = base.Context;
            return await db.Queryable<Model.Live_V_InsuranceScheme>().Where(t => t.ID == id).FirstAsync();
        }
        public async Task<bool> ExistID(int id)
        {
            return await base.Context.Queryable<Live_InsuranceScheme>().AnyAsync(it => it.ID == id);
        }

         public async Task<Model.Live_InsuranceEmployeeBill> CalculationModel(int id)
        {
            var item = await GetView(id);
            Model.Live_InsuranceEmployeeBill bill = new Live_InsuranceEmployeeBill();
            bill.YlSupplierIdSheBao = item.YlSupplierIdSheBao;
            bill.BaseAmountYangLao = item.BaseAmountYangLao;
            bill.BaseAmountYiLiao = item.BaseAmountYiLiao;
            bill.BaseAmountShiYe = item.BaseAmountShiYe;
            bill.BaseAmountShengYu = item.BaseAmountShengYu;
            bill.BaseAmountGongShang = item.BaseAmountGongShang;
            bill.BaseAmountCanBao = item.BaseAmountCanBao;
            bill.UnitPercentYangLao = item.UnitPercentYangLao;
            bill.UnitPercentYiLiao = item.UnitPercentYiLiao;
            bill.UnitPercentShiYe = item.UnitPercentShiYe;
            bill.UnitPercentShengYu = item.UnitPercentShengYu;
            bill.UnitPercentGongShang = item.UnitPercentGongShang;
            bill.UnitPercentCanBao = item.UnitPercentCanBao;
            bill.EmployeePercentYangLao = item.EmployeePercentYangLao;
            bill.EmployeePercentYiLiao = item.EmployeePercentYiLiao;
            bill.EmployeePercentShiYe = item.EmployeePercentShiYe;
            bill.EmployeePercentShengYu = item.EmployeePercentShengYu;
            bill.EmployeePercentGongShang = item.EmployeePercentGongShang;
            bill.EmployeePercentCanBao = item.EmployeePercentCanBao;
            bill.EmployeeAmountCanBao = item.EmployeeAmountCanBao;
            bill.EmployeeAmountDaBingYiLiao = item.EmployeeAmountDaBingYiLiao;
            bill.EmployeeAmountYiLiaoBuChong = item.EmployeeAmountYiLiaoBuChong;
            bill.UnitAmountCanBao = item.UnitAmountCanBao;
            bill.UnitAmountDaBingYiLiao = item.UnitAmountDaBingYiLiao;
            bill.UnitAmountYiLiaoBuChong = item.UnitAmountYiLiaoBuChong;
            bill.YlSupplierIdGongJiJin = item.YlSupplierIdGongJiJin;
            bill.BaseAmountGongJiJin = item.BaseAmountGongJiJin;
            bill.EmployeePercentGongJiJin = item.EmployeePercentGongJiJin;
            bill.UnitPercentGongJiJin = item.UnitPercentGongJiJin;
            bill.YlRule = item.YlRule;
            bill.TimeNode = item.TimeNode;


            //计算属性
            bill.ServiceBaseAmountGongJiJin = 0;
            bill.UnitAmountGongJiJin = 0;
            bill.EmployeeAmounGongJiJin = 0;
            bill.ServiceBaseAmountSheBao = 0;
            bill.UnitAmountYangLao = 0;
            bill.EmployeeAmountYangLao = 0;
            bill.UnitAmountYiLiao = 0;
            bill.EmployeeAmountYiLiao = 0;
            bill.UnitAmountShiYe = 0;
            bill.EmployeeAmountShiYe = 0;
            bill.UnitAmountShengYu = 0;
            bill.EmployeeAmountShengYu = 0;
            bill.UnitAmountGongShang = 0;
            bill.EmployeeAmountGongShang = 0;
            bill.UnitAmountCountCanBao = 0;
            bill.EmployeeAmountCountCanBao = 0;


            //公积金计算
            bill.UnitAmountGongJiJin = Math.Ceiling(item.BaseAmountGongJiJin * item.UnitPercentGongJiJin/ 100);
            bill.EmployeeAmounGongJiJin = Math.Ceiling(item.BaseAmountGongJiJin * item.EmployeePercentGongJiJin / 100);
            bill.ServiceBaseAmountGongJiJin = item.GongJiJinServiceAmount != null ? (decimal)item.GongJiJinServiceAmount : 0;
            //社保计算

            bill.UnitAmountYangLao = decimal.Round(item.BaseAmountYangLao * item.UnitPercentYangLao / 100, 2, MidpointRounding.AwayFromZero);
            bill.EmployeeAmountYangLao = decimal.Round(item.BaseAmountYangLao * item.EmployeePercentYangLao / 100, 2, MidpointRounding.AwayFromZero);
            // 医疗
            bill.UnitAmountYiLiao = decimal.Round(item.BaseAmountYiLiao * item.UnitPercentYiLiao / 100, 2, MidpointRounding.AwayFromZero);
            bill.EmployeeAmountYiLiao = decimal.Round(item.BaseAmountYiLiao * item.EmployeePercentYiLiao / 100, 2, MidpointRounding.AwayFromZero);

            // 失业
            bill.UnitAmountShiYe = decimal.Round(item.BaseAmountShiYe * item.UnitPercentShiYe / 100, 2, MidpointRounding.AwayFromZero);
            bill.EmployeeAmountShiYe = decimal.Round(item.BaseAmountShiYe * item.EmployeePercentShiYe / 100, 2, MidpointRounding.AwayFromZero);


            // 生育
            bill.UnitAmountShengYu = decimal.Round(item.BaseAmountShengYu * item.UnitPercentShengYu / 100, 2, MidpointRounding.AwayFromZero);
            bill.EmployeeAmountShengYu = decimal.Round(item.BaseAmountShengYu * item.EmployeePercentShengYu / 100, 2, MidpointRounding.AwayFromZero);
            // 工伤
            bill.UnitAmountGongShang = decimal.Round(item.BaseAmountGongShang * item.UnitPercentGongShang / 100, 2, MidpointRounding.AwayFromZero);
            bill.EmployeeAmountGongShang = decimal.Round(item.BaseAmountShengYu * item.EmployeePercentGongShang / 100, 2, MidpointRounding.AwayFromZero);

            // 保障金
            bill.UnitAmountCountCanBao = decimal.Round(item.BaseAmountCanBao * item.UnitPercentCanBao / 100, 2, MidpointRounding.AwayFromZero);
            bill.EmployeeAmountCountCanBao = item.EmployeeAmountCanBao;
            bill.ServiceBaseAmountSheBao = (item.SheBaoServiceAmount != null ? (decimal)item.SheBaoServiceAmount : 0);

            bill.EmployeeSumSheBao =
                            bill.EmployeeAmountYangLao
                          + bill.EmployeeAmountYiLiao
                          + bill.EmployeeAmountShiYe
                          + bill.EmployeeAmountCountCanBao
                          + bill.EmployeeAmountDaBingYiLiao
                          + bill.EmployeeAmountYiLiaoBuChong;


            bill.UnitSumSheBao =
                bill.UnitAmountYangLao
              + bill.UnitAmountYiLiao
              + bill.UnitAmountShiYe
              + bill.UnitAmountShengYu
              + bill.UnitAmountGongShang
              + bill.UnitAmountCountCanBao
              + bill.UnitAmountDaBingYiLiao
              + bill.UnitAmountYiLiaoBuChong;

            bill.TotalAmountSheBao = bill.EmployeeSumSheBao + bill.UnitSumSheBao;

            bill.TotalAmountGongJiJin = bill.UnitAmountGongJiJin + bill.EmployeeAmounGongJiJin;

            bill.ServiceFeeAmount = bill.ServiceBaseAmountGongJiJin + bill.ServiceBaseAmountSheBao;
            bill.TotalAmount = bill.TotalAmountSheBao + bill.TotalAmountGongJiJin;

            return bill;
        }
        public async Task<byte[]> ExportExcel(List<Live_V_InsuranceScheme> list)
        {

            var expable = Expressionable.Create<Model.SYS_Dictionary>();

            var rexpable = Expressionable.Create<Model.Live_RegionAddress>();
            var regions = await regionAddressService.List(rexpable.ToExpression());
            string query = "ecf62977-c49f-4ea3-84bb-2c541aef4805";
            var ids = query.Split(',');
            expable.And(it => it.PID.HasValue && ids.Contains(it.PID.ToString()));
            var dics = await base.Context.Queryable<SYS_Dictionary>().Where(expable.ToExpression()).OrderBy(it => it.Sort).ToListAsync();
            var rules = dics.Where(t => t.PID == Guid.Parse("ecf62977-c49f-4ea3-84bb-2c541aef4805")).ToList();






            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("Sheet1");
            var font = workbook.CreateFont();
            font.FontHeightInPoints = 9;
            font.FontName = "微软雅黑";
            font.IsBold = true;
            double width = 14 * 256;
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
                "预设方案",
                "区域选择（省）",
                "区域选择（市）",
                "增减规则",
                "时间节点",
                "社保供应商",
                "公积金供应商",
                "社保基数","社保基数","社保基数","社保基数","社保基数","社保基数",
                "公积金基数",
                "社保个人比例","社保个人比例","社保个人比例","社保个人比例","社保个人比例","社保个人比例","社保个人比例","社保个人比例",
                "社保企业缴费","社保企业缴费","社保企业缴费","社保企业缴费","社保企业缴费","社保企业缴费","社保企业缴费","社保企业缴费",
                "公积金","公积金",
                "补退金额",
                "备注"
            };

            string[] heads2 = {
                "预设方案",
                "区域选择（省）",
                "区域选择（市）",
                "增减规则",
                "时间节点",
                "社保供应商",
                "公积金供应商",
                "养老基数","医疗基数","失业基数","生育基数","工伤基数","残保金基数",
                "公积金基数",
                "养老比例","医疗比例","失业比例","生育比例","工伤比例","残保金","大病医疗","医疗补充",
                "养老比例","医疗比例","失业比例","生育比例","工伤比例","残保金","大病医疗","医疗补充",
                "公积金个人比例","公积金单位比例",
                "补退金额",
                "备注"

            };
            ExcelHeler.SetHeader(workbook, sheet, font, heads1, heads2, width, height);

            #endregion

            #region 表格主体
            var font1 = workbook.CreateFont();
            font1.FontHeightInPoints = 9;
            font1.FontName = "微软雅黑";
            int index = 1;



            foreach (var x in list)
            {
                var rule = rules.Find(t => t.NO == x.YlRule.ToString());
                int row = 0;
                sheet.CreateRow(index + 1);//index=1,从第三行开始,前2行是表头
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.Name);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.Province);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.City);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, rule == null ? "" : rule.Name);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.TimeNode.HasValue ? x.TimeNode.Value.ToString() : "");
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.SheBaoSupplierName);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.GongJiJinSupplierName); 
                row++;;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.BaseAmountYangLao);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.BaseAmountYiLiao);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.BaseAmountShiYe);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.BaseAmountShengYu);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.BaseAmountGongShang);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.BaseAmountCanBao);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.BaseAmountGongJiJin);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.EmployeePercentYangLao);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.EmployeePercentYiLiao);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.EmployeePercentShiYe);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.EmployeePercentShengYu);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.EmployeePercentGongShang);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.EmployeeAmountCanBao);//社保个人残保金
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.EmployeeAmountDaBingYiLiao);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.EmployeeAmountYiLiaoBuChong);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.UnitPercentYangLao);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.UnitPercentYiLiao);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.UnitPercentShiYe);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.UnitPercentShengYu);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.UnitPercentGongShang);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.UnitPercentCanBao);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.UnitAmountDaBingYiLiao);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.UnitAmountYiLiaoBuChong);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.EmployeePercentGongJiJin);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.UnitPercentGongJiJin);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.BaseDifferenceAmount);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.BaseRemark);
                row++;
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


            //string filePath = System.IO.Directory.GetCurrentDirectory() + "/uploadfiles/社保员工账单模板.xlsx";
            //using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            //{
            //    workbook.Write(stream);
            //}
            //return filePath;
        }

    }
}
