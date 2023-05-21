using MoviesApi.Data.Models;

namespace MoviesApi.Data.Repositories.Interfaces;

public interface IMoviesRepository
{
    Task<ICollection<FavoriteMovie>> GetFavoritesMovies(string userId);
    Task<ICollection<FavoriteMovie>> GetTopFavoritesMovies();
    Task AddFavoriteMovie(string userId, FavoriteMovie favoriteMovie);
    Task<Rating> GetMovieRatingAsync(int movieId);
}