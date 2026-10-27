using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.PagedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Finance.Service.Interface
{
    public interface IPrepaidApplyRecordService
    {
        Task<int> Add(Live_PrepaidApplyRecord model);
        Task<bool> ApplyRecordAudit(Live_PrepaidApplyRecord record);
        Task<bool> Delete(Guid id);
        Task<bool> Exist(Guid id);
        Task<bool> Exist(Expression<Func<Live_PrepaidApplyRecord, bool>> exp);
        Task<decimal> SumMoney(Expression<Func<Live_PrepaidApplyRecord, bool>> exp);
        Task<Live_PrepaidApplyRecord> Get(Expression<Func<Live_PrepaidApplyRecord, bool>> exp);
        Task<bool> ChangeStatus(Guid Id, PrepaidRecordStatusEnumType status);
        Task<bool> ChangeApplyAmount(Guid Id, decimal money);
        Task<bool> ChangeApplyAmount(Guid Id, decimal money, decimal hour);
        Task<List<Live_V_PrepaidApplyRecord>> List(Expression<Func<Live_V_PrepaidApplyRecord, bool>> exp, PagedModelBase query);
        Task<List<Live_V_PrepaidApplyRecordExtension>> PageViewList(Expression<Func<Live_V_PrepaidApplyRecordExtension, bool>> exp, PagedModelBase query);
        Task<bool> Update(Live_PrepaidApplyRecord model);
        Task<List<Live_PrepaidApplyRecordExtension>> ListNav(Expression<Func<Live_PrepaidApplyRecordExtension, bool>> exp);
        Task<bool> UpdateNav(Live_PrepaidApplyRecordExtension model);
        Task<bool> UpdateNav(List<Live_PrepaidApplyRecordExtension> models);
        Task<List<Live_V_PrepaidApplyRecord>> ListView(Expression<Func<Live_V_PrepaidApplyRecord, bool>> exp);
        Task<int> Update(List<Live_PrepaidApplyRecord> models);
        Task<List<Live_PrepaidApplyRecord>> List(Expression<Func<Live_PrepaidApplyRecord, bool>> exp);
        Task<Live_V_PrepaidApplyRecord> GetView(Expression<Func<Live_V_PrepaidApplyRecord, bool>> exp);
        Task<List<Live_V_PrepaidRepaymentRecord>> ListRepaymentListView(Expression<Func<Live_V_PrepaidRepaymentRecord, bool>> exp);
        Task<Live_PrepaidApplyRecordExtension> GetNav(Guid id);
    }
}
