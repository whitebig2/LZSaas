using LZ.Model;
using LZ.Model.PagedModel;
using System.Linq.Expressions;

namespace LZ.Invoice.Service.Interface
{
    public interface IInvoiceInfoService
    {
        Task<List<FIN_InvoiceInfo>> List(Expression<Func<FIN_InvoiceInfo, bool>> exp, PagedModelBase query);

        Task<List<FIN_InvoiceInfo>> List(Expression<Func<FIN_InvoiceInfo, bool>> exp);

        Task<int> Add(FIN_InvoiceInfo model);

        Task<bool> Update(FIN_InvoiceInfo model);
        Task<bool> Delete(Guid id);
        Task<bool> Exist(Guid id);
        Task<bool> Exist(Expression<Func<FIN_InvoiceInfo, bool>> exp);

        Task<FIN_InvoiceInfo> Get(Guid id);
        Task<FIN_InvoiceInfo> Get(Expression<Func<FIN_InvoiceInfo, bool>> exp);
    }
}
