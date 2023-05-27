namespace MoviesApi.Data.Models;

public class FavoriteMovie
{ 
    public int FavoriteMovieId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ReleaseDate { get; set; } = string.Empty;
    public string Overview { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string BackdropPath { get; set; } = string.Empty;
}