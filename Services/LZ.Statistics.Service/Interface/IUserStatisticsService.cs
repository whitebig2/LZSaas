using LZ.Core.EventData;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace LZ.Statistics.Service.Interface
{
    public interface IUserStatisticsService
    {
        Task<int> UserCount(Expression<Func<Model.Live_User, bool>> exp=null);

    }
}
