using LZ.Model;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Statistics.Service.Interface
{
    public interface IBudgetService
    {
        Task<List<Model.Statistics.EntryModel>> StatusCount(Expression<Func<Model.Live_BudgetApplyRecord, bool>> exp);
    }
}
