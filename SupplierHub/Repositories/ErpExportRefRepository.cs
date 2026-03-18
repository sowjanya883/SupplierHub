using Microsoft.EntityFrameworkCore;
using SupplierHub.Models;
using SupplierHub.Repositories.Interface;
using SupplierHub.Config; // Ensure this points to your AppDbContext namespace

namespace SupplierHub.Repositories
{
	public class ErpExportRefRepository : IErpExportRefRepository
	{
		private readonly AppDbContext _context;

		public ErpExportRefRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<ErpExportRef>> GetAllActiveAsync()
		{
			return await _context.ErpExportRefs
				.Where(x => !x.IsDeleted)
				.ToListAsync();
		}

		public async Task<ErpExportRef?> GetByIdAsync(long id)
		{
			return await _context.ErpExportRefs
				.FirstOrDefaultAsync(x => x.ErprefID == id && !x.IsDeleted);
		}

		public async Task AddAsync(ErpExportRef erpExportRef)
		{
			await _context.ErpExportRefs.AddAsync(erpExportRef);
		}

		public void Update(ErpExportRef erpExportRef)
		{
			_context.ErpExportRefs.Update(erpExportRef);
		}

		public async Task SaveChangesAsync()
		{
			await _context.SaveChangesAsync();
		}
	}
}