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
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Mvc;
using LZ.Common;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using LZ.Core;

namespace LZ.Users.Service.Services
{
    public class WhiteListService : ServicesBase<Model.Live_WhiteList>, IWhiteListService, IScopedDependency
    {
        public async Task<List<Model.Live_WhiteList>> List(Expression<Func<Model.Live_WhiteList, bool>> exp, UserPagedModel query, string order = "")
        {
            var count = new RefAsync<int>();
            var sco = base.Context.Queryable<Live_WhiteList>().Where(exp);
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

        public async Task<List<Model.Live_V_WhiteList>> ListView(Expression<Func<Model.Live_V_WhiteList, bool>> exp, UserPagedModel query, string order = "")
        {
            var count = new RefAsync<int>();
            var sco = base.Context.Queryable<Live_V_WhiteList>().Where(exp);
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
        public async Task<int> Count(Expression<Func<Model.Live_WhiteList, bool>> exp)
        {
            return await base.Context.Queryable<Live_WhiteList>().Where(exp).CountAsync();
        }
        public async Task<int> Add(Model.Live_WhiteList model)
        {
            return await base.Context.Insertable(model).ExecuteReturnIdentityAsync();
        }
        public async Task<bool> Update(Model.Live_WhiteList model)
        {
            return await base.Context.Updateable(model).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Update(List<Model.Live_WhiteList> list)
        {
            return await base.Context.Updateable(list).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Disabled(string cardID,bool disabled)
        {
            return await base.Context.Updateable<Model.Live_WhiteList>().SetColumns(t=>t.Enabled == disabled).Where(t=>t.IdentityCard == cardID).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Delete(int id)
        {
            return await base.Context.Deleteable<Model.Live_WhiteList>().Where(t => t.ID == id).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> DeleteList(List<Live_WhiteList> list)
        {
            return await base.Context.Deleteable<Model.Live_WhiteList>(list).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Exist(Expression<Func<Live_WhiteList, bool>> exp)
        {
            return await base.Context.Queryable<Live_WhiteList>().AnyAsync(exp);
        }
        public async Task<Model.Live_WhiteList> Get(int id)
        {
            return await base.Context.Queryable<Model.Live_WhiteList>().Where(t => t.ID == id).FirstAsync();
        }
        public async Task<Model.Live_WhiteList> Get(Expression<Func<Live_WhiteList, bool>> exp)
        {
            return await base.Context.Queryable<Model.Live_WhiteList>().Where(exp).FirstAsync();
        }
        public async Task<bool> ExistID(int id)
        {
            return await base.Context.Queryable<Live_WhiteList>().AnyAsync(it => it.ID == id);
        }

        public byte[] ExportExcel(List<Model.Live_WhiteList> list)
        {

            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("白名单");
            var font = workbook.CreateFont();
            font.FontHeightInPoints = 9;
            font.FontName = "微软雅黑";
            font.IsBold = true;
            double width = 35 * 256;//默认宽度
            short height = 20 * 20;//默认高度

            sheet.DefaultRowHeight = height;
            #region excel 表头
            string[] heads = {
                "姓名",
                "身份证",
                "标签"
            };
            ExcelHeler.SetHeaderSingle(workbook, sheet, heads, font, width, height);

            #endregion
            var font1 = workbook.CreateFont();
            font1.FontHeightInPoints = 9;
            font1.FontName = "微软雅黑";
            int index = 0;
            foreach (var x in list)
            {
                List<object> arr = new();
                int col = 0;
                arr.Add(x.Name);
                arr.Add(x.IdentityCard);
                arr.Add(x.LabelName);

                sheet.CreateRow(index + 1);//index=1,从第三行开始,前2行是表头
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
