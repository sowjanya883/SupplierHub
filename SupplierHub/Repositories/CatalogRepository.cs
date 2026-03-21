using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SupplierHub.Models;
using SupplierHub.Repositories.Interface;

namespace SupplierHub.Repositories
{
	public class CatalogRepository : ICatalogRepository
	{
		private readonly AppDbContext _db;

		public CatalogRepository(AppDbContext db)
		{
			_db = db;
		}

		// ✅ CREATE
		public async Task<Catalog> CreateAsync(Catalog entity, CancellationToken ct)
		{
			await _db.Set<Catalog>().AddAsync(entity, ct);
			await _db.SaveChangesAsync(ct);
			return entity;
		}

		// ✅ GET BY ID
		public async Task<Catalog?> GetByIdAsync(long catalogId, CancellationToken ct)
		{
			return await _db.Set<Catalog>()
				.FirstOrDefaultAsync(c =>
					c.CatalogID == catalogId &&
					!c.IsDeleted,
					ct);
		}

		// ✅ GET ALL
		public async Task<IEnumerable<Catalog>> GetAllAsync(CancellationToken ct)
		{
			return await _db.Set<Catalog>()
				.Where(c => !c.IsDeleted)
				.ToListAsync(ct);
		}

		// ✅ UPDATE
		public async Task<Catalog> UpdateAsync(Catalog entity, CancellationToken ct)
		{
			_db.Set<Catalog>().Update(entity);
			await _db.SaveChangesAsync(ct);
			return entity;
		}

		// ✅ SOFT DELETE
		public async Task<bool> DeleteAsync(long catalogId, CancellationToken ct)
		{
			var catalog = await _db.Set<Catalog>()
				.FirstOrDefaultAsync(c =>
					c.CatalogID == catalogId &&
					!c.IsDeleted,
					ct);

			if (catalog == null)
				return false;

			catalog.IsDeleted = true;
			catalog.UpdatedOn = DateTime.UtcNow;

			await _db.SaveChangesAsync(ct);
			return true;
		}
	}
}