using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;


namespace MoviesApi.Data.Models;

public class FavoriteMovie
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int FavoriteMovieId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ReleaseDate { get; set; } = string.Empty;
    public string Overview { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public DbSet<Favorites> FavoritesList { get; set; } = null!;
}