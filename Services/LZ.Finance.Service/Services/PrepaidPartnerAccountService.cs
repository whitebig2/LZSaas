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

namespace LZ.Finance.Service.Services
{
    public class PrepaidPartnerAccountService : ServicesBase<Live_PrepaidPartnerAccount>,  IPrepaidPartnerAccountService, IScopedDependency
    {

        public async Task<List<Live_V_PrepaidPartnerAccount>> PageViewList(Expression<Func<Live_V_PrepaidPartnerAccount, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await   base.Context.Queryable<Live_V_PrepaidPartnerAccount>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            }
            var list = await   base.Context.Queryable<Live_V_PrepaidPartnerAccount>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }


        public async Task<List<Live_PrepaidPartnerAccount>> List(Expression<Func<Live_PrepaidPartnerAccount, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await   base.Context.Queryable<Live_PrepaidPartnerAccount>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            }
            var list = await   base.Context.Queryable<Live_PrepaidPartnerAccount>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }


        public async Task<List<Live_V_PrepaidPartnerAccount>> ListView(Expression<Func<Live_V_PrepaidPartnerAccount, bool>> exp)
        {
            return await   base.Context.Queryable<Live_V_PrepaidPartnerAccount>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }

        public async Task<int> Add(Live_PrepaidPartnerAccountExtension model)
        {
            return await   base.Context.Insertable(model).ExecuteCommandAsync();
        }
        public async Task<bool> Exist(Guid id)
        {
            return await   base.Context.Queryable<Live_PrepaidPartnerAccount>().AnyAsync(it => it.ID == id);
        }

        public async Task<bool> UpdateEntity(Live_PrepaidPartnerAccount model)
        {
            return await   base.Context.Updateable(model).IsEnableUpdateVersionValidation().ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Update(Live_PrepaidPartnerAccountExtension model)
        {
            return await   base.Context.Updateable<Live_PrepaidPartnerAccountExtension>(model).IsEnableUpdateVersionValidation().ExecuteCommandAsync() > 0;
        }
        public async Task<Live_PrepaidPartnerAccountExtension> Get(Guid id)
        {
            return await   base.Context.Queryable<Live_PrepaidPartnerAccountExtension>().FirstAsync(it => it.ID == id);
        }

        public async Task<Live_PrepaidPartnerAccount> GetEntity(Guid id)
        {
            return await   base.Context.Queryable<Live_PrepaidPartnerAccount>().FirstAsync(it => it.ID == id);
        }
        public async Task<Live_PrepaidPartnerAccountExtension> Get(Expression<Func<Live_PrepaidPartnerAccountExtension, bool>> exp)
        {
            return await   base.Context.Queryable<Live_PrepaidPartnerAccountExtension>().Where(exp).OrderByDescending(it => it.CreateTime).FirstAsync();
        }
        public async Task<bool> Delete(Guid id)
        {
            return await   base.Context.Deleteable<Live_PrepaidPartnerAccount>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }


        public async Task<bool> AddApplyRecord(Live_PrepaidApplyRecord record)
        {
            return await   base.Context.Insertable<Live_PrepaidApplyRecord>(record).ExecuteCommandAsync() > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public async Task<bool> ApplyRecordAudit(Guid recordID)
        {
            var record = await   base.Context.Queryable<Live_PrepaidApplyRecord>().FirstAsync(f => f.ID == recordID);
            record.Status = Model.Enums.PrepaidRecordStatusEnumType.待发放;
            var prepaidAccount = await   base.Context.Queryable<Live_PrepaidPartnerAccountExtension>().FirstAsync(it => it.ID == record.PrepaidPartnerAccountID);
            if (prepaidAccount.AvailableAmount > record.ApplyAmount)
            {
                prepaidAccount.SendSuccess(record.ApplyAmount);
            }
          itenant.BeginTran();
            await   base.Context.Updateable<Live_PrepaidPartnerAccountExtension>(prepaidAccount).IsEnableUpdateVersionValidation().ExecuteCommandAsync();
            await   base.Context.Updateable<Live_PrepaidApplyRecord>(prepaidAccount).ExecuteCommandAsync();
          itenant.CommitTran();
            return true;
        }

        public async Task<Live_PrepaidApplyRecord> GetApplyRecord(Guid recordID)
        {
            var record = await   base.Context.Queryable<Live_PrepaidApplyRecord>().FirstAsync(f => f.ID == recordID);
            return record;
        }
        public async Task<bool> UpdateApplyRecord(Live_PrepaidApplyRecord record)
        {
            return await   base.Context.Updateable<Live_PrepaidApplyRecord>(record).ExecuteCommandAsync()>0;
        }
        /// <summary>
        ///  现金充值金额
        /// </summary>
        /// <param name="recordID"></param>
        /// <returns></returns>
        public async Task<bool> CashRecharge(Guid recordID)
        {
            var record = await   base.Context.Queryable<Live_PrepaidApplyRecord>().FirstAsync(f => f.ID == recordID);
            var prepaidAccount = await   base.Context.Queryable<Live_PrepaidPartnerAccountExtension>().FirstAsync(it => it.ID == record.PrepaidPartnerAccountID);
            if (prepaidAccount.AvailableAmount > record.ApplyAmount)
            {
                // 减去可用金额
                prepaidAccount.AvailableAmount -= record.ApplyAmount;
                //累加已用金额
                prepaidAccount.UsedAmount += record.ApplyAmount;
            }
            return await   base.Context.Insertable<Live_PrepaidApplyRecord>(record).ExecuteCommandAsync() > 0;
        }


        public async Task<List<Live_V_PrepaidApplyRecord>> PageRecordViewList(Expression<Func<Live_V_PrepaidApplyRecord, bool>> exp, PagedModelBase query)
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





    }
}
