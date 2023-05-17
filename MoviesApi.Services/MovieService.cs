using Microsoft.Extensions.Configuration;
using MoviesDb.Services.Interfaces;
using tmdb_api;


namespace MoviesDb.Services;

public class MovieService : IMovieService
{
    private readonly IMoviesClient _moviesClient;
    private string api_key;

    public MovieService(IMoviesClient moviesClient, IConfiguration configuration)
    {
        _moviesClient = moviesClient;
        api_key = configuration.GetSection("TMDB")["APIKey"];
    }

    public async Task<MoviesListResponse> GetMovies(int id, string language, int? page)
    {
        return await _moviesClient.GetListOfMoviesAsync(id, api_key, language, page);
    }
}