using Microsoft.Extensions.Configuration;
using MoviesApi.Services.Interfaces;
using MoviesDB.API.Swagger.Controllers.Generated;
using tmdb_api;
using MovieResponse = tmdb_api.MovieResponse;


namespace MoviesApi.Services;

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

    public async Task<MoviesResponseDto> GetMoviesByTitleAsync(string movieName, string language = "en_US")
    {
        var moviesResponse = await _moviesClient.GetMoviesByTitleAsync(api_key,movieName, null, language);

        return new MoviesResponseDto
        {
            Results = moviesResponse.Results
                .Select(r => new MovieByTitleResponseDto
                {
                    Id = r.Id,
                    Title = r.Title,
                    Description = r.Overview,
                    Poster_path = !string.IsNullOrEmpty(r.Poster_path)
                        ? $"https://image.tmdb.org/t/p/w500{r.Poster_path}"
                        : null
                })
                .ToList()
        };
    }
}