using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Users.Service.Interface
{
    public interface IInjuryDocumentLabelService
    {
        Task<List<Model.Live_InjuryDocumentLabel>> List(Expression<Func<Model.Live_InjuryDocumentLabel, bool>> exp, Model.PagedModel.UserPagedModel query, string order = "");
        Task<int> Add(Model.Live_InjuryDocumentLabel model);
        Task<bool> Update(Model.Live_InjuryDocumentLabel model);
        Task<bool> ExistID(int id);

        Task<bool> ExistName(string companyName);
        Task<bool> Delete(int id);
        Task<Model.Live_InjuryDocumentLabel> Get(int id);
        Task<Model.Live_InjuryDocumentLabel> GetByName(string name);
    }
}
