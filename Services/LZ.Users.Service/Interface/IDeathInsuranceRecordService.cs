using LZ.Model;
using LZ.Model.PagedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Users.Service.Interface
{
    public interface IDeathInsuranceRecordService
    {
        Task<bool> Add(Live_DeathInsuranceRecord model);
        Task<bool> Add(List<Live_DeathInsuranceRecord> models);
        Task Delete(Expression<Func<Live_DeathInsuranceRecord, bool>> exp);
        Task<Live_DeathInsuranceRecord> Get(Expression<Func<Live_DeathInsuranceRecord, bool>> exp);
        Task<List<Live_DeathInsuranceRecord>> List(Expression<Func<Live_DeathInsuranceRecord, bool>> exp);
        Task MatchAllRecord(DateTime insuranceStartDate, DateTime insuranceEndDate);
        Task<List<Live_DeathInsuranceRecord>> PageList(Expression<Func<Live_DeathInsuranceRecord, bool>> exp, PagedModelBase query);
        Task PushAllRecord();
        Task PushProcess();
        Task<bool> Update(Live_DeathInsuranceRecord model);
        Task<bool> Update(List<Live_DeathInsuranceRecord> models);
    }
}
