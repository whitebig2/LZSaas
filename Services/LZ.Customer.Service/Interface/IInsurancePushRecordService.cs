using LZ.Model;
using LZ.Model.PagedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Customer.Service.Interface
{
    public interface IInsurancePushRecordService
    {
        Task<int> Add(Live_InsurancePushRecord model);
        Task<bool> Delete(Guid id);
        Task<bool> Exist(Guid id);
        Task<Live_InsurancePushRecord> Get(Guid id);
        Task<Live_InsurancePushRecord> Get(Expression<Func<Live_InsurancePushRecord, bool>> exp);
        Task<List<Live_InsurancePushRecord>> List(Expression<Func<Live_InsurancePushRecord, bool>> exp);
        Task<List<Live_V_InsurancePushRecord>> ListView(Expression<Func<Live_V_InsurancePushRecord, bool>> exp);
        Task<List<Live_V_InsurancePushRecord>> PageList(Expression<Func<Live_V_InsurancePushRecord, bool>> exp, PagedModelBase query);
        Task<bool> Update(Live_InsurancePushRecord model);
    }
}
