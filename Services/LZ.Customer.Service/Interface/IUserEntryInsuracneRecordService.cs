using LZ.Model;
using LZ.Model.PagedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Customer.Service.Interface
{
    public interface IUserEntryInsuracneRecordService
    {
        Task<int> Add(List<Live_UserEntryInsuracneRecord> models);
        Task<int> Add(Live_UserEntryInsuracneRecord model);
        Task<bool> Delete(Guid id);
        Task<bool> Delete(Expression<Func<Live_UserEntryInsuracneRecord, bool>> exp);
        Task<Live_UserEntryInsuracneRecord> Get(Expression<Func<Live_UserEntryInsuracneRecord, bool>> exp);
        Task<List<Live_UserEntryInsuracneRecord>> List(Expression<Func<Live_UserEntryInsuracneRecord, bool>> exp);
        Task<List<Live_UserEntryInsuracneRecord>> PageList(Expression<Func<Live_UserEntryInsuracneRecord, bool>> exp, PagedModelBase query);
        Task<bool> Update(Live_UserEntryInsuracneRecord model);
        Task<bool> Update(List<Live_UserEntryInsuracneRecord> models);
        //Task<int> UpdateEntry(Live_UserEntry entryModel);
        //Task<Live_UserEntry> GetEntry(Expression<Func<Live_UserEntry, bool>> exp);
        //Task<List<Live_UserEntry>> ListEntry(Expression<Func<Live_UserEntry, bool>> exp);

    }
}
