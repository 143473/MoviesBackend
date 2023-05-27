using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MoviesApi.Data.Repositories.Interfaces;
using tmdb_api;

namespace MoviesApi.Tests.IntegrationTests;

public class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    public readonly Mock<IMoviesRepository> MoviesRepositoryMock = new();
    public readonly Mock<IMoviesClient> MoviesClientMock = new();
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.Remove(services.Single(d => d.ServiceType == typeof(IMoviesRepository)));
            services.AddSingleton(MoviesRepositoryMock.Object);
            
            services.Remove(services.Single(d => d.ServiceType == typeof(IMoviesClient)));
            services.AddSingleton(MoviesClientMock.Object);
        });

        builder.UseEnvironment("Development");
    }
}