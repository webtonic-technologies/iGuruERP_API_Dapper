using FeesManagement_API.Repository.Implementations;
using FeesManagement_API.Repository.Interfaces;
using FeesManagement_API.Services.Implementations;
using FeesManagement_API.Services.Interfaces;
using System.Data.SqlClient;
using System.Data;
using Configuration.Repository.Implementations;
using Configuration.Repository.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IDbConnection>(sp =>
    new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

// Dependency Injection for Repositories and Services
builder.Services.AddScoped<IFeeHeadRepository, FeeHeadRepository>();
builder.Services.AddScoped<IFeeHeadService, FeeHeadService>();

builder.Services.AddScoped<IFeeGroupService, FeeGroupService>();
builder.Services.AddScoped<IFeeGroupRepository, FeeGroupRepository>();

builder.Services.AddScoped<ILateFeeRepository, LateFeeRepository>();
builder.Services.AddScoped<ILateFeeService, LateFeeService>();

builder.Services.AddScoped<IConcessionService, ConcessionService>();
builder.Services.AddScoped<IConcessionRepository, ConcessionRepository>();

builder.Services.AddScoped<IOptionalFeeService, OptionalFeeService>();
builder.Services.AddScoped<IOptionalFeeRepository, OptionalFeeRepository>();

builder.Services.AddScoped<INumberSchemeService, NumberSchemeService>();
builder.Services.AddScoped<INumberSchemeRepository, NumberSchemeRepository>();

builder.Services.AddScoped<IOfferService, OfferService>();
builder.Services.AddScoped<IOfferRepository, OfferRepository>();

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
