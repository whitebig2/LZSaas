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
    public interface ICityPartnerSalaryRecordService
    {
        Task<int> Add(Live_CityPartnerSalaryRecord model);
        Task<int> Add(List<Live_CityPartnerSalaryRecord> models);
        Task<bool> Delete(int id);
        Task<bool> Delete(Expression<Func<Live_CityPartnerSalaryRecord, bool>> exp = null);
        Task<Live_CityPartnerSalaryRecord> Get(Expression<Func<Live_CityPartnerSalaryRecord, bool>> exp);
        Task<List<Live_CityPartnerSalaryRecord>> List(Expression<Func<Live_CityPartnerSalaryRecord, bool>> exp = null);
        Task<List<Live_CityPartnerSalaryRecord>> PageList(Expression<Func<Live_CityPartnerSalaryRecord, bool>> exp, PagedModelBase query);
        Task<int> RecordCount(Expression<Func<Live_CityPartnerSalaryRecord, bool>> exp = null);
        Task<bool> Update(Live_CityPartnerSalaryRecord model);
    }
}
