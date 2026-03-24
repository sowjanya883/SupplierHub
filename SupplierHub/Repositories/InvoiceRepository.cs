using Microsoft.EntityFrameworkCore;
using SupplierHub.Models;
using SupplierHub.Repositories.Interface;

namespace SupplierHub.Repositories
{
	public class InvoiceRepository : IInvoiceRepository
	{
		private readonly AppDbContext _context;

		public InvoiceRepository(AppDbContext context) => _context = context;

		public async Task<IEnumerable<Invoice>> GetAllAsync() =>
			await _context.Invoices.Where(x => !x.IsDeleted).ToListAsync();

		public async Task<Invoice?> GetByIdAsync(long id) =>
			await _context.Invoices.FirstOrDefaultAsync(x => x.InvoiceID == id && !x.IsDeleted);

		public async Task<IEnumerable<Invoice>> GetByPoIdAsync(long poId) =>
			await _context.Invoices.Where(x => x.PoID == poId && !x.IsDeleted).ToListAsync();

		public async Task AddAsync(Invoice invoice) => await _context.Invoices.AddAsync(invoice);

		public void Update(Invoice invoice) => _context.Invoices.Update(invoice);

		public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
	}
}