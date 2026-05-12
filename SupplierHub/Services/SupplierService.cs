using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SupplierHub.DTOs.SupplierDTO;
using SupplierHub.Services.Interface;
using SupplierHub.Repositories.Interface;
using SupplierHub.Models;
using Microsoft.Extensions.Logging;

namespace SupplierHub.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly ISuppliersRepository _repo;
        private readonly IMapper _mapper;
		private readonly INotificationService _notif;

		public SupplierService(ISuppliersRepository repo, IMapper mapper, INotificationService notif)
        {
            _repo = repo;
            _mapper = mapper;
            _notif = notif;
        }

		// CREATE
		public async Task<GetSupplierByIdDto?> CreateAsync(SupplierCreateDto dto, CancellationToken ct)
		{
			var entity = _mapper.Map<Supplier>(dto);
			entity.CreatedOn = DateTime.UtcNow;
			entity.UpdatedOn = DateTime.UtcNow;

			var created = await _repo.CreateAsync(entity, ct);

			// Notify Buyers and CategoryManagers
			var msg = $"New supplier added: {created.LegalName}";
			await _notif.SendToRoleAsync("Buyer", msg, "Supplier", created.SupplierID);
			await _notif.SendToRoleAsync("CategoryManager", msg, "Supplier", created.SupplierID);
			await _notif.SendToRoleAsync("Admin", msg, "Supplier", created.SupplierID);

			return _mapper.Map<GetSupplierByIdDto>(created);
		}

		// GET ALL
		public async Task<IEnumerable<GetAllSupplierDto>> GetAllAsync(CancellationToken ct)
        {
            var suppliers = await _repo.GetAllAsync(ct);
            return _mapper.Map<List<GetAllSupplierDto>>(suppliers);
        }

        // GET BY ID
        public async Task<GetSupplierByIdDto?> GetByIdAsync(long supplierId, CancellationToken ct)
        {
            var supplier = await _repo.GetByIdAsync(supplierId, ct);
            return _mapper.Map<GetSupplierByIdDto>(supplier);
        }

        // UPDATE
        public async Task<GetSupplierByIdDto?> UpdateAsync(UpdateSupplierDto dto, CancellationToken ct)
        {
            var supplier = await _repo.GetByIdAsync(dto.SupplierID, ct);
            if (supplier == null)
                return null;

            _mapper.Map(dto, supplier);
            supplier.UpdatedOn = DateTime.UtcNow;

            var updated = await _repo.UpdateAsync(supplier, ct);
            return _mapper.Map<GetSupplierByIdDto>(updated);
        }

        // DELETE (SOFT DELETE)
        public async Task<bool> DeleteAsync(SupplierDeleteDto dto, CancellationToken ct)
        {
            return await _repo.DeleteAsync(dto, ct);
        }
    }
}