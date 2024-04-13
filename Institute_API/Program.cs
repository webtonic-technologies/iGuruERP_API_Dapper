using Institute_API.Repository.Implementations;
using Institute_API.Repository.Interfaces;
using Institute_API.Services.Implementations;
using Institute_API.Services.Interfaces;
using System.Data;
using System.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;

builder.Services.AddTransient<IDbConnection>(c => new SqlConnection(connectionString));
builder.Services.AddTransient<IInstituteDetailsRepository, InstituteDetailsRepository>();
builder.Services.AddTransient<IInstituteDetailsServices, InstituteDetailsServices>();
builder.Services.AddTransient<IInstituteAffiliationRepository, InstituteAffiliationRepository>();
builder.Services.AddTransient<IInstituteAffiliationServices, InstituteAffiliationServices>();
builder.Services.AddTransient<IInstituteHouseRepository, InstituteHouseRepository>();
builder.Services.AddTransient<IInstituteHouseServices, InstituteHouseServices>();
builder.Services.AddTransient<IAdminDepartmentRepository, AdminDepartmentRepository>();
builder.Services.AddTransient<IAdminDepartmentServices, AdminDepartmentServices>();
builder.Services.AddTransient<IAdminDesignationRepository, AdminDesignationRepository>();
builder.Services.AddTransient<IAdminDesignationServices, AdminDesignationServices>();
builder.Services.AddTransient<IAcademicConfigRepository, AcademicConfigRepository>();
builder.Services.AddTransient<IAcademicConfigServices, AcademicConfigServices>();
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
