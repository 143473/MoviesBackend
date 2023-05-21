using Microsoft.EntityFrameworkCore;
using MoviesApi.Data.Models;

namespace MoviesApi.Data;

public interface IMoviesContext
{
    DbSet<FavoriteMovie> FavoriteMovies { get; set; }
    DbSet<Favorites> Favorites { get; set; }
}