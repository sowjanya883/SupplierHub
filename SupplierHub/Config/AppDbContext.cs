using Microsoft.EntityFrameworkCore;

using SupplierHub.Config.Configurations;
using SupplierHub.Constants.Enum;
using SupplierHub.Models;

namespace SupplierHub
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

		
		// Identity & Access
		public DbSet<Role> Roles { get; set; }
		public DbSet<Permission> Permissions { get; set; }
		public DbSet<RolePermission> Rolepermissions { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<UserRole> Userroles { get; set; }
		public DbSet<AuditLog> AuditLogs { get; set; }

		


		// Suppliers / Organization / Compliance / Risk
		public DbSet<Organization> Organizations { get; set; }
		public DbSet<Supplier> Suppliers { get; set; }
		public DbSet<SupplierContact> SupplierContacts { get; set; }
		public DbSet<ComplianceDoc> ComplianceDocs { get; set; }
		public DbSet<SupplierRisk> SupplierRisks { get; set; }

		


		// Categories / Items / Catalogs / Contracts
		public DbSet<Category> Categories { get; set; }
		public DbSet<Item> Items { get; set; }
		public DbSet<Catalog> Catalogs { get; set; }
		public DbSet<CatalogItem> CatalogItems { get; set; }
		public DbSet<Contract> Contracts { get; set; }

		


		// RFx & Awards
		public DbSet<RfxEvent> RfxEvents { get; set; }
		public DbSet<RfxLine> RfxLines { get; set; }
		public DbSet<RfxInvite> RfxInvites { get; set; }
		public DbSet<Bid> Bids { get; set; }
		public DbSet<BidLine> BidLines { get; set; }
		public DbSet<Award> Awards { get; set; }

		


		// Requisitions & Approvals
		public DbSet<Requisition> Requisitions { get; set; }
		public DbSet<PrLine> PRLines { get; set; } // Note the name here
		public DbSet<ApprovalStep> ApprovalSteps { get; set; }

		

		// Purchase Orders, Acknowledgement, Revision, ERP Export
		public DbSet<PurchaseOrder> PurchaseOrders { get; set; }

		public DbSet<PoLine> PLines { get; set; }

		public DbSet<PoLine> PoLines { get; set; }

		public DbSet<PoAck> PoAcks { get; set; }
		public DbSet<PoRevision> PoRevisions { get; set; }
		public DbSet<ErpExportRef> ErpExportRefs { get; set; }

		

		// Shipments / ASN / Delivery Slot
		public DbSet<Shipment> Shipments { get; set; }
		public DbSet<Asn> Asns { get; set; }
		public DbSet<AsnItem> AsnItems { get; set; }
		public DbSet<DeliverySlot> DeliverySlots { get; set; }

		


		// Receiving (GRN) & Quality (Inspection / NCR)
		public DbSet<GrnRef> GrnRefs { get; set; }
		public DbSet<GrnItemRef> GrnItemRefs { get; set; }
		public DbSet<Inspection> Inspections { get; set; }
		public DbSet<Ncr> Ncrs { get; set; }

		
		// Invoices & Matching
		
		public DbSet<Invoice> Invoices { get; set; }
		public DbSet<InvoiceLine> InvoiceLines { get; set; }
		public DbSet<MatchRef> MatchRefs { get; set; }



		// Supplier Performance & Scorecards
		public DbSet<SupplierKpi> SupplierKpis { get; set; }
		public DbSet<Scorecard> Scorecards { get; set; }



		// Notifications
		public DbSet<Notification> Notifications { get; set; }



		// Admin
		public DbSet<SystemConfig> SystemConfigs { get; set; }
		public DbSet<ApprovalRule> ApprovalRules { get; set; }




		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// This line will automatically discover and apply all IEntityTypeConfiguration<T>
			// classes in your assembly (e.g., IdentityConfig, SupplierConfig, CatalogConfig, etc.).
			modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
		}
	}
}