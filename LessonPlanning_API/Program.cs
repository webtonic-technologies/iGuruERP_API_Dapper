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

// Registering repositories and services
builder.Services.AddTransient<ICurriculumRepository, CurriculumRepository>();
builder.Services.AddTransient<ICurriculumService, CurriculumService>();

builder.Services.AddTransient<ILessonPlanningRepository, LessonPlanningRepository>();
builder.Services.AddTransient<ILessonPlanningService, LessonPlanningService>();

builder.Services.AddTransient<IHomeworkRepository, HomeworkRepository>();
builder.Services.AddTransient<IHomeworkService, HomeworkService>();

builder.Services.AddTransient<IAssignmentRepository, AssignmentRepository>();
builder.Services.AddTransient<IAssignmentService, AssignmentService>();

builder.Services.AddScoped<IPlanTypeRepository, PlanTypeRepository>();
builder.Services.AddScoped<IPlanTypeService, PlanTypeService>();

builder.Services.AddScoped<IChapterSubtopicListRepository, ChapterSubtopicListRepository>();
builder.Services.AddScoped<IChapterSubtopicListService, ChapterSubtopicListService>();

builder.Services.AddScoped<ILessonPlanningChapterRepository, LessonPlanningChapterRepository>();
builder.Services.AddScoped<ILessonPlanningChapterService, LessonPlanningChapterService>();

builder.Services.AddScoped<ILessonPlanningSubtopicRepository, LessonPlanningSubtopicRepository>();
builder.Services.AddScoped<ILessonPlanningSubtopicService, LessonPlanningSubtopicService>();

builder.Services.AddScoped<IHomeWorkTypeRepository, HomeWorkTypeRepository>();
builder.Services.AddScoped<IHomeWorkTypeService, HomeWorkTypeService>();

builder.Services.AddScoped<IAssignmentTypeRepository, AssignmentTypeRepository>();
builder.Services.AddScoped<IAssignmentTypeService, AssignmentTypeService>();

// Adding support for hosting environment (e.g., static files)
builder.Services.AddSingleton<IWebHostEnvironment>(builder.Environment);  // Ensure that the hosting environment is properly injected

builder.Services.AddDirectoryBrowser();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Serve static files (this ensures WebRootPath is set)
app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();
