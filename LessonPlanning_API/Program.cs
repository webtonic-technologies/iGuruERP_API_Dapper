using Lesson_API.Repository.Implementations;
using Lesson_API.Repository.Interfaces;
using Lesson_API.Services.Implementations;
using Lesson_API.Services.Interfaces;
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

builder.Services.AddTransient<ICurriculumRepository, CurriculumRepository>();
builder.Services.AddTransient<ICurriculumService, CurriculumService>();

builder.Services.AddTransient<ILessonPlanningRepository, LessonPlanningRepository>();
builder.Services.AddTransient<ILessonPlanningService, LessonPlanningService>();

builder.Services.AddTransient<IHomeworkRepository, HomeworkRepository>();
builder.Services.AddTransient<IHomeworkService, HomeworkService>();

builder.Services.AddTransient<IAssignmentRepository, AssignmentRepository>();
builder.Services.AddTransient<IAssignmentService, AssignmentService>();

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
