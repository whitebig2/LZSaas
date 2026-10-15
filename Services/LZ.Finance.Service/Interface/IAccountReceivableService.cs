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
    public interface IAccountReceivableService
    {
        Task<bool> Add(Live_AccountReceivableExtension model);
        Task<bool> Add(List<Live_AccountReceivableExtension> models);
        Task<bool> AddNav(Live_AccountReceivableExtension model);
        Task<bool> AddNav(List<Live_AccountReceivableExtension> models);
        Task<bool> DeleteNav(Live_AccountReceivableExtension model);
        Task<Live_AccountReceivableExtension> Get(Expression<Func<Live_AccountReceivableExtension, bool>> exp);
        Task<Live_AccountReceivableExtension> GetNav(Expression<Func<Live_AccountReceivableExtension, bool>> exp);
        Task<List<Live_AccountReceivableExtension>> PageList(Expression<Func<Live_AccountReceivableExtension, bool>> exp, PagedModelBase query);
        Task<bool> UpdateNav(Live_AccountReceivableExtension model);
        Task<List<Live_AccountReceivableExtension>> List(Expression<Func<Live_AccountReceivableExtension, bool>> exp, PagedModelBase query);
        byte[] ExportExcel(List<Live_AccountReceivableExtension> list);

    }
}
