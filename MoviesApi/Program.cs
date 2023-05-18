using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Data;
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

//Identity framework
var connectionString = builder.Configuration.GetConnectionString("IdentityDb");
builder.Services.AddDbContext<MoviesContext>(opt => opt.UseSqlServer(connectionString));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<MoviesContext>();

// NOT WORKING in cloud 
builder.Services.AddDataProtection()
    .SetApplicationName("MoviesApp")
    .PersistKeysToFileSystem(new DirectoryInfo(@"c:\temp-keys\"));

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = ".Yummy";
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