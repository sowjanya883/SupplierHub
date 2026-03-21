using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SupplierHub.Models;
using SupplierHub.Repositories.Interface;
using SupplierHub.DTOs.ItemDTO;

namespace SupplierHub.Repositories
{
	public class ItemRepository : IItemRepository
	{
		private readonly AppDbContext _db;

		public ItemRepository(AppDbContext db)
		{
			_db = db;
		}

		public async Task<Item> CreateAsync(
			Item entity,
			CancellationToken ct)
		{
			await _db.Set<Item>().AddAsync(entity, ct);
			await _db.SaveChangesAsync(ct);
			return entity;
		}

		public async Task<Item?> GetByIdAsync(
			long itemId,
			CancellationToken ct)
		{
			return await _db.Set<Item>()
				.FirstOrDefaultAsync(i =>
					i.ItemID == itemId &&
					!i.IsDeleted,
					ct);
		}

		public async Task<IEnumerable<Item>> GetAllAsync(
			CancellationToken ct)
		{
			return await _db.Set<Item>()
				.Where(i => !i.IsDeleted)
				.ToListAsync(ct);
		}

		public async Task<Item> UpdateAsync(
			Item entity,
			CancellationToken ct)
		{
			_db.Set<Item>().Update(entity);
			await _db.SaveChangesAsync(ct);
			return entity;
		}

		public async Task<bool> DeleteAsync(
			ItemDeleteDto dto,
			CancellationToken ct)
		{
			var item = await _db.Set<Item>()
				.FirstOrDefaultAsync(i =>
					i.ItemID == dto.ItemID &&
					!i.IsDeleted,
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