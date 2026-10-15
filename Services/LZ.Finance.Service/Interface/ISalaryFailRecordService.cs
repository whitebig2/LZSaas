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
    public interface ISalaryFailRecordService
    {
        Task<bool> Add(Live_SalaryFailRecord model);
        Task<bool> Delete(Expression<Func<Live_SalaryFailRecord, bool>> exp);
        Task<Live_SalaryFailRecord> Get(Expression<Func<Live_SalaryFailRecord, bool>> exp, string strOrderByFileds = null);
        Task<Live_V_SalaryFailRecord> GetView(Expression<Func<Live_V_SalaryFailRecord, bool>> exp, string strOrderByFileds = null);
        Task<List<Live_SalaryFailRecord>> List(Expression<Func<Live_SalaryFailRecord, bool>> exp, string strOrderByFileds = null);
        Task<List<Live_V_SalaryFailRecord>> ListView(Expression<Func<Live_V_SalaryFailRecord, bool>> exp, string strOrderByFileds = null);
        Task<List<Live_V_SalaryFailRecord>> PageList(Expression<Func<Live_V_SalaryFailRecord, bool>> exp, PagedModelBase query);
        Task<bool> Update(Live_SalaryFailRecord model);
        Task<bool> Update(List<Live_SalaryFailRecord> models);
    }
}
