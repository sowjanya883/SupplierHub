using AutoMapper;
using SupplierHub.DTOs.ApprovalDto;
using SupplierHub.DTOs.AuditLogDTO;
using SupplierHub.DTOs.AwardDTO;
using SupplierHub.DTOs.BidDTO;
using SupplierHub.DTOs.BidLineDTO;
using SupplierHub.DTOs.CatalogDTO;
using SupplierHub.DTOs.CatalogItemDTO;
using SupplierHub.DTOs.CategoryDTO;
using SupplierHub.DTOs.ComplianceDocDTO;
using SupplierHub.DTOs.ContractDTO;
using SupplierHub.DTOs.ErpExportRefDTO;
using SupplierHub.DTOs.GrnItemRefDTO;
using SupplierHub.DTOs.GrnRefDTO;
using SupplierHub.DTOs.InspectionDTO;
using SupplierHub.DTOs.InvoiceDTO;
using SupplierHub.DTOs.InvoiceLineDTO;
using SupplierHub.DTOs.ItemDTO;
using SupplierHub.DTOs.MatchRefDTO;
using SupplierHub.DTOs.NcrDTO;
using SupplierHub.DTOs.NotificationDTO;
using SupplierHub.DTOs.OrganizationDTO;
using SupplierHub.DTOs.PermissionDTO;
using SupplierHub.DTOs.PoAckDTO;
using SupplierHub.DTOs.PoLineDTO;
using SupplierHub.DTOs.PoRevisionDTO;
//Neeraj DTOs
using SupplierHub.DTOs.PurchaseOrderDTO;
using SupplierHub.DTOs.RequisitionDto;
using SupplierHub.DTOs.RfxEventDTO;
using SupplierHub.DTOs.RfxInviteDTO;
using SupplierHub.DTOs.RFxLineDTO;
using SupplierHub.DTOs.RoleDTO;
using SupplierHub.DTOs.RolePermissionDTO;
using SupplierHub.DTOs.ScorecardDTO;
using SupplierHub.DTOs.ShippingDto;
using SupplierHub.DTOs.SupplierContactDTO;
// DTOs
using SupplierHub.DTOs.SupplierDTO;
using SupplierHub.DTOs.SupplierKpiDTO;
using SupplierHub.DTOs.SupplierRiskDTO;
using SupplierHub.DTOs.UserDTO;
using SupplierHub.DTOs.UserRoleDTO;
using SupplierHub.DTOs.ApprovalRuleDTO;
using SupplierHub.DTOs.SystemConfigDTO;
using SupplierHub.Models;


