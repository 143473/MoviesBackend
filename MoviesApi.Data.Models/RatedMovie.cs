
namespace MoviesApi.Data.Models;

public class RatedMovie
{
    public int RatedMovieId { get; set; }
    public string UserId { get; set; } = null!;
    public double Rating { get; set; }
}