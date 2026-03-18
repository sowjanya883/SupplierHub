using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SupplierHub.DTOs.RoleDTO;
using SupplierHub.Models;
using SupplierHub.Repositories.Interface;
using SupplierHub.Services.Interface;

namespace SupplierHub.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _repo;
        private readonly IMapper _mapper;

        public RoleService(IRoleRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<List<RoleDto>> GetAllAsync(bool includeDeleted = false, CancellationToken ct = default)
        {
            var items = await _repo.GetAllAsync(includeDeleted, ct);
            return _mapper.Map<List<RoleDto>>(items);
        }

        public async Task<RoleDto?> GetByIdAsync(long id, CancellationToken ct = default)
        {
            var role = await _repo.GetByIdAsync(id, false, ct);
            if (role == null) return null;
            return _mapper.Map<RoleDto>(role);
        }

        public async Task<RoleDto> CreateAsync(CreateRoleDto dto, CancellationToken ct = default)
        {
            var role = _mapper.Map<Role>(dto);
            role.CreatedOn = System.DateTime.UtcNow;
            role.UpdatedOn = role.CreatedOn;
            role.IsDeleted = false;

            await _repo.AddAsync(role, ct);
            await _repo.SaveChangesAsync(ct);

            return _mapper.Map<RoleDto>(role);
        }

        public async Task<RoleDto?> UpdateAsync(long id, UpdateRoleDto dto, CancellationToken ct = default)
        {
            var role = await _repo.GetByIdAsync(id, true, ct);
            if (role == null) return null;

            if (!string.IsNullOrWhiteSpace(dto.RoleName)) role.RoleName = dto.RoleName;
            if (!string.IsNullOrWhiteSpace(dto.Status)) role.Status = dto.Status;
            if (dto.IsDeleted.HasValue) role.IsDeleted = dto.IsDeleted.Value;

            role.UpdatedOn = System.DateTime.UtcNow;

            await _repo.UpdateAsync(role, ct);
            await _repo.SaveChangesAsync(ct);

            return _mapper.Map<RoleDto>(role);
        }

        public async Task<bool> SoftDeleteAsync(long id, CancellationToken ct = default)
        {
            var ok = await _repo.SoftDeleteAsync(id, ct);
            if (!ok) return false;
            await _repo.SaveChangesAsync(ct);
            return true;
        }

        public Task<bool> ExistsAsync(long id, CancellationToken ct = default)
        {
            return _repo.ExistsAsync(id, false, ct);
        }
    }
}

