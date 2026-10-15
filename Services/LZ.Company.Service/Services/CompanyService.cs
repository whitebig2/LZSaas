using LZ.Company.Service.Interface;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Core.Redis;
using LZ.Model;
using LZ.Model.PagedModel;
using SqlSugar;
using SqlSugar.IOC;
using System.Linq.Expressions;
using System.Text.Json;

namespace LZ.Company.Service.Services
{
    public class CompanyService(IRedisService redisService) : ServicesBase<SYS_Company>, ICompanyService, IScopedDependency
    {
        public async Task<List<SYS_Company>> List(Expression<Func<SYS_Company, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<SYS_Company>().Where(exp).OrderByDescending(it => it.CompanyID).ToListAsync();
            }
            var list = await base.Context.Queryable<SYS_Company>().Where(exp).OrderByDescending(it => it.CompanyID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<List<Live_V_CompanyPerFunction>> PerFunctionList(Expression<Func<Live_V_CompanyPerFunction, bool>> exp, PagedModelBase query)
        {
            return await base.Context.Queryable<Live_V_CompanyPerFunction>().Where(exp).OrderBy(it => it.PerSort).ToListAsync();
        }
        public async Task<List<Live_CompanyPerFunction>> PerFunctionList(Expression<Func<Live_CompanyPerFunction, bool>> exp, PagedModelBase query) 
        {
            return await base.Context.Queryable<Live_CompanyPerFunction>().Where(exp).OrderBy(it => it.Sort).ToListAsync();
        }
        public async Task<int> Add(SYS_Company model)
        {
            var id = await base.Context.Insertable(model).ExecuteReturnIdentityAsync();
            if (id > 0)
            {
               await CompanyInit(id,Common.Tool.Md5Psw32(model.Password));
            }
            return id;
        }
        private async Task CompanyInit(int companyId, string password)
        {
            var keyP = new SugarParameter("@id", companyId);
            var passP = new SugarParameter("@password", password);
            await base.Context.Ado.UseStoredProcedure().GetDataTableAsync("SP_CompanyInit", keyP, passP);
        }
        public async Task<bool> Exist(int id)
        {
            return await base.Context.Queryable<SYS_Company>().AnyAsync(it => it.CompanyID == id);
        }
        public async Task<bool> Exist(Expression<Func<SYS_Company, bool>> exp)
        {
            return await base.Context.Queryable<SYS_Company>().AnyAsync(exp);
        }
        public async Task<bool> Update(SYS_Company model)
        {
            var has = await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
            if (has)
            {
                var companyObj = new Model.CompanyModel();
                companyObj.Company = model;
                if (!string.IsNullOrEmpty(model.Config))
                {
                    companyObj.Config = JsonSerializer.Deserialize<Model.CompanyConfig>(model.Config, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                }
                else
                {
                    companyObj.Config = new Model.CompanyConfig();
                }
                if (companyObj.Config.EnableDockJDY && !string.IsNullOrEmpty(model.JDYForms))
                {
                    try
                    {
                        companyObj.JDYForms = JsonSerializer.Deserialize<List<Model.JDYFormModel>>(model.JDYForms, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                    }
                    catch (Exception ex)
                    {
                        Common.StaticLog.Error(ex);
                    }
                }
                await redisService.SetAsync(string.Format(CoreRedisConstants.CompanyConfig, model.CompanyGID), companyObj, TimeSpan.FromHours(10));
            }
            return has;
        }
        public async Task<bool> UpdateConfig(Model.CompanyConfig model,Guid id)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true // 格式化输出
            };
            var config = JsonSerializer.Serialize(model, options);
            var has = await base.Context.Updateable<Model.SYS_Company>().SetColumns(it => it.Config == config).Where(it => it.CompanyGID == id).ExecuteCommandAsync() > 0;
            if (has)
            {
                var companyObj = await redisService.GetAsync<Model.CompanyModel>(string.Format(CoreRedisConstants.CompanyConfig, id));
                companyObj.Config = model;
                await redisService.SetAsync(string.Format(CoreRedisConstants.CompanyConfig, id), companyObj, TimeSpan.FromHours(10));
            }
            return has;
        }
        public async Task<bool> ChangeStatus(int id,int status)
        {
            return await base.Context.Updateable<SYS_Company>().SetColumns(it => it.Status == status).Where(it => it.CompanyID == id).ExecuteCommandAsync() > 0;
        }
        public async Task SetPerFunction(int id, List<Model.Live_CompanyPerFunction> list)
        {
            await base.Context.Deleteable<Model.Live_CompanyPerFunction>().Where(it => it.CompanyID == id).ExecuteCommandAsync();
            await base.Context.Insertable(list).ExecuteCommandAsync();
        }
        public async Task<SYS_Company> Get(int id)
        {
            return await base.Context.Queryable<SYS_Company>().FirstAsync(it=>it.CompanyID == id);
        }

        public async Task<SYS_Company> Get(Guid id)
        {
            return await base.Context.Queryable<SYS_Company>().FirstAsync(it => it.CompanyGID == id);
        }
        public async Task<bool> Delete(int id)
        {
            return await base.Context.Deleteable<SYS_Company>().Where(it => it.CompanyID == id).ExecuteCommandAsync() > 0;
        }
    }
}
