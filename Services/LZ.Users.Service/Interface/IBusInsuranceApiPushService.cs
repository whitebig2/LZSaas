using LZ.Model;
using LZ.Model.ModelExtensions;
using LZ.Model.PagedModel;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LZ.Users.Service.Interface
{
    public interface IBusInsuranceApiPushService
    {
        Task<bool> Add(Live_BusInsuranceApiPush model);
        Task<bool> Add(List<Live_BusInsuranceApiPush> models);
        Task<bool> AddNav(Live_BusInsuranceApiPushExtension model);
        Task<bool> AddNav(List<Live_BusInsuranceApiPushExtension> models);
        Task Delete(Expression<Func<Live_BusInsuranceApiPush, bool>> exp);
        Task<bool> DeleteNav(Guid id);
        Task<bool> Exist(Expression<Func<Live_BusInsuranceApiPush, bool>> exp);
        Task<Live_BusInsuranceApiPush> Get(Expression<Func<Live_BusInsuranceApiPush, bool>> exp);
        Task<Live_BusInsuranceApiPushDetail> GetDetail(Expression<Func<Live_BusInsuranceApiPushDetail, bool>> exp);
        Task<Live_BusInsuranceApiPushExtension> GetNav(Expression<Func<Live_BusInsuranceApiPushExtension, bool>> exp);
        Task<List<Live_BusInsuranceApiPush>> List(Expression<Func<Live_BusInsuranceApiPush, bool>> exp);
        Task<List<Live_BusInsuranceApiPushDetail>> ListDetail(Expression<Func<Live_BusInsuranceApiPushDetail, bool>> exp);
        Task<List<Live_BusInsuranceApiPushExtension>> ListNav(Expression<Func<Live_BusInsuranceApiPushExtension, bool>> exp);
        Task<List<Live_BusInsuranceApiPush>> PageList(Expression<Func<Live_BusInsuranceApiPush, bool>> exp, PagedModelBase query);
        Task<List<Live_BusInsuranceApiPushDetail>> PageDetailList(Expression<Func<Live_BusInsuranceApiPushDetail, bool>> exp, PagedModelBase query);
        Task<bool> Update(Live_BusInsuranceApiPush model);
        Task<bool> Update(List<Live_BusInsuranceApiPush> models);
        Task<bool> UpdateDetail(Live_BusInsuranceApiPushDetail model);
        Task<bool> UpdateNav(Live_BusInsuranceApiPushExtension model);
        Task<bool> DeleteDetail(Guid id);
        /// <summary>
        /// 确认推送：已推送待确认 → 已确认
        /// </summary>
        Task Confirm(Guid id);
    }
}
