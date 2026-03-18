using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SupplierHub.DTOs.PermissionDTO;

namespace SupplierHub.Services.Interface
{
    public interface IPermissionService
    {
        Task<List<PermissionDto>> GetAllAsync(bool includeDeleted = false, CancellationToken ct = default);
        Task<PermissionDto?> GetByIdAsync(long id, CancellationToken ct = default);
        Task<PermissionDto> CreateAsync(CreatePermissionDto dto, CancellationToken ct = default);
        Task<PermissionDto?> UpdateAsync(long id, UpdatePermissionDto dto, CancellationToken ct = default);
        Task<bool> SoftDeleteAsync(long id, CancellationToken ct = default);
        Task<bool> ExistsAsync(long id, CancellationToken ct = default);
    }
}

