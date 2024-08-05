using HostelManagement_API.Repository.Implementations;
using HostelManagement_API.Repository.Interfaces;
using HostelManagement_API.Services.Implementations;
using HostelManagement_API.Services.Interfaces;
using System.Data;
using System.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
builder.Services.AddTransient<IDbConnection>(c => new SqlConnection(connectionString));

builder.Services.AddTransient<IBlockRepository, BlockRepository>();
builder.Services.AddTransient<IBlockService, BlockService>();

builder.Services.AddTransient<IBuildingRepository, BuildingRepository>();
builder.Services.AddTransient<IBuildingService, BuildingService>();

builder.Services.AddTransient<IFloorRepository, FloorRepository>();
builder.Services.AddTransient<IFloorService, FloorService>();

builder.Services.AddTransient<IRoomTypeRepository, RoomTypeRepository>();
builder.Services.AddTransient<IRoomTypeService, RoomTypeService>();

builder.Services.AddTransient<IHostelRepository, HostelRepository>();
builder.Services.AddTransient<IHostelService, HostelService>();

builder.Services.AddTransient<IRoomRepository, RoomRepository>();
builder.Services.AddTransient<IRoomService, RoomService>();

builder.Services.AddTransient<IOutpassRepository, OutpassRepository>();
builder.Services.AddTransient<IOutpassService, OutpassService>();

builder.Services.AddTransient<IVisitorLogRepository, VisitorLogRepository>();
builder.Services.AddTransient<IVisitorLogService, VisitorLogService>();

builder.Services.AddTransient<ICautionDepositRepository, CautionDepositRepository>();
builder.Services.AddTransient<ICautionDepositService, CautionDepositService>();
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
