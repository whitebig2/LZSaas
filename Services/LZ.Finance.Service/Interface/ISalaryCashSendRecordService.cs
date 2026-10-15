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
    public interface ISalaryCashSendRecordService
    {
        Task<bool> Add(Live_SalaryCashSendRecord model);
        Task<bool> Delete(Expression<Func<Live_SalaryCashSendRecord, bool>> exp);
        Task<Live_SalaryCashSendRecord> Get(Expression<Func<Live_SalaryCashSendRecord, bool>> exp, string strOrderByFileds = null);
        Task<Live_V_SalaryCashSendRecord> GetView(Expression<Func<Live_V_SalaryCashSendRecord, bool>> exp, string strOrderByFileds = null);
        Task<List<Live_SalaryCashSendRecord>> List(Expression<Func<Live_SalaryCashSendRecord, bool>> exp, string strOrderByFileds = null);
        Task<List<Live_V_SalaryCashSendRecord>> ListView(Expression<Func<Live_V_SalaryCashSendRecord, bool>> exp, string strOrderByFileds = null);
        Task<List<Live_V_SalaryCashSendRecord>> PageList(Expression<Func<Live_V_SalaryCashSendRecord, bool>> exp, PagedModelBase query);
        Task<bool> Update(Live_SalaryCashSendRecord model);
        Task<bool> UpdateCashSignStatus(Live_V_SalaryCashSendRecord model);
        Task<bool> UpdateTotalAmount(List<Live_SalaryCashSendRecord> models);
    }
}
