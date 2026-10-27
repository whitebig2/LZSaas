using LZ.Model;
using SqlSugar.IOC;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LZ.Core.Interface;
using LZ.Users.Service.Interface;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.Util;
using LZ.Common;
using NPOI.SS.Formula.Functions;
using NPOI.HSSF.UserModel;
using Senparc.Weixin.MP.AdvancedAPIs.Card;
using System.Drawing;
using LZ.Core;

namespace LZ.Users.Service.Services
{
    public class InjuryInfoService : ServicesBase<Live_InjuryInfo>, IInjuryInfoService, IScopedDependency
    {
        public async Task<List<Model.Live_V_InjuryInfo>> List(Expression<Func<Model.Live_V_InjuryInfo, bool>> exp, Model.PagedModel.UserPagedModel query, string order = "")
        {
            var count = new RefAsync<int>();
            var sco = base.Context.Queryable<Live_V_InjuryInfo>().Where(exp);
            sco.OrderBy("ShowTime desc,InjuryTime desc");
            //sco.OrderBy(t => t.InjuryTime, OrderByType.Desc);
            //sco.OrderBy(t => t.ShowTime, OrderByType.Desc);
            //if (!string.IsNullOrEmpty(order))
            //{
            //    sco.OrderBy(order);
            //}

            if (query.Page == 0)
            {
                return await sco.ToListAsync();
            }
            var list = await sco.ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<int> Add(Model.Live_InjuryInfo model)
        {
            var db = base.Context;
            return await db.Insertable(model).ExecuteReturnIdentityAsync();
        }
        public async Task<bool> Update(Model.Live_InjuryInfo model)
        {
            var db = base.Context;
            return await db.Updateable(model).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> UpdateStatus(int id,int status,string msg = "")
        {
           
            var db = base.Context;
            var ub = db.Updateable<Live_InjuryInfo>();
            if(status>0)
            {
                ub.SetColumns(it => it.ProcessStatus == status);
            }
            if(!string.IsNullOrEmpty(msg))
            {
                ub.SetColumns(it => it.FailMsg == msg);
            }
            ub.Where(t => t.ID == id);
            return await ub.ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Delete(int id)
        {
            var db = base.Context;
            return await db.Deleteable<Model.Live_InjuryInfo>().Where(t => t.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Exist(Expression<Func<Live_InjuryInfo, bool>> exp)
        {
            return await base.Context.Queryable<Live_InjuryInfo>().AnyAsync(exp);
        }
        public async Task<Model.Live_V_InjuryInfo> GetView(int id)
        {
            var db = base.Context;
            return await db.Queryable<Model.Live_V_InjuryInfo>().Where(t => t.ID == id).FirstAsync();
        }
        public async Task<Model.Live_InjuryInfo> Get(int id)
        {
            var db = base.Context;
            return await db.Queryable<Model.Live_InjuryInfo>().Where(t => t.ID == id).FirstAsync();
        }
        public async Task<Model.Live_InjuryInfo> View(Expression<Func<Model.Live_InjuryInfo, bool>> exp)
        {
            var db = base.Context;
            return await db.Queryable<Model.Live_InjuryInfo>().FirstAsync(exp);
        }
        public async Task<bool> ExistID(int id)
        {
            return await base.Context.Queryable<Live_InjuryInfo>().AnyAsync(it => it.ID == id);
        }
        public async Task<int> DictionarySet(Dictionary<string, object> dt)
        {
            return await base.Context.Updateable<Model.Live_InjuryInfo>(dt).With(SqlWith.UpdLock).ExecuteCommandAsync();
        }
        public byte[] ApplyCompensationExportExcel(Live_InjuryInfo model)
        {

            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("Sheet1");
            var font = workbook.CreateFont();
            font.FontHeightInPoints = 10;
            font.FontName = "微软雅黑";
            //font.IsBold = true;
            double width = 14 * 256;
            short height = 23 * 20;

            sheet.DefaultRowHeight = height;
            for (int x = 0; x < 6; x++)
            {
                sheet.SetColumnWidth(x, width);
            }
            sheet.SetColumnWidth(6, 25 * 256);
            for (int y = 0; y < 27; y++)
            {
                sheet.CreateRow(y);
            }
            
            ICellStyle style = workbook.CreateCellStyle();
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            
            style.SetFont(font);
            #region 生成工伤结算表
            //第1行
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 0, 0, 0, 6, "工伤费用结算表");
            //第2行
            ExcelHeler.SetCellValue(workbook, sheet, font, 1, 0, "姓名");
            ExcelHeler.SetCellValue(workbook, sheet, font, 1, 1, model.UserName);
            ExcelHeler.SetCellValue(workbook, sheet, font, 1, 2, "性别");
            ExcelHeler.SetCellValue(workbook, sheet, font, 1, 3, model.UserSex);
            ExcelHeler.SetCellValue(workbook, sheet, font, 1, 4, "身份证");
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 1, 1, 5, 6, model.UserCardNo);
            //第3行
            ExcelHeler.SetCellValue(workbook, sheet, font, 2, 0, "单位名称");
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 2, 2, 1, 2, model.CustomerName);
            ExcelHeler.SetCellValue(workbook, sheet, font, 2, 3, "工伤时间");
            ExcelHeler.SetCellValue(workbook, sheet, font, 2, 4, model.InjuryTime.HasValue?model.InjuryTime.Value.ToString("yyyy-MM-dd"):"/");
            ExcelHeler.SetCellValue(workbook, sheet, font, 2, 5, "伤残等级");
            ExcelHeler.SetCellValue(workbook, sheet, font, 2, 6, model.InjuryLevel);
            //第4行
            ExcelHeler.SetCellValue(workbook, sheet, font, 3, 0, "事故类型");
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 3, 3, 1, 6, model.InjuryType);
            //第5行
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 4, 4, 0, 6, "银行信息");
            //第6行
            ExcelHeler.SetCellValue(workbook, sheet, font, 5, 0, "代理人姓名");
            ExcelHeler.SetCellValue(workbook, sheet, font, 5, 1, model.AgentName);
            ExcelHeler.SetCellValue(workbook, sheet, font, 5, 2, "开户银行");
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 5, 5, 3, 4, model.AgentBank);
            ExcelHeler.SetCellValue(workbook, sheet, font, 5, 5, "银行卡号");
            ExcelHeler.SetCellValue(workbook, sheet, font, 5, 6, model.AgentBankNo);
            //第7行
            ExcelHeler.SetCellValue(workbook, sheet, font, 6, 0, "员工姓名");
            ExcelHeler.SetCellValue(workbook, sheet, font, 6, 1, model.UserName);
            ExcelHeler.SetCellValue(workbook, sheet, font, 6, 2, "开户银行");
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 6, 6, 3, 4, model.UserBank);
            ExcelHeler.SetCellValue(workbook, sheet, font, 6, 5, "银行卡号");
            ExcelHeler.SetCellValue(workbook, sheet, font, 6, 6, model.UserBankNo);
            //8
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 7, 7, 0, 1, "工伤赔付项目");
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 7, 7, 2, 3, "赔偿员工金额");
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 7, 7, 4, 5, "保险理赔金额");
            ExcelHeler.SetCellValue(workbook, sheet, font, 7, 6, "风险金承担金额");
            //9
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 8, 8, 0, 1, "医疗费");
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 8, 8, 2, 3, model.StaffMedicalExpense);
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 8, 8, 4, 5, model.InsuranceMedicalExpense);
            ExcelHeler.SetCellValue(workbook, sheet, font, 8, 6, model.RiskMedicalExpense);
            //10
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 9, 9, 0, 1, "误工费");
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 9, 9, 2, 3, model.StaffLossOfWorkingTime);
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 9, 9, 4, 5, model.InsuranceLossOfWorkingTime);
            ExcelHeler.SetCellValue(workbook, sheet, font, 9, 6, model.RiskLossOfWorkingTime);
            //11
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 10, 10, 0, 1, "住院津贴");
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 10, 10, 2, 3, model.StaffHospitalizationAllowance);
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 10, 10, 4, 5, model.InsuranceHospitalizationAllowance);
            ExcelHeler.SetCellValue(workbook, sheet, font, 10, 6, model.RiskHospitalizationAllowance);
            //12
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 11, 11, 0, 1, "住院伙食补助费");
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 11, 11, 2, 3, model.StaffHospitalizationMealAllowance);
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 11, 11, 4, 5, model.InsuranceHospitalizationMealAllowance);
            ExcelHeler.SetCellValue(workbook, sheet, font, 11, 6, model.RiskHospitalizationMealAllowance);
            //13
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 12, 12, 0, 1, "交通费");
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 12, 12, 2, 3, model.StaffTransportationCosts);
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 12, 12, 4, 5, model.InsuranceTransportationCosts);
            ExcelHeler.SetCellValue(workbook, sheet, font, 12, 6, model.RiskTransportationCosts);
            //14
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 13, 13, 0, 1, "护工费");
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 13, 13, 2, 3, model.StaffNursingFees);
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 13, 13, 4, 5, model.InsuranceNursingFees);
            ExcelHeler.SetCellValue(workbook, sheet, font, 13, 6, model.RiskNursingFees);
            //15
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 14, 14, 0, 1, "辅助器具费");
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 14, 14, 2, 3, model.StaffAssistiveDevices);
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 14, 14, 4, 5, model.InsuranceAssistiveDevices);
            ExcelHeler.SetCellValue(workbook, sheet, font, 14, 6, model.RiskAssistiveDevices);
            //16
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 15, 15, 0, 1, "一次性伤残补助金");
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 15, 15, 2, 3, model.StaffOneTimeDisabilityAllowance);
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 15, 15, 4, 5, model.InsuranceOneTimeDisabilityAllowance);
            ExcelHeler.SetCellValue(workbook, sheet, font, 15, 6, model.RiskOneTimeDisabilityAllowance);
            //17
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 16, 16, 0, 1, "一次性就业补助金");
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 16, 16, 2, 3, model.StaffOneTimeEmploymentSubsidy);
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 16, 16, 4, 5, model.InsuranceOneTimeEmploymentSubsidy);
            ExcelHeler.SetCellValue(workbook, sheet, font, 16, 6, model.RiskOneTimeEmploymentSubsidy);
            //18
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 17, 17, 0, 1, "一次性医疗补助金");
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 17, 17, 2, 3, model.StaffOneTimeMedicalSubsidy);
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 17, 17, 4, 5, model.InsuranceOneTimeMedicalSubsidy);
            ExcelHeler.SetCellValue(workbook, sheet, font, 17, 6, model.RiskOneTimeMedicalSubsidy);
            //19
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 18, 18, 0, 1, "三个一次性打包费用");
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 18, 18, 2, 3, model.StaffOneTimePackAmount);
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 18, 18, 4, 5, model.InsuranceOneTimePackAmount);
            ExcelHeler.SetCellValue(workbook, sheet, font, 18, 6, model.RiskOneTimePackAmount);
            //20
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 19, 19, 0, 1, "一次性工亡补助金");
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 19, 19, 2, 3, model.StaffOneTimeWorkDeathAllowance);
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 19, 19, 4, 5, model.InsuranceOneTimeWorkDeathAllowance);
            ExcelHeler.SetCellValue(workbook, sheet, font, 19, 6, model.RiskOneTimeWorkDeathAllowance);
            //21
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 20, 20, 0, 1, "鉴定费");
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 20, 20, 2, 3, model.StaffAppraisalCosts);
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 20, 20, 4, 5, model.InsuranceAppraisalCosts);
            ExcelHeler.SetCellValue(workbook, sheet, font, 20, 6, model.RiskAppraisalCosts);
            //22
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 21, 21, 0, 1, "其它费用");
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 21, 21, 2, 3, model.StaffOtherCosts);
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 21, 21, 4, 5, model.InsuranceOtherCosts);
            ExcelHeler.SetCellValue(workbook, sheet, font, 21, 6, model.RiskOtherCosts);
            //23
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 22, 22, 0, 1, "工伤赔付小计");
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 22, 22, 2, 3, model.StaffSubtotal);
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 22, 22, 4, 5, model.InsuranceSubtotal);
            ExcelHeler.SetCellValue(workbook, sheet, font, 22, 6, model.RiskSubtotal);
            //24
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 23, 23, 0, 1, "定期费用");
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 23, 23, 2, 3, model.StaffPeriodicFee);
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 23, 23, 4, 5, model.InsurancePeriodicFee);
            ExcelHeler.SetCellValue(workbook, sheet, font, 23, 6, model.RiskPeriodicFee);
            //25
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 24, 24, 0, 1, "支付周期（月）");
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 24, 24, 2, 3, model.StaffPaymentCycle);
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 24, 24, 4, 5, model.InsurancePaymentCycle);
            ExcelHeler.SetCellValue(workbook, sheet, font, 24, 6,"/");
            //26
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 25, 25, 0, 1, "定期费用合计");
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 25, 25, 2, 3, model.StaffPeriodicTotalAmount);
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 25, 25, 4, 5, model.InsurancePeriodicTotalAmount);
            ExcelHeler.SetCellValue(workbook, sheet, font, 25, 6, model.RiskPeriodicTotalAmount);
            //27
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 26, 26, 0, 1, "合计");
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 26, 26, 2, 3, model.StaffTotalAmount);
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 26, 26, 4, 5, model.InsuranceTotalAmount);
            ExcelHeler.SetCellValue(workbook, sheet, font, 26, 6, model.RiskTotalAmount);
            #endregion

            //string filePath = System.IO.Directory.GetCurrentDirectory() + $"/uploadfiles/excel/{model.UserName}工伤结算表.xlsx";
            //using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            //{
            //    workbook.Write(stream);
            //}


            byte[] buffer = new byte[1024 * 5];
            using (MemoryStream stream = new MemoryStream())
            {
                workbook.Write(stream);
                buffer = stream.ToArray();
            }
            return buffer;
        }

        public async Task<Live_InjuryExtension> Test(Expression<Func<Model.Live_InjuryExtension, bool>> exp)
        {
            return await base.Context
                .Queryable<Live_InjuryExtension>()
                .Where(exp)
                .Includes(f => f.Records)
                .FirstAsync();
        }
        public async Task<byte[]> StatisticsExportExcel(List<Live_V_InjuryInfo> list)
        {
            var expable = Expressionable.Create<Model.SYS_Dictionary>();

            string query = "a4ee9d24-9d41-441d-bcb9-955c0a5c9d4a";
            var ids = query.Split(',');
            expable.And(it => it.PID.HasValue && ids.Contains(it.PID.ToString()));
            var dics = await base.Context.Queryable<SYS_Dictionary>().Where(expable.ToExpression()).OrderBy(it => it.Sort).ToListAsync();
            var cyclelist = dics.Where(t => t.PID == Guid.Parse("a4ee9d24-9d41-441d-bcb9-955c0a5c9d4a")).ToList();

            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("Sheet1");
            var font = workbook.CreateFont();
            font.FontHeightInPoints = 9;
            font.FontName = "微软雅黑";
            font.IsBold = true;
            double width = 14 * 256;//默认宽度
            short height = 20 * 20;//默认高度

            sheet.DefaultRowHeight = height;
            for (int x = 0; x < 6; x++)
            {
                sheet.SetColumnWidth(x, width);//设置宽度
            }
            sheet.SetColumnWidth(6, 25 * 256);
            #region excel 表头
            string[] heads1 = { 
                "序号",
                "员工姓名",
                "身份证号",
                "性别",
                "服务商",
                "客户单位",
                "工种",
                "入职时间",
                "险种",
                "工伤时间",
                "工伤地点",
                "事故报告",
                "受伤部位",
                "流程状态",
                "是否需要鉴定",
                "鉴定等级",
                "员工赔付金额","员工赔付金额","员工赔付金额","员工赔付金额","员工赔付金额","员工赔付金额","员工赔付金额","员工赔付金额","员工赔付金额","员工赔付金额","员工赔付金额","员工赔付金额","员工赔付金额","员工赔付金额","员工赔付金额",
                "员工赔付金额合计",
                "工伤理赔金额","工伤理赔金额","工伤理赔金额","工伤理赔金额","工伤理赔金额","工伤理赔金额","工伤理赔金额","工伤理赔金额","工伤理赔金额","工伤理赔金额","工伤理赔金额","工伤理赔金额","工伤理赔金额","工伤理赔金额","工伤理赔金额",
                "工伤理赔金额合计",
                "工伤风险金金额","工伤风险金金额","工伤风险金金额","工伤风险金金额","工伤风险金金额","工伤风险金金额","工伤风险金金额","工伤风险金金额","工伤风险金金额","工伤风险金金额","工伤风险金金额","工伤风险金金额","工伤风险金金额","工伤风险金金额","工伤风险金金额",
                "工伤风险金金额合计",
                "是否仲裁"
            };

            string[] heads2 = {
                "序号",
                "员工姓名",
                "身份证号",
                "性别",
                "服务商",
                "客户单位",
                "工种",
                "入职时间",
                "险种",
                "工伤时间",
                "工伤地点",
                "事故报告",
                "受伤部位",
                "流程状态",
                "是否需要鉴定",
                "鉴定等级",
                "医疗费","误工费","住院津贴","住院伙食补助费","交通费","护工费","辅助器具费","一次性伤残补助金","一次性就业补助金","一次性医疗补助金","三个一次性打包费用","一次性工亡补助金","鉴定费","其它费用","定期费用",
                "员工赔付金额合计",
                "医疗费","误工费","住院津贴","住院伙食补助费","交通费","护工费","辅助器具费","一次性伤残补助金","一次性就业补助金","一次性医疗补助金","三个一次性打包费用","一次性工亡补助金","鉴定费","其它费用","定期费用",
                "工伤理赔金额合计",
                "医疗费","误工费","住院津贴","住院伙食补助费","交通费","护工费","辅助器具费","一次性伤残补助金","一次性就业补助金","一次性医疗补助金","三个一次性打包费用","一次性工亡补助金","鉴定费","其它费用","定期费用",
                "工伤风险金金额合计",
                "是否仲裁"
            };
            ExcelHeler.SetHeader(workbook, sheet,font, heads1, heads2, width, height);

            #endregion

            #region 表格主体
            var font1 = workbook.CreateFont();
            font1.FontHeightInPoints = 9;
            font1.FontName = "微软雅黑";
            int index = 1;
            
            Model.Live_V_InjuryInfo last = new Live_V_InjuryInfo();
            foreach(var x in list)
            {
                var cycle = cyclelist.Find(t => t.NO == x.ProcessStatus.ToString());
                int row = 0;
                sheet.CreateRow(index + 1);//创建一行，不创建则无法填充数据
                ExcelHeler.SetCellValue(workbook, sheet, font1, index+1, row, index);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.UserName);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.UserCardNo);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.UserSex);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.PartnerName);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.CustomerName);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.UserType);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.EntryTime.HasValue ? x.EntryTime.Value.ToString("yyyy年MM月dd日") : "/");
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, ""); //险种
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.InjuryTime.HasValue ? x.InjuryTime.Value.ToString("yyyy年MM月dd日") : "/");
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.CustomerName); 
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.InjuryDescription);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.InjuryType);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, cycle == null ? "" : cycle.Name);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.IsDisabilityAssessed ? "是": "否");
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.InjuryLevel);
                row++;

                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.StaffMedicalExpense);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.StaffLossOfWorkingTime);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.StaffHospitalizationAllowance);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.StaffHospitalizationMealAllowance);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.StaffTransportationCosts);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.StaffNursingFees);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.StaffAssistiveDevices);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.StaffOneTimeDisabilityAllowance);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.StaffOneTimeEmploymentSubsidy);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.StaffOneTimeMedicalSubsidy);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.StaffOneTimePackAmount);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.StaffOneTimeWorkDeathAllowance);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.StaffAppraisalCosts);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.StaffOtherCosts);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.StaffPeriodicTotalAmount);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.StaffTotalAmount);
                row++;

                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.InsuranceMedicalExpense);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.InsuranceLossOfWorkingTime);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.InsuranceHospitalizationAllowance);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.InsuranceHospitalizationMealAllowance);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.InsuranceTransportationCosts);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.InsuranceNursingFees);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.InsuranceAssistiveDevices);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.InsuranceOneTimeDisabilityAllowance);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.InsuranceOneTimeEmploymentSubsidy);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.InsuranceOneTimeMedicalSubsidy);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.InsuranceOneTimePackAmount);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.InsuranceOneTimeWorkDeathAllowance);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.InsuranceAppraisalCosts);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.InsuranceOtherCosts);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.InsurancePeriodicTotalAmount);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.InsuranceTotalAmount);
                row++;

                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.RiskMedicalExpense);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.RiskLossOfWorkingTime);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.RiskHospitalizationAllowance);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.RiskHospitalizationMealAllowance);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.RiskTransportationCosts);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.RiskNursingFees);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.RiskAssistiveDevices);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.RiskOneTimeDisabilityAllowance);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.RiskOneTimeEmploymentSubsidy);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.RiskOneTimeMedicalSubsidy);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.RiskOneTimePackAmount);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.RiskOneTimeWorkDeathAllowance);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.RiskAppraisalCosts);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.RiskOtherCosts);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.RiskPeriodicTotalAmount);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.RiskTotalAmount);
                row++;
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, x.IsArbitration ? "是" : "否");
                row++;

                last.StaffMedicalExpense += x.StaffMedicalExpense;
                last.StaffLossOfWorkingTime += x.StaffLossOfWorkingTime;
                last.StaffHospitalizationAllowance += x.StaffHospitalizationAllowance;
                last.StaffHospitalizationMealAllowance += x.StaffHospitalizationMealAllowance;
                last.StaffTransportationCosts += x.StaffTransportationCosts;
                last.StaffNursingFees += x.StaffNursingFees;
                last.StaffAssistiveDevices += x.StaffAssistiveDevices; 
                last.StaffOneTimeDisabilityAllowance += x.StaffOneTimeDisabilityAllowance;
                last.StaffOneTimeEmploymentSubsidy += x.StaffOneTimeEmploymentSubsidy;
                last.StaffOneTimeMedicalSubsidy += x.StaffOneTimeMedicalSubsidy;
                last.StaffOneTimePackAmount += x.StaffOneTimePackAmount;
                last.StaffOneTimeWorkDeathAllowance += x.StaffOneTimeWorkDeathAllowance;
                last.StaffAppraisalCosts += x.StaffAppraisalCosts;
                last.StaffOtherCosts += x.StaffOtherCosts;
                last.StaffPeriodicTotalAmount += x.StaffPeriodicTotalAmount;
                last.StaffTotalAmount += x.StaffTotalAmount;

                last.InsuranceMedicalExpense += x.InsuranceMedicalExpense;
                last.InsuranceLossOfWorkingTime += x.InsuranceLossOfWorkingTime;
                last.InsuranceHospitalizationAllowance += x.InsuranceHospitalizationAllowance;
                last.InsuranceHospitalizationMealAllowance += x.InsuranceHospitalizationMealAllowance;
                last.InsuranceTransportationCosts += x.InsuranceTransportationCosts;
                last.InsuranceNursingFees += x.InsuranceNursingFees;
                last.InsuranceAssistiveDevices += x.InsuranceAssistiveDevices;
                last.InsuranceOneTimeDisabilityAllowance += x.InsuranceOneTimeDisabilityAllowance;
                last.InsuranceOneTimeEmploymentSubsidy += x.InsuranceOneTimeEmploymentSubsidy;
                last.InsuranceOneTimeMedicalSubsidy += x.InsuranceOneTimeMedicalSubsidy;
                last.InsuranceOneTimePackAmount += x.InsuranceOneTimePackAmount;
                last.InsuranceOneTimeWorkDeathAllowance += x.InsuranceOneTimeWorkDeathAllowance;
                last.InsuranceAppraisalCosts += x.InsuranceAppraisalCosts;
                last.InsuranceOtherCosts += x.InsuranceOtherCosts;
                last.InsurancePeriodicTotalAmount += x.InsurancePeriodicTotalAmount;
                last.InsuranceTotalAmount += x.InsuranceTotalAmount;

                last.RiskMedicalExpense += x.RiskMedicalExpense;
                last.RiskLossOfWorkingTime += x.RiskLossOfWorkingTime;
                last.RiskHospitalizationAllowance += x.RiskHospitalizationAllowance;
                last.RiskHospitalizationMealAllowance += x.RiskHospitalizationMealAllowance;
                last.RiskTransportationCosts += x.RiskTransportationCosts;
                last.RiskNursingFees += x.RiskNursingFees;
                last.RiskAssistiveDevices += x.RiskAssistiveDevices;
                last.RiskOneTimeDisabilityAllowance += x.RiskOneTimeDisabilityAllowance;
                last.RiskOneTimeEmploymentSubsidy += x.RiskOneTimeEmploymentSubsidy;
                last.RiskOneTimeMedicalSubsidy += x.RiskOneTimeMedicalSubsidy;
                last.RiskOneTimePackAmount += x.RiskOneTimePackAmount;
                last.RiskOneTimeWorkDeathAllowance += x.RiskOneTimeWorkDeathAllowance;
                last.RiskAppraisalCosts += x.RiskAppraisalCosts;
                last.RiskOtherCosts += x.RiskOtherCosts;
                last.RiskPeriodicTotalAmount += x.RiskPeriodicTotalAmount;
                last.RiskTotalAmount += x.RiskTotalAmount;



                index++;
            }

            #endregion

            #region 合计
            sheet.CreateRow(index + 1);
            int lastrow = 15;
            ExcelHeler.SetCellRangeValue(workbook, sheet, font1, index+1, index+1, 0, lastrow, "合计");
            lastrow++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, lastrow, last.StaffMedicalExpense);
            lastrow++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, lastrow, last.StaffLossOfWorkingTime);
            lastrow++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, lastrow, last.StaffHospitalizationAllowance);
            lastrow++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, lastrow, last.StaffHospitalizationMealAllowance);
            lastrow++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, lastrow, last.StaffTransportationCosts);
            lastrow++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, lastrow, last.StaffNursingFees);
            lastrow++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, lastrow, last.StaffAssistiveDevices);
            lastrow++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, lastrow, last.StaffOneTimeDisabilityAllowance);
            lastrow++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, lastrow, last.StaffOneTimeEmploymentSubsidy);
            lastrow++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, lastrow, last.StaffOneTimeMedicalSubsidy);
            lastrow++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, lastrow, last.StaffOneTimePackAmount);
            lastrow++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, lastrow, last.StaffOneTimeWorkDeathAllowance);
            lastrow++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, lastrow, last.StaffAppraisalCosts);
            lastrow++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, lastrow, last.StaffOtherCosts);
            lastrow++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, lastrow, last.StaffPeriodicTotalAmount);
            lastrow++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, lastrow, last.StaffTotalAmount);
            lastrow++;

            ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, lastrow, last.InsuranceMedicalExpense);
            lastrow++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, lastrow, last.InsuranceLossOfWorkingTime);
            lastrow++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, lastrow, last.InsuranceHospitalizationAllowance);
            lastrow++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, lastrow, last.InsuranceHospitalizationMealAllowance);
            lastrow++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, lastrow, last.InsuranceTransportationCosts);
            lastrow++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, lastrow, last.InsuranceNursingFees);
            lastrow++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, lastrow, last.InsuranceAssistiveDevices);
            lastrow++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, lastrow, last.InsuranceOneTimeDisabilityAllowance);
            lastrow++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, lastrow, last.InsuranceOneTimeEmploymentSubsidy);
            lastrow++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, lastrow, last.InsuranceOneTimeMedicalSubsidy);
            lastrow++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, lastrow, last.InsuranceOneTimePackAmount);
            lastrow++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, lastrow, last.InsuranceOneTimeWorkDeathAllowance);
            lastrow++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, lastrow, last.InsuranceAppraisalCosts);
            lastrow++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, lastrow, last.InsuranceOtherCosts);
            lastrow++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, lastrow, last.InsurancePeriodicTotalAmount);
            lastrow++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, lastrow, last.InsuranceTotalAmount);
            lastrow++;

            ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, lastrow, last.RiskMedicalExpense);
            lastrow++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, lastrow, last.RiskLossOfWorkingTime);
            lastrow++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, lastrow, last.RiskHospitalizationAllowance);
            lastrow++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, lastrow, last.RiskHospitalizationMealAllowance);
            lastrow++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, lastrow, last.RiskTransportationCosts);
            lastrow++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, lastrow, last.RiskNursingFees);
            lastrow++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, lastrow, last.RiskAssistiveDevices);
            lastrow++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, lastrow, last.RiskOneTimeDisabilityAllowance);
            lastrow++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, lastrow, last.RiskOneTimeEmploymentSubsidy);
            lastrow++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, lastrow, last.RiskOneTimeMedicalSubsidy);
            lastrow++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, lastrow, last.RiskOneTimePackAmount);
            lastrow++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, lastrow, last.RiskOneTimeWorkDeathAllowance);
            lastrow++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, lastrow, last.RiskAppraisalCosts);
            lastrow++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, lastrow, last.RiskOtherCosts);
            lastrow++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, lastrow, last.RiskPeriodicTotalAmount);
            lastrow++;
            ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, lastrow, last.RiskTotalAmount);
            lastrow++;

            ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, lastrow, "/");
            lastrow++;
            #endregion

            byte[] buffer = new byte[1024 * 5];
            using (MemoryStream stream = new MemoryStream())
            {
                workbook.Write(stream);
                buffer = stream.ToArray();
            }
            return buffer;


            //string filePath = System.IO.Directory.GetCurrentDirectory() + "/uploadfiles/excel/社保员工账单模板.xlsx";
            //using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            //{
            //    workbook.Write(stream);
            //}
            //return filePath;
        }
    }
}
