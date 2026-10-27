using LZ.Model;
using LZ.Model.PagedModel;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LZ.Bank.Service.Interface
{
    public interface ICzyPayOrderService
    {
        Task<bool> AddNav(Czy_PayOrderExtension model);
        Task<bool> DeleteNav(Guid id);
        Task<bool> Exist(Expression<Func<Czy_PayOrder, bool>> exp);
        Task<bool> ExistDetail(Expression<Func<Czy_PayOrderDetail, bool>> exp);
        Task<Czy_PayOrder> Get(Expression<Func<Czy_PayOrder, bool>> exp);
        Task<Czy_PayOrderExtension> GetNav(Expression<Func<Czy_PayOrderExtension, bool>> exp);
        Task<List<Czy_PayOrder>> List(Expression<Func<Czy_PayOrder, bool>> exp);
        Task<List<Czy_PayOrderExtension>> ListNav(Expression<Func<Czy_PayOrderExtension, bool>> exp);
        Task<List<Czy_PayOrder>> PageList(Expression<Func<Czy_PayOrder, bool>> exp, PagedModelBase query);
        Task<List<Czy_PayOrderDetail>> PageDetailList(Expression<Func<Czy_PayOrderDetail, bool>> exp, PagedModelBase query);
        Task<bool> Update(Czy_PayOrder model);
        Task<bool> UpdateNav(Czy_PayOrderExtension model);
        Task<bool> UpdatePushTime(Guid id, DateTime pushTime);
    }
}
