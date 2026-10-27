using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LZ.Model.PagedModel;

namespace LZ.Basic.Service.Interface
{
    public interface IInsuranceSupplierService
    {
        Task<List<Model.Live_V_InsuranceSupplier>> List(Expression<Func<Model.Live_V_InsuranceSupplier, bool>> exp, BasicPagedModel query, string order = "");
        Task<int> Add(Model.Live_InsuranceSupplier model);
        Task<bool> Update(Model.Live_InsuranceSupplier model);
        Task<bool> Exist(Expression<Func<Model.Live_InsuranceSupplier, bool>> exp);
        Task<bool> ExistID(int id);
        Task<bool> Delete(int id);
        Task<int> Count(Expression<Func<Model.Live_InsuranceSupplier, bool>> exp);
        Task<Model.Live_InsuranceSupplier> Get(int id);
    }
}
