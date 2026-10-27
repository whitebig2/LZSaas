using System.Linq.Expressions;
using LZ.Model.PagedModel;

namespace LZ.Company.Service.Interface
{
    public interface ICompanyDictionaryService
    {
        Task<List<Model.SYS_CompanyDictionary>> List(Expression<Func<Model.SYS_CompanyDictionary, bool>> exp, PagedModelBase query);

        Task<int> Add(Model.SYS_CompanyDictionary model);

        Task<bool> Update(Model.SYS_CompanyDictionary model);
        Task<bool> Delete(Guid id, int companyId);
        Task<bool> Exist(int id);
        Task<bool> Set(int id, bool enabled);
        Task<Model.SYS_CompanyDictionary> Get(int id);
        Task<Model.SYS_CompanyDictionary> Get(Guid id);
    }
}
