using LZ.Model;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace LZ.Users.Service.Interface
{
    public interface IIntegralService
    {
        Task<List<Model.Shop_IntegralLog>> List(Expression<Func<Model.Shop_IntegralLog, bool>> exp, Model.PagedModel.UserPagedModel query);
        Task<int> Add(Model.Shop_IntegralLog model);
        Task<bool> Exist(Expression<Func<Shop_IntegralLog, bool>> exp);
        Task<int?> Sum(Expression<Func<Shop_IntegralLog, bool>> exp);
        Task<object> CatagoryStatistics(DateTime startTime, DateTime endTime);
    }
}
