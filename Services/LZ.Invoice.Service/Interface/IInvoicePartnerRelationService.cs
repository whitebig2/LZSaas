using LZ.Model;
using LZ.Model.PagedModel;
using System.Linq.Expressions;

namespace LZ.Invoice.Service.Interface
{
    public interface IInvoicePartnerRelationService
    {
        Task<List<FIN_InvoicePartnerRelation>> List(Expression<Func<FIN_InvoicePartnerRelation, bool>> exp, PagedModelBase query);

        Task<List<FIN_InvoicePartnerRelation>> List(Expression<Func<FIN_InvoicePartnerRelation, bool>> exp);

        Task<int> Add(FIN_InvoicePartnerRelation model);
        Task<int> Add(List<FIN_InvoicePartnerRelation> models);

        Task<bool> Update(FIN_InvoicePartnerRelation model);
        Task<bool> Delete(int id);
        Task<bool> Delete(Expression<Func<FIN_InvoicePartnerRelation, bool>> exp);
        Task<bool> Exist(int id);
        Task<bool> Exist(Expression<Func<FIN_InvoicePartnerRelation, bool>> exp);

        Task<FIN_InvoicePartnerRelation> Get(int id);
        Task<FIN_InvoicePartnerRelation> Get(Expression<Func<FIN_InvoicePartnerRelation, bool>> exp);
    }
}
