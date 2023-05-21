using MoviesDB.API.Swagger.Controllers.Generated;
using MovieResponse = tmdb_api.MovieResponse;

namespace MoviesApi.Services.Interfaces;

public interface IMovieService
{
    Task<MovieResponse> GetMovieAsync(int movie_id, string language = "en_US");
    Task<MoviesResponseDto> GetMoviesByTitleAsync(string movieName, string language = "en_US");
    Task<ICollection<MovieDto>> GetFavoriteMovies(int userId);
    Task<ICollection<MovieDto>> GetTopFavoriteMovies();
}