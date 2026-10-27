using LZ.Model;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Users.Service.Interface
{
    public interface IInjuryApplyRecordService
    {
        Task<List<Model.Live_V_InjuryApplyRecord>> List(Expression<Func<Model.Live_V_InjuryApplyRecord, bool>> exp, Model.PagedModel.UserPagedModel query, string order = "");
        Task<int> Add(Model.Live_InjuryApplyRecord model);
        Task<bool> Update(Model.Live_InjuryApplyRecord model);
        Task<bool> Exist(Expression<Func<Model.Live_InjuryApplyRecord, bool>> exp);
        Task<bool> ExistID(int id);
        Task<bool> UpdateStatus(int id, int status, string msg = "");
        Task<bool> Delete(int id);
        Task<bool> Delete(Expression<Func<Model.Live_InjuryApplyRecord, bool>> exp);
        Task<Model.Live_InjuryApplyRecord> Get(int id);
        Task<Model.Live_InjuryApplyRecord> Get(Expression<Func<Live_InjuryApplyRecord, bool>> exp);
        Task<Live_V_InjuryApplyRecord> View(Expression<Func<Live_V_InjuryApplyRecord, bool>> exp);
        Task<Model.Live_V_InjuryApplyRecord> View(int id,int injuryID, string statusArray, int moneyType = 0, int companyId = 0);

        Task<Model.Live_V_InjuryApplyRecord> LoanAdd(Model.Live_InjuryApplyRecord model);
        Task<byte[]> ExportExcel(List<Live_V_InjuryApplyRecord> list);
        Task<int> DictionarySet(Dictionary<string, object> dt);
    }
}
