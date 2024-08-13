using SiteAdmin_API.Repository.Implementations;
using SiteAdmin_API.Repository.Interfaces;
using SiteAdmin_API.Services.Implementations;
using SiteAdmin_API.Services.Interfaces;
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

builder.Services.AddTransient<IModuleRepository, ModuleRepository>();
builder.Services.AddTransient<IModuleService, ModuleService>();

builder.Services.AddTransient<ISubModuleRepository, SubModuleRepository>();
builder.Services.AddTransient<ISubModuleService, SubModuleService>();

builder.Services.AddTransient<ICreatePackageRepository, CreatePackageRepository>();
builder.Services.AddTransient<ICreatePackageService, CreatePackageService>();

builder.Services.AddTransient<IInstituteOnboardRepository, InstituteOnboardRepository>();
builder.Services.AddTransient<IInstituteOnboardService, InstituteOnboardService>();

builder.Services.AddTransient<IFunctionalityRepository, FunctionalityRepository>();
builder.Services.AddTransient<IFunctionalityService, FunctionalityService>();


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
