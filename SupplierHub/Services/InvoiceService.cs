using AutoMapper;
using SupplierHub.Constants.Enum;
using SupplierHub.DTOs.InvoiceDTO;
using SupplierHub.Models;
using SupplierHub.Repositories.Interface;
using SupplierHub.Services.Interface;

namespace SupplierHub.Services
{
	public class InvoiceService : IInvoiceService
	{
		private readonly IInvoiceRepository _repository;
		private readonly IMapper _mapper;

		public InvoiceService(IInvoiceRepository repository, IMapper mapper)
		{
			_repository = repository;
			_mapper = mapper;
		}

		public async Task<IEnumerable<InvoiceResponseDto>> GetAllAsync() =>
			_mapper.Map<IEnumerable<InvoiceResponseDto>>(await _repository.GetAllAsync());

		public async Task<InvoiceResponseDto?> GetByIdAsync(long id)
		{
			var invoice = await _repository.GetByIdAsync(id);
			return invoice == null ? null : _mapper.Map<InvoiceResponseDto>(invoice);
		}

		public async Task<IEnumerable<InvoiceResponseDto>> GetByPoIdAsync(long poId) =>
			_mapper.Map<IEnumerable<InvoiceResponseDto>>(await _repository.GetByPoIdAsync(poId));

		public async Task<InvoiceResponseDto> CreateAsync(InvoiceCreateDto createDto)
		{
			var invoice = _mapper.Map<Invoice>(createDto);

			invoice.CreatedOn = DateTime.UtcNow;
			invoice.UpdatedOn = DateTime.UtcNow;
			invoice.IsDeleted = false;

			// Business Rule: New invoices always start as 'Submitted'
			invoice.Status = nameof(InvoiceStatus.Submitted);

			await _repository.AddAsync(invoice);
			await _repository.SaveChangesAsync();

			return _mapper.Map<InvoiceResponseDto>(invoice);
		}

		public async Task<InvoiceResponseDto?> UpdateAsync(long id, InvoiceUpdateDto updateDto)
		{
			var existing = await _repository.GetByIdAsync(id);
			if (existing == null) return null;

			_mapper.Map(updateDto, existing);
			existing.UpdatedOn = DateTime.UtcNow;

			_repository.Update(existing);
			await _repository.SaveChangesAsync();

			return _mapper.Map<InvoiceResponseDto>(existing);
		}
	}
}