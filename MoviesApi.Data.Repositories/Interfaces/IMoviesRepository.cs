using MoviesApi.Data.Models;

namespace MoviesApi.Data.Repositories.Interfaces;

public interface IMoviesRepository
{
    Task<ICollection<FavoriteMovie>> GetFavoritesMovies(string userId);
    Task<ICollection<FavoriteMovie>> GetTopFavoritesMovies();
    Task<Rating> GetMovieRatingAsync(int movieId);
    Task AddRatedMovieAsync(RatedMovie movie);
}