using LZ.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Statistics.Service.Interface
{
    public interface ISaleCustomerService
    {
        Task<List<Model.Model.SaleCustomerModel>> SaleEntryStatistics(string ids);
        Task<string> SaleUpdatePersonnel(int userID, int cid,int type);
        Task<List<Live_PersonnelItems>> PersonnelTopReserve(DateTime time);
    }
}
