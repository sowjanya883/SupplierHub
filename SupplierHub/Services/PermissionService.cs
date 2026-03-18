using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SupplierHub.DTOs.PermissionDTO;
using SupplierHub.Models;
using SupplierHub.Repositories.Interface;
using SupplierHub.Services.Interface;

namespace SupplierHub.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _repo;
        private readonly IMapper _mapper;

        public PermissionService(IPermissionRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<List<PermissionDto>> GetAllAsync(bool includeDeleted = false, CancellationToken ct = default)
        {
            var items = await _repo.GetAllAsync(includeDeleted, ct);
            return _mapper.Map<List<PermissionDto>>(items);
        }

        public async Task<PermissionDto?> GetByIdAsync(long id, CancellationToken ct = default)
        {
            var p = await _repo.GetByIdAsync(id, false, ct);
            if (p == null) return null;
            return _mapper.Map<PermissionDto>(p);
        }

        public async Task<PermissionDto> CreateAsync(CreatePermissionDto dto, CancellationToken ct = default)
        {
            var p = _mapper.Map<Permission>(dto);
            p.CreatedOn = System.DateTime.UtcNow;
            p.UpdatedOn = p.CreatedOn;
            p.IsDeleted = false;

            await _repo.AddAsync(p, ct);
            await _repo.SaveChangesAsync(ct);

            return _mapper.Map<PermissionDto>(p);
        }

        public async Task<PermissionDto?> UpdateAsync(long id, UpdatePermissionDto dto, CancellationToken ct = default)
        {
            var p = await _repo.GetByIdAsync(id, true, ct);
            if (p == null) return null;

            if (!string.IsNullOrWhiteSpace(dto.Code)) p.Code = dto.Code!;
            if (!string.IsNullOrWhiteSpace(dto.PermissionName)) p.PermissionName = dto.PermissionName!;
            if (!string.IsNullOrWhiteSpace(dto.Status)) p.Status = dto.Status!;
            if (dto.IsDeleted.HasValue) p.IsDeleted = dto.IsDeleted.Value;

            p.UpdatedOn = System.DateTime.UtcNow;

            await _repo.UpdateAsync(p, ct);
            await _repo.SaveChangesAsync(ct);

            return _mapper.Map<PermissionDto>(p);
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

