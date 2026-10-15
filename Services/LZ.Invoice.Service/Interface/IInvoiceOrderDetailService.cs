using LZ.Model;
using LZ.Model.PagedModel;
using System.Linq.Expressions;

namespace LZ.Invoice.Service.Interface
{
    public interface IInvoiceOrderDetailService
    {
        Task<List<FIN_InvoiceOrderDetail>> List(Expression<Func<FIN_InvoiceOrderDetail, bool>> exp, PagedModelBase query);

        Task<List<FIN_InvoiceOrderDetail>> List(Expression<Func<FIN_InvoiceOrderDetail, bool>> exp);

        Task<int> Add(FIN_InvoiceOrderDetail model);
        Task<int> Add(List<FIN_InvoiceOrderDetail> models);

        Task<bool> Update(FIN_InvoiceOrderDetail model);
        Task<bool> Delete(int id);
        Task<bool> Delete(Expression<Func<FIN_InvoiceOrderDetail, bool>> exp);
        Task<bool> Exist(int id);
        Task<bool> Exist(Expression<Func<FIN_InvoiceOrderDetail, bool>> exp);

        Task<FIN_InvoiceOrderDetail> Get(int id);
        Task<FIN_InvoiceOrderDetail> Get(Expression<Func<FIN_InvoiceOrderDetail, bool>> exp);
    }
}
