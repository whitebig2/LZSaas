using LZ.Model;
using LZ.Model.PagedModel;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Basic.Service.Interface
{
    public interface ICityPartnerService
    {
        Task<int> Add(Live_CityPartner model);
        Task<bool> Delete(Expression<Func<Live_CityPartner, bool>> exp);
        Task<Live_CityPartner> Get(Expression<Func<Live_CityPartner, bool>> exp);
        Task<List<Live_CityPartner>> List(Expression<Func<Live_CityPartner, bool>> exp);
        Task<List<Live_CityPartner>> PageList(Expression<Func<Live_CityPartner, bool>> exp, PagedModelBase query);
        Task<bool> Update(Live_CityPartner model);
    }
}
