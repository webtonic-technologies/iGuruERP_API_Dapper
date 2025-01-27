using LibraryManagement_API.Repository.Implementations;
using LibraryManagement_API.Repository.Interfaces;
using LibraryManagement_API.Services.Implementations;
using LibraryManagement_API.Services.Interfaces;
using System.Data;
using System.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;

builder.Services.AddTransient<IDbConnection>(c => new SqlConnection(connectionString));
builder.Services.AddTransient<ILibraryRepository, LibraryRepository>();
builder.Services.AddTransient<ILibraryService, LibraryService>();

builder.Services.AddTransient<ICategoryRepository, CategoryRepository>();
builder.Services.AddTransient<ICategoryService, CategoryService>();

builder.Services.AddTransient<IGenreRepository, GenreRepository>();
builder.Services.AddTransient<IGenreService, GenreService>();

builder.Services.AddTransient<ILanguageRepository, LanguageRepository>();
builder.Services.AddTransient<ILanguageService, LanguageService>();

builder.Services.AddTransient<IPublisherRepository, PublisherRepository>();
builder.Services.AddTransient<IPublisherService, PublisherService>();

builder.Services.AddTransient<IAuthorRepository, AuthorRepository>();
builder.Services.AddTransient<IAuthorService, AuthorService>();

builder.Services.AddTransient<ICatalogueRepository, CatalogueRepository>();
builder.Services.AddTransient<ICatalogueService, CatalogueService>();

builder.Services.AddTransient<IIssueRepository, IssueRepository>();
builder.Services.AddTransient<IIssueService, IssueService>();

builder.Services.AddTransient<IReturnRepository, ReturnRepository>();
builder.Services.AddTransient<IReturnService, ReturnService>();

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
