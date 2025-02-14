using OfficeOpenXml;
using StudentManagement_API.Repository.Implementations;
using StudentManagement_API.Repository.Interfaces;
using StudentManagement_API.Services.Implementations;
using StudentManagement_API.Services.Interfaces;


// Set the license context for EPPlus to NonCommercial
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IStudentInformationService, StudentInformationService>();
builder.Services.AddScoped<IStudentInformationRepository, StudentInformationRepository>();


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
