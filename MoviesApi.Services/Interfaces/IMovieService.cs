

using tmdb_api;

namespace MoviesDb.Services.Interfaces;

public interface IMovieService
{
    Task<MovieResponse> GetMovieAsync(int movie_id, string language = "en_US");
}