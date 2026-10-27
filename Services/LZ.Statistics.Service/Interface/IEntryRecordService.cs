using LZ.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Statistics.Service.Interface
{
    public interface IEntryRecordService
    {
        Task<Live_V_UserEntryRecord> Model(Expression<Func<Live_V_UserEntryRecord, bool>> exp);
    }
}
