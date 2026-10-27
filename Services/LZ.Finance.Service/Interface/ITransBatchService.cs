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
    public interface ITransBatchService
    {
        Task<bool> AddNav(List<Live_TransBatchExtension> models);
        Task<bool> AddNav(Live_TransBatchExtension model);
        Task<bool> Exist(Guid id);
        Task<Live_TransBatchExtension> GetNav(Expression<Func<Live_TransBatchExtension, bool>> exp);
        Task<List<Live_TransBatch>> List(Expression<Func<Live_TransBatch, bool>> exp);
        Task<List<Live_TransBatchExtension>> ListNav(Expression<Func<Live_TransBatchExtension, bool>> exp);
        Task<List<Live_TransBatch>> PageList(Expression<Func<Live_TransBatch, bool>> exp, PagedModelBase query);
        Task<bool> Update(Live_TransBatch model);
        Task<bool> UpdateNav(Live_TransBatchExtension model);
    }
}
