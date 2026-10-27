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
    public interface ICityPartnerEmpRecordService
    {
        Task<int> Add(List<Live_CityPartnerEmpRecord> models);
        Task<bool> Delete(int id);
        Task<bool> Delete(Expression<Func<Live_CityPartnerEmpRecord, bool>> exp = null);
        Task<Live_CityPartnerEmpRecord> Get(Expression<Func<Live_CityPartnerEmpRecord, bool>> exp);
        Task<List<Live_CityPartnerEmpRecord>> List(Expression<Func<Live_CityPartnerEmpRecord, bool>> exp = null);
        Task<List<Live_CityPartnerEmpRecord>> PageList(Expression<Func<Live_CityPartnerEmpRecord, bool>> exp, PagedModelBase query);
        Task<bool> Update(Live_CityPartnerEmpRecord model);
    }
}
