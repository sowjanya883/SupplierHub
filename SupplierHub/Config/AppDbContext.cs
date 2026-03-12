using Microsoft.EntityFrameworkCore;
using SupplierHub.Models;

namespace SupplierHub.Config
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

		public DbSet<ASN> ASNs => Set<ASN>();
		public DbSet<ASNItem> ASNItems => Set<ASNItem>();
		public DbSet<ApprovalRule> ApprovalRules => Set<ApprovalRule>();
		public DbSet<ApprovalStep> ApprovalSteps => Set<ApprovalStep>();
		public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
		public DbSet<Catalog> Catalogs => Set<Catalog>();
		public DbSet<CatalogItem> CatalogItems => Set<CatalogItem>();
		public DbSet<Category> Categories => Set<Category>();
		public DbSet<ComplianceDoc> ComplianceDocs => Set<ComplianceDoc>();
		public DbSet<Contract> Contracts => Set<Contract>();
		public DbSet<DeliverySlot> DeliverySlots => Set<DeliverySlot>();
		public DbSet<ErpExportRef> ErpExportRefs => Set<ErpExportRef>();
		public DbSet<GRNItemRef> GRNItemRefs => Set<GRNItemRef>();
		public DbSet<GRNRef> GRNRefs => Set<GRNRef>();
		public DbSet<Inspection> Inspections => Set<Inspection>();
		public DbSet<Invoice> Invoices => Set<Invoice>();
		public DbSet<InvoiceLine> InvoiceLines => Set<InvoiceLine>();
		public DbSet<Item> Items => Set<Item>();
		public DbSet<MatchRef> MatchRefs => Set<MatchRef>();
		public DbSet<NCR> NCRs => Set<NCR>();
		public DbSet<Notification> Notifications => Set<Notification>();
		public DbSet<Organization> Organizations => Set<Organization>();
		public DbSet<Permission> Permissions => Set<Permission>();
		public DbSet<PoAck> PoAcks => Set<PoAck>();
		public DbSet<POLine> POLines => Set<POLine>();
		public DbSet<PORevision> PORevisions => Set<PORevision>();
		public DbSet<PRLine> PRLines => Set<PRLine>();
		public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
		public DbSet<Requisition> Requisitions => Set<Requisition>();
		public DbSet<RFxEvent> RFxEvents => Set<RFxEvent>();
		public DbSet<Role> Roles => Set<Role>();
		public DbSet<Rolepermission> RolePermissions => Set<Rolepermission>();
		public DbSet<Scorecard> Scorecards => Set<Scorecard>();
		public DbSet<Shipment> Shipments => Set<Shipment>();
		public DbSet<Supplier> Suppliers => Set<Supplier>();
		public DbSet<SupplierContact> SupplierContacts => Set<SupplierContact>();
		public DbSet<SupplierKPI> SupplierKPIs => Set<SupplierKPI>();
		public DbSet<SupplierRisk> SupplierRisks => Set<SupplierRisk>();
		public DbSet<User> Users => Set<User>();
		public DbSet<Userrole> UserRoles => Set<Userrole>();

		public DbSet<RFxLine> RFxLines => Set<RFxLine>();

		public DbSet<RFxInvite> RFxInvites => Set<RFxInvite>();

		public DbSet<Bid> Bids => Set<Bid>();

		public DbSet<BidLine> BidLines => Set<BidLine>();

		public DbSet<Award> Awards => Set<Award>();

		public DbSet<SystemConfig> SystemConfigs => Set<SystemConfig>();


		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// Automatically applies all IEntityTypeConfiguration<T> classes in this assembly
			base.OnModelCreating(modelBuilder);
			modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
		}
	}
}