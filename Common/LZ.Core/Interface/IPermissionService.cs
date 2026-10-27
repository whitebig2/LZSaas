using LZ.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LZ.Model;

namespace LZ.Core.Interface
{
    public interface IPermissionService
    {
        Task<bool> ManagePermission(string menu,string operates,int uid, PlatformType type);
        Task<List<ManageUserRoles>> Roles(int uid);
        Task<List<ManageUserRoles>> CompanyRoles(int uid);
        Task<bool> CompanyLicense(string number, string action, int uid);
        Task<List<bool>> CompanyLicenseList(string number, string action, int uid);
        Task<bool> ManageLicense(string number, string action, int uid);
        Task<List<bool>> ManageLicenseList(string number, string action, int uid);
        Task DeleteRoles(int uid);
        Task DeleteCompanyRoles(int uid);
    }
}
