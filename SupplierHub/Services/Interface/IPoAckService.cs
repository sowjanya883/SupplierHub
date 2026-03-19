using SupplierHub.DTOs.PoAckDTO;

namespace SupplierHub.Services.Interface
{
	public interface IPoAckService
	{
		Task<IEnumerable<PoAckResponseDto>> GetAllAsync();
		Task<PoAckResponseDto?> GetByIdAsync(long id);
		Task<PoAckResponseDto> CreateAsync(PoAckCreateDto createDto);
		Task<PoAckResponseDto?> UpdateAsync(long id, PoAckUpdateDto updateDto);
	}
}