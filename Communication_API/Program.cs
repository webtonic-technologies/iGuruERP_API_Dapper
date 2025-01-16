using Communication_API.Repository.Implementations.Configuration;
using Communication_API.Repository.Implementations.DigitalDiary;
using Communication_API.Repository.Implementations.DiscussionBoard;
using Communication_API.Repository.Implementations.Email;
using Communication_API.Repository.Implementations.NoticeBoard;
using Communication_API.Repository.Implementations.PushNotification;
using Communication_API.Repository.Implementations.SMS;
using Communication_API.Repository.Implementations.Survey;
using Communication_API.Repository.Implementations.WhatsApp;
using Communication_API.Repository.Interfaces.Configuration;
using Communication_API.Repository.Interfaces.DigitalDiary;
using Communication_API.Repository.Interfaces.DiscussionBoard;
using Communication_API.Repository.Interfaces.Email;
using Communication_API.Repository.Interfaces.NoticeBoard;
using Communication_API.Repository.Interfaces.PushNotification;
using Communication_API.Repository.Interfaces.SMS;
using Communication_API.Repository.Interfaces.Survey;
using Communication_API.Repository.Interfaces.WhatsApp;
using Communication_API.Services.Implementations.Configuration;
using Communication_API.Services.Implementations.DigitalDiary;
using Communication_API.Services.Implementations.DiscussionBoard;
using Communication_API.Services.Implementations.Email;
using Communication_API.Services.Implementations.NoticeBoard;
using Communication_API.Services.Implementations.PushNotification;
using Communication_API.Services.Implementations.SMS;
using Communication_API.Services.Implementations.Survey;
using Communication_API.Services.Implementations.WhatsApp;
using Communication_API.Services.Interfaces.Configuration;
using Communication_API.Services.Interfaces.DigitalDiary;
using Communication_API.Services.Interfaces.DiscussionBoard;
using Communication_API.Services.Interfaces.Email;
using Communication_API.Services.Interfaces.NoticeBoard;
using Communication_API.Services.Interfaces.PushNotification;
using Communication_API.Services.Interfaces.SMS;
using Communication_API.Services.Interfaces.Survey;
using Communication_API.Services.Interfaces.WhatsApp;
using System.Data.SqlClient;
using System.Data;
using OfficeOpenXml;

var builder = WebApplication.CreateBuilder(args);
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;

// Set the license context for EPPlus to NonCommercial
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

builder.Services.AddTransient<IDbConnection>(c => new SqlConnection(connectionString));

builder.Services.AddTransient<IGroupRepository, GroupRepository>();
builder.Services.AddTransient<IGroupService, GroupService>();

builder.Services.AddTransient<INotificationRepository, NotificationRepository>();
builder.Services.AddTransient<INotificationService, NotificationService>();

builder.Services.AddTransient<ISMSRepository, SMSRepository>();
builder.Services.AddTransient<ISMSService, SMSService>();

builder.Services.AddTransient<INoticeBoardRepository, NoticeBoardRepository>();
builder.Services.AddTransient<INoticeBoardService, NoticeBoardService>();

builder.Services.AddTransient<IEmailRepository, EmailRepository>();
builder.Services.AddTransient<IEmailService, EmailService>();

builder.Services.AddTransient<ISurveyRepository, SurveyRepository>();
builder.Services.AddTransient<ISurveyService, SurveyService>();

builder.Services.AddTransient<IWhatsAppRepository, WhatsAppRepository>();
builder.Services.AddTransient<IWhatsAppService, WhatsAppService>();

builder.Services.AddTransient<IDiscussionBoardRepository, DiscussionBoardRepository>();
builder.Services.AddTransient<IDiscussionBoardService, DiscussionBoardService>();

builder.Services.AddTransient<IDigitalDiaryRepository, DigitalDiaryRepository>();
builder.Services.AddTransient<IDigitalDiaryService, DigitalDiaryService>();

// Add services to the container.

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

app.MapControllers();

app.Run();
