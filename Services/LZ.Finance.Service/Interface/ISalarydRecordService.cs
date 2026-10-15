//using LZ.Model;
//using LZ.Model.PagedModel;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Text;
//using System.Threading.Tasks;

//namespace LZ.Finance.Service.Interface
//{
//    public interface ISalarydRecordService
//    {
//        Task<bool> Delete(Expression<Func<Live_SalaryRecord, bool>> exp);
//        Task<Live_SalaryRecord> Get(Expression<Func<Live_SalaryRecord, bool>> exp, string strOrderByFileds = null);
//        Task<Live_V_SalaryRecord> GetView(Expression<Func<Live_V_SalaryRecord, bool>> exp, string strOrderByFileds = null);
//        Task<List<Live_V_SalaryRecord>> ListView(Expression<Func<Live_V_SalaryRecord, bool>> exp);
//        Task<List<Live_V_SalaryRecord>> PageList(Expression<Func<Live_V_SalaryRecord, bool>> exp, PagedModelBase query);
//        Task<bool> Update(Live_SalaryRecord model);
//        void VerifyPendingSend(Live_SalaryRecord model);
//    }
//}
