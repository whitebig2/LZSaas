using LZ.Model;
using LZ.Model.PagedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Basic.Service.Interface
{
    public interface ICitySalesmanService
    {
        Task<int> Add(Live_CitySalesman model);
        Task<bool> Delete(Expression<Func<Live_CitySalesman, bool>> exp);
        Task<Live_CitySalesman> Get(Expression<Func<Live_CitySalesman, bool>> exp);
        Task<List<Live_CitySalesman>> List(Expression<Func<Live_CitySalesman, bool>> exp);
        Task<List<Live_CitySalesman>> PageList(Expression<Func<Live_CitySalesman, bool>> exp, PagedModelBase query);
        Task<bool> Update(Live_CitySalesman model);
    }
}
