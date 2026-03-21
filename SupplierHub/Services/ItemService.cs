using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SupplierHub.Services.Interface;
using SupplierHub.Repositories.Interface;
using SupplierHub.DTOs.ItemDTO;
using SupplierHub.Models;

namespace SupplierHub.Services
{
	public class ItemService : IItemService
	{
		private readonly IItemRepository _repo;
		private readonly IMapper _mapper;

		public ItemService(IItemRepository repo, IMapper mapper)
		{
			_repo = repo;
			_mapper = mapper;
		}

		public async Task<ItemGetByIdDto> CreateAsync(
			ItemCreateDto dto,
			CancellationToken ct)
		{
			var entity = _mapper.Map<Item>(dto);

			entity.IsDeleted = false;
			entity.CreatedOn = DateTime.UtcNow;
			entity.UpdatedOn = DateTime.UtcNow;

			var created = await _repo.CreateAsync(entity, ct);
			return _mapper.Map<ItemGetByIdDto>(created);
		}

		public async Task<ItemGetByIdDto?> GetByIdAsync(long itemId, CancellationToken ct)
		{
			var item = await _repo.GetByIdAsync(itemId, ct);
			return _mapper.Map<ItemGetByIdDto>(item);
		}

		public async Task<IEnumerable<ItemGetAllDto>> GetAllAsync(CancellationToken ct)
		{
			var items = await _repo.GetAllAsync(ct);
			return _mapper.Map<List<ItemGetAllDto>>(items);
		}

		public async Task<ItemGetByIdDto?> UpdateAsync(
			ItemUpdateDto dto,
			CancellationToken ct)
		{
			var item = await _repo.GetByIdAsync(dto.ItemID, ct);
			if (item == null)
				return null;

			_mapper.Map(dto, item);
			item.UpdatedOn = DateTime.UtcNow;

			var updated = await _repo.UpdateAsync(item, ct);
			return _mapper.Map<ItemGetByIdDto>(updated);
		}

		public async Task<bool> DeleteAsync(
			ItemDeleteDto dto,
			CancellationToken ct)
		{
			return await _repo.DeleteAsync(dto, ct);
		}
	}
}