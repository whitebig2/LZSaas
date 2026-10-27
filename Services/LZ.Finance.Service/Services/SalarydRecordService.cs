//using LZ.Finance.Service.Interface;
//using LZ.Model;
//using LZ.Model.PagedModel;
//using SqlSugar.IOC;
//using SqlSugar;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Text;
//using System.Threading.Tasks;
//using LZ.Core.Interface;
//using LZ.Core;

//namespace LZ.Finance.Service.Services
//{
//    public class SalarydRecordService : ServicesBase, ISalarydRecordService, IScopedDependency
//    {

//        public async Task<List<Live_V_SalaryRecord>> PageList(Expression<Func<Live_V_SalaryRecord, bool>> exp, PagedModelBase query)
//        {
//            var count = new RefAsync<int>();
//            var list = await   base.Context.Queryable<Live_V_SalaryRecord>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
//            query.Count = count.Value;
//            return list;
//        }

//        public async Task<List<Live_V_SalaryRecord>> ListView(Expression<Func<Live_V_SalaryRecord, bool>> exp)
//        {
//            var list = await   base.Context.Queryable<Live_V_SalaryRecord>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
//            return list;
//        }


//        public async Task<Live_SalaryRecord> Get(Expression<Func<Live_SalaryRecord, bool>> exp, string strOrderByFileds = null)
//        {
//            return await   base.Context.Queryable<Live_SalaryRecord>()
//                .OrderByIF(!string.IsNullOrWhiteSpace(strOrderByFileds), strOrderByFileds)
//                .Where(exp).FirstAsync();
//        }

//        public async Task<Live_V_SalaryRecord> GetView(Expression<Func<Live_V_SalaryRecord, bool>> exp, string strOrderByFileds = null)
//        {
//            return await   base.Context.Queryable<Live_V_SalaryRecord>()
//                .OrderByIF(!string.IsNullOrWhiteSpace(strOrderByFileds), strOrderByFileds)
//                .Where(exp).FirstAsync();
//        }

      
//        public async Task<bool> Update(Live_SalaryRecord model)
//        {
//            return await   base.Context.Updateable<Live_SalaryRecord>(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
//        }

//        public void VerifyPendingSend(Live_SalaryRecord model)
//        {
//            bool isb = true; 
//            if (model.IsCashSend&&!model.IsCashSignSuc)
//            {
//                isb = false;
//            }
//            if (model.IsDelayPayment && !model.IsDelayPaymentSuc)
//            {
//                isb = false;
//            }
//            if (model.IsFailSalary && !model.IsFailSalarySuc)
//            {
//                isb = false;
//            }
//            if (isb)
//            {
//                model.Status = Model.Enums.SalaryRecordSendStatusEnumType.待发放;   
//            }
//        }

//        public async Task<bool> Delete(Expression<Func<Live_SalaryRecord, bool>> exp)
//        {
//            return await   base.Context.Deleteable<Live_SalaryRecord>(exp).ExecuteCommandAsync() > 0;
//        }




   








//    }
//}
