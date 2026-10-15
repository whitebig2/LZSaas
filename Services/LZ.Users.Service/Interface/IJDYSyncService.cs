using LZ.Model;
using LZ.Model.Exceptions;

namespace LZ.Users.Service.Interface
{
    public interface IJDYSyncService
    {
        Task PersonnelSync(int id);

        /// <summary>
        /// 同步简道云
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
       Task UserSync(int id);
        JDYFieldModel GetJDYField(List<JDYFieldModel> list, string name);
        /// <summary>
        /// 同步简道云合同状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task UserContractSync(int id);
        /// <summary>
        /// 获取工资记录
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="company"></param>
        /// <returns></returns>
        Task<List<Dictionary<string, object>>> GetWageInfo(string cardId, CompanyModel company);
    }
}
