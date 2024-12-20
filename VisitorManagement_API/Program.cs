using System.Data.SqlClient;
using System.Data;
using VisitorManagement_API.Repository.Implementations;
using VisitorManagement_API.Repository.Interfaces;
using VisitorManagement_API.Services.Implementations;
using VisitorManagement_API.Services.Interfaces;
using OfficeOpenXml;

var builder = WebApplication.CreateBuilder(args);
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;

// Set the license context for EPPlus to NonCommercial
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

builder.Services.AddTransient<IDbConnection>(c => new SqlConnection(connectionString));
builder.Services.AddTransient<ISourceRepository, SourceRepository>();
builder.Services.AddTransient<ISourceService, SourceService>();

builder.Services.AddTransient<IPurposeTypeRepository, PurposeTypeRepository>();
builder.Services.AddTransient<IPurposeTypeService, PurposeTypeService>();

builder.Services.AddTransient<IVisitorLogRepository, VisitorLogRepository>();
builder.Services.AddTransient<IVisitorLogService, VisitorLogService>();

builder.Services.AddTransient<IEmployeeGatePassRepository, EmployeeGatePassRepository>();
builder.Services.AddTransient<IEmployeeGatePassService, EmployeeGatePassService>();

builder.Services.AddTransient<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddTransient<IAppointmentService, AppointmentService>();

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
