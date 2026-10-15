using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;
using System.Linq.Expressions;

namespace LZ.Users.Service.Interface
{
    public interface IPersonnelService
    {
        Task<Model.Live_Personnel> Login(string phone, int companyId);
        Task<List<Model.Live_V_Personnel>> List(Expression<Func<Model.Live_V_Personnel, bool>> exp, Model.PagedModel.UserPagedModel query);
        Task<int> Add(Model.Live_Personnel model);
        Task<int> AddOpenID(Model.Live_PersonnelOpenIds model);
        Task<bool> Update(Model.Live_Personnel model);
        Task<bool> UpdatePrivateQrcode(int userId, string qrcode);
        Task<bool> UpdateClientQrcode(int userId, string qrcode);
        Task<bool> UpdateJDYNumber(List<Model.Live_Personnel> models);
        Task<bool> Delete(int id, int companyId = 0);
        Task<bool> Exist(int id, int companyId = 0);
        Task<bool> Exist(Expression<Func<Model.Live_Personnel, bool>> exp);
        Task<bool> ExistOpenID(string openId, int uid, int clientId = 0, string type = "");
        Task<bool> Set(int id, bool enabled, int companyId = 0);
        Task<bool> SetBindMP(int id, bool enabled);
        Task<Model.Live_Personnel> Get(int id, int companyId = 0);
        Task<Model.Live_V_Personnel> GetView(int id, int companyId = 0);
        Task<Model.Live_Personnel> Get(Expression<Func<Model.Live_Personnel, bool>> exp);
        Task<Model.Live_PersonnelOpenIds> GetOpenID(string openid);
        Task<Model.Live_PersonnelOpenIds> GetOpenID(int uid,int clientId);
        Task<int> ChangePicture(int uid, string picture);
        Task<Model.Live_Personnel> ExistPhone(string phone, int companyId=0);
    }
}
