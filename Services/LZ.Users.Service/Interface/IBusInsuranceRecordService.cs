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
    public interface IBusInsuranceRecordService
    {
        Task<bool> Add(Live_BusInsuranceRecord model);
        Task<bool> Add(List<Live_BusInsuranceRecord> models);
        Task Delete(Expression<Func<Live_BusInsuranceRecord, bool>> exp);
        Task<Live_BusInsuranceRecord> Get(Expression<Func<Live_BusInsuranceRecord, bool>> exp);
        Task<List<Live_BusInsuranceRecord>> List(Expression<Func<Live_BusInsuranceRecord, bool>> exp);
        Task<List<Live_BusInsuranceRecord>> PageList(Expression<Func<Live_BusInsuranceRecord, bool>> exp, PagedModelBase query);
        Task<bool> Update(Live_BusInsuranceRecord model);
        Task<bool> Update(List<Live_BusInsuranceRecord> models);
        Task<int> UpdateRecordMatchStatus(List<Live_BusInsuranceRecord> models);
    }
}
