using AutoMapper;
using SupplierHub.DTOs.ApprovalDto;
using SupplierHub.DTOs.RequisitionDto;
using SupplierHub.Models;
using SupplierHub.Services.Interface;
using SupplierHub.Repositories.Interface;

public class RequisitionService : IRequisitionService
{
	private readonly IRequisitionRepository _repo;
	private readonly IMapper _mapper;
	private readonly INotificationService _notif;

	public RequisitionService(
		IRequisitionRepository repo,
		IMapper mapper,
		INotificationService notif)
	{
		_repo = repo;
		_mapper = mapper;
		_notif = notif;
	}

	public async Task<RequisitionReadDto> CreateRequisitionAsync(RequisitionCreateDto dto)
	{
		var entity = _mapper.Map<Requisition>(dto);
		entity.Status = "DRAFT";
		entity.CreatedOn = DateTime.UtcNow;
		entity.UpdatedOn = DateTime.UtcNow;
		entity.IsDeleted = false;

		var saved = await _repo.AddRequisitionAsync(entity);

		// Notify Admin and CategoryManagers that a new PR was raised
		var msg = $"📋 New Purchase Requisition PR-{saved.PrID} raised " +
				  $"for Org #{saved.OrgID}" +
				  (string.IsNullOrEmpty(saved.CostCenter)
					  ? "."
					  : $" (Cost Center: {saved.CostCenter}).");
		await _notif.SendToRoleAsync("Admin", msg, "Requisition", saved.PrID);
		await _notif.SendToRoleAsync("CategoryManager", msg, "Requisition", saved.PrID);

		return _mapper.Map<RequisitionReadDto>(saved);
	}

	public async Task<PrLineReadDto> AddPrLineAsync(PrLineCreateDto dto)
	{
		var entity = _mapper.Map<PrLine>(dto);
		entity.CreatedOn = DateTime.UtcNow;
		entity.UpdatedOn = DateTime.UtcNow;
		entity.IsDeleted = false;

		var saved = await _repo.AddPrLineAsync(entity);
		return _mapper.Map<PrLineReadDto>(saved);
	}

	public async Task<ApprovalStepReadDto> CreateApprovalStepAsync(ApprovalStepCreateDto dto)
	{
		var entity = _mapper.Map<ApprovalStep>(dto);
		entity.CreatedOn = DateTime.UtcNow;
		entity.UpdatedOn = DateTime.UtcNow;
		entity.IsDeleted = false;

		var saved = await _repo.AddApprovalStepAsync(entity);

		// Notify the approver that they have a pending decision
		await _notif.SendAsync(
			saved.ApproverID,
			$"⏳ You have been assigned to approve PR-{saved.PrID}. " +
			$"Please review and record your decision.",
			"Requisition",
			saved.PrID);

		return _mapper.Map<ApprovalStepReadDto>(saved);
	}

	public async Task<RequisitionReadDto?> GetRequisitionByIdAsync(long id)
	{
		var entity = await _repo.GetRequisitionByIdAsync(id);
		return entity == null ? null : _mapper.Map<RequisitionReadDto>(entity);
	}

	public async Task<List<RequisitionReadDto>> GetAllRequisitionsAsync()
	{
		var entities = await _repo.GetAllRequisitionsAsync();
		return _mapper.Map<List<RequisitionReadDto>>(entities);
	}

	public async Task<List<PrLineReadDto>> GetLinesByPrIdAsync(long prId)
	{
		var lines = await _repo.GetLinesByPrIdAsync(prId);
		return _mapper.Map<List<PrLineReadDto>>(lines);
	}

	public async Task<List<ApprovalStepReadDto>> GetApprovalsByPrIdAsync(long prId)
	{
		var steps = await _repo.GetApprovalHistoryByPrIdAsync(prId);
		return _mapper.Map<List<ApprovalStepReadDto>>(steps);
	}

