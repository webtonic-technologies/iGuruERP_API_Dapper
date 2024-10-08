using EventGallery_API.Repository.Implementations;
using EventGallery_API.Repository.Interfaces;
using EventGallery_API.Services.Implementations;
using EventGallery_API.Services.Interfaces;
using OfficeOpenXml;
using System.Data;
using System.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Set up Dapper's IDbConnection
builder.Services.AddScoped<IDbConnection>(sp =>
    new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register repositories and services
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IEventService, EventService>();

builder.Services.AddScoped<IHolidayRepository, HolidayRepository>();
builder.Services.AddScoped<IHolidayService, HolidayService>();

builder.Services.AddScoped<IGalleryRepository, GalleryRepository>();
builder.Services.AddScoped<IGalleryService, GalleryService>();

builder.Services.AddScoped<IEventApprovalService, EventApprovalService>();
builder.Services.AddScoped<IEventApprovalRepository, EventApprovalRepository>();

builder.Services.AddScoped<IHolidayApprovalRepository, HolidayApprovalRepository>();
builder.Services.AddScoped<IHolidayApprovalService, HolidayApprovalService>();

// Set the EPPlus license context
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;


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
