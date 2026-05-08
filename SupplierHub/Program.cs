using AutoMapper;

using Microsoft.AspNetCore.Authentication.JwtBearer;

using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Mvc.Authorization;

using Microsoft.EntityFrameworkCore;

using Microsoft.IdentityModel.Tokens;

using SupplierHub;

using SupplierHub.MapProfile;

using SupplierHub.Middleware;

using SupplierHub.Models;

using SupplierHub.Repositories;

using SupplierHub.Repositories.Interface;

using SupplierHub.Services;

using SupplierHub.Services.Interface;

using System.Security.Claims;

using System.Text;

using System.Text.Json.Serialization;
 
var builder = WebApplication.CreateBuilder(args);
 
// Add services to the container.

builder.Services.AddDbContext<AppDbContext>(options =>

{

    options.UseSqlServer(builder.Configuration.GetConnectionString("AppDb"));

    // Helpful in development to see parameter values in EF logs. Enable only for dev.

    if (builder.Environment.IsDevelopment())

        options.EnableSensitiveDataLogging();

});
 
// FIX: Merge these and remove the second AddControllers() call later in the file

builder.Services.AddControllers(options =>

{

	var adminPolicy = new AuthorizationPolicyBuilder()

		.RequireAuthenticatedUser()

		.RequireRole("Admin")

		.Build();
 
	options.Filters.Add(new AuthorizeFilter(adminPolicy));

})

.AddJsonOptions(opts =>

{

	opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

});
 
 
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddOpenApi();
 
// Make overload resolution unambiguous by passing a Type so the params Type[] overload is selected

builder.Services.AddAutoMapper(typeof(ApplicationMapperProfile).Assembly);
 
 
// ==========================================

// DEPENDENCY INJECTION (SERVICES & REPOS)

// ==========================================
 
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
 
// Module 1: Requisition (Procurement)

builder.Services.AddScoped<IRequisitionRepository, RequisitionRepository>();

builder.Services.AddScoped<IRequisitionService, RequisitionService>();

builder.Services.AddScoped<IRfxRepository, RfxRepository>();

builder.Services.AddScoped<IRfxService, RfxService>();
 
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
 
// ============================

// PERFORMANCE & QUALITY (SPLIT)

// ============================

builder.Services.AddScoped<ISupplierKpiRepository, SupplierKpiRepository>();

builder.Services.AddScoped<ISupplierKpiService, SupplierKpiService>();
 
builder.Services.AddScoped<IScorecardRepository, ScorecardRepository>();

builder.Services.AddScoped<IScorecardService, ScorecardService>();
 
builder.Services.AddScoped<IGrnRepository, GrnRepository>();

builder.Services.AddScoped<IGrnService, GrnService>();
 
builder.Services.AddScoped<IGrnItemRepository, GrnItemRepository>();

builder.Services.AddScoped<IGrnItemService, GrnItemService>();
 
builder.Services.AddScoped<IInspectionRepository, InspectionRepository>();

builder.Services.AddScoped<IInspectionService, InspectionService>();
 
builder.Services.AddScoped<INcrRepository, NcrRepository>();

builder.Services.AddScoped<INcrService, NcrService>();
 
// ==================

// NEERAJ MODULES

// ==================
 
// PurchaseOrder Repository and Service

builder.Services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();

builder.Services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
 
// ErpExportRef Repository and Service

builder.Services.AddScoped<IErpExportRefRepository, ErpExportRefRepository>();

builder.Services.AddScoped<IErpExportRefService, ErpExportRefService>();
 
// PoLine Repository and Service

builder.Services.AddScoped<IPoLineRepository, PoLineRepository>();

builder.Services.AddScoped<IPoLineService, PoLineService>();
 
// PoAck Repository and Services

builder.Services.AddScoped<IPoAckRepository, PoAckRepository>();

builder.Services.AddScoped<IPoAckService, PoAckService>();
 
// PoRevision Repository and Services

builder.Services.AddScoped<IPoRevisionRepository, PoRevisionRepository>();

builder.Services.AddScoped<IPoRevisionService, PoRevisionService>();
 
// Invoice Repository and Services

builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();

builder.Services.AddScoped<IInvoiceService, InvoiceService>();
 
// InvoiceLine Repository and Services

builder.Services.AddScoped<IInvoiceLineRepository, InvoiceLineRepository>();

builder.Services.AddScoped<IInvoiceLineService, InvoiceLineService>();
 
// MatchRef Repository and Service

builder.Services.AddScoped<IMatchRefRepository, MatchRefRepository>();

builder.Services.AddScoped<IMatchRefService, MatchRefService>();
 
// ==================

// SECURITY & AUTH

// ==================
 
// PASSWORD HASHER (REQUIRED)

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
 
// Authentication

builder.Services.AddScoped<IAuthService, AuthService>();
 
// --------------------

// CONTROLLERS & API

// --------------------
 
 
//// cors

//builder.Services.AddCors(options =>

//{

//    options.AddPolicy("AllowFrontend", policy =>

//    {

//        policy

//            .WithOrigins(

//                "http://localhost:5173",

//                "http://localhost:4200",

//                "http://localhost:3000",

//                "https://your-frontend-domain.com"

//            )

//            .AllowAnyHeader()

//            .AllowAnyMethod()

//            .AllowCredentials(); // only if using cookies / auth headers

//    });

//});
 
// ── 1. Register CORS (add this with your other builder.Services calls) ──

builder.Services.AddCors(options =>

{

	options.AddPolicy("AllowFrontend", policy =>

	{

		policy.WithOrigins("http://localhost:5173")  // your Vite dev server

			  .AllowAnyHeader()

			  .AllowAnyMethod();

	});

});
 
builder.Services.AddAuthentication(options =>

{

    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

})

.AddJwtBearer(options =>

{

    var key = builder.Configuration["Jwt:Key"];

    var issuer = builder.Configuration["Jwt:Issuer"];

    var audience = builder.Configuration["Jwt:Audience"];
 
    options.TokenValidationParameters = new TokenValidationParameters

    {

        ValidateIssuerSigningKey = true,

        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!)),

        ValidateIssuer = true,

        ValidIssuer = issuer,

        ValidateAudience = true,

        ValidAudience = audience,

        ValidateLifetime = true,

        ClockSkew = TimeSpan.FromSeconds(30),
 
        // 👇 Important for [Authorize(Roles = "...")]

        RoleClaimType = ClaimTypes.Role,

        NameClaimType = ClaimTypes.NameIdentifier

    };

});
 
builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
 
var app = builder.Build();
 
if (app.Environment.IsDevelopment())

{

    app.MapOpenApi();

}
 
//app.UseCors("AllowFrontend"); // <-- Fixed: Tells the app to actually use the policy defined above!

//if (!app.Environment.IsDevelopment())

//{

//	app.UseHttpsRedirection();  // only redirect in production

//}

// ── 2. Use CORS (add this in the middleware pipeline, BEFORE UseAuthorization) ──

app.UseCors("AllowFrontend");
 
 
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();
 
using (var scope = app.Services.CreateScope())

{

	var services = scope.ServiceProvider;

	try

	{

		var context = services.GetRequiredService<AppDbContext>();

		var passwordHasher = services.GetRequiredService<IPasswordHasher<User>>();

		await AppDbContext.SeedAdminUser(context, passwordHasher);

	}

	catch (Exception ex)

	{

		var logger = services.GetRequiredService<ILogger<Program>>();

		logger.LogError(ex, "An error occurred while seeding the database.");

	}

}

app.Run();
 