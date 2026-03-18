using SupplierHub.DTOs.ErpExportRefDTO;

namespace SupplierHub.Services.Interface
{
	public interface IErpExportRefService
	{
		Task<IEnumerable<ErpExportRefResponseDto>> GetAllAsync();
		Task<ErpExportRefResponseDto?> GetByIdAsync(long id);
		Task<ErpExportRefResponseDto> CreateAsync(ErpExportRefCreateDto createDto);
		Task<ErpExportRefResponseDto?> UpdateAsync(long id, ErpExportRefUpdateDto updateDto);
		Task<bool> DeleteAsync(long id);
	}
}