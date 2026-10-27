using LZ.Model.PagedModel;
using LZ.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Users.Service.Interface
{
    public interface IInjuryElectronicDocumentsService
    {
        Task<List<Model.Live_InjuryElectronicDocuments>> List(Expression<Func<Model.Live_InjuryElectronicDocuments, bool>> exp, Model.PagedModel.UserPagedModel query, string order = "");
        Task<int> Add(Model.Live_InjuryElectronicDocuments model);
        Task<bool> Update(Model.Live_InjuryElectronicDocuments model);
        Task<bool> ExistID(int id);
        Task<bool> Delete(int id);
        Task<Model.Live_InjuryElectronicDocuments> Get(int id);
        Task<Model.Live_InjuryElectronicDocuments> GetByLoan(int id);

    }
}
