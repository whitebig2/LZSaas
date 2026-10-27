using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LZ.Model;
using LZ.Model.PagedModel;

namespace LZ.Basic.Service.Interface
{
    public interface IInsuranceSchemeService
    {
        Task<List<Model.Live_InsuranceScheme>> List(Expression<Func<Model.Live_InsuranceScheme, bool>> exp, BasicPagedModel query, string order = "");
        Task<List<Model.Live_V_InsuranceScheme>> ListV(Expression<Func<Model.Live_V_InsuranceScheme, bool>> exp, BasicPagedModel query, string order = "");
        Task<int> Add(Model.Live_InsuranceScheme model);

        Task<int> Adds(List<Model.Live_InsuranceScheme> models);
        Task<bool> Update(Model.Live_InsuranceScheme model);
        Task<bool> Exist(Expression<Func<Model.Live_InsuranceScheme, bool>> exp);
        Task<bool> ExistID(int id);
        Task<bool> Delete(int id);


        Task<Model.Live_InsuranceEmployeeBill> CalculationModel(int id);
        Task<Model.Live_InsuranceScheme> Get(int id);
        Task<Model.Live_V_InsuranceScheme> GetView(int id);
        Task<byte[]> ExportExcel(List<Live_V_InsuranceScheme> list);
    }
}
