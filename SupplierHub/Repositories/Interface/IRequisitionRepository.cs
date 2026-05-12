using SupplierHub.Models;

namespace SupplierHub.Repositories.Interface
{
	public interface IRequisitionRepository
	{
		// Requisition
		Task<Requisition> AddRequisitionAsync(Requisition requisition);
		Task<Requisition?> GetRequisitionByIdAsync(long id);
		Task<List<Requisition>> GetAllRequisitionsAsync();
		Task<Requisition?> UpdateRequisitionAsync(Requisition requisition);

		// PR Lines
		Task<PrLine> AddPrLineAsync(PrLine line);
		Task<List<PrLine>> GetLinesByPrIdAsync(long prId);

		// Approval Steps
		Task<ApprovalStep> AddApprovalStepAsync(ApprovalStep step);
		Task<ApprovalStep?> GetApprovalStepByIdAsync(long id);
		Task<List<ApprovalStep>> GetApprovalHistoryByPrIdAsync(long prId);
		Task UpdateApprovalStepAsync(ApprovalStep step);
	}
}