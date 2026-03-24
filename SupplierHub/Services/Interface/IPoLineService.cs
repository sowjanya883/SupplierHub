using SupplierHub.DTOs.PoLineDTO;

namespace SupplierHub.Services.Interface
{
	public interface IPoLineService
	{
		Task<IEnumerable<PoLineResponseDto>> GetAllByPoIdAsync(long poId);
		Task<PoLineResponseDto?> GetByIdAsync(long id);
		Task<PoLineResponseDto> CreateAsync(PoLineCreateDto createDto);
		Task<PoLineResponseDto?> UpdateAsync(long id, PoLineUpdateDto updateDto);
		Task<bool> DeleteAsync(long id);
	}
}