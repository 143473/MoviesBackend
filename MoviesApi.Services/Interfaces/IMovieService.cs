using MoviesDB.API.Swagger.Controllers.Generated;
using MovieResponse = tmdb_api.MovieResponse;
using Rating = MoviesApi.Data.Models.Rating;

namespace MoviesApi.Services.Interfaces;

public interface IMovieService
{
    Task<MovieResponse> GetMovieAsync(int movie_id, string language = "en_US");
    Task<MoviesResponseDto> GetMoviesByTitleAsync(string movieName, string language = "en_US");
    Task<MoviesResponseDto> GetFavoriteMovies(string userId);
    Task<MoviesResponseDto> GetTopFavoriteMovies();
    Task AddMovieToFavorite(FavoritesDto favoritesDto);
    Task<Rating> GetMovieRatingAsync(int movieId);
}