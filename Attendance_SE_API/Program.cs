using Attendance_SE_API.Repository.Implementations;
using Attendance_SE_API.Repository.Interfaces;
using Attendance_SE_API.Services.Implementations;
using Attendance_SE_API.Services.Interfaces;
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

// Set the license context for EPPlus to NonCommercial
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

// Register repositories and services
builder.Services.AddScoped<IStudentAttendanceStatusRepository, StudentAttendanceStatusRepository>();
builder.Services.AddScoped<IStudentAttendanceStatusService, StudentAttendanceStatusService>();

builder.Services.AddScoped<IStudentAttendanceRepository, StudentAttendanceRepository>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();

builder.Services.AddScoped<IStudentAttendanceReportRepository, StudentAttendanceReportRepository>();
builder.Services.AddScoped<IStudentAttendanceReportService, StudentAttendanceReportService>();

builder.Services.AddScoped<IEmployeeAttendanceStatusRepository, EmployeeAttendanceStatusRepository>();
builder.Services.AddScoped<IEmployeeAttendanceStatusService, EmployeeAttendanceStatusService>();

builder.Services.AddScoped<IEmployeeAttendanceRepository, EmployeeAttendanceRepository>();
builder.Services.AddScoped<IEmployeeAttendanceService, EmployeeAttendanceService>();

builder.Services.AddScoped<IEmployeeAttendanceReportRepository, EmployeeAttendanceReportRepository>();
builder.Services.AddScoped<IEmployeeAttendanceReportService, EmployeeAttendanceReportService>();

builder.Services.AddScoped<IEmployeeShiftRepository, EmployeeShiftRepository>();
builder.Services.AddScoped<IEmployeeShiftService, EmployeeShiftService>();

builder.Services.AddScoped<IGeoFencingRepository, GeoFencingRepository>();
builder.Services.AddScoped<IGeoFencingService, GeoFencingService>();

builder.Services.AddScoped<IMarkAttendanceService, MarkAttendanceService>();
builder.Services.AddScoped<IMarkAttendanceRepository, MarkAttendanceRepository>();

builder.Services.AddScoped<IClassAttendanceAnalysisRepository, ClassAttendanceAnalysisRepository>();
builder.Services.AddScoped<IClassAttendanceAnalysisService, ClassAttendanceAnalysisService>();

builder.Services.AddScoped<ISubjectAttendanceAnalysisRepository, SubjectAttendanceAnalysisRepository>();
builder.Services.AddScoped<ISubjectAttendanceAnalysisService, SubjectAttendanceAnalysisService>();

builder.Services.AddScoped<IGeoFencingEntryRepository, GeoFencingEntryRepository>();
builder.Services.AddScoped<IGeoFencingEntryService, GeoFencingEntryService>();

builder.Services.AddScoped<IAttendanceDashboardRepository, AttendanceDashboardRepository>();
builder.Services.AddScoped<IAttendanceDashboardService, AttendanceDashboardService>();

var app = builder.Build();


// Global error handling
app.Use(async (context, next) =>
{
    try
    {
        await next.Invoke();
    }
    catch (Exception ex)
    {
        // Log the exception
        Console.WriteLine($"Error: {ex.Message}");
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync("An unexpected error occurred.");
    }
});

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
