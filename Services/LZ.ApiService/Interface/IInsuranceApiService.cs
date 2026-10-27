using LZ.Model.InsuranceApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.ApiService.Interface
{
    public interface IInsuranceApiService
    {
        Task<BatchAddEmployeeResponse> BatchAddEmployee(BatchAddEmployeeRequest request);
        Task<BatchDelEmployeeResponse> BatchDelEmployee(BatchDelEmployeeRequest request);
        Task<BatchSearchStatusResponse> BatchSearchStatus(BatchSearchStatusRequest request);
        Task<OrderDetailResponse> GetOrderDetail(string insureId);
    }
}
