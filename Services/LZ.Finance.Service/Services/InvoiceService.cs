using LZ.Finance.Service.Interface;
using LZ.Model;
using LZ.Model.PagedModel;
using SqlSugar.IOC;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LZ.Core.Interface;
using LZ.Core;
using Microsoft.AspNetCore.Authorization;
using NPOI.SS.Formula.Functions;
using LZ.Common;
using LZ.Model.Enums.Budget;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Org.BouncyCastle.Utilities;
using LZ.Model.Enums;
using Senparc.CO2NET.Extensions;
using Quartz.Util;

namespace LZ.Finance.Service.Services
{

    public class InvoiceService : ServicesBase<Live_Invoice>, IInvoiceService, IScopedDependency
    {

        public async Task<List<Live_V_Invoice>> PageViewList(Expression<Func<Live_V_Invoice, bool>> exp, PagedModelBase query)
        {

            if (string.IsNullOrWhiteSpace(query.Order))
            {
                query.Order = "desc";
            }


            if (string.IsNullOrWhiteSpace(query.OrderField))
            {
                query.OrderField = "CreateTime";
            }

            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await   base.Context.Queryable<Live_V_Invoice>()
                    .Where(exp)
                    .OrderByIF(!string.IsNullOrWhiteSpace(query.OrderField),$"{query.OrderField} {query.Order} ")
                    .ToListAsync();
            }
            var list = await   base.Context.Queryable<Live_V_Invoice>().Where(exp).OrderByIF(!string.IsNullOrWhiteSpace(query.OrderField), $"{query.OrderField} {query.Order} ").ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<List<Live_Invoice>> PageList(Expression<Func<Live_Invoice, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
         
            var list = await   base.Context.Queryable<Live_Invoice>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<List<Live_Invoice>> List(Expression<Func<Live_Invoice, bool>> exp)
        {
            var count = new RefAsync<int>();
            var list = await   base.Context.Queryable<Live_Invoice>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            return list;
        }
        public async Task<int> Add(Live_Invoice model)
        {
            return await   base.Context.Insertable(model).ExecuteCommandAsync();
        }

        public async Task<int> Add(List<Live_Invoice> models)
        {
            return await   base.Context.Insertable(models).ExecuteCommandAsync();
        }


        public async Task<int> Add(List<Live_InvoiceExtension> models)
        {
            return await   base.Context.Insertable(models).ExecuteCommandAsync();
        }

        public async Task<bool> Exist(Guid id)
        {
            return await   base.Context.Queryable<Live_Invoice>().AnyAsync(it => it.ID == id);
        }
        public async Task<bool> Update(Live_Invoice model)
        {
            return await   base.Context.Updateable<Live_Invoice>(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        //public async Task<bool> ChangeStatus(Guid id, int status)
        //{
        //    return await   base.Context.Updateable<Live_InvoiceConfig>().SetColumns(it => it.Status == status).Where(it => it.Id == id).ExecuteCommandAsync() > 0;
        //}

        public async Task<Live_Invoice> Get(Guid id)
        {
            return await   base.Context.Queryable<Live_Invoice>().FirstAsync(it => it.ID == id);
        }
        public async Task<Live_V_Invoice> GetView(Expression<Func<Live_V_InvoiceExtension, bool>> exp)
        {
            return await   base.Context.Queryable<Live_V_InvoiceExtension>()
                .Includes(f=>f.AttachFiles)
                .Includes(f=>f.TaxRecords)
                .FirstAsync(exp);
        }
        public async Task<bool> Delete(Guid id)
        {
            return await   base.Context.Deleteable<Live_Invoice>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Delete(Expression<Func<Live_Invoice, bool>> exp)
        {
            return await   base.Context.Deleteable<Live_Invoice>().Where(exp).ExecuteCommandAsync() > 0;
        }


        public async Task<bool> AddAttachFile(Live_InvoiceAttachFile model)
        {
            return await   base.Context.Insertable(model).ExecuteCommandAsync() > 0;
        }

        public async Task<Live_InvoiceAttachFile> GetAttachFile(Expression<Func<Live_InvoiceAttachFile, bool>> exp)
        {
            return await   base.Context.Queryable<Live_InvoiceAttachFile>().Where(exp).FirstAsync();
        }
        public async Task<List<Live_InvoiceAttachFile>> ListAttachFiles(Expression<Func<Live_InvoiceAttachFile, bool>> exp)
        {
            return await   base.Context.Queryable<Live_InvoiceAttachFile>().Where(exp).ToListAsync();
        }

        public async Task<bool> UpdateAttachFiles(List<Live_InvoiceAttachFile> model)
        {
            return await   base.Context.Updateable(model).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> AddInvoiceTaxRecords(List<Live_InvoiceTaxRecord> records)
        {
            return await   base.Context.Insertable(records).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> DeleteAttachFile(Expression<Func<Live_InvoiceAttachFile, bool>> exp)
        {
            return await   base.Context.Deleteable<Live_InvoiceAttachFile>(exp).ExecuteCommandAsync() > 0;
        }
        public async Task<Live_InvoiceExtension> GetNav(Expression<Func<Live_InvoiceExtension, bool>> exp)
        {
            return await   base.Context.Queryable<Live_InvoiceExtension>()
                .Includes(f => f.AttachFiles)
                .Includes(f => f.TaxRecords)
                .FirstAsync(exp);
        }

        public async Task<bool> AddNav(Live_InvoiceExtension model)
        {
            return await   base.Context.InsertNav(model)
                .Include(f => f.AttachFiles)
                  .Include(f => f.TaxRecords)
                .ExecuteCommandAsync();
        }

        public async Task<bool> UpdateNav(Live_InvoiceExtension model)
        {
            return await   base.Context.UpdateNav(model)
                //.Include(f => f.AttachFiles)
                 .Include(f => f.TaxRecords)
                .ExecuteCommandAsync();
        }

        public async Task<bool> DeleteNav(Expression<Func<Live_InvoiceExtension, bool>> exp)
        {
            return await   base.Context.DeleteNav(exp)
                .Include(f => f.AttachFiles)
                  .Include(f => f.TaxRecords)
                .ExecuteCommandAsync();
        }
        public async Task<int> DictionarySet(Dictionary<string, object> dt)
        {
            return await   base.Context.Updateable<Model.Live_Invoice>(dt).With(SqlWith.UpdLock).ExecuteCommandAsync();
        }

        public async Task<byte[]> ExportExcel(List<Live_V_Invoice> list)
        {
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("发票信息");
            var font = workbook.CreateFont();
            font.FontHeightInPoints = 9;
            font.FontName = "微软雅黑";
            font.IsBold = true;
            double width = 14 * 256;//默认宽度
            short height = 20 * 20;//默认高度

            sheet.DefaultRowHeight = height;
            #region excel 表头
            string[] heads = {
                "收票公司名称",
                "客户公司名称",
                "开票公司名称",
                "发票类别",
                "发票类型",
                "开票金额",
                "实收服务费",
                "人数",
                "开票名称",
                "实际包含内容",
                "纳税人识别号",
                "状态",
                "申请人",
                "锁定时间",
                "开具时间",
                "创建时间"
            };
            double width1 = 15 * 256;
            double width2 = 25 * 256;
            double width3 = 35 * 256;
            //for (int x = 0; x < heads.Length -1; x++)
            //{

            //    sheet.SetColumnWidth(x, width);
            //}
            sheet.SetColumnWidth(2, width2);//收票公司名称
            sheet.SetColumnWidth(0, width2);//客户公司名称
            sheet.SetColumnWidth(1, width2);//开票公司名称
            sheet.SetColumnWidth(3, width1);//发票类别
            sheet.SetColumnWidth(4, width1);//发票类型
            sheet.SetColumnWidth(5, width1);//开票金额
            sheet.SetColumnWidth(6, width1);//实收服务费
            sheet.SetColumnWidth(7, width1);//人数
            sheet.SetColumnWidth(8, width2);//开票名称
            sheet.SetColumnWidth(9, width3);//实际包含内容
            sheet.SetColumnWidth(10, width2);//纳税人识别号
            sheet.SetColumnWidth(11, width1);//状态
            sheet.SetColumnWidth(12, width1);//申请人
            sheet.SetColumnWidth(13, width1);//锁定时间
            sheet.SetColumnWidth(14, width1);//开具时间
            sheet.SetColumnWidth(15, width1);//创建时间

            ExcelHeler.SetHeaderSingleNoSetWidth(workbook, sheet, heads, font, width, height);

            #endregion

            string[] InvoiceTypeEnumType = {
                "增值税专用发票",
                "增值税普通发票",
                "值税电子普通发票",
                "增值税电子专用发票"
            };
            var font1 = workbook.CreateFont();
            font1.FontHeightInPoints = 9;
            font1.FontName = "微软雅黑";
            int index = 0;

            foreach (var x in list)
            {
           
                List<object> arr = new();
                int col = 0;
                arr.Add(x.RecipientName);
                arr.Add(x.CustomerName);
                arr.Add(x.IssuerName);
                arr.Add((PrintTemplateEnumType)x.BusinessType);
                arr.Add(InvoiceTypeEnumType[x.InvoiceType]);
                arr.Add(x.TotalAmount);
                arr.Add(x.ServiceFeeAmount);
                arr.Add(x.RecordCount);
                arr.Add(x.Title);
                arr.Add(x.Description);
                arr.Add(x.RecipientCompanyNo);
                arr.Add((InvoiceStatusEnumType)(int)x.Status);
                arr.Add(x.ApplyUserFullName);
                arr.Add(x.LockedTime?.ToString("yyyy-MM-dd HH:mm:ss"));
                arr.Add(x.IssuedTime?.ToString("yyyy-MM-dd HH:mm:ss"));
                arr.Add(x.CreateTime?.ToString("yyyy-MM-dd HH:mm:ss"));

                sheet.CreateRow(index + 1);
                foreach (var value in arr)
                {
                    ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, col, value);
                    col++;
                }
                index++;
            }

            




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
