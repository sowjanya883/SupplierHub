using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SupplierHub.DTOs.RolePermissionDTO;
using SupplierHub.Models;
using SupplierHub.Repositories.Interface;
using SupplierHub.Services.Interface;

namespace SupplierHub.Services
{
    public class RolePermissionService : IRolePermissionService
    {
        private readonly IRolePermissionRepository _repo;
        private readonly IMapper _mapper;

        public RolePermissionService(IRolePermissionRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<List<RolePermissionDto>> GetAllAsync(bool includeDeleted = false, CancellationToken ct = default)
        {
            var items = await _repo.GetAllAsync(includeDeleted, ct);
            return _mapper.Map<List<RolePermissionDto>>(items);
        }

        public async Task<List<RolePermissionDto>> GetByRoleAsync(long roleId, bool includeDeleted = false, CancellationToken ct = default)
        {
            var items = await _repo.GetByRoleAsync(roleId, includeDeleted, ct);
            return _mapper.Map<List<RolePermissionDto>>(items);
        }

        public async Task<RolePermissionDto?> GetByIdsAsync(long roleId, long permissionId, CancellationToken ct = default)
        {
            var rp = await _repo.GetByIdsAsync(roleId, permissionId, false, ct);
            if (rp == null) return null;
            return _mapper.Map<RolePermissionDto>(rp);
        }

        public async Task<RolePermissionDto> CreateAsync(CreateRolePermissionDto dto, CancellationToken ct = default)
        {
            var rp = _mapper.Map<RolePermission>(dto);
            rp.CreatedOn = System.DateTime.UtcNow;
            rp.UpdatedOn = rp.CreatedOn;
            rp.IsDeleted = false;

            await _repo.AddAsync(rp, ct);
            await _repo.SaveChangesAsync(ct);

            return _mapper.Map<RolePermissionDto>(rp);
        }

        public async Task<RolePermissionDto?> UpdateAsync(long roleId, long permissionId, UpdateRolePermissionDto dto, CancellationToken ct = default)
        {
            var rp = await _repo.GetByIdsAsync(roleId, permissionId, true, ct);
            if (rp == null) return null;

            if (!string.IsNullOrWhiteSpace(dto.Status)) rp.Status = dto.Status!;
            if (dto.IsDeleted.HasValue) rp.IsDeleted = dto.IsDeleted.Value;

            rp.UpdatedOn = System.DateTime.UtcNow;

            await _repo.UpdateAsync(rp, ct);
            await _repo.SaveChangesAsync(ct);

            return _mapper.Map<RolePermissionDto>(rp);
        }

        public async Task<bool> SoftDeleteAsync(long roleId, long permissionId, CancellationToken ct = default)
        {
            var ok = await _repo.SoftDeleteAsync(roleId, permissionId, ct);
            if (!ok) return false;
            await _repo.SaveChangesAsync(ct);
            return true;
        }

        public Task<bool> ExistsAsync(long roleId, long permissionId, CancellationToken ct = default)
        {
            return _repo.ExistsAsync(roleId, permissionId, false, ct);
        }
    }
}

