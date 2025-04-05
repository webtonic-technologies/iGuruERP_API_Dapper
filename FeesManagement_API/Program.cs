using FeesManagement_API.Repository.Implementations;
using FeesManagement_API.Repository.Interfaces;
using FeesManagement_API.Services.Implementations;
using FeesManagement_API.Services.Interfaces;
using System.Data.SqlClient;
using System.Data;
using Configuration.Repository.Implementations;
using Configuration.Repository.Interfaces;
using OfficeOpenXml;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IDbConnection>(sp =>
    new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

// Set the license context for EPPlus to NonCommercial
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

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

builder.Services.AddScoped<IFeeStructureService, FeeStructureService>();
builder.Services.AddScoped<IFeeStructureRepository, FeeStructureRepository>();

builder.Services.AddScoped<IStudentFeeService, StudentFeeService>();
builder.Services.AddScoped<IStudentFeeRepository, StudentFeeRepository>();

builder.Services.AddScoped<IConcessionMappingService, ConcessionMappingService>();
builder.Services.AddScoped<IConcessionMappingRepository, ConcessionMappingRepository>();

builder.Services.AddScoped<INonAcademicFeeService, NonAcademicFeeService>();
builder.Services.AddScoped<INonAcademicFeeRepository, NonAcademicFeeRepository>();

builder.Services.AddScoped<IRefundRepository, RefundRepository>();
builder.Services.AddScoped<IRefundService, RefundService>();

builder.Services.AddScoped<IWalletRepository, WalletRepository>();
builder.Services.AddScoped<IWalletService, WalletService>();

builder.Services.AddScoped<IFeeCollectionService, FeeCollectionService>();
builder.Services.AddScoped<IFeeCollectionRepository, FeeCollectionRepository>();

builder.Services.AddScoped<IStudentInformationService, StudentInformationService>();
builder.Services.AddScoped<IStudentInformationRepository, StudentInformationRepository>();

builder.Services.AddScoped<IFeeWaiverService, FeeWaiverService>();
builder.Services.AddScoped<IFeeWaiverRepository, FeeWaiverRepository>();

builder.Services.AddScoped<IFeeDiscountRepository, FeeDiscountRepository>();
builder.Services.AddScoped<IFeeDiscountService, FeeDiscountService>();
 
builder.Services.AddScoped<IChequeTrackingRepository, ChequeTrackingRepository>();
builder.Services.AddScoped<IChequeTrackingService, ChequeTrackingService>();

builder.Services.AddScoped<IFeesReportsRepository, FeesReportsRepository>();
builder.Services.AddScoped<IFeesReportsService, FeesReportsService>();

builder.Services.AddScoped<IFeesDashboardRepository, FeesDashboardRepository>();
builder.Services.AddScoped<IFeesDashboardService, FeesDashboardService>();

builder.Services.AddScoped<IFeeReceiptRepository, FeeReceiptRepository>(); 
builder.Services.AddScoped<IFeeReceiptService, FeeReceiptService>();



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
