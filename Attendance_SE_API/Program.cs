using Attendance_API.Repository.Implementations;
using Attendance_API.Repository.Interfaces;
using Attendance_API.Services.Implementations;
using Attendance_API.Services.Interfaces;
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
