using LZ.Model.ApiModel;
using LZ.Model.Dtos;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;

namespace LZ.ApiService.Interface
{
    public interface IAliyunCommonService
    {
        /// <summary>
        /// 获取银行卡归属地
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        Task<BankInfoResultModel?> BankCardInfo(string number);
        /// <summary>
        /// 电子证件照
        /// </summary>
        /// <param name="path"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        Task<IdPhotoModelResultModel?> IdPhoto(string path, Model.CompanyConfig config);
        /// <summary>
        /// 逆地址解析
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        Task<string> Geocoder(string location);
        /// <summary>
        /// 获取快递信息
        /// </summary>
        /// <param name="expressNo"></param>
        /// <param name="expressType"></param>
        /// <returns></returns>
        Task<string> ExpressList(string expressNo, string expressType);
        /// <summary>
        /// 订阅快递物流轨迹，状态变化后由第三方推送到 callBackUrl
        /// </summary>
        Task<bool> ExpressSubscribe(string expressNo, string expressCode, string callBackUrl, string mobile = null);
        Task<CheckbodyResultRecord> IdentityAuthentication2(string name, string cardID);
    }
}
