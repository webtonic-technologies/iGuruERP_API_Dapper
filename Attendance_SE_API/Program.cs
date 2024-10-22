using Attendance_SE_API.Repository.Implementations;
using Attendance_SE_API.Repository.Interfaces;
using Attendance_SE_API.Services.Implementations;
using Attendance_SE_API.Services.Interfaces;
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
