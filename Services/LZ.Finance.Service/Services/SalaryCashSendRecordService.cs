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
using LZ.Model.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LZ.Finance.Service.Services
{
    public class SalaryCashSendRecordService : ServicesBase<Live_SalaryCashSendRecord>, ISalaryCashSendRecordService, IScopedDependency
    {

        public async Task<List<Live_V_SalaryCashSendRecord>> PageList(Expression<Func<Live_V_SalaryCashSendRecord, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var list = await   base.Context.Queryable<Live_V_SalaryCashSendRecord>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

       

        public async Task<List<Live_SalaryCashSendRecord>> List(Expression<Func<Live_SalaryCashSendRecord, bool>> exp, string strOrderByFileds = null)
        {
            return await   base.Context.Queryable<Live_SalaryCashSendRecord>()
                .OrderByIF(!string.IsNullOrWhiteSpace(strOrderByFileds), strOrderByFileds)
                .Where(exp).ToListAsync();
        }

        public async Task<List<Live_V_SalaryCashSendRecord>> ListView(Expression<Func<Live_V_SalaryCashSendRecord, bool>> exp, string strOrderByFileds = null)
        {
            return await   base.Context.Queryable<Live_V_SalaryCashSendRecord>()
                .OrderByIF(!string.IsNullOrWhiteSpace(strOrderByFileds), strOrderByFileds)
                .Where(exp).ToListAsync();
        }
        public async Task<Live_SalaryCashSendRecord> Get(Expression<Func<Live_SalaryCashSendRecord, bool>> exp, string strOrderByFileds = null)
        {
            return await   base.Context.Queryable<Live_SalaryCashSendRecord>()
                .OrderByIF(!string.IsNullOrWhiteSpace(strOrderByFileds), strOrderByFileds)
                .Where(exp).FirstAsync();
        }

        public async Task<Live_V_SalaryCashSendRecord> GetView(Expression<Func<Live_V_SalaryCashSendRecord, bool>> exp, string strOrderByFileds = null)
        {
            return await   base.Context.Queryable<Live_V_SalaryCashSendRecord>()
                .OrderByIF(!string.IsNullOrWhiteSpace(strOrderByFileds), strOrderByFileds)
                .Where(exp).FirstAsync();
        }
        public async Task<bool> Add(Live_SalaryCashSendRecord model)
        {
            return await   base.Context.Insertable<Live_SalaryCashSendRecord>(model).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Update(Live_SalaryCashSendRecord model)
        {
            var success = await   base.Context.Updateable<Live_SalaryCashSendRecord>(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
            return success;
        }

        public async Task<bool> UpdateCashSignStatus(Live_V_SalaryCashSendRecord model)
        {
            var success = await base.Context.Updateable<Model.Live_SalaryCashSendRecord>().SetColumns(it => new Model.Live_SalaryCashSendRecord() { Status = model.Status, Msg = model.Msg, SignTime = model.SignTime, SucFilePath = model.SucFilePath }).Where(it => it.ID == model.ID).ExecuteCommandAsync() > 0;
            return success;
        }

        public async Task<bool> UpdateTotalAmount(List<Live_SalaryCashSendRecord> models)
        {
            var success = await   base.Context.Updateable<Live_SalaryCashSendRecord>(models).UpdateColumns(f=>new { f.TotalAmount}).ExecuteCommandAsync() > 0;
            return success;

        }

        public async Task<bool> Delete(Expression<Func<Live_SalaryCashSendRecord, bool>> exp)
        {
            return await   base.Context.Deleteable<Live_SalaryCashSendRecord>(exp).ExecuteCommandAsync() > 0;
        }








    }
}
