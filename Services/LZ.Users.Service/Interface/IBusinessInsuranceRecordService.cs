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
    public interface IBusinessInsuranceRecordService
    {
        Task<int> Add(Live_BusinessInsuranceRecord model);
        Task<int> Add(List<Live_BusinessInsuranceRecord> models);
        Task<bool> Delete(Guid id);
        Task<bool> Delete(Expression<Func<Live_BusinessInsuranceRecord, bool>> exp);
        Task<bool> Exist(Expression<Func<Live_BusinessInsuranceRecord, bool>> exp);
        Task<Live_BusinessInsuranceRecord> Get(Expression<Func<Live_BusinessInsuranceRecord, bool>> exp);
        Task<List<Live_BusinessInsuranceRecord>> List(Expression<Func<Live_BusinessInsuranceRecord, bool>> exp);
        Task<List<Live_V_BusinessInsuranceRecord>> ListView(Expression<Func<Live_V_BusinessInsuranceRecord, bool>> exp);
        Task<bool> ManageUserEntryPlan(int userEntryID);
        Task<bool> ManageUserEntryPlanRecord(int userEntryID);
        Task<List<Live_BusinessInsuranceRecord>> PageList(Expression<Func<Live_BusinessInsuranceRecord, bool>> exp, PagedModelBase query);
        Task<bool> Update(List<Live_BusinessInsuranceRecord> models);
        Task<bool> Update(Live_BusinessInsuranceRecord model);
    }
}
