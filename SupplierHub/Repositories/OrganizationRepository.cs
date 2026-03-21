using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SupplierHub.Models;
using SupplierHub.Repositories.Interface;
using SupplierHub.DTOs.OrganizationDTO;

namespace SupplierHub.Repositories
{
	public class OrganizationRepository : IOrganizationRepository
	{
		private readonly AppDbContext _db;

		public OrganizationRepository(AppDbContext db)
		{
			_db = db;
		}

		public async Task<Organization> CreateAsync(
			Organization entity,
			CancellationToken ct)
		{
			await _db.Set<Organization>().AddAsync(entity, ct);
			await _db.SaveChangesAsync(ct);
			return entity;
		}

		public async Task<Organization?> GetByIdAsync(
			long orgId,
			CancellationToken ct)
		{
			return await _db.Set<Organization>()
				.FirstOrDefaultAsync(o => o.OrgID == orgId && !o.IsDeleted, ct);
		}

		public async Task<IEnumerable<Organization>> GetAllAsync(
			CancellationToken ct)
		{
			return await _db.Set<Organization>()
				.Where(o => !o.IsDeleted)
				.ToListAsync(ct);
		}

		public async Task<Organization> UpdateAsync(
			Organization entity,
			CancellationToken ct)
		{
			_db.Set<Organization>().Update(entity);
			await _db.SaveChangesAsync(ct);
			return entity;
		}

		public async Task<bool> DeleteAsync(
			OrganizationDeleteDto dto,
			CancellationToken ct)
		{
			var org = await _db.Set<Organization>()
				.FirstOrDefaultAsync(o =>
					!o.IsDeleted &&
					(dto.OrgID == null || o.OrgID == dto.OrgID) &&
					(dto.OrganizationName == null || o.OrganizationName == dto.OrganizationName) &&
					(dto.TaxID == null || o.TaxID == dto.TaxID),
					ct);

			if (org == null)
				return false;

			org.IsDeleted = true;
			org.UpdatedOn = DateTime.UtcNow;
			await _db.SaveChangesAsync(ct);
			return true;
		}
	}
}
