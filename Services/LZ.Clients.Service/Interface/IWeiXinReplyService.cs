using System.Linq.Expressions;
using LZ.Model;
using LZ.Model.PagedModel;

namespace LZ.Clients.Service.Interface
{
    public interface IWeiXinReplyService
    {
        Task<List<Model.Live_WeiXinReply>> List(Expression<Func<Model.Live_WeiXinReply, bool>> exp, PagedModelBase query);

        Task<int> Add(Model.Live_WeiXinReply model);

        Task<bool> Update(Model.Live_WeiXinReply model);
        Task<bool> Delete(int id);
        Task<bool> Exist(int id);
        Task<bool> Exist(Expression<Func<Model.Live_WeiXinReply, bool>> exp);
        Task<Model.Live_WeiXinReply> Get(int id);
        Task<string> QueryKeywords(string keyword,int clientId);
        /// <summary>
        /// 根据回复类型获取
        /// </summary>
        /// <param name="type"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        Task<Live_WeiXinReply> Get(int type, int clientId);
    }
}
