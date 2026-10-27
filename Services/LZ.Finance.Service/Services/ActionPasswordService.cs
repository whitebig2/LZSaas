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
    public class ActionPasswordService : ServicesBase<Live_ActionPassword>, IActionPasswordService, IScopedDependency
    {

        public async Task<List<Live_ActionPassword>> PageList(Expression<Func<Live_ActionPassword, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await  base.Context.Queryable<Live_ActionPassword>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            }
            var list = await  base.Context.Queryable<Live_ActionPassword>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

       
        public async Task<int> Add(Live_ActionPassword model)
        {
            return await  base.Context.Insertable(model).ExecuteCommandAsync();
        }
        public async Task<bool> Exist(Guid id)
        {
            return await  base.Context.Queryable<Live_ActionPassword>().AnyAsync(it => it.ID == id);
        }
        public async Task<bool> Update(Live_ActionPassword model)
        {
            return await  base.Context.Updateable<Live_ActionPassword>(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        //public async Task<bool> ChangeStatus(Guid id, int status)
        //{
        //    return await  base.Context.Updateable<Live_ActionPassword>().SetColumns(it => it.Status == status).Where(it => it.Id == id).ExecuteCommandAsync() > 0;
        //}

        public async Task<Live_ActionPassword> Get(Guid id)
        {
            return await  base.Context.Queryable<Live_ActionPassword>().FirstAsync(it => it.ID == id);
        }

        public async Task<Live_ActionPassword> Get(Expression<Func<Live_ActionPassword, bool>> exp)
        {
            return await  base.Context.Queryable<Live_ActionPassword>().FirstAsync(exp);
        }
        public async Task<bool> Delete(Guid id)
        {
            return await  base.Context.Deleteable<Live_ActionPassword>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
    }
}
