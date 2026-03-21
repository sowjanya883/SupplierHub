using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SupplierHub.Repositories.Interface;
using SupplierHub.DTOs.SupplierDTO;
using SupplierHub.Models;

namespace SupplierHub.Repositories
{
	public class SuppliersRepository : ISuppliersRepository
	{
		private readonly AppDbContext _db;

		public SuppliersRepository(AppDbContext db)
		{
			_db = db;
		}

		public async Task<Supplier> CreateAsync(Supplier entity, CancellationToken ct)
		{
			await _db.Set<Supplier>().AddAsync(entity, ct);
			await _db.SaveChangesAsync(ct);
			return entity;
		}

		public async Task<Supplier?> GetByIdAsync(long supplierId, CancellationToken ct)
		{
			return await _db.Set<Supplier>()
				.FirstOrDefaultAsync(s => s.SupplierID == supplierId && !s.IsDeleted, ct);
		}

		public async Task<IEnumerable<Supplier>> GetAllAsync(CancellationToken ct)
		{
			return await _db.Set<Supplier>()
				.Where(s => !s.IsDeleted)
				.ToListAsync(ct);
		}

		public async Task<Supplier> UpdateAsync(Supplier entity, CancellationToken ct)
		{
			_db.Set<Supplier>().Update(entity);
			await _db.SaveChangesAsync(ct);
			return entity;
		}

		public async Task<bool> DeleteAsync(SupplierDeleteDto dto, CancellationToken ct)
		{
			var supplier = await _db.Set<Supplier>()
				.FirstOrDefaultAsync(s =>
					!s.IsDeleted &&
					(dto.SupplierID == null || s.SupplierID == dto.SupplierID) &&
					(dto.LegalName == null || s.LegalName == dto.LegalName) &&
					(dto.TaxID == null || s.TaxID == dto.TaxID) &&
					(dto.DunsOrRegNo == null || s.DunsOrRegNo == dto.DunsOrRegNo),
					ct);

			if (supplier == null)
				return false;

			supplier.IsDeleted = true;
			supplier.UpdatedOn = DateTime.UtcNow;

			await _db.SaveChangesAsync(ct);
			return true;
		}
	}
}