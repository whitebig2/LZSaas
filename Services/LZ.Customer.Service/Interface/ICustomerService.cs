using System.Linq.Expressions;
using LZ.Model;
using LZ.Model.ModelExtensions;
using LZ.Model.PagedModel;

namespace LZ.Customer.Service.Interface
{
    public interface ICustomerService
    {
        Task<List<Model.Live_V_Customer>> List(Expression<Func<Model.Live_V_Customer, bool>> exp, PagedModelBase query);

        Task<List<Model.Live_Customer>> List(Expression<Func<Model.Live_Customer, bool>> exp, PagedModelBase query);

        Task<int> Add(Model.Live_Customer model);

        Task<bool> Update(Model.Live_Customer model);

        Task<bool> UpdateList(List<Model.Live_Customer> list);
        Task<bool> Delete(int id);
        Task<bool> Exist(int id);
        Task<bool> Exist(Expression<Func<Live_Customer, bool>> exp);
        Task<bool> ChangeStatus(int id, int status);
        /// <summary>
        /// 更改客户默认
        /// </summary>
        /// <param name="id"></param>
        /// <param name="workName"></param>
        /// <param name="workClass"></param>
        /// <returns></returns>
        Task<bool> ChangeWork(int id, string workName, string workClass);
        Task<Model.Live_Customer> Get(int id);
        Task<Model.Live_Customer> Get(string name);
        Task<Live_CustomerExtension> GetNav(Guid CustomerGID);
        Task<bool> UpdateNav(Live_CustomerExtension model);
        Task<Live_Customer> Get(Expression<Func<Live_Customer, bool>> exp);

        Task<Model.Model.CustomerAuthModel> GetCustomerAuth(Expression<Func<Model.Model.CustomerAuthModel, bool>> exp);
        Task<bool> UpdateCustomerAuth(Model.Model.CustomerAuthModel model);
        Task<Live_V_Customer> GetView(Expression<Func<Live_V_Customer, bool>> exp);

        Task<int> DictionarySet(Dictionary<string, object> dt);

        byte[] Export(List<Model.Live_V_Customer> list);

    }
}
