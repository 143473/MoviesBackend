using Microsoft.Extensions.Configuration;
using MoviesDB.API.Swagger.Controllers.Generated;
using MoviesDb.Services.Interfaces;
using tmdb_api;
using MovieResponse = tmdb_api.MovieResponse;


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

    public async Task<MovieResponse> GetMovieAsync(int movie_id, string language = "en_US")
    {
        return await _moviesClient.GetMovieAsync(movie_id, api_key, language);
    }

    public async Task<MoviesResponse> GetMoviesByTitleAsync(string movieName, string language = "en_US")
    {
        return await _moviesClient.GetMoviesByTitleAsync(api_key,movieName, null, language);
    }
}