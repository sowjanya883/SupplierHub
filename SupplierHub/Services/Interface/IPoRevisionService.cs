using SupplierHub.DTOs.PoRevisionDTO;

namespace SupplierHub.Services.Interface
{
	public interface IPoRevisionService
	{
		Task<IEnumerable<PoRevisionResponseDto>> GetAllByPoIdAsync(long poId);
		Task<PoRevisionResponseDto?> GetByIdAsync(long id);
		Task<PoRevisionResponseDto> CreateAsync(PoRevisionCreateDto createDto);
	}
}