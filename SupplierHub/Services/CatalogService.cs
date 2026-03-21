using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SupplierHub.Services.Interface;
using SupplierHub.Repositories.Interface;
using SupplierHub.DTOs.CatalogDTO;
using SupplierHub.Models;

namespace SupplierHub.Services
{
	public class CatalogService : ICatalogService
	{
		private readonly ICatalogRepository _repo;
		private readonly IMapper _mapper;

		public CatalogService(ICatalogRepository repo, IMapper mapper)
		{
			_repo = repo;
			_mapper = mapper;
		}

		public async Task<CatalogGetByIdDto> CreateAsync(
			CatalogCreateDto dto,
			CancellationToken ct)
		{
			var entity = _mapper.Map<Catalog>(dto);

			entity.IsDeleted = false;
			entity.CreatedOn = DateTime.UtcNow;
			entity.UpdatedOn = DateTime.UtcNow;

			var created = await _repo.CreateAsync(entity, ct);
			return _mapper.Map<CatalogGetByIdDto>(created);
		}

		public async Task<CatalogGetByIdDto?> GetByIdAsync(long itemId, CancellationToken ct)
		{
			var item = await _repo.GetByIdAsync(itemId, ct);
			return _mapper.Map<CatalogGetByIdDto>(item);
		}

		public async Task<IEnumerable<CatalogGetAllDto>> GetAllAsync(CancellationToken ct)
		{
			var items = await _repo.GetAllAsync(ct);
			return _mapper.Map<List<CatalogGetAllDto>>(items);
		}

		public async Task<CatalogGetByIdDto?> UpdateAsync(
			CatalogUpdateDto dto,
			CancellationToken ct)
		{
			var item = await _repo.GetByIdAsync(dto.ItemID, ct);
			if (item == null)
				return null;

			_mapper.Map(dto, item);
			item.UpdatedOn = DateTime.UtcNow;

			var updated = await _repo.UpdateAsync(item, ct);
			return _mapper.Map<CatalogGetByIdDto>(updated);
		}

		public async Task<bool> DeleteAsync(
			CatalogDeleteDto dto,
			CancellationToken ct)
		{
			return await _repo.DeleteAsync(dto, ct);
		}
	}
}