using LZ.Model.PagedModel;
using LZ.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Basic.Service.Interface
{
    public interface ILabelManagementService
    {
        Task<List<Model.Live_LabelManagement>> List(Expression<Func<Model.Live_LabelManagement, bool>> exp, BasicPagedModel query, string order = "");
        Task<int> Add(Model.Live_LabelManagement model);

        Task<int> Adds(List<Model.Live_LabelManagement> models);
        Task<bool> Update(Model.Live_LabelManagement model);
        Task<bool> Exist(Expression<Func<Model.Live_LabelManagement, bool>> exp);
        Task<bool> ExistID(int id);
        Task<bool> Delete(int id);
        Task<Model.Live_LabelManagement> Get(int id);
        Task<Model.Live_LabelManagement> Get(Expression<Func<Model.Live_LabelManagement, bool>> exp);
    }
}
