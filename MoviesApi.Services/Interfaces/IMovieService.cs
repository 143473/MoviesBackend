

using tmdb_api;

namespace MoviesDb.Services.Interfaces;

public interface IMovieService
{
    Task<MoviesListResponse> GetMovies(int id, string language, int? page);
}