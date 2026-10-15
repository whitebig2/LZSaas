using LZ.Model;
using LZ.Model.Dtos.SalarySlip;
using LZ.Model.Model;
using LZ.Model.PagedModel;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Finance.Service.Interface
{
    public interface ISalaryCustomerService 
    {

        //Task<List<Model.Live_V_SalaryCompany>> List(Expression<Func<Model.Live_V_SalaryConfig, bool>> exp, PagedModelBase query);

        Task<List<Model.Live_SalaryCustomer>> List(Expression<Func<Model.Live_SalaryCustomer, bool>> exp);

        //Task<int> Add(Model.Live_SalaryCustomer model);

        Task<bool> Update(Model.Live_SalaryCustomer model);
        Task<bool> Delete(Guid id);
        Task<bool> Exist(Guid id);

        Task<Model.Live_SalaryCustomer> Get(Guid id);
        //Task<bool> Add(Live_SalaryCustomer model, List<Live_SalaryRecord> records);
        //Task<bool> Add(List<Live_SalaryCustomer> models, List<Live_SalaryRecord> records);
        Task<bool> AddSpecialDeduction(List<Live_SalarySpecialDedcution> dedcutions);
        Task<List<Live_SalaryCustomer>> QueryList(Expression<Func<Live_SalaryCustomer, bool>> exp, string strOrderByFileds = null);
        Task<bool> AddNav(Live_SalaryCustomerExtension model);
        Task<bool> AddNav(List<Live_SalaryCustomerExtension> models);
        Task<Live_SalaryCustomerExtension> GetNav(Expression<Func<Live_SalaryCustomerExtension, bool>> exp);
        Task<List<Live_SalaryCustomerExtension>> QueryNavList(Expression<Func<Live_SalaryCustomerExtension, bool>> exp, string strOrderByFileds = null);
        Task<List<Live_V_SalaryRecord>> QuerySalaryRecordViewList(Expression<Func<Live_V_SalaryRecord, bool>> exp, string strOrderByFileds = null);
        Task<List<Live_V_SalarySpecialDedcution>> QuerySpecialDeductionList(Expression<Func<Live_V_SalarySpecialDedcution, bool>> exp);
        Task<bool> UpdateNav(Live_SalaryCustomerExtension model);
        Task<bool> DeleteNav(Live_SalaryCustomerExtension model);
        Task<Live_V_SalaryCustomerExtension> GetNavView(Expression<Func<Live_V_SalaryCustomerExtension, bool>> exp);
        Task<Live_SalaryCustomerExtension> GetTransNav(Expression<Func<Live_SalaryCustomerExtension, bool>> exp);
        Task<bool> UpdateTransNav(List<Live_SalaryCustomerExtension> models);
        Task<List<Live_SalaryCustomerExtension>> ListTransNav(Expression<Func<Live_SalaryCustomerExtension, bool>> exp);

        Task<List<Live_V_SalaryCustomer>> PageList(Expression<Func<Live_V_SalaryCustomer, bool>> exp, PagedModelBase query);
        Task<bool> UpdateTransNav(Live_SalaryCustomerExtension model);
        Task<List<Dictionary<int, string>>> ListSalaryRecordSlipRecord(Guid id);

        Task<List<UserSalarySlip>> ListUserSalaryRecordSlipRecord(Expression<Func<Live_V_SalaryRecordSlipRecord, bool>> exp);
        Task<bool> BatchSendSuccess(Live_SalaryCustomerExtension model);
        Task<bool> BatchSendFail(Live_SalaryCustomerExtension model);
        Task<Live_V_SalaryCustomer> GetView(Guid id);

        byte[] Export(List<Model.ExcelTemplateModel.SalaryRecordExportExcelTemplate> list);
        Task<bool> SetCustomerPaid(Guid id);
        SalayCustomerCalculateTimes SalaryCustomerCalculateTimes();
        Task<bool> SetReviewRejected(Guid id);
        Task<bool> SetPendingPush(Guid id);
        Task<bool> SetPendingReview(Guid id);
        Task<bool> SetPendingSend(Guid id);
        Task<bool> SetLocked(Guid id,bool isLocked);
        bool ResetSalaryCustomerCalculateTimes();
        Task<Live_SalaryCustomer> Get(Expression<Func<Live_SalaryCustomer, bool>> exp);
        Task<bool> RollbackToPendingSend(Guid id);

        Task<int> DictionarySet(Dictionary<string, object> dt);
    }
}
