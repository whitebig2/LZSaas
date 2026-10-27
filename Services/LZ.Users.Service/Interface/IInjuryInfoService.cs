using LZ.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Users.Service.Interface
{
    public interface IInjuryInfoService
    {
        Task<List<Model.Live_V_InjuryInfo>> List(Expression<Func<Model.Live_V_InjuryInfo, bool>> exp, Model.PagedModel.UserPagedModel query, string order = "");
        Task<int> Add(Model.Live_InjuryInfo model);
        Task<bool> Update(Model.Live_InjuryInfo model);
        Task<bool> UpdateStatus(int id,int status,string msg="");
        Task<bool> Exist(Expression<Func<Model.Live_InjuryInfo, bool>> exp);
        Task<Model.Live_InjuryInfo> View(Expression<Func<Model.Live_InjuryInfo, bool>> exp);
        Task<bool> ExistID(int id);
        Task<bool> Delete(int id);
        Task<Model.Live_InjuryInfo> Get(int id);
        Task<Model.Live_V_InjuryInfo> GetView(int id);
        Task<int> DictionarySet(Dictionary<string, object> dt);
        byte[] ApplyCompensationExportExcel(Live_InjuryInfo model);
        Task<byte[]> StatisticsExportExcel(List<Live_V_InjuryInfo> list);
        Task<Live_InjuryExtension> Test(Expression<Func<Model.Live_InjuryExtension, bool>> exp);
    }
}
