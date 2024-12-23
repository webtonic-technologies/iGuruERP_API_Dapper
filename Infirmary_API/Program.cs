using Infirmary_API.Repositories.Implementations;
using Infirmary_API.Repositories.Interfaces;
using Infirmary_API.Repository.Implementations;
using Infirmary_API.Repository.Interfaces;
using Infirmary_API.Services.Implementations;
using Infirmary_API.Services.Interfaces;
using InfirmaryVisit_API.Repositories.Implementations;
using InfirmaryVisit_API.Repositories.Interfaces;
using InfirmaryVisit_API.Services.Implementations;
using InfirmaryVisit_API.Services.Interfaces;
using OfficeOpenXml;
using System.Data;
using System.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;

// Set the license context for EPPlus to NonCommercial
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

builder.Services.AddTransient<IDbConnection>(c => new SqlConnection(connectionString));
builder.Services.AddTransient<IInfirmaryRepository, InfirmaryRepository>();
builder.Services.AddTransient<IInfirmaryService, InfirmaryService>();

builder.Services.AddTransient<IVaccinationRepository, VaccinationRepository>();
builder.Services.AddTransient<IVaccinationService, VaccinationService>();

builder.Services.AddTransient<IItemTypeRepository, ItemTypeRepository>();
builder.Services.AddTransient<IItemTypeService, ItemTypeService>();

builder.Services.AddTransient<IStockEntryRepository, StockEntryRepository>();
builder.Services.AddTransient<IStockEntryService, StockEntryService>();

builder.Services.AddTransient<IInfirmaryVisitRepository, InfirmaryVisitRepository>();
builder.Services.AddTransient<IInfirmaryVisitService, InfirmaryVisitService>();

builder.Services.AddTransient<IStudentVaccinationRepository, StudentVaccinationRepository>();
builder.Services.AddTransient<IStudentVaccinationService, StudentVaccinationService>();

builder.Services.AddTransient<IVaccinationFetchRepository, VaccinationFetchRepository>();
builder.Services.AddTransient<IVaccinationFetchService, VaccinationFetchService>();

builder.Services.AddTransient<IItemTypeFetchRepository, ItemTypeFetchRepository>();
builder.Services.AddTransient<IItemTypeFetchService, ItemTypeFetchService>();

builder.Services.AddTransient<IInfirmaryVisitorTypeRepository, InfirmaryVisitorTypeRepository>();
builder.Services.AddTransient<IInfirmaryVisitorTypeService, InfirmaryVisitorTypeService>();

builder.Services.AddScoped<IStudentVaccinationFetchService, StudentVaccinationFetchService>();
builder.Services.AddScoped<IStudentVaccinationFetchRepository, StudentVaccinationFetchRepository>();

builder.Services.AddScoped<IInfirmaryVisitFetchRepository, InfirmaryVisitFetchRepository>();
builder.Services.AddScoped<IInfirmaryVisitFetchService, InfirmaryVisitFetchService>();



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
