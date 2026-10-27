using LZ.Core.Interface;
using LZ.Model.PagedModel;
using LZ.Model;
using SqlSugar.IOC;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Core.Services
{
    public class FileUploadService : ServicesBase<Live_FileUpload>, IFileUploadService, IScopedDependency
    {
        public async Task<List<Model.Live_FileUpload>> List(Expression<Func<Model.Live_FileUpload, bool>> exp, UserPagedModel query, string order = "")
        {
            var count = new RefAsync<int>();
            var sco = base.Context.Queryable<Live_FileUpload>().Where(exp);
            if (!string.IsNullOrEmpty(order))
            {
                sco.OrderBy(order);
            }

            if (query.Page == 0)
            {
                return await sco.ToListAsync();
            }
            var list = await sco.ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<int> Add(Model.Live_FileUpload model)
        {
            var db = base.Context;
            return await db.Insertable(model).ExecuteCommandAsync();
        }
        public async Task<int> Add(List<Model.Live_FileUpload> list)
        {
            var db = base.Context;
            return await db.Insertable(list).ExecuteCommandAsync();
        }
        public async Task<bool> Update(Model.Live_FileUpload model)
        {
            var db = base.Context;
            return await db.Updateable(model).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Delete(int id)
        {
            var db = base.Context;
            return await db.Deleteable<Model.Live_FileUpload>().Where(t => t.ID == id).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> DeleteFID(int id,int key)
        {
            var db = base.Context;
            return await db.Deleteable<Model.Live_FileUpload>().Where(t => t.FID == id && t.TypeKey==key).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Exist(Expression<Func<Live_FileUpload, bool>> exp)
        {
            return await base.Context.Queryable<Live_FileUpload>().AnyAsync(exp);
        }
        public async Task<Model.Live_FileUpload> Get(int id)
        {
            var db = base.Context;
            return await db.Queryable<Model.Live_FileUpload>().Where(t => t.ID == id).FirstAsync();
        }
        public async Task<bool> ExistID(int id)
        {
            return await base.Context.Queryable<Live_FileUpload>().AnyAsync(it => it.ID == id);
        }
    }
}
