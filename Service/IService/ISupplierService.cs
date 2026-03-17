using SupplierHub.DTOs.SupplierDTO;


namespace SupplierHub.Services
{
	public interface ISupplierService
	{
		Task<SupplierResponseDto?> GetByIdAsync(long id);
		Task<List<SupplierListDto>> GetAllAsync();
		Task<SupplierResponseDto> CreateAsync(SupplierCreateDto dto);
		Task<SupplierResponseDto?> UpdateAsync(SupplierUpdateDto dto);
	}
}