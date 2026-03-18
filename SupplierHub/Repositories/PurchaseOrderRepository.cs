using SupplierHub.Models;
using Microsoft.EntityFrameworkCore;
using SupplierHub.Repositories.Interface;

namespace SupplierHub.Repositories
{
	public class PurchaseOrderRepository : IPurchaseOrderRepository
	{
		private readonly AppDbContext _context;

		public PurchaseOrderRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task<PurchaseOrder?> GetByIdAsync(long id)
		{
			return await _context.PurchaseOrders
				.FirstOrDefaultAsync(x => x.PoID == id && !x.IsDeleted);
		}

		public async Task<IEnumerable<PurchaseOrder>> GetAllActiveAsync()
		{
			return await _context.PurchaseOrders
				.Where(x => !x.IsDeleted)
				.ToListAsync();
		}

		public async Task AddAsync(PurchaseOrder order) => await _context.PurchaseOrders.AddAsync(order);

		public void Update(PurchaseOrder order) => _context.PurchaseOrders.Update(order);

		public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
	}
}
