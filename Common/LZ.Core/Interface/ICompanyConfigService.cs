using LZ.Model.Enums;
using LZ.Model;

namespace LZ.Core.Interface
{
    public interface ICompanyConfigService
    {

        /// <summary>
        /// 获得公司配置
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        Task<Model.CompanyModel> Get(Guid? id);
    }
}
