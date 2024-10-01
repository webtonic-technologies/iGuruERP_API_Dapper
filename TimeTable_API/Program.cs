using TimeTable_API.Repository.Implementations;
using TimeTable_API.Repository.Interfaces;
using TimeTable_API.Services.Implementations;
using TimeTable_API.Services.Interfaces;
using System.Data;
using System.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IDbConnection>(sp =>
    new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

// Dependency Injection for Repositories and Services
builder.Services.AddScoped<IGroupRepository, GroupRepository>();
builder.Services.AddScoped<IGroupService, GroupService>();

builder.Services.AddScoped<IDaySetupRepository, DaySetupRepository>();
builder.Services.AddScoped<IDaySetupService, DaySetupService>();

builder.Services.AddScoped<ITimeTableRepository, TimeTableRepository>();
builder.Services.AddScoped<ITimeTableService, TimeTableService>();

builder.Services.AddScoped<IClassWiseRepository, ClassWiseRepository>();
builder.Services.AddScoped<IClassWiseService, ClassWiseService>();

builder.Services.AddScoped<IEmployeeWiseRepository, EmployeeWiseRepository>();
builder.Services.AddScoped<IEmployeeWiseService, EmployeeWiseService>();

builder.Services.AddScoped<IEmployeeWorkloadService, EmployeeWorkloadService>();
builder.Services.AddScoped<IEmployeeWorkloadRepository, EmployeeWorkloadRepository>(); 


var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
