using AutoMapper;
using SupplierHub.Constants.Enum;
using SupplierHub.DTOs.InvoiceLineDTO;
using SupplierHub.Models;
using SupplierHub.Repositories.Interface;
using SupplierHub.Services.Interface;

namespace SupplierHub.Services
{
    public class InvoiceLineService : IInvoiceLineService
    {
        private readonly IInvoiceLineRepository _repository;
        private readonly IMapper _mapper;

        public InvoiceLineService(IInvoiceLineRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<InvoiceLineResponseDto>> GetByInvoiceIdAsync(long invoiceId) => 
            _mapper.Map<IEnumerable<InvoiceLineResponseDto>>(await _repository.GetByInvoiceIdAsync(invoiceId));

        public async Task<InvoiceLineResponseDto?> GetByIdAsync(long id)
        {
            var line = await _repository.GetByIdAsync(id);
            return line == null ? null : _mapper.Map<InvoiceLineResponseDto>(line);
        }

        public async Task<InvoiceLineResponseDto> CreateAsync(InvoiceLineCreateDto createDto)
        {
            var line = _mapper.Map<InvoiceLine>(createDto);
            
            line.CreatedOn = DateTime.UtcNow;
            line.UpdatedOn = DateTime.UtcNow;
            line.IsDeleted = false;
            
            // Business Logic: Calculate line total and set match status to pending
            line.LineTotal = (line.Qty ?? 0) * (line.UnitPrice ?? 0);
            line.MatchStatus = nameof(MatchResult.Pending);

            await _repository.AddAsync(line);
            await _repository.SaveChangesAsync();

            return _mapper.Map<InvoiceLineResponseDto>(line);
        }

        public async Task<InvoiceLineResponseDto?> UpdateAsync(long id, InvoiceLineUpdateDto updateDto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return null;

            _mapper.Map(updateDto, existing);
            existing.UpdatedOn = DateTime.UtcNow;
            existing.LineTotal = (existing.Qty ?? 0) * (existing.UnitPrice ?? 0);

            _repository.Update(existing);
            await _repository.SaveChangesAsync();

            return _mapper.Map<InvoiceLineResponseDto>(existing);
        }
    }
}