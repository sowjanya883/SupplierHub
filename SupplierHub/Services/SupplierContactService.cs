using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SupplierHub.Services.Interface;
using SupplierHub.Repositories.Interface;
using SupplierHub.DTOs.SupplierContactDTO;
using SupplierHub.Models;

namespace SupplierHub.Services
{
	public class SupplierContactService : ISupplierContactService
	{
		private readonly ISupplierContactRepository _repo;
		private readonly IMapper _mapper;

		public SupplierContactService(ISupplierContactRepository repo, IMapper mapper)
		{
			_repo = repo;
			_mapper = mapper;
		}

		public async Task<SupplierContactGetByIdDto> CreateAsync(
			SupplierContactCreateDto dto,
			CancellationToken ct)
		{
			var entity = _mapper.Map<SupplierContact>(dto);

			entity.IsDeleted = false;
			entity.CreatedOn = DateTime.UtcNow;
			entity.UpdatedOn = DateTime.UtcNow;

			var created = await _repo.CreateAsync(entity, ct);
			return _mapper.Map<SupplierContactGetByIdDto>(created);
		}

		public async Task<SupplierContactGetByIdDto?> GetByIdAsync(long contactId, CancellationToken ct)
		{
			var contact = await _repo.GetByIdAsync(contactId, ct);
			return _mapper.Map<SupplierContactGetByIdDto>(contact);
		}

		public async Task<IEnumerable<SupplierContactGetAllDto>> GetAllAsync(CancellationToken ct)
		{
			var contacts = await _repo.GetAllAsync(ct);
			return _mapper.Map<List<SupplierContactGetAllDto>>(contacts);
		}

		public async Task<SupplierContactGetByIdDto?> UpdateAsync(
			SupplierContactUpdateDto dto,
			CancellationToken ct)
		{
			var contact = await _repo.GetByIdAsync(dto.ContactID, ct);
			if (contact == null)
				return null;

			_mapper.Map(dto, contact);
			contact.UpdatedOn = DateTime.UtcNow;

			var updated = await _repo.UpdateAsync(contact, ct);
			return _mapper.Map<SupplierContactGetByIdDto>(updated);
		}

		public async Task<bool> DeleteAsync(
			SupplierContactDeleteDto dto,
			CancellationToken ct)
		{
			return await _repo.DeleteAsync(dto, ct);
		}
	}
}