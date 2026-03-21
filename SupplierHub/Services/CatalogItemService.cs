using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SupplierHub.Services.Interface;
using SupplierHub.Repositories.Interface;
using SupplierHub.DTOs.CatalogItemDTO;
using SupplierHub.Models;

namespace SupplierHub.Services
{
	public class CatalogItemService : ICatalogItemService
	{
		private readonly ICatalogItemRepository _repo;
		private readonly IMapper _mapper;

		public CatalogItemService(ICatalogItemRepository repo, IMapper mapper)
		{
			_repo = repo;
			_mapper = mapper;
		}

		public async Task<CatalogItemGetByIdDto> CreateAsync(
			CatalogItemCreateDto dto,
			CancellationToken ct)
		{
			var entity = _mapper.Map<CatalogItem>(dto);

			entity.IsDeleted = false;
			entity.CreatedOn = DateTime.UtcNow;
			entity.UpdatedOn = DateTime.UtcNow;

			var created = await _repo.CreateAsync(entity, ct);
			return _mapper.Map<CatalogItemGetByIdDto>(created);
		}

		public async Task<CatalogItemGetByIdDto?> GetByIdAsync(long catItemId, CancellationToken ct)
		{
			var item = await _repo.GetByIdAsync(catItemId, ct);
			return _mapper.Map<CatalogItemGetByIdDto>(item);
		}

		public async Task<IEnumerable<CatalogItemGetAllDto>> GetAllAsync(CancellationToken ct)
		{
			var items = await _repo.GetAllAsync(ct);
			return _mapper.Map<List<CatalogItemGetAllDto>>(items);
		}

		public async Task<CatalogItemGetByIdDto?> UpdateAsync(
			CatalogItemUpdateDto dto,
			CancellationToken ct)
		{
			var item = await _repo.GetByIdAsync(dto.CatItemID, ct);
			if (item == null)
				return null;

			_mapper.Map(dto, item);
			item.UpdatedOn = DateTime.UtcNow;

			var updated = await _repo.UpdateAsync(item, ct);
			return _mapper.Map<CatalogItemGetByIdDto>(updated);
		}

		public async Task<bool> DeleteAsync(
			CatalogItemDeleteDto dto,
			CancellationToken ct)
		{
			return await _repo.DeleteAsync(dto, ct);
		}
	}
}