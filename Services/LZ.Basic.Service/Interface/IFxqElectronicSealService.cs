using LZ.Model;
using LZ.Model.PagedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Basic.Service.Interface
{
    public interface IFxqElectronicSealService
    {
        Task<int> Add(Live_FxqElectronicSeal model);
        Task<bool> Delete(Guid id);
        Task<bool> Exist(Guid id);
        Task<Live_FxqElectronicSeal> Get(Guid id);
        Task<Live_FxqElectronicSeal> Get(Expression<Func<Live_FxqElectronicSeal, bool>> exp);
        Task<List<Live_FxqElectronicSeal>> List(Expression<Func<Live_FxqElectronicSeal, bool>> exp, PagedModelBase query);
        Task<bool> Update(Live_FxqElectronicSeal model);
    }
}
