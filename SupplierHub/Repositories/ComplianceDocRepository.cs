using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SupplierHub.DTOs.ComplianceDocDTO;
using SupplierHub.Models;
using SupplierHub.Repositories.Interface;

namespace SupplierHub.Repositories
{
	public class ComplianceDocRepository : IComplianceDocRepository
	{
		private readonly AppDbContext _db;

		public ComplianceDocRepository(AppDbContext db)
		{
			_db = db;
		}

		public async Task<ComplianceDoc> CreateAsync(
			ComplianceDoc entity,
			CancellationToken ct)
		{
			await _db.Set<ComplianceDoc>().AddAsync(entity, ct);
			await _db.SaveChangesAsync(ct);
			return entity;
		}

		public async Task<ComplianceDoc?> GetByIdAsync(long docId, CancellationToken ct)
		{
			return await _db.Set<ComplianceDoc>()
				.FirstOrDefaultAsync(d => d.DocID == docId && !d.IsDeleted, ct);
		}

		public async Task<IEnumerable<ComplianceDoc>> GetAllAsync(CancellationToken ct)
		{
			return await _db.Set<ComplianceDoc>()
				.Where(d => !d.IsDeleted)
				.ToListAsync(ct);
		}

		public async Task<ComplianceDoc> UpdateAsync(
			ComplianceDoc entity,
			CancellationToken ct)
		{
			_db.Set<ComplianceDoc>().Update(entity);
			await _db.SaveChangesAsync(ct);
			return entity;
		}

		public async Task<bool> DeleteAsync(
			ComplianceDocDeleteDto dto,
			CancellationToken ct)
		{
			var doc = await _db.Set<ComplianceDoc>()
				.FirstOrDefaultAsync(d =>
					!d.IsDeleted &&
					(dto.DocID == null || d.DocID == dto.DocID) &&
					(dto.SupplierID == null || d.SupplierID == dto.SupplierID) &&
					(dto.DocType == null || d.DocType == dto.DocType),
					ct);

			if (doc == null)
				return false;

			doc.IsDeleted = true;
			doc.UpdatedOn = DateTime.UtcNow;
			await _db.SaveChangesAsync(ct);
			return true;
		}
	}
}