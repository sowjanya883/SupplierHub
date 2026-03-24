using AutoMapper;
using SupplierHub.DTOs.PoLineDTO;
using SupplierHub.Models;
using SupplierHub.Repositories.Interface;
using SupplierHub.Services.Interface;

namespace SupplierHub.Services
{
	public class PoLineService : IPoLineService
	{
		private readonly IPoLineRepository _repository;
		private readonly IMapper _mapper;

		public PoLineService(IPoLineRepository repository, IMapper mapper)
		{
			_repository = repository;
			_mapper = mapper;
		}

		public async Task<IEnumerable<PoLineResponseDto>> GetAllByPoIdAsync(long poId)
		{
			var lines = await _repository.GetAllByPoIdAsync(poId);
			return _mapper.Map<IEnumerable<PoLineResponseDto>>(lines);
		}

		public async Task<PoLineResponseDto?> GetByIdAsync(long id)
		{
			var line = await _repository.GetByIdAsync(id);
			return line == null ? null : _mapper.Map<PoLineResponseDto>(line);
		}

		public async Task<PoLineResponseDto> CreateAsync(PoLineCreateDto createDto)
		{
			var line = _mapper.Map<PoLine>(createDto);

			line.CreatedOn = DateTime.UtcNow;
			line.UpdatedOn = DateTime.UtcNow;
			line.IsDeleted = false;

			// Business Logic: Auto-calculate line total
			line.LineTotal = (line.Qty ?? 0) * (line.UnitPrice ?? 0);

			await _repository.AddAsync(line);
			await _repository.SaveChangesAsync();

			return _mapper.Map<PoLineResponseDto>(line);
		}

		public async Task<PoLineResponseDto?> UpdateAsync(long id, PoLineUpdateDto updateDto)
		{
			var existingLine = await _repository.GetByIdAsync(id);
			if (existingLine == null) return null;

			_mapper.Map(updateDto, existingLine);
			existingLine.UpdatedOn = DateTime.UtcNow;

			// Recalculate Line Total on update
			existingLine.LineTotal = (existingLine.Qty ?? 0) * (existingLine.UnitPrice ?? 0);

			_repository.Update(existingLine);
			await _repository.SaveChangesAsync();

			return _mapper.Map<PoLineResponseDto>(existingLine);
		}

		public async Task<bool> DeleteAsync(long id)
		{
			var line = await _repository.GetByIdAsync(id);
			if (line == null) return false;

			line.IsDeleted = true;
			line.UpdatedOn = DateTime.UtcNow;

			_repository.Update(line);
			await _repository.SaveChangesAsync();
			return true;
		}
	}
}