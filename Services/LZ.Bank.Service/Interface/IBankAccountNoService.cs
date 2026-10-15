//using LZ.Model;
//using LZ.Model.PagedModel;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Text;
//using System.Threading.Tasks;

//namespace LZ.Bank.Service.Interface
//{
//    public interface IBankAccountNoService
//    {
//        Task<int> Add(Live_BankAccountNo model);
//        Task<bool> Delete(Guid id);
//        Task<bool> Exist(Expression<Func<Live_BankAccountNo, bool>> exp);
//        Task<Live_BankAccountNo> Get(Expression<Func<Live_BankAccountNo, bool>> exp);
//        Task<List<Live_BankAccountNo>> List(Expression<Func<Live_BankAccountNo, bool>> exp);
//        Task<List<Live_BankAccountNo>> PageList(Expression<Func<Live_BankAccountNo, bool>> exp, PagedModelBase query);
//        Task<bool> Update(Live_BankAccountNo model);
//    }
//}
