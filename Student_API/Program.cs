using Student_API.Repository.Implementations;
using Student_API.Repository.Interfaces;
using Student_API.Services.Implementations;
using Student_API.Services.Interfaces;
using System.Data;
using System.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;


builder.Services.AddTransient<IDbConnection>(c => new SqlConnection(connectionString));
builder.Services.AddTransient<IStudentInformationServices, StudentInformationServices>();
builder.Services.AddTransient<IStudentInformationRepository, StudentInformationRepository>();
builder.Services.AddTransient<IStudentInfoDropdownRepository, StudentInfoDropdownRepository>();
builder.Services.AddTransient<IStudentInfoDropdownService, StudentInfoDropdownService>();
builder.Services.AddTransient<ITimetableRepository, TimetableRepository>();
builder.Services.AddTransient<ITimetableServices, TimetableServices>();
builder.Services.AddTransient<IImageService, ImageService>();
builder.Services.AddTransient<IStudentQRService, StudentQRService>();
builder.Services.AddTransient<IStudentQRRepository, StudentQRRepository>();
builder.Services.AddTransient<IDocumentConfigService, DocumentConfigService>();
builder.Services.AddTransient<IDocumentConfigRepository, DocumentConfigRepository>();
builder.Services.AddTransient<IDocumentManagerRepository, DocumentManagerRepository>();
builder.Services.AddTransient<IDocumentManagerService, DocumentManagerService>();
builder.Services.AddTransient<IPermissionSlipService, PermissionSlipService>();
builder.Services.AddTransient<IPermissionSlipRepository, PermissionSlipRepository>();
builder.Services.AddTransient<IStudentDashboardRepository, StudentDashboardRepository>();
builder.Services.AddTransient<IStudentDashboardService, StudentDashboardService>();
builder.Services.AddTransient<ITemplateService, TemplateService>();
builder.Services.AddTransient<ITemplateRepository, TemplateRepository>();
builder.Services.AddTransient<IStudentPromotionService, StudentPromotionService>();
builder.Services.AddTransient<IStudentPromotionRepository, StudentPromotionRepository>();
builder.Services.AddTransient<IIMEIRegistrationService, IMEIRegistrationService>();
builder.Services.AddTransient<IIMEIRegistrationRepo, IMEIRegistrationRepo>();
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

app.MapControllers();
 
app.Run();
