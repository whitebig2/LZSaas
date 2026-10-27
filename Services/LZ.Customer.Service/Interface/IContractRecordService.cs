using System.Linq.Expressions;
using LZ.Model;
using LZ.Model.Dtos;
using LZ.Model.ModelExtensions;
using LZ.Model.PagedModel;

namespace LZ.Customer.Service.Interface
{
    public interface IContractRecordService
    {
        Task<List<Model.Live_ContractRecord>> List(Expression<Func<Model.Live_ContractRecord, bool>> exp, PagedModelBase query);

        Task<int> Add(Model.Live_ContractRecord model);

        Task<bool> Update(Model.Live_ContractRecord model);
        Task<bool> Delete(Guid id);
        Task<bool> Exist(Guid id);

        Task<Model.Live_ContractRecord> Get(Guid id);
        Task<Live_ContractRecord> Add(Live_ContractRecord model, List<Live_ContractRecordSigner> signers);
        Task<List<Live_ContractRecordSigner>> GetRecordSigners(Guid recordId);
        Task<bool> UpdateRecordSigners(List<Live_ContractRecordSigner> signers);
        Task<bool> UpdateRecordSigners(Live_ContractRecord record, List<Live_ContractRecordSigner> signers);
      
        Task<List<Live_V_ContractRecord>> PageList(Expression<Func<Live_V_ContractRecord, bool>> exp, PagedModelBase query);
        Task<List<Live_ContractRecordExtension>> ListWithDetail(Expression<Func<Live_ContractRecordExtension, bool>> exp);
        Task<Live_ContractRecordExtension> GetNav(Guid id);
        Task<bool> UpdateNav(Live_ContractRecordExtension model);
        Task<List<Live_ContractRecordExtension>> ListNav(Expression<Func<Live_ContractRecordExtension, bool>> exp, PagedModelBase query);
        Task<bool> AddNav(Live_ContractRecordExtension model);
        Task<bool> DeleteNav(Guid id);
        Task<Live_ContractRecordExtension> GetNav(Expression<Func<Live_ContractRecordExtension, bool>> exp);
        Task<Live_ContractRecord> Get(Expression<Func<Live_ContractRecord, bool>> exp);
        Task<List<Live_ContractRecord>> List(Expression<Func<Live_ContractRecord, bool>> exp);
        Task<bool> Update(List<Live_ContractRecord> models);
        Task<List<Live_UserEntry>> UserEntryList(Expression<Func<Live_UserEntry, bool>> exp);
        Task<bool> UpdateUserEntry(Live_UserEntry model);
        Task<List<Live_V_ContractRecord>> ListView(Expression<Func<Live_V_ContractRecord, bool>> exp);
        Task<Live_V_ContractRecord> GetView(Expression<Func<Live_V_ContractRecord, bool>> exp);
    }
}
