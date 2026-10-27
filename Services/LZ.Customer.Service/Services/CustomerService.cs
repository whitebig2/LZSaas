using LZ.Customer.Service.Interface;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Model;
using SqlSugar;
using SqlSugar.IOC;
using System.Linq.Expressions;
using LZ.Model.PagedModel;
using LZ.Model.ModelExtensions;
using LZ.Model.Exceptions;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Core.EventData;
using LZ.Model.Enums.Sale;
using System.Security.Policy;
using NPOI.SS.Formula.Functions;
using NetTaste;
using System;
using LZ.Common;
using LZ.Model.Enums.Insurance;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;


namespace LZ.Customer.Service.Services
{
    public class CustomerService(IEventPublisher publisher) : ServicesBase<Live_Customer>, ICustomerService, IScopedDependency
    {
        public async Task<List<Live_V_Customer>> List(Expression<Func<Live_V_Customer, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_V_Customer>().Where(exp).OrderByDescending(it => it.CustomerID).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_V_Customer>().Where(exp).OrderByDescending(it => it.CustomerID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<List<Live_Customer>> List(Expression<Func<Live_Customer, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_Customer>().Where(exp).OrderByDescending(it => it.CustomerID).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_Customer>().Where(exp).OrderByDescending(it => it.CustomerID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }


        public async Task<int> Add(Live_Customer model)
        {
            return await base.Context.Insertable(model).ExecuteReturnIdentityAsync();
        }
        public async Task<bool> Exist(int id)
        {
            return await base.Context.Queryable<Live_Customer>().AnyAsync(it => it.CustomerID == id);
        }
        public async Task<bool> Exist(Expression<Func<Live_Customer, bool>> exp)
        {
            return await base.Context.Queryable<Live_Customer>().AnyAsync(exp);
        }
        public async Task<bool> Update(Live_Customer model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> UpdateList(List<Live_Customer> list)
        {
            return await base.Context.Updateable(list).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> ChangeStatus(int id, int status)
        {
            return await base.Context.Updateable<Live_Customer>().SetColumns(it => it.Status == status).Where(it => it.CustomerID == id).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> ChangeWork(int id, string workName, string workClass)
        {
            return await base.Context.Updateable<Live_Customer>().SetColumns(it => new Model.Live_Customer() { EntryWorkClass = workClass, EntryWorkName = workName }).Where(it => it.CustomerID == id).ExecuteCommandAsync() > 0;
        }
        public async Task<Live_Customer> Get(int id)
        {
            return await base.Context.Queryable<Live_Customer>().FirstAsync(it => it.CustomerID == id);
        }
        public async Task<Live_Customer> Get(string name)
        {
            return await base.Context.Queryable<Live_Customer>().FirstAsync(it => it.CustomerName.Contains(name));
        }

        public async Task<Live_Customer> Get(Expression<Func<Live_Customer, bool>> exp)
        {
            return await base.Context.Queryable<Live_Customer>().Where(exp).OrderByDescending(it => it.CustomerID).FirstAsync();
        }

        public async Task<Live_V_Customer> GetView(Expression<Func<Live_V_Customer, bool>> exp)
        {
            return await base.Context.Queryable<Live_V_Customer>().Where(exp).OrderByDescending(it => it.CustomerID).FirstAsync();
        }
        public async Task<bool> Delete(int id)
        {
            var entry = await base.Context.Queryable<Model.Live_UserEntry>().AnyAsync(it => it.CustomerID == id);
            var per = await base.Context.Queryable<Model.Live_PersonnelItems>().AnyAsync(it => it.CID == id);
            var par = await base.Context.Queryable<Model.Live_PartnerCustomer>().AnyAsync(it => it.CID == id);
            if (entry || per || par)
            {
                throw new ErrorCodeException(-1, "该客户已被关联无法删除！");
            }
            return await base.Context.Deleteable<Live_Customer>().Where(it => it.CustomerID == id).ExecuteCommandAsync() > 0;
        }


        public async Task<Live_CustomerExtension> GetNav(Guid CustomerGID)
        {
            return await base.Context.Queryable<Live_CustomerExtension>()
                .Includes(f => f.ContractConfigs)
                .Where(f => f.CustomerGID == CustomerGID).FirstAsync();
        }

        public async Task<bool> UpdateNav(Live_CustomerExtension model)
        {
            return await base.Context.UpdateNav<Live_CustomerExtension>(model, new UpdateNavRootOptions() { IsDisableUpdateRoot = true })
                .Include(f => f.ContractConfigs, new SqlSugar.UpdateNavOptions()
                {
                    OneToManyInsertOrUpdate = true,//配置启用 插入、更新或删除模式
                }).ExecuteCommandAsync();
        }
        public async Task<Model.Model.CustomerAuthModel> GetCustomerAuth(Expression<Func<Model.Model.CustomerAuthModel, bool>> exp)
        {
            return await base.Context.Queryable<Model.Model.CustomerAuthModel>().Where(exp).OrderByDescending(it => it.CustomerID).FirstAsync();
        }

        public async Task<bool> UpdateCustomerAuth(Model.Model.CustomerAuthModel model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<int> DictionarySet(Dictionary<string, object> dt)
        {
            return await base.Context.Updateable<Model.Live_Customer>(dt).With(SqlWith.UpdLock).ExecuteCommandAsync();
        }

        public byte[] Export(List<Model.Live_V_Customer> list)
        {

            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("客户信息");
            var font = workbook.CreateFont();
            font.FontHeightInPoints = 9;
            font.FontName = "微软雅黑";
            font.IsBold = true;
            double width = 35 * 256;//默认宽度
            short height = 20 * 20;//默认高度

            sheet.DefaultRowHeight = height;
            #region excel 表头
            string[] heads = {
                "识别符",
                "客户名称",
                "简介"
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
                arr.Add(x.CustomerID);
                arr.Add(x.CustomerName);
                arr.Add(x.About);
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
