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


using SupplierHub.DTOs.UserDTO;
using SupplierHub.DTOs.RoleDTO;
using SupplierHub.DTOs.PermissionDTO;
using SupplierHub.DTOs.RolePermissionDTO;
using SupplierHub.DTOs.UserRoleDTO;
using SupplierHub.DTOs.AuditLogDTO;
using SupplierHub.DTOs.NotificationDTO;

using SupplierHub.DTOs.InspectionDTO;
using SupplierHub.DTOs.GrnRefDTO;
using SupplierHub.Models;

//Neeraj DTOs
using SupplierHub.DTOs.PurchaseOrderDTO;
using SupplierHub.DTOs.PoLineDTO;
using SupplierHub.DTOs.PoAckDTO;
using SupplierHub.DTOs.PoRevisionDTO;
using SupplierHub.DTOs.ErpExportRefDTO;
using SupplierHub.DTOs.InvoiceDTO;
using SupplierHub.DTOs.InvoiceLineDTO;
using SupplierHub.DTOs.MatchRefDTO;

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

			// Role mappings
			CreateMap<Role, CreateRoleDto>().ReverseMap();
			CreateMap<Role, UpdateRoleDto>().ReverseMap();
			CreateMap<Role, RoleDto>().ReverseMap();
			CreateMap<Role, RoleListItemDto>().ReverseMap();

			// Permission mappings
			CreateMap<Models.Permission, CreatePermissionDto>().ReverseMap();
			CreateMap<Models.Permission, UpdatePermissionDto>().ReverseMap();
			CreateMap<Models.Permission, PermissionDto>().ReverseMap();
			CreateMap<Models.Permission, PermissionListItemDto>().ReverseMap();

			// RolePermission mappings
			CreateMap<Models.RolePermission, CreateRolePermissionDto>().ReverseMap();
			CreateMap<Models.RolePermission, UpdateRolePermissionDto>().ReverseMap();
			CreateMap<Models.RolePermission, RolePermissionDto>().ReverseMap();
			CreateMap<Models.RolePermission, RolePermissionListItemDto>().ReverseMap();

			// UserRole mappings
			CreateMap<Models.UserRole, CreateUserRoleDto>().ReverseMap();
			CreateMap<Models.UserRole, UpdateUserRoleDto>().ReverseMap();
			CreateMap<Models.UserRole, UserRoleDto>().ReverseMap();
			CreateMap<Models.UserRole, UserRoleListItemDto>().ReverseMap();

			// AuditLog mappings
			CreateMap<Models.AuditLog, CreateAuditLogDto>().ReverseMap();
			CreateMap<Models.AuditLog, UpdateAuditLogDto>().ReverseMap();
			CreateMap<Models.AuditLog, AuditLogDto>().ReverseMap();
			CreateMap<Models.AuditLog, AuditLogListItemDto>().ReverseMap();

			// User Mappings
			CreateMap<User, CreateUserDto>().ReverseMap();
			CreateMap<User, UpdateUserDto>().ReverseMap();
			CreateMap<User, UserDto>().ReverseMap();

			//Notification Mappings
			CreateMap<Notification, NotificationCreateDto>().ReverseMap();
			CreateMap<Notification, NotificationUpdateDto>().ReverseMap();
			CreateMap<Notification, NotificationDto>().ReverseMap();
			CreateMap<Notification, NotificationListItemDto>().ReverseMap();


			//Inspection
			CreateMap<Inspection, InspectionCreateDto>().ReverseMap();
            CreateMap<Inspection, InspectionReadDto>().ReverseMap();
            CreateMap<Inspection, InspectionUpdateDto>().ReverseMap();

            //GRNRef

            CreateMap<GrnRef, GrnCreateDto>().ReverseMap();
            CreateMap<GrnRef, GrnReadDto>().ReverseMap();
            CreateMap<GrnRef, GrnStatusUpdateDto>().ReverseMap();
            CreateMap<GrnRef, GrnUpdateDto>().ReverseMap();

        
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


			// PurchaseOrder mappings
			CreateMap<PurchaseOrder, PurchaseOrderCreateDto>().ReverseMap();
			CreateMap<PurchaseOrder, PurchaseOrderUpdateDto>().ReverseMap();
			CreateMap<PurchaseOrder, PurchaseOrderResponseDto>().ReverseMap();

			// PoLine mappings
			CreateMap<PoLine, PoLineCreateDto>().ReverseMap();
			CreateMap<PoLine, PoLineUpdateDto>().ReverseMap();
			CreateMap<PoLine, PoLineResponseDto>().ReverseMap();

			//PoAck mappings
			CreateMap<PoAck, PoAckCreateDto>().ReverseMap();
			CreateMap<PoAck, PoAckUpdateDto>().ReverseMap();
			CreateMap<PoAck, PoAckResponseDto>().ReverseMap();

			//PoRevsion mappings
			CreateMap<PoRevision, PoRevisionCreateDto>().ReverseMap();
			CreateMap<PoRevision, PoRevisionUpdateDto>().ReverseMap();
			CreateMap<PoRevision, PoRevisionResponseDto>().ReverseMap();

			//ErpExportRef mappings
			CreateMap<ErpExportRef, ErpExportRefCreateDto>().ReverseMap();
			CreateMap<ErpExportRef, ErpExportRefUpdateDto>().ReverseMap();
			CreateMap<ErpExportRef, ErpExportRefResponseDto>().ReverseMap();

			//Invoice mappings
			CreateMap<Invoice, InvoiceCreateDto>().ReverseMap();
			CreateMap<Invoice, InvoiceUpdateDto>().ReverseMap();
			CreateMap<Invoice, InvoiceResponseDto>().ReverseMap();

			//InvoiceLine mappings
			CreateMap<InvoiceLine, InvoiceLineCreateDto>().ReverseMap();
			CreateMap<InvoiceLine, InvoiceLineUpdateDto>().ReverseMap();
			CreateMap<InvoiceLine, InvoiceLineResponseDto>().ReverseMap();

			//MatchRef mappings
			CreateMap<MatchRef, MatchRefCreateDto>().ReverseMap();
			CreateMap<MatchRef, MatchRefUpdateDto>().ReverseMap();
			CreateMap<MatchRef, MatchRefResponseDto>().ReverseMap();

		}
	}
}