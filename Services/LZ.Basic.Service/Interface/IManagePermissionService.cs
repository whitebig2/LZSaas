namespace LZ.Basic.Service.Interface
{
    public interface IManagePermissionService
    {
        /// <summary>
        /// 判断是否存在该权限
        /// </summary>
        /// <param name="number"></param>
        /// <param name="action"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        Task<bool> License(string number, string action, int uid);
        Task<List<bool>> LicenseList(string number, string action, int uid);
    }
}
