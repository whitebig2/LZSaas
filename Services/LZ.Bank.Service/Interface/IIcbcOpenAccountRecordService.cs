using LZ.Model;
using LZ.Model.PagedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Bank.Service.Interface
{
    public interface IIcbcOpenAccountRecordService
    {
        Task<int> Add(Live_IcbcOpenAccountRecord model);
        Task<bool> Add(List<Live_IcbcOpenAccountRecord> models);
        Task<bool> Delete(Guid id);
        Task<bool> Exist(Expression<Func<Live_IcbcOpenAccountRecord, bool>> exp);
        Task<Live_IcbcOpenAccountRecord> Get(Expression<Func<Live_IcbcOpenAccountRecord, bool>> exp);
        Task<List<Live_IcbcOpenAccountRecord>> List(Expression<Func<Live_IcbcOpenAccountRecord, bool>> exp);
        Task<List<Live_IcbcOpenAccountRecord>> PageList(Expression<Func<Live_IcbcOpenAccountRecord, bool>> exp, PagedModelBase query);
        Task<bool> Update(Live_IcbcOpenAccountRecord model);
        Task<bool> Update(List<Live_IcbcOpenAccountRecord> models);
    }
}
