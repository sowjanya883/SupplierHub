using AutoMapper;
using SupplierHub.Models;

// DTOs
using SupplierHub.DTOs.SupplierDTO;
using SupplierHub.DTOs.OrganizationDTO;
using SupplierHub.DTOs.SupplierContactDTO;
using SupplierHub.DTOs.ComplianceDocDTO;
using SupplierHub.DTOs.SupplierRiskDTO;
using SupplierHub.DTOs.InspectionDTO;
using SupplierHub.DTOs.GRNRefDTO;

namespace SupplierHub.MapProfile
{
	// This is the Mapping profile for AutoMapper to map our DTOs
	public class ApplicationMapperProfile : Profile
	{
		public ApplicationMapperProfile()
		{
			// Supplier mappings
			CreateMap<Supplier, SupplierCreateDto>().ReverseMap();
			CreateMap<Supplier, SupplierUpdateDto>().ReverseMap();
			CreateMap<Supplier, SupplierResponseDto>().ReverseMap();
			CreateMap<Supplier, SupplierListDto>().ReverseMap();

			// Organization mappings
			CreateMap<Organization, OrganizationCreateDto>().ReverseMap();
			CreateMap<Organization, OrganizationUpdateDto>().ReverseMap();
			CreateMap<Organization, OrganizationResponseDto>().ReverseMap();

			// SupplierContact mappings
			CreateMap<SupplierContact, SupplierContactCreateDto>().ReverseMap();
			CreateMap<SupplierContact, SupplierContactUpdateDto>().ReverseMap();
			CreateMap<SupplierContact, SupplierContactResponseDto>().ReverseMap();

			// ComplianceDoc mappings
			CreateMap<ComplianceDoc, ComplianceDocCreateDto>().ReverseMap();
			CreateMap<ComplianceDoc, ComplianceDocUpdateDto>().ReverseMap();
			CreateMap<ComplianceDoc, ComplianceDocResponseDto>().ReverseMap();

			// SupplierRisk mappings
			CreateMap<SupplierRisk, SupplierRiskCreateDto>().ReverseMap();
			CreateMap<SupplierRisk, SupplierRiskUpdateDto>().ReverseMap();
			CreateMap<SupplierRisk, SupplierRiskResponseDto>().ReverseMap();

            //Inspection
            CreateMap<Inspection, InspectionCreateDto>().ReverseMap();
            CreateMap<Inspection, InspectionReadDto>().ReverseMap();
            CreateMap<Inspection, InspectionUpdateDto>().ReverseMap();

            //GRNRef
            CreateMap<GRNRef, GRNCreateDto>().ReverseMap();
            CreateMap<GRNRef, GRNReadDto>().ReverseMap();
            CreateMap<GRNRef, GRNStatusUpdateDto>().ReverseMap();
            CreateMap<GRNRef, GRNUpdateDto>().ReverseMap();
        }
	}
}