using Microsoft.AspNetCore.Mvc;
using SupplierHub.DTOs.ComplianceDocDTO;
using SupplierHub.DTOs.OrganizationDTO;
using SupplierHub.DTOs.SupplierContactDTO;
using SupplierHub.DTOs.SupplierDTO;
using SupplierHub.DTOs.SupplierRiskDTO;
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

		// Get all suppliers
		[HttpGet]
		public async Task<IActionResult> GetAllSuppliers() =>
			Ok(await _service.GetAllSuppliersAsync());

		// Get supplier by id
		[HttpGet("{id:long}")]
		public async Task<IActionResult> GetSupplier(long id)
		{
			var result = await _service.GetSupplierByIdAsync(id);
			return result == null ? NotFound() : Ok(result);
		}

		// Create supplier
		[HttpPost]
		public async Task<IActionResult> CreateSupplier(SupplierCreateDto dto) =>
			Ok(await _service.CreateSupplierAsync(dto));

		// Update supplier
		[HttpPut]
		public async Task<IActionResult> UpdateSupplier(SupplierUpdateDto dto)
		{
			var result = await _service.UpdateSupplierAsync(dto);
			return result == null ? NotFound() : Ok(result);
		}

		// Update supplier contact
		[HttpPut("contacts")]
		public async Task<IActionResult> UpdateContact(SupplierContactUpdateDto dto)
		{
			var result = await _service.UpdateContactAsync(dto);
			return result == null ? NotFound() : Ok(result);
		}

		// Update compliance document
		[HttpPut("compliance-docs")]
		public async Task<IActionResult> UpdateComplianceDoc(ComplianceDocUpdateDto dto)
		{
			var result = await _service.UpdateComplianceDocAsync(dto);
			return result == null ? NotFound() : Ok(result);
		}

		// Update supplier risk
		[HttpPut("risks")]
		public async Task<IActionResult> UpdateRisk(SupplierRiskUpdateDto dto)
		{
			var result = await _service.UpdateRiskAsync(dto);
			return result == null ? NotFound() : Ok(result);
		}

		// Update organization
		[HttpPut("organizations")]
		public async Task<IActionResult> UpdateOrganization(OrganizationUpdateDto dto)
		{
			var result = await _service.UpdateOrganizationAsync(dto);
			return result == null ? NotFound() : Ok(result);
		}
	}
}
