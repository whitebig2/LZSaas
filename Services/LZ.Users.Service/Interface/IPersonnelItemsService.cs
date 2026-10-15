using LZ.Model;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace LZ.Users.Service.Interface
{
    public interface IPersonnelItemsService
    {
        Task<List<Model.Live_V_PersonnelItems>> List(Expression<Func<Model.Live_V_PersonnelItems, bool>> exp, Model.PagedModel.UserPagedModel query);

        Task<List<Model.Live_PersonnelItems>> ListModel(Expression<Func<Model.Live_PersonnelItems, bool>> exp, Model.PagedModel.UserPagedModel query);
        Task<int> Add(Model.Live_PersonnelItems model);
        Task<bool> Add(List<Model.Live_PersonnelItems> models);
        Task<bool> Update(Model.Live_PersonnelItems model);
        Task<bool> Delete(int id);
        Task<int> DeleteList(List<Model.Live_PersonnelItems> list);
        Task<bool> Exist(int id, int companyId = 0);
        Task<bool> Exist(Expression<Func<Model.Live_V_PersonnelItems, bool>> exp);
        Task<bool> Set(int id, bool enabled);
        Task<bool> SetManage(int id, bool enabled);
        Task<Model.Live_V_PersonnelItems> Get(int id, int companyId = 0);
        Task<Model.Live_V_PersonnelItems> Get(Guid id);
        Task<Model.Live_V_PersonnelItems> Get(Expression<Func<Model.Live_V_PersonnelItems, bool>> exp);
        Task<Model.Live_PersonnelItems> GetInfo(Expression<Func<Model.Live_PersonnelItems, bool>> exp);
        Task<List<Live_PersonnelItems>> SaleBatchQR(string phones, int customerID);
        Task<List<Model.SalePersonnelModel>> SalePersonnelList(int cid);
        Task<bool> SaveQrcode(int id, string path);
        Task<bool> SaveJDYJson(int id, string json,string jdyId);
    }
}
