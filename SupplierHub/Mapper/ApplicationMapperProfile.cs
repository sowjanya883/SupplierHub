using AutoMapper;
using SupplierHub.DTOs.ComplianceDocDTO;
using SupplierHub.DTOs.OrganizationDTO;
using SupplierHub.DTOs.RfxEventDTO;
using SupplierHub.DTOs.RFxLineDTO;
using SupplierHub.DTOs.BidDTO;
using SupplierHub.DTOs.BidLineDTO;
using SupplierHub.DTOs.AwardDTO;
using SupplierHub.DTOs.RfxInviteDTO;
using SupplierHub.DTOs.SupplierContactDTO;
// DTOs
using SupplierHub.DTOs.SupplierDTO;
using SupplierHub.DTOs.SupplierRiskDTO;
using SupplierHub.Models;

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

			CreateMap<RfxEvent, RFxEventCreateDto>().ReverseMap();
			CreateMap<RfxEvent, RFxEventReadDto>().ReverseMap();
			CreateMap<RfxEvent, RFxEventUpdateDto>().ReverseMap();

			CreateMap<RfxLine, RfxLineCreateDto>().ReverseMap();
			CreateMap<RfxLine, RfxLineReadDto>().ReverseMap();
			CreateMap<RfxLine, RfxLineUpdateDto>().ReverseMap();

			// Bid mappings
			CreateMap<Bid, BidCreateDto>().ReverseMap();
			CreateMap<Bid, BidReadDto>().ReverseMap();
			CreateMap<Bid, BidUpdateDto>().ReverseMap();

			// BidLine mappings
			CreateMap<BidLine, BidLineCreateDto>().ReverseMap();
			CreateMap<BidLine, BidLineReadDto>().ReverseMap();
			CreateMap<BidLine, BidLineUpdateDto>().ReverseMap();

			// Award mappings
			CreateMap<Award, AwardCreateDto>().ReverseMap();
			CreateMap<Award, AwardReadDto>().ReverseMap();
			CreateMap<Award, AwardUpdateDto>().ReverseMap();

			// RfxInvite mappings
			CreateMap<RfxInvite, RfxInviteCreateDto>().ReverseMap();
			CreateMap<RfxInvite, RfxInviteReadDto>().ReverseMap();
			CreateMap<RfxInvite, RfxInviteUpdateDto>().ReverseMap();
		}
	}
}