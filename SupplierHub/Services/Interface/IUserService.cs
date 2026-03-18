using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SupplierHub.DTOs.UserDTO;

namespace SupplierHub.Services.Interface
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllAsync(bool includeDeleted = false, CancellationToken ct = default);
        Task<UserDto?> GetByIdAsync(long id, CancellationToken ct = default);
        Task<UserDto> CreateAsync(CreateUserDto dto, CancellationToken ct = default);
        Task<UserDto?> UpdateAsync(long id, UpdateUserDto dto, CancellationToken ct = default);
        Task<bool> SoftDeleteAsync(long id, CancellationToken ct = default);
        Task<bool> ExistsAsync(long id, CancellationToken ct = default);
    }
}
