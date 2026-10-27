using LZ.Core.EventData;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace LZ.Statistics.Service.Interface
{
    public interface IPartnerStatisticsService
    {
        Task<int> Count(Expression<Func<Model.Live_Partner, bool>> exp=null);

    }
}
