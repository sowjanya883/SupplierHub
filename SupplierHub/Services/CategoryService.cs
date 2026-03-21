using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SupplierHub.Services.Interface;
using SupplierHub.Repositories.Interface;
using SupplierHub.DTOs.CategoryDTO;
using SupplierHub.Models;

namespace SupplierHub.Services
{
	public class CategoryService : ICategoryService
	{
		private readonly ICategoryRepository _repo;
		private readonly IMapper _mapper;

		public CategoryService(ICategoryRepository repo, IMapper mapper)
		{
			_repo = repo;
			_mapper = mapper;
		}

		public async Task<CategoryGetByIdDto> CreateAsync(
			CategoryCreateDto dto,
			CancellationToken ct)
		{
			var entity = _mapper.Map<Category>(dto);

			entity.IsDeleted = false;
			entity.CreatedOn = DateTime.UtcNow;
			entity.UpdatedOn = DateTime.UtcNow;

			var created = await _repo.CreateAsync(entity, ct);
			return _mapper.Map<CategoryGetByIdDto>(created);
		}

		public async Task<CategoryGetByIdDto?> GetByIdAsync(long categoryId, CancellationToken ct)
		{
			var category = await _repo.GetByIdAsync(categoryId, ct);
			return _mapper.Map<CategoryGetByIdDto>(category);
		}

		public async Task<IEnumerable<CategoryGetAllDto>> GetAllAsync(CancellationToken ct)
		{
			var categories = await _repo.GetAllAsync(ct);
			return _mapper.Map<List<CategoryGetAllDto>>(categories);
		}

		public async Task<CategoryGetByIdDto?> UpdateAsync(
			CategoryUpdateDto dto,
			CancellationToken ct)
		{
			var category = await _repo.GetByIdAsync(dto.CategoryID, ct);
			if (category == null)
				return null;

			_mapper.Map(dto, category);
			category.UpdatedOn = DateTime.UtcNow;

			var updated = await _repo.UpdateAsync(category, ct);
			return _mapper.Map<CategoryGetByIdDto>(updated);
		}

		public async Task<bool> DeleteAsync(
			CategoryDeleteDto dto,
			CancellationToken ct)
		{
			return await _repo.DeleteAsync(dto, ct);
		}
	}
}