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

	public RequisitionService(IRequisitionRepository repo, IMapper mapper)
	{
		_repo = repo;
		_mapper = mapper;
	}

	public async Task<RequisitionReadDto> CreateRequisitionAsync(RequisitionCreateDto dto)
	{
		var entity = _mapper.Map<Requisition>(dto);
		entity.Status = "DRAFT";
		entity.CreatedOn = DateTime.UtcNow;
		entity.UpdatedOn = DateTime.UtcNow; // Match your model's [Required] attribute
		entity.IsDeleted = false;
		var saved = await _repo.AddRequisitionAsync(entity);
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
		return _mapper.Map<ApprovalStepReadDto>(saved);
	}

	// --- ADD THESE TO FIX THE INTERFACE ERROR ---

	public async Task<RequisitionReadDto?> GetRequisitionByIdAsync(long id)
	{
		var entity = await _repo.GetRequisitionByIdAsync(id);
		return entity == null ? null : _mapper.Map<RequisitionReadDto>(entity);
	}

	public async Task<List<PrLineReadDto>> GetLinesByPrIdAsync(long prId)
	{
		var lines = await _repo.GetLinesByPrIdAsync(prId);
		return _mapper.Map<List<PrLineReadDto>>(lines);
	}

	public async Task<List<RequisitionReadDto>> GetAllRequisitionsAsync()
	{
		var entities = await _repo.GetAllRequisitionsAsync();
		return _mapper.Map<List<RequisitionReadDto>>(entities);
	}

	public async Task<List<ApprovalStepReadDto>> GetApprovalsByPrIdAsync(long prId)
	{
		var steps = await _repo.GetApprovalHistoryByPrIdAsync(prId);
		return _mapper.Map<List<ApprovalStepReadDto>>(steps);
	}

	public async Task<ApprovalStepReadDto?> UpdateApprovalDecisionAsync(long stepId, ApprovalStepUpdateDto dto)
	{
		var existing = await _repo.GetApprovalStepByIdAsync(stepId);
		if (existing == null) return null;

		_mapper.Map(dto, existing);
		existing.UpdatedOn = DateTime.UtcNow;
		existing.DecisionDate = DateTime.UtcNow;

		await _repo.UpdateApprovalStepAsync(existing);
		return _mapper.Map<ApprovalStepReadDto>(existing);
	}
}