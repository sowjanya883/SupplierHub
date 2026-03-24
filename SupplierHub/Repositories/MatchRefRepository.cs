using Microsoft.EntityFrameworkCore;
using SupplierHub.Models;
using SupplierHub.Repositories.Interface;
using SupplierHub.Constants.Enum;

namespace SupplierHub.Repositories
{
	public class MatchRefRepository : IMatchRefRepository
	{
		private readonly AppDbContext _context;

		public MatchRefRepository(AppDbContext context) => _context = context;

		public async Task<IEnumerable<MatchRef>> GetByInvoiceIdAsync(long invoiceId)
		{
			return await _context.MatchRefs
				.Where(x => x.InvoiceID == invoiceId && x.Status != MatchRefStatus.Closed)
				.ToListAsync();
		}

		public async Task<MatchRef?> GetByIdAsync(long id) =>
			await _context.MatchRefs
				.FirstOrDefaultAsync(x => x.MatchID == id && x.Status != MatchRefStatus.Closed);

		public async Task AddAsync(MatchRef matchRef) => await _context.MatchRefs.AddAsync(matchRef);

		public void Update(MatchRef matchRef) => _context.MatchRefs.Update(matchRef);

		public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
	}
} 