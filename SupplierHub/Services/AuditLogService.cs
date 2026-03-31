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

		// Keep: Necessary for Admins to view the history
		public async Task<List<AuditLogDto>> GetAllAsync(bool includeDeleted = false, CancellationToken ct = default)
		{
			var items = await _repo.GetAllAsync(includeDeleted, ct);
			return _mapper.Map<List<AuditLogDto>>(items);
		}

		// Keep: Necessary for detailed investigation of a specific event
		public async Task<AuditLogDto?> GetByIdAsync(long id, CancellationToken ct = default)
		{
			var a = await _repo.GetByIdAsync(id, false, ct);
			if (a == null) return null;
			return _mapper.Map<AuditLogDto>(a);
		}
// Keep: This is the primary way logs are generated in the system [cite: 48, 521]
		public async Task<AuditLogDto> CreateAsync(CreateAuditLogDto dto, CancellationToken ct = default)
		{
			var a = _mapper.Map<AuditLog>(dto);

			// Set immutable timestamps
			a.Timestamp = dto.Timestamp ?? System.DateTime.UtcNow;
			a.CreatedOn = System.DateTime.UtcNow;
			a.UpdatedOn = a.CreatedOn;
			a.IsDeleted = false;

			await _repo.AddAsync(a, ct);
			await _repo.SaveChangesAsync(ct);

			return _mapper.Map<AuditLogDto>(a);
		}

		// Logic: Audit logs are immutable and should never be modified.

	// Logic: Audit logs must be permanent for security and compliance[cite: 563].

		public Task<bool> ExistsAsync(long id, CancellationToken ct = default)
		{
			return _repo.ExistsAsync(id, false, ct);
		}
	}
}
