namespace MoviesApi.Data.Models;

public class Favorites
{
    public FavoriteMovie FavoriteMovie { get; set; } = null!;
    // setting a composite pk out of these two ids in the onModelCreate in dbcontext
    public int FavoriteMovieId { get; set; }
    public string UserId { get; set; } = null!;
}