namespace SupplierHub.MapProfile
{
	// This is the Mapping profile for AutoMapper to map our DTOs
	public class ApplicationMapperProfile : Profile
	{
		public ApplicationMapperProfile()
		{
			// SUPPLIER
			CreateMap<Supplier, SupplierCreateDto>().ReverseMap();
			CreateMap<Supplier, UpdateSupplierDto>().ReverseMap();
			CreateMap<Supplier, GetSupplierByIdDto>().ReverseMap();
			CreateMap<Supplier, GetAllSupplierDto>().ReverseMap();
			CreateMap<Supplier, SupplierDeleteDto>().ReverseMap();


			// ORGANIZATION			
			CreateMap<Organization, OrganizationCreateDto>().ReverseMap();
			CreateMap<Organization, OrganizationUpdateDto>().ReverseMap();
			CreateMap<Organization, OrganizationGetByIdDto>().ReverseMap();
			CreateMap<Organization, OrganizationGetAllDto>().ReverseMap();
			CreateMap<Organization, OrganizationDeleteDto>().ReverseMap();


			// SUPPLIER CONTACT			
			CreateMap<SupplierContact, SupplierContactCreateDto>().ReverseMap();
			CreateMap<SupplierContact, SupplierContactUpdateDto>().ReverseMap();
			CreateMap<SupplierContact, SupplierContactGetByIdDto>().ReverseMap();
			CreateMap<SupplierContact, SupplierContactGetAllDto>().ReverseMap();
			CreateMap<SupplierContact, SupplierContactDeleteDto>().ReverseMap();


			// COMPLIANCE DOCUMENT			
			CreateMap<ComplianceDoc, ComplianceDocCreateDto>().ReverseMap();
			CreateMap<ComplianceDoc, ComplianceDocUpdateDto>().ReverseMap();
			CreateMap<ComplianceDoc, ComplianceDocGetByIdDto>().ReverseMap();
			CreateMap<ComplianceDoc, ComplianceDocGetAllDto>().ReverseMap();
			CreateMap<ComplianceDoc, ComplianceDocDeleteDto>().ReverseMap();


			// SUPPLIER RISK			
			CreateMap<SupplierRisk, SupplierRiskCreateDto>().ReverseMap();
			CreateMap<SupplierRisk, SupplierRiskUpdateDto>().ReverseMap();
			CreateMap<SupplierRisk, SupplierRiskGetByIdDto>().ReverseMap();
			CreateMap<SupplierRisk, SupplierRiskGetAllDto>().ReverseMap();
			CreateMap<SupplierRisk, SupplierRiskDeleteDto>().ReverseMap();


			// CATEGORY			
			CreateMap<Category, CategoryCreateDto>().ReverseMap();
			CreateMap<Category, CategoryUpdateDto>().ReverseMap();
			CreateMap<Category, CategoryGetByIdDto>().ReverseMap();
			CreateMap<Category, CategoryGetAllDto>().ReverseMap();
			CreateMap<Category, CategoryDeleteDto>().ReverseMap();


			// ITEM			
			CreateMap<Item, ItemCreateDto>().ReverseMap();
			CreateMap<Item, ItemUpdateDto>().ReverseMap();
			CreateMap<Item, ItemGetByIdDto>().ReverseMap();
			CreateMap<Item, ItemGetAllDto>().ReverseMap();
			CreateMap<Item, ItemDeleteDto>().ReverseMap();

			// CATALOG
			CreateMap<Catalog, CatalogCreateDto>().ReverseMap();
			CreateMap<Catalog, CatalogUpdateDto>().ReverseMap();
			CreateMap<Catalog, CatalogGetByIdDto>().ReverseMap();
			CreateMap<Catalog, CatalogGetAllDto>().ReverseMap();
			CreateMap<Catalog, CatalogDeleteDto>().ReverseMap();


			// CATALOG ITEM			
			CreateMap<CatalogItem, CatalogItemCreateDto>().ReverseMap();
			CreateMap<CatalogItem, CatalogItemUpdateDto>().ReverseMap();
			CreateMap<CatalogItem, CatalogItemGetByIdDto>().ReverseMap();
			CreateMap<CatalogItem, CatalogItemGetAllDto>().ReverseMap();
			CreateMap<CatalogItem, CatalogItemDeleteDto>().ReverseMap();


			// CONTRACT			
			CreateMap<Contract, ContractCreateDto>().ReverseMap();
			CreateMap<Contract, ContractUpdateDto>().ReverseMap();
			CreateMap<Contract, ContractGetByIdDto>().ReverseMap();
			CreateMap<Contract, ContractGetAllDto>().ReverseMap();
			CreateMap<Contract, ContractDeleteDto>().ReverseMap();

			// Role mappings
			CreateMap<Role, CreateRoleDto>().ReverseMap();
			CreateMap<Role, UpdateRoleDto>().ReverseMap();
			CreateMap<Role, RoleDto>().ReverseMap();
			CreateMap<Role, RoleListItemDto>().ReverseMap();

			// Permission mappings
			CreateMap<Permission, CreatePermissionDto>().ReverseMap();
			CreateMap<Permission, UpdatePermissionDto>().ReverseMap();
			CreateMap<Permission, PermissionDto>().ReverseMap();
			CreateMap<Permission, PermissionListItemDto>().ReverseMap();

			// RolePermission mappings
			CreateMap<RolePermission, CreateRolePermissionDto>().ReverseMap();
			CreateMap<RolePermission, UpdateRolePermissionDto>().ReverseMap();
			CreateMap<RolePermission, RolePermissionDto>().ReverseMap();
			CreateMap<RolePermission, RolePermissionListItemDto>().ReverseMap();

			// SystemConfig mappings
			CreateMap<SystemConfig, SystemConfigCreateDto>().ReverseMap();
			CreateMap<SystemConfig, SystemConfigReadDto>().ReverseMap();
			CreateMap<SystemConfig, SystemConfigUpdateDto>().ReverseMap();

			// ApprovalRule mappings
			CreateMap<ApprovalRule, ApprovalRuleCreateDto>().ReverseMap();
			CreateMap<ApprovalRule, ApprovalRuleReadDto>().ReverseMap();
			CreateMap<ApprovalRule, ApprovalRuleUpdateDto>().ReverseMap();

			// UserRole mappings
			CreateMap<UserRole, CreateUserRoleDto>().ReverseMap();
			CreateMap<UserRole, UpdateUserRoleDto>().ReverseMap();
			CreateMap<UserRole, UserRoleDto>().ReverseMap();
			CreateMap<UserRole, UserRoleListItemDto>().ReverseMap();

			// AuditLog mappings
			CreateMap<AuditLog, CreateAuditLogDto>().ReverseMap();
			CreateMap<AuditLog, AuditLogDto>().ReverseMap();
			CreateMap<AuditLog, AuditLogListItemDto>().ReverseMap();

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

            //GrnRef

            CreateMap<GrnRef, GrnCreateDto>().ReverseMap();
            CreateMap<GrnRef, GrnReadDto>().ReverseMap();
            CreateMap<GrnRef, GrnStatusUpdateDto>().ReverseMap();
            CreateMap<GrnRef, GrnUpdateDto>().ReverseMap();

			//GrnItemRef

			CreateMap<GrnItemRef, GrnItemCreateDto>().ReverseMap();
			CreateMap<GrnItemRef, GrnItemReadDto>().ReverseMap();
			CreateMap<GrnItemRef, GrnItemUpdateDto>().ReverseMap();

			//Ncr

			CreateMap<Ncr, NcrCreateDto>().ReverseMap();
			CreateMap<Ncr, NcrReadDto>().ReverseMap();
			CreateMap<Ncr, NcrUpdateDto>().ReverseMap();

            //SupplierKpi

			CreateMap<SupplierKpi, SupplierKpiCreateDto>().ReverseMap();
			CreateMap<SupplierKpi, SupplierKpiReadDto>().ReverseMap();
			CreateMap<SupplierKpi, SupplierKpiUpdateDto>().ReverseMap();

            //Scorecard

			CreateMap<Scorecard, ScorecardCreateDto>().ReverseMap();
			CreateMap<Scorecard, ScorecardReadDto>().ReverseMap();
			CreateMap<Scorecard, ScorecardUpdateDto>().ReverseMap();

            // RfxEvent mappings

            CreateMap<RfxEvent, RFxEventCreateDto>().ReverseMap();
			CreateMap<RfxEvent, RFxEventReadDto>().ReverseMap();
			CreateMap<RfxEvent, RFxEventUpdateDto>().ReverseMap();

            // RfxLine mappings

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

			// SystemConfig mappings
			CreateMap<SystemConfig, SystemConfigCreateDto>().ReverseMap();
			CreateMap<SystemConfig, SystemConfigReadDto>().ReverseMap();
			CreateMap<SystemConfig, SystemConfigUpdateDto>().ReverseMap();

			// ApprovalRule mappings
			CreateMap<ApprovalRule, ApprovalRuleCreateDto>().ReverseMap();
			CreateMap<ApprovalRule, ApprovalRuleReadDto>().ReverseMap();
			CreateMap<ApprovalRule, ApprovalRuleUpdateDto>().ReverseMap();

			// UserRole mappings
			CreateMap<UserRole, AssignRoleDto>().ReverseMap();
			CreateMap<UserRole, DeleteRoleDto>().ReverseMap();
			CreateMap<UserRole, RoleResponseDto>().ReverseMap();


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
			//Requistion mappings
			CreateMap<Requisition, RequisitionCreateDto>().ReverseMap();
			CreateMap<Requisition, RequisitionReadDto>().ReverseMap();
			CreateMap<Requisition, RequisitionUpdateDto>().ReverseMap();
			//PRLine mappings
			CreateMap<PrLine, PrLineCreateDto>().ReverseMap();
			CreateMap<PrLine, PrLineReadDto>().ReverseMap();
			CreateMap<PrLine, PrLineUpdateDto>().ReverseMap();
			//ApprovalStep mappings
			CreateMap<ApprovalStep, ApprovalStepCreateDto>().ReverseMap();
			CreateMap<ApprovalStep, ApprovalStepReadDto>().ReverseMap();
			CreateMap<ApprovalStep, ApprovalStepUpdateDto>().ReverseMap();

			//Shipment mappings
			CreateMap<Shipment, ShipmentCreateDto>().ReverseMap();
			CreateMap<Shipment, ShipmentReadDto>().ReverseMap();
			CreateMap<Shipment, ShipmentUpdateDto>().ReverseMap();
			//ASN mappings
			CreateMap<Asn, AsnCreateDto>().ReverseMap();
			CreateMap<Asn, AsnReadDto>().ReverseMap();
			CreateMap<Asn, AsnUpdateDto>().ReverseMap();
			//ASNItem mappings
			CreateMap<AsnItem, AsnItemCreateDto>().ReverseMap();
			CreateMap<AsnItem, AsnItemReadDto>().ReverseMap();
			CreateMap<AsnItem, AsnItemUpdateDto>().ReverseMap();

			//DeliverySlot mappings
			CreateMap<DeliverySlot, DeliverySlotCreateDto>().ReverseMap();
			CreateMap<DeliverySlot, DeliverySlotReadDto>().ReverseMap();
			CreateMap<DeliverySlot, DeliverySlotUpdateDto>().ReverseMap();
		}
	}
}