using SqlSugar.IOC;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Basic.Service.Interface
{
    public interface IInsuranceCompanyBillService
    {
        Task<List<Model.Live_V_InsuranceCompanyBill>> List(Expression<Func<Model.Live_V_InsuranceCompanyBill, bool>> exp, Model.PagedModel.BasicPagedModel query, string order = "");

        Task<int> Add(Model.Live_InsuranceCompanyBill model);

        Task<int> Adds(List<Model.Live_InsuranceCompanyBill> models);
        Task<bool> Update(Model.Live_InsuranceCompanyBill model);
        Task<bool> Exist(Expression<Func<Model.Live_InsuranceCompanyBill, bool>> exp);
        Task<bool> ExistID(int id);
        Task<bool> ExistCardID(int? companyID, DateTime date);
        Task<bool> Delete(int id);
        Task<Model.Live_InsuranceCompanyBill> Get(int id);
        Task<int> DictionarySet(Dictionary<string, object> dt);
        Task<(List<Model.Live_Invoice> list, string msg)> BatchEmployeeCreate(DateTime date,int cid);
        Task<byte[]> CompanyBillExcel(Model.Live_InsuranceCompanyBill model);
        byte[] PayNoticeExcel(Model.Live_InsuranceCompanyBill model);

    }
}
