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
    public interface ISalaryRecordService
    {
        Task<Live_SalaryRecord> Get(Expression<Func<Live_SalaryRecord, bool>> exp, string strOrderByFileds = null);
        Task<Live_V_SalaryRecord> GetView(Expression<Func<Live_V_SalaryRecord, bool>> exp, string strOrderByFileds = null);
        Task<List<Live_SalaryRecord>> List(Expression<Func<Live_SalaryRecord, bool>> exp, string strOrderByFileds = null);
        Task<List<Live_V_SalaryRecord>> ListView(Expression<Func<Live_V_SalaryRecord, bool>> exp);
        Task<List<Live_V_SalaryRecord>> PageList(Expression<Func<Live_V_SalaryRecord, bool>> exp, PagedModelBase query);
        Task<bool> SetSendSuccess(Live_SalaryRecord model);
        Task<bool> Update(Live_SalaryRecord model);
        Task<bool> Update(List<Live_SalaryRecord> models);
        void VerifyPendingSend(Live_SalaryRecord model);
        Task<decimal> AvgSalary(Expression<Func<Live_V_SalaryRecord, bool>> exp);
        Task<bool> SetSendFail(Live_SalaryRecord model, SalaryFailTypeEnumType failType, string failMsg);
    }
}
