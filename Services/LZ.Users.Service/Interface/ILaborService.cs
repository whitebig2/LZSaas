using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace LZ.Users.Service.Interface
{
    public interface ILaborService
    {
        Task<Model.Live_Labor> Login(string openid,int companyId);
        Task<List<Model.Live_Labor>> List(Expression<Func<Model.Live_Labor, bool>> exp, Model.PagedModel.UserPagedModel query);
        Task<Model.Live_Labor> Info(int uid);
        Task<int> Add(Model.Live_Labor model);
        Task<bool> Update(Model.Live_Labor model);
        Task<bool> Delete(int id);
        Task<bool> Exist(int id);
        Task<bool> ExistName(string name);
        Task<bool> Set(int id, bool enabled);
        Task<Model.Live_Labor> Get(int id);
        Task<Model.Live_LaborOpenIds> GetOpenID(string openid);
        Task<int> ChangePicture(int uid, string picture);
    }
}
