using AutoMapper;
using SupplierHub.DTOs.ErpExportRefDTO;
using SupplierHub.Models;
using SupplierHub.Repositories.Interface;
using SupplierHub.Services.Interface;
using SupplierHub.Constants.Enum; // Using your enums

namespace SupplierHub.Services
{
	public class ErpExportRefService : IErpExportRefService
	{
		private readonly IErpExportRefRepository _repository;
		private readonly IMapper _mapper;

		public ErpExportRefService(IErpExportRefRepository repository, IMapper mapper)
		{
			_repository = repository;
			_mapper = mapper;
		}

		public async Task<IEnumerable<ErpExportRefResponseDto>> GetAllAsync()
		{
			var records = await _repository.GetAllActiveAsync();
			return _mapper.Map<IEnumerable<ErpExportRefResponseDto>>(records);
		}

		public async Task<ErpExportRefResponseDto?> GetByIdAsync(long id)
		{
			var record = await _repository.GetByIdAsync(id);
			if (record == null) return null;

			return _mapper.Map<ErpExportRefResponseDto>(record);
		}

		public async Task<ErpExportRefResponseDto> CreateAsync(ErpExportRefCreateDto createDto)
		{
			var record = _mapper.Map<ErpExportRef>(createDto);

			// Business Logic: Audit fields
			record.CreatedOn = DateTime.UtcNow;
			record.UpdatedOn = DateTime.UtcNow;
			record.IsDeleted = false;

			// Enforce Enum string representation if Status is not provided
			if (string.IsNullOrWhiteSpace(record.Status))
			{
				record.Status = nameof(ErpExportRefStatus.Queued);
			}

			await _repository.AddAsync(record);
			await _repository.SaveChangesAsync();

			return _mapper.Map<ErpExportRefResponseDto>(record);
		}

		public async Task<ErpExportRefResponseDto?> UpdateAsync(long id, ErpExportRefUpdateDto updateDto)
		{
			var existingRecord = await _repository.GetByIdAsync(id);
			if (existingRecord == null) return null;

			// Maps updated fields onto the tracked entity
			_mapper.Map(updateDto, existingRecord);
			existingRecord.UpdatedOn = DateTime.UtcNow;

			_repository.Update(existingRecord);
			await _repository.SaveChangesAsync();

			return _mapper.Map<ErpExportRefResponseDto>(existingRecord);
		}

		public async Task<bool> DeleteAsync(long id)
		{
			var record = await _repository.GetByIdAsync(id);
			if (record == null) return false;

			// Soft delete
			record.IsDeleted = true;
			record.UpdatedOn = DateTime.UtcNow;

			_repository.Update(record);
			await _repository.SaveChangesAsync();

			return true;
		}
	}
}