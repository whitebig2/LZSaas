using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Core.Interface
{
    public interface IFileUploadService
    {
        Task<List<Model.Live_FileUpload>> List(Expression<Func<Model.Live_FileUpload, bool>> exp, Model.PagedModel.UserPagedModel query, string order = "");
        Task<int> Add(Model.Live_FileUpload model);

        Task<int> Add(List<Model.Live_FileUpload> list);

        Task<bool> Update(Model.Live_FileUpload model);
        Task<bool> Exist(Expression<Func<Model.Live_FileUpload, bool>> exp);
        Task<bool> ExistID(int id);
        Task<bool> Delete(int id);
        Task<Model.Live_FileUpload> Get(int id);
        Task<bool> DeleteFID(int id, int key);
    }
}
