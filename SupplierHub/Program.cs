using Microsoft.EntityFrameworkCore;
using SupplierHub;
using SupplierHub.MapProfile;
using AutoMapper;
using SupplierHub.Repositories;
using SupplierHub.Repositories.Interface;
using SupplierHub.Services;
using SupplierHub.Services.Interface;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("AppDb"));
    // Helpful in development to see parameter values in EF logs. Enable only for dev.
    if (builder.Environment.IsDevelopment())
        options.EnableSensitiveDataLogging();
});
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("AppDb")));


builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Make overload resolution unambiguous by passing a Type so the params Type[] overload is selected
builder.Services.AddAutoMapper(typeof(ApplicationMapperProfile).Assembly);


// register services before Build
// SUPPLIER
builder.Services.AddScoped<ISuppliersRepository, SuppliersRepository>();
builder.Services.AddScoped<ISupplierService, SupplierService>();


// SUPPLIER RISK
builder.Services.AddScoped<ISupplierRiskRepository, SupplierRiskRepository>();
builder.Services.AddScoped<ISupplierRiskService, SupplierRiskService>();


// SUPPLIER CONTACT
builder.Services.AddScoped<ISupplierContactRepository, SupplierContactRepository>();
builder.Services.AddScoped<ISupplierContactService, SupplierContactService>();

// ADMIN (SystemConfig & ApprovalRule)
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IAdminService, AdminService>();


// ORGANIZATION
builder.Services.AddScoped<IOrganizationRepository, OrganizationRepository>();
builder.Services.AddScoped<IOrganizationService, OrganizationService>();


// CATEGORY
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();


// ITEM
builder.Services.AddScoped<IItemRepository, ItemRepository>();
builder.Services.AddScoped<IItemService, ItemService>();


// CATALOG
builder.Services.AddScoped<ICatalogRepository, CatalogRepository>();
builder.Services.AddScoped<ICatalogService, CatalogService>();


// CATALOG ITEM
builder.Services.AddScoped<ICatalogItemRepository, CatalogItemRepository>();
builder.Services.AddScoped<ICatalogItemService, CatalogItemService>();


// CONTRACT
builder.Services.AddScoped<IContractRepository, ContractRepository>();
builder.Services.AddScoped<IContractService, ContractService>();


// COMPLIANCE DOCUMENT
builder.Services.AddScoped<IComplianceDocRepository, ComplianceDocRepository>();
builder.Services.AddScoped<IComplianceDocService, ComplianceDocService>();



builder.Services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
builder.Services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();

//ErpExportRef
builder.Services.AddScoped<IErpExportRefRepository, ErpExportRefRepository>();
builder.Services.AddScoped<IErpExportRefService, ErpExportRefService>();

// Module 1: Requisition (Procurement)
builder.Services.AddScoped<IRequisitionRepository, RequisitionRepository>();
builder.Services.AddScoped<IRequisitionService, RequisitionService>();

builder.Services.AddScoped<IRfxRepository, RfxRepository>();
builder.Services.AddScoped<IRfxService, RfxService>();

builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IAdminService, AdminService>();


// Module 2: Shipping (Logistics)
builder.Services.AddScoped<IShippingRepository, ShippingRepository>();
builder.Services.AddScoped<IShippingService, ShippingService>();


builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// User repository & service
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

// Role repository & service
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRoleService, RoleService>();

// Permission repository & service
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IPermissionService, PermissionService>();

// RolePermission repository & service
builder.Services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
builder.Services.AddScoped<IRolePermissionService, RolePermissionService>();

// UserRole repository & service
builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
builder.Services.AddScoped<IUserRoleService, UserRoleService>();

// AuditLog repository & service
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();

// ==================
// NEERAJ MODULES
// ===================

//PurchaseOrder Repository and Service
builder.Services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
builder.Services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();

//ErpExportRef Repository and Service
builder.Services.AddScoped<IErpExportRefRepository, ErpExportRefRepository>();
builder.Services.AddScoped<IErpExportRefService, ErpExportRefService>();

//PoLine Repository and Service
builder.Services.AddScoped<IPoLineRepository, PoLineRepository>();
builder.Services.AddScoped<IPoLineService, PoLineService>();

//PoAck Repository and Services
builder.Services.AddScoped<IPoAckRepository, PoAckRepository>();
builder.Services.AddScoped<IPoAckService, PoAckService>();

// PoRevision Repository and Services
builder.Services.AddScoped<IPoRevisionRepository, PoRevisionRepository>();
builder.Services.AddScoped<IPoRevisionService, PoRevisionService>();

// Invoice Repository and Services
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();

//InvoiceLine Repository and Services
builder.Services.AddScoped<IInvoiceLineRepository, InvoiceLineRepository>();
builder.Services.AddScoped<IInvoiceLineService, InvoiceLineService>();






var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();





