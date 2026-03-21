using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SupplierHub.Models;
using SupplierHub.Repositories.Interface;
using SupplierHub.DTOs.CategoryDTO;

namespace SupplierHub.Repositories
{
	public class CategoryRepository : ICategoryRepository
	{
		private readonly AppDbContext _db;

		public CategoryRepository(AppDbContext db)
		{
			_db = db;
		}

		public async Task<Category> CreateAsync(
			Category entity,
			CancellationToken ct)
		{
			await _db.Set<Category>().AddAsync(entity, ct);
			await _db.SaveChangesAsync(ct);
			return entity;
		}

		public async Task<Category?> GetByIdAsync(
			long categoryId,
			CancellationToken ct)
		{
			return await _db.Set<Category>()
				.FirstOrDefaultAsync(c =>
					c.CategoryID == categoryId &&
					!c.IsDeleted,
					ct);
		}

		public async Task<IEnumerable<Category>> GetAllAsync(
			CancellationToken ct)
		{
			return await _db.Set<Category>()
				.Where(c => !c.IsDeleted)
				.ToListAsync(ct);
		}

		public async Task<Category> UpdateAsync(
			Category entity,
			CancellationToken ct)
		{
			_db.Set<Category>().Update(entity);
			await _db.SaveChangesAsync(ct);
			return entity;
		}

		public async Task<bool> DeleteAsync(
			CategoryDeleteDto dto,
			CancellationToken ct)
		{
			var category = await _db.Set<Category>()
				.FirstOrDefaultAsync(c =>
					!c.IsDeleted &&
					(dto.CategoryID == null || c.CategoryID == dto.CategoryID) &&
					(dto.ParentCategoryID == null || c.ParentCategoryID == dto.ParentCategoryID) &&
					(dto.CategoryName == null || c.CategoryName == dto.CategoryName),
					ct);

			if (category == null)
				return false;

			category.IsDeleted = true;
			category.UpdatedOn = DateTime.UtcNow;
			await _db.SaveChangesAsync(ct);
			return true;
		}
	}
}