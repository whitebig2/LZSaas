using System.Linq.Expressions;
using LZ.Model;
using LZ.Model.PagedModel;

namespace LZ.Customer.Service.Interface
{
    public interface ICustomerJobsService
    {
        Task<List<Model.Live_V_CustomerJobs>> List(Expression<Func<Model.Live_V_CustomerJobs, bool>> exp);

        Task<int> Add(Model.Live_CustomerJobs models);
        Task<Live_V_CustomerJobs> Get(Expression<Func<Live_V_CustomerJobs, bool>> exp);
        Task<bool> Exist(Expression<Func<Live_V_CustomerJobs, bool>> exp);
        /// <summary>
        /// 获取客户默认工种
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        Task<Live_V_CustomerJobs> IsDefault(Expression<Func<Live_V_CustomerJobs, bool>> exp);
        Task<bool> Delete(int id, int jobid);
        Task<int> UpdateDefault(int id, int jobid, bool isDefault);
    }
}
