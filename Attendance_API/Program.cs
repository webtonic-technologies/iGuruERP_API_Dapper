using System.Data.SqlClient;
using System.Data;
using Attendance_API.Services.Implementations;
using Attendance_API.Services.Interfaces;
using Attendance_API.Repository.Implementations;
using Attendance_API.Repository.Interfaces;

var builder = WebApplication.CreateBuilder(args);


string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;

builder.Services.AddTransient<IDbConnection>(c => new SqlConnection(connectionString));
// Add services to the container.

builder.Services.AddScoped<IStudentAttendanceStatusService, StudentAttendanceStatusService>();
builder.Services.AddScoped<IStudentAttendanceStatusRepository, StudentAttendanceStatusRepository>();
builder.Services.AddScoped<IStudentAttendanceMasterService, StudentAttendanceMasterService>();
builder.Services.AddScoped<IStudentAttendanceMasterRepository, StudentAttendanceMasterRepository>();
builder.Services.AddScoped<IEmployeeAttendanceStatusMasterService, EmployeeAttendanceStatusMasterService>();
builder.Services.AddScoped<IEmployeeAttendanceStatusMasterRepository, EmployeeAttendanceStatusMasterRepository>();
builder.Services.AddScoped<IEmployeeAttendanceService, EmployeeAttendanceService>();
builder.Services.AddScoped<IEmployeeAttendanceRepository, EmployeeAttendanceRepository>();
builder.Services.AddScoped<IGeoFencingService, GeoFencingService>();
builder.Services.AddScoped<IGeoFencingRepository, GeoFencingRepository>();
builder.Services.AddScoped<IShiftTimingRepository, ShiftTimingRepository>();
builder.Services.AddScoped<IShiftTimingService, ShiftTimingService>();
builder.Services.AddScoped<IStudentAttendanceReportService, StudentAttendanceReportService>();
builder.Services.AddScoped<IStudentAttendanceReportRepository, StudentAttendanceReportRepository>();
builder.Services.AddScoped<IClassAttendanceAnalysisRepo, ClassAttendanceAnalysisRepo>();
builder.Services.AddScoped<ISubjectAttendanceAnalysisRepo, SubjectAttendanceAnalysisRepo>();


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

app.UseExceptionMiddleware();

app.MapControllers();

app.Run();
