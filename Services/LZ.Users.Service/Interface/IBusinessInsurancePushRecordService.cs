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
    public interface IBusinessInsurancePushRecordService
    {
        Task<bool> Delete(Guid id);
        Task<Live_BusinessInsurancePushRecord> Get(Expression<Func<Live_BusinessInsurancePushRecord, bool>> exp);
        Task<List<Live_BusinessInsurancePushRecord>> List(Expression<Func<Live_BusinessInsurancePushRecord, bool>> exp);
        Task<List<Live_V_BusinessInsurancePushRecord>> ListView(Expression<Func<Live_V_BusinessInsurancePushRecord, bool>> exp);
        Task<List<Live_BusinessInsurancePushRecord>> PageList(Expression<Func<Live_BusinessInsurancePushRecord, bool>> exp, PagedModelBase query);
        Task<bool> Update(Live_BusinessInsurancePushRecord model);
    }
}
