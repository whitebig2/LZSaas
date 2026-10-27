using LZ.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Statistics.Service.Interface
{
    public interface IBusinessInsuranceRecordService
    {
        Task<int> Count(Expression<Func<Live_BusinessInsuranceRecord, bool>> exp);
    }
}
