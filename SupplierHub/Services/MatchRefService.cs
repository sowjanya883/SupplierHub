using AutoMapper;
using SupplierHub.Constants.Enum;
using SupplierHub.DTOs.MatchRefDTO;
using SupplierHub.Models;
using SupplierHub.Repositories.Interface;
using SupplierHub.Services.Interface;

namespace SupplierHub.Services
{
	public class MatchRefService : IMatchRefService
	{
		private readonly IMatchRefRepository _repository;
		private readonly IMapper _mapper;

		public MatchRefService(IMatchRefRepository repository, IMapper mapper)
		{
			_repository = repository;
			_mapper = mapper;
		}

		public async Task<IEnumerable<MatchRefResponseDto>> GetByInvoiceIdAsync(long invoiceId) =>
			_mapper.Map<IEnumerable<MatchRefResponseDto>>(await _repository.GetByInvoiceIdAsync(invoiceId));

		public async Task<MatchRefResponseDto?> GetByIdAsync(long id)
		{
			var match = await _repository.GetByIdAsync(id);
			return match == null ? null : _mapper.Map<MatchRefResponseDto>(match);
		}

		public async Task<MatchRefResponseDto> CreateAsync(MatchRefCreateDto createDto)
		{
			var matchRecord = _mapper.Map<MatchRef>(createDto);

			// Set standard audit fields and your specific defaults
			matchRecord.CreatedOn = DateTime.UtcNow;
			matchRecord.UpdatedOn = DateTime.UtcNow;

			// Defaulting to your specific Enum structures
			matchRecord.Result = MatchResult.Pending;
			matchRecord.Status = MatchRefStatus.Active;

			await _repository.AddAsync(matchRecord);
			await _repository.SaveChangesAsync();

			return _mapper.Map<MatchRefResponseDto>(matchRecord);
		}

		public async Task<MatchRefResponseDto?> UpdateAsync(long id, MatchRefUpdateDto updateDto)
		{
			var existing = await _repository.GetByIdAsync(id);
			if (existing == null) return null;

			_mapper.Map(updateDto, existing);
			existing.UpdatedOn = DateTime.UtcNow;

			_repository.Update(existing);
			await _repository.SaveChangesAsync();

			return _mapper.Map<MatchRefResponseDto>(existing);
		}
	}
}