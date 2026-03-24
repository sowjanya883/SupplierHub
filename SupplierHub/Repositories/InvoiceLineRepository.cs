using Microsoft.EntityFrameworkCore;
using SupplierHub.Models;
using SupplierHub.Repositories.Interface;

namespace SupplierHub.Repositories
{
	public class InvoiceLineRepository : IInvoiceLineRepository
	{
		private readonly AppDbContext _context;

		public InvoiceLineRepository(AppDbContext context) => _context = context;

		public async Task<IEnumerable<InvoiceLine>> GetByInvoiceIdAsync(long invoiceId) =>
			await _context.InvoiceLines.Where(x => x.InvoiceID == invoiceId && !x.IsDeleted).ToListAsync();

		public async Task<InvoiceLine?> GetByIdAsync(long id) =>
			await _context.InvoiceLines.FirstOrDefaultAsync(x => x.InvLineID == id && !x.IsDeleted);

		public async Task AddAsync(InvoiceLine line) => await _context.InvoiceLines.AddAsync(line);
		public void Update(InvoiceLine line) => _context.InvoiceLines.Update(line);
		public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
	}
}