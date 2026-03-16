using Microsoft.EntityFrameworkCore;
using SupplierHub;
using SupplierHub.MapProfile;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("AppDb")));

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Make overload resolution unambiguous by passing a Type so the params Type[] overload is selected
builder.Services.AddAutoMapper(typeof(ApplicationMapperProfile).Assembly);


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





