using Microsoft.EntityFrameworkCore;
using SupplierHub.Models;
using SupplierHub.Repositories.Interface;

namespace SupplierHub.Repositories
{
    public class ReceivingQualityRepository : IReceivingQualityRepository
    {
        private readonly AppDbContext _db;

        public ReceivingQualityRepository(AppDbContext db)
        {
            _db = db;
        }

        // =======================
        // --- GRN Header ---
        // =======================
        public Task<GrnRef?> GetGrnByIdAsync(long id) =>
            _db.GrnRefs.FirstOrDefaultAsync(x => x.GrnID == id && !x.IsDeleted);

        public Task<List<GrnRef>> GetAllGrnsAsync() =>
            _db.GrnRefs.Where(x => !x.IsDeleted).ToListAsync();

        public async Task<GrnRef> AddGrnAsync(GrnRef grn)
        {
            if (grn.AsnID.HasValue && grn.AsnID.Value == 0)
                grn.AsnID = null;

            if (grn.AsnID.HasValue)
            {
                var exists = await _db.Asns.AnyAsync(a => a.AsnID == grn.AsnID.Value);
                if (!exists)
                    throw new InvalidOperationException($"Asn with ID {grn.AsnID.Value} does not exist.");
            }

            grn.CreatedOn = DateTime.UtcNow;
            grn.UpdatedOn = DateTime.UtcNow;
            grn.IsDeleted = false;

            _db.GrnRefs.Add(grn);
            await _db.SaveChangesAsync();
            return grn;
        }

        public async Task<GrnRef?> UpdateGrnAsync(GrnRef grn)
        {
            _db.GrnRefs.Update(grn);
            await _db.SaveChangesAsync();
            return grn;
        }

        public async Task<bool> DeleteGrnAsync(long id)
        {
            var entity = await GetGrnByIdAsync(id);
            if (entity == null) return false;
            entity.IsDeleted = true;
            _db.GrnRefs.Update(entity);
            await _db.SaveChangesAsync();
            return true;
        }

        // =======================
        // --- GRN Items ---
        // =======================
        public Task<GrnItemRef?> GetGrnItemByIdAsync(long id) =>
            _db.GrnItemRefs.FirstOrDefaultAsync(x => x.GrnItemID == id && !x.IsDeleted);

        public Task<List<GrnItemRef>> GetAllGrnItemsAsync() =>
            _db.GrnItemRefs.Where(x => !x.IsDeleted).ToListAsync();

        public Task<List<GrnItemRef>> GetItemsByGrnIdAsync(long grnId) =>
            _db.GrnItemRefs.Where(x => x.GrnID == grnId && !x.IsDeleted).ToListAsync();

        public async Task<GrnItemRef> AddGrnItemAsync(GrnItemRef item)
        {
            _db.GrnItemRefs.Add(item);
            await _db.SaveChangesAsync();
            return item;
        }

        public async Task<GrnItemRef?> UpdateGrnItemAsync(GrnItemRef item)
        {
            _db.GrnItemRefs.Update(item);
            await _db.SaveChangesAsync();
            return item;
        }

        public async Task<bool> DeleteGrnItemAsync(long id)
        {
            var entity = await GetGrnItemByIdAsync(id);
            if (entity == null) return false;
            entity.IsDeleted = true;
            _db.GrnItemRefs.Update(entity);
            await _db.SaveChangesAsync();
            return true;
        }

        // =======================
        // --- Inspection ---
        // =======================
        public Task<Inspection?> GetInspectionByIdAsync(long id) =>
            _db.Inspections.FirstOrDefaultAsync(x => x.InspID == id && !x.IsDeleted);

        public Task<List<Inspection>> GetAllInspectionsAsync() =>
            _db.Inspections.Where(x => !x.IsDeleted).ToListAsync();

        public Task<List<Inspection>> GetInspectionsByItemIdAsync(long grnItemId) =>
            _db.Inspections.Where(x => x.GrnItemID == grnItemId && !x.IsDeleted).ToListAsync();

        public async Task<Inspection> AddInspectionAsync(Inspection inspection)
        {
            _db.Inspections.Add(inspection);
            await _db.SaveChangesAsync();
            return inspection;
        }

        public async Task<Inspection?> UpdateInspectionAsync(Inspection inspection)
        {
            _db.Inspections.Update(inspection);
            await _db.SaveChangesAsync();
            return inspection;
        }

        public async Task<bool> DeleteInspectionAsync(long id)
        {
            var entity = await GetInspectionByIdAsync(id);
            if (entity == null) return false;
            entity.IsDeleted = true;
            _db.Inspections.Update(entity);
            await _db.SaveChangesAsync();
            return true;
        }

        // =======================
        // --- NCR ---
        // =======================
        public Task<Ncr?> GetNcrByIdAsync(long id) =>
            _db.Ncrs.FirstOrDefaultAsync(x => x.NcrID == id && !x.IsDeleted);

        public Task<List<Ncr>> GetAllNcrsAsync() =>
            _db.Ncrs.Where(x => !x.IsDeleted).ToListAsync();

        public Task<List<Ncr>> GetNcrsByItemIdAsync(long grnItemId) =>
            _db.Ncrs.Where(x => x.GrnItemID == grnItemId && !x.IsDeleted).ToListAsync();

        public async Task<Ncr> AddNcrAsync(Ncr ncr)
        {
            _db.Ncrs.Add(ncr);
            await _db.SaveChangesAsync();
            return ncr;
        }

        public async Task<Ncr?> UpdateNcrAsync(Ncr ncr)
        {
            _db.Ncrs.Update(ncr);
            await _db.SaveChangesAsync();
            return ncr;
        }

        public async Task<bool> DeleteNcrAsync(long id)
        {
            var entity = await GetNcrByIdAsync(id);
            if (entity == null) return false;
            entity.IsDeleted = true;
            _db.Ncrs.Update(entity);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}