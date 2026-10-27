using LZ.Model;
using LZ.Model.ApiModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.ApiService.Interface
{
    public interface ISaleService
    {
        Task<int> GetCount(Dictionary<string, string> data, string url);
        Task<List<Model.SaleManageUser>?> GetManageUserList(string json, string url);
        Task<SaleMessageResult> AuditManageUser(string json, string url);
        Task<List<Model.CustomerCountByCityPartnerModel>?> GetCustomerCountList(Dictionary<string, string> data, string url);
        Task<SaleCityPartnerResult> CityPartnerBillList(Dictionary<string, string> dic, string url,int type);
    }
}
