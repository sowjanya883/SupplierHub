using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SupplierHub.Models;
using SupplierHub.Repositories.Interface;
using SupplierHub.DTOs.SupplierContactDTO;

namespace SupplierHub.Repositories
{
	public class SupplierContactRepository : ISupplierContactRepository
	{
		private readonly AppDbContext _db;

		public SupplierContactRepository(AppDbContext db)
		{
			_db = db;
		}

		public async Task<SupplierContact> CreateAsync(
			SupplierContact entity,
			CancellationToken ct)
		{
			await _db.Set<SupplierContact>().AddAsync(entity, ct);
			await _db.SaveChangesAsync(ct);
			return entity;
		}

		public async Task<SupplierContact?> GetByIdAsync(
			long contactId,
			CancellationToken ct)
		{
			return await _db.Set<SupplierContact>()
				.FirstOrDefaultAsync(c => c.ContactID == contactId && !c.IsDeleted, ct);
		}

		public async Task<IEnumerable<SupplierContact>> GetAllAsync(
			CancellationToken ct)
		{
			return await _db.Set<SupplierContact>()
				.Where(c => !c.IsDeleted)
				.ToListAsync(ct);
		}

		public async Task<SupplierContact> UpdateAsync(
			SupplierContact entity,
			CancellationToken ct)
		{
			_db.Set<SupplierContact>().Update(entity);
			await _db.SaveChangesAsync(ct);
			return entity;
		}

		public async Task<bool> DeleteAsync(
			SupplierContactDeleteDto dto,
			CancellationToken ct)
		{
			var contact = await _db.Set<SupplierContact>()
				.FirstOrDefaultAsync(c =>
					!c.IsDeleted &&
					(dto.ContactID == null || c.ContactID == dto.ContactID) &&
					(dto.SupplierID == null || c.SupplierID == dto.SupplierID) &&
					(dto.Email == null || c.Email == dto.Email) &&
					(dto.Phone == null || c.Phone == dto.Phone),
					ct);

			if (contact == null)
				return false;

			contact.IsDeleted = true;
			contact.UpdatedOn = DateTime.UtcNow;
			await _db.SaveChangesAsync(ct);
			return true;
		}
	}
}