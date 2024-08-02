using ProductUpdates_API.Repository.Implementations;
using ProductUpdates_API.Repository.Interfaces;
using ProductUpdates_API.Services.Implementations;
using ProductUpdates_API.Services.Interfaces;
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

builder.Services.AddTransient<IProductUpdatePostRepository, ProductUpdatePostRepository>();
builder.Services.AddTransient<IProductUpdatePostService, ProductUpdatePostService>();

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
