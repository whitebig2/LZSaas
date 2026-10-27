using LZ.Common;
using LZ.Common.Extensions;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Core.Redis;
using LZ.Model;
using LZ.Model.Dtos;
using LZ.Model.Enums;
using LZ.Model.PagedModel;
using LZ.Users.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System.Linq.Expressions;

namespace LZ.Users.Service.Services
{
    public class IntegralService : ServicesBase<Shop_IntegralLog>, IIntegralService, IScopedDependency
    {
        public async Task<List<Shop_IntegralLog>> List(Expression<Func<Shop_IntegralLog, bool>> exp, Model.PagedModel.UserPagedModel query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Shop_IntegralLog>().Where(exp).OrderByDescending(it => it.ID).ToListAsync();
            }
            var list = await base.Context.Queryable<Shop_IntegralLog>().Where(exp).OrderByDescending(it => it.ID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<int> Add(Shop_IntegralLog model)
        {
            return await base.Context.Insertable(model).ExecuteReturnIdentityAsync();
        }
        public async Task<bool> Exist(Expression<Func<Shop_IntegralLog, bool>> exp)
        {
            return await base.Context.Queryable<Shop_IntegralLog>().AnyAsync(exp);
        }
        public async Task<int?> Sum(Expression<Func<Shop_IntegralLog, bool>> exp)
        {
            return await base.Context.Queryable<Shop_IntegralLog>().Where(exp).SumAsync(it => it.Integral);
        }

        public async Task<object> CatagoryStatistics(DateTime startTime, DateTime endTime)
        {
            var validTypes = Enum.GetValues(typeof(IntegralType)).Cast<int>().ToList();
            return await base.Context.Queryable<Shop_IntegralLog>().Where(it => it.CreateTime >= startTime && it.CreateTime <= endTime
                                       && it.Integral != null && it.IntegralType != null && validTypes.Contains(it.IntegralType.Value))
                .GroupBy(it => it.IntegralType)
                .Select(it => new IntegralCatagoryStatisticsResultDto
                {
                    IntegralType = it.IntegralType!.Value,
                    Count = SqlFunc.AggregateSum(it.Integral!.Value),
                }, true)
                .Mapper(it =>
                {
                    it.Catagory = Tool.GetEnumDescription(it.IntegralType!.ToEnum<IntegralType>());
                })
                .ToListAsync();
        }
    }
}
