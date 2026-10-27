using System.Linq.Expressions;
using LZ.Model.PagedModel;

namespace LZ.Basic.Service.Interface
{
    public interface IPersonnelFunctionService
    {
        Task<List<Model.Live_PersonnelFunction>> List(Expression<Func<Model.Live_PersonnelFunction, bool>> exp, PagedModelBase query);
        Task<List<Model.Live_PersonnelFunctionItems>> Items(Expression<Func<Model.Live_PersonnelFunctionItems, bool>> exp);

        Task<int> Add(Model.Live_PersonnelFunction model);

        Task<bool> Update(Model.Live_PersonnelFunction model);
        Task<bool> Delete(int id);
        Task<bool> Exist(int id);
        Task<bool> Set(int id, bool enabled);
        Task<Model.Live_PersonnelFunction> Get(int id);
        Task UserTypeFunction(List<Model.Live_PersonnelFunctionItems> list);
    }
}
