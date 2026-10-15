using LZ.Model;
using SqlSugar.IOC;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LZ.Basic.Service.Interface;
using LZ.Core.Interface;
using LZ.Core;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
using NPOI.SS.UserModel;
using LZ.Common;
using NPOI.XSSF.UserModel;
using LZ.Core.Services;
using LZ.Model.Exceptions;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using LZ.Model.Enums;
using Consul;

namespace LZ.Basic.Service.Services
{
    public class InsuranceCompanyBillService() : ServicesBase<Live_InsuranceCompanyBill>, IInsuranceCompanyBillService, IScopedDependency
    {
        public async Task<List<Model.Live_V_InsuranceCompanyBill>> List(Expression<Func<Model.Live_V_InsuranceCompanyBill, bool>> exp, Model.PagedModel.BasicPagedModel query, string order = "")
        {
            var count = new RefAsync<int>();
            var sco = base.Context.Queryable<Live_V_InsuranceCompanyBill>().Where(exp);
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

        public async Task<int> Add(Model.Live_InsuranceCompanyBill model)
        {
            var db = base.Context;
            return await db.Insertable(model).ExecuteCommandAsync();
        }

        public async Task<int> Adds(List<Model.Live_InsuranceCompanyBill> models)
        {
            var db = base.Context;
            return await db.Insertable(models).ExecuteCommandAsync();
        }
        public async Task<bool> Update(Model.Live_InsuranceCompanyBill model)
        {
            var db = base.Context;
            return await db.Updateable(model).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Delete(int id)
        {
            var db = base.Context;
            return await db.Deleteable<Model.Live_InsuranceCompanyBill>().Where(t => t.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Exist(Expression<Func<Live_InsuranceCompanyBill, bool>> exp)
        {
            return await base.Context.Queryable<Live_InsuranceCompanyBill>().AnyAsync(exp);
        }
        public async Task<Model.Live_InsuranceCompanyBill> Get(int id)
        {
            var db = base.Context;
            return await db.Queryable<Model.Live_InsuranceCompanyBill>().Where(t => t.ID == id).FirstAsync();
        }
        public async Task<bool> ExistID(int id)
        {
            return await base.Context.Queryable<Live_InsuranceCompanyBill>().AnyAsync(it => it.ID == id);
        }
        public async Task<bool> ExistCardID(int? companyID, DateTime date)
        {
            return await base.Context.Queryable<Live_InsuranceCompanyBill>().AnyAsync(it => it.EmployeeCompanyId == companyID && it.BillMonth == date);
        }

        public async Task<int> DictionarySet(Dictionary<string, object> dt)
        {
            return await base.Context.Updateable<Model.Live_InsuranceCompanyBill>(dt).With(SqlWith.UpdLock).ExecuteCommandAsync();
        }

        public async Task<(List<Model.Live_Invoice> list,string msg)> BatchEmployeeCreate(DateTime date,int cid)
        {
            List<Model.Live_Invoice> infos = new List<Live_Invoice>();
            string msg = "";
           
                var sco = base.Context.Queryable<Live_V_InsuranceEmployeeBill>().Distinct().Select(t => new { t.CompanyID, t.EmployeeCompanyId,t.EmployeeCompanyName, t.BillMonth,t.EmployeeCompanySocialEnabled }).Where(t => t.BillMonth == date && t.EmployeeCompanySocialEnabled==true).OrderBy(t => t.BillMonth);
                List<Live_InsuranceCompanyBill> bills = new List<Live_InsuranceCompanyBill>();
                var sco1 = base.Context.Queryable<Live_V_InsuranceEmployeeBill>().Where(t => t.BillMonth == date && t.EmployeeCompanySocialEnabled);
                var sco2 = base.Context.Queryable<Live_V_InsuranceCompanyBill>().Where(t => t.BillMonth == date && t.ProcessStatus !=6);

                if (cid > 0)
                {
                    sco.Where(t => t.CompanyID == cid);
                    sco1.Where(t => t.CompanyID == cid);
                    sco2.Where(t => t.CompanyID == cid);
                }

                var list = await sco.ToListAsync();
                var models = await sco1.ToListAsync();
                var existModels = await sco2.ToListAsync();

                foreach (var x in list)
                {
               
                    if (existModels.Where(t => t.EmployeeCompanyId == x.EmployeeCompanyId && t.CompanyID == x.CompanyID).ToList().Count > 0)
                    {
                        // 公司账单已存在
                        continue;
                    }


                    var smodels = models.Where(t => t.EmployeeCompanyId == x.EmployeeCompanyId && t.CompanyID == x.CompanyID).ToList();
                    if (smodels.Count > 0)
                    {
                        Live_InsuranceCompanyBill bill = new Live_InsuranceCompanyBill();
                        bill.CompanyID = smodels[0].CompanyID;
                        bill.CompanyGID = smodels[0].CompanyGID;
                        bill.BillMonth = date;
                        bill.IsInvoicePrinted = false;
                        bill.IsInvoicePaid = false;
                        bill.CreateTime = DateTime.Now;
                        try
                        {
                            bill.EmployeeCompanyId = smodels[0].EmployeeCompanyId;
                            bill.EmployeeCompanyGID = smodels[0].EmployeeCompanyGID;
                            bill.EmployeeCompanyName = smodels[0].EmployeeCompanyName;
                            bill.EmployeeCompanyNo = smodels[0].EmployeeCompanNo;
                            bill.EmployeeCompanyBank = smodels[0].EmployeeCompanyBank;
                            bill.EmployeeCompanyBankNo = smodels[0].EmployeeCompanyBankNo;
                            bill.EmployeeCompanyAddress = smodels[0].EmployeeCompanyAddress;
                            bill.EmployeeCompanyPhone = smodels[0].EmployeeCompanyPhone;
                        }
                        catch(Exception ex)
                        {
                        msg += string.Format("{0}({1})生成失败,客户单位(EmployeeCompany)信息不全", x.EmployeeCompanyName, x.EmployeeCompanyId);
                        continue;
                        }

                    try
                    {
                        bill.CooperationUnitGID = smodels[0].CooperationUnitGID;
                        bill.CooperationName = smodels[0].CooperationUnitName;
                        bill.CooperationNo = smodels[0].CooperationUnitNo;
                        bill.CooperationAddress = smodels[0].CooperationUnitAddress;
                        bill.CooperationPhone = smodels[0].CooperationUnitPhone;
                        bill.CooperationBank = smodels[0].CooperationUnitBankName;
                        bill.CooperationBankNo = smodels[0].CooperationUnitBankNo;
                    }
                    catch (Exception ex)
                    {
                        msg += string.Format("{0}({1})生成失败,商务合作(Cooperation)信息不全", x.EmployeeCompanyName, x.EmployeeCompanyId);
                        continue;
                    }
                        

                        bill.RecordCount = smodels.Count;
                        bill.EmployeeSumSheBao = smodels.Sum(t => t.EmployeeSumSheBao);
                        bill.UnitSumSheBao = smodels.Sum(t => t.UnitSumSheBao);
                        bill.EmployeeSumGongJiJin = smodels.Sum(t => t.EmployeeAmounGongJiJin);
                        bill.UnitSumGongJiJin = smodels.Sum(t => t.UnitAmountGongJiJin);
                        bill.TotalServiceFee = smodels.Sum(t => t.EmployeeCompanySocialFee);
                        bill.TotalSheBao = smodels.Sum(t => t.TotalAmountSheBao);
                        bill.TotalGongJiJin = smodels.Sum(t => t.TotalAmountGongJiJin);
                        bill.TotalAmount = smodels.Sum(t => t.TotalAmount);
                        bill.TotalAmount = bill.TotalServiceFee + bill.TotalAmount;
                        bill.ActualAmount = bill.TotalAmount;
                        bill.ProcessStatus = 2;

                    try
                    {
                        Model.Live_Invoice invo = new Live_Invoice();
                        invo.ID = Guid.NewGuid();
                        bill.CompanyInvoiceId = invo.ID;
                        invo.CustomerGID = bill.EmployeeCompanyGID.Value;
                        //invo.CustomerName = bill.EmployeeCompanyName;
                        invo.CompanyID = bill.CompanyID;
                        invo.CustomerName = bill.EmployeeCompanyName;
                        invo.IssuerName = bill.CooperationName;//甲方
                        invo.IssuerCompanyNo = bill.CooperationNo;
                        invo.IssuerPhone = bill.CooperationPhone;
                        invo.IssuerAddress = bill.CooperationAddress;
                        invo.IssuerBank = bill.CooperationBank;
                        invo.IssuerBankNo = bill.CooperationBankNo;
                        invo.RecipientName = bill.EmployeeCompanyName;//乙方
                        invo.RecipientCompanyNo = bill.EmployeeCompanyNo;
                        invo.RecipientPhone = bill.EmployeeCompanyPhone;
                        invo.RecipientAddress = bill.EmployeeCompanyAddress;
                        invo.RecipientBank = bill.EmployeeCompanyBank ?? "无";
                        invo.RecipientBankNo = bill.EmployeeCompanyBankNo ?? "";
                        invo.InvoiceType = InvoiceTypeEnumType.ESpecialInvoice;
                        invo.BusinessType = PrintTemplateEnumType.社保公积金发票;
                        invo.Title = "人力资源服务*经纪人代理服务";
                        invo.ApplyUserID = 8;
                        invo.ApplyUserName = "lzadmin";
                        invo.IsAddedTax = false;
                        invo.IsAdditionalTax = false;
                        invo.AddedTaxAmount = 0;
                        invo.AdditionalTaxAmount = 0;
                        invo.AddedTaxDiscountAmount = 0;
                        invo.AdditionalTaxDiscountAmount = 0;
                        invo.ChangeAmount = 0;
                        invo.IsLocked = false;
                        invo.IsIssued = false;
                        invo.RecordCount = 0;
                        invo.ServiceFeeAmount = 0;
                        invo.Description = string.Format("{0}{1},代缴社保金额：{2}元,代缴公积金金额：{3}元,服务费金额{4}元", invo.RecipientName, date.ToString("yyyy年MM月"), bill.TotalSheBao.ToString("#0.######"), bill.TotalGongJiJin.ToString("#0.######"), bill.TotalServiceFee.ToString("#0.######"));
                        invo.Remark = "";
                        invo.ApplyTime = date;
                        invo.CreateTime = DateTime.Now;
                        invo.IssuerUnitID = bill.CooperationUnitGID.Value;
                        invo.RecipientUnitID = bill.EmployeeCompanyGID.Value;
                        invo.AddedTaxPointAmount = 0;
                        invo.AdditionalTaxPointAmount = 0;
                        invo.TotalAmount = bill.TotalAmount;
                        invo.InvoiceAmount = bill.TotalAmount;
                        invo.AddedTaxSumAmount = 0;
                        invo.AdditionTaxSumAmount = 0;
                        invo.SfAddedTaxAmount = 0;
                        invo.SfAddedTaxPointAmount = 0;
                        invo.SfPersonalTaxAmount = 0;
                        invo.SfPersonalTaxPointAmount = 0;
                        invo.IsSf = false;
                        invo.SfOtherPaidAmount = 0;
                        invo.SfSocialAmount = 0;
                        invo.SfTaxRefundAmount = 0;
                        invo.IsCancel = false;
                        invo.Status = InvoiceStatusEnumType.待锁定;

                        infos.Add(invo);


                        bills.Add(bill);
                    }
                    catch(Exception ex)
                    {
                        msg += string.Format("{0}({1})生成失败,发票生成失败", x.EmployeeCompanyName, x.EmployeeCompanyId);
                        continue;
                    }
                        

                    }
                    else
                    {
                        continue;
                    }
            }
                if (bills.Count > 0)
                {
                    int i = await Adds(bills);
                }

            msg += "生成结束";
            return (infos, msg);
        }
        public async Task<byte[]> CompanyBillExcel(Model.Live_InsuranceCompanyBill model)
        {

            var sco = base.Context.Queryable<Live_V_InsuranceEmployeeBill>().Where(t =>t.EmployeeCompanyId==model.EmployeeCompanyId && t.BillMonth == model.BillMonth && t.CompanyID==model.CompanyID && t.EmployeeCompanySocialEnabled);


            var list = await sco.ToListAsync();
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("Sheet1");
            var font = workbook.CreateFont();
            font.FontHeightInPoints = 9;
            font.FontName = "微软雅黑";
            font.IsBold = true;
            double width = 14 * 256;
            short height = 20 * 20;

            sheet.DefaultRowHeight = height;
            #region excel 表头
            string[] heads1 = {
                "参保地区","参保地区",
                "参保节点",
                "增减规则",
                "社保起缴月",
                "公积金起缴月",
                "账单生成月",
                "社保缴纳月",
                "公积金缴纳月",
                "姓名",
                "身份证",
                "客户",
                "基本养老保险","基本养老保险","基本养老保险","基本养老保险","基本养老保险",
                "基本医疗保险","基本医疗保险","基本医疗保险","基本医疗保险","基本医疗保险",
                "失业保险","失业保险","失业保险","失业保险","失业保险",
                "生育保险","生育保险","生育保险",
                "工伤保险","工伤保险","工伤保险",
                "残疾人保障金","残疾人保障金",
                "重大疾病医疗","重大疾病医疗",
                "补充医疗险","补充医疗险",
                "个人缴纳合计",
               "单位缴纳合计",
               "社保合计",
               "公积金","公积金","公积金","公积金","公积金",
               "公积金合计",
               "服务费",
               "总计"

            };

            string[] heads2 = {
               "省","市",
               "参保节点",
               "增减规则",
               "社保起缴月",
               "公积金起缴月",
               "账单生成月",
               "社保缴纳月",
               "公积金缴纳月",
               "姓名",
               "身份证",
               "客户",
               "缴费基数","个人费率%","个人缴纳","单位费率%","单位缴纳",
               "缴费基数","个人费率%","个人缴纳","单位费率%","单位缴纳",
               "缴费基数","个人费率%","个人缴纳","单位费率%","单位缴纳",
               "缴费基数","单位费率%","单位缴纳",
               "缴费基数","单位费率%","单位缴纳",
               "个人缴纳","单位缴纳",
               "个人缴纳","单位缴纳",
               "个人缴纳","单位缴纳",
               "个人缴纳合计",
               "单位缴纳合计",
               "社保合计",
               "缴费基数","个人费率%","个人缴纳","单位费率%","单位缴纳",
               "公积金合计",
               "服务费",
               "总计"
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
                List<object> arr = new();
                arr.Add(x.Province);
                arr.Add(x.City);
                arr.Add(x.TimeNode.ToString() ?? "未知");
                arr.Add(x.RuleName);
                arr.Add(x.StartDateSheBao.HasValue?x.StartDateSheBao.Value.ToString("yyyy-MM"):"/");
                arr.Add(x.StartDateGongJiJin.HasValue?x.StartDateGongJiJin.Value.ToString("yyyy-MM") : "/");
                arr.Add(x.BillMonth.HasValue ? x.BillMonth.Value : "/");
                if(x.StartDateSheBao.HasValue)
                {
                    arr.Add(x.BillMonth.HasValue ? x.BillMonth.Value.ToString("yyyy-MM") : "/");
                }
                else
                {
                    arr.Add("/");
                }
                if (x.StartDateGongJiJin.HasValue)
                {
                    arr.Add(x.BillMonth.HasValue ? x.BillMonth.Value.ToString("yyyy-MM") : "/");
                }
                else
                {
                    arr.Add("/");
                }

                arr.Add(x.Name);
                arr.Add(x.IdcardNo);
                arr.Add(x.EmployeeCompanyName);
                if(x.StartDateSheBao.HasValue)
                {
                    arr.Add(x.BaseAmountYangLao);
                    arr.Add(x.EmployeePercentYangLao);
                    arr.Add(x.EmployeeAmountYangLao);
                    arr.Add(x.UnitPercentYangLao);
                    arr.Add(x.UnitAmountYangLao);
                    arr.Add(x.BaseAmountYiLiao);
                    arr.Add(x.EmployeePercentYiLiao);
                    arr.Add(x.EmployeeAmountYiLiao);
                    arr.Add(x.UnitPercentYiLiao);
                    arr.Add(x.UnitAmountYiLiao);
                    arr.Add(x.BaseAmountShiYe);
                    arr.Add(x.EmployeePercentShiYe);
                    arr.Add(x.EmployeeAmountShiYe);
                    arr.Add(x.UnitPercentShiYe);
                    arr.Add(x.UnitAmountShiYe);
                    arr.Add(x.BaseAmountShengYu);
                    arr.Add(x.UnitPercentShengYu);
                    arr.Add(x.UnitAmountShengYu);
                    arr.Add(x.BaseAmountGongShang);
                    arr.Add(x.UnitPercentGongShang);
                    arr.Add(x.UnitAmountGongShang);
                    arr.Add(x.EmployeeAmountCountCanBao);
                    arr.Add(x.UnitAmountCountCanBao);
                    arr.Add(x.EmployeeAmountDaBingYiLiao);
                    arr.Add(x.UnitAmountDaBingYiLiao);
                    arr.Add(x.EmployeeAmountYiLiaoBuChong);
                    arr.Add(x.UnitAmountYiLiaoBuChong);
                    arr.Add(x.EmployeeSumSheBao);
                    arr.Add(x.UnitSumSheBao);
                    arr.Add(x.TotalAmountSheBao);
                }
                else
                {
                    for (int i = 0; i < 30; i++)
                    {
                        arr.Add("/");
                    }
                }
                if (x.StartDateGongJiJin.HasValue)
                {
                    arr.Add(x.BaseAmountGongJiJin);
                    arr.Add(x.EmployeePercentGongJiJin);
                    arr.Add(x.EmployeeAmounGongJiJin);
                    arr.Add(x.UnitPercentGongJiJin);
                    arr.Add(x.UnitAmountGongJiJin);
                    arr.Add(x.TotalAmountGongJiJin);
                    
                }
                else
                {
                    for (int i = 0; i < 6; i++)
                    {
                        arr.Add("/");
                    }
                }

                arr.Add(x.EmployeeCompanySocialFee);
                arr.Add(x.TotalAmount + x.EmployeeCompanySocialFee);




                int row = 0;
                sheet.CreateRow(index + 1);//index=1,从第三行开始,前2行是表头
                foreach (var value in arr)
                {
                    ExcelHeler.SetCellValueNoStyle(workbook, sheet, style, font1, index + 1, row, value);
                    row++;
                }


                index++;
            }
            #endregion

            #region 合计
            List<object> lastArr = new List<object>();
            for (int i = 0; i < 27; i++)
            {
                lastArr.Add("/");
            }
            lastArr.Add(model.EmployeeSumSheBao);
            lastArr.Add(model.UnitSumSheBao);
            lastArr.Add(model.TotalSheBao);
            for (int i = 0; i < 5; i++)
            {
                lastArr.Add("/");
            }
            lastArr.Add(model.TotalGongJiJin);
            lastArr.Add(model.TotalServiceFee);
            lastArr.Add(model.TotalAmount);


            sheet.CreateRow(index + 1);
            int lastrow = 11;
            ExcelHeler.SetCellRangeValue(workbook, sheet, font1, index + 1, index + 1, 0, lastrow, "合计");
            lastrow++;
            foreach (var value in lastArr)
            {
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, lastrow, value);
                lastrow++;
            }
            index++;
            int start = index + 1;
            for(int i = 0; i<5; i++)
            {
                sheet.CreateRow(index+1);
                index++;
            }
            string info = string.Format("汇款信息:\n开户名称 :{0}\n开户银行:{1}\n人民币帐号:{2}", model.CooperationName, model.CooperationBank, model.CooperationBankNo);
            XSSFRichTextString richString = new XSSFRichTextString(info);
            ExcelHeler.SetCellRangeValue(workbook, sheet, font1, start +1 , index, 0, 4, richString);



            #endregion







            byte[] buffer = new byte[1024 * 5];
            using (MemoryStream stream = new MemoryStream())
            {
                workbook.Write(stream);
                buffer = stream.ToArray();
            }
            return buffer;
        }


        public byte[] PayNoticeExcel(Model.Live_InsuranceCompanyBill model)
        {
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("Sheet1");
            var font = workbook.CreateFont();
            font.FontHeightInPoints = 9;
            font.FontName = "微软雅黑";
            font.IsBold = true;
            double width = 14 * 256;
            short height = 20 * 20;

            sheet.DefaultRowHeight = height;
            sheet.SetColumnWidth(0,42*256);
            for (int x = 1; x < 6; x++)
            {
                sheet.SetColumnWidth(x, width);
            }
            
            sheet.CreateRow(0);
            sheet.CreateRow(1);
            ExcelHeler.SetCellRangeValue(workbook, sheet, font, 0, 1, 0, 5, model.EmployeeCompanyName+"付款通知书");

            ICellStyle style = workbook.CreateCellStyle();
            style.Alignment = HorizontalAlignment.Left;
            style.VerticalAlignment = VerticalAlignment.Center;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderTop = BorderStyle.Thin;
            style.WrapText = true;
            style.IsHidden = true;
            style.SetFont(font);

            ICellStyle style1 = workbook.CreateCellStyle();
            style1.Alignment = HorizontalAlignment.Left;
            style1.VerticalAlignment = VerticalAlignment.Center;
            style1.BorderLeft = BorderStyle.Thin;
            style1.BorderBottom = BorderStyle.Thin;
            style1.BorderRight = BorderStyle.Thin;
            style1.BorderTop = BorderStyle.Thin;
            style1.WrapText = true;
            style1.IsHidden = true;
            var font1 = workbook.CreateFont();
            font1.FontHeightInPoints = 9;
            font1.FontName = "微软雅黑";
            style1.SetFont(font1);

            sheet.CreateRow(2);
            ExcelHeler.CommonSetCell(workbook, sheet, style, 2, 0, "帐单月（社保缴纳月）：");
            ExcelHeler.CommonSetCellRangeValue(workbook, sheet, style, 2, 2,1,5, model.BillMonth.HasValue?model.BillMonth.Value.ToString("yyyy-MM"):"");
            sheet.CreateRow(3);
            ExcelHeler.CommonSetCell(workbook, sheet, style1, 3, 0, "单位名称");
            ExcelHeler.CommonSetCell(workbook, sheet, style1, 3, 1, "人数");
            ExcelHeler.CommonSetCell(workbook, sheet, style1, 3, 2, "服务费");
            ExcelHeler.CommonSetCell(workbook, sheet, style1, 3, 3, "社保合计金额");
            ExcelHeler.CommonSetCell(workbook, sheet, style1, 3, 4, "公积金合计");
            ExcelHeler.CommonSetCell(workbook, sheet, style1, 3, 5, "总合计");
            sheet.CreateRow(4);
            ExcelHeler.CommonSetCell(workbook, sheet, style1, 4, 0,model.EmployeeCompanyName);
            ExcelHeler.CommonSetCell(workbook, sheet, style1, 4, 1, model.RecordCount.HasValue?model.RecordCount:0);
            ExcelHeler.CommonSetCell(workbook, sheet, style1, 4, 2, model.TotalServiceFee);
            ExcelHeler.CommonSetCell(workbook, sheet, style1, 4, 3, model.TotalSheBao);
            ExcelHeler.CommonSetCell(workbook, sheet, style1, 4, 4, model.TotalGongJiJin);
            ExcelHeler.CommonSetCell(workbook, sheet, style1, 4, 5, model.TotalAmount);

            int start = 7;
            int last = start;
            for (int i = 0; i < 5; i++)
            {
                sheet.CreateRow(last);
                last++;
            }
            string info = string.Format("汇款信息:\n开户名称 :{0}\n开户银行:{1}\n人民币帐号:{2}", model.CooperationName, model.CooperationBank, model.CooperationBankNo);
            XSSFRichTextString richString = new XSSFRichTextString(info);
            ExcelHeler.SetCellRangeValue(workbook, sheet, font1, start, last, 0, 4, richString);

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
