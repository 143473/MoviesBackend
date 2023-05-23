using Microsoft.EntityFrameworkCore;
using MoviesApi.Data;
using MoviesApi.Data.Repositories;
using MoviesApi.Data.Repositories.Interfaces;
using MoviesApi.Services;
using MoviesApi.Services.Interfaces;
using tmdb_api;

var builder = WebApplication.CreateBuilder(args);

// External services
builder.Services.AddScoped<IMoviesClient>( _ => new MoviesClient(){BaseUrl = builder.Configuration.GetConnectionString("tmdb")});

// Internal

// Services
builder.Services.AddScoped<IMovieService, MovieService>();

//Repositories
builder.Services.AddScoped<IMoviesRepository, MoviesRepository>();

// EF
var connectionString = builder.Configuration.GetConnectionString("MoviesDb");
builder.Services.AddDbContext<IMoviesContext, MoviesContext>(opt =>
{
    opt.UseSqlServer(connectionString);
    opt.EnableSensitiveDataLogging();
});

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