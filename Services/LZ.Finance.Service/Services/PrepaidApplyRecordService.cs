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
using LZ.Model.Exceptions;
using LZ.Model.Enums;
using Microsoft.AspNetCore.JsonPatch.Internal;

namespace LZ.Finance.Service.Services
{
    public class PrepaidApplyRecordService : ServicesBase<Live_PrepaidApplyRecord>, IPrepaidApplyRecordService, IScopedDependency
    {

        public async Task<List<Live_V_PrepaidApplyRecordExtension>> PageViewList(Expression<Func<Live_V_PrepaidApplyRecordExtension, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await   base.Context.Queryable<Live_V_PrepaidApplyRecordExtension>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            }
            var list = await   base.Context.Queryable<Live_V_PrepaidApplyRecordExtension>().Includes(f=>f.Records).Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }


        public async Task<List<Live_V_PrepaidApplyRecord>> List(Expression<Func<Live_V_PrepaidApplyRecord, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await   base.Context.Queryable<Live_V_PrepaidApplyRecord>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            }
            var list = await   base.Context.Queryable<Live_V_PrepaidApplyRecord>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }


        public async Task<List<Live_PrepaidApplyRecord>> List(Expression<Func<Live_PrepaidApplyRecord, bool>> exp)
        {
            return await   base.Context.Queryable<Live_PrepaidApplyRecord>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }



        public async Task<List<Live_V_PrepaidApplyRecord>> ListView(Expression<Func<Live_V_PrepaidApplyRecord, bool>> exp)
        {
            return await   base.Context.Queryable<Live_V_PrepaidApplyRecord>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }

        public async Task<List<Live_V_PrepaidRepaymentRecord>> ListRepaymentListView(Expression<Func<Live_V_PrepaidRepaymentRecord, bool>> exp)
        {
            return await   base.Context.Queryable<Live_V_PrepaidRepaymentRecord>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }

        public async Task<int> Add(Live_PrepaidApplyRecord model)
        {
            return await   base.Context.Insertable(model).ExecuteCommandAsync();
        }

        public async Task<int> Update(List<Live_PrepaidApplyRecord> models)
        {
            return await   base.Context.Updateable(models).ExecuteCommandAsync();
        }
        public async Task<bool> Exist(Guid id)
        {
            return await   base.Context.Queryable<Live_PrepaidApplyRecord>().AnyAsync(it => it.ID == id);
        }
        public async Task<bool> Exist(Expression<Func<Live_PrepaidApplyRecord, bool>> exp)
        {
            return await   base.Context.Queryable<Live_PrepaidApplyRecord>().AnyAsync(exp);
        }
        public async Task<decimal> SumMoney(Expression<Func<Live_PrepaidApplyRecord, bool>> exp)
        {
            return await   base.Context.Queryable<Live_PrepaidApplyRecord>().Where(exp).SumAsync(it=>it.ApplyAmount);
        }
        public async Task<bool> Update(Live_PrepaidApplyRecord model)
        {

            
            return await   base.Context.Updateable<Live_PrepaidApplyRecord>(model).ExecuteCommandAsync() > 0;
        }
        public async Task<Live_PrepaidApplyRecord> Get(Guid id)
        {
            return await   base.Context.Queryable<Live_PrepaidApplyRecord>().FirstAsync(it => it.ID == id);
        }
        public async Task<bool> ChangeStatus(Guid Id, PrepaidRecordStatusEnumType status)
        {
            return await   base.Context.Updateable<Live_PrepaidApplyRecord>().SetColumns(it => it.Status == status).Where(it=>it.ID==Id).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> ChangeApplyAmount(Guid Id, decimal money)
        {
            return await   base.Context.Updateable<Live_PrepaidApplyRecord>().SetColumns(it => it.ApplyAmount == money).Where(it => it.ID == Id && it.Status==PrepaidRecordStatusEnumType.待审核).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> ChangeApplyAmount(Guid Id, decimal money,decimal hour)
        {
            return await   base.Context.Updateable<Live_PrepaidApplyRecord>().SetColumns(it => new Live_PrepaidApplyRecord() { ApplyAmount = money, MoneyHours=hour }).Where(it => it.ID == Id && it.Status == PrepaidRecordStatusEnumType.待审核).ExecuteCommandAsync() > 0;
        }
        public async Task<Live_PrepaidApplyRecord> Get(Expression<Func<Live_PrepaidApplyRecord, bool>> exp)
        {
            return await   base.Context.Queryable<Live_PrepaidApplyRecord>().Where(exp).OrderByDescending(it => it.CreateTime).FirstAsync();
        }
        public async Task<Live_V_PrepaidApplyRecord> GetView(Expression<Func<Live_V_PrepaidApplyRecord, bool>> exp)
        {
            return await   base.Context.Queryable<Live_V_PrepaidApplyRecord>().Where(exp).OrderByDescending(it => it.CreateTime).FirstAsync();
        }
        public async Task<bool> Delete(Guid id)
        {
            return await   base.Context.Deleteable<Live_PrepaidApplyRecord>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> ApplyRecordAudit(Live_PrepaidApplyRecord record)
        {
            var db =  itenant;
            var result = await db.UseTranAsync(async () =>
            {
                var prepaidAccount = await base.Context.Queryable<Live_PrepaidPartnerAccountExtension>().FirstAsync(it => it.ID == record.PrepaidPartnerAccountID);
                await base.Context.Updateable<Live_PrepaidApplyRecord>().SetColumns(it =>new Live_PrepaidApplyRecord() { Status=record.Status,AuditTime=DateTime.Now}).Where(it=>it.ID==record.ID).ExecuteCommandAsync();
                await base.Context.Updateable<Live_PrepaidPartnerAccountExtension>(prepaidAccount).IsEnableUpdateVersionValidation().ExecuteCommandAsync();
                await base.Context.Updateable<Live_PrepaidApplyRecord>(prepaidAccount).ExecuteCommandAsync();
                return true;
            });
            return result.Data;
        }


        public async Task<List<Live_PrepaidApplyRecordExtension>> ListNav(Expression<Func<Live_PrepaidApplyRecordExtension, bool>> exp)
        {
            return await   base.Context.Queryable<Live_PrepaidApplyRecordExtension>()
                .Includes(f => f.RepaymentRecords)
                .Where(exp).OrderByDescending(it =>new { it.IdCardNo,it.CreateTime}).ToListAsync();
        }


        public async Task<Live_PrepaidApplyRecordExtension> GetNav(Guid id)
        {
            return await   base.Context.Queryable<Live_PrepaidApplyRecordExtension>()
                .Includes(f => f.RepaymentRecords)
                .Where(f => f.ID == id)
                .FirstAsync();
        }
        public async Task<bool> UpdateNav(Live_PrepaidApplyRecordExtension model)
        {
            return await   base.Context.UpdateNav<Live_PrepaidApplyRecordExtension>(model).Include(f => f.RepaymentRecords).ExecuteCommandAsync();
        }
        public async Task<bool> UpdateNav(List<Live_PrepaidApplyRecordExtension> models)
        {
            return await   base.Context.UpdateNav<Live_PrepaidApplyRecordExtension>(models).Include(f => f.RepaymentRecords).ExecuteCommandAsync();
        }
      
    }
}
