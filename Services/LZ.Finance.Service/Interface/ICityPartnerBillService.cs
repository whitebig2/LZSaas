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
    public interface ICityPartnerBillService
    {
        Task<int> Add(Live_CityPartnerBill model);
        Task<int> Add(List<Live_CityPartnerBill> models);
        Task<bool> Delete(int id);
        Task<Live_CityPartnerBill> Get(Expression<Func<Live_CityPartnerBill, bool>> exp);
        Task<List<Live_CityPartnerBill>> List(Expression<Func<Live_CityPartnerBill, bool>> exp = null);
        Task<List<Live_CityPartnerBill>> PageList(Expression<Func<Live_CityPartnerBill, bool>> exp, PagedModelBase query);
        Task<bool> Update(Live_CityPartnerBill model);
    }
}
