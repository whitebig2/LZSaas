using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;
using System.Linq.Expressions;

namespace LZ.Users.Service.Interface
{
    public interface IUserShopService
    {
        Task<List<Model.Shop_UserShop>> List(Expression<Func<Model.Shop_UserShop, bool>> exp, Model.PagedModel.UserPagedModel query);
        Task<int> Add(Model.Shop_UserShop model);
        Task<bool> Update(Model.Shop_UserShop model);
        Task<bool> Exist(Expression<Func<Model.Shop_UserShop, bool>> exp);
        Task<Model.Shop_UserShop> Get(int id);
        Task<Model.Shop_UserShop> Get(Expression<Func<Model.Shop_UserShop, bool>> exp);
    }
}
