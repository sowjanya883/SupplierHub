using System;
using System.Collections.Generic;
using System.Linq;
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

		public async Task<Catalog> CreateAsync(Catalog entity)
		{
			await _db.Set<Catalog>().AddAsync(entity);
			await _db.SaveChangesAsync();
			return entity;
		}

		public async Task<Catalog?> GetByIdAsync(long catalogId)
		{
			return await _db.Set<Catalog>()
				.FirstOrDefaultAsync(c =>
					c.CatalogID == catalogId && !c.IsDeleted);
		}

		public async Task<IEnumerable<Catalog>> GetAllAsync()
		{
			return await _db.Set<Catalog>()
				.Where(c => !c.IsDeleted)
				.OrderByDescending(c => c.CreatedOn)
				.ToListAsync();
		}

		public async Task<Catalog> UpdateAsync(Catalog entity)
		{
			_db.Set<Catalog>().Update(entity);
			await _db.SaveChangesAsync();
			return entity;
		}

		public async Task<bool> DeleteAsync(long catalogId)
		{
			var existing = await GetByIdAsync(catalogId);
			if (existing == null)
				return false;

			existing.IsDeleted = true;
			existing.UpdatedOn = DateTime.UtcNow;

			await _db.SaveChangesAsync();
			return true;
		}
	}
}