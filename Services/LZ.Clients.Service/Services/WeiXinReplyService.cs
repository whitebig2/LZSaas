using LZ.Clients.Service.Interface;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Core.Redis;
using LZ.Model;
using LZ.Model.PagedModel;
using SqlSugar;
using SqlSugar.IOC;
using System.Linq.Expressions;


namespace LZ.Clients.Service.Services
{
    public class WeiXinReplyService : ServicesBase<Model.Live_WeiXinReply>, IWeiXinReplyService, IScopedDependency
    {
        public async Task<List<Live_WeiXinReply>> List(Expression<Func<Live_WeiXinReply, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_WeiXinReply>().Where(exp).OrderByDescending(it => it.ID).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_WeiXinReply>().Where(exp).OrderByDescending(it => it.ID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<int> Add(Live_WeiXinReply model)
        {
          return  await base.Context.Insertable(model).ExecuteCommandAsync();
        }
        public async Task<bool> Exist(int id)
        {
            var expable = Expressionable.Create<Model.Live_WeiXinReply>();
            expable.And(it => it.ID == id);
            var exp = expable.ToExpression();
            return await Exist(exp);
        }
        public async Task<string> QueryKeywords(string keyword, int clientId)
        {
            var keyP = new SugarParameter("@keywords", keyword);
            var cliP = new SugarParameter("@clientId", clientId);
            var conP = new SugarParameter("@content", null, true);
            var content = await base.Context.Ado.UseStoredProcedure().GetDataTableAsync("SP_QueryKeywords", keyP, cliP, conP);
            return conP.Value.ToString();
        }
        public async Task<bool> Exist(Expression<Func<Live_WeiXinReply, bool>> exp)
        {
            return await base.Context.Queryable<Live_WeiXinReply>().AnyAsync(exp);
        }
        public async Task<bool> Update(Live_WeiXinReply model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public async Task<Live_WeiXinReply> Get(int type,int clientId)
        {
            return await base.Context.Queryable<Live_WeiXinReply>().FirstAsync(it => it.ClientID == clientId && it.ReplyType==type);
        }
        public async Task<Live_WeiXinReply> Get(int id)
        {
            return await base.Context.Queryable<Live_WeiXinReply>().FirstAsync(it=>it.ID==id);
        }
        public async Task<bool> Delete(int id)
        {
            return await base.Context.Deleteable<Live_WeiXinReply>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
    }
}
