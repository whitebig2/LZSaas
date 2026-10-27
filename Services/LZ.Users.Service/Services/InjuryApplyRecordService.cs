using LZ.Core.Interface;
using LZ.Model;
using LZ.Users.Service.Interface;
using SqlSugar.IOC;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NPOI.SS.Util;
using LZ.Common;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel; // 对于 .xlsx 文件
using NPOI.HSSF.UserModel;
using LZ.Core; // 对于 .xls 文件

namespace LZ.Users.Service.Services
{
    public class InjuryApplyRecordService : ServicesBase<Live_InjuryApplyRecord>, IInjuryApplyRecordService, IScopedDependency
    {
        public async Task<List<Model.Live_V_InjuryApplyRecord>> List(Expression<Func<Model.Live_V_InjuryApplyRecord, bool>> exp, Model.PagedModel.UserPagedModel query, string order = "")
        {
            var count = new RefAsync<int>();
            var sco = base.Context.Queryable<Live_V_InjuryApplyRecord>().Where(exp);
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

        public async Task<int> Add(Model.Live_InjuryApplyRecord model)
        {
            var db = base.Context;
            return await db.Insertable(model).ExecuteReturnIdentityAsync();
        }
        public async Task<bool> Update(Model.Live_InjuryApplyRecord model)
        {
            var db = base.Context;
            return await db.Updateable(model).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Delete(int id)
        {
            var db = base.Context;
            return await db.Deleteable<Model.Live_InjuryApplyRecord>().Where(t => t.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Delete(Expression<Func<Model.Live_InjuryApplyRecord, bool>> exp)
        {
            var db = base.Context;
            return await db.Deleteable<Model.Live_InjuryApplyRecord>().Where(exp).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Exist(Expression<Func<Live_InjuryApplyRecord, bool>> exp)
        {
            return await base.Context.Queryable<Live_InjuryApplyRecord>().AnyAsync(exp);
        }

        public async Task<Model.Live_V_InjuryApplyRecord> LoanAdd(Model.Live_InjuryApplyRecord model)
        {
            var db = base.Context;
            int i = await db.Insertable(model).ExecuteReturnIdentityAsync();
            return await View(i,0,"");
        }
        public async Task<Model.Live_InjuryApplyRecord> Get(int id)
        {
            var db = base.Context;
            return await db.Queryable<Model.Live_InjuryApplyRecord>().Where(t => t.ID == id).FirstAsync();
        }
        public async Task<Model.Live_InjuryApplyRecord> Get(Expression<Func<Live_InjuryApplyRecord, bool>> exp)
        {
            var db = base.Context;
            return await db.Queryable<Model.Live_InjuryApplyRecord>().Where(exp).FirstAsync();
        }
        public async Task<bool> UpdateStatus(int id, int status, string msg = "")
        {

            var db = base.Context;
            var ub = db.Updateable<Live_InjuryApplyRecord>();
            if (status > 0)
            {
                ub.SetColumns(it => it.Status == status);
            }
            if (!string.IsNullOrEmpty(msg))
            {
                ub.SetColumns(it => it.Remark == msg);
            }
            ub.Where(t => t.ID == id);
            return await ub.ExecuteCommandAsync() > 0;
        }
        public async Task<bool> ExistID(int id)
        {
            return await base.Context.Queryable<Live_InjuryApplyRecord>().AnyAsync(it => it.ID == id);
        }
        public async Task<Live_V_InjuryApplyRecord> View(int id,int injuryID, string statusArray, int moneyType = 0, int companyId = 0)
        {
            var expable = Expressionable.Create<Model.Live_V_InjuryApplyRecord>();
            if (id > 0)
            {
                expable.And(it => it.ID == id);
            }
            if (injuryID > 0)
            {
                expable.And(it => it.InjuryID == injuryID);
            }
            if (companyId > 0)
            {
                expable.And(it => it.CompanyID == companyId);
            }
            if (moneyType > 0)
            {
                expable.And(it => it.MoneyType == moneyType);
            }
            if (!string.IsNullOrEmpty(statusArray))
            {
                var ids = statusArray.Split(',');
                expable.And(t => ids.Contains(t.Status.ToString()));
            }

            var exp = expable.ToExpression();
            return await View(exp);
        }
        public async Task<Live_V_InjuryApplyRecord> View(Expression<Func<Live_V_InjuryApplyRecord, bool>> exp)
        {
            return await base.Context.Queryable<Live_V_InjuryApplyRecord>().FirstAsync(exp);
        }

         public async Task<int> DictionarySet(Dictionary<string, object> dt)
        {
            return await base.Context.Updateable<Model.Live_InjuryApplyRecord>(dt).With(SqlWith.UpdLock).ExecuteCommandAsync();
        }

        public async Task<byte[]> ExportExcel(List<Live_V_InjuryApplyRecord> list)
        {

            var expable = Expressionable.Create<Model.SYS_Dictionary>();

            string query = "a4ee9d24-9d41-441d-bcb9-955c0a5c9d4a,561f491b-292a-49d8-baf3-3e8cf922b711,0f192c81-7439-44cb-a9ee-98ecd2f7173d,0a1ac30d-e803-49dd-bd73-fbe64c9c2344";
            var ids = query.Split(',');
            expable.And(it => it.PID.HasValue && ids.Contains(it.PID.ToString()));
           var dics = await base.Context.Queryable<SYS_Dictionary>().Where(expable.ToExpression()).OrderBy(it => it.Sort).ToListAsync();
            var cyclelist = dics.Where(t => t.PID == Guid.Parse("a4ee9d24-9d41-441d-bcb9-955c0a5c9d4a")).ToList();
            var moneyTypelist = dics.Where(t => t.PID == Guid.Parse("0a1ac30d-e803-49dd-bd73-fbe64c9c2344")).ToList();
            var loanStatuslist = dics.Where(t => t.PID == Guid.Parse("561f491b-292a-49d8-baf3-3e8cf922b711")).ToList();
            var repayStatuslist = dics.Where(t => t.PID == Guid.Parse("0f192c81-7439-44cb-a9ee-98ecd2f7173d")).ToList();

            IWorkbook workbook = new XSSFWorkbook();   
            ISheet sheet = workbook.CreateSheet("Sheet1");
            var font = workbook.CreateFont();
            font.FontHeightInPoints = 10;
            font.FontName = "微软雅黑";
            font.IsBold = true;
            double width = 14 * 256;
            short height = 17 * 20;
            int row = 1;//数据从第几行开始，第一行为0
            #region excel 表头
            string[] heads = { "签章单位", "客户单位","服务商", "姓名", "身份证号", "借款金额", "工伤类型", "工伤时间", "工伤状态", "申请类型", "发放状态", "签章单位所属行", "签章单位银行卡号", "代理人", "代理人身份证号", "代理人所属行", "代理人银行卡号", "创建时间" };
            //string[] heads2 = { "序号", "序号", "序号", "地址", "测绘", "可补偿", "未经", "面积" };
            ExcelHeler.SetHeaderSingle(workbook, sheet, heads, font,width,height);
            var dataFont = workbook.CreateFont();
            dataFont.FontHeightInPoints = 10;
            dataFont.FontName = "微软雅黑";
            foreach (var x in list)
            {
                sheet.CreateRow(row);
                sheet.DefaultRowHeight = height;
                var cycle = cyclelist.Find(t => t.NO == x.ProcessStatus.ToString());
                var moneyType = moneyTypelist.Find(t => t.NO == x.MoneyType.ToString());
                
                int index = 0;
                ExcelHeler.SetCellValue(workbook,sheet, dataFont, row, index, x.SealCompanyName);
                index++;
                ExcelHeler.SetCellValue(workbook, sheet, dataFont, row, index, x.CustomerName);
                index++;
                ExcelHeler.SetCellValue(workbook, sheet, dataFont, row, index, x.PartnerName);
                index++;
                ExcelHeler.SetCellValue(workbook, sheet, dataFont, row, index, x.UserName);
                index++;
                
                ExcelHeler.SetCellValue(workbook, sheet, dataFont, row, index, x.UserCardNo);
                index++;
                ExcelHeler.SetCellValue(workbook, sheet, dataFont, row, index, x.Money);
                index++;
                ExcelHeler.SetCellValue(workbook, sheet, dataFont, row, index, x.InjuryType);
                index++;
                ExcelHeler.SetCellValue(workbook, sheet, dataFont, row, index, x.InjuryTime.Value.ToString("yyyy-MM-dd"));
                index++;
                ExcelHeler.SetCellValue(workbook, sheet, dataFont, row, index, cycle==null?"": cycle.Name);
                index++;
                var statusName = "";
                if (x.MoneyType == 1)
                {
                    var s = loanStatuslist.Find(t => t.NO == x.Status.ToString());
                    if(s!=null)
                    {
                        statusName = s.Name;
                    }
                    
                } else if(x.MoneyType == 2)
                {
                    var s = repayStatuslist.Find(t => t.NO == x.Status.ToString());
                    if(s!=null)
                    {
                        statusName = s.Name;
                    }
                    
                }
                ExcelHeler.SetCellValue(workbook, sheet, dataFont, row, index, moneyType==null?"": moneyType.Name);
                index++;
                ExcelHeler.SetCellValue(workbook, sheet, dataFont, row, index, statusName);
                index++;
                ExcelHeler.SetCellValue(workbook, sheet, dataFont, row, index, x.SealCompanyBank);
                index++;
                ExcelHeler.SetCellValue(workbook, sheet, dataFont, row, index, x.SealCompanyBankNo);
                index++;
                ExcelHeler.SetCellValue(workbook, sheet, dataFont, row, index, x.AgentName);
                index++;
                ExcelHeler.SetCellValue(workbook, sheet, dataFont, row, index, x.AgentIdcardNo);
                index++;
                ExcelHeler.SetCellValue(workbook, sheet, dataFont, row, index, x.AgentBank);
                index++;
                ExcelHeler.SetCellValue(workbook, sheet, dataFont, row, index, x.AgentBankNo);
                index++;
                ExcelHeler.SetCellValue(workbook, sheet, dataFont, row, index, x.CreateTime.HasValue? x.CreateTime.Value.ToString("yyyy-MM-dd"):"/");
                row++;
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
