using MoviesDB.API.Swagger.Controllers.Generated;
using Rating = MoviesApi.Data.Models.Rating;

namespace MoviesApi.Services.Interfaces;

public interface IMovieService
{
    Task<MovieResponseDto> GetMovieAsync(string? userId, int movie_id, string language = "en_US");
    Task<MovieListDto> GetMoviesByTitleAsync(string? userId, string movieName, string language = "en_US");
    Task<MovieListDto> GetFavoriteMovies(string userId);
    Task<MovieListDto> GetTopFavorites(string? userId);
    Task AddFavorite(FavoritesDto favoritesDto);
    Task<Rating> GetMovieRatingAsync(int movieId);
    Task<RatedMovieDto> AddRatedMovieAsync(RatedMovieDto ratedMovie);
    Task<RatingDto> AddRatingAsync(RatedMovieDto ratedMovie);
    Task RemoveFavorite(FavoritesDto favoritesDto);
    Task<MoviesExtendedResponseDto> GetFilteredMovies(DateTimeOffset? fromDate, DateTimeOffset? toDate,
        SortBy? sortBy, string language = "en-US", int page = 1);
    Task<MovieCreditsResponseDto> GetMovieCreditsAsync(int movieId);
}
