using Consul;
using LZ.Basic.Service.Interface;
using LZ.Common;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Core.Services;
using LZ.Model;
using LZ.Model.Enums.Insurance;
using MathNet.Numerics;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using SqlSugar;
using SqlSugar.IOC;
using System.DirectoryServices.ActiveDirectory;
using System.Linq.Expressions;

namespace LZ.Basic.Service.Services
{
    public class InsuranceEmployeeBillService(IAuthService authService) : ServicesBase<Live_InsuranceEmployeeBill>, IInsuranceEmployeeBillService, IScopedDependency
    {
        public async Task<List<Model.Live_V_InsuranceEmployeeBill>> List(Expression<Func<Model.Live_V_InsuranceEmployeeBill, bool>> exp, Model.PagedModel.BasicPagedModel query, string order = "")
        {
            var count = new RefAsync<int>();
            var sco = base.Context.Queryable<Live_V_InsuranceEmployeeBill>().Where(exp);
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

        

        public async Task<int> Add(Model.Live_InsuranceEmployeeBill model)
        {
            var db = base.Context;
            return await db.Insertable(model).ExecuteCommandAsync();
        }

        public async Task<int> Adds(List<Model.Live_InsuranceEmployeeBill> models)
        {
            var db = base.Context;
            return await db.Insertable(models).ExecuteCommandAsync();
        }
        public async Task<bool> Update(Model.Live_InsuranceEmployeeBill model)
        {
            var db = base.Context;
            return await db.Updateable(model).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Delete(int id)
        {
            var db = base.Context;
            return await db.Deleteable<Model.Live_InsuranceEmployeeBill>().Where(t => t.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Exist(Expression<Func<Live_InsuranceEmployeeBill, bool>> exp)
        {
            return await base.Context.Queryable<Live_InsuranceEmployeeBill>().AnyAsync(exp);
        }
        public async Task<Model.Live_V_InsuranceEmployeeBill> Get(int id)
        {
            var db = base.Context;
            return await db.Queryable<Model.Live_V_InsuranceEmployeeBill>().Where(t => t.ID == id).FirstAsync();
        }
        public async Task<bool> ExistID(int id)
        {
            return await base.Context.Queryable<Live_InsuranceEmployeeBill>().AnyAsync(it => it.ID == id);
        }
        public async Task<bool> ExistCardID(string IdcardNo,DateTime date)
        {
            return await base.Context.Queryable<Live_InsuranceEmployeeBill>().AnyAsync(it => it.IdcardNo == IdcardNo && it.BillMonth == date);
        }
        public async Task<string> BatchEmployeeCreate(List<Model.Live_V_InsuranceEmployee> Employees, DateTime date)
        {
            string msg = "";
            var info = await authService.UserInfo<Model.SYS_CompanyUser>();
            List<Model.Live_InsuranceEmployeeBill> bills = new List<Live_InsuranceEmployeeBill>();
            var existModels = await base.Context.Queryable<Live_V_InsuranceEmployeeBill>().Where(t => t.BillMonth == date && t.CompanyID == Employees[0].CompanyID).ToListAsync();
            if (Employees.Count == 0)
            {
                msg = "该月份没有符合条件的员工";
            }
            else
            {
                foreach(var item in Employees)
                {
                    if (existModels.Where(t=>t.IdcardNo == item.IdcardNo).ToList().Count>0)
                    {
                        // 员工已存在
                        continue;
                    }
                    try
                    {
                        Model.Live_InsuranceEmployeeBill bill = new Live_InsuranceEmployeeBill();
                        bill.CreateTime = DateTime.Now;
                        if(info.UserType==2)
                        {
                            bill.PartnerID = info.PartnerID;
                        }
                        
                        bill.CompanyID = item.CompanyID;
                        bill.CompanyGID = item.CompanyGID;
                        bill.BillMonth = date;
                        bill.Name = item.Name;
                        bill.IdcardNo = item.IdcardNo;
                        bill.IdcardAddress = item.IdcardAddress;
                        bill.Sex = item.Sex;
                        bill.Nation = item.Nation;
                        bill.Phone = item.Phone;
                        bill.Education = item.Education;
                        bill.EmployeeCompanyId = item.EmployeeCompanyId;
                        bill.EmployeeCompanyGID = item.EmployeeCompanyGID;
                        bill.EmployeeCompanyName = item.CustomerName;
                        bill.EmployeeCompanNo = item.CustomerCompanyNo;
                        bill.EmployeeCompanyAddress = item.CustomerAddress;
                        bill.EmployeeCompanyPhone = item.CustomerPhone;
                        bill.EmployeeCompanyBank = item.CustomerBank;
                        bill.EmployeeCompanyBankNo = item.CustomerBankAccount;
                        bill.EmployeeCompanySocialFee = item.CustomerSocialFee.Value;
                        bill.EmployeeCompanySocialEnabled = item.CustomerSociaEnabled;

                        bill.ProcessStatus = item.ProcessStatus;
                        bill.ShemeType = item.ShemeType;
                        bill.StartDateGongJiJin = item.StartDateGongJiJin;
                        bill.EndDateGongJiJin = item.EndDateGongJiJin;
                        bill.StartDateSheBao = item.StartDateSheBao;
                        bill.EndDateSheBao = item.EndDateSheBao;
                        bill.Province = item.Province;
                        bill.ProvinceCode = item.ProvinceCode;
                        bill.City = item.City;
                        bill.CityCode = item.CityCode;
                        bill.YlSchemeId = item.YlSchemeId;
                        bill.Enable = true;
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
                        bill.CooperationUnitGID = item.CooperationUnitGID;
                        bill.CooperationUnitName = item.CooperationUnitName;
                        bill.CooperationUnitAddress = item.CooperationUnitAddress;
                        bill.CooperationUnitPhone = item.CooperationUnitPhone;
                        bill.CooperationUnitBankName = item.CooperationUnitBankName;
                        bill.CooperationUnitBankNo = item.CooperationUnitBankNo;
                        bill.CooperationUnitNo = item.CooperationUnitCompanyNo;
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
                        //bill.EmployeeAmountDaBingYiLiao = 0;
                        //bill.EmployeeAmountYiLiaoBuChong = 0;
                        //bill.UnitAmountDaBingYiLiao = 0;
                        //bill.UnitAmountYiLiaoBuChong = 0;
                        DateTime now = (DateTime)item.CreateTime;
                       
                        
                    
                        
                        //公积金计算
                        if(item.StartDateGongJiJin.HasValue)
                        {
                            DateTime startGongJiJin = Convert.ToDateTime(item.StartDateGongJiJin);
                            if (date >= (bill.YlRule != 3 ? startGongJiJin : startGongJiJin.AddMonths(-1)))
                            {
                                //系数----规则是当月增减次月的情况下如果起始月等于创建时间，则收费2次
                                int xishu = 1;
                                if (bill.YlRule == 3 && date == startGongJiJin && startGongJiJin.Month == now.Month && startGongJiJin.Year == now.Year)
                                {
                                    xishu = 2;//双倍规则，临时设为1
                                }
                                // 公积金 向上取整！！
                                bill.UnitAmountGongJiJin = Math.Ceiling(item.BaseAmountGongJiJin * item.UnitPercentGongJiJin / 100) * xishu;
                                bill.EmployeeAmounGongJiJin = Math.Ceiling(item.BaseAmountGongJiJin * item.EmployeePercentGongJiJin / 100) * xishu;
                                bill.ServiceBaseAmountGongJiJin = (item.GongJiJinServiceAmount != null ? (decimal)item.GongJiJinServiceAmount : 0) * xishu;


                            }
                        }
                        
                        if(item.StartDateSheBao.HasValue)
                        {
                            DateTime startSheBao = Convert.ToDateTime(item.StartDateSheBao);
                            //社保计算
                            if (date >= (bill.YlRule != 3 ? startSheBao : startSheBao.AddMonths(-1)))
                            {
                                int xishu = 1;
                                if (bill.YlRule == 3 && date == startSheBao && startSheBao.Month == now.Month && startSheBao.Year == now.Year)
                                {
                                    xishu = 2;//双倍规则，临时设为1
                                }
                                // 养老保险
                                bill.UnitAmountYangLao = decimal.Round(item.BaseAmountYangLao * item.UnitPercentYangLao / 100, 2, MidpointRounding.AwayFromZero) * xishu;
                                bill.EmployeeAmountYangLao = decimal.Round(item.BaseAmountYangLao * item.EmployeePercentYangLao / 100, 2, MidpointRounding.AwayFromZero) * xishu;
                                // 医疗
                                bill.UnitAmountYiLiao = decimal.Round(item.BaseAmountYiLiao * item.UnitPercentYiLiao / 100, 2, MidpointRounding.AwayFromZero) * xishu;
                                bill.EmployeeAmountYiLiao = decimal.Round(item.BaseAmountYiLiao * item.EmployeePercentYiLiao / 100, 2, MidpointRounding.AwayFromZero) * xishu;

                                // 失业
                                bill.UnitAmountShiYe = decimal.Round(item.BaseAmountShiYe * item.UnitPercentShiYe / 100, 2, MidpointRounding.AwayFromZero) * xishu;
                                bill.EmployeeAmountShiYe = decimal.Round(item.BaseAmountShiYe * item.EmployeePercentShiYe / 100, 2, MidpointRounding.AwayFromZero) * xishu;


                                // 生育
                                bill.UnitAmountShengYu = decimal.Round(item.BaseAmountShengYu * item.UnitPercentShengYu / 100, 2, MidpointRounding.AwayFromZero) * xishu;
                                bill.EmployeeAmountShengYu = decimal.Round(item.BaseAmountShengYu * item.EmployeePercentShengYu / 100, 2, MidpointRounding.AwayFromZero) * xishu;
                                // 工伤
                                bill.UnitAmountGongShang = decimal.Round(item.BaseAmountGongShang * item.UnitPercentGongShang / 100, 2, MidpointRounding.AwayFromZero) * xishu;
                                bill.EmployeeAmountGongShang = decimal.Round(item.BaseAmountShengYu * item.EmployeePercentGongShang / 100, 2, MidpointRounding.AwayFromZero) * xishu;

                                // 保障金
                                bill.UnitAmountCountCanBao = decimal.Round(item.BaseAmountCanBao * item.UnitPercentCanBao / 100, 2, MidpointRounding.AwayFromZero) * xishu;
                                bill.EmployeeAmountCountCanBao = item.EmployeeAmountCanBao * xishu;
                                bill.ServiceBaseAmountSheBao = (item.SheBaoServiceAmount != null ? (decimal)item.SheBaoServiceAmount : 0) * xishu;
                            }
                        }
                        
                        //switch (bill.ShemeType)
                        //{
                        //    case 1:
                        //        break;
                        //    case 2:
                        //        bill.ServiceBaseAmountGongJiJin = 0;
                        //        bill.UnitAmountGongJiJin = 0;
                        //        bill.EmployeeAmounGongJiJin = 0;
                        //        break;
                        //    case 3:
                        //        bill.ServiceBaseAmountSheBao = 0;
                        //        bill.UnitAmountYangLao = 0;
                        //        bill.EmployeeAmountYangLao = 0;
                        //        bill.UnitAmountYiLiao = 0;
                        //        bill.EmployeeAmountYiLiao = 0;
                        //        bill.UnitAmountShiYe = 0;
                        //        bill.EmployeeAmountShiYe = 0;
                        //        bill.UnitAmountShengYu = 0;
                        //        bill.EmployeeAmountShengYu = 0;
                        //        bill.UnitAmountGongShang = 0;
                        //        bill.EmployeeAmountGongShang = 0;
                        //        bill.UnitAmountCountCanBao = 0;
                        //        bill.EmployeeAmountCountCanBao = 0;
                        //        bill.EmployeeAmountDaBingYiLiao = 0;
                        //        bill.EmployeeAmountYiLiaoBuChong = 0;
                        //        bill.UnitAmountDaBingYiLiao = 0;
                        //        bill.UnitAmountYiLiaoBuChong = 0;
                        //        break;
                        //    default:
                        //        break;
                        //}

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

                       

                       
                        if (item.YlSupplierIdGongJiJin==item.YlSupplierIdSheBao)
                        {
                            bill.ServiceBaseAmountGongJiJin = 0;
                        }
                        bill.ServiceFeeAmount = bill.ServiceBaseAmountGongJiJin + bill.ServiceBaseAmountSheBao;
                        bill.TotalAmount = bill.TotalAmountSheBao + bill.TotalAmountGongJiJin;
                        bills.Add(bill);
                    }
                    catch(Exception ex)
                    {
                        //msg += item.Name + "导入错误, "+ex.ToString()+"\n";
                        msg += item.Name + "导出错误, "  +ex.ToString();
                        continue;
                    }
                }
                await Adds(bills);
            }




            msg += "导入结束";
            return msg;
        }


        public byte[] Export(List<Model.Live_V_InsuranceEmployeeBill> list)
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
            var font1 = workbook.CreateFont();
            font1.FontHeightInPoints = 9;
            font1.FontName = "微软雅黑";
            int index = 1;
            foreach (var x in list)
            {
                List<object> arr = new();
                arr.Add(x.Province);
                arr.Add(x.City);
                arr.Add(x.TimeNode.HasValue ? x.TimeNode : 1);
                arr.Add(x.RuleName);
                arr.Add(x.StartDateSheBao.HasValue ? x.StartDateSheBao : "/");
                arr.Add(x.StartDateGongJiJin.HasValue ? x.StartDateGongJiJin : "/");
                arr.Add(x.BillMonth.HasValue ? x.BillMonth.Value : "/");
                if (x.ShemeType != 3)
                {
                    arr.Add(x.BillMonth.HasValue ? x.BillMonth.Value : "/");
                }
                else
                {
                    arr.Add("/");
                }
                if (x.ShemeType != 2)
                {
                    arr.Add(x.BillMonth.HasValue ? x.BillMonth.Value : "/");
                }
                else
                {
                    arr.Add("/");
                }

                arr.Add(x.Name);
                arr.Add(x.IdcardNo);
                arr.Add(x.EmployeeCompanyName);
                if (x.ShemeType != 3)
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
                if (x.ShemeType != 2)
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

                arr.Add(x.ServiceBaseAmountGongJiJin + x.ServiceBaseAmountSheBao);
                arr.Add(x.TotalAmount);




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





            byte[] buffer = new byte[1024 * 5];
            using (MemoryStream stream = new MemoryStream())
            {
                workbook.Write(stream);
                buffer = stream.ToArray();
            }
            return buffer;
        }




        public async Task<string> ExportExcel(List<Live_V_InjuryInfo> list)
        {
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("Sheet1");

            #region excel 表头
            string[] heads1 = { "序号", "序号", "序号", "地址", "测绘面积", "可补偿安置面积", "未经登记面积", "面积" };
            string[] heads2 = { "序号", "序号", "序号", "地址", "测绘", "可补偿", "未经", "面积" };
            //ExcelHeler.SetHeader(workbook, sheet, heads1, heads2);

            //for (int j = 0; j < heads2.Length; j++)
            //{
            //    headerRow2.CreateCell(j).SetCellValue($"{heads2[j]}");
            //}

            #endregion


            string filePath = System.IO.Directory.GetCurrentDirectory() + "/uploadfiles/excel/社保员工账单模板.xlsx";
            using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                workbook.Write(stream);
            }
            return filePath;
        }


        public string ExportToExcel(List<Model.Live_V_InsuranceEmployeeBill> list, string fileName, string filePath, int firstRow = 0, int columns = 10, int height = 800)
        {
            var path = System.IO.Directory.GetCurrentDirectory() + filePath;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string fullPth = path + fileName;
            try
            {
                FileStream fs = new FileStream(fullPth, FileMode.Open, FileAccess.Read);
                IWorkbook workbook = WorkbookFactory.Create(fs);
                ISheet sheet = workbook.GetSheetAt(0);
                IRow row = sheet.CreateRow(firstRow - 1);
                // row.CreateCell(firstRow);
                row.Height = 40 * 20;
                ICellStyle style = workbook.CreateCellStyle();
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                row.RowStyle = style;
                for (int j = 0; j < columns; j++)
                {
                    row.CreateCell(j);  //在行里面创建4个单元格
                    row.Cells[j].SetCellValue(j + "数据");
                }

                sheet.Append(row);
                using MemoryStream stream = new MemoryStream();
                workbook.Write(stream, true);
                string savePath = System.IO.Directory.GetCurrentDirectory() + $"/uploadfiles/file/{DateTime.Now.ToString("yyyyMMdd")}";
                if (!Directory.Exists(savePath))
                {
                    Directory.CreateDirectory(savePath);
                }
                string newFileName = savePath + '/' + fileName;
                using (FileStream fileStream = new FileStream(newFileName, FileMode.Create, FileAccess.Write))
                {
                    workbook.Write(fileStream);
                }



                return newFileName;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

        }
        
    }
}
