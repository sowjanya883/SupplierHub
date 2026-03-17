using Microsoft.AspNetCore.Mvc;
using SupplierHub.DTOs.ComplianceDocDTO;
using SupplierHub.DTOs.OrganizationDTO;
using SupplierHub.DTOs.SupplierContactDTO;
using SupplierHub.DTOs.SupplierDTO;
using SupplierHub.DTOs.SupplierRiskDTO;
using SupplierHub.Models;
using SupplierHub.Services.Interface;

namespace SupplierHub.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class SuppliersController : ControllerBase
	{
		private readonly ISuppliersService _service;

		public SuppliersController(ISuppliersService service)
		{
			_service = service;
		}

		// Supplier
		[HttpGet("suppliers")]
		public async Task<IActionResult> GetAllSuppliers() =>
			Ok(await _service.GetAllSuppliersAsync());

		[HttpGet("suppliers/{id:long}")]
		public async Task<IActionResult> GetSupplier(long id)
		{
			var result = await _service.GetSupplierByIdAsync(id);
			return result == null ? NotFound() : Ok(result);
		}

		[HttpPost("suppliers")]
		public async Task<IActionResult> CreateSupplier(SupplierCreateDto dto) =>
			Ok(await _service.CreateSupplierAsync(dto));

		[HttpPut("suppliers")]
		public async Task<IActionResult> UpdateSupplier(SupplierUpdateDto dto)
		{
			var result = await _service.UpdateSupplierAsync(dto);
			return result == null ? NotFound() : Ok(result);
		}

		// SupplierContact
		[HttpPut("contacts")]
		public async Task<IActionResult> UpdateContact(SupplierContactUpdateDto dto)
		{
			var result = await _service.UpdateContactAsync(dto);
			return result == null ? NotFound() : Ok(result);
		}

		// ComplianceDoc
		[HttpPut("compliance-docs")]
		public async Task<IActionResult> UpdateComplianceDoc(ComplianceDocUpdateDto dto)
		{
			var result = await _service.UpdateComplianceDocAsync(dto);
			return result == null ? NotFound() : Ok(result);
		}

		// SupplierRisk
		[HttpPut("risks")]
		public async Task<IActionResult> UpdateRisk(SupplierRiskUpdateDto dto)
		{
			var result = await _service.UpdateRiskAsync(dto);
			return result == null ? NotFound() : Ok(result);
		}

		// Organization
		[HttpPut("organizations")]
		public async Task<IActionResult> UpdateOrganization(OrganizationUpdateDto dto)
		{
			var result = await _service.UpdateOrganizationAsync(dto);
			return result == null ? NotFound() : Ok(result);
		}
	}
}