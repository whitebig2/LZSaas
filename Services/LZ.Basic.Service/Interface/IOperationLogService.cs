using LZ.Model;
using LZ.Model.PagedModel;
using System.Linq.Expressions;

namespace LZ.Basic.Service.Interface
{
    public interface IOperationLogService
    {
        Task<List<SYS_OperationLog>> List(Expression<Func<SYS_OperationLog, bool>> exp, PagedModelBase query);
        Task Insert(Model.SYS_OperationLog model);
    }
}
