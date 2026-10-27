using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace LZ.Basic.Service.Interface
{
    public interface IManageUserService
    {
        Task<Model.SYS_ManageUser> Login(string userName, string password);
        Task<List<Model.SYS_ManageUser>> List(Expression<Func<Model.SYS_ManageUser, bool>> exp, UserPagedModel query);
        Task<Model.SYS_ManageUser> Info(int uid);
        Task<int> Add(Model.SYS_ManageUser model);
        Task<bool> Update(Model.SYS_ManageUser model);
        Task<bool> Delete(int id);
        Task<bool> Exist(int id);
        Task<bool> ExistName(string name);
        Task<bool> ExistName(string name,int id);
        Task<bool> Set(int id, bool enabled);
        Task<Model.SYS_ManageUser> Get(int id);
        Task<int> ChangePassword(int uid,string ypassword, string npassword);
        Task<int> ChangePicture(int uid, string picture);
    }
}
