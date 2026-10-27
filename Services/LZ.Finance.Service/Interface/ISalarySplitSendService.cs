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
    public interface ISalarySplitSendService
    {
        Task<bool> Add(Live_SalarySplitSend model);
        Task<bool> Delete(Expression<Func<Live_SalarySplitSend, bool>> exp);
        Task<bool> DeleteNav(Live_SalarySplitSendExtension model);
        Task<bool> DeleteNav(Expression<Func<Live_SalarySplitSendExtension, bool>> exp);
        Task<Live_SalarySplitSend> Get(Expression<Func<Live_SalarySplitSend, bool>> exp, string strOrderByFileds = null);
        Task<Live_SalarySplitSendExtension> GetNav(Expression<Func<Live_SalarySplitSendExtension, bool>> exp, string strOrderByFileds = null);
        Task<live_V_SalarySplitSendRecord> GetRecordView(Expression<Func<live_V_SalarySplitSendRecord, bool>> exp);
        Task<Live_V_SalarySplitSend> GetView(Expression<Func<Live_V_SalarySplitSend, bool>> exp);
        Task<List<live_V_SalarySplitSendRecord>> ListRecordViewList(Expression<Func<live_V_SalarySplitSendRecord, bool>> exp);
        Task<List<Live_V_SalarySplitSendExtension>> PageList(Expression<Func<Live_V_SalarySplitSendExtension, bool>> exp, PagedModelBase query);
        Task<List<live_V_SalarySplitSendRecord>> PageRecordList(Expression<Func<live_V_SalarySplitSendRecord, bool>> exp, PagedModelBase query);
        Task<bool> Update(Live_SalarySplitSend model);
        Task<bool> UpdateNav(Live_SalarySplitSendExtension model);
    }
}
