using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SupplierHub.DTOs.AuditLogDTO;

namespace SupplierHub.Services.Interface
{
    public interface IAuditLogService
    {
        Task<List<AuditLogDto>> GetAllAsync(bool includeDeleted = false, CancellationToken ct = default);
        Task<AuditLogDto?> GetByIdAsync(long id, CancellationToken ct = default);
        Task<AuditLogDto> CreateAsync(CreateAuditLogDto dto, CancellationToken ct = default);
        Task<bool> ExistsAsync(long id, CancellationToken ct = default);
    }
}

