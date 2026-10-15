using System.Linq.Expressions;
using LZ.Model.PagedModel;

namespace LZ.Clients.Service.Interface
{
    public interface INoticeTemplateService
    {
        Task<List<Model.Live_NoticeTemplate>> List(Expression<Func<Model.Live_NoticeTemplate, bool>> exp, PagedModelBase query);

        Task<int> Add(Model.Live_NoticeTemplate model);

        Task<bool> Update(Model.Live_NoticeTemplate model);
        Task<bool> Delete(int id);
        Task<bool> Exist(int id);
        Task<bool> Exist(Expression<Func<Model.Live_NoticeTemplate, bool>> exp);
        Task<Model.Live_NoticeTemplate> Get(int id);
    }
}
