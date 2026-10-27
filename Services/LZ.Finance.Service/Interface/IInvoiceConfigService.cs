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
    public interface IInvoiceConfigService 
    {

        Task<List<Model.Live_V_InvoiceConfig>> List(Expression<Func<Model.Live_V_InvoiceConfig, bool>> exp, PagedModelBase query);

        Task<List<Model.Live_InvoiceConfig>> List(Expression<Func<Model.Live_InvoiceConfig, bool>> exp);

        Task<int> Add(Model.Live_InvoiceConfig model);

        Task<bool> Update(Model.Live_InvoiceConfig model);
        Task<bool> Delete(Guid id);
        Task<bool> Exist(Guid id);

        Task<Model.Live_InvoiceConfig> Get(Guid id);
        Task<Live_InvoiceConfig> Get(Expression<Func<Live_InvoiceConfig, bool>> exp);
        Task<List<Live_V_InvoiceConfig>> ListView(Expression<Func<Live_V_InvoiceConfig, bool>> exp);
        Task<Live_V_InvoiceConfig> GetView(Expression<Func<Live_V_InvoiceConfig, bool>> exp);
    }
}
