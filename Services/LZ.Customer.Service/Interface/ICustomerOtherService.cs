using LZ.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Customer.Service.Interface
{
    public interface ICustomerOtherService
    {
        Task<ApiRequestResultModel<Live_Enterprise>> GetEnterpriseByIdAsync(Guid id);
        Task<ApiRequestResultModel<Live_V_UserEntry>> GetUserEntryRecord(int id);
        Task<ApiRequestResultModel<List<Live_V_UserEntry>>> GetUserEntryListByUserID(int userID);
        Task<ApiRequestResultModel<Live_V_SalaryCashSendRecord>> GetSalaryCashSendRecordByIdAsync(Guid id);
        Task<ApiRequestResultModel<Live_V_UserEntryContract>> GetUserEntryContract(int id);
        Task<ApiRequestResultModel<List<Live_V_UserEntry>>> GetUserEntryListByInsuranceStartDate(Guid customerGID, DateTime startDate);
        Task<ApiRequestResultModel<List<Live_UserLeaveReason>>> ListLeaveReason(int id);
        Task<RequestResultModel> GetClientShortLink(int companyId, string clientType, string pageUrl, string pageTitle);
    }
}
