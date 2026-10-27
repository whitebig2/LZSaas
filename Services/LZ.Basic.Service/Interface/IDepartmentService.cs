using LZ.Basic.Service.Departments;

using LZ.Model;

namespace LZ.Basic.Service.Interface;

public interface IDepartmentService
{
    Task<List<Live_Partner>> GetDeptTreeAsync(int companyId, CancellationToken ct = default);
    Task<Live_Partner> GetAsync(Guid deptGID, CancellationToken ct = default);
    Task<int> AddAsync(Live_Partner dept, CancellationToken ct = default);
    Task<bool> UpdateAsync(Live_Partner dept, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid deptGID, CancellationToken ct = default);
    Task<bool> CanDeleteAsync(Guid deptGID, CancellationToken ct = default);
}
