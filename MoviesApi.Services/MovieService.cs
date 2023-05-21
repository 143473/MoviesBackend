using Microsoft.Extensions.Configuration;
using MoviesApi.Data.Models;
using MoviesApi.Data.Repositories.Interfaces;
using MoviesApi.Services.Interfaces;
using MoviesDB.API.Swagger.Controllers.Generated;
using tmdb_api;
using MovieResponse = tmdb_api.MovieResponse;
using Rating = MoviesDB.API.Swagger.Controllers.Generated.Rating;

namespace MoviesApi.Services;

public class MovieService : IMovieService
{
    private readonly IMoviesClient _moviesClient;
    private readonly IMoviesRepository _repository;
    private string api_key;

    public MovieService(IMoviesClient moviesClient, IConfiguration configuration, IMoviesRepository repository)
    {
        _moviesClient = moviesClient;
        _repository = repository;
        api_key = configuration.GetSection("TMDB")["APIKey"];
    }

    public async Task<MovieResponse> GetMovieAsync(int movie_id, string language = "en_US")
    {
        var movieResponse = await _moviesClient.GetMovieAsync(movie_id, api_key, language);
        movieResponse.Poster_path = !string.IsNullOrEmpty(movieResponse.Poster_path)
            ? $"https://image.tmdb.org/t/p/w500{movieResponse.Poster_path}"
            : null;
        return movieResponse;
    }

    public async Task<MoviesResponseDto> GetMoviesByTitleAsync(string movieName, string language = "en_US")
    {
        var moviesResponse = await _moviesClient.GetMoviesByTitleAsync(api_key,movieName, null, language);

        return new MoviesResponseDto
        {
            Results = moviesResponse.Results
                .Select(r => new MovieDto
                {
                    MovieId = r.Id ?? 0,
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
    
    public async Task<MoviesResponseDto> GetFavoriteMovies(string userId)
    {
        return ToMovieDto(await _repository.GetFavoritesMovies(userId));
    }
    
    public async Task<MoviesResponseDto> GetTopFavoriteMovies()
    {
        return ToMovieDto(await _repository.GetTopFavoritesMovies());
    }

    public async Task AddMovieToFavorite(FavoritesDto favoritesDto)
    {
        var movie = await GetMovieAsync(favoritesDto.MovieId);
        var favoriteMovie = new FavoriteMovie
        {
            FavoriteMovieId = movie.Id ?? 0,
            Title = movie.Title,
            Overview = movie.Overview,
            ReleaseDate = movie.Release_date,
            ImageUrl = movie.Poster_path
        };
        
           await _repository.AddFavoriteMovie(favoritesDto.UserId, favoriteMovie);
    }

    private static MoviesResponseDto ToMovieDto(IEnumerable<FavoriteMovie> favoriteMovies)
    {
        return new MoviesResponseDto { Results = favoriteMovies.Select(ToMovieDto).ToList() };
    }
    public async Task<Data.Models.Rating> GetMovieRatingAsync(int movieId)
    {
        return await _repository.GetMovieRatingAsync(movieId);
    }
    private static MovieDto ToMovieDto(FavoriteMovie favoriteMovie)
    {
        return new MovieDto
        {
            MovieId = favoriteMovie.FavoriteMovieId,
            Title = favoriteMovie.Title,
            Description = favoriteMovie.Overview,
            ReleaseDate = favoriteMovie.ReleaseDate,
            PosterPath = favoriteMovie.ImageUrl
        };
    }
}