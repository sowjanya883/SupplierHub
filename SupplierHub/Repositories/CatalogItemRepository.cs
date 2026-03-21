using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SupplierHub.Models;
using SupplierHub.Repositories.Interface;
using SupplierHub.DTOs.CatalogItemDTO;

namespace SupplierHub.Repositories
{
	public class CatalogItemRepository : ICatalogItemRepository
	{
		private readonly AppDbContext _db;

		public CatalogItemRepository(AppDbContext db)
		{
			_db = db;
		}

		public async Task<CatalogItem> CreateAsync(
			CatalogItem entity,
			CancellationToken ct)
		{
			await _db.Set<CatalogItem>().AddAsync(entity, ct);
			await _db.SaveChangesAsync(ct);
			return entity;
		}

		public async Task<CatalogItem?> GetByIdAsync(
			long catItemId,
			CancellationToken ct)
		{
			return await _db.Set<CatalogItem>()
				.FirstOrDefaultAsync(ci =>
					ci.CatItemID == catItemId &&
					!ci.IsDeleted,
					ct);
		}

		public async Task<IEnumerable<CatalogItem>> GetAllAsync(
			CancellationToken ct)
		{
			return await _db.Set<CatalogItem>()
				.Where(ci => !ci.IsDeleted)
				.ToListAsync(ct);
		}

		public async Task<CatalogItem> UpdateAsync(
			CatalogItem entity,
			CancellationToken ct)
		{
			_db.Set<CatalogItem>().Update(entity);
			await _db.SaveChangesAsync(ct);
			return entity;
		}

		public async Task<bool> DeleteAsync(
			CatalogItemDeleteDto dto,
			CancellationToken ct)
		{
			var item = await _db.Set<CatalogItem>()
				.FirstOrDefaultAsync(ci =>
					!ci.IsDeleted &&
					(dto.CatItemID == null || ci.CatItemID == dto.CatItemID) &&
					(dto.CatalogID == null || ci.CatalogID == dto.CatalogID) &&
					(dto.ItemID == null || ci.ItemID == dto.ItemID),
					ct);

			if (item == null)
				return false;

			item.IsDeleted = true;
			item.UpdatedOn = DateTime.UtcNow;
			await _db.SaveChangesAsync(ct);
			return true;
		}
	}
}