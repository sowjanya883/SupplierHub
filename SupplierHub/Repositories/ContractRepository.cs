using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SupplierHub.Models;
using SupplierHub.Repositories.Interface;
using SupplierHub.DTOs.ContractDTO;

namespace SupplierHub.Repositories
{
	public class ContractRepository : IContractRepository
	{
		private readonly AppDbContext _db;

		public ContractRepository(AppDbContext db)
		{
			_db = db;
		}

		public async Task<Contract> CreateAsync(
			Contract entity,
			CancellationToken ct)
		{
			await _db.Set<Contract>().AddAsync(entity, ct);
			await _db.SaveChangesAsync(ct);
			return entity;
		}

		public async Task<Contract?> GetByIdAsync(
			long contractId,
			CancellationToken ct)
		{
			return await _db.Set<Contract>()
				.FirstOrDefaultAsync(c =>
					c.ContractID == contractId &&
					!c.IsDeleted,
					ct);
		}

		public async Task<IEnumerable<Contract>> GetAllAsync(
			CancellationToken ct)
		{
			return await _db.Set<Contract>()
				.Where(c => !c.IsDeleted)
				.ToListAsync(ct);
		}

		public async Task<Contract> UpdateAsync(
			Contract entity,
			CancellationToken ct)
		{
			_db.Set<Contract>().Update(entity);
			await _db.SaveChangesAsync(ct);
			return entity;
		}

		public async Task<bool> DeleteAsync(
			ContractDeleteDto dto,
			CancellationToken ct)
		{
			var contract = await _db.Set<Contract>()
				.FirstOrDefaultAsync(c =>
					!c.IsDeleted &&
					(dto.ContractID == null || c.ContractID == dto.ContractID) &&
					(dto.SupplierID == null || c.SupplierID == dto.SupplierID) &&
					(dto.ItemID == null || c.ItemID == dto.ItemID),
					ct);

			if (contract == null)
				return false;

			contract.IsDeleted = true;
			contract.UpdatedOn = DateTime.UtcNow;
			await _db.SaveChangesAsync(ct);
			return true;
		}
	}
}