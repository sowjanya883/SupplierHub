using SupplierHub.DTOs.InvoiceDTO;

namespace SupplierHub.Services.Interface
{
	public interface IInvoiceService
	{
		Task<IEnumerable<InvoiceResponseDto>> GetAllAsync();
		Task<InvoiceResponseDto?> GetByIdAsync(long id);
		Task<IEnumerable<InvoiceResponseDto>> GetByPoIdAsync(long poId);
		Task<InvoiceResponseDto> CreateAsync(InvoiceCreateDto createDto);
		Task<InvoiceResponseDto?> UpdateAsync(long id, InvoiceUpdateDto updateDto);
	}
}