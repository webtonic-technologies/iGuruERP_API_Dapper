using System.Data;
using System.Data.SqlClient;
using Transport_API.Repository.Implementations;
using Transport_API.Repository.Interfaces;
using Transport_API.Services.Implementations;
using Transport_API.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;

builder.Services.AddTransient<IDbConnection>(c => new SqlConnection(connectionString));
builder.Services.AddScoped<IVehiclesRepository, VehiclesRepository>();
builder.Services.AddScoped<IVehiclesService, VehiclesService>();

builder.Services.AddScoped<IRoutePlanRepository, RoutePlanRepository>();
builder.Services.AddScoped<IRoutePlanService, RoutePlanService>();

builder.Services.AddScoped<IRouteMappingRepository, RouteMappingRepository>();
builder.Services.AddScoped<IRouteMappingService, RouteMappingService>();

builder.Services.AddScoped<ITransportAttendanceRepository, TransportAttendanceRepository>();
builder.Services.AddScoped<ITransportAttendanceService, TransportAttendanceService>();

builder.Services.AddScoped<IVehicleMaintenanceRepository, VehicleMaintenanceRepository>();
builder.Services.AddScoped<IVehicleMaintenanceService, VehicleMaintenanceService>();

builder.Services.AddScoped<IReportsRepository, ReportsRepository>();
builder.Services.AddScoped<IReportsService, ReportsService>();

builder.Services.AddScoped<IRouteFeesRepository, RouteFeesRepository>();
builder.Services.AddScoped<IRouteFeesServices, RouteFeesServices>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
