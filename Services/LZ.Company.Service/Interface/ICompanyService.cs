using LZ.Model;
using LZ.Model.PagedModel;
using System.Linq.Expressions;

namespace LZ.Company.Service.Interface
{
    public interface ICompanyService
    {
        Task<List<Model.SYS_Company>> List(Expression<Func<Model.SYS_Company, bool>> exp, PagedModelBase query);
        Task<List<Model.Live_V_CompanyPerFunction>> PerFunctionList(Expression<Func<Model.Live_V_CompanyPerFunction, bool>> exp, PagedModelBase query);
        Task<List<Model.Live_CompanyPerFunction>> PerFunctionList(Expression<Func<Model.Live_CompanyPerFunction, bool>> exp, PagedModelBase query);
        Task<int> Add(Model.SYS_Company model);

        Task<bool> Update(Model.SYS_Company model);
        Task<bool> UpdateConfig(Model.CompanyConfig model, Guid id);
        Task<bool> Delete(int id);
        Task<bool> Exist(int id);
        Task<bool> Exist(Expression<Func<SYS_Company, bool>> exp);
        Task<bool> ChangeStatus(int id, int status);
        /// <summary>
        /// 设置蓝招小程序功能列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        Task SetPerFunction(int id, List<Model.Live_CompanyPerFunction> list);

        Task<Model.SYS_Company> Get(int id);
        Task<Model.SYS_Company> Get(Guid id);
    }
}
