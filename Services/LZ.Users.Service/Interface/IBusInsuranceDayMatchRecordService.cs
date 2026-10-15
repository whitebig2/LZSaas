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
   

    public interface IBusInsuranceDayMatchRecordService
    {
        Task<bool> Add(List<Live_BusInsuranceDayMatchRecord> models);
        Task<bool> Add(Live_BusInsuranceDayMatchRecord model);
        Task Delete(Expression<Func<Live_BusInsuranceDayMatchRecord, bool>> exp);
        Task<Live_BusInsuranceDayMatchRecord> Get(Expression<Func<Live_BusInsuranceDayMatchRecord, bool>> exp);
        Task<List<Live_BusInsuranceDayMatchRecord>> List(Expression<Func<Live_BusInsuranceDayMatchRecord, bool>> exp);
        Task<List<Live_BusInsuranceDayMatchRecord>> PageList(Expression<Func<Live_BusInsuranceDayMatchRecord, bool>> exp, PagedModelBase query);
        Task<bool> Update(List<Live_BusInsuranceDayMatchRecord> models);
        Task<bool> Update(Live_BusInsuranceDayMatchRecord model);
    }
}
