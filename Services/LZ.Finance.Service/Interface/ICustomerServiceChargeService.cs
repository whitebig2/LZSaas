using LZ.Model;
using LZ.Model.PagedModel;
using MathNet.Numerics.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Finance.Service.Interface
{
    public interface ICustomerServiceChargeService
    {
        /// <summary>
        /// 每个月1号批量生成预估费用
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        Task<bool> Batch(DateTime date);
        Task<Live_V_CustomerServiceCharge> Get(Expression<Func<Live_V_CustomerServiceCharge, bool>> exp);
        Task<Live_CustomerServiceCharge>  Statistics(Expression<Func<Live_V_CustomerServiceCharge, bool>> exp);
        Task<List<Live_V_CustomerServiceCharge>> List(Expression<Func<Live_V_CustomerServiceCharge, bool>> exp, PagedModelBase query);
        /// <summary>
        /// 更改工资发放时间
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> ChangeSendTime(Guid id);
        /// <summary>
        /// 根据客户工资客户ID生成服务费记录，如果已存在则不生成
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> Create(Guid id);
    }
}