	// Lightweight status-only update — used by SupplierUser to Accept / Decline
	// a PR (separate from internal approval which sets Approved / Rejected).
	public async Task<RequisitionReadDto?> UpdateRequisitionStatusAsync(long prId, string newStatus)
	{
		var pr = await _repo.GetRequisitionByIdAsync(prId);
		if (pr == null) return null;

		pr.Status = newStatus;
		pr.UpdatedOn = DateTime.UtcNow;
		await _repo.UpdateRequisitionAsync(pr);

		// Notify the requester + Admin / CategoryManager that the supplier responded.
		var msg = $"Supplier has marked PR-{pr.PrID} as '{newStatus}'.";
		await _notif.SendAsync(pr.RequesterUserID, msg, "Requisition", pr.PrID);
		await _notif.SendToRoleAsync("Admin", msg, "Requisition", pr.PrID);
		await _notif.SendToRoleAsync("CategoryManager", msg, "Requisition", pr.PrID);
		await _notif.SendToRoleAsync("Buyer", msg, "Requisition", pr.PrID);

		return _mapper.Map<RequisitionReadDto>(pr);
	}

	public async Task<ApprovalStepReadDto?> UpdateApprovalDecisionAsync(
		long stepId, ApprovalStepUpdateDto dto)
	{
		var existing = await _repo.GetApprovalStepByIdAsync(stepId);
		if (existing == null) return null;

		_mapper.Map(dto, existing);
		existing.UpdatedOn = DateTime.UtcNow;
		existing.DecisionDate = DateTime.UtcNow;

		await _repo.UpdateApprovalStepAsync(existing);

		// Notify the requester of the decision
		var pr = await _repo.GetRequisitionByIdAsync(existing.PrID);
		if (pr != null)
		{
			var decision = existing.Decision?.ToLower() ?? "updated";
			var emoji = decision == "approved" ? "✅" : decision == "rejected" ? "❌" : "📋";
			var msg = $"{emoji} Your PR-{existing.PrID} has been {decision}" +
						   (string.IsNullOrEmpty(existing.Remarks)
							   ? "."
							   : $". Remarks: {existing.Remarks}");

			await _notif.SendAsync(pr.RequesterUserID, msg, "Requisition", existing.PrID);

			// Also notify Admin if rejected
			if (decision == "rejected")
				await _notif.SendToRoleAsync(
					"Admin",
					$"❌ PR-{existing.PrID} has been rejected by approver #{existing.ApproverID}.",
					"Requisition",
					existing.PrID);

			// ── Auto-flip the PR's overall status based on its approval chain ──
			// - Any single Rejected → PR is Rejected
			// - All decided steps are Approved (and there's at least one decision) → PR is Approved
			// - Otherwise leave as-is (still DRAFT / Pending)
			var allSteps = await _repo.GetApprovalHistoryByPrIdAsync(existing.PrID);
			var nonDeleted = allSteps.Where(s => !s.IsDeleted).ToList();
			var anyRejected   = nonDeleted.Any(s => string.Equals(s.Decision, "Rejected", StringComparison.OrdinalIgnoreCase));
			var anyPending    = nonDeleted.Any(s => string.IsNullOrEmpty(s.Decision)
														|| string.Equals(s.Decision, "Pending", StringComparison.OrdinalIgnoreCase)
														|| string.Equals(s.Decision, "PENDING", StringComparison.OrdinalIgnoreCase));
			var hasAnyDecision = nonDeleted.Any(s => !string.IsNullOrEmpty(s.Decision)
														 && !string.Equals(s.Decision, "Pending", StringComparison.OrdinalIgnoreCase)
														 && !string.Equals(s.Decision, "PENDING", StringComparison.OrdinalIgnoreCase));

			string? newStatus = null;
			if (anyRejected) newStatus = "Rejected";
			else if (hasAnyDecision && !anyPending) newStatus = "Approved";

			if (newStatus != null && !string.Equals(pr.Status, newStatus, StringComparison.OrdinalIgnoreCase))
			{
				pr.Status = newStatus;
				pr.UpdatedOn = DateTime.UtcNow;
				await _repo.UpdateRequisitionAsync(pr);
			}
		}

		return _mapper.Map<ApprovalStepReadDto>(existing);
	}
}