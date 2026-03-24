using Microsoft.EntityFrameworkCore;
using SupplierHub.Models;
using SupplierHub.Repositories.Interface;

namespace SupplierHub.Repositories
{
	public class PoLineRepository : IPoLineRepository
	{
		private readonly AppDbContext _context;

		public PoLineRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<PoLine>> GetAllByPoIdAsync(long poId)
		{
			return await _context.PoLines
				.Where(x => x.PoID == poId && !x.IsDeleted)
				.ToListAsync();
		}

		public async Task<PoLine?> GetByIdAsync(long id)
		{
			return await _context.PoLines
				.FirstOrDefaultAsync(x => x.PoLineID == id && !x.IsDeleted);
		}

		public async Task AddAsync(PoLine line) => await _context.PoLines.AddAsync(line);

		public void Update(PoLine line) => _context.PoLines.Update(line);

		public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
	}
}