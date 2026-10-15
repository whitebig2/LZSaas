using System.Linq.Expressions;
using LZ.Model.PagedModel;

namespace LZ.Company.Service.Interface
{
    public interface ICompanyFunctionService
    {
        Task<List<Model.SYS_CompanyFunction>> List(Expression<Func<Model.SYS_CompanyFunction, bool>> exp, PagedModelBase query);

        Task<int> Add(List<Model.SYS_CompanyFunction> models);
    }
}
