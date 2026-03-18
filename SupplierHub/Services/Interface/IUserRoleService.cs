using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SupplierHub.DTOs.UserRoleDTO;

namespace SupplierHub.Services.Interface
{
    public interface IUserRoleService
    {
        Task<List<UserRoleDto>> GetAllAsync(bool includeDeleted = false, CancellationToken ct = default);
        Task<List<UserRoleDto>> GetByUserAsync(long userId, bool includeDeleted = false, CancellationToken ct = default);
        Task<UserRoleDto?> GetByIdsAsync(long userId, long roleId, CancellationToken ct = default);
        Task<UserRoleDto> CreateAsync(CreateUserRoleDto dto, CancellationToken ct = default);
        Task<UserRoleDto?> UpdateAsync(long userId, long roleId, UpdateUserRoleDto dto, CancellationToken ct = default);
        Task<bool> SoftDeleteAsync(long userId, long roleId, CancellationToken ct = default);
        Task<bool> ExistsAsync(long userId, long roleId, CancellationToken ct = default);
    }
}

