using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LZ.Model.PagedModel;

namespace LZ.Basic.Service.Interface
{
    public interface IInsuranceEmployeeService
    {
        Task<List<Model.Live_InsuranceEmployee>> List(Expression<Func<Model.Live_InsuranceEmployee, bool>> exp, Model.PagedModel.BasicPagedModel query, string order = "");
        Task<List<Model.Live_V_InsuranceEmployee>> ListView(Expression<Func<Model.Live_V_InsuranceEmployee, bool>> exp, Model.PagedModel.BasicPagedModel query, string order = "");
        Task<int> Add(Model.Live_InsuranceEmployee model);

        Task<int> Adds(List<Model.Live_InsuranceEmployee> models);
        Task<bool> Update(Model.Live_InsuranceEmployee model);
        Task<bool> Update(List<Model.Live_InsuranceEmployee> list);
        Task<bool> Exist(Expression<Func<Model.Live_InsuranceEmployee, bool>> exp);
        Task<bool> ExistID(int id);
        Task<bool> Delete(int id);
        Task<bool> ChangeProcessStatus(int id, int status);
        Task<Model.Live_InsuranceEmployee> Get(int id);

        Task<Model.Live_InsuranceEmployee> Get(Expression<Func<Model.Live_InsuranceEmployee, bool>> exp);

        byte[] Export(List<Model.Live_V_InsuranceEmployee> list);

        byte[] CreateDefaultExcel();
    }
}
