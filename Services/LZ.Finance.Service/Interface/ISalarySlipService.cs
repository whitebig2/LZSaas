using LZ.Model;
using LZ.Model.PagedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Finance.Service.Interface
{
    public interface ISalarySlipService
    {
        Task<bool> AddNav(Live_SalarySlipExtension model);
        Task<bool> DeleteNav(Live_SalarySlipExtension model);
        Task<bool> Exist(Expression<Func<Live_SalarySlip, bool>> exp);
        Task<Live_SalarySlip> Get(Expression<Func<Live_SalarySlip, bool>> exp);
        Task<Live_SalarySlipExtension> GetNav(Guid id);
        Task<Live_SalarySlipRecord> GetSalarySlipRecord(Expression<Func<Live_SalarySlipRecord, bool>> exp);
        Task<List<Live_SalarySlip>> List(Expression<Func<Live_SalarySlip, bool>> exp);
        Task<List<Live_V_SalarySlip>> ListView(Expression<Func<Live_V_SalarySlip, bool>> exp);
        Task<List<Live_V_SalarySlip>> PageList(Expression<Func<Live_V_SalarySlip, bool>> exp, PagedModelBase query);
    }
}
