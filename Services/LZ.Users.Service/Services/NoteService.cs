using LZ.Users.Service.Interface;
using LZ.Core.Interface;
using LZ.Core.Redis;
using LZ.Model;
using SqlSugar;
using SqlSugar.IOC;
using System.Linq.Expressions;
using LZ.Model.PagedModel;
using LZ.Core;

namespace LZ.Users.Service.Services
{
    public class NoteService(EntryService entryService) : ServicesBase<Live_UserEntryNote>, INoteService, IScopedDependency
    {
        public async Task<List<Live_V_UserEntryNote>> List(Expression<Func<Live_V_UserEntryNote, bool>> exp, Model.PagedModel.UserPagedModel query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_V_UserEntryNote>().Where(exp).OrderByDescending(it=>it.ID).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_V_UserEntryNote>().Where(exp).OrderByDescending(it => it.ID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<int> Add(Live_UserEntryNote model)
        {
            await entryService.UpdateLastNote(model.EntryID.Value, model.Remarks);
            return await base.Context.Insertable(model).ExecuteCommandAsync();
        }

        public async Task<bool> Delete(int id)
        {
            return await base.Context.Deleteable<Live_UserEntryNote>().Where(it => it.UserID == id).ExecuteCommandAsync() > 0;
        }
    }
}
