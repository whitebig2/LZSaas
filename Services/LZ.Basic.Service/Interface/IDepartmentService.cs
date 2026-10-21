using LZ.Basic.Service.Departments;

namespace LZ.Basic.Service.Interface;

public interface IDepartmentService
{
    Task<List<DepartmentRecord>> GetDeptTreeAsync(int companyId, CancellationToken ct = default);
    Task<DepartmentRecord?> GetAsync(Guid deptGID, CancellationToken ct = default);
    Task<int> AddAsync(DepartmentRecord dept, CancellationToken ct = default);
    Task<bool> UpdateAsync(DepartmentRecord dept, CancellationToken ct = default);
    Task<bool> DisableAsync(Guid deptGID, CancellationToken ct = default);
    Task<bool> CanDeleteAsync(Guid deptGID, CancellationToken ct = default);
}
