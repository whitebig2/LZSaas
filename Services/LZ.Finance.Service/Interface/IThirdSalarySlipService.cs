using LZ.Model;
using LZ.Model.ModelExtensions;
using LZ.Model.PagedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Finance.Service.Interface
{
    public interface IThirdSalarySlipService
    {
        Task<bool> AddNav(Live_ThirdSalarySlipExtension model);
        Task<bool> DeleteNav(Live_ThirdSalarySlipExtension model);
        Task<Live_ThirdSalarySlipExtension> GetNav(Expression<Func<Live_ThirdSalarySlipExtension, bool>> exp, string strOrderByFileds = null);
        Task<Live_ThirdSalarySlipRecord> GetRecord(Expression<Func<Live_ThirdSalarySlipRecord, bool>> exp);
        Task<List<Live_ThirdSalarySlipRecord>> ListRecords(Expression<Func<Live_ThirdSalarySlipRecord, bool>> exp);
        Task<List<Live_ThirdSalarySlip>> PageList(Expression<Func<Live_ThirdSalarySlip, bool>> exp, PagedModelBase query);
        Task<List<Live_ThirdSalarySlipRecord>> PageRecordList(Expression<Func<Live_ThirdSalarySlipRecord, bool>> exp, PagedModelBase query);
        Task<bool> UpdateNav(Live_ThirdSalarySlipExtension model);
    }
}
