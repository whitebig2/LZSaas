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
    public interface IInvoiceService 
    {

        Task<List<Model.Live_V_Invoice>> PageViewList(Expression<Func<Model.Live_V_Invoice, bool>> exp, PagedModelBase query);

        Task<List<Model.Live_Invoice>> List(Expression<Func<Model.Live_Invoice, bool>> exp);

        Task<int> Add(Model.Live_Invoice model);
        Task<int> Add(List<Model.Live_Invoice> list);

        Task<bool> Update(Model.Live_Invoice model);
        Task<bool> Delete(Guid id);
        Task<bool> Exist(Guid id);

        Task<Model.Live_Invoice> Get(Guid id);
        Task<bool> AddAttachFile(Live_InvoiceAttachFile model);
        Task<bool> UpdateAttachFiles(List<Live_InvoiceAttachFile> model);
        Task<bool> DeleteAttachFile(Expression<Func<Live_InvoiceAttachFile, bool>> exp);
        Task<List<Live_InvoiceAttachFile>> ListAttachFiles(Expression<Func<Live_InvoiceAttachFile, bool>> exp);
        Task<Live_InvoiceAttachFile> GetAttachFile(Expression<Func<Live_InvoiceAttachFile, bool>> exp);
        Task<Live_V_Invoice> GetView(Expression<Func<Live_V_InvoiceExtension, bool>> exp);
        Task<bool> AddNav(Live_InvoiceExtension model);
        Task<Live_InvoiceExtension> GetNav(Expression<Func<Live_InvoiceExtension, bool>> exp);
        Task<bool> UpdateNav(Live_InvoiceExtension model);
        Task<bool> DeleteNav(Expression<Func<Live_InvoiceExtension, bool>> exp);
        Task<int> Add(List<Live_InvoiceExtension> models);
        Task<bool> Delete(Expression<Func<Live_Invoice, bool>> exp);
        Task<List<Live_Invoice>> PageList(Expression<Func<Live_Invoice, bool>> exp, PagedModelBase query);
        Task<byte[]> ExportExcel(List<Live_V_Invoice> list);
        Task<bool> AddInvoiceTaxRecords(List<Live_InvoiceTaxRecord> records);
    }
}
