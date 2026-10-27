using LZ.Core.EventData;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace LZ.Statistics.Service.Interface
{
    public interface IBasicDataService
    {
        Task<List<Live_PersonnelFunctionItems>> PersonnelFunctionItems(Expression<Func<Live_PersonnelFunctionItems, bool>> exp);

    }
}
