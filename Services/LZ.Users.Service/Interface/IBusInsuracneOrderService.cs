using LZ.Model;
using LZ.Model.PagedModel;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LZ.Users.Service.Interface
{
    public interface IBusInsuracneOrderService
    {
        Task<bool> Add(List<Live_BusInsuracneOrder> models);
        Task<bool> Add(Live_BusInsuracneOrder model);
        Task<bool> AddNav(Live_BusInsuracneOrderExtension model);
        Task<bool> AddNav(List<Live_BusInsuracneOrderExtension> models);
        Task Delete(Expression<Func<Live_BusInsuracneOrder, bool>> exp);
        Task<bool> DeleteNav(Expression<Func<Live_BusInsuracneOrderExtension, bool>> exp);
        Task<Live_BusInsuracneOrder> Get(Expression<Func<Live_BusInsuracneOrder, bool>> exp);
        Task<Live_BusInsuracneOrderExtension> GetNav(Expression<Func<Live_BusInsuracneOrderExtension, bool>> exp);
        Task<List<Live_BusInsuracneOrder>> List(Expression<Func<Live_BusInsuracneOrder, bool>> exp);
        Task<List<Live_V_BusInsuracneOrderPlan>> ListOrderPlan(Expression<Func<Live_V_BusInsuracneOrderPlan, bool>> exp);
        Task<List<Live_BusInsuracneOrder>> PageList(Expression<Func<Live_BusInsuracneOrder, bool>> exp, PagedModelBase query);
        Task<bool> Update(List<Live_BusInsuracneOrder> models);
        Task<bool> Update(Live_BusInsuracneOrder model);
        Task<bool> UpdateNav(Live_BusInsuracneOrderExtension model);
    }
}
