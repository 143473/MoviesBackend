using Microsoft.EntityFrameworkCore;
using MoviesApi.Data.Models;

namespace MoviesApi.Data;

public class MoviesContext : DbContext
{
    public MoviesContext(DbContextOptions options) : base(options)
    {
        Database.Migrate();
    }

    public DbSet<FavoriteMovie> FavoriteMovies { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FavoriteMovie>()
            .HasOne<Favorites>()
            .WithMany()
            .HasForeignKey(f => f.FavoriteMovieId);
        
        base.OnModelCreating(modelBuilder);
    }
}
