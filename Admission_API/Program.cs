using Admission_API.Repository.Implementations;
using Admission_API.Repository.Interfaces;
using Admission_API.Services.Implementations;
using Admission_API.Services.Interfaces;
using System.Data;
using System.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;

builder.Services.AddTransient<IDbConnection>(c => new SqlConnection(connectionString));

builder.Services.AddTransient<ILeadStageRepository, LeadStageRepository>();
builder.Services.AddTransient<ILeadStageService, LeadStageService>();

builder.Services.AddTransient<ILeadSourceRepository, LeadSourceRepository>();
builder.Services.AddTransient<ILeadSourceService, LeadSourceService>();

builder.Services.AddTransient<IEnquirySetupRepository, EnquirySetupRepository>();
builder.Services.AddTransient<IEnquirySetupService, EnquirySetupService>();

builder.Services.AddTransient<IRegistrationSetupRepository, RegistrationSetupRepository>();
builder.Services.AddTransient<IRegistrationSetupService, RegistrationSetupService>();

builder.Services.AddTransient<INumberSchemeRepository, NumberSchemeRepository>();
builder.Services.AddTransient<INumberSchemeService, NumberSchemeService>();

builder.Services.AddTransient<IEnquiryRepository, EnquiryRepository>();
builder.Services.AddTransient<IEnquiryService, EnquiryService>();

builder.Services.AddTransient<IRegistrationRepository, RegistrationRepository>();
builder.Services.AddTransient<IRegistrationService, RegistrationService>();

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
