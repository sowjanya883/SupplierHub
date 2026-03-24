using Microsoft.EntityFrameworkCore;
using SupplierHub.Models;
using SupplierHub.Repositories.Interface;

namespace SupplierHub.Repositories
{
	public class PoRevisionRepository : IPoRevisionRepository
	{
		private readonly AppDbContext _context;

		public PoRevisionRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<PoRevision>> GetAllByPoIdAsync(long poId)
		{
			return await _context.PoRevisions
				.Where(x => x.PoID == poId)
				.OrderByDescending(x => x.RevisionNo)
				.ToListAsync();
		}

		public async Task<PoRevision?> GetByIdAsync(long id)
		{
			return await _context.PoRevisions
				.FirstOrDefaultAsync(x => x.PorevID == id);
		}

		public async Task<int> GetMaxRevisionNoAsync(long poId)
		{
			var exists = await _context.PoRevisions.AnyAsync(x => x.PoID == poId);
			if (!exists) return 0;

			return await _context.PoRevisions
				.Where(x => x.PoID == poId)
				.MaxAsync(x => x.RevisionNo);
		}

		public async Task AddAsync(PoRevision revision) => await _context.PoRevisions.AddAsync(revision);

		public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
	}
}