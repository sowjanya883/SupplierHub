using System.ComponentModel.DataAnnotations;

namespace SupplierHub.DTOs.SupplierDTO
{

	// Lightweight list item for searching/dropdowns.
	// Inherits to reuse common fields, but only exposes what list needs.

	public sealed class SupplierListDto : CreateSupplierDto
	{
		public long SupplierID { get; init; }

	}
}