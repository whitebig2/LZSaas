using LZ.Model;
using LZ.Model.PagedModel;
using System.Linq.Expressions;

namespace LZ.Invoice.Service.Interface
{
    public interface IInvoiceConfigService
    {
        Task<List<FIN_InvoiceConfig>> List(Expression<Func<FIN_InvoiceConfig, bool>> exp, PagedModelBase query);

        Task<List<FIN_InvoiceConfig>> List(Expression<Func<FIN_InvoiceConfig, bool>> exp);

        Task<int> Add(FIN_InvoiceConfig model);

        Task<bool> Update(FIN_InvoiceConfig model);
        Task<bool> Delete(int id);
        Task<bool> Exist(int id);
        Task<bool> Exist(Expression<Func<FIN_InvoiceConfig, bool>> exp);
        Task<FIN_InvoiceConfig> Get(int id);
        Task<FIN_InvoiceConfig> Get(Expression<Func<FIN_InvoiceConfig, bool>> exp);
        Task<List<FIN_InvoiceConfig>> ListView(Expression<Func<FIN_InvoiceConfig, bool>> exp);
        Task<FIN_InvoiceConfig> GetView(Expression<Func<FIN_InvoiceConfig, bool>> exp);
    }
}
