using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SupplierHub.DTOs.CatalogDTO;
using SupplierHub.Models;
using SupplierHub.Repositories.Interface;
using SupplierHub.Services.Interface;

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

		public async Task<CatalogCreateDto> CreateAsync(
			CatalogCreateDto dto,
			CancellationToken ct = default)
		{
			var entity = _mapper.Map<Catalog>(dto);

			entity.CreatedOn = DateTime.UtcNow;
			entity.UpdatedOn = entity.CreatedOn;
			entity.IsDeleted = false;

			await _repo.CreateAsync(entity);

			return _mapper.Map<CatalogCreateDto>(entity);
		}

		public async Task<IEnumerable<object>> GetAllAsync(CancellationToken ct = default)
		{
			return await _repo.GetAllAsync();
		}

		public async Task<object?> GetByIdAsync(long catalogId, CancellationToken ct = default)
		{
			return await _repo.GetByIdAsync(catalogId);
		}

		public async Task<object?> UpdateAsync(
			CatalogUpdateDto dto,
			CancellationToken ct = default)
		{
			var existing = await _repo.GetByIdAsync(dto.CatalogID);
			if (existing == null)
				return null;

			_mapper.Map(dto, existing);
			existing.UpdatedOn = DateTime.UtcNow;

			await _repo.UpdateAsync(existing);

			return existing;
		}

		public async Task<bool> DeleteAsync(
			CatalogDeleteDto dto,
			CancellationToken ct = default)
		{
			if (!dto.CatalogID.HasValue)
				return false;

			return await _repo.DeleteAsync(dto.CatalogID.Value);
		}
	}
}