

using MoviesDB.API.Swagger.Controllers.Generated;
using tmdb_api;
using MovieResponse = tmdb_api.MovieResponse;

namespace MoviesDb.Services.Interfaces;

public interface IMovieService
{
    Task<MovieResponse> GetMovieAsync(int movie_id, string language = "en_US");
    Task<MoviesResponse> GetMoviesByTitleAsync(string movieName, string language = "en_US");

}