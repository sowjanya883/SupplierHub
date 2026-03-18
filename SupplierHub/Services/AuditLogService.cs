using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SupplierHub.DTOs.AuditLogDTO;
using SupplierHub.Models;
using SupplierHub.Repositories.Interface;
using SupplierHub.Services.Interface;

namespace SupplierHub.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IAuditLogRepository _repo;
        private readonly IMapper _mapper;

        public AuditLogService(IAuditLogRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<List<AuditLogDto>> GetAllAsync(bool includeDeleted = false, CancellationToken ct = default)
        {
            var items = await _repo.GetAllAsync(includeDeleted, ct);
            return _mapper.Map<List<AuditLogDto>>(items);
        }

        public async Task<AuditLogDto?> GetByIdAsync(long id, CancellationToken ct = default)
        {
            var a = await _repo.GetByIdAsync(id, false, ct);
            if (a == null) return null;
            return _mapper.Map<AuditLogDto>(a);
        }

        public async Task<AuditLogDto> CreateAsync(CreateAuditLogDto dto, CancellationToken ct = default)
        {
            var a = _mapper.Map<AuditLog>(dto);
            a.Timestamp = dto.Timestamp ?? System.DateTime.UtcNow;
            a.CreatedOn = System.DateTime.UtcNow;
            a.UpdatedOn = a.CreatedOn;
            a.IsDeleted = false;

            await _repo.AddAsync(a, ct);
            await _repo.SaveChangesAsync(ct);

            return _mapper.Map<AuditLogDto>(a);
        }

        public async Task<AuditLogDto?> UpdateAsync(long id, UpdateAuditLogDto dto, CancellationToken ct = default)
        {
            var a = await _repo.GetByIdAsync(id, true, ct);
            if (a == null) return null;

            if (dto.UserID.HasValue) a.UserID = dto.UserID;
            if (!string.IsNullOrWhiteSpace(dto.Action)) a.Action = dto.Action!;
            if (!string.IsNullOrWhiteSpace(dto.Resource)) a.Resource = dto.Resource!;
            if (!string.IsNullOrWhiteSpace(dto.Metadata)) a.Metadata = dto.Metadata!;
            if (dto.Timestamp.HasValue) a.Timestamp = dto.Timestamp.Value;
            if (!string.IsNullOrWhiteSpace(dto.Status)) a.Status = dto.Status!;
            if (dto.IsDeleted.HasValue) a.IsDeleted = dto.IsDeleted.Value;

            a.UpdatedOn = System.DateTime.UtcNow;

            await _repo.UpdateAsync(a, ct);
            await _repo.SaveChangesAsync(ct);

            return _mapper.Map<AuditLogDto>(a);
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

