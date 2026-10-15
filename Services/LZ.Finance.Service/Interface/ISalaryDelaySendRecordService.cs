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
    public interface ISalaryDelaySendRecordService
    {
        Task<bool> Add(Live_SalaryDelaySendRecord model);
        Task<bool> Delete(Expression<Func<Live_SalaryDelaySendRecord, bool>> exp);
        Task<Live_SalaryDelaySendRecord> Get(Expression<Func<Live_SalaryDelaySendRecord, bool>> exp, string strOrderByFileds = null);
        Task<Live_V_SalaryDelaySendRecord> GetView(Expression<Func<Live_V_SalaryDelaySendRecord, bool>> exp, string strOrderByFileds = null);
        Task<List<Live_SalaryDelaySendRecord>> List(Expression<Func<Live_SalaryDelaySendRecord, bool>> exp, string strOrderByFileds = null);
        Task<List<Live_V_SalaryDelaySendRecord>> ListView(Expression<Func<Live_V_SalaryDelaySendRecord, bool>> exp, string strOrderByFileds = null);
        Task<List<Live_V_SalaryDelaySendRecord>> PageList(Expression<Func<Live_V_SalaryDelaySendRecord, bool>> exp, PagedModelBase query);
        Task<bool> Update(Live_SalaryDelaySendRecord model);
        Task<bool> Update(List<Live_SalaryDelaySendRecord> models);
        Task<bool> UpdateRecord(Live_SalaryDelaySendRecord model);
        Task<bool> UpdateTotalAmount(List<Live_SalaryDelaySendRecord> models);
    }
}
