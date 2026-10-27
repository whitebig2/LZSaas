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
    public class SalarySpecialDedcutionService : ServicesBase<Live_SalarySpecialDedcution>,  ISalarySpecialDedcutionService, IScopedDependency
    {

        public async Task<List<Live_SalarySpecialDedcution>> PageList(Expression<Func<Live_SalarySpecialDedcution, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var list = await   base.Context.Queryable<Live_SalarySpecialDedcution>().Where(exp).OrderByDescending(it => it.ID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<List<Live_SalarySpecialDedcution>> List(Expression<Func<Live_SalarySpecialDedcution, bool>> exp, PagedModelBase query)
        {
            return await   base.Context.Queryable<Live_SalarySpecialDedcution>().Where(exp).OrderByDescending(it => it.ID).ToListAsync();
        }
        public async Task<int> Add(List<Live_SalarySpecialDedcution> models)
        {
            return await   base.Context.Insertable(models).ExecuteCommandAsync();
        }
        public async Task<bool> Exist(int id)
        {
            return await   base.Context.Queryable<Live_SalarySpecialDedcution>().AnyAsync(it => it.ID == id);
        }
        public async Task<bool> Update(Live_SalarySpecialDedcution model)
        {
            return await   base.Context.Updateable<Live_SalarySpecialDedcution>(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<Live_SalarySpecialDedcution> Get(int id)
        {
            return await   base.Context.Queryable<Live_SalarySpecialDedcution>().FirstAsync(it => it.ID == id);
        }
        public async Task<Live_SalarySpecialDedcution> Get(Expression<Func<Live_SalarySpecialDedcution, bool>> exp)
        {
            return await   base.Context.Queryable<Live_SalarySpecialDedcution>().Where(exp).OrderByDescending(it => it.TaxCountMonth).FirstAsync();
        }
        public async Task<bool> Delete(int id)
        {
            return await   base.Context.Deleteable<Live_SalarySpecialDedcution>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Delete(Expression<Func<Live_SalarySpecialDedcution, bool>> exp)
        {
            return await   base.Context.Deleteable<Live_SalarySpecialDedcution>().Where(exp).ExecuteCommandAsync() > 0;
        }



    }
}
