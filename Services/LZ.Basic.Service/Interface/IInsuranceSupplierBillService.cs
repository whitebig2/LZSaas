using LZ.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Basic.Service.Interface
{
    public interface IInsuranceSupplierBillService
    {
        Task<List<Model.Live_V_InsuranceSupplierBill>> List(Expression<Func<Model.Live_V_InsuranceSupplierBill, bool>> exp, Model.PagedModel.BasicPagedModel query, string order = "");

        Task<int> Add(Model.Live_InsuranceSupplierBill model);

        Task<int> Adds(List<Model.Live_InsuranceSupplierBill> models);
        Task<bool> Update(Model.Live_InsuranceSupplierBill model);
        Task<bool> Exist(Expression<Func<Model.Live_InsuranceSupplierBill, bool>> exp);
        Task<bool> ExistID(int id);
        Task<bool> ExistCardID(int? companyID, DateTime date);
        Task<bool> Delete(int id);
        Task<Model.Live_InsuranceSupplierBill> Get(int id);

        Task<int> CheckAndSaveCompany(List<Live_Settlement> settles);
        Task<int> DictionarySet(Dictionary<string, object> dt);
        Task<(List<Live_InsuranceSupplierBill> list, string msg)> BatchEmployeeCreate(DateTime date, int cid);
        Task<byte[]> SupplierBillExcel(Model.Live_InsuranceSupplierBill model);
    }
}
