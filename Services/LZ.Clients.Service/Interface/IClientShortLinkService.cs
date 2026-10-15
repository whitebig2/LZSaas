using System.Linq.Expressions;
using LZ.Model;
using LZ.Model.PagedModel;

namespace LZ.Clients.Service.Interface
{
    public interface IClientShortLinkService
    {
        Task<List<Model.Live_V_ClientShortLink>> List(Expression<Func<Model.Live_V_ClientShortLink, bool>> exp, PagedModelBase query);

        Task<int> Add(Model.Live_ClientShortLink model);

        Task<bool> Delete(int id);
        Task<Model.Live_ClientShortLink> Get(int id);
        /// <summary>
        /// 获取小程序链接
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="clientType">蓝领端还是企业端</param>
        /// <param name="pageUrl"></param>
        /// <param name="pageTitle"></param>
        /// <returns></returns>
        Task<string> GetLink(int companyId, string clientType, string pageUrl, string pageTitle);
    }
}
