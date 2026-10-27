using LZ.Customer.Service.Interface;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Model;
using SqlSugar;
using SqlSugar.IOC;
using System.Linq.Expressions;
using LZ.Model.PagedModel;
using LZ.Model.Dtos;
using LZ.Model.ModelExtensions;
using Senparc.CO2NET.Cache.Redis;


namespace LZ.Customer.Service.Services
{
    public class CashSignRecordService : ServicesBase<Live_CashSignRecord>, ICashSignRecordService, IScopedDependency
    {


        public async Task<List<Live_UserEntry>> UserEntryList(Expression<Func<Live_UserEntry, bool>> exp)
        {
            var list = base.Context.Queryable<Live_UserEntry>().Where(exp).OrderByDescending(it => it.CreateTime).ToList();
            return list;
        }

        public async Task<bool> UpdateUserEntry(Live_UserEntry model)
        {
            return await base.Context.Updateable<Live_UserEntry>(model).ExecuteCommandAsync() > 0;
        }

        public async Task<List<Live_CashSignRecord>> PageList(Expression<Func<Live_CashSignRecord, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var queryable = base.Context.Queryable<Live_CashSignRecord>().Where(exp).OrderByDescending(it => it.CreateTime);
           
            var list = await queryable.ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        

        public async Task<Live_CashSignRecord> Get(Expression<Func<Live_CashSignRecord, bool>> exp)
        {
            return await base.Context.Queryable<Live_CashSignRecord>().OrderByDescending(f => f.CreateTime)
                .Where(exp).OrderByDescending(f=>f.ExpirationDate).FirstAsync();
        }

        public async Task<bool> Add(Live_CashSignRecord model)
        {
            return await base.Context.Insertable(model).ExecuteCommandIdentityIntoEntityAsync();
        }

        public async Task<bool> Update(Live_CashSignRecord model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Exist(Expression<Func<Live_CashSignRecord, bool>> exp)
        {
            return await base.Context.Queryable<Live_CashSignRecord>().AnyAsync(exp);
        }


        public async Task<bool> Delete(Expression<Func<Live_CashSignRecord, bool>> exp)
        {
            return await base.Context.Deleteable<Live_CashSignRecord>().Where(exp).ExecuteCommandAsync() > 0;
        }



        public async Task<List<Live_CashSignRecord>> List(Expression<Func<Live_CashSignRecord, bool>> exp)
        {
            var list = await base.Context.Queryable<Live_CashSignRecord>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            return list;
        }


        public async Task<List<Live_ContractRecordExtension>> ListNav(Expression<Func<Live_ContractRecordExtension, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_ContractRecordExtension>().Includes(f=>f.Signers).Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_ContractRecordExtension>().Includes(f=>f.Signers).Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

  
  

        public async Task<List<Live_V_ContractRecord>> ListView(Expression<Func<Live_V_ContractRecord, bool>> exp)
        {
            var list = await base.Context.Queryable<Live_V_ContractRecord>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            return list;
        }


        public async Task<List<Live_ContractRecordExtension>> ListWithDetail(Expression<Func<Live_ContractRecordExtension, bool>> exp)
        {
            return await base.Context.Queryable<Live_ContractRecordExtension>().AS("Live_ContractRecord").OrderByDescending(f => f.CreateTime)
                .Includes(f => f.Signers)
                .Where(exp).ToListAsync();
        }
        public async Task<int> Add(Live_ContractRecord model)
        {
          return  await base.Context.Insertable(model).ExecuteCommandAsync();
        }
        public async Task<Live_ContractRecord> Add(Live_ContractRecord model, List<Live_ContractRecordSigner> signers)
        {
            try
            {
                await itenant.BeginTranAsync();
                await base.Context.Insertable(model).ExecuteCommandAsync();
                if (signers != null && signers.Any())
                {
                    await base.Context.Insertable(signers).ExecuteCommandAsync();
                }
                await itenant.CommitTranAsync();
                return model;
            }
            catch (Exception ex)
            {
                itenant.RollbackTran();
                throw ex;
            }
        }
        public async Task<bool> Exist(Guid id)
        {
            return await base.Context.Queryable<Live_ContractRecord>().AnyAsync(it => it.ID == id);
        }
        public async Task<bool> Update(Live_ContractRecord model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Update(List<Live_ContractRecord> models)
        {
            return await base.Context.Updateable(models).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public async Task<Live_ContractRecord> Get(Guid id)
        {
            return await base.Context.Queryable<Live_ContractRecord>().FirstAsync(it=>it.ID == id);
        }

       
        public async Task<List<Live_ContractRecordSigner>> GetRecordSigners(Guid recordId)
        {
            return await base.Context.Queryable<Live_ContractRecordSigner>().Where(f=>f.ContractRecordID==recordId).ToListAsync();
        }
        public async Task<bool> UpdateRecordSigners(List<Live_ContractRecordSigner>  signers)
        {
            return await base.Context.Updateable(signers).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> UpdateRecordSigners(Live_ContractRecord record, List<Live_ContractRecordSigner> signers)
        {
            try
            {
                await itenant.BeginTranAsync();
                if (record != null)
                {
                    await base.Context.Updateable(record).ExecuteCommandAsync();
                }
                if (signers != null && signers.Count > 0)
                {
                    await base.Context.Updateable(signers).ExecuteCommandAsync();
                }
                await itenant.CommitTranAsync();
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public async Task<bool> Delete(Guid id)
        {
            return await base.Context.Deleteable<Live_ContractRecord>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }


        public async Task<Live_ContractRecordExtension> GetNav(Guid id)
        {
            var count = new RefAsync<int>();
            return await base.Context.Queryable<Live_ContractRecordExtension>().AS("Live_ContractRecord").OrderByDescending(f => f.CreateTime)
                .Includes(f => f.Signers)
                .FirstAsync(f => f.ID == id);
        }

        public async Task<Live_ContractRecordExtension> GetNav(Expression<Func<Live_ContractRecordExtension, bool>> exp)
        {
            var count = new RefAsync<int>();
            return await base.Context.Queryable<Live_ContractRecordExtension>().OrderByDescending(f => f.CreateTime)
                .Includes(f => f.Signers)
                .FirstAsync(exp);
        }
        public async Task<bool> UpdateNav(Live_ContractRecordExtension model)
        {
          return  await base.Context.UpdateNav(model)
                .Include(f => f.Signers)
                .ExecuteCommandAsync();


        }
        public async Task<bool> AddNav(Live_ContractRecordExtension model)
        {
            return await base.Context.InsertNav(model)
                  .Include(f => f.Signers)
                  .ExecuteCommandAsync();


        }

        public async Task<bool> DeleteNav(Guid id)
        {
            return await base.Context.DeleteNav<Live_ContractRecordExtension>(f => f.ID == id).Include(f => f.Signers).ExecuteCommandAsync();
        }


        public async Task<int> SetDisabled(Expression<Func<Live_CashSignRecord, bool>> exp)
        {
            return await base.Context.Updateable<Live_CashSignRecord>().SetColumns(f => f.IsEnable == false).Where(exp).ExecuteCommandAsync();
        }
    }
}
