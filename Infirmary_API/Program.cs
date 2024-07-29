using Infirmary_API.Repository.Implementations;
using Infirmary_API.Repository.Interfaces;
using Infirmary_API.Services.Implementations;
using Infirmary_API.Services.Interfaces;
using System.Data;
using System.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;

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
