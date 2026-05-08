using SupplierHub.DTOs.RequisitionDto;
using SupplierHub.DTOs.ApprovalDto;

namespace SupplierHub.Services.Interface
{
	public interface IRequisitionService
	{
		// Requisition
		Task<RequisitionReadDto> CreateRequisitionAsync(RequisitionCreateDto dto);
		Task<RequisitionReadDto?> GetRequisitionByIdAsync(long id);
		Task<List<RequisitionReadDto>> GetAllRequisitionsAsync();

		// PR Lines
		Task<PrLineReadDto> AddPrLineAsync(PrLineCreateDto dto);
		Task<List<PrLineReadDto>> GetLinesByPrIdAsync(long prId);

		// Approval Steps
		Task<ApprovalStepReadDto> CreateApprovalStepAsync(ApprovalStepCreateDto dto);
		Task<ApprovalStepReadDto?> UpdateApprovalDecisionAsync(long stepId, ApprovalStepUpdateDto dto);
		Task<List<ApprovalStepReadDto>> GetApprovalsByPrIdAsync(long prId);
	}
}