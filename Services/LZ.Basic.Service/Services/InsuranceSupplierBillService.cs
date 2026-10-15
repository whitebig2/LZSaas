using LZ.Basic.Service.Interface;
using LZ.Core.Interface;
using LZ.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LZ.Model;
using SqlSugar.IOC;
using SqlSugar;
using System.Linq.Expressions;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using static System.Runtime.InteropServices.JavaScript.JSType;
using LZ.Common;
using LZ.Model.Enums.Insurance;
using NPOI.SS.Formula.Functions;
using LZ.Model.Enums;

namespace LZ.Basic.Service.Services
{
    public class InsuranceSupplierBillService : ServicesBase<Live_InsuranceSupplierBill>, IInsuranceSupplierBillService, IScopedDependency
    {
        public async Task<List<Model.Live_V_InsuranceSupplierBill>> List(Expression<Func<Model.Live_V_InsuranceSupplierBill, bool>> exp, Model.PagedModel.BasicPagedModel query, string order = "")
        {
            var count = new RefAsync<int>();
            var sco = base.Context.Queryable<Live_V_InsuranceSupplierBill>().Where(exp);
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

        public async Task<int> Add(Model.Live_InsuranceSupplierBill model)
        {
            var db = base.Context;
            return await db.Insertable(model).ExecuteCommandAsync();
        }

        public async Task<int> Adds(List<Model.Live_InsuranceSupplierBill> models)
        {
            var db = base.Context;
            return await db.Insertable(models).ExecuteCommandAsync();
        }
        public async Task<bool> Update(Model.Live_InsuranceSupplierBill model)
        {
            var db = base.Context;
            return await db.Updateable(model).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Delete(int id)
        {
            var db = base.Context;
            return await db.Deleteable<Model.Live_InsuranceSupplierBill>().Where(t => t.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Exist(Expression<Func<Live_InsuranceSupplierBill, bool>> exp)
        {
            return await base.Context.Queryable<Live_InsuranceSupplierBill>().AnyAsync(exp);
        }
        public async Task<Model.Live_InsuranceSupplierBill> Get(int id)
        {
            var db = base.Context;
            return await db.Queryable<Model.Live_InsuranceSupplierBill>().Where(t => t.ID == id).FirstAsync();
        }
        public async Task<bool> ExistID(int id)
        {
            return await base.Context.Queryable<Live_InsuranceSupplierBill>().AnyAsync(it => it.ID == id);
        }
        public async Task<bool> ExistCardID(int? companyID, DateTime date)
        {
            return await base.Context.Queryable<Live_InsuranceSupplierBill>().AnyAsync(it => it.YlSupplierId == companyID && it.BillMonth == date);
        }

        public async Task<int> CheckAndSaveCompany(List<Live_Settlement> settles)
        {
            List<Model.Live_InvoiceCompany> list = new List<Live_InvoiceCompany>();
            foreach (var model in settles)
            {
                var issuer = await base.Context.Queryable<Model.Live_InvoiceCompany>().Where(t => t.CompanyName == model.IssuerName).FirstAsync();
                if (issuer == null)
                {
                    Model.Live_InvoiceCompany item = new Live_InvoiceCompany();
                    item.ID = Guid.NewGuid();
                    item.CompanyName = model.IssuerName;
                    item.CompanyNo = model.IssuerCompanyNo;
                    item.LegalPerson = "";
                    item.Address = model.IssuerAddress;
                    item.Phone = model.IssuerPhone;
                    item.Bank = model.IssuerBank;
                    item.BankNo = model.IssuerBankNo;
                    item.IsEnabled = true;
                    item.CreateTime = DateTime.Now;
                    item.UserID = 0;
                    list.Add(item);
                }
                var recipient = await base.Context.Queryable<Model.Live_InvoiceCompany>().Where(t => t.CompanyName == model.RecipientName).FirstAsync();

                if (recipient == null)
                {
                    Model.Live_InvoiceCompany item = new Live_InvoiceCompany();
                    item.ID = Guid.NewGuid();
                    item.CompanyName = model.RecipientName;
                    item.CompanyNo = model.RecipientCompanyNo;
                    item.LegalPerson = "";
                    item.Address = model.RecipientAddress;
                    item.Phone = model.RecipientPhone;
                    item.Bank = model.RecipientBank;
                    item.BankNo = model.RecipientBankNo;
                    item.IsEnabled = true;
                    item.CreateTime = DateTime.Now;
                    item.UserID = 0;
                    list.Add(item);
                }
            }


            return await base.Context.Insertable(list).ExecuteCommandAsync();
        }

        public async Task<int> DictionarySet(Dictionary<string, object> dt)
        {
            return await base.Context.Updateable<Model.Live_InsuranceSupplierBill>(dt).With(SqlWith.UpdLock).ExecuteCommandAsync();
        }
        public async Task<(List<Live_InsuranceSupplierBill> list, string msg)> BatchEmployeeCreate(DateTime date, int cid)
        {
            List<Live_InsuranceSupplierBill> bills = new List<Live_InsuranceSupplierBill>();
            string msg = "";
            try
            {
                var sco = base.Context.Queryable<Live_InsuranceSupplier>().Where(t => t.Enable == true);

                var sco1 = base.Context.Queryable<Live_V_InsuranceEmployeeBill>().Where(t => t.BillMonth == date.AddMonths(-1) || t.BillMonth == date);
                var sco2 = base.Context.Queryable<Live_V_InsuranceSupplierBill>().Where(t => t.BillMonth == date && t.ProcessStatus!=6);

                if (cid > 0)
                {
                    sco.Where(t => t.CompanyID == cid);
                    sco1.Where(t => t.CompanyID == cid);
                    sco2.Where(t => t.CompanyID == cid);
                }

                var list = await sco.ToListAsync();
                var models = await sco1.ToListAsync();
                var existModels = await sco2.ToListAsync();



                foreach (var x in list)//供应商
                {

                    var smodels = models.Where(t => ((t.YlSupplierIdSheBao == x.ID && t.StartDateSheBao.HasValue && t.BillMonth == date.AddMonths(-1)) || (t.YlSupplierIdGongJiJin == x.ID && t.StartDateGongJiJin.HasValue && t.BillMonth == date)) && x.CompanyID == x.CompanyID).ToList();
                    if (smodels.Count > 0)//客户单位
                    {
                        List<string> added = new List<string>();
                        foreach (var sm in smodels)
                        {


                            Live_InsuranceSupplierBill bill = new Live_InsuranceSupplierBill();
                            if (existModels.Where(t => t.YlSupplierId == x.ID && t.CooperationUnitGID == sm.CooperationUnitGID && t.CompanyID == x.CompanyID).ToList().Count > 0)
                            {
                                continue;
                            }
                            if (sm.CooperationUnitName == x.JointCompanyName || sm.CooperationUnitName == x.Name)
                            {
                                continue;
                            }
                            try
                            {
                                var sb = bills.Where(t => t.YlSupplierId == x.ID && t.CooperationUnitGID == sm.CooperationUnitGID).FirstOrDefault();
                                if (sb != null)
                                {

                                    bill = sb;
                                    bills.Remove(sb);
                                    if (added.Find(t => t == sm.Name + sm.IdcardNo) == null)
                                    {
                                        bill.RecordCount = bill.RecordCount + 1;
                                        added.Add(sm.Name + sm.IdcardNo);
                                    }


                                    if (sm.BillMonth == date)
                                    {
                                        bill.EmployeeSumGongJiJin += sm.EmployeeAmounGongJiJin;
                                        bill.UnitSumGongJiJin += sm.UnitAmountGongJiJin;
                                        bill.TotalGongJiJin += sm.TotalAmountGongJiJin;
                                        bill.TotalServiceFee += sm.ServiceBaseAmountGongJiJin;
                                        bill.TotalAmount += (sm.TotalAmountGongJiJin + sm.ServiceBaseAmountGongJiJin);
                                    }
                                    else
                                    {
                                        bill.EmployeeSumSheBao += sm.EmployeeSumSheBao;
                                        bill.UnitSumSheBao += sm.UnitSumSheBao;
                                        bill.TotalSheBao += sm.TotalAmountSheBao;
                                        bill.TotalServiceFee += sm.ServiceBaseAmountSheBao;
                                        bill.TotalAmount += (sm.TotalAmountSheBao + sm.ServiceBaseAmountSheBao);
                                    }
                                    bill.ActualAmount = bill.TotalAmount;
                                    bill.ProcessStatus = 2;

                                    bills.Add(bill);
                                }
                                else
                                {
                                    bill.CompanyID = x.CompanyID;
                                    bill.CompanyGID = x.CompanyGID;
                                    bill.BillMonth = date;
                                    bill.IsInvoicePrinted = false;
                                    bill.IsInvoicePaid = false;
                                    bill.IsSettlementPass = false;
                                    bill.CreateTime = DateTime.Now;
                                    bill.GongJiJinBill = date;
                                    bill.SheBaoBill = date.AddMonths(-1);
                                    bill.SupplierCompanyNo = x.EmployeeCompanyNo;
                                    bill.SupplierCompanyBank = x.EmployeeCompanyBank;
                                    bill.SupplierCompanyBankNo = x.EmployeeCompanyBankNo;

                                    bill.YlSupplierId = x.ID;
                                    bill.YlSupplierGID = x.SupplierGID;
                                    bill.YlSupplierName = x.Name;
                                    bill.InvoiceCompanyName = x.IsJoint ? x.JointCompanyName : x.Name;
                                    bill.CooperationUnitGID = sm.CooperationUnitGID;
                                    bill.CooperationName = sm.CooperationUnitName;
                                    bill.CooperationAddress = sm.CooperationUnitAddress;
                                    bill.CooperationPhone = sm.CooperationUnitPhone;
                                    bill.CooperationBank = sm.CooperationUnitBankName;
                                    bill.CooperationBankNo = sm.CooperationUnitBankNo;
                                    bill.CooperationNo = sm.CooperationUnitNo;
                                    //计算属性
                                    bill.RecordCount = 1;
                                    //bill.TotalServiceFee = sm.YlSupplierIdGongJiJin != bill.YlSupplierId ? sm.ServiceBaseAmountSheBao:sm.ServiceBaseAmountGongJiJin;
                                    if (sm.BillMonth == date)
                                    {
                                        bill.EmployeeSumGongJiJin = sm.EmployeeAmounGongJiJin;
                                        bill.UnitSumGongJiJin = sm.UnitAmountGongJiJin;
                                        bill.TotalGongJiJin = sm.TotalAmountGongJiJin;
                                        bill.TotalServiceFee = sm.ServiceBaseAmountGongJiJin;
                                        bill.TotalAmount = sm.TotalAmountGongJiJin + sm.ServiceBaseAmountGongJiJin;


                                    }
                                    else
                                    {
                                        bill.EmployeeSumSheBao = sm.EmployeeSumSheBao;
                                        bill.UnitSumSheBao = sm.UnitSumSheBao;
                                        bill.TotalSheBao = sm.TotalAmountSheBao;
                                        bill.TotalServiceFee = sm.ServiceBaseAmountSheBao;
                                        bill.TotalAmount = sm.TotalAmountSheBao + sm.ServiceBaseAmountSheBao;
                                    }
                                    added.Add(sm.Name + sm.IdcardNo);
                                    bill.ProcessStatus = 2;

                                    if (bill.TotalAmount > 0)
                                    {
                                        bill.ActualAmount = bill.TotalAmount;
                                        bills.Add(bill);
                                    }

                                }

                            }
                            catch (Exception e)
                            {
                                msg += sm.Name + "导入错误：" + e.ToString() + ",";
                            }


                        }


                    }
                    else
                    {
                        continue;
                    }
                }


            }
            catch (Exception ex)
            {
                msg = ex.ToString();
            }
            msg += "生成完成";
            return (bills, msg);
        }
        public async Task<byte[]> SupplierBillExcel(Model.Live_InsuranceSupplierBill model)
        {
            var sco = base.Context.Queryable<Live_V_InsuranceEmployeeBill>().Where(t => ((t.BillMonth == model.SheBaoBill && t.YlSupplierIdSheBao == model.YlSupplierId) || (t.BillMonth == model.GongJiJinBill && t.YlSupplierIdGongJiJin == model.YlSupplierId)) && t.CompanyID == model.CompanyID && t.CooperationUnitGID == model.CooperationUnitGID).OrderBy(t => t.BillMonth, OrderByType.Desc);

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
                "账单月份",
                "姓名",
                 "身份证",
                "参保地区","参保地区",
                "参保节点",
                "增减规则",
                "客户单位",
                "社保起缴月",
                "公积金起缴月",
                "社保缴纳月",
                "公积金缴纳月",
                "社保企业缴费","社保企业缴费","社保企业缴费","社保企业缴费","社保企业缴费","社保企业缴费","社保企业缴费","社保企业缴费","社保企业缴费","社保企业缴费","社保企业缴费","社保企业缴费","社保企业缴费","社保企业缴费","社保企业缴费","社保企业缴费","社保企业缴费","社保企业缴费","社保企业缴费",
               "社保个人缴费","社保个人缴费","社保个人缴费","社保个人缴费","社保个人缴费","社保个人缴费","社保个人缴费","社保个人缴费","社保个人缴费","社保个人缴费","社保个人缴费","社保个人缴费",
               "公积金","公积金","公积金","公积金","公积金","公积金","公积金",
               "社保合计",
                "服务费",
               "总计"

            };

            string[] heads2 = {
                "账单月份",
                "姓名",
                "身份证",
               "省","市",
               "参保节点",
               "增减规则",
               "客户单位",
               "社保起缴月",
               "公积金起缴月",
               "社保缴纳月",
               "公积金缴纳月",
               "养老基数","养老比例%","养老金额","医疗基数","医疗比例%","医疗金额","失业基数","失业比例%","失业金额","工伤基数","工伤比例%","工伤金额","生育基数","生育比例%","生育金额","大病保险","医疗补充","残保金","企业合计",
               "养老基数","养老比例%","养老金额","医疗基数","医疗比例%","医疗金额","失业基数","失业比例%","失业金额","大病保险","医疗补充","个人合计",
               "公积金基数（个人）","公积金个人比例%","个人金额","公积金基数（企业）","公积金单位比例%","企业金额","合计",
               "社保合计",
               "服务费",
               "总计"
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
                List<object> arr = new();
                arr.Add(x.BillMonth.HasValue ? x.BillMonth.Value : "");
                arr.Add(x.Name);
                arr.Add(x.IdcardNo);
                arr.Add(x.Province);
                arr.Add(x.City);
                arr.Add(x.TimeNode.HasValue ? x.TimeNode.Value.ToString() : "01");
                arr.Add((InsuranceSocialRuleEnum)x.YlRule);
                arr.Add(x.EmployeeCompanyName);
                arr.Add(x.StartDateSheBao.HasValue ? x.StartDateSheBao : "/");
                arr.Add(x.StartDateGongJiJin.HasValue ? x.StartDateGongJiJin : "/");

                if (x.BillMonth == model.BillMonth)
                {
                    var shebao = list.Find(t => t.IdcardNo == x.IdcardNo && t.BillMonth != model.BillMonth);
                    if (shebao != null)
                    {
                        arr.Add(shebao.BillMonth);
                        arr.Add(x.BillMonth);
                        arr.Add(shebao.BaseAmountYangLao);
                        arr.Add(shebao.UnitPercentYangLao);
                        arr.Add(shebao.UnitAmountYangLao);
                        arr.Add(shebao.BaseAmountYiLiao);
                        arr.Add(shebao.UnitPercentYiLiao);
                        arr.Add(shebao.UnitAmountYiLiao);
                        arr.Add(shebao.BaseAmountShiYe);
                        arr.Add(shebao.UnitPercentShiYe);
                        arr.Add(shebao.UnitAmountShiYe);
                        arr.Add(shebao.BaseAmountGongShang);
                        arr.Add(shebao.UnitPercentGongShang);
                        arr.Add(shebao.UnitAmountGongShang);
                        arr.Add(shebao.BaseAmountShengYu);
                        arr.Add(shebao.UnitPercentShengYu);
                        arr.Add(shebao.UnitAmountShengYu);
                        arr.Add(shebao.UnitAmountDaBingYiLiao);
                        arr.Add(shebao.UnitAmountYiLiaoBuChong);
                        arr.Add(shebao.UnitAmountCanBao);
                        arr.Add(shebao.UnitSumSheBao);

                        arr.Add(shebao.BaseAmountYangLao);
                        arr.Add(shebao.EmployeePercentYangLao);
                        arr.Add(shebao.EmployeeAmountYangLao);
                        arr.Add(shebao.BaseAmountYiLiao);
                        arr.Add(shebao.EmployeePercentYiLiao);
                        arr.Add(shebao.EmployeeAmountYiLiao);
                        arr.Add(shebao.BaseAmountShiYe);
                        arr.Add(shebao.EmployeePercentShiYe);
                        arr.Add(shebao.EmployeeAmountShiYe);
                        arr.Add(shebao.EmployeeAmountDaBingYiLiao);
                        arr.Add(shebao.EmployeeAmountYiLiaoBuChong);
                        arr.Add(shebao.EmployeeSumSheBao);
                    }
                    else
                    {
                        arr.Add("/");
                        arr.Add(x.BillMonth);
                        for (int i = 0; i < 31; i++)
                        {
                            arr.Add("/");
                        }
                    }




                    arr.Add(x.BaseAmountGongJiJin);
                    arr.Add(x.EmployeePercentGongJiJin);
                    arr.Add(x.EmployeeAmounGongJiJin);
                    arr.Add(x.BaseAmountGongJiJin);
                    arr.Add(x.UnitPercentGongJiJin);
                    arr.Add(x.UnitAmountGongJiJin);
                    arr.Add(x.TotalAmountGongJiJin);
                    if (shebao != null)
                    {
                        arr.Add(shebao.TotalAmountSheBao);
                        arr.Add(x.ServiceBaseAmountGongJiJin + shebao.ServiceBaseAmountSheBao);
                        arr.Add(shebao.TotalAmountSheBao + shebao.ServiceBaseAmountSheBao + x.TotalAmountGongJiJin + x.ServiceBaseAmountGongJiJin);
                    }
                    else
                    {
                        arr.Add("/");
                        arr.Add(x.ServiceBaseAmountGongJiJin);
                        arr.Add(x.TotalAmountGongJiJin + x.ServiceBaseAmountGongJiJin);
                    }



                }
                else
                {
                    var gongjijin = list.Find(t => t.IdcardNo == x.IdcardNo && t.BillMonth == model.BillMonth);
                    if (gongjijin != null)
                    {
                        continue;
                    }
                    arr.Add(x.BillMonth);
                    arr.Add("/");
                    arr.Add(x.BaseAmountYangLao);
                    arr.Add(x.UnitPercentYangLao);
                    arr.Add(x.UnitAmountYangLao);
                    arr.Add(x.BaseAmountYiLiao);
                    arr.Add(x.UnitPercentYiLiao);
                    arr.Add(x.UnitAmountYiLiao);
                    arr.Add(x.BaseAmountShiYe);
                    arr.Add(x.UnitPercentShiYe);
                    arr.Add(x.UnitAmountShiYe);
                    arr.Add(x.BaseAmountGongShang);
                    arr.Add(x.UnitPercentGongShang);
                    arr.Add(x.UnitAmountGongShang);
                    arr.Add(x.BaseAmountShengYu);
                    arr.Add(x.UnitPercentShengYu);
                    arr.Add(x.UnitAmountShengYu);
                    arr.Add(x.UnitAmountDaBingYiLiao);
                    arr.Add(x.UnitAmountYiLiaoBuChong);
                    arr.Add(x.UnitAmountCanBao);
                    arr.Add(x.UnitSumSheBao);

                    arr.Add(x.BaseAmountYangLao);
                    arr.Add(x.EmployeePercentYangLao);
                    arr.Add(x.EmployeeAmountYangLao);
                    arr.Add(x.BaseAmountYiLiao);
                    arr.Add(x.EmployeePercentYiLiao);
                    arr.Add(x.EmployeeAmountYiLiao);
                    arr.Add(x.BaseAmountShiYe);
                    arr.Add(x.EmployeePercentShiYe);
                    arr.Add(x.EmployeeAmountShiYe);
                    arr.Add(x.EmployeeAmountDaBingYiLiao);
                    arr.Add(x.EmployeeAmountYiLiaoBuChong);
                    arr.Add(x.EmployeeSumSheBao);
                    for (int i = 0; i < 7; i++)
                    {
                        arr.Add("/");
                    }
                    arr.Add(x.TotalAmountSheBao);
                    arr.Add(x.ServiceBaseAmountSheBao);
                    arr.Add(x.TotalAmountSheBao + x.ServiceBaseAmountSheBao);
                }





                int row = 0;
                sheet.CreateRow(index + 1);//index=1,从第三行开始,前2行是表头
                foreach (var value in arr)
                {
                    ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, row, value);
                    row++;
                }


                index++;
            }
            #endregion
            #region 合计
            List<object> lastArr = new List<object>();
            sheet.CreateRow(index + 1);
            int lastrow = 11;
            ExcelHeler.SetCellRangeValue(workbook, sheet, font1, index + 1, index + 1, 0, lastrow, "合计");
            lastrow++;

            for (int i = 0; i < 18; i++)
            {
                lastArr.Add("/");
            }
            lastArr.Add(model.UnitSumSheBao);
            for (int i = 0; i < 11; i++)
            {
                lastArr.Add("/");
            }
            lastArr.Add(model.EmployeeSumSheBao);
            for (int i = 0; i < 6; i++)
            {
                lastArr.Add("/");
            }
            lastArr.Add(model.TotalGongJiJin);
            lastArr.Add(model.TotalSheBao);
            lastArr.Add(model.TotalServiceFee);
            lastArr.Add(model.TotalAmount);
            foreach (var value in lastArr)
            {
                ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, lastrow, value);
                lastrow++;
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
