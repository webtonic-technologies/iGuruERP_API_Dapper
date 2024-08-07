using Employee_API.Repository.Implementations;
using Employee_API.Repository.Interfaces;
using Employee_API.Services.Implementations;
using Employee_API.Services.Interfaces;
using System.Data;
using System.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
builder.Services.AddScoped<IEmployeeProfileRepository, EmployeeProfileRepository>();
builder.Services.AddScoped<IEmployeeProfileServices, EmployeeProfileServices>();
builder.Services.AddScoped<IEmployeeDashboardRepository, EmployeeDashboardRepository>();
builder.Services.AddScoped<IEmployeeDashboardServices, EmployeeDashboardServices>();
builder.Services.AddTransient<IDbConnection>(c => new SqlConnection(connectionString));
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
