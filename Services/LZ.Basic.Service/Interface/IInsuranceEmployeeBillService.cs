using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LZ.Model.PagedModel;

namespace LZ.Basic.Service.Interface
{
    public interface IInsuranceEmployeeBillService
    {
        Task<List<Model.Live_V_InsuranceEmployeeBill>> List(Expression<Func<Model.Live_V_InsuranceEmployeeBill, bool>> exp, Model.PagedModel.BasicPagedModel query, string order = "");

        Task<int> Add(Model.Live_InsuranceEmployeeBill model);

        Task<int> Adds(List<Model.Live_InsuranceEmployeeBill> models);
        Task<bool> Update(Model.Live_InsuranceEmployeeBill model);
        Task<bool> Exist(Expression<Func<Model.Live_InsuranceEmployeeBill, bool>> exp);
        Task<bool> ExistID(int id);
        Task<bool> ExistCardID(string IdcardNo, DateTime date);
        Task<bool> Delete(int id);
        Task<Model.Live_V_InsuranceEmployeeBill> Get(int id);

        byte[] Export(List<Model.Live_V_InsuranceEmployeeBill> list);
        Task<string> BatchEmployeeCreate(List<Model.Live_V_InsuranceEmployee> Employees,DateTime date);
        public string ExportToExcel(List<Model.Live_V_InsuranceEmployeeBill> list,string fileName, string filePath, int firstRow = 0, int columns = 10, int height = 800);
    }
}
