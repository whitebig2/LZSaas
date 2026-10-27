using System.Linq.Expressions;
using LZ.Model;
using LZ.Model.PagedModel;

namespace LZ.Clients.Service.Interface
{
    public interface IClientsService
    {
        Task<List<Model.Live_V_Clients>> List(Expression<Func<Model.Live_V_Clients, bool>> exp, PagedModelBase query);

        Task<int> Add(Model.Live_Clients model);

        Task<bool> Update(Model.Live_Clients model);
        Task<bool> ChangeStatus(int id, int status, DateTime? time=null);
        Task<bool> Delete(int id);
        Task<bool> Exist(int id);
        Task<bool> Exist(string appid, int id = 0);
        Task<bool> Exist(Expression<Func<Model.Live_Clients, bool>> exp);
        Task<bool> Set(int id, bool enabled);
        Task<Model.Live_ClientPage> GetPage(int id, string type);
        Task<Model.Live_Clients> Get(int id);
        Task<Model.Live_Clients> Get(Guid id);
        /// <summary>
        /// 根据类型获取
        /// </summary>
        /// <param name="company"></param>
        /// <param name="type">幸福职，蓝招</param>
        /// <param name="appType">小程序，公众号</param>
        /// <returns></returns>
        Task<Model.Live_Clients> Get(int company, string type,string appType);
        Task<Model.Live_Clients> Get(string appid);
        Live_Clients GetSyn(string appid);
    }
}
