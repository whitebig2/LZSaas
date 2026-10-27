using LZ.Model;
using LZ.Model.PagedModel;
using System.Linq.Expressions;

namespace LZ.Invoice.Service.Interface
{
    public interface IInvoiceCustomerService
    {
        Task<List<FIN_InvoiceCustomer>> PageList(Expression<Func<FIN_InvoiceCustomer, bool>> exp, PagedModelBase query);

        Task<List<FIN_InvoiceCustomer>> List(Expression<Func<FIN_InvoiceCustomer, bool>> exp);

        Task<int> Add(FIN_InvoiceCustomer model);

        Task<bool> Update(FIN_InvoiceCustomer model);
        Task<bool> Delete(Guid id);

        Task<FIN_InvoiceCustomer> Get(Guid id);
        Task<FIN_InvoiceCustomer> Get(Expression<Func<FIN_InvoiceCustomer, bool>> exp);
        Task<bool> Exist(Expression<Func<FIN_InvoiceCustomer, bool>> exp);
    }
}
