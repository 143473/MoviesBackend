using Microsoft.Extensions.Configuration;
using MoviesApi.Data.Models;
using MoviesApi.Data.Repositories.Interfaces;
using MoviesApi.Services.Interfaces;
using MoviesDB.API.Swagger.Controllers.Generated;
using tmdb_api;
using MovieResponse = tmdb_api.MovieResponse;


namespace MoviesApi.Services;

public class MovieService : IMovieService
{
    private readonly IMoviesClient _moviesClient;
    private string api_key;
    private readonly IMoviesRepository _repository;

    public MovieService(IMoviesClient moviesClient, IConfiguration configuration, IMoviesRepository repository)
    {
        _moviesClient = moviesClient;
        _repository = repository;
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
                .Select(r => new MovieDto
                {
                    Id = r.Id,
                    Title = r.Title,
                    Description = r.Overview,
                    ReleaseDate = r.Release_date,
                    PosterPath = !string.IsNullOrEmpty(r.Poster_path)
                        ? $"https://image.tmdb.org/t/p/w500{r.Poster_path}"
                        : null
                })
                .ToList()
        };
    }
    
    public async Task<ICollection<MovieDto>> GetFavoriteMovies(int userId)
    {
        var favorites = await _repository.GetFavoritesMovies(userId);
        return favorites.Select(ToMovieDto).ToList();
    }
    
    public async Task<ICollection<MovieDto>> GetTopFavoriteMovies()
    {
        var favorites = await _repository.GetTopFavoritesMovies();
        return favorites.Select(ToMovieDto).ToList();
    }

    private static MovieDto ToMovieDto(FavoriteMovie favoriteMovie)
    {
        return new MovieDto
        {
            Id = favoriteMovie.FavoriteMovieId,
            Title = favoriteMovie.Title,
            Description = favoriteMovie.Overview,
            ReleaseDate = favoriteMovie.ReleaseDate,
            PosterPath = favoriteMovie.ImageUrl
        };
    }
}