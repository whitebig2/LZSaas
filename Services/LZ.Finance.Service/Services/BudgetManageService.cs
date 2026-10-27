using LZ.Model.PagedModel;
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
using LZ.Finance.Service.Interface;
using Consul;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using LZ.Common;
using LZ.Model.Enums.Budget;
using LZ.Core;

namespace LZ.Finance.Service.Services
{
    public class BudgetManageService : ServicesBase<Live_BudgetManage>, IBudgetManageService, IScopedDependency
    {
        public async Task<List<Live_V_BudgetManage>> List(Expression<Func<Live_V_BudgetManage, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                if (!string.IsNullOrEmpty(query.Order))
                {
                    if (query.Order == "partner")
                    {
						return await base.Context.Queryable<Live_V_BudgetManage>().Where(exp).OrderByDescending(it => it.PartnerGID).ToListAsync();
					}
                }
                return await base.Context.Queryable<Live_V_BudgetManage>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_V_BudgetManage>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<List<Live_BudgetManage>> ListModel(Expression<Func<Live_BudgetManage, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                if (!string.IsNullOrEmpty(query.Order))
                {
                    if (query.Order == "partner")
                    {
                        return await base.Context.Queryable<Live_BudgetManage>().Where(exp).OrderByDescending(it => it.PartnerGID).ToListAsync();
                    }
                }
                return await base.Context.Queryable<Live_BudgetManage>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_BudgetManage>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<int> Add(Live_BudgetManage model)
        {
            return await base.Context.Insertable(model).ExecuteCommandAsync();
        }
        public async Task<int> Add(List<Live_BudgetManage> list)
        {
            return await base.Context.Insertable(list).ExecuteCommandAsync();
        }
        public async Task<bool> Exist(int id)
        {
            return await base.Context.Queryable<Live_BudgetManage>().AnyAsync(it => it.ID == id);
        }

        public async Task<bool> Exist(Expression<Func<Live_BudgetManage, bool>> exp)
        {
            return await base.Context.Queryable<Live_BudgetManage>().AnyAsync(exp);
        }

        public async Task<bool> Update(Live_BudgetManage model)
        {
            return await base.Context.Updateable<Live_BudgetManage>(model).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Update(List<Live_BudgetManage> list)
        {
            return await base.Context.Updateable<Live_BudgetManage>(list).ExecuteCommandAsync() > 0;
        }

        public async Task<Live_BudgetManage> Get(int id)
        {
            return await base.Context.Queryable<Live_BudgetManage>().FirstAsync(it => it.ID == id);
        }
        public async Task<Live_V_BudgetManage> GetView(int id)
        {
            return await base.Context.Queryable<Live_V_BudgetManage>().FirstAsync(it => it.ID == id);
        }
        public async Task<Live_BudgetManage> Get(Expression<Func<Live_BudgetManage, bool>> exp)
        {
            return await base.Context.Queryable<Live_BudgetManage>().Where(exp).FirstAsync();
        }

        public async Task<Live_V_BudgetManage> GetV(Expression<Func<Live_V_BudgetManage, bool>> exp)
        {
            return await base.Context.Queryable<Live_V_BudgetManage>().Where(exp).FirstAsync();
        }

        public async Task<bool> UpdateMoney(Live_V_BudgetManage model)
        {

            var db = base.Context;
            var ub = db.Updateable<Live_BudgetManage>();
            ub.SetColumns(it => it.TotalAmount == model.TotalAmount);
            ub.SetColumns(it => it.UsedAmount == model.UsedAmount);
            ub.SetColumns(it => it.LockAmount == model.LockAmount);
            ub.SetColumns(it => it.AvailableAmount == model.AvailableAmount);
            ub.Where(t => t.ID == model.ID);
            return await ub.ExecuteCommandAsync() > 0;
        }
        public async Task<bool> UpdateHidden(int id, bool isHidden)
        {

            var db = base.Context;
            var ub = db.Updateable<Live_BudgetManage>();
            ub.SetColumns(it => it.Hidden == isHidden);
            ub.Where(t => t.ID == id);
            return await ub.ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Delete(int id)
        {
            return await base.Context.Deleteable<Live_BudgetManage>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
        public async Task<int> Delete(Expression<Func<Live_BudgetManage, bool>> exp)
        {
            return await base.Context.Deleteable<Live_BudgetManage>().Where(exp).ExecuteCommandAsync();
        }

        public async Task<int> DictionarySet(Dictionary<string, object> dt)
        {
            return await base.Context.Updateable<Model.Live_BudgetManage>(dt).With(SqlWith.UpdLock).ExecuteCommandAsync();
        }

        public byte[] ExportExcel(List<Live_V_BudgetManage> list, List<Live_BudgetRechargeLog> logs)
        {

			IWorkbook workbook = new XSSFWorkbook();
			ISheet sheet = workbook.CreateSheet("总预算管理");
			var font = workbook.CreateFont();
			font.FontHeightInPoints = 9;
			font.FontName = "微软雅黑";
			font.IsBold = true;
			double width = 14 * 256;//默认宽度
			short height = 20 * 20;//默认高度

			sheet.DefaultRowHeight = height;
			#region excel 表头
			string[] heads = {
				"年度",
                "服务商",
				"费用名称",
				"成本类型",
				"生成方式",
				"提取百分比",
				"直接通过",
                "总部申请金额",
                "总金额",
				"已使用金额",
                "可用金额"
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
                arr.Add(x.Year ?? 0);
                arr.Add(x.PartnerName);
                arr.Add(x.ExpensesTitle);
                arr.Add((CostType)x.CostType);
                arr.Add((GenerationType)x.GenerateType);
                arr.Add(x.Percentage);
                arr.Add(x.DirectlyPass?"是":"否");
                //arr.Add(x.HeadOfficeUsedAmount);
                arr.Add(x.TotalAmount);
                arr.Add(x.UsedAmount+x.LockAmount);
                arr.Add(x.LockAmount);
                arr.Add(x.AvailableAmount);

                sheet.CreateRow(index + 1);//index=1,从第三行开始,前2行是表头
                foreach (var value in arr)
                {
                    ExcelHeler.SetCellValue(workbook, sheet, font1, index + 1, col, value);
                    col++;
                }
                index++;
            }

            ISheet sheet1 = workbook.CreateSheet("总预算操作记录");

            string[] heads1 = {
                "年度",
                "事业合伙人",
                "费用名称",
                "成本类型",
                "生成方式",
                "提取百分比",
                "直接通过",
                "总部申请金额",
                "总金额",
                "已申请金额",
                "可用金额",
                "充值提现/划转金额",
                "操作金额",
                "操作方式",
                "申请人",
                "申请时间",
                "备注"
            };
            ExcelHeler.SetHeaderSingle(workbook, sheet1, heads1, font, width, height);

            int row = 0;
            foreach (var d in logs)
            {
                List<object> arr = new();
                var x = list.Find(t => t.ID == d.CID);
                int col = 0;
                if (x == null) continue;
                arr.Add(x.Year ?? 0);
                arr.Add(x.PartnerName);
                arr.Add(x.ExpensesTitle);
                arr.Add(!x.CostType.HasValue?"/":((CostType)x.CostType).ToString());
                arr.Add(!x.GenerateType.HasValue?"/":(GenerationType)x.GenerateType);
                arr.Add(x.Percentage);
                arr.Add(x.DirectlyPass?"是":"否");
                arr.Add(d.HeadOfficeUsedAmount);


                if(!d.Type.HasValue)
                {
                    continue;
                }
                if(d.Type== (int)BudgetManageRecordActionEnum.服务费充值 || d.Type == (int)BudgetManageRecordActionEnum.管理员充值)
                {
                    arr.Add(d.TotalAmount);
                    arr.Add(d.LockAmount + d.UsedAmount);
                    arr.Add(d.AvailableAmount);
                    arr.Add(d.AddAmount);
                    arr.Add(d.AddAmount);
                    arr.Add((BudgetManageRecordActionEnum)d.Type);
                }
                else if (d.Type == (int)BudgetManageRecordActionEnum.管理员提现)
                {
                    arr.Add(d.TotalAmount);
                    arr.Add(d.LockAmount + d.UsedAmount);
                    arr.Add(d.AvailableAmount);
                    arr.Add(-d.Money);
                    arr.Add(-d.Money);
                    arr.Add((BudgetManageRecordActionEnum)d.Type);
                }
                else if (d.Type == (int)BudgetManageRecordActionEnum.划拨划入)
                {
                    arr.Add(d.TotalAmount);
                    arr.Add(d.LockAmount + d.UsedAmount);
                    arr.Add(d.AvailableAmount);
                    arr.Add(d.Money);
                    arr.Add(d.Money);
                    arr.Add((BudgetManageRecordActionEnum)d.Type+"-"+d.OtherPartnerName);
                }
                else if(d.Type== (int)BudgetManageRecordActionEnum.划拨划出)
                {
                    arr.Add(d.TotalAmount);
                    arr.Add(d.LockAmount + d.UsedAmount);
                    arr.Add(d.AvailableAmount);
                    arr.Add(-d.Money);
                    arr.Add(-d.Money);
                    arr.Add((BudgetManageRecordActionEnum)d.Type + "-" + d.OtherPartnerName);
                }
                else if (d.Type == (int)BudgetManageRecordActionEnum.客服申请扣款)
                {
                    arr.Add(d.TotalAmount);
                    arr.Add(d.LockAmount + d.UsedAmount);
                    arr.Add(d.AvailableAmount);
                    arr.Add(0);
                    arr.Add(d.Money);
                    arr.Add("申请扣款");
                }
                else if (d.Type == (int)BudgetManageRecordActionEnum.作废)
                {
                    arr.Add(d.TotalAmount);
                    arr.Add(d.LockAmount + d.UsedAmount);
                    arr.Add(d.AvailableAmount);
                    arr.Add(0);
                    arr.Add(-d.Money);
                    arr.Add("打款前作废");
                }
                else if (d.Type == (int)BudgetManageRecordActionEnum.回滚作废)
                {
                    arr.Add(d.TotalAmount);
                    arr.Add(d.LockAmount + d.UsedAmount);
                    arr.Add(d.AvailableAmount);
                    arr.Add(0);
                    arr.Add(-d.Money);
                    arr.Add((BudgetManageRecordActionEnum)d.Type);
                }
                else
                {
                    continue;
                }



                arr.Add(d.UserName);
                arr.Add(d.CreateTime.Value.ToString("yyyy-MM-dd"));
                arr.Add(d.Content);

                sheet1.CreateRow(row + 1);
                foreach (var value in arr)
                {
                    ExcelHeler.SetCellValue(workbook, sheet1, font1, row + 1, col, value);
                    col++;
                }
                row++;
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
