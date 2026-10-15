using LZ.Bank.Service.Interface;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Core.Services;
using LZ.Model;
using LZ.Model.Dtos;
using LZ.Model.Dtos.BankDo;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.Abstractions;
using Newtonsoft.Json;
using NPOI.HSSF.Record;
using SqlSugar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Bank.Service.Services
{
    public class BankTransactionService : ServicesBase<Live_BankTransaction>, IBankTransactionService, IScopedDependency
    {

        private readonly IHttpService _httpService;

        public BankTransactionService(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<List<Live_BankTransaction>> PageList(Expression<Func<Live_BankTransaction, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_BankTransaction>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_BankTransaction>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<List<Live_BankTransactionRecord>> PageRecordList(Expression<Func<Live_BankTransactionRecord, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_BankTransactionRecord>().Where(exp).OrderByDescending(it => it.ReceiveAccountName).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_BankTransactionRecord>().Where(exp).OrderByDescending(it => it.ReceiveAccountName).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<List<Live_BankTransaction>> List(Expression<Func<Live_BankTransaction, bool>> exp)
        {
            return await base.Context.Queryable<Live_BankTransaction>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }

        public async Task<Live_BankTransaction> Get(Expression<Func<Live_BankTransaction, bool>> exp)
        {
            return await base.Context.Queryable<Live_BankTransaction>()
                .Where(exp).OrderByDescending(it => it.CreateTime).FirstAsync();
        }



        public async Task<Live_BankTransactionExtension> GetNav(Guid id)
        {
            return await base.Context.Queryable<Live_BankTransactionExtension>()
                .Includes(f => f.Records)
                .Where(f => f.ID == id).OrderByDescending(it => it.CreateTime).FirstAsync();
        }

        public async Task<Live_BankTransactionExtension> GetNav(Expression<Func<Live_BankTransactionExtension, bool>> exp)
        {
            return await base.Context.Queryable<Live_BankTransactionExtension>()
                 .Includes(f => f.Records)
                .Where(exp).OrderByDescending(it => it.CreateTime).FirstAsync();
        }

        public async Task<bool> AddNav(Live_BankTransactionExtension model)
        {
            model.TotalCount = model.Records.Count;
            model.TotalAmount = model.Records.Sum(f => f.PrivateAmount);

            return await base.Context.InsertNav<Live_BankTransactionExtension>(model)
                .Include(f => f.Records).ExecuteCommandAsync();
        }

        public async Task<bool> AddNav(List<Live_BankTransactionExtension> models)
        {
            return await base.Context.InsertNav<Live_BankTransactionExtension>(models)
                .Include(f => f.Records).ExecuteCommandAsync();
        }
        public async Task<bool> Update(Live_BankTransaction model)
        {
            return await base.Context.Updateable<Live_BankTransaction>(model).ExecuteCommandAsync()>0;
        }

        public async Task<bool> UpdateNav(Live_BankTransactionExtension model)
        {
            return await base.Context.UpdateNav<Live_BankTransactionExtension>(model)
                .Include(f => f.Records, new SqlSugar.UpdateNavOptions()
                {
                    OneToManyInsertOrUpdate = true,//配置启用 插入、更新或删除模式
                }).ExecuteCommandAsync();
        }

        public async Task<bool> ResetNav(Live_BankTransactionExtension model)
        {
            model.Status = Model.Enums.BankTransactionStatusEnumType.待锁定;
            model.TransMsg = string.Empty;
            model.SucCount = 0;
            model.FailCount = 0;
            model.TransTime = null;

            model.IsSendNotice = false;
            model.SendNoticeCount = 0;
            model.NoticeTime = null;

            foreach (var item in model.Records)
            {
                item.RecordTransStatus = TransactionStatusEnumType.待发放;
                item.RecordTransMsg = string.Empty;
                item.RecordTransTime = null;
            }
            return await UpdateNav(model);
        }


        public async Task<bool> CancelNav(Live_BankTransactionExtension model)
        {
            model.Status = Model.Enums.BankTransactionStatusEnumType.作废;

            foreach (var item in model.Records)
            {
                item.RecordTransStatus = TransactionStatusEnumType.作废;
            }
            return await UpdateNav(model);
        }

        public async Task<bool> NoticeResult(Live_BankTransactionExtension model)
        {

            List<BankTransactionStatusEnumType> status = new List<BankTransactionStatusEnumType>() {
            BankTransactionStatusEnumType.发放成功,
            BankTransactionStatusEnumType.发放失败,
            BankTransactionStatusEnumType.作废
            };

            if (!status.Contains(model.Status))
            {
                return false;
            }

            BankTransactionResultNoticeDto noticeModel = new BankTransactionResultNoticeDto()
            {
                BusinessID = model.BusinessID,
                TransactionID = model.TransactionID,
                TransMsg = model.TransMsg,
                IsTransSuc = model.Status == BankTransactionStatusEnumType.发放成功,
                TransTime = model.TransTime,
                Records = model.Records.Select(f => new BankTransactionResultNoticeItemDto()
                {
                    CardID = f.CardID,
                    Name = f.ReceiveAccountName,
                    RecordBusinessID = f.RecordBusinessID,
                    RecordTransactionID = f.RecordTransactionID,
                    RecordTransAmount = f.PrivateAmount,
                    RecordTransMsg = f.RecordTransMsg,
                    RecordTransTime = f.RecordTransTime,
                    RecordTransStatus = f.RecordTransStatus,
                    IsRecordTransSuc = f.RecordTransStatus == TransactionStatusEnumType.已发放
                }).ToList()
            };

            switch (model.Status)
            {
                case BankTransactionStatusEnumType.发放成功:
                    noticeModel.TransStatus = TransactionStatusEnumType.已发放;
                    break;
                case BankTransactionStatusEnumType.发放失败:
                    noticeModel.TransStatus = TransactionStatusEnumType.发放失败;
                    break;
                case BankTransactionStatusEnumType.作废:
                    noticeModel.TransStatus = TransactionStatusEnumType.作废;
                    break;
                default:
                    noticeModel.TransStatus = TransactionStatusEnumType.待发放;
                    break;
            }

            string json = JsonConvert.SerializeObject(noticeModel);
            var res = await _httpService.PostAsync(ServiceNames.FinanceService, "/api/SalaryTransaction/TransactionResultNotice", json);

            if (res != null)
            {
                // 通知完成
                if (res.Code == 200)
                {
                    model.IsSendNotice = true;
                    model.SendNoticeCount++;
                    model.NoticeTime = DateTime.Now;
                    await Update(model);
                    return true;
                }
                else
                {
                    model.IsSendNotice = false;
                    model.SendNoticeCount++;
                    model.NoticeTime = DateTime.Now;
                    await Update(model);
                    return false;
                }
            }
            else
            {
                model.IsSendNotice = false;
                model.SendNoticeCount++;
                model.NoticeTime = DateTime.Now;
                await UpdateNav(model);
                return false;
            }
        }

    }
}
