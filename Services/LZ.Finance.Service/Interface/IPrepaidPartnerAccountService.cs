using LZ.Model;
using LZ.Model.PagedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Finance.Service.Interface
{
    public interface IPrepaidPartnerAccountService
    {
        Task<int> Add(Live_PrepaidPartnerAccountExtension model);
        Task<bool> AddApplyRecord(Live_PrepaidApplyRecord record);
        Task<bool> ApplyRecordAudit(Guid recordID);
        Task<bool> Delete(Guid id);
        Task<bool> Exist(Guid id);
        Task<Live_PrepaidPartnerAccountExtension> Get(Guid id);
        Task<Live_PrepaidPartnerAccountExtension> Get(Expression<Func<Live_PrepaidPartnerAccountExtension, bool>> exp);
        Task<Live_PrepaidApplyRecord> GetApplyRecord(Guid recordID);
        Task<Live_PrepaidPartnerAccount> GetEntity(Guid id);
        Task<List<Live_PrepaidPartnerAccount>> List(Expression<Func<Live_PrepaidPartnerAccount, bool>> exp, PagedModelBase query);
        Task<List<Live_V_PrepaidPartnerAccount>> ListView(Expression<Func<Live_V_PrepaidPartnerAccount, bool>> exp);
        Task<List<Live_V_PrepaidApplyRecord>> PageRecordViewList(Expression<Func<Live_V_PrepaidApplyRecord, bool>> exp, PagedModelBase query);
        Task<List<Live_V_PrepaidPartnerAccount>> PageViewList(Expression<Func<Live_V_PrepaidPartnerAccount, bool>> exp, PagedModelBase query);
        Task<bool> Update(Live_PrepaidPartnerAccountExtension model);
        Task<bool> UpdateApplyRecord(Live_PrepaidApplyRecord record);
        Task<bool> UpdateEntity(Live_PrepaidPartnerAccount model);
    }
}
