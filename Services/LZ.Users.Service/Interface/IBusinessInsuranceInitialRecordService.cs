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
    public interface IBusinessInsuranceInitialRecordService
    {
        Task<int> Add(List<Live_BusinessInsuranceInitialRecord> models);
        Task<int> Add(Live_BusinessInsuranceInitialRecord model);
        Task<bool> Delete(Guid id);
        Task<bool> Delete(Expression<Func<Live_BusinessInsuranceInitialRecord, bool>> exp);
        Task<Live_BusinessInsuranceInitialRecord> Get(Expression<Func<Live_BusinessInsuranceInitialRecord, bool>> exp);
        Task<List<Live_BusinessInsuranceInitialRecord>> List(Expression<Func<Live_BusinessInsuranceInitialRecord, bool>> exp);
        Task<List<Live_BusinessInsuranceInitialRecord>> PageList(Expression<Func<Live_BusinessInsuranceInitialRecord, bool>> exp, PagedModelBase query);
        Task<bool> Update(Live_BusinessInsuranceInitialRecord model);
        Task<bool> Update(List<Live_BusinessInsuranceInitialRecord> models);
    }
}
