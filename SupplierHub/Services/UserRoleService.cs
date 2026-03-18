using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SupplierHub.DTOs.UserRoleDTO;
using SupplierHub.Models;
using SupplierHub.Repositories.Interface;
using SupplierHub.Services.Interface;

namespace SupplierHub.Services
{
    public class UserRoleService : IUserRoleService
    {
        private readonly IUserRoleRepository _repo;
        private readonly IMapper _mapper;

        public UserRoleService(IUserRoleRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<List<UserRoleDto>> GetAllAsync(bool includeDeleted = false, CancellationToken ct = default)
        {
            var items = await _repo.GetAllAsync(includeDeleted, ct);
            return _mapper.Map<List<UserRoleDto>>(items);
        }

        public async Task<List<UserRoleDto>> GetByUserAsync(long userId, bool includeDeleted = false, CancellationToken ct = default)
        {
            var items = await _repo.GetByUserAsync(userId, includeDeleted, ct);
            return _mapper.Map<List<UserRoleDto>>(items);
        }

        public async Task<UserRoleDto?> GetByIdsAsync(long userId, long roleId, CancellationToken ct = default)
        {
            var ur = await _repo.GetByIdsAsync(userId, roleId, false, ct);
            if (ur == null) return null;
            return _mapper.Map<UserRoleDto>(ur);
        }

        public async Task<UserRoleDto> CreateAsync(CreateUserRoleDto dto, CancellationToken ct = default)
        {
            var ur = _mapper.Map<UserRole>(dto);
            ur.CreatedOn = System.DateTime.UtcNow;
            ur.UpdatedOn = ur.CreatedOn;
            ur.IsDeleted = false;

            await _repo.AddAsync(ur, ct);
            await _repo.SaveChangesAsync(ct);

            return _mapper.Map<UserRoleDto>(ur);
        }

        public async Task<UserRoleDto?> UpdateAsync(long userId, long roleId, UpdateUserRoleDto dto, CancellationToken ct = default)
        {
            var ur = await _repo.GetByIdsAsync(userId, roleId, true, ct);
            if (ur == null) return null;

            if (!string.IsNullOrWhiteSpace(dto.Status)) ur.Status = dto.Status!;
            if (dto.IsDeleted.HasValue) ur.IsDeleted = dto.IsDeleted.Value;

            ur.UpdatedOn = System.DateTime.UtcNow;

            await _repo.UpdateAsync(ur, ct);
            await _repo.SaveChangesAsync(ct);

            return _mapper.Map<UserRoleDto>(ur);
        }

        public async Task<bool> SoftDeleteAsync(long userId, long roleId, CancellationToken ct = default)
        {
            var ok = await _repo.SoftDeleteAsync(userId, roleId, ct);
            if (!ok) return false;
            await _repo.SaveChangesAsync(ct);
            return true;
        }

        public Task<bool> ExistsAsync(long userId, long roleId, CancellationToken ct = default)
        {
            return _repo.ExistsAsync(userId, roleId, false, ct);
        }
    }
}

