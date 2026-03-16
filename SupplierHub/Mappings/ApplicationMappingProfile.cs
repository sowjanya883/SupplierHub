using AutoMapper;
using SupplierHub.Models;

// DTOs
using SupplierHub.DTOs.SupplierDTO;

namespace SupplierHub.MapProfile
{
	// This is the Mapping profile for AutoMapper to map our DTO's
	public class ApplicationMapperProfile : Profile
	{
		public ApplicationMapperProfile()
		{

			// Supplier
			CreateMap<Supplier, SupplierResponseDto>().ReverseMap();
			CreateMap<Supplier, CreateSupplierDto>().ReverseMap();
			CreateMap<Supplier, UpdateSupplierDto>().ReverseMap();
			CreateMap<Supplier, SupplierListDto>().ReverseMap();
		}
	}
}