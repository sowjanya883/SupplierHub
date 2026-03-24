using AutoMapper;
using SupplierHub.DTOs.PoRevisionDTO;
using SupplierHub.Models;
using SupplierHub.Repositories.Interface;
using SupplierHub.Services.Interface;

namespace SupplierHub.Services
{
	public class PoRevisionService : IPoRevisionService
	{
		private readonly IPoRevisionRepository _repository;
		private readonly IMapper _mapper;

		public PoRevisionService(IPoRevisionRepository repository, IMapper mapper)
		{
			_repository = repository;
			_mapper = mapper;
		}

		public async Task<IEnumerable<PoRevisionResponseDto>> GetAllByPoIdAsync(long poId)
		{
			var revisions = await _repository.GetAllByPoIdAsync(poId);
			return _mapper.Map<IEnumerable<PoRevisionResponseDto>>(revisions);
		}

		public async Task<PoRevisionResponseDto?> GetByIdAsync(long id)
		{
			var revision = await _repository.GetByIdAsync(id);
			return revision == null ? null : _mapper.Map<PoRevisionResponseDto>(revision);
		}

		public async Task<PoRevisionResponseDto> CreateAsync(PoRevisionCreateDto createDto)
		{
			var revision = _mapper.Map<PoRevision>(createDto);

			// BUSINESS LOGIC: Auto-increment the Revision Number
			var currentMaxRevision = await _repository.GetMaxRevisionNoAsync(revision.PoID);
			revision.RevisionNo = currentMaxRevision + 1;

			// Set the timestamp
			revision.ChangeDate = DateTime.UtcNow;

			await _repository.AddAsync(revision);
			await _repository.SaveChangesAsync();

			return _mapper.Map<PoRevisionResponseDto>(revision);
		}
	}
}