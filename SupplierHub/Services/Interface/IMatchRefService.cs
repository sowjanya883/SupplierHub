using SupplierHub.DTOs.MatchRefDTO;

namespace SupplierHub.Services.Interface
{
	public interface IMatchRefService
	{
		Task<IEnumerable<MatchRefResponseDto>> GetByInvoiceIdAsync(long invoiceId);
		Task<MatchRefResponseDto?> GetByIdAsync(long id);
		Task<MatchRefResponseDto> CreateAsync(MatchRefCreateDto createDto);
		Task<MatchRefResponseDto?> UpdateAsync(long id, MatchRefUpdateDto updateDto);
	}
}