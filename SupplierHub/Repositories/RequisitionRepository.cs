using Microsoft.EntityFrameworkCore;
using SupplierHub.Models;
using SupplierHub.Repositories.Interface;

namespace SupplierHub.Repositories
{
	public class RequisitionRepository : IRequisitionRepository
	{
		private readonly AppDbContext _context;

		public RequisitionRepository(AppDbContext context)
		{
			_context = context;
		}

		// --- Requisition Methods ---
		public async Task<Requisition> AddRequisitionAsync(Requisition requisition)
		{
			_context.Requisitions.Add(requisition);
			await _context.SaveChangesAsync();
			return requisition;
		}

		public async Task<Requisition?> GetRequisitionByIdAsync(long id)
		{
			return await _context.Requisitions
				.FirstOrDefaultAsync(r => r.PrID == id && !r.IsDeleted);
		}

		public async Task<List<Requisition>> GetAllRequisitionsAsync()
		{
			return await _context.Requisitions
				.Where(r => !r.IsDeleted)
				.ToListAsync();
		}

		// --- PR Lines Methods ---
		// In RequisitionRepository.cs
		public async Task<PrLine> AddPrLineAsync(PrLine line)
		{
			_context.PRLines.Add(line); // Changed from PrLines to PRLines
			await _context.SaveChangesAsync();
			return line;
		}

		public async Task<List<PrLine>> GetLinesByPrIdAsync(long prId)
		{
			return await _context.PRLines // Changed from PrLines to PRLines
				.Where(l => l.PrID == prId && !l.IsDeleted)
				.ToListAsync();
		}

		// --- Approval Steps Methods ---
		public async Task<ApprovalStep> AddApprovalStepAsync(ApprovalStep step)
		{
			_context.ApprovalSteps.Add(step);
			await _context.SaveChangesAsync();
			return step;
		}

		public async Task<ApprovalStep?> GetApprovalStepByIdAsync(long id)
		{
			return await _context.ApprovalSteps
				.FirstOrDefaultAsync(a => a.StepID == id && !a.IsDeleted);
		}

		public async Task<List<ApprovalStep>> GetApprovalHistoryByPrIdAsync(long prId)
		{
			return await _context.ApprovalSteps
				.Where(a => a.PrID == prId && !a.IsDeleted)
				.OrderBy(a => a.CreatedOn)
				.ToListAsync();
		}

		public async Task UpdateApprovalStepAsync(ApprovalStep step)
		{
			_context.ApprovalSteps.Update(step);
			await _context.SaveChangesAsync();
		}
	}
}