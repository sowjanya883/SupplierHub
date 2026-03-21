using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SupplierHub.DTOs.SupplierRiskDTO;
using SupplierHub.Models;
using SupplierHub.Repositories.Interface;

namespace SupplierHub.Repositories
{
	public class SupplierRiskRepository : ISupplierRiskRepository
	{
		private readonly AppDbContext _db;

		public SupplierRiskRepository(AppDbContext db)
		{
			_db = db;
		}

		public async Task<SupplierRisk> CreateAsync(SupplierRisk entity, CancellationToken ct)
		{
			await _db.Set<SupplierRisk>().AddAsync(entity, ct);
			await _db.SaveChangesAsync(ct);
			return entity;
		}

		public async Task<SupplierRisk?> GetByIdAsync(long riskId, CancellationToken ct)
		{
			return await _db.Set<SupplierRisk>()
				.FirstOrDefaultAsync(r => r.RiskID == riskId && !r.IsDeleted, ct);
		}

		public async Task<IEnumerable<SupplierRisk>> GetAllAsync(CancellationToken ct)
		{
			return await _db.Set<SupplierRisk>()
				.Where(r => !r.IsDeleted)
				.ToListAsync(ct);
		}

		public async Task<SupplierRisk> UpdateAsync(SupplierRisk entity, CancellationToken ct)
		{
			_db.Set<SupplierRisk>().Update(entity);
			await _db.SaveChangesAsync(ct);
			return entity;
		}

		public async Task<bool> DeleteAsync(SupplierRiskDeleteDto dto, CancellationToken ct)
		{
			var risk = await _db.Set<SupplierRisk>()
				.FirstOrDefaultAsync(r =>
					!r.IsDeleted &&
					(dto.RiskID == null || r.RiskID == dto.RiskID) &&
					(dto.SupplierID == null || r.SupplierID == dto.SupplierID) &&
					(dto.RiskType == null || r.RiskType == dto.RiskType),
					ct);

			if (risk == null)
				return false;

			risk.IsDeleted = true;
			risk.UpdatedOn = DateTime.UtcNow;
			await _db.SaveChangesAsync(ct);
			return true;
		}
	}
}