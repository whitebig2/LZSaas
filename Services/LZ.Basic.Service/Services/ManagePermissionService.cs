using LZ.Basic.Service.Interface;
using LZ.Core.Interface;
using LZ.Core.Redis;
using LZ.Model;
using SqlSugar;
using SqlSugar.IOC;

namespace LZ.Basic.Service.Services
{
    public class ManagePermissionService(IPermissionService permissionService) : IManagePermissionService, IScopedDependency
    {
        /// <summary>
        /// 判断是否存在该权限
        /// </summary>
        /// <param name="number"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public async Task<bool> License(string number, string action, int uid)
        {
            var models = await permissionService.Roles(uid);
            return models.Exists(it => it.Actions != null && it.Actions.Contains(action) && it.PermissionId == number);
        }

        /// <summary>
        /// 判断是否存在该权限
        /// </summary>
        /// <param name="number"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public async Task<List<bool>> LicenseList(string number, string action, int uid)
        {
            var models = await permissionService.Roles(uid);
            string[] ops = action.Split(',');
            List<bool> b = new List<bool>();
            foreach (var op in ops)
            {
                b.Add(models.Exists(it => it.Actions != null && it.PermissionId == number && it.Actions.Contains(op)));
            }
            return b;
        }
    }
}
