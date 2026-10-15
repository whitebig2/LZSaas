using LZ.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Customer.Service.Interface
{
    public interface IAdviceNoteService
    {
        Task<int> Add(Live_AdviceNote model);
        Task<int> Add(List<Live_AdviceNote> models);
        Task<List<Live_AdviceNote>> List(Expression<Func<Live_AdviceNote, bool>> exp);
        Task<bool> Update(Live_AdviceNote model);
    }
}
