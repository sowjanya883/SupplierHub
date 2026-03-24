using SupplierHub.Models;

namespace SupplierHub.Repositories.Interface
{
    public interface IReceivingQualityRepository
    {
        // --- GRN Header ---
        Task<GrnRef> AddGrnAsync(GrnRef grn);
        Task<List<GrnRef>> GetAllGrnsAsync();
        Task<GrnRef?> GetGrnByIdAsync(long id);
        Task<GrnRef?> UpdateGrnAsync(GrnRef grn);
        Task<bool> DeleteGrnAsync(long id); // New: Delete

        // --- GRN Items ---
        Task<GrnItemRef> AddGrnItemAsync(GrnItemRef item);
        Task<List<GrnItemRef>> GetAllGrnItemsAsync(); // New: Get All
        Task<GrnItemRef?> GetGrnItemByIdAsync(long id);
        Task<List<GrnItemRef>> GetItemsByGrnIdAsync(long grnId);
        Task<GrnItemRef?> UpdateGrnItemAsync(GrnItemRef item);
        Task<bool> DeleteGrnItemAsync(long id); // New: Delete

        // --- Inspection ---
        Task<Inspection> AddInspectionAsync(Inspection inspection);
        Task<List<Inspection>> GetAllInspectionsAsync(); // New: Get All
        Task<Inspection?> GetInspectionByIdAsync(long id);
        Task<List<Inspection>> GetInspectionsByItemIdAsync(long grnItemId);
        Task<Inspection?> UpdateInspectionAsync(Inspection inspection);
        Task<bool> DeleteInspectionAsync(long id); // New: Delete

        // --- NCR ---
        Task<Ncr> AddNcrAsync(Ncr ncr);
        Task<List<Ncr>> GetAllNcrsAsync(); // New: Get All
        Task<Ncr?> GetNcrByIdAsync(long id);
        Task<List<Ncr>> GetNcrsByItemIdAsync(long grnItemId);
        Task<Ncr?> UpdateNcrAsync(Ncr ncr);
        Task<bool> DeleteNcrAsync(long id); // New: Delete
    }
}