using LZ.Core;
using LZ.Core.Interface;
using LZ.Model;
using LZ.Model.PagedModel;
using LZ.Users.Service.Interface;
using SqlSugar;
using SqlSugar.IOC;
using System.Linq.Expressions;


namespace LZ.Users.Service.Services
{
    public class BusinessInsuranceOrderEmpoyeeService : ServicesBase<Live_BusinessInsuranceOrderEmpoyee>, IBusinessInsuranceOrderEmpoyeeService, IScopedDependency
    {
        public async Task<List<Live_BusinessInsuranceOrderEmpoyee>> PageList(Expression<Func<Live_BusinessInsuranceOrderEmpoyee, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var list = await base.Context.Queryable<Live_BusinessInsuranceOrderEmpoyee>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<List<Live_BusinessInsuranceOrderEmpoyee>> List(Expression<Func<Live_BusinessInsuranceOrderEmpoyee, bool>> exp)
        {
            return await base.Context.Queryable<Live_BusinessInsuranceOrderEmpoyee>().Where(exp).OrderByDescending(it => it.ID).ToListAsync();
        }


        public async Task<bool> Update(Live_BusinessInsuranceOrderEmpoyee model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Exist(Expression<Func<Live_BusinessInsuranceOrderEmpoyee, bool>> exp)
        {
            return await base.Context.Queryable<Live_BusinessInsuranceOrderEmpoyee>().AnyAsync(exp);
        }

        public async Task<bool> ExistView(Expression<Func<Live_V_BusinessInsuranceOrderEmpoyee, bool>> exp)
        {
            return await base.Context.Queryable<Live_V_BusinessInsuranceOrderEmpoyee>().AnyAsync(exp);
        }

        public async Task<Live_V_BusinessInsuranceOrderEmpoyee> GetView(Expression<Func<Live_V_BusinessInsuranceOrderEmpoyee, bool>> exp)
        {
            return await base.Context.Queryable<Live_V_BusinessInsuranceOrderEmpoyee>().Where(exp).FirstAsync();
        }

        public async Task<Live_BusinessInsuranceOrderEmpoyee> Get(Expression<Func<Live_BusinessInsuranceOrderEmpoyee, bool>> exp)
        {
            return await base.Context.Queryable<Live_BusinessInsuranceOrderEmpoyee>().FirstAsync(exp);
        }
        public async Task<bool> Delete(Guid id)
        {
            return await base.Context.Deleteable<Live_BusinessInsuranceOrderEmpoyee>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
    }
}
