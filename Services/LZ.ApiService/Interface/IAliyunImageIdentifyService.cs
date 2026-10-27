using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;

namespace LZ.ApiService.Interface
{
    public interface IAliyunImageIdentifyService
    {
        Task<Model.ApiModel.IdcradImageResultModel?> IdcardIdentify(string imageUrl);

        Task<Model.ApiModel.BankImageResultModel?> BankIdentify(string imageUrl);

        Task<Model.ApiModel.BusinessLicenseModel?> BusinessLicense(string imageUrl);

        Task<Model.ApiModel.InvoiceModel?> Invoice(string imageUrl);
        //Task<string> Invoice(string imageUrl);
    }
}
