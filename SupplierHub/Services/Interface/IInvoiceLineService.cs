using SupplierHub.DTOs.InvoiceLineDTO;

namespace SupplierHub.Services.Interface
{
	public interface IInvoiceLineService
	{
		Task<IEnumerable<InvoiceLineResponseDto>> GetByInvoiceIdAsync(long invoiceId);
		Task<InvoiceLineResponseDto?> GetByIdAsync(long id);
		Task<InvoiceLineResponseDto> CreateAsync(InvoiceLineCreateDto createDto);
		Task<InvoiceLineResponseDto?> UpdateAsync(long id, InvoiceLineUpdateDto updateDto);
	}
}