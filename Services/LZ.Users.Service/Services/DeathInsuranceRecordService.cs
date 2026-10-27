using LZ.Core.Interface;
using LZ.Core;
using LZ.Model.PagedModel;
using LZ.Model;
using LZ.Users.Service.Interface;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SqlSugar.Extensions;
using NPOI.HSSF.Record;
using LZ.ApiService.Interface;
using LZ.Model.IExcelTemplate;
using LZ.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace LZ.Users.Service.Services
{


    public class DeathInsuranceRecordService : ServicesBase<Live_DeathInsuranceRecord>, IDeathInsuranceRecordService, IScopedDependency
    {

        private readonly IWebHostEnvironment _environment;
        private readonly IEntryService _entryService;
        private readonly IMailSenderService _mailSenderService;
        public DeathInsuranceRecordService(IEntryService entryService, IMailSenderService mailSenderService, IWebHostEnvironment environment)
        {
            _entryService = entryService;
            _mailSenderService = mailSenderService;
            _environment = environment;
        }

        public async Task<List<Live_DeathInsuranceRecord>> PageList(Expression<Func<Live_DeathInsuranceRecord, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var list = await base.Context.Queryable<Live_DeathInsuranceRecord>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<List<Live_DeathInsuranceRecord>> List(Expression<Func<Live_DeathInsuranceRecord, bool>> exp)
        {
            return await base.Context.Queryable<Live_DeathInsuranceRecord>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }

        public async Task<Live_DeathInsuranceRecord> Get(Expression<Func<Live_DeathInsuranceRecord, bool>> exp)
        {
            return await base.Context.Queryable<Live_DeathInsuranceRecord>().Where(exp).OrderByDescending(it => it.CreateTime).FirstAsync();
        }
        public async Task<bool> Update(Live_DeathInsuranceRecord model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Add(Live_DeathInsuranceRecord model)
        {
            return await base.Context.Insertable(model).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Add(List<Live_DeathInsuranceRecord> models)
        {
            return await base.Context.Insertable(models).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Update(List<Live_DeathInsuranceRecord> models)
        {
            return await base.Context.Updateable(models).ExecuteCommandAsync() > 0;
        }

        public async Task Delete(Expression<Func<Live_DeathInsuranceRecord, bool>> exp)
        {
            await base.Context.Deleteable<Live_DeathInsuranceRecord>().Where(exp).ExecuteCommandAsync();
        }


        public async Task MatchAllRecord(DateTime insuranceStartDate,DateTime  insuranceEndDate)
        {
            var today=DateTime.Today;
            var nextDay= today.AddDays(1);



         await   Delete(f => f.Status == Model.Enums.BusInsuranceRecordStatusEnumType.待增员);
            // 所有已参保记录
            var records = await List(f =>  f.Status == Model.Enums.BusInsuranceRecordStatusEnumType.已增员 || f.Status == Model.Enums.BusInsuranceRecordStatusEnumType.待减员);
            // 所有需要参保的人员
            var entrys = await _entryService.ListInsurance(f => (f.InsuranceType != Model.Enums.InsuranceTypeEnumType.不参保 ||f.IsManuallyInsurance) && f.Status == 2 && f.IsUseDeathInsurance && !string.IsNullOrWhiteSpace(f.WorkName));
            List<int> recordIds = new List<int>();
            List<int> entryIds = new List<int>();
            Live_DeathInsuranceRecord record;
            foreach (var item in entrys)
            {
                record = records.FirstOrDefault(f => f.Name == item.Name && f.CardID.Equals(item.CardID, StringComparison.OrdinalIgnoreCase) && f.SignUnitName == item.SignUnitName);
                if (record != null)
                {
                    recordIds.Add(record.ID);
                    if (record.Status == Model.Enums.BusInsuranceRecordStatusEnumType.待减员)
                    {
                        record.Status = Model.Enums.BusInsuranceRecordStatusEnumType.已增员;
                        record.ChangeType = "批增";
                        record.EndDate = record.InsuranceStartDate;
                        await Update(record);
                    }
                }
                else
                {
                    entryIds.Add(item.ID);
                }
            }
            var delRecords = records.Where(f => !recordIds.Contains(f.ID)).ToList();
            foreach (var item in delRecords)
            {
                item.EndDate = today;
                item.Status= Model.Enums.BusInsuranceRecordStatusEnumType.待减员;
                item.ChangeType= "批减";
            }


            var addRecords = entrys.Where(f => entryIds.Contains(f.ID)).Select(f => new Live_DeathInsuranceRecord()
            {
                CreateTime = DateTime.Now,
                ChangeType = "批增",
                CardID = f.CardID,
                ApplyDate = today,
                CustomerName = f.CustomerName,
                ID = 0,
                StartDate = nextDay,
                EndDate = insuranceEndDate,
                InsuranceStartDate = insuranceStartDate,
                InsuranceEndDate = insuranceEndDate,
                InsuranceName = "医疗健康险",
                IsHasSocialInsurance = false,
                Name = f.Name,
                Passport = string.Empty,
                PlanName = string.Empty,
                Remark = string.Empty,
                SignUnitAddress = string.Empty,
                SignUnitName = f.SignUnitName,
                Status = Model.Enums.BusInsuranceRecordStatusEnumType.待增员,
                WorkName = f.WorkName,
            }).ToList();


            await Add(addRecords);
            await Update(delRecords);
        }


        public async Task PushAllRecord()
        {
            var today = DateTime.Today;
            var nextDay = today.AddDays(1);
            var records = await List(f => f.Status == Model.Enums.BusInsuranceRecordStatusEnumType.待减员 || f.Status == Model.Enums.BusInsuranceRecordStatusEnumType.待增员);
            if (records.Any())
            {
                var pushRecords = records.OrderBy(f=>f.Status).Select(f => new DeathInsuranceItemExcelTemp()
                {
                    CardID = f.CardID,
                    ChangeType = f.ChangeType,
                    CompanyAddress = string.Empty,
                    CompanyName = f.SignUnitName,
                    IsHaveSocialInsurance = "无",
                    Name = f.Name,
                    Passport = string.Empty,
                    PlanName = string.Empty,
                    Remark = string.Empty,
                    StartDate = nextDay.ToString("yyyy-MM-dd"),
                    WorkName = f.WorkName,
                }).ToList();
                var fileBytes = NpoiExcelHelper.WriteDeathInsurance(pushRecords);

                if (_environment.IsProduction())
                {

                    bool isSuc = await _mailSenderService.SendUseForInsuranceAsync("customer19@qidianbx.com", $"意外身故险_{DateTime.Today.ToString("yyyy-MM-dd")}", $"意外身故险_{DateTime.Today.ToString("yyyy-MM-dd")}", null, $"意外身故险_{DateTime.Today.ToString("yyyy-MM-dd")}.xlsx", fileBytes);
                    if (isSuc)
                    {

                        foreach (var record in records)
                        {

                            if (record.Status == Model.Enums.BusInsuranceRecordStatusEnumType.待增员)
                            {
                                record.Status = Model.Enums.BusInsuranceRecordStatusEnumType.已增员;
                            }
                            else
                            {
                                record.Status = Model.Enums.BusInsuranceRecordStatusEnumType.已减员;
                                record.EndDate=DateTime.Today;
                            }
                        }

                        await Update(records);

                    }
                }
                else { 
                
                        bool isSuc = await _mailSenderService.SendUseForInsuranceAsync("zk@cn-labor.cn", $"意外身故险_{DateTime.Today.ToString("yyyy-MM-dd")}", $"意外身故险_{DateTime.Today.ToString("yyyy-MM-dd")}", null, $"意外身故险_{DateTime.Today.ToString("yyyy-MM-dd")}.xlsx", fileBytes);
                }
            }
        }

        public async Task PushProcess()
        {
            var startDate = new DateTime(2026, 05, 01);
            var EndDate = new DateTime(2027, 04, 30);
            await MatchAllRecord(startDate, EndDate);
            await Task.Delay(1000);
            await PushAllRecord();
        }
    }
}
