using LZ.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LZ.Model;
using LZ.Core.Interface;
using LZ.Core.Redis;
using SqlSugar;
using SqlSugar.IOC;

namespace LZ.Core.Services
{
    public class PermissionService(IRedisService redisService) : ServicesBase<SYS_ManageRoles>, IPermissionService, IScopedDependency
    {

        /// <summary>
        /// 权限验证
        /// </summary>
        /// <param name="menus"></param>
        /// <param name="operates"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public async Task<bool> ManagePermission(string menus,string operates,int uid,PlatformType type)
        {
            List<ManageUserRoles> roles = null;
            roles = type==PlatformType.Admin? await Roles(uid):await CompanyRoles(uid);
            if (roles == null)
            {
                return false;
            }
            if (!string.IsNullOrEmpty(operates))
            {
                string[] ms = menus.Split(',');
                foreach (string m in ms)
                {
                   var ex = roles.Exists(it => it.PermissionId == m);
                    if (ex) return ex;
                }
            }
            else
            {
                string[] ops = operates.Split(',');
                foreach (string op in ops)
                {
                    var ex = roles.Exists(it => it.Actions != null && it.Actions.Contains(op) && it.PermissionId == menus);
                    if (ex) return ex;
                }
            }
            return false;
        }
        public async Task<List<ManageUserRoles>> Roles(int uid)
        {
            var um = await redisService.GetAsync<List<ManageUserRoles>>(string.Format(CoreRedisConstants.ManageUserRoles, uid));
            if (um == null)
            {
                um = await base.Context.Queryable<SYS_ManageRoles, SYS_RolesFunciton, SYS_FunctionMenu>((r, f, m) => new object[] {
        JoinType.Left,r.RGID==f.RGID,JoinType.Left,f.FGID==m.FGID}).Where(r => r.UserID == uid).GroupBy((r, f, m) => new { f.OperateNumber, m.Number }).Select((r, f, m) => new Model.ManageUserRoles { PermissionId = m.Number, Actions = f.OperateNumber }).ToListAsync();
                if (um != null)
                {
                    await redisService.SetAsync(string.Format(CoreRedisConstants.ManageUserRoles, uid), um);
                    return um;
                }
                return null;
            }
            else
            {
                return um;
            }
        }
        public async Task DeleteRoles(int uid)
        {
            await redisService.DeleteAsync(string.Format(CoreRedisConstants.ManageUserRoles, uid));
        }
            public async Task<List<ManageUserRoles>> CompanyRoles(int uid)
        {
            var um = await redisService.GetAsync<List<ManageUserRoles>>(string.Format(CoreRedisConstants.CompanyUserRoles, uid));
            if (um == null)
            {
                um = await base.Context.Queryable<SYS_CompanyUserRoles, SYS_RolesFunciton, SYS_FunctionMenu>((r, f, m) => new object[] {
        JoinType.Left,r.RGID==f.RGID,JoinType.Left,f.FGID==m.FGID}).Where(r => r.UserID == uid).GroupBy((r, f, m) => new { f.OperateNumber, m.Number }).Select((r, f, m) => new Model.ManageUserRoles { PermissionId = m.Number, Actions = f.OperateNumber }).ToListAsync();
                if (um != null)
                {
                    await redisService.SetAsync(string.Format(CoreRedisConstants.CompanyUserRoles, uid), um);
                    return um;
                }
                return null;
            }
            else
            {
                return um;
            }
        }
        public async Task DeleteCompanyRoles(int uid)
        {
            await redisService.DeleteAsync(string.Format(CoreRedisConstants.CompanyUserRoles, uid));
        }
        /// <summary>
        /// 判断是否存在该权限
        /// </summary>
        /// <param name="number"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public async Task<bool> CompanyLicense(string number, string action, int uid)
        {
            var models =await CompanyRoles(uid);
            return models.Exists(it => it.Actions != null && it.Actions.Contains(action) && it.PermissionId == number);
        }

        /// <summary>
        /// 判断是否存在该权限
        /// </summary>
        /// <param name="number"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public async Task<List<bool>> CompanyLicenseList(string number, string action, int uid)
        {
            var models = await CompanyRoles(uid);
            string[] ops = action.Split(',');
            List<bool> b = new List<bool>();
            foreach (var op in ops)
            {
                b.Add(models.Exists(it => it.Actions != null && it.PermissionId == number && it.Actions.Contains(op)));
            }
            return b;
        }
        /// <summary>
        /// 判断是否存在该权限
        /// </summary>
        /// <param name="number"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public async Task<bool> ManageLicense(string number, string action, int uid)
        {
            var models = await Roles(uid);
            return models.Exists(it => it.Actions != null && it.Actions.Contains(action) && it.PermissionId == number);
        }

        /// <summary>
        /// 判断是否存在该权限
        /// </summary>
        /// <param name="number"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public async Task<List<bool>> ManageLicenseList(string number, string action, int uid)
        {
            var models = await Roles(uid);
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
