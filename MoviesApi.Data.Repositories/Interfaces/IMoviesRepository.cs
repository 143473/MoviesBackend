using MoviesApi.Data.Models;

namespace MoviesApi.Data.Repositories.Interfaces;

public interface IMoviesRepository
{
    Task<ICollection<FavoriteMovie>> GetFavoritesMovies(int userId);
    Task<ICollection<FavoriteMovie>> GetTopFavoritesMovies();
}