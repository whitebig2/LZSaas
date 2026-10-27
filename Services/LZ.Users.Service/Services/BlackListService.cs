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
using NPOI.SS.UserModel;
using LZ.Common;
using NPOI.XSSF.UserModel;
using LZ.Model.Enums.Budget;
using LZ.Core;

namespace LZ.Users.Service.Services
{
    public class BlackListService : ServicesBase<Live_BlackList>, IBlackListService, IScopedDependency
    {
        public async Task<List<Model.Live_BlackList>> List(Expression<Func<Model.Live_BlackList, bool>> exp, UserPagedModel query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_BlackList>().Where(exp).OrderByDescending(it => it.ID).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_BlackList>().Where(exp).OrderByDescending(it => it.ID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<List<Model.Live_V_BlackList>> ListView(Expression<Func<Model.Live_V_BlackList, bool>> exp, UserPagedModel query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_V_BlackList>().Where(exp).OrderByDescending(it=>it.ID).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_V_BlackList>().Where(exp).OrderByDescending(it => it.ID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<int> Add(Model.Live_BlackList model)
        {
            var db = base.Context;
            return await db.Insertable(model).ExecuteReturnIdentityAsync();
        }
        public async Task<bool> Update(Model.Live_BlackList model)
        {
            var db = base.Context;
            return await db.Updateable(model).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Delete(int id)
        {
            var db = base.Context;
            return await db.Deleteable<Model.Live_BlackList>().Where(t => t.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Exist(Expression<Func<Live_BlackList, bool>> exp)
        {
            return await base.Context.Queryable<Live_BlackList>().AnyAsync(exp);
        }
        public async Task<Model.Live_BlackList> Get(int id)
        {
            var db = base.Context;
            return await db.Queryable<Model.Live_BlackList>().Where(t => t.ID == id).FirstAsync();
        }
        public async Task<bool> ExistID(int id)
        {
            return await base.Context.Queryable<Live_BlackList>().AnyAsync(it => it.ID == id);
        }

        public byte[] ExportExcel(List<Model.Live_V_BlackList> list)
        {

            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("黑名单");
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
                "服务商",
                "身份证",
                "标签",
                "添加时间"
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
                arr.Add(x.PartnerName);
                arr.Add(x.IdentityCard);
                arr.Add(x.LabelNames);
                arr.Add(x.CreateTime.HasValue?x.CreateTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : "");
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
