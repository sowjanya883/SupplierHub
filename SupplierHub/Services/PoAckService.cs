using AutoMapper;
using SupplierHub.Constants.Enum;
using SupplierHub.DTOs.PoAckDTO;
using SupplierHub.Models;
using SupplierHub.Repositories.Interface;
using SupplierHub.Services.Interface;

namespace SupplierHub.Services
{
	public class PoAckService : IPoAckService
	{
		private readonly IPoAckRepository _repository;
		private readonly IMapper _mapper;

		public PoAckService(IPoAckRepository repository, IMapper mapper)
		{
			_repository = repository;
			_mapper = mapper;
		}

		public async Task<IEnumerable<PoAckResponseDto>> GetAllAsync()
		{
			var acks = await _repository.GetAllActiveAsync();
			return _mapper.Map<IEnumerable<PoAckResponseDto>>(acks);
		}

		public async Task<PoAckResponseDto?> GetByIdAsync(long id)
		{
			var ack = await _repository.GetByIdAsync(id);
			return ack == null ? null : _mapper.Map<PoAckResponseDto>(ack);
		}

		public async Task<PoAckResponseDto> CreateAsync(PoAckCreateDto createDto)
		{
			// Business Rule: Require notes if countering
			if (createDto.Decision == PoAckDecision.Counter && string.IsNullOrWhiteSpace(createDto.CounterNotes))
			{
				throw new ArgumentException("Counter notes are strictly required when countering a Purchase Order.");
			}

			var ack = _mapper.Map<PoAck>(createDto);

			ack.CreatedOn = DateTime.UtcNow;
			ack.UpdatedOn = DateTime.UtcNow;
			ack.IsDeleted = false;

			if (string.IsNullOrWhiteSpace(ack.Status))
			{
				ack.Status = nameof(PoAckStatus.Active);
			}

			await _repository.AddAsync(ack);
			await _repository.SaveChangesAsync();

			return _mapper.Map<PoAckResponseDto>(ack);
		}

		public async Task<PoAckResponseDto?> UpdateAsync(long id, PoAckUpdateDto updateDto)
		{
			var existingAck = await _repository.GetByIdAsync(id);
			if (existingAck == null) return null;

			if (updateDto.Decision == PoAckDecision.Counter && string.IsNullOrWhiteSpace(updateDto.CounterNotes))
			{
				throw new ArgumentException("Counter notes are strictly required when countering a Purchase Order.");
			}

			_mapper.Map(updateDto, existingAck);
			existingAck.UpdatedOn = DateTime.UtcNow;

			_repository.Update(existingAck);
			await _repository.SaveChangesAsync();

			return _mapper.Map<PoAckResponseDto>(existingAck);
		}
	}
}