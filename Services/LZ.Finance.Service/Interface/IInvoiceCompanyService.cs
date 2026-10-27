using LZ.Model;
using LZ.Model.PagedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Finance.Service.Interface
{
    public interface IInvoiceCompanyService 
    {

        Task<List<Model.Live_InvoiceCompany>> PageList(Expression<Func<Model.Live_InvoiceCompany, bool>> exp, PagedModelBase query);

        Task<List<Model.Live_InvoiceCompany>> List(Expression<Func<Model.Live_InvoiceCompany, bool>> exp);

        Task<int> Add(Model.Live_InvoiceCompany model);

        Task<bool> Update(Model.Live_InvoiceCompany model);
        Task<bool> Delete(Guid id);

        Task<Model.Live_InvoiceCompany> Get(Guid id);
        Task<Live_InvoiceCompany> Get(Expression<Func<Live_InvoiceCompany, bool>> exp);
        Task<bool> Exist(Expression<Func<Live_InvoiceCompany, bool>> exp);
    }
}
