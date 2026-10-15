using LZ.Finance.Service.Services;
using LZ.Model;
using LZ.Model.ModelExtensions;
using LZ.Model.Response;
using LZ.Model.Response.Xfz;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Finance.Service.Interface
{
    public interface IFinanceOtherService
    {
        Task<XfzBaseResponse<bool>> BatchTransaction(BatchTransactionRequest req);
        Task<XfzBaseResponse<EIST_Q01000407_RES>> BatchTransactionDetail(Guid businessID);
        Task<ApiRequestResultModel<string>> BudgetApplyRecordAuditing([FromForm] int id);
        Task<ApiRequestResultModel<int>> BudgetApplyRecordUpdateStatus(int id, int status, string msg = "客户工资重置");
        Task<XfzBaseResponse<XfzPageResponse<EIST_Q01000404_RES_ITEM>>> EcpSignalTransactionPageList(DateTime dateFrom, DateTime dateTo, string subAccountBookNo, int pageNo, int pageSize);
        Task<ApiRequestResultModel<List<Live_WhiteList>>> GeCashWhiteAllListAsync();
        Task<XfzBaseResponse<List<FinanceOtherService.CmbcBankInfo>>> GetCmbcAccounts(List<string> idCardNoList);
        Task<ApiRequestResultModel<Live_ContractFileConfigExtension>> GetContractFileConfigByIdAsync(Guid id);
        Task<ApiRequestResultModel<Live_V_CustomerContractConfig>> GetContractFilesByCustomerGIDAsync(Guid customerGID);
        Task<ApiRequestResultModel<Live_ContractSigner>> GetContractSignerByIdno(string idno);
        Task<ApiRequestResultModel<Live_V_Customer>> GetCustomerByGIDAsync(Guid customerGID);
        Task<ApiRequestResultModel<Live_Customer>> GetCustomerByID(int id);
        Task<ApiRequestResultModel<Live_V_Customer>> GetCustomerByNameAsync(string name);
        Task<ApiRequestResultModel<Live_CustomerContractConfig>> GetCustomerContractConfigByIdAsync(Guid id);
        Task<ApiRequestResultModel<List<SYS_Dictionary>>> GetDeductionHouseRentCityListAsync(string name);
        Task<Live_Enterprise> GetEnterpriseAsync(Guid? id, string name);
        Task<ApiRequestResultModel<Live_Enterprise>> GetEnterpriseByIdAsync(Guid id);
        Task<ApiRequestResultModel<Live_Enterprise>> GetEnterpriseByNameAsync(string name);
        Task<ApiRequestResultModel<Live_Partner>> GetPartnerByID(int id);
        Task<ApiRequestResultModel<Live_Partner>> GetPartnerByName(string name);
        Task<ApiRequestResultModel<List<Live_V_Personnel>>> GetPersonnelListByCustomerGIDAsync(Guid customerGID);
        Task<ApiRequestResultModel<Live_V_UserEntry>> GetUserEntryByID(int id);
        Task<ApiRequestResultModel<List<Live_V_UserEntry>>> GetUserEntryListBySalaryMonth(Guid customerGID, DateTime salaryMonth);
        Task<ApiRequestResultModel<List<Live_V_UserEntry>>> GetUserEntryListByUserID(int userID);
        Task<ApiRequestResultModel<List<Live_V_UserEntry>>> GetZeroSalaryUserEntryList(Guid customerGID);
        Task<ApiRequestResultModel<List<Live_CityPartner>>> ListCityPartners(string title, bool? enabled);
        Task<ApiRequestResultModel<List<SYS_Company>>> ListCompany(Guid cityParterID);
        Task<ApiRequestResultModel<FinanceOtherService.ListCustomerRecord>> ListCustomerPage(string name, string cityPartnerTitle, bool? enabled, int? page, int? pageSize);
        Task<ApiRequestResultModel<List<Live_V_Customer>>> ListCustomers(string name, string cityPartnerTitle,string salesman, bool? enabled);
        Task<ApiRequestResultModel<List<Live_Partner>>> ListPartners(Guid Guid);
        Task<ApiRequestResultModel<List<Live_Partner>>> ListPartners(Guid cityPartnerID, int partnerType);
        Task<ApiRequestResultModel<int>> SalaryCustomerAutoComplete(Guid gid, Guid userGID, string userName, int companyID, Guid companyGID);
        Task<XfzBaseResponse<bool>> SingleTransaction(SingleTransactionRequest req);
        Task<XfzBaseResponse<EIST_Q01000405_RES>> SingleTransactionDetail(Guid businessID);
        Task<FinanceOtherService.ListPartnersRecord> ListPartner(Guid cityPartnerID, int? partnerType, bool? isBusinessPartner);
        Task<List<Live_BusinessInsuranceConfig>> BusinessInsuranceConfigListAll();
        Task<ApiRequestResultModel<List<Live_V_Customer>>> ListCustomersByCompanyID(int companyID);
    }
}
