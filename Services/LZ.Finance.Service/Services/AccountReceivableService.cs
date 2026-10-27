using LZ.Core.Interface;
using LZ.Core;
using LZ.Finance.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LZ.Model.PagedModel;
using LZ.Model;
using SqlSugar.IOC;
using SqlSugar;
using System.Linq.Expressions;
using NPOI.SS.Formula.Functions;
using Microsoft.AspNetCore.Mvc;
using LZ.Common;
using LZ.Model.Enums;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace LZ.Finance.Service.Services
{
    public class AccountReceivableService : ServicesBase<Live_AccountReceivable>, IAccountReceivableService, IScopedDependency
    {

        public async Task<List<Live_AccountReceivableExtension>> PageList(Expression<Func<Live_AccountReceivableExtension, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var list = await base.Context.Queryable<Live_AccountReceivableExtension>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<List<Live_AccountReceivableExtension>> List(Expression<Func<Live_AccountReceivableExtension, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var list = await  base.Context.Queryable<Live_AccountReceivableExtension>().Where(exp).OrderByDescending(it => it.CreateTime).Includes(f => f.Records).ToListAsync();
            query.Count = count.Value;
            return list;
        }



        public async Task<bool> Add(Live_AccountReceivableExtension model)
        {
            return await  base.Context
                  .Insertable<Live_AccountReceivableExtension>(model)
                  .ExecuteCommandAsync()>0;
        }

        public async Task<bool> Add(List<Live_AccountReceivableExtension> models)
        {
            return await  base.Context
                  .Insertable<Live_AccountReceivableExtension>(models)
                  .ExecuteCommandAsync() > 0;
        }

        public async Task<bool> AddNav(Live_AccountReceivableExtension model)
        {
            return await  base.Context
                  .InsertNav<Live_AccountReceivableExtension>(model)
                  .Include(f => f.Records)
                  .ExecuteCommandAsync();
        }

        public async Task<bool> AddNav(List<Live_AccountReceivableExtension> models)
        {
            return await  base.Context
                  .InsertNav<Live_AccountReceivableExtension>(models)
                  .Include(f => f.Records)
                  .ExecuteCommandAsync();
        }

        public async Task<Live_AccountReceivableExtension> Get(Expression<Func<Live_AccountReceivableExtension, bool>> exp)
        {
            return await  base.Context
                  .Queryable<Live_AccountReceivableExtension>()
                  .Where(exp)
                  .FirstAsync();
        }
        public async Task<Live_AccountReceivableExtension> GetNav(Expression<Func<Live_AccountReceivableExtension, bool>> exp)
        {
            return await  base.Context
                  .Queryable<Live_AccountReceivableExtension>()
                  .Where(exp)
                  .Includes(f => f.Records)
                  .FirstAsync();
        }
        public async Task<bool> UpdateNav(Live_AccountReceivableExtension model)
        {
            return await  base.Context .UpdateNav<Live_AccountReceivableExtension>(model).Include(f => f.Records, 
                new SqlSugar.UpdateNavOptions()
            {
                OneToManyInsertOrUpdate = true,//配置启用 插入、更新或删除模式
            }).ExecuteCommandAsync();
        }

   


        public async Task<bool> DeleteNav(Live_AccountReceivableExtension model)
        {
            return await  base.Context.DeleteNav<Live_AccountReceivableExtension>(model).Include(f => f.Records).ExecuteCommandAsync();
        }

        public byte[] ExportExcel(List<Live_AccountReceivableExtension> list)
        {
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("应收账款");
            var font = workbook.CreateFont();
            font.FontHeightInPoints = 9;
            font.FontName = "微软雅黑";
            font.IsBold = true;
            double width = 14 * 256;//默认宽度
            short height = 20 * 20;//默认高度

            sheet.DefaultRowHeight = height;
            #region excel 表头
            string[] heads = {
                "服务商",
                "客户单位",
                "付款单位（收票方）",
                "收款单位（开票方）",
                "应收账款金额",
                "本次收款金额",
                "待收款金额",
                "累计已收款金额",
                "代扣代缴",
                "状态",
                "创建时间",
                "收款时间",
                "操作时间",
                "操作人",
                "备注"
            };
            double width1 = 15 * 256;
            double width2 = 25 * 256;
            double width3 = 35 * 256;
            //for (int x = 0; x < heads.Length -1; x++)
            //{

            //    sheet.SetColumnWidth(x, width);
            //}
            sheet.SetColumnWidth(0, width2);//服务商
            sheet.SetColumnWidth(1, width3);//客户单位
            sheet.SetColumnWidth(2, width3);//付款单位（收票方）
            sheet.SetColumnWidth(3, width3);//收款单位（开票方）
            sheet.SetColumnWidth(4, width1);//应收账款金额
            sheet.SetColumnWidth(5, width1);//本次收款金额
            sheet.SetColumnWidth(6, width1);//待收款金额
            sheet.SetColumnWidth(7, width1);//累计已收款金额
            sheet.SetColumnWidth(8, width1);//代扣代缴
            sheet.SetColumnWidth(9, width1);//状态
            sheet.SetColumnWidth(10, width2);//创建时间
            sheet.SetColumnWidth(11, width2);//收款时间
            sheet.SetColumnWidth(12, width2);//操作时间
            sheet.SetColumnWidth(13, width1);//申请人
            sheet.SetColumnWidth(14, width3);//备注
            

            ExcelHeler.SetHeaderSingleNoSetWidth(workbook, sheet, heads, font, width, height);
            #endregion
            var font1 = workbook.CreateFont();
            font1.FontHeightInPoints = 9;
            font1.FontName = "微软雅黑";
            int index = 0;

            foreach (var x in list)
            {
                var records = x.Records.Where(t => t.AccountReceivableID == x.ID).OrderBy(t=>t.CreateTime).ToList();
                decimal lastAmount = 0;

                if (records.Count == 0)
                {
                    List<object> arr = new();
                    int col = 0;
                    arr.Add(x.PartnerName);
                    arr.Add(x.CustomerName);
                    arr.Add(x.RecipientName);
                    arr.Add(x.IssuerName);
                    arr.Add(x.TotalAmount);
                    arr.Add(lastAmount);
                    arr.Add(x.TotalAmount - lastAmount);
                    arr.Add(lastAmount);
                    arr.Add(x.SfWithholdAmount);
                    if (lastAmount != 0 && lastAmount < x.TotalAmount)
                    {
                        arr.Add((AccountReceivableStatusEnumType)AccountReceivableStatusEnumType.收款中);
                    }
                    else
                    {
                        arr.Add((AccountReceivableStatusEnumType)x.Status);
                    }

                    arr.Add(x.CreateTime.HasValue ? x.CreateTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : "/");
                    arr.Add("/");
                    arr.Add("/");
                    arr.Add("/");
                    arr.Add("/");
                    sheet.CreateRow(index + 1);
                    foreach (var value in arr)
                    {
                        ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, col, value);
                        col++;
                    }
                    index++;
                }
                else
                {
                    foreach (var record in records)
                    {
                        List<object> arr = new();
                        int col = 0;

                        lastAmount += record.PaidAmount;
                        arr.Add(x.PartnerName);
                        arr.Add(x.CustomerName);
                        arr.Add(x.RecipientName);
                        arr.Add(x.IssuerName);
                        arr.Add(x.TotalAmount);
                        arr.Add(record.PaidAmount);
                        arr.Add(x.TotalAmount - lastAmount);
                        arr.Add(lastAmount);
                        arr.Add(x.SfWithholdAmount);
                        if (lastAmount != 0 && lastAmount < x.TotalAmount)
                        {
                            arr.Add((AccountReceivableStatusEnumType)AccountReceivableStatusEnumType.收款中);
                        }
                        else
                        {
                            arr.Add((AccountReceivableStatusEnumType)x.Status);
                        }

                        arr.Add(x.CreateTime.HasValue ? x.CreateTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : "/");
                        arr.Add(record.PaidTime.HasValue ? record.PaidTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : "/");
                        arr.Add(record.CreateTime.HasValue ? record.CreateTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : "/");
                        
                        arr.Add(record.ActionUserName);
                        arr.Add(record.Remark);
                        sheet.CreateRow(index + 1);
                        foreach (var value in arr)
                        {
                            ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, col, value);
                            col++;
                        }
                        index++;

                    }
                }

                    

               






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
