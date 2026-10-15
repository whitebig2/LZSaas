//using LZ.Model;
//using LZ.Model.ModelExtensions;
//using LZ.Model.PagedModel;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Text;
//using System.Threading.Tasks;

//namespace LZ.Customer.Service.Interface
//{
//    public interface IInsurancePushService
//    {
//        Task<bool> AddNav(Live_InsurancePushExtension model);
//        Task<bool> DeleteNav(Live_InsurancePushExtension model);
//        Task<Live_InsurancePush> Get(Expression<Func<Live_InsurancePush, bool>> exp);
//        Task<Live_InsurancePushExtension> GetNav(Expression<Func<Live_InsurancePushExtension, bool>> exp);
//        Task<List<Live_V_InsurancePushExtension>> GetViewListNav(Expression<Func<Live_V_InsurancePushExtension, bool>> exp);
//        Task<Live_V_InsurancePushExtension> GetViewNav(Expression<Func<Live_V_InsurancePushExtension, bool>> exp);
//        Task<List<Live_InsurancePush>> List(Expression<Func<Live_InsurancePush, bool>> exp);
//        Task<List<Live_InsurancePushExtension>> ListNav(Expression<Func<Live_InsurancePushExtension, bool>> exp);
//        Task<List<Live_InsurancePush>> PageList(Expression<Func<Live_InsurancePush, bool>> exp, PagedModelBase query);
//        Task<List<Live_V_InsurancePush>> PageViewList(Expression<Func<Live_V_InsurancePush, bool>> exp, PagedModelBase query);
//        Task<bool> UpdateNav(Live_InsurancePushExtension model);
//    }
//}
