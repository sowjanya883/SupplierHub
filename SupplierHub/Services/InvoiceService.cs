using AutoMapper;
using SupplierHub.Constants;
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
		private readonly INotificationService _notif;

		public InvoiceService(IInvoiceRepository repository, IMapper mapper, INotificationService notif)
		{
			_repository = repository;
			_mapper = mapper;
			_notif = notif;
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
			await _notif.SendToRoleAsync("AccountsPayable",$"New invoice INV-{invoice.InvoiceID} submitted by Supplier #{invoice.SupplierID}" +$"{(invoice.PoID.HasValue ? $" for PO-{invoice.PoID}" : "")}. Total: {invoice.TotalAmount} {invoice.Currency}.","Invoice",invoice.InvoiceID);
			await _notif.SendToRoleAsync("Admin",$"Invoice INV-{invoice.InvoiceID} submitted and awaiting processing.","Invoice",invoice.InvoiceID);
			return _mapper.Map<InvoiceResponseDto>(invoice);
		}

		public async Task<InvoiceResponseDto?> UpdateAsync(long id, InvoiceUpdateDto updateDto)
		{
			var existing = await _repository.GetByIdAsync(id);
			if (existing == null) return null;

			var oldStatus = existing.Status;
			_mapper.Map(updateDto, existing);
			existing.UpdatedOn = DateTime.UtcNow;

			_repository.Update(existing);
			await _repository.SaveChangesAsync();

			if (existing.Status != oldStatus)
			{
				var statusMsg = existing.Status switch
				{
					"Approved" => $"Your invoice INV-{id} has been approved and will be processed for payment.",
					"Paid" => $"Your invoice INV-{id} has been marked as Paid.",
					"Rejected" => $"Your invoice INV-{id} has been rejected. Please contact the AP team.",
					"On_Hold" => $"Your invoice INV-{id} has been placed on hold. Please review.",
					_ => $"Your invoice INV-{id} status has been updated to {existing.Status}.",
				};
				await _notif.SendToRoleAsync("SupplierUser", statusMsg, "Invoice", id);

				// Also notify AP if put on hold
				if (existing.Status == "On_Hold")
					await _notif.SendToRoleAsync(
						"AccountsPayable",
						$"Invoice INV-{id} is on hold and requires investigation.",
						"Invoice", id);
			}

			return _mapper.Map<InvoiceResponseDto>(existing);
		}
	}
}