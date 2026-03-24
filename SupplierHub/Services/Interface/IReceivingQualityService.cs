using SupplierHub.DTOs.GrnRefDTO;
using SupplierHub.DTOs.GrnItemRefDTO;
using SupplierHub.DTOs.NcrDTO;
using SupplierHub.DTOs.InspectionDTO;

namespace SupplierHub.Services.Interface
{
    public interface IReceivingQualityService
    {
        // GRN Header
        Task<GrnReadDto> CreateGrnAsync(GrnCreateDto dto);
        Task<List<GrnReadDto>> GetAllGrnsAsync();
        Task<GrnReadDto?> GetGrnByIdAsync(long id);
        Task<GrnReadDto?> UpdateGrnAsync(long id, GrnUpdateDto dto);
        Task<bool> DeleteGrnAsync(long id);

        // GRN Items
        Task<GrnItemReadDto> AddGrnItemAsync(GrnItemCreateDto dto);
        Task<List<GrnItemReadDto>> GetAllGrnItemsAsync();
        Task<GrnItemReadDto?> GetGrnItemByIdAsync(long id);
        Task<List<GrnItemReadDto>> GetGrnItemsByGrnIdAsync(long grnId);
        Task<GrnItemReadDto?> UpdateGrnItemAsync(long id, GrnItemUpdateDto dto);
        Task<bool> DeleteGrnItemAsync(long id);

        // Inspections
        Task<InspectionReadDto> CreateInspectionAsync(InspectionCreateDto dto);
        Task<List<InspectionReadDto>> GetAllInspectionsAsync();
        Task<InspectionReadDto?> GetInspectionByIdAsync(long id);
        Task<List<InspectionReadDto>> GetInspectionsByItemIdAsync(long grnItemId);
        Task<InspectionReadDto?> UpdateInspectionAsync(long id, InspectionUpdateDto dto);
        Task<bool> DeleteInspectionAsync(long id);

        // NCR
        Task<NcrReadDto> CreateNcrAsync(NcrCreateDto dto);
        Task<List<NcrReadDto>> GetAllNcrsAsync();
        Task<NcrReadDto?> GetNcrByIdAsync(long id);
        Task<List<NcrReadDto>> GetNcrsByItemIdAsync(long grnItemId);
        Task<NcrReadDto?> UpdateNcrAsync(long id, NcrUpdateDto dto);
        Task<bool> DeleteNcrAsync(long id);
    }
}