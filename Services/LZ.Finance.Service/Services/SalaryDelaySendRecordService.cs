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

namespace LZ.Finance.Service.Services
{
    public class SalaryDelaySendRecordService : ServicesBase<Live_SalaryDelaySendRecord>, ISalaryDelaySendRecordService, IScopedDependency
    {

        public async Task<List<Live_V_SalaryDelaySendRecord>> PageList(Expression<Func<Live_V_SalaryDelaySendRecord, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var list = await   base.Context.Queryable<Live_V_SalaryDelaySendRecord>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

       

        public async Task<List<Live_SalaryDelaySendRecord>> List(Expression<Func<Live_SalaryDelaySendRecord, bool>> exp, string strOrderByFileds = null)
        {
            return await   base.Context.Queryable<Live_SalaryDelaySendRecord>()
                .OrderByIF(!string.IsNullOrWhiteSpace(strOrderByFileds), strOrderByFileds)
                .Where(exp).ToListAsync();
        }

        public async Task<List<Live_V_SalaryDelaySendRecord>> ListView(Expression<Func<Live_V_SalaryDelaySendRecord, bool>> exp, string strOrderByFileds = null)
        {
            return await   base.Context.Queryable<Live_V_SalaryDelaySendRecord>()
                .OrderByIF(!string.IsNullOrWhiteSpace(strOrderByFileds), strOrderByFileds)
                .Where(exp).ToListAsync();
        }
        public async Task<Live_SalaryDelaySendRecord> Get(Expression<Func<Live_SalaryDelaySendRecord, bool>> exp, string strOrderByFileds = null)
        {
            return await   base.Context.Queryable<Live_SalaryDelaySendRecord>()
                .OrderByIF(!string.IsNullOrWhiteSpace(strOrderByFileds), strOrderByFileds)
                .Where(exp).FirstAsync();
        }

        public async Task<Live_V_SalaryDelaySendRecord> GetView(Expression<Func<Live_V_SalaryDelaySendRecord, bool>> exp, string strOrderByFileds = null)
        {
            return await   base.Context.Queryable<Live_V_SalaryDelaySendRecord>()
                .OrderByIF(!string.IsNullOrWhiteSpace(strOrderByFileds), strOrderByFileds)
                .Where(exp).FirstAsync();
        }

        public async Task<bool> UpdateRecord(Live_SalaryDelaySendRecord model)
        {
            var isSuc = await   base.Context.Updateable<Live_SalaryDelaySendRecord>(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
            return isSuc;
        }



        public async Task<bool> Update(Live_SalaryDelaySendRecord model)
        {
            var isSuc = await   base.Context.Updateable<Live_SalaryDelaySendRecord>(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
            return isSuc;
        }

        public async Task<bool> Update(List<Live_SalaryDelaySendRecord> models)
        {
            var isSuc = await   base.Context.Updateable(models).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
            return isSuc;
        }


        //public async Task<bool> Update(Live_SalaryDelaySendRecord model)
        //{
        //      base.Context.BeginTran();

        //    if (model.Status == Model.Enums.SalaryDelaySendStatusEnumType.已处理)
        //    {
        //        var salaryRecord = await   base.Context.Queryable<Live_SalaryRecord>().FirstAsync(f => f.ID == model.SalaryRecordID);
        //        if (salaryRecord != null)
        //        {
        //            if (salaryRecord.UserEntryID > 0 && salaryRecord.DelayType == Model.Enums.SalaryDelaySendTypeEnumType.银行卡暂缓)
        //            {
        //                var userEntry = await   base.Context.Queryable<Live_UserEntry>().FirstAsync(f => f.ID == salaryRecord.UserEntryID);
        //                if (userEntry != null && !string.IsNullOrWhiteSpace(userEntry.BankName) && !string.IsNullOrWhiteSpace(userEntry.BankNumber))
        //                {
        //                    salaryRecord.Bank = userEntry.BankName;
        //                    salaryRecord.BankUserName = userEntry.Name;
        //                    salaryRecord.BankNo = userEntry.BankNumber;
        //                }
        //            }
        //            salaryRecord.IsDelayPaymentSuc = true;
        //            if (salaryRecord.IsCashSend)
        //            {
        //                // 现金已签署
        //                if (salaryRecord.IsCashSignSuc)
        //                {
        //                    salaryRecord.Status = Model.Enums.SalaryRecordSendStatusEnumType.待发放;
        //                }
        //                else
        //                {

        //                    salaryRecord.Status = Model.Enums.SalaryRecordSendStatusEnumType.待处理;
        //                }
        //            }
        //            else
        //            {
        //                salaryRecord.Status = Model.Enums.SalaryRecordSendStatusEnumType.待发放;
        //            }

        //            await   base.Context.Updateable<Live_SalaryRecord>(salaryRecord).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();

        //        }
        //    }

        //    var isSuc = await   base.Context.Updateable<Live_SalaryDelaySendRecord>(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;

        //      base.Context.CommitTran();

        //    return isSuc;
        //}

        public async Task<bool> UpdateTotalAmount(List<Live_SalaryDelaySendRecord> models)
        {
            return await   base.Context.Updateable<Live_SalaryDelaySendRecord>(models).UpdateColumns(f => new { f.TotalAmount }).ExecuteCommandAsync() > 0;
        }


        public async Task<bool> Add(Live_SalaryDelaySendRecord model)
        {
            return await   base.Context.Insertable<Live_SalaryDelaySendRecord>(model).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Delete(Expression<Func<Live_SalaryDelaySendRecord, bool>> exp)
        {
            return await   base.Context.Deleteable<Live_SalaryDelaySendRecord>(exp).ExecuteCommandAsync() > 0;
        }








    }
}
