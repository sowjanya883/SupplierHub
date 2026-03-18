using SupplierHub.DTOs.PurchaseOrderDTO;

namespace SupplierHub.Services.Interface
{
	public interface IPurchaseOrderService
	{
		Task<IEnumerable<PurchaseOrderResponseDto>> GetAllAsync();
		Task<PurchaseOrderResponseDto?> GetByIdAsync(long id);
		Task<PurchaseOrderResponseDto> CreateAsync(PurchaseOrderCreateDto createdto);
		Task<PurchaseOrderResponseDto?> UpdateAsync(long id, PurchaseOrderUpdateDto updateDto);
		Task<bool> DeleteAsync(long id);
	}
}
