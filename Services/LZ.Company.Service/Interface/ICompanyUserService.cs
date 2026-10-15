using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace LZ.Company.Service.Interface
{
    public interface ICompanyUserService
    {
        Task<Model.SYS_CompanyUser> Login(string userName, string password);
        Task<List<Model.SYS_CompanyUser>> List(Expression<Func<Model.SYS_CompanyUser, bool>> exp, UserPagedModel query);
        Task<Model.SYS_CompanyUser> Info(int uid);
        Task<int> Add(Model.SYS_CompanyUser model);
        Task<bool> Update(Model.SYS_CompanyUser model);

        Task<Model.SYS_CompanyUser> Get(Expression<Func<Model.SYS_CompanyUser, bool>> exp);

        Task<bool> Delete(int id, int companyId);
        Task<bool> Exist(int id);
        Task<bool> ExistName(string name);
        Task<bool> ExistUserName(string name,int id);
        Task<bool> Set(int id, bool enabled);
        Task<Model.SYS_CompanyUser> Get(int id);
        Task<int> ChangePassword(int uid,string ypassword, string npassword, int companyId);
        Task<int> ChangePicture(int uid, string picture, int companyId);
        Task<bool> ExistName(string name, int companyID);
    }
}
