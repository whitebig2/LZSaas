using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;
using System.Linq.Expressions;

namespace LZ.Users.Service.Interface
{
    public interface IUserAddressService
    {
        Task<List<Model.Shop_UserAddress>> List(Expression<Func<Model.Shop_UserAddress, bool>> exp, Model.PagedModel.UserPagedModel query);
        Task<int> Add(Model.Shop_UserAddress model);
        Task<bool> Update(Model.Shop_UserAddress model);
        Task<bool> Exist(Expression<Func<Model.Shop_UserAddress, bool>> exp);
        Task<Model.Shop_UserAddress> Get(int id);
        Task<Model.Shop_UserAddress> Get(Expression<Func<Model.Shop_UserAddress, bool>> exp);
    }
}